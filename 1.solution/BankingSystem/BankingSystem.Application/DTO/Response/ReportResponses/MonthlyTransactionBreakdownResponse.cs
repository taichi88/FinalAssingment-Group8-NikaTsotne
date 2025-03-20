namespace BankingSystem.Application.DTO.Response.ReportResponses;

public class MonthlyTransactionBreakdownResponse
{
    public string ReportType { get; set; } = "monthly_transaction_breakdown";
    public List<DailyTransactionCount> TransactionsByDay { get; set; } = new();
}