using System.Data;
using BankingSystem.Application.Helpers;
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
            "INSERT INTO Cards(FirstName, Lastname, CardNumber, ExpirationDate, CVV, PinCode, AccountId) " +
            "VALUES (@Firstname, @Lastname, @CardNumber, @ExpirationDate, @Cvv, @PinCode, @AccountId)";

        await _connection.ExecuteAsync(query, card, _transaction);
    }

    public async Task<Card?> GetCardByNumberAsync(string cardNumber)
    {
        string encryptedCardNumber = CardSecurityHelper.Encrypt(cardNumber);

        var encryptedCard = await _connection.QuerySingleOrDefaultAsync<Card>(
            "SELECT * FROM Cards WHERE CardNumber = @CardNumber",
            new { CardNumber = encryptedCardNumber },
            _transaction);

        if (encryptedCard == null) return null;

        return CardConverter.DecryptCard(encryptedCard);
    }



    public async Task UpdateCardAsync(Card card)
    {
        var encryptedCard = CardConverter.EncryptCard(card);

        const string query =
            "UPDATE Cards SET Firstname = @Firstname, Lastname = @Lastname, ExpirationDate = @ExpirationDate, " +
            "CVV = @Cvv, PinCode = @PinCode, AccountId = @AccountId WHERE CardNumber = @CardNumber";

        await _connection.ExecuteAsync(query, encryptedCard, _transaction);
    }


    public async Task<Account?> GetAccountByCardNumberAsync(string cardNumber)
    {
        string encryptedCardNumber = CardSecurityHelper.Encrypt(cardNumber);

        const string query = @"SELECT a.Id AS AccountId, a.*
                              FROM Accounts a
                              INNER JOIN Cards c ON a.Id = c.AccountId
                              WHERE c.CardNumber = @CardNumber";

        return await _connection.QuerySingleOrDefaultAsync<Account>(
            query,
            new { CardNumber = encryptedCardNumber },
            _transaction);
    }

    public async Task<bool> ValidateCardAsync(string cardNumber, string pinCode)
    {
        string encryptedCardNumber = CardSecurityHelper.Encrypt(cardNumber);

        var card = await _connection.QuerySingleOrDefaultAsync<Card>(
            "SELECT * FROM Cards WHERE CardNumber = @CardNumber",
            new { CardNumber = encryptedCardNumber },
            _transaction);

        if (card == null) return false;

        // Use the VerifyPinCode method to check if the provided pinCode matches the stored hash
        return CardSecurityHelper.VerifyPinCode(pinCode, card.PinCode);
    }

}
