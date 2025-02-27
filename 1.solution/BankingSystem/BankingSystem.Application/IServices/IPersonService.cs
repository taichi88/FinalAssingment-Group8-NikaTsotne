using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.IServices;

public interface IPersonService
{
    Task<Person?> GetPersonById(string id);
}