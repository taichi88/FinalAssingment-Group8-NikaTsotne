using System.Data;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;
using Dapper;

namespace BankingSystem.Infrastructure.Repository;

public class PersonRepository : IPersonRepository
{

    private readonly IDbConnection _connection;
    private IDbTransaction _transaction = null!;

    public PersonRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }
    public async Task<Person?> GetUserByIdAsync(string id)
    {
        const string query = @"
                                SELECT u.Id as PersonID, u.[Name], u.LastName, u.Email, u.IdNumber, u.BirthDate,
                                       a.Id as AccountID, a.IBAN, a.Balance, a.Currency, a.PersonId,
                                       c.Id as CardID, c.Firstname, c.Lastname, c.CardNumber, c.ExpirationDate, c.PinCode, c.CVV, c.AccountId
                                FROM [BankingSystem].[dbo].[AspNetUsers] u
                                LEFT JOIN Accounts a ON u.Id = a.PersonId
                                LEFT JOIN Cards c ON a.Id = c.AccountId
                                WHERE u.Id = @ID;";

        var userDictionary = new Dictionary<string, Person>();

        var users = await _connection.QueryAsync<Person, Account, Card, Person>(
            query,
            (person, Account, Card) =>
            {
                if (!userDictionary.TryGetValue(person.PersonId, out var currentUser))
                {
                    currentUser = person;
                    currentUser.Accounts = new List<Account>();
                    currentUser.Cards = new List<Card>();
                    userDictionary.Add(currentUser.PersonId, currentUser);
                }

                if (Account != null! && currentUser.Accounts!.All(a => a.AccountId != Account.AccountId))
                    currentUser.Accounts!.Add(Account);

                if (Card != null! && currentUser.Cards!.All(c => c.CardId != Card.CardId))
                    currentUser.Cards!.Add(Card);

                return currentUser;
            },
            new { ID = id },
            splitOn: "AccountID,CardID",
            transaction: _transaction);

        return users.FirstOrDefault();
    }
    public async Task<Person?> GetUserByUsernameAsync(string username)
    {
        const string query = @"
            SELECT Id FROM [BankingSystem].[dbo].[AspNetUsers] WHERE u.UserName = @Username";

        var users = await _connection.QueryFirstOrDefaultAsync<Person>(query, _transaction);

        return users;
    }

    public async Task<string> GetUserByIdNumberAsync(string IdNumber)
    {
        const string query = @"
            SELECT Id FROM AspNetUsers WHERE IdNumber = @IdNumber";

        var users = await _connection.QueryFirstOrDefaultAsync<string>(query, new { IdNumber }, _transaction);

        return users;
    }
}