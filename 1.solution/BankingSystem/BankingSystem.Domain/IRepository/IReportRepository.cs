
namespace BankingSystem.Domain.IRepository
{
    public interface IReportRepository : ITransaction
    {
        Task<int> GetUserCountThisYearAsync();
        Task<int> GetUserCountLastYearAsync();
        Task<int> GetUserCountLast30DaysAsync();

        Task<int> GetTransactionCountLastMonthAsync();
        Task<int> GetTransactionCountLast6MonthsAsync();
        Task<int> GetTransactionCountLastYearAsync();

        Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLastMonthAsync();
        Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLast6MonthsAsync();
        Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLastYearAsync();
        Task<IEnumerable<(string Currency, decimal Amount)>> GetAverageTransactionIncomeAsync();

        Task<IEnumerable<(DateTime Day, int Count)>> GetTransactionsByDayLastMonthAsync();

        Task<decimal> GetTotalAtmWithdrawalAmountAsync();
    }
}