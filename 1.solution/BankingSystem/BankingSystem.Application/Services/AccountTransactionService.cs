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

            // Get accounts
            var fromAccount = await unitOfWork.AccountRepository.GetAccountByIdAsync(transactionDto.FromAccountId);
            var toAccount = await unitOfWork.AccountRepository.GetAccountByIdAsync(transactionDto.ToAccountId);

            // Authorization check
            if (fromAccount.PersonId != userId)
                throw new UnauthorizedException("You are not authorized to perform this transaction");

            // Validate transaction type
            if (!Enum.TryParse(transactionDto.TransactionType, out TransactionType transactionType))
                throw new ValidationException("Invalid transaction type");

            // Create transaction
            var transaction = new Transaction
            {
                FromAccountId = transactionDto.FromAccountId,
                ToAccountId = transactionDto.ToAccountId,
                Currency = fromAccount.Currency,
                Amount = transactionDto.Amount,
                TransactionDate = DateTime.Now,
                IsATM = false,
                TransactionType = transactionType
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
                    transaction.Amount = await ConvertCurrencyAsync(transaction.Amount, fromAccount.Currency, toAccount.Currency);
                    toAccount.Balance += transaction.Amount;
                    break;

                case TransactionType.ToMyAccount:
                    if (fromAccount.PersonId != toAccount.PersonId)
                        throw new ValidationException("Transfer to another user's account is not allowed");

                    if (transaction.Amount > fromAccount.Balance)
                        throw new ValidationException("The transaction was failed. You don't have enough money");

                    fromAccount.Balance -= transaction.Amount;
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

    private async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return amount;

        var rates = new Dictionary<string, decimal>
        {
            { "USD", await exchangeRateApi.GetExchangeRate("USD") },
            { "EUR", await exchangeRateApi.GetExchangeRate("EUR") }
        };

        return fromCurrency switch
        {
            "GEL" when rates.ContainsKey(toCurrency) => amount / rates[toCurrency],
            "USD" when toCurrency == "GEL" => amount * rates["USD"],
            "EUR" when toCurrency == "GEL" => amount * rates["EUR"],
            "USD" when toCurrency == "EUR" => amount * (rates["EUR"] / rates["USD"]),
            "EUR" when toCurrency == "USD" => amount * (rates["USD"] / rates["EUR"]),
            _ => amount
        };
    }
}
