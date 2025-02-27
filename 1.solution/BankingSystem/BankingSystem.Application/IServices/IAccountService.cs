using BankingSystem.Application.DTO;

namespace BankingSystem.Application.IServices;

public interface IAccountService
{
    Task<bool> CreateAccountAsync(AccountRegisterDto AccountRegisterDto);
}