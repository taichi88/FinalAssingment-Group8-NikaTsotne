using BankingSystem.Application.DTO;

namespace BankingSystem.Application.IServices;

public interface ICardService
{
    Task<bool> CreateCardAsync(CardRegisterDto CardRegisterDto);
}