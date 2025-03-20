namespace BankingSystem.Application.DTO.Response.ReportResponses;

public class DailyTransactionCount
{
    public string Day { get; set; } = string.Empty;
    public int Count { get; set; }
}