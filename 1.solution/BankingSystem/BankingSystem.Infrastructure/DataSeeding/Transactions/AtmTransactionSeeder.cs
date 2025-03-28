using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;

namespace BankingSystem.Infrastructure.DataSeeding.Transactions;

public class AtmTransactionSeeder
{
    private readonly IAccountTransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public AtmTransactionSeeder(
        IAccountTransactionRepository transactionRepository,
        IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task SeedAtmWithdrawalAsync(Account account)
    {
        var withdrawalAmount = 200m;
        
        var fee = withdrawalAmount * 0.02m;
        var totalAmount = withdrawalAmount + fee;

        if (account.Balance >= totalAmount)
        {
            var atmTransaction = new Transaction
            {
                Amount = withdrawalAmount,
                TransactionFee = fee,
                Currency = account.Currency,
                TransactionDate = DateTime.Now,
                FromAccountId = account.AccountId,
                ToAccountId = account.AccountId, 
                TransactionType = null 
            };

            await _transactionRepository.AddAccountTransactionAsync(atmTransaction);

            account.Balance -= totalAmount;
            await _accountRepository.UpdateAccountAsync(account);
        }
    }
}
