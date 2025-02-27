using BankingSystem.Application.DTO;
using BankingSystem.Domain.Entities;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.IUnitOfWork;

namespace BankingSystem.Application.Services;

public class AccountService(IUnitOfWork unitOfWork) : IAccountService
{
    public async Task<bool> CreateAccountAsync(AccountRegisterDto AccountRegisterDto)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var person = await unitOfWork.PersonRepository.GetUserByUsernameAsync(AccountRegisterDto.Username);

            var Account = new Account
            {
                IBAN = AccountRegisterDto.Iban,
                Balance = AccountRegisterDto.Balance,
                PersonId = person!.PersonId,
                Currency = AccountRegisterDto.Currency
            };

            await unitOfWork.AccountRepository.CreateAccountAsync(Account);

            await unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}