using System.Data;
using BankingSystem.Domain.IRepository;
using BankingSystem.Domain.IUnitOfWork;

namespace BankingSystem.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public IPersonRepository PersonRepository { get; }
    public IAccountTransactionRepository TransactionRepository { get; }
    public ICardRepository CardRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public IReportRepository ReportRepository { get; }

    public UnitOfWork(IDbConnection connection, IPersonRepository personRepository,
        IAccountTransactionRepository transactionRepository, ICardRepository cardRepository,
        IAccountRepository accountRepository, IReportRepository reportRepository)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        PersonRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        TransactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        CardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        AccountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        ReportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        try
        {
            _connection.Open();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to open database connection", ex);
        }

    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = await Task.Run(() => _connection.BeginTransaction());
            PersonRepository.SetTransaction(_transaction);
            TransactionRepository.SetTransaction(_transaction);
            CardRepository.SetTransaction(_transaction);
            AccountRepository.SetTransaction(_transaction);
            ReportRepository.SetTransaction(_transaction);
        }

    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await Task.Run(() => _transaction.Commit());
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await Task.Run(() => _transaction.Rollback());
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        await Task.Run(() => _transaction?.Dispose());
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await RollbackAsync();
            }

            _connection.Close();
            _connection.Dispose();
            _disposed = true;
        }
    }
}
