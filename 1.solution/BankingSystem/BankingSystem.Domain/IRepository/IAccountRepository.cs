using BankingSystem.Domain.Entities;
using System.Data;

namespace BankingSystem.Domain.IRepository;

public interface IAccountRepository : ITransaction
{
    Task CreateAccountAsync(Account account);

    Task UpdateAccountAsync(Account account);

    Task<Account> GetAccountByIdAsync(int id);
}
