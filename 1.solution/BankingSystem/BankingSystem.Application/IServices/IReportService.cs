using BankingSystem.Application.DTO.Response.ReportResponses;

namespace BankingSystem.Application.IServices
{
    public interface IReportService
    {
        Task<UserStatisticsResponse> GetUserStatisticsAsync();
        Task<TransactionStatisticsResponse> GetTransactionStatisticsAsync();
        Task<MonthlyTransactionBreakdownResponse> GetMonthlyTransactionBreakdownAsync();
        Task<AtmWithdrawalStatisticsResponse> GetAtmWithdrawalStatisticsAsync();
    }
}