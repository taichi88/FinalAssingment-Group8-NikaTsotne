using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;
using BankingSystem.Infrastructure.DataSeeding.Transactions;

namespace BankingSystem.Infrastructure.DataSeeding;

public class TransactionSeeder
{
    private readonly AtmTransactionSeeder _atmTransactionSeeder;
    private readonly TransferTransactionSeeder _transferTransactionSeeder;

    public TransactionSeeder(
        IAccountTransactionRepository transactionRepository,
        IAccountRepository accountRepository)
    {
        _atmTransactionSeeder = new AtmTransactionSeeder(transactionRepository, accountRepository);
        _transferTransactionSeeder = new TransferTransactionSeeder(
            transactionRepository, 
            accountRepository, 
            new CurrencyConversionHelper());
    }

    public async Task SeedSampleTransactionsAsync(List<Account> accounts)
    {
        // Ensure we have enough accounts for all scenarios
        if (accounts.Count < 5) return;

        // 1. ATM Withdrawal
        await _atmTransactionSeeder.SeedAtmWithdrawalAsync(accounts[0]);

        // 2. Transfer to Others (Same Currency)
        var fromAccount = accounts[2];
        var toAccount = accounts.FirstOrDefault(a => 
            a.PersonId != fromAccount.PersonId && 
            a.Currency == fromAccount.Currency);
        
        if (toAccount != null)
        {
            await _transferTransactionSeeder.SeedSameCurrencyTransferAsync(fromAccount, toAccount);
        }

        // 3. Transfer to Others (Different Currency)
        var fromAccountDiff = accounts.FirstOrDefault(a => a.Currency == "USD");
        var toAccountDiff = accounts.FirstOrDefault(a => 
            a.Currency == "EUR" && 
            a.PersonId != (fromAccountDiff?.PersonId ?? ""));

        if (fromAccountDiff != null && toAccountDiff != null)
        {
            await _transferTransactionSeeder.SeedDifferentCurrencyTransferAsync(fromAccountDiff, toAccountDiff);
        }
    }
}
