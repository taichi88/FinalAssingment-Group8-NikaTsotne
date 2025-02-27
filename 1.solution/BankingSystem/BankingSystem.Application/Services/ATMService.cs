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
}