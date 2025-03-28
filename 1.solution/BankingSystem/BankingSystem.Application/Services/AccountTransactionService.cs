using BankingSystem.Application.Constants;
using BankingSystem.Application.DTO;
using BankingSystem.Application.Exceptions;
using BankingSystem.Domain.Entities;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.IExternalApi;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.Services;

public class AccountTransactionService : IAccountTransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExchangeRateApi _exchangeRateApi;
    private readonly TransactionConstants _transactionConstants;

    public AccountTransactionService(
        IUnitOfWork unitOfWork, 
        IExchangeRateApi exchangeRateApi,
        TransactionConstants transactionConstants)
    {
        _unitOfWork = unitOfWork;
        _exchangeRateApi = exchangeRateApi;
        _transactionConstants = transactionConstants;
    }

    public async Task<string> TransactionBetweenAccountsAsync(TransactionDto transactionDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var fromAccount = await GetAccountSafelyAsync(transactionDto.FromAccountId, "Source");
            var toAccount = await GetAccountSafelyAsync(transactionDto.ToAccountId, "Destination");

            if (fromAccount.PersonId != userId)
                throw new UnauthorizedException("You are not authorized to perform this transaction");

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

            switch (transaction.TransactionType)
            {
                case TransactionType.TransferToOthers:
                    if (fromAccount.PersonId == toAccount.PersonId)
                        throw new ValidationException("Transfer to your own account is not allowed");

                    var transactionFee = transaction.Amount * _transactionConstants.TransferToOthersFeeRate + 
                                        _transactionConstants.TransferToOthersBaseFee;
                    
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

            await _unitOfWork.AccountRepository.UpdateAccountAsync(fromAccount);
            await _unitOfWork.AccountRepository.UpdateAccountAsync(toAccount);
            await _unitOfWork.TransactionRepository.AddAccountTransactionAsync(transaction);

            await _unitOfWork.CommitAsync();
            return "The transaction was completed successfully.";
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task<Account> GetAccountSafelyAsync(int accountId, string accountType)
    {
        try
        {
            return await _unitOfWork.AccountRepository.GetAccountByIdAsync(accountId);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException($"{accountType} account {accountId} not found");
        }
    }

    private async Task<decimal> ConvertCurrencyAsync(decimal amount, CurrencyType fromCurrency, CurrencyType toCurrency)
    {
        if (fromCurrency == toCurrency)
            return amount;

        string baseCurrency = _transactionConstants.BaseCurrency;
        string fromCurrencyStr = fromCurrency.ToString();
        string toCurrencyStr = toCurrency.ToString();

        var rates = new System.Collections.Concurrent.ConcurrentDictionary<string, decimal>();
        
        if (fromCurrencyStr != baseCurrency)
        {
            var fromRate = await _exchangeRateApi.GetExchangeRate(fromCurrencyStr);
            rates.TryAdd(fromCurrencyStr, fromRate);
        }
        
        if (toCurrencyStr != baseCurrency)
        {
            var toRate = await _exchangeRateApi.GetExchangeRate(toCurrencyStr);
            rates.TryAdd(toCurrencyStr, toRate);
        }

        if (fromCurrencyStr == baseCurrency)
            return amount / rates[toCurrencyStr];

        if (toCurrencyStr == baseCurrency)
            return amount * rates[fromCurrencyStr];

        return amount * (rates[fromCurrencyStr] / rates[toCurrencyStr]);
    }
}
