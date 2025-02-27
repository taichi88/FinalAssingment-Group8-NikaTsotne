using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Identity;

namespace BankingSystem.Application.IServices;

public interface IPersonAuthService
{
    Task<AuthenticationResponse> AuthenticationPersonAsync(PersonLoginDto loginDto);
    Task<bool> RegisterPersonAsync(PersonRegisterDto registerDto);
    public Task<AuthenticationResponse> GenerateJwtToken(IdentityPerson user);
}
