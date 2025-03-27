using BankingSystem.Application.IServices;
using BankingSystem.Filters;
using BankingSystem.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankingSystem.Controllers
{
    [ValidateModel]
    [ApiController]
    [Authorize(Roles = "Manager")]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var result = await _reportService.GetUserStatisticsAsync();
            return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionStatistics()
        {
            var result = await _reportService.GetTransactionStatisticsAsync();
            return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
        }

        [HttpGet("transactions/monthly")]
        public async Task<IActionResult> GetMonthlyTransactionBreakdown()
        {
            var result = await _reportService.GetMonthlyTransactionBreakdownAsync();
            return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
        }

        [HttpGet("atm-withdrawals")]
        public async Task<IActionResult> GetAtmWithdrawalStatistics()
        {
            var result = await _reportService.GetAtmWithdrawalStatisticsAsync();
            return Ok(ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result));
        }
    }
}
