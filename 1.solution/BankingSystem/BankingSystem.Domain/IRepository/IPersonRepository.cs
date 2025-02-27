using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.IRepository;

public interface IPersonRepository : ITransaction
{
    Task<Person?> GetUserByUsernameAsync(string username);
    Task<Person?> GetUserByIdAsync(string id);
}