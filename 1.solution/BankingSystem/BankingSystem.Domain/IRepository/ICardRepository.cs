using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.IRepository;

public interface ICardRepository : ITransaction
{
    Task CreateCardAsync(Card card);
    Task<Card?> GetCardByNumberAsync(string cardNumber);
    Task UpdateCardAsync(Card card);
    Task<Account?> GetAccountByCardNumberAsync(string cardNumber);
    Task<bool> ValidateCardAsync(string cardNumber,string pinCode);
}