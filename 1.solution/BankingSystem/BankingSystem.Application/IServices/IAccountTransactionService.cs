using BankingSystem.Application.DTO;

namespace BankingSystem.Application.IServices;

public interface IAccountTransactionService
{
    Task<string> TransactionBetweenAccountsAsync(TransactionDto transactionDto, string userId);
}