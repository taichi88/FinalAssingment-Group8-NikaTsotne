// BankingSystem.Application/IServices/IAtmTokenService.cs
using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;

namespace BankingSystem.Application.IServices;

public interface IAtmTokenService
{
    Task<AtmAuthenticationResponse> GenerateAtmTokenAsync(string cardNumber);
}