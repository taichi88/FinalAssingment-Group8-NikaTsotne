using BankingSystem.Application.DTO;

public interface IAtmService
{
    Task<object?> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto);
    Task<object> ViewBalanceAsync(string cardNumber);
    Task<object> WithdrawMoneyAsync(string cardNumber, WithdrawMoneyDto withdrawMoneyDto);
    Task<object?> ChangePinCodeAsync(string cardNumber, ChangePinCodeDto changePinCodeDto);
}