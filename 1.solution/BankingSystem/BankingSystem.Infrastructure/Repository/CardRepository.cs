using System.Data;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;
using Dapper;

namespace BankingSystem.Infrastructure.Repository;

public class CardRepository : ICardRepository
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction = null!;

    public CardRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }
    public async Task CreateCardAsync(Card card)
    {
        const string query =
            "INSERT INTO Cards(FirstName, Lastname, CardNumber, ExpirationDate, CVV, PinCode, AccountId) VALUES (@FirstName, @Lastname, @CardNumber, @ExpirationDate, @CVV, @PinCode, @AccountId)";

        await _connection.ExecuteAsync(query, card, _transaction);
    }

    public async Task<Card?> GetCardByNumberAsync(string cardNumber)
    {
        return await _connection.QuerySingleOrDefaultAsync<Card>(
            "select * from Cards where cardNumber = @CardNumber", new
            {
                CardNumber = cardNumber
            });
    }

    public async Task UpdateCardAsync(Card card)
    {
        const string query =
            "UPDATE Cards SET FirstName = @FirstName, Lastname = @Lastname, CardNumber = @CardNumber, ExpirationDate = @ExpirationDate, CVV = @CVV, PinCode = @PinCode, AccountId = @AccountId WHERE cardNumber = @CardNumber";
        await _connection.ExecuteAsync(query, card, _transaction);
    }


    public async Task<Account?> GetAccountByCardNumberAsync(string cardNumber)
    {
        const string query = @"SELECT a.Id AS AccountId, a.*
                                FROM Accounts a
                                INNER JOIN Cards c ON a.Id = c.AccountId
                                WHERE c.CardNumber = @cardNumber";

        return await _connection.QuerySingleOrDefaultAsync<Account>(query, new { cardNumber }, _transaction);
    }

    public async Task<bool> ValidateCardAsync(string cardNumber, string pinCode)
    {
        var card = await GetCardByNumberAsync(cardNumber);
        return card != null && pinCode == card.PinCode;
    }
}