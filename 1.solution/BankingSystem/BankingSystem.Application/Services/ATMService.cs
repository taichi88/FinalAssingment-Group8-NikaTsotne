using System.Net;
using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.IUnitOfWork;

namespace BankingSystem.Application.Services;

public class AtmService : IAtmService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiResponse _response;


    public AtmService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _response = new();
    }

    public async Task<ApiResponse> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto)
    {
        try
        {
            var authorized = await _unitOfWork.CardRepository.ValidateCardAsync(cardAuthorizationDto.CardNumber,
                cardAuthorizationDto.PinCode);
            if (!authorized)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Card number or pin code is invalid.");
                return _response;
            }

            _response.StatusCode = HttpStatusCode.NoContent;
            return _response;
        }
        catch (Exception ex)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add(ex.Message);
            return _response;
        }
    }
    public async Task<ApiResponse> ViewBalanceAsync(string cardNumber)
    {
        var response = new ApiResponse();
        try
        {
            var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);
            if (account == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { "Card not found" };
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = new { Balance = account.Balance };
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ApiResponse> WithdrawMoneyAsync(WithdrawMoneyDto withdrawMoneyDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> ChangePinCodeAsync(ChangePinCodeDto changePinCodeDto)
    {
        var response = new ApiResponse();
        try
        {
            var card = await _unitOfWork.CardRepository.GetCardByNumberAsync(changePinCodeDto.CardNumber);
            if (card == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { "Card not found" };
            }
            else if (card.PinCode != changePinCodeDto.OldPinCode)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { "Old PIN code is incorrect" };
            }
            else
            {
                card.PinCode = changePinCodeDto.NewPinCode;
                await _unitOfWork.CardRepository.UpdateCardAsync(card);
                await _unitOfWork.CommitAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }
}
