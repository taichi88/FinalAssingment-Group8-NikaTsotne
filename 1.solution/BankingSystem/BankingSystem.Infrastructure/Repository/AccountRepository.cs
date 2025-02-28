using System.Data;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;
using Dapper;

namespace BankingSystem.Infrastructure.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction = null!;

    public AccountRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CreateAccountAsync(Account account)
    {
        const string query = "INSERT INTO Accounts(IBAN, Balance, Currency, PersonId) VALUES (@IBAN, @Balance, @Currency, @PersonId)";

        await _connection.ExecuteAsync(query, new{account.IBAN, account.Balance, account.Currency, account.PersonId }, _transaction);
    }

    public async Task UpdateAccountAsync(Account account)
    {
        const string query = "UPDATE Accounts SET IBAN = @IBAN, Balance = @Balance, Currency = @Currency,  PersonId= @PersonId WHERE Id = @AccountId";

        await _connection.ExecuteAsync(query, account, _transaction);
    }

    public async Task<Account> GetAccountByIdAsync(int id)
    {
        const string query = "SELECT Id AS AccountId,IBAN, Balance, Currency, PersonId FROM Accounts WHERE Id = @Id";

        return await _connection.QueryFirstAsync<Account>(query, new { Id = id }, _transaction);
    }
}