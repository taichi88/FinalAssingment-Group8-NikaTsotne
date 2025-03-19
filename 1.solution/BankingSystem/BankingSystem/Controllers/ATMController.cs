using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using BankingSystem.Middleware;
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
    private readonly ILogger<AtmController> _logger;

    public AtmController(IAtmService atmService, ILogger<AtmController> logger)
    {
        _atmService = atmService;
        _logger = logger;
    }

    [HttpPost("authorize-card")]
    public async Task<IActionResult> Authorize(CardAuthorizationDto cardAuthorizationDto)
    {
        var result = await _atmService.AuthorizeCardAsync(cardAuthorizationDto);
        HttpContext.Session.SetString("AuthorizedCard", cardAuthorizationDto.CardNumber);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpGet("view-balance")]
    public async Task<IActionResult> ViewBalance()
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        var result = await _atmService.ViewBalanceAsync(cardNumber);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpPost("withdraw-money")]
    public async Task<IActionResult> WithdrawMoney(WithdrawMoneyDto withdrawMoneyDto)
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        var result = await _atmService.WithdrawMoneyAsync(cardNumber, withdrawMoneyDto);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpPost("change-pin")]
    public async Task<IActionResult> ChangePinCode(ChangePinCodeDto changePinCodeDto)
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        var result = await _atmService.ChangePinCodeAsync(cardNumber, changePinCodeDto);

        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }
}
