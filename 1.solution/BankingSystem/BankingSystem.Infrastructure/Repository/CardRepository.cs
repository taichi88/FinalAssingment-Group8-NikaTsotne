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
        // Card is already encrypted/hashed by the service before reaching the repository
        const string query =
            "INSERT INTO Cards(FirstName, Lastname, CardNumber, ExpirationDate, CVV, PinCode, AccountId) " +
            "VALUES (@Firstname, @Lastname, @CardNumber, @ExpirationDate, @Cvv, @PinCode, @AccountId)";

        await _connection.ExecuteAsync(query, card, _transaction);
    }

    public async Task<Card?> GetCardByNumberAsync(string cardNumber)
    {
        // Encrypt the card number for database lookup
        string encryptedCardNumber = CardSecurityHelper.Encrypt(cardNumber);

        return await _connection.QuerySingleOrDefaultAsync<Card>(
            "SELECT * FROM Cards WHERE CardNumber = @CardNumber",
            new { CardNumber = encryptedCardNumber },
            _transaction);
    }

    public async Task UpdateCardAsync(Card card)
    {
        // Card is already encrypted/hashed by the service before reaching the repository
        const string query =
            "UPDATE Cards SET Firstname = @Firstname, Lastname = @Lastname, ExpirationDate = @ExpirationDate, " +
            "CVV = @Cvv, PinCode = @PinCode, AccountId = @AccountId WHERE CardNumber = @CardNumber";

        await _connection.ExecuteAsync(query, card, _transaction);
    }

    public async Task<Account?> GetAccountByCardNumberAsync(string cardNumber)
    {
        // Encrypt the card number for database lookup
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
        // Encrypt the card number for database lookup
        string encryptedCardNumber = CardSecurityHelper.Encrypt(cardNumber);

        // Get the card using the encrypted card number
        var card = await _connection.QuerySingleOrDefaultAsync<Card>(
            "SELECT * FROM Cards WHERE CardNumber = @CardNumber",
            new { CardNumber = encryptedCardNumber },
            _transaction);

        if (card == null) return false;

        // Hash the pin code for comparison
        string hashedPinCode = CardSecurityHelper.HashPinCode(pinCode);

        // Compare the hashed PIN with the stored hashed PIN
        return hashedPinCode == card.PinCode;
    }
}
