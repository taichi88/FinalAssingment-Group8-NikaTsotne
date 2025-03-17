using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Controllers;
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
    public async Task<ActionResult<ApiResponse>> Authorize(CardAuthorizationDto cardAuthorizationDto)
    {
        _logger.LogInformation("Received request to authorize card {CardNumber}", cardAuthorizationDto.CardNumber);
        var response = await _atmService.AuthorizeCardAsync(cardAuthorizationDto);
        if (response.IsSuccess)
        {
            HttpContext.Session.SetString("AuthorizedCard", cardAuthorizationDto.CardNumber);
            return Ok(response);
        }
        _logger.LogWarning("Authorization failed for card {CardNumber}", cardAuthorizationDto.CardNumber);
        return BadRequest(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpGet("view-balance")]
    public async Task<ActionResult<ApiResponse>> ViewBalance()
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        _logger.LogInformation("Received request to view balance for card {CardNumber}", cardNumber);
        var response = await _atmService.ViewBalanceAsync(cardNumber);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        _logger.LogWarning("Failed to view balance for card {CardNumber}", cardNumber);
        return BadRequest(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpPost("withdraw-money")]
    public async Task<ActionResult<ApiResponse>> WithdrawMoney(WithdrawMoneyDto withdrawMoneyDto)
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        _logger.LogInformation("Received request to withdraw money for card {CardNumber}", cardNumber);
        var response = await _atmService.WithdrawMoneyAsync(cardNumber, withdrawMoneyDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        _logger.LogWarning("Failed to withdraw money for card {CardNumber}", cardNumber);
        return BadRequest(response);
    }

    [ServiceFilter(typeof(CardAuthorizationFilter))]
    [HttpPost("change-pin")]
    public async Task<ActionResult<ApiResponse>> ChangePinCode(ChangePinCodeDto changePinCodeDto)
    {
        var cardNumber = HttpContext.Session.GetString("AuthorizedCard");
        _logger.LogInformation("Received request to change PIN code for card {CardNumber}", cardNumber);
        var response = await _atmService.ChangePinCodeAsync(cardNumber, changePinCodeDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        _logger.LogWarning("Failed to change PIN code for card {CardNumber}", cardNumber);
        return BadRequest(response);
    }
}
