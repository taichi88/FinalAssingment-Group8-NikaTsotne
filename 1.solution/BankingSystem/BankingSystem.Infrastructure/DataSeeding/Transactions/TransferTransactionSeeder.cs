using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.IRepository;

namespace BankingSystem.Infrastructure.DataSeeding.Transactions;

public class TransferTransactionSeeder
{
    private readonly IAccountTransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly CurrencyConversionHelper _currencyConverter;

    public TransferTransactionSeeder(
        IAccountTransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        CurrencyConversionHelper currencyConverter)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _currencyConverter = currencyConverter;
    }

    public async Task SeedSameCurrencyTransferAsync(Account fromAccount, Account toAccount)
    {
        var transferAmount = 150m;
        
        var fee = transferAmount * 0.01m + 0.5m;
        var totalDeduction = transferAmount + fee;

        if (fromAccount.Balance >= totalDeduction)
        {
            var transaction = new Transaction
            {
                Amount = transferAmount,
                TransactionFee = fee,
                Currency = fromAccount.Currency,
                TransactionDate = DateTime.Now,
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                IsATM = false,
                TransactionType = TransactionType.TransferToOthers
            };

            await _transactionRepository.AddAccountTransactionAsync(transaction);

            fromAccount.Balance -= totalDeduction;
            toAccount.Balance += transferAmount;

            await _accountRepository.UpdateAccountAsync(fromAccount);
            await _accountRepository.UpdateAccountAsync(toAccount);
        }
    }

    public async Task SeedDifferentCurrencyTransferAsync(Account fromAccount, Account toAccount)
    {
        var transferAmount = 100m;
        
        var fee = transferAmount * 0.01m + 0.5m;
        var totalDeduction = transferAmount + fee;

        if (fromAccount.Balance >= totalDeduction)
        {
            var conversionRates = new Dictionary<string, decimal>
            {
                { CurrencyType.USD.ToString(), 2.5m }, 
                { CurrencyType.EUR.ToString(), 3.0m }, 
                { CurrencyType.GEL.ToString(), 1.0m } 
            };

            decimal convertedAmount = _currencyConverter.ConvertCurrency(
                transferAmount, 
                fromAccount.Currency.ToString(), 
                toAccount.Currency.ToString(), 
                conversionRates);

            var transaction = new Transaction
            {
                Amount = transferAmount,
                TransactionFee = fee,
                Currency = fromAccount.Currency,
                TransactionDate = DateTime.Now,
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                IsATM = false,
                TransactionType = TransactionType.TransferToOthers
            };

            await _transactionRepository.AddAccountTransactionAsync(transaction);

            fromAccount.Balance -= totalDeduction;
            toAccount.Balance += convertedAmount;

            await _accountRepository.UpdateAccountAsync(fromAccount);
            await _accountRepository.UpdateAccountAsync(toAccount);
        }
    }
}
