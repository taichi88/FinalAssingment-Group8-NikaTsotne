using System.Data;
using BankingSystem.Domain.IRepository;
using Dapper;

namespace BankingSystem.Infrastructure.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction = null!;

        public ReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        #region User Statistics Methods

        public async Task<int> GetUserCountThisYearAsync()
        {
            const string storedProcedure = "sp_GetUserCountThisYear";
            return await _connection.ExecuteScalarAsync<int>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        public async Task<int> GetUserCountLastYearAsync()
        {
            const string storedProcedure = "sp_GetUserCountLastYear";
            return await _connection.ExecuteScalarAsync<int>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        public async Task<int> GetUserCountLast30DaysAsync()
        {
            const string storedProcedure = "sp_GetUserCountLast30Days";
            return await _connection.ExecuteScalarAsync<int>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        #endregion

        #region Transaction Statistics Methods

        public async Task<int> GetTransactionCountLastMonthAsync()
        {
            const string storedProcedure = "sp_GetTransactionCountLastMonth";
            return await _connection.ExecuteScalarAsync<int>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        public async Task<int> GetTransactionCountLast6MonthsAsync()
        {
            const string storedProcedure = "sp_GetTransactionCountLast6Months";
            return await _connection.ExecuteScalarAsync<int>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        public async Task<int> GetTransactionCountLastYearAsync()
        {
            const string storedProcedure = "sp_GetTransactionCountLastYear";
            return await _connection.ExecuteScalarAsync<int>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        #endregion

        #region Transaction Income Methods

        public async Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLastMonthAsync()
        {
            const string storedProcedure = "sp_GetTransactionIncomeLastMonth";
            var results = await _connection.QueryAsync<CurrencyAmountResult>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return results.Select(r => (r.Currency, r.Amount));
        }

        public async Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLast6MonthsAsync()
        {
            const string storedProcedure = "sp_GetTransactionIncomeLast6Months";
            var results = await _connection.QueryAsync<CurrencyAmountResult>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return results.Select(r => (r.Currency, r.Amount));
        }

        public async Task<IEnumerable<(string Currency, decimal Amount)>> GetTransactionIncomeLastYearAsync()
        {
            const string storedProcedure = "sp_GetTransactionIncomeLastYear";
            var results = await _connection.QueryAsync<CurrencyAmountResult>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return results.Select(r => (r.Currency, r.Amount));
        }

        public async Task<IEnumerable<(string Currency, decimal Amount)>> GetAverageTransactionIncomeAsync()
        {
            const string storedProcedure = "sp_GetAverageTransactionIncome";
            var results = await _connection.QueryAsync<CurrencyAmountResult>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return results.Select(r => (r.Currency, r.Amount));
        }

        #endregion

        #region Daily Transaction Breakdown

        public async Task<IEnumerable<(DateTime Day, int Count)>> GetTransactionsByDayLastMonthAsync()
        {
            const string storedProcedure = "sp_GetTransactionsByDayLastMonth";
            var results = await _connection.QueryAsync<DailyTransactionResult>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return results.Select(r => (r.Day, r.Count));
        }

        #endregion

        #region ATM Withdrawal Statistics

        public async Task<decimal> GetTotalAtmWithdrawalAmountAsync()
        {
            const string storedProcedure = "sp_GetTotalAtmWithdrawalAmount";
            return await _connection.ExecuteScalarAsync<decimal>(
                storedProcedure,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction);
        }

        #endregion

        #region Helper Classes for Dapper

        private class CurrencyAmountResult
        {
            public string Currency { get; set; } = string.Empty;
            public decimal Amount { get; set; }
        }

        private class DailyTransactionResult
        {
            public DateTime Day { get; set; }
            public int Count { get; set; }
        }

        #endregion
    }
}
