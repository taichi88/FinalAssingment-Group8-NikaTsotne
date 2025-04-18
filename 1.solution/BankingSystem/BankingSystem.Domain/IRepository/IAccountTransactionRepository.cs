﻿using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.IRepository;

public interface IAccountTransactionRepository : ITransaction
{
    Task AddAccountTransactionAsync(Transaction accountTransaction);
    Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId, DateTime date);

}