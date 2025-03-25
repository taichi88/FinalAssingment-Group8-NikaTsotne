// BankingSystem/Controllers/ATMController.cs
using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using BankingSystem.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BankingSystem.Controllers;

[ValidateModel]
[ApiController]
[Route("api/[controller]")]
public class AtmController : ControllerBase
{
    private readonly IAtmService _atmService;
    private readonly IAtmTokenService _atmTokenService;
    private readonly ILogger<AtmController> _logger;

    public AtmController(
        IAtmService atmService,
        IAtmTokenService atmTokenService,
        ILogger<AtmController> logger)
    {
        _atmService = atmService;
        _atmTokenService = atmTokenService;
        _logger = logger;
    }

    [HttpPost("authorize-card")]
    public async Task<IActionResult> Authorize(CardAuthorizationDto cardAuthorizationDto)
    {
        // First validate the card credentials
        await _atmService.AuthorizeCardAsync(cardAuthorizationDto);

        // If validation is successful, generate a token
        var authResponse = await _atmTokenService.GenerateAtmTokenAsync(cardAuthorizationDto.CardNumber);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, authResponse);
        return Ok(response);
    }

    [Authorize]
    [AtmCardAuthorization]
    [HttpGet("view-balance")]
    public async Task<IActionResult> ViewBalance()
    {
        var cardNumber = User.FindFirst("cardNumber")?.Value;
        if (string.IsNullOrEmpty(cardNumber))
        {
            return Unauthorized();
        }

        var result = await _atmService.ViewBalanceAsync(cardNumber);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [Authorize]
    [AtmCardAuthorization]
    [HttpPost("withdraw-money")]
    public async Task<IActionResult> WithdrawMoney(WithdrawMoneyDto withdrawMoneyDto)
    {
        var cardNumber = User.FindFirst("cardNumber")?.Value;
        if (string.IsNullOrEmpty(cardNumber))
        {
            return Unauthorized();
        }

        var result = await _atmService.WithdrawMoneyAsync(cardNumber, withdrawMoneyDto);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [Authorize]
    [AtmCardAuthorization]
    [HttpPost("change-pin")]
    public async Task<IActionResult> ChangePinCode(ChangePinCodeDto changePinCodeDto)
    {
        var cardNumber = User.FindFirst("cardNumber")?.Value;
        if (string.IsNullOrEmpty(cardNumber))
        {
            return Unauthorized();
        }

        var result = await _atmService.ChangePinCodeAsync(cardNumber, changePinCodeDto);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }
}
