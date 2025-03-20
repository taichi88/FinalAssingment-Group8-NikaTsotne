using BankingSystem.Application.DTO;

namespace BankingSystem.Application.IServices;

public interface IAtmService
{
    Task<string> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto);
    Task<string> ViewBalanceAsync(string cardNumber);
    Task<string> WithdrawMoneyAsync(string cardNumber, WithdrawMoneyDto withdrawMoneyDto);
    Task<string> ChangePinCodeAsync(string cardNumber, ChangePinCodeDto changePinCodeDto);
}