using System.Data;

namespace BankingSystem.Domain.IRepository;

public interface ITransaction
{
    void SetTransaction(IDbTransaction transaction);
}