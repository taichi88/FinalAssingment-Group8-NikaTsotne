using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IUnitOfWork;

namespace BankingSystem.Application.Services;

public class PersonService(IUnitOfWork unitOfWork) : IPersonService
{
    public async Task<Person?> GetPersonById(string id)
    {
        return await unitOfWork.PersonRepository.GetUserByIdAsync(id);
    }
}
