using BankingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingSystem.Domain.IRepository
{
    public interface IReportRepository : ITransaction
    {
        // User statistics methods
        Task<int> GetUserCountThisYearAsync();
        Task<int> GetUserCountLastYearAsync();
        Task<int> GetUserCountLast30DaysAsync();

        // Transaction statistics methods
        Task<int> GetTransactionCountLastMonthAsync();
        Task<int> GetTransactionCountLast6MonthsAsync();
        Task<int> GetTransactionCountLastYearAsync();

        // Transaction income by currency
        Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLastMonthAsync();
        Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLast6MonthsAsync();
        Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLastYearAsync();
        Task<IEnumerable<(string Currency, decimal Amount)>> GetAverageTransactionIncomeAsync();

        // Daily transaction breakdown
        Task<IEnumerable<(DateTime Day, int Count)>> GetTransactionsByDayLastMonthAsync();

        // ATM withdrawal statistics
        Task<decimal> GetTotalAtmWithdrawalAmountAsync();
    }
}