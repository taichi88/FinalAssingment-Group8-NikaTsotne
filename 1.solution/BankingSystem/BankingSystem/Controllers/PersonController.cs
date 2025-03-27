using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BankingSystem.Middleware;

namespace BankingSystem.Controllers;

[ValidateModel]
[ApiController]
[Authorize(Roles = "Person")]
[Route("api/[controller]")]
public class PersonController(IAccountTransactionService transactionService, IPersonService personService) : ControllerBase
{
    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction(TransactionDto transactionDto)
    {
        var userId = User.FindFirst("userId")!.Value;
        var result = await transactionService.TransactionBetweenAccountsAsync(transactionDto, userId);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst("userId")!.Value;
        var result = await personService.GetPersonById(userId);
        return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
    }
}