using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IRepository;

namespace BankingSystem.Infrastructure.DataSeeding;

public class CardSeeder
{
    private readonly ICardRepository _cardRepository;

    public CardSeeder(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task SeedCardsForAccountsAsync(List<Account> accounts)
    {
        foreach (var account in accounts)
        {
            // Create two cards for each account
            for (int i = 0; i < 2; i++)
            {
                var card = new Card
                {
                    Firstname = "Card" + (i + 1),
                    Lastname = "Holder" + (i + 1),
                    CardNumber = DataSeederHelpers.GenerateCardNumber(),
                    ExpirationDate = DateTime.Now.AddYears(4),
                    Cvv = DataSeederHelpers.GenerateCvv(),
                    PinCode = new Random().Next(1000, 9999).ToString(),
                    AccountId = account.AccountId
                };

                await _cardRepository.CreateCardAsync(card);
            }
        }
    }
}
