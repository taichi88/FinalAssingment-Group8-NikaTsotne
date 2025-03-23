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
        
        // Calculate fee as per AccountTransactionService (1% of amount + 0.5 flat fee)
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

            // Update account balances
            fromAccount.Balance -= totalDeduction;
            toAccount.Balance += transferAmount; // Recipient gets full amount without fee

            await _accountRepository.UpdateAccountAsync(fromAccount);
            await _accountRepository.UpdateAccountAsync(toAccount);
        }
    }

    public async Task SeedDifferentCurrencyTransferAsync(Account fromAccount, Account toAccount)
    {
        var transferAmount = 100m; // USD
        
        // Calculate fee as per AccountTransactionService (1% of amount + 0.5 flat fee)
        var fee = transferAmount * 0.01m + 0.5m;
        var totalDeduction = transferAmount + fee;

        if (fromAccount.Balance >= totalDeduction)
        {
            // Simple conversion rates for demonstration - in real app this would come from exchange rate API
            var conversionRates = new Dictionary<string, decimal>
            {
                { "USD", 2.5m }, // USD to GEL
                { "EUR", 3.0m }, // EUR to GEL
                { "GEL", 1.0m }  // GEL is base currency
            };

            // Calculate converted amount using same logic as in AccountTransactionService
            decimal convertedAmount = _currencyConverter.ConvertCurrency(
                transferAmount, 
                fromAccount.Currency, 
                toAccount.Currency, 
                conversionRates);

            var transaction = new Transaction
            {
                Amount = transferAmount, // Original amount in source currency
                TransactionFee = fee,
                Currency = fromAccount.Currency,
                TransactionDate = DateTime.Now,
                FromAccountId = fromAccount.AccountId,
                ToAccountId = toAccount.AccountId,
                IsATM = false,
                TransactionType = TransactionType.TransferToOthers
            };

            await _transactionRepository.AddAccountTransactionAsync(transaction);

            // Update account balances with currency conversion
            fromAccount.Balance -= totalDeduction;
            toAccount.Balance += convertedAmount;

            await _accountRepository.UpdateAccountAsync(fromAccount);
            await _accountRepository.UpdateAccountAsync(toAccount);
        }
    }
}
