using BankingSystem.Domain.Entities;
using System.Data;

namespace BankingSystem.Domain.IRepository;

public interface IAccountTransactionRepository : ITransaction
{
    Task AddAccountTransactionAsync(AccountTransaction accountTransaction);
}