using BankingSystem.Application.Identity;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.IRepository;

namespace BankingSystem.Infrastructure.DataSeeding;

public class AccountSeeder
{
    private readonly IAccountRepository _accountRepository;

    public AccountSeeder(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<List<Account>> SeedAccountsForUserAsync(IdentityPerson person, int accountCount)
    {
        var accounts = new List<Account>();
            var currencies = new[] { CurrencyType.GEL, CurrencyType.USD, CurrencyType.EUR };

        for (int i = 0; i < accountCount; i++)
        {
            CurrencyType currency = currencies[i % currencies.Length];

            var account = new Account
            {
                IBAN = DataSeederHelpers.GenerateIban(),
                Balance = 1000m * (i + 1),
                Currency = currency,
                PersonId = person.Id
            };

            await _accountRepository.CreateAccountAsync(account);

            var createdAccount = new Account
            {
                AccountId = i + 1, 
                IBAN = account.IBAN,
                Balance = account.Balance,
                Currency = account.Currency,
                PersonId = account.PersonId
            };

            accounts.Add(createdAccount);
        }

        return accounts;
    }
}
