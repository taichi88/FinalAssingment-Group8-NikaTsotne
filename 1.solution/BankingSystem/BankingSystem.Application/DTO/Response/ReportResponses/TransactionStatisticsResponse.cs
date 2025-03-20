namespace BankingSystem.Application.DTO.Response.ReportResponses;

public class TransactionStatisticsResponse
{
    public string ReportType { get; set; } = "transaction_statistics";
    public int Last1MonthCount { get; set; }
    public int Last6MonthsCount { get; set; }
    public int Last1YearCount { get; set; }
    public CurrencyAmount Last1MonthIncome { get; set; } = new();
    public CurrencyAmount Last6MonthsIncome { get; set; } = new();
    public CurrencyAmount Last1YearIncome { get; set; } = new();
    public CurrencyAmount AverageIncome { get; set; } = new();
}