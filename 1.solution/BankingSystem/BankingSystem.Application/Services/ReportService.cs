using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingSystem.Application.DTO.Response.ReportResponses;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.IRepository;
using BankingSystem.Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserStatisticsResponse> GetUserStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving user statistics");
                await _unitOfWork.BeginTransactionAsync();
                
                // Get user statistics from repository
                var thisYearCount = await _unitOfWork.ReportRepository.GetUserCountThisYearAsync();
                var lastYearCount = await _unitOfWork.ReportRepository.GetUserCountLastYearAsync();
                var last30DaysCount = await _unitOfWork.ReportRepository.GetUserCountLast30DaysAsync();
                
                await _unitOfWork.CommitAsync();
                
                // Map to response DTO
                return new UserStatisticsResponse
                {
                    ThisYear = thisYearCount,
                    LastYear = lastYearCount,
                    Last30Days = last30DaysCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user statistics");
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<TransactionStatisticsResponse> GetTransactionStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving transaction statistics");
                await _unitOfWork.BeginTransactionAsync();
                
                // Get transaction counts
                var last1MonthCount = await _unitOfWork.ReportRepository.GetTransactionCountLastMonthAsync();
                var last6MonthsCount = await _unitOfWork.ReportRepository.GetTransactionCountLast6MonthsAsync();
                var last1YearCount = await _unitOfWork.ReportRepository.GetTransactionCountLastYearAsync();
                
                // Get transaction income by currency
                var last1MonthIncome = await _unitOfWork.ReportRepository.GetTransactionIncomeLastMonthAsync();
                var last6MonthsIncome = await _unitOfWork.ReportRepository.GetTransactionIncomeLast6MonthsAsync();
                var last1YearIncome = await _unitOfWork.ReportRepository.GetTransactionIncomeLastYearAsync();
                var averageIncome = await _unitOfWork.ReportRepository.GetAverageTransactionIncomeAsync();
                
                await _unitOfWork.CommitAsync();
                
                // Map results to response DTO
                return new TransactionStatisticsResponse
                {
                    Last1MonthCount = last1MonthCount,
                    Last6MonthsCount = last6MonthsCount,
                    Last1YearCount = last1YearCount,
                    Last1MonthIncome = MapToCurrencyAmount(last1MonthIncome),
                    Last6MonthsIncome = MapToCurrencyAmount(last6MonthsIncome),
                    Last1YearIncome = MapToCurrencyAmount(last1YearIncome),
                    AverageIncome = MapToCurrencyAmount(averageIncome)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction statistics");
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<MonthlyTransactionBreakdownResponse> GetMonthlyTransactionBreakdownAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving monthly transaction breakdown");
                await _unitOfWork.BeginTransactionAsync();
                
                // Get daily transactions
                var transactionsByDay = await _unitOfWork.ReportRepository.GetTransactionsByDayLastMonthAsync();
                
                await _unitOfWork.CommitAsync();
                
                // Map to response DTO
                var response = new MonthlyTransactionBreakdownResponse
                {
                    TransactionsByDay = transactionsByDay.Select(t => new DailyTransactionCount
                    {
                        Day = t.Day.ToString("yyyy-MM-dd"),
                        Count = t.Count
                    }).ToList()
                };
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly transaction breakdown");
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<AtmWithdrawalStatisticsResponse> GetAtmWithdrawalStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving ATM withdrawal statistics");
                await _unitOfWork.BeginTransactionAsync();
                
                // Get total withdrawal amount
                var totalWithdrawalAmount = await _unitOfWork.ReportRepository.GetTotalAtmWithdrawalAmountAsync();
                
                await _unitOfWork.CommitAsync();
                
                // Map to response DTO
                return new AtmWithdrawalStatisticsResponse
                {
                    TotalWithdrawalAmount = totalWithdrawalAmount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ATM withdrawal statistics");
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // Helper method to map currency tuples to CurrencyAmount DTO
        private CurrencyAmount MapToCurrencyAmount(IEnumerable<(string Currency, decimal Amount)> currencyAmounts)
        {
            var result = new CurrencyAmount();
            
            foreach (var (currency, amount) in currencyAmounts)
            {
                switch (currency)
                {
                    case "GEL":
                        result.GEL = amount;
                        break;
                    case "USD":
                        result.USD = amount;
                        break;
                    case "EUR":
                        result.EUR = amount;
                        break;
                }
            }
            
            return result;
        }
    }
}
