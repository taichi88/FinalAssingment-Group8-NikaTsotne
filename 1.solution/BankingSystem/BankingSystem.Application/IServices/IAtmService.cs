using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;

namespace BankingSystem.Application.IServices;

public interface IAtmService
{
    Task<ApiResponse> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto);
    Task<ApiResponse> ViewBalanceAsync(string cardNumber);
    Task<ApiResponse> WithdrawMoneyAsync(string cardNumber, WithdrawMoneyDto withdrawMoneyDto);
    Task<ApiResponse> ChangePinCodeAsync(string cardNumber, ChangePinCodeDto changePinCodeDto);
}
