using BankingSystem.Application.Constants;
using BankingSystem.Application.DTO;
using BankingSystem.Application.Exceptions;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.IExternalApi;
using BankingSystem.Domain.IRepository;
using BankingSystem.Domain.IUnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankingSystem.Tests.Application.Services;

public class AccountTransactionServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IExchangeRateApi> _mockExchangeRateApi;
    private readonly Mock<ILogger<AccountTransactionService>> _mockLogger;
    private readonly AccountTransactionService _service;
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IAccountTransactionRepository> _mockTransactionRepository;

    private const string UserId = "user123";
    private const string OtherUserId = "user456";
    private const string UnauthorizedUserId = "unauthorizedUser";

    private const string SourceIban = "GE23CD0905119827449079";
    private const string DestinationIban = "GE24CD6536896856566140";

    private const decimal TransferRate = 0.01m;
    private const decimal BaseFee = 0.5m;

    public AccountTransactionServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockExchangeRateApi = new Mock<IExchangeRateApi>();
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockTransactionRepository = new Mock<IAccountTransactionRepository>();
        _mockLogger = new Mock<ILogger<AccountTransactionService>>();

        var mockConfiguration = SetupMockConfiguration();

        _mockUnitOfWork.Setup(u => u.AccountRepository).Returns(_mockAccountRepository.Object);
        _mockUnitOfWork.Setup(u => u.TransactionRepository).Returns(_mockTransactionRepository.Object);

        _service = new AccountTransactionService(
            _mockUnitOfWork.Object,
            _mockExchangeRateApi.Object,
            new TransactionConstants(mockConfiguration.Object),
            _mockLogger.Object);
    }

    private Mock<IConfiguration> SetupMockConfiguration()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var sections = new Dictionary<string, string> {
            { "Transaction:TransferToOthersFeeRate", "0.01" },
            { "Transaction:TransferToOthersBaseFee", "0.5" },
            { "Transaction:ToMyAccountFee", "0.0" },
            { "Transaction:BaseCurrency", "GEL" }
        };

        foreach (var section in sections)
        {
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.Value).Returns(section.Value);
            mockConfiguration.Setup(c => c.GetSection(section.Key)).Returns(mockSection.Object);
        }

        return mockConfiguration;
    }

    private Account CreateAccount(int accountId, decimal balance, string personId,
        CurrencyType currency = CurrencyType.GEL) => new Account
        {
            AccountId = accountId,
            Balance = balance,
            PersonId = personId,
            Currency = currency,
            IBAN = accountId == 1 ? SourceIban : DestinationIban
        };

    private TransactionDto CreateTransactionDto(int fromAccountId, int toAccountId,
        decimal amount, TransactionType transactionType) => new TransactionDto
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            TransactionType = transactionType
        };

    private void SetupAccountRepositoryMocks(Account fromAccount, Account toAccount)
    {
        _mockAccountRepository.Setup(repo => repo.GetAccountByIdAsync(fromAccount.AccountId))
            .ReturnsAsync(fromAccount);
        _mockAccountRepository.Setup(repo => repo.GetAccountByIdAsync(toAccount.AccountId))
            .ReturnsAsync(toAccount);
    }

    private void VerifyTransactionSuccess(Account fromAccount, Account toAccount, decimal expectedFromBalance,
        decimal expectedToBalance, TransactionDto transactionDto, decimal expectedFee = 0m)
    {
        Assert.Equal(expectedFromBalance, fromAccount.Balance);
        Assert.Equal(expectedToBalance, toAccount.Balance);

        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(fromAccount), Times.Once);
        _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(toAccount), Times.Once);

        _mockTransactionRepository.Verify(repo =>
            repo.AddAccountTransactionAsync(It.Is<Transaction>(t =>
                t.FromAccountId == transactionDto.FromAccountId &&
                t.ToAccountId == transactionDto.ToAccountId &&
                t.Amount == transactionDto.Amount &&
                t.TransactionFee == expectedFee &&
                t.TransactionType == transactionDto.TransactionType)), Times.Once);
    }

    private void VerifyTransactionFailure()
    {
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task TransactionBetweenAccountsAsync_ToMyAccount_SuccessfulTransaction()
    {
        var fromAccount = CreateAccount(1, 1000m, UserId);
        var toAccount = CreateAccount(2, 500m, UserId);
        var transactionDto = CreateTransactionDto(1, 2, 100m, TransactionType.ToMyAccount);

        SetupAccountRepositoryMocks(fromAccount, toAccount);

        var result = await _service.TransactionBetweenAccountsAsync(transactionDto, UserId);

        Assert.Equal("The transaction was completed successfully.", result);
        VerifyTransactionSuccess(fromAccount, toAccount, 900m, 600m, transactionDto);
    }

    [Fact]
    public async Task TransactionBetweenAccountsAsync_TransferToOthers_SuccessfulTransaction()
    {
        var fromAccount = CreateAccount(1, 1000m, UserId);
        var toAccount = CreateAccount(2, 500m, OtherUserId);
        var transactionDto = CreateTransactionDto(1, 2, 100m, TransactionType.TransferToOthers);

        SetupAccountRepositoryMocks(fromAccount, toAccount);
        decimal expectedFee = transactionDto.Amount * TransferRate + BaseFee;

        var result = await _service.TransactionBetweenAccountsAsync(transactionDto, UserId);

        Assert.Equal("The transaction was completed successfully.", result);
        VerifyTransactionSuccess(
            fromAccount, toAccount,
            1000m - transactionDto.Amount - expectedFee,
            500m + transactionDto.Amount,
            transactionDto, expectedFee);
    }

    [Fact]
    public async Task TransactionBetweenAccountsAsync_DifferentCurrencies_ConvertsCurrencyCorrectly()
    {
        var fromAccount = CreateAccount(1, 1000m, UserId);
        var toAccount = CreateAccount(2, 500m, UserId, CurrencyType.EUR);
        var transactionDto = CreateTransactionDto(1, 2, 100m, TransactionType.ToMyAccount);

        _mockExchangeRateApi.Setup(api => api.GetExchangeRate("EUR"))
            .ReturnsAsync(0.85m);

        SetupAccountRepositoryMocks(fromAccount, toAccount);

        var result = await _service.TransactionBetweenAccountsAsync(transactionDto, UserId);

        Assert.Equal("The transaction was completed successfully.", result);
        _mockExchangeRateApi.Verify(api => api.GetExchangeRate("EUR"), Times.Once);

        Assert.Equal(900m, fromAccount.Balance);
        Assert.Equal(617.65m, toAccount.Balance, 2);

        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task TransactionBetweenAccountsAsync_UnauthorizedUser_ThrowsException()
    {
        var fromAccount = CreateAccount(1, 1000m, UserId);
        var toAccount = CreateAccount(2, 500m, "otherUser");
        var transactionDto = CreateTransactionDto(1, 2, 100m, TransactionType.ToMyAccount);

        SetupAccountRepositoryMocks(fromAccount, toAccount);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _service.TransactionBetweenAccountsAsync(transactionDto, UnauthorizedUserId));

        Assert.Equal("You are not authorized to access this account.", exception.Message);
        VerifyTransactionFailure();
    }

    [Theory]
    [InlineData(TransactionType.TransferToOthers, "user123", "user123", "Transfer to your own account is not allowed")]
    [InlineData(TransactionType.ToMyAccount, "user123", "user456", "Transfer to another user's account is not allowed")]
    public async Task TransactionBetweenAccountsAsync_InvalidTransferType_ThrowsException(
        TransactionType transactionType, string fromUserId, string toUserId, string expectedErrorMessage)
    {
        var fromAccount = CreateAccount(1, 1000m, fromUserId);
        var toAccount = CreateAccount(2, 500m, toUserId);
        var transactionDto = CreateTransactionDto(1, 2, 100m, transactionType);

        SetupAccountRepositoryMocks(fromAccount, toAccount);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.TransactionBetweenAccountsAsync(transactionDto, fromUserId));

        Assert.Equal(expectedErrorMessage, exception.Message);
        VerifyTransactionFailure();
    }

    [Theory]
    [InlineData(TransactionType.ToMyAccount, 50, 100)]
    [InlineData(TransactionType.TransferToOthers, 101, 100)]
    public async Task TransactionBetweenAccountsAsync_InsufficientFunds_ThrowsException(
        TransactionType transactionType, decimal balance, decimal amount)
    {
        var userId = UserId;
        var otherUserId = transactionType == TransactionType.TransferToOthers ? OtherUserId : UserId;

        var fromAccount = CreateAccount(1, balance, userId);
        var toAccount = CreateAccount(2, 500m, otherUserId);
        var transactionDto = CreateTransactionDto(1, 2, amount, transactionType);

        SetupAccountRepositoryMocks(fromAccount, toAccount);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.TransactionBetweenAccountsAsync(transactionDto, userId));

        Assert.Equal("The transaction was failed. You don't have enough money", exception.Message);
        VerifyTransactionFailure();
    }

    [Fact]
    public async Task TransactionBetweenAccountsAsync_InvalidTransactionType_ThrowsException()
    {
        var fromAccount = CreateAccount(1, 1000m, UserId);
        var toAccount = CreateAccount(2, 500m, UserId);
        var transactionDto = new TransactionDto
        {
            FromAccountId = fromAccount.AccountId,
            ToAccountId = toAccount.AccountId,
            Amount = 100m,
            TransactionType = (TransactionType)999
        };

        SetupAccountRepositoryMocks(fromAccount, toAccount);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.TransactionBetweenAccountsAsync(transactionDto, UserId));

        Assert.Equal("Invalid transaction type", exception.Message);
        VerifyTransactionFailure();
    }

    [Fact]
    public async Task TransactionBetweenAccountsAsync_AccountNotFound_ThrowsException()
    {
        var fromAccountId = 1;
        var toAccountId = 2;
        var transactionDto = CreateTransactionDto(fromAccountId, toAccountId, 100m, TransactionType.ToMyAccount);

        _mockAccountRepository.Setup(repo => repo.GetAccountByIdAsync(fromAccountId))
            .ThrowsAsync(new InvalidOperationException());

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.TransactionBetweenAccountsAsync(transactionDto, UserId));

        Assert.Equal($"Source account {fromAccountId} not found", exception.Message);
        VerifyTransactionFailure();
    }
}
