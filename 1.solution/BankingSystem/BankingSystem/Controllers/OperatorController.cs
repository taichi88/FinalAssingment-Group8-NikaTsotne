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
[Authorize(Roles = "Operator")]
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

    [HttpPost("user")]
    public async Task<IActionResult> RegisterUser([FromForm] PersonRegisterDto registerModel)
    {
        var result = await _authService.RegisterPersonAsync(registerModel);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }

    [HttpPost("account")]
    public async Task<IActionResult> CreateAccount([FromForm] AccountRegisterDto accountRegisterDto)
    {
        var result = await _accountService.CreateAccountAsync(accountRegisterDto);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }

    [HttpPost("card")]
    public async Task<IActionResult> CreateCard([FromForm] CardRegisterDto cardRegisterDto)
    {
        var result = await _cardService.CreateCardAsync(cardRegisterDto);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }
}