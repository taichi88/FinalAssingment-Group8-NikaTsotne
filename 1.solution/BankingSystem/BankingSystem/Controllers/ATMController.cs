using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace InternetBank.UI.Controllers;
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
    public async Task<ActionResult<ApiResponse>> ViewBalance([FromQuery] string cardNumber)
    {
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
        var response = await _atmService.WithdrawMoneyAsync(withdrawMoneyDto);
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
        var response = await _atmService.ChangePinCodeAsync(changePinCodeDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}