// BankingSystem.Application/IServices/ICardService.cs

using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;


namespace BankingSystem.Application.IServices;

public interface ICardService
{
    Task<CardResponseDto> CreateCardAsync(CardRegisterDto cardRegisterDto);
}