using BankingSystem.Application.DTO;

namespace BankingSystem.Application.IServices;

public interface IAccountService
{
    Task<string> CreateAccountAsync(AccountRegisterDto accountRegisterDto);
}