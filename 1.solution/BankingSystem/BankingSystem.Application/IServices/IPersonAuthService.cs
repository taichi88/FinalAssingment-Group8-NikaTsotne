using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;

namespace BankingSystem.Application.IServices;

public interface IPersonAuthService
{
    Task<AuthenticationResponse> AuthenticationPersonAsync(PersonLoginDto loginDto);
    Task<string> RegisterPersonAsync(PersonRegisterDto registerDto);
}
