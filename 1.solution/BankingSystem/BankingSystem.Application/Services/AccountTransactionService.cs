using BankingSystem.Application.Constants;
using BankingSystem.Application.DTO;
using BankingSystem.Application.Exceptions;
using BankingSystem.Domain.Entities;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.IExternalApi;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.Services;

public class AccountTransactionService : IAccountTransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExchangeRateApi _exchangeRateApi;
    private readonly TransactionConstants _transactionConstants;
    private readonly ILogger<AccountTransactionService> _logger;

    public AccountTransactionService(
        IUnitOfWork unitOfWork, 
        IExchangeRateApi exchangeRateApi,
        TransactionConstants transactionConstants,
        ILogger<AccountTransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _exchangeRateApi = exchangeRateApi;
        _transactionConstants = transactionConstants;
        _logger = logger;
    }

    public async Task<string> TransactionBetweenAccountsAsync(TransactionDto transactionDto, string userId)
    {
        try
        {
            _logger.LogInformation($"Starting transaction between accounts. From: {transactionDto.FromAccountIban}, To: {transactionDto.ToAccountIban}");
            await _unitOfWork.BeginTransactionAsync();

            var fromAccount = await GetAccountSafelyAsync(transactionDto.FromAccountIban, "Source");
            var toAccount = await GetAccountSafelyAsync(transactionDto.ToAccountIban, "Destination");

            if (fromAccount.PersonId != userId)
                throw new UnauthorizedException("You are not authorized to access this account.");

            var transaction = new Transaction
            {
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                Currency = fromAccount.Currency,
                Amount = transactionDto.Amount,
                TransactionDate = DateTime.Now,
                IsATM = false,
                TransactionType = transactionDto.TransactionType
            };

            switch (transaction.TransactionType)
            {
                case TransactionType.TransferToOthers:
                    _logger.LogInformation("Processing TransferToOthers transaction for user {UserId}", userId);

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
                    _logger.LogInformation("Processing ToMyAccount transaction for user {UserId}", userId);

                    if (fromAccount.PersonId != toAccount.PersonId)
                        throw new ValidationException("Transfer to another user's account is not allowed");

                    if (transaction.Amount > fromAccount.Balance)
                        throw new ValidationException("The transaction was failed. You don't have enough money");

                    fromAccount.Balance -= transaction.Amount;
                    transaction.TransactionFee = _transactionConstants.ToMyAccountFee;
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
            _logger.LogInformation("Transaction completed successfully. From: {FromAccountId}, To: {ToAccountId}, Amount: {Amount}",
                transaction.FromAccountId, transaction.ToAccountId, transaction.Amount);
            
            
            return "The transaction was completed successfully.";
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task<Account> GetAccountSafelyAsync(string iban, string accountType)
    {
        try
        {
            _logger.LogInformation("Retrieving {AccountType} account with ID: {AccountId}", accountType, iban);

            return await _unitOfWork.AccountRepository.GetAccountByIbanAsync(iban);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException($"{accountType} account {iban} not found");
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
