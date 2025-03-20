using BankingSystem.Application.DTO.Response;
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
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        //[Authorize(Roles = "Manager")]
        [HttpGet("user-statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var result = await _reportService.GetUserStatisticsAsync();
            var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
            return Ok(response);
        }

        //[Authorize(Roles = "Manager")]
        [HttpGet("transaction-statistics")]
        public async Task<IActionResult> GetTransactionStatistics()
        {
            var result = await _reportService.GetTransactionStatisticsAsync();
            var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
            return Ok(response);
        }

        //[Authorize(Roles = "Manager")]
        [HttpGet("monthly-transaction-breakdown")]
        public async Task<IActionResult> GetMonthlyTransactionBreakdown()
        {
            var result = await _reportService.GetMonthlyTransactionBreakdownAsync();
            var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
            return Ok(response);
        }

        //[Authorize(Roles = "Manager")]
        [HttpGet("atm-withdrawal-statistics")]
        public async Task<IActionResult> GetAtmWithdrawalStatistics()
        {
            var result = await _reportService.GetAtmWithdrawalStatisticsAsync();
            var response = ErrorHandlingMiddleware.CreateSuccessResponse(HttpStatusCode.OK, result);
            return Ok(response);
        }
    }
}
