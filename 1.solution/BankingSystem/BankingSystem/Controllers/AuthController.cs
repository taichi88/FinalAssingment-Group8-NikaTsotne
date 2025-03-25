using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using BankingSystem.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankingSystem.Controllers;

[ValidateModel]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPersonAuthService _authService;

    public AuthController(IPersonAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(PersonLoginDto loginModel)
    {
        var result = await _authService.AuthenticationPersonAsync(loginModel);
        var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
        return Ok(response);
    }
}
