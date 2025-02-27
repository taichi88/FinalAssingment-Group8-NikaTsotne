using BankingSystem.Domain.Entities;
using System.Data;

namespace BankingSystem.Domain.RepositoryContracts;

public interface ICardRepository : ITransaction
{
    Task CreateCardAsync(Card card);
    Task<bool> ValidateCardAsync(string cardNumber,string pinCode);
}