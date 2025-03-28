using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using BankingSystem.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankingSystem.Controllers;

[ValidateModel]
[ApiController]
[Authorize]
[AtmCardAuthorization]
[Route("api/[controller]")]
public class AtmController : ControllerBase
{
    private readonly IAtmService _atmService;
    private readonly IAtmTokenService _atmTokenService;
    public AtmController(
        IAtmService atmService,
        IAtmTokenService atmTokenService)
    {
        _atmService = atmService;
        _atmTokenService = atmTokenService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authorize(CardAuthorizationDto cardAuthorizationDto)
    {
        await _atmService.AuthorizeCardAsync(cardAuthorizationDto);
        var authResponse = await _atmTokenService.GenerateAtmTokenAsync(cardAuthorizationDto.CardNumber);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, authResponse));
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var cardNumber = User.FindFirst("cardNumber").Value;
        var result = await _atmService.ViewBalanceAsync(cardNumber);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> WithdrawMoney(WithdrawMoneyDto withdrawMoneyDto)
    {
        var cardNumber = User.FindFirst("cardNumber").Value;
        var result = await _atmService.WithdrawMoneyAsync(cardNumber, withdrawMoneyDto);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }

    [HttpPut("pin")]
    public async Task<IActionResult> UpdatePin(ChangePinCodeDto changePinCodeDto)
    {
        var cardNumber = User.FindFirst("cardNumber").Value;
        var result = await _atmService.ChangePinCodeAsync(cardNumber, changePinCodeDto);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }
}
