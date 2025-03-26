using BankingSystem.Application.DTO;
using BankingSystem.Application.Exceptions;
using BankingSystem.Domain.Entities;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.ExternalApiContracts;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Services;

public class AccountTransactionService(IUnitOfWork unitOfWork, IExchangeRateApi exchangeRateApi) : IAccountTransactionService
{
    public async Task<string> TransactionBetweenAccountsAsync(TransactionDto transactionDto, string userId)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var fromAccount = await GetAccountSafelyAsync(transactionDto.FromAccountId, "Source");
            var toAccount = await GetAccountSafelyAsync(transactionDto.ToAccountId, "Destination");

            // Authorization check
            if (fromAccount.PersonId != userId)
                throw new UnauthorizedException("You are not authorized to perform this transaction");

            // Create transaction
            var transaction = new Transaction
            {
                FromAccountId = transactionDto.FromAccountId,
                ToAccountId = transactionDto.ToAccountId,
                Currency = fromAccount.Currency,
                Amount = transactionDto.Amount,
                TransactionDate = DateTime.Now,
                IsATM = false,
                TransactionType = Enum.Parse<TransactionType>(transactionDto.TransactionType)
            };

            // Process based on transaction type
            switch (transaction.TransactionType)
            {
                case TransactionType.TransferToOthers:
                    if (fromAccount.PersonId == toAccount.PersonId)
                        throw new ValidationException("Transfer to your own account is not allowed");

                    var transactionFee = transaction.Amount * 0.01m + 0.5m;
                    if ((transaction.Amount + transactionFee) > fromAccount.Balance)
                        throw new ValidationException("The transaction was failed. You don't have enough money");

                    fromAccount.Balance -= transaction.Amount + transactionFee;
                    transaction.TransactionFee = transactionFee;
                    transaction.Amount = await ConvertCurrencyAsync(transaction.Amount, fromAccount.Currency, toAccount.Currency);
                    toAccount.Balance += transaction.Amount;
                    break;

                case TransactionType.ToMyAccount:
                    if (fromAccount.PersonId != toAccount.PersonId)
                        throw new ValidationException("Transfer to another user's account is not allowed");

                    if (transaction.Amount > fromAccount.Balance)
                        throw new ValidationException("The transaction was failed. You don't have enough money");

                    fromAccount.Balance -= transaction.Amount;
                    transaction.TransactionFee = 0; // No fee for this transaction type
                    transaction.Amount = await ConvertCurrencyAsync(transaction.Amount, fromAccount.Currency, toAccount.Currency);
                    toAccount.Balance += transaction.Amount;
                    break;

                default:
                    throw new ValidationException("Invalid transaction type");
            }

            // Save changes
            await unitOfWork.AccountRepository.UpdateAccountAsync(fromAccount);
            await unitOfWork.AccountRepository.UpdateAccountAsync(toAccount);
            await unitOfWork.TransactionRepository.AddAccountTransactionAsync(transaction);

            // Commit transaction
            await unitOfWork.CommitAsync();
            return "The transaction was completed successfully.";
        }
        catch (Exception)
        {
            // Single point of rollback
            await unitOfWork.RollbackAsync();
            throw; // Re-throw to be handled by middleware
        }
    }


    private async Task<Account> GetAccountSafelyAsync(int accountId, string accountType)
    {
        try
        {
            return await unitOfWork.AccountRepository.GetAccountByIdAsync(accountId);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException($"{accountType} account {accountId} not found");
        }
    }

    private async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        // No conversion needed if currencies are the same
        if (fromCurrency == toCurrency)
            return amount;

        // Base currency for the system is GEL
        const string baseCurrency = "GEL";

        // Get exchange rates using a thread-safe ConcurrentDictionary
        var rates = new System.Collections.Concurrent.ConcurrentDictionary<string, decimal>();
        
        if (fromCurrency != baseCurrency)
        {
            var fromRate = await exchangeRateApi.GetExchangeRate(fromCurrency);
            rates.TryAdd(fromCurrency, fromRate);
        }
        
        if (toCurrency != baseCurrency)
        {
            var toRate = await exchangeRateApi.GetExchangeRate(toCurrency);
            rates.TryAdd(toCurrency, toRate);
        }

        // Conversion logic
        // 1. Converting from base currency to another
        if (fromCurrency == baseCurrency)
            return amount / rates[toCurrency];

        // 2. Converting to base currency
        if (toCurrency == baseCurrency)
            return amount * rates[fromCurrency];

        // 3. Cross-currency conversion (through base currency)
        return amount * (rates[fromCurrency] / rates[toCurrency]);
    }


}
