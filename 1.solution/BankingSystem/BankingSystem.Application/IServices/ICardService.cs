using BankingSystem.Application.DTO;

namespace BankingSystem.Application.IServices;

public interface ICardService
{
    Task<string> CreateCardAsync(CardRegisterDto cardRegisterDto);
}