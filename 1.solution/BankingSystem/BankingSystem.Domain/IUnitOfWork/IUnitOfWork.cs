﻿using BankingSystem.Domain.IRepository;

namespace BankingSystem.Domain.IUnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IPersonRepository PersonRepository { get; }
        public IAccountTransactionRepository TransactionRepository { get; }
        public ICardRepository CardRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IReportRepository ReportRepository { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}