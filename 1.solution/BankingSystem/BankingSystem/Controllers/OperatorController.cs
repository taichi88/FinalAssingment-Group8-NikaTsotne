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
[Route("api/[controller]")]
public class OperatorController : ControllerBase
{
    private readonly IPersonAuthService _authService;
    private readonly IAccountService _accountService;
    private readonly ICardService _cardService;
    public OperatorController(IPersonAuthService authService, IAccountService accountService, ICardService cardService)
    {
        _authService = authService;
        _accountService = accountService;
        _cardService = cardService;
    }

    [Authorize(Roles = "Operator")]
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(PersonRegisterDto registerModel)
    {
        var result = await _authService.RegisterPersonAsync(registerModel);
        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [Authorize(Roles = "Operator")]
    [HttpPost("create-bank-account")]
    public async Task<IActionResult> CreateAccount(AccountRegisterDto accountRegisterDto)
    {
        var result = await _accountService.CreateAccountAsync(accountRegisterDto);
        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }

    [Authorize(Roles = "Operator")]
    [HttpPost("create-bank-card")]
    public async Task<IActionResult> CreateCard(CardRegisterDto cardRegisterDto)
    {
        var result = await _cardService.CreateCardAsync(cardRegisterDto);
        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }
}