using System.Data;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;
using Dapper;

namespace BankingSystem.Infrastructure.Repository;

public class TransactionRepository : IAccountTransactionRepository
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction = null!;

    public TransactionRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task AddAccountTransactionAsync(Transaction transactionObj)
    {
        const string query =
            "INSERT INTO Transactions(Amount, Currency, TransactionDate, FromAccountId, ToAccountId, IsATM, TransactionType, TransactionFee) VALUES (@Amount, @Currency, @TransactionDate, @FromAccountId, @ToAccountId, @IsATM, @TransactionType, @TransactionFee)";

        await _connection.ExecuteAsync(query, transactionObj, _transaction);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId, DateTime date)
    {
        const string query =
            "SELECT * FROM Transactions WHERE FromAccountId = @AccountId AND CAST(TransactionDate AS DATE) = @Date";

        return await _connection.QueryAsync<Transaction>(query, new { AccountId = accountId, Date = date }, _transaction);
    }

}
