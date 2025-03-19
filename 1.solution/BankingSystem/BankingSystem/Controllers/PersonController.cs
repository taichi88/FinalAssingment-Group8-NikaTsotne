using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BankingSystem.Middleware;

namespace BankingSystem.Controllers
{
    [ValidateModel]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController(IAccountTransactionService transactionService, IPersonService personService) : ControllerBase
    {
        [Authorize(Roles = "Person")]
        [HttpPost("transfer-money")]
        public async Task<IActionResult> TransferMoney(TransactionDto transactionDto)
        {
            var userId = User.FindFirst("userId")!.Value;
            var result = await transactionService.TransactionBetweenAccountsAsync(transactionDto, userId);

            var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, new { Message = result });
            return Ok(response);
        }

        [Authorize(Roles = "Person")]
        [HttpGet("get-info")]
        public async Task<IActionResult> GetPersonInfo()
        {
            var userId = User.FindFirst("userId")!.Value;

            var result = await personService.GetPersonById(userId);

            var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
            return Ok(response);
        }
    }
}