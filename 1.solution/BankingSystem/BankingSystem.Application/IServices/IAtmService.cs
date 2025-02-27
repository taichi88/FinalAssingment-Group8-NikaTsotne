using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;

namespace BankingSystem.Application.IServices;

public interface IAtmService
{
    Task<ApiResponse> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto);
}