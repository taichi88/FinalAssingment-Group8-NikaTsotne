namespace BankingSystem.Application.DTO.Response.ReportResponses;

public class AtmWithdrawalStatisticsResponse
{
    public string ReportType { get; set; } = "atm_withdrawal_statistics";
    public decimal TotalWithdrawalAmount { get; set; }
}