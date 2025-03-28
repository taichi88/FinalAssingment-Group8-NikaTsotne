using BankingSystem.Application.Helpers;
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
            for (int i = 0; i < 2; i++)
            {
                string cardNumber = CardSecurityHelper.GenerateCardNumber(); 
                string cvv = CardSecurityHelper.GenerateCvv();
                string pinCode = CardSecurityHelper.GeneratePinCode();

                Console.WriteLine($"Account {account.AccountId}, Card {i + 1}:");
                Console.WriteLine($"Card Number: {cardNumber}");
                Console.WriteLine($"CVV: {cvv}");
                Console.WriteLine($"PIN: {pinCode}");
                Console.WriteLine("----------------------------");

                var card = new Card
                {
                    Firstname = "Card" + (i + 1),
                    Lastname = "Holder" + (i + 1),
                    CardNumber = CardSecurityHelper.Encrypt(cardNumber), // Encrypt card number
                    ExpirationDate = DateTime.Now.AddYears(4),
                    Cvv = CardSecurityHelper.Encrypt(cvv), // Encrypt CVV
                    PinCode = CardSecurityHelper.HashPinCode(pinCode), // Hash PIN code
                    AccountId = account.AccountId
                };

                await _cardRepository.CreateCardAsync(card);
            }
        }
    }
}
