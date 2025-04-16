using BankingSystem.Application.DTO.Response;

namespace BankingSystem.Application.IServices;

public interface IAtmTokenService
{
    Task<AtmAuthenticationResponse> GenerateAtmTokenAsync(string cardNumber);
}