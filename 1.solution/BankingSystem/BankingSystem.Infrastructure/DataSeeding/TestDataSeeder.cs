using BankingSystem.Application.Identity;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BankingSystem.Infrastructure.DataSeeding;

public class TestDataSeeder
{
    private readonly UserManager<IdentityPerson> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IAccountTransactionRepository _transactionRepository;
    private readonly IDbConnection _connection;
    private readonly ILogger<TestDataSeeder> _logger;
    private IDbTransaction _transaction = null!;

    public TestDataSeeder(
        UserManager<IdentityPerson> userManager,
        RoleManager<IdentityRole> roleManager,
        IAccountRepository accountRepository,
        ICardRepository cardRepository,
        IAccountTransactionRepository transactionRepository,
        IDbConnection connection,
        ILogger<TestDataSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
        _transactionRepository = transactionRepository;
        _connection = connection;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            // Set transaction for all repositories
            _accountRepository.SetTransaction(_transaction);
            _cardRepository.SetTransaction(_transaction);
            _transactionRepository.SetTransaction(_transaction);

            await SeedInitialDataAsync();

            _transaction.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during seed data creation");
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _connection.Close();
        }
    }

    public async Task SeedInitialDataAsync()
    {
        var roleSeeder = new RoleSeeder(_roleManager);
        await roleSeeder.SeedRolesAsync();

        var userSeeder = new UserSeeder(_userManager);
        var manager = await userSeeder.SeedManagerUserAsync();
        var operator1 = await userSeeder.SeedOperatorUserAsync();
        var persons = await userSeeder.SeedPersonUsersAsync(5);

        var accountSeeder = new AccountSeeder(_accountRepository);
        var allAccounts = new List<Account>();
        foreach (var person in persons)
        {
            var accounts = await accountSeeder.SeedAccountsForUserAsync(person, 3);
            allAccounts.AddRange(accounts);
        }

        var cardSeeder = new CardSeeder(_cardRepository);
        await cardSeeder.SeedCardsForAccountsAsync(allAccounts);

        var transactionSeeder = new TransactionSeeder(_transactionRepository, _accountRepository);
        await transactionSeeder.SeedSampleTransactionsAsync(allAccounts);
    }
}
