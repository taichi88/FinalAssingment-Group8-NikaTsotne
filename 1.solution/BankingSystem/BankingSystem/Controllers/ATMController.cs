using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AtmController : ControllerBase
{
    private readonly IAtmService _atmService;

    public AtmController(IAtmService atmService)
    {
        _atmService = atmService;
    }

    [HttpPost("authorize-card")]
    public async Task<ActionResult<ApiResponse>> Authorize(CardAuthorizationDto cardAuthorizationDto)
    {
        var response = await _atmService.AuthorizeCardAsync(cardAuthorizationDto);
        if (response.IsSuccess)
        {
            // Store the authorized card number in session
            HttpContext.Session.SetString("AuthorizedCard", cardAuthorizationDto.CardNumber);
            return Ok(response);
        }
        return BadRequest(response);
    }


    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpGet("view-balance")]
    public async Task<ActionResult<ApiResponse>> ViewBalance()
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        var response = await _atmService.ViewBalanceAsync(cardNumber);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpPost("withdraw-money")]
    public async Task<ActionResult<ApiResponse>> WithdrawMoney(WithdrawMoneyDto withdrawMoneyDto)
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        var response = await _atmService.WithdrawMoneyAsync(cardNumber, withdrawMoneyDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpPost("change-pin")]
    public async Task<ActionResult<ApiResponse>> ChangePinCode(ChangePinCodeDto changePinCodeDto)
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        var response = await _atmService.ChangePinCodeAsync(cardNumber, changePinCodeDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

}