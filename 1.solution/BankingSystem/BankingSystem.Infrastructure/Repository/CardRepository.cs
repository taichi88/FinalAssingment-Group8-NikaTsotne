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
            "INSERT INTO Cards(Name, Lastname, CardNumber, ExpirationDate, CVV, PinCode, AccountId) VALUES (@Name, @Lastname, @CardNumber, @ExpirationDate, @CVV, @PinCode, @AccountId)";

        await _connection.ExecuteAsync(query, card, _transaction);
    }

    public async Task<Card?> GetCardAsync(string cardNumber)
    {
        return await _connection.QuerySingleOrDefaultAsync<Card>(
            "select * from Cards where cardNumber = @CardNumber", new
            {
                CardNumber = cardNumber
            });
    }
    public async Task<bool> ValidateCardAsync(string cardNumber, string pinCode)
    {
        var card = await GetCardAsync(cardNumber);
        return card != null && pinCode == card.PinCode;
    }
}