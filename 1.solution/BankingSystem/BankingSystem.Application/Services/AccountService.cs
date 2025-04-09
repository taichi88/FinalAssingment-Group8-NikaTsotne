using BankingSystem.Application.DTO;
using BankingSystem.Domain.Entities;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Application.Exceptions;
using Microsoft.Extensions.Logging;
using BankingSystem.Application.Helpers;


namespace BankingSystem.Application.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> CreateAccountAsync(AccountRegisterDto accountRegisterDto)
    {
        _logger.LogInformation("Creating bank account for user with ID number {IdNumber}", accountRegisterDto.IdNumber);

        var personId = await _unitOfWork.PersonRepository.GetUserByIdNumberAsync(accountRegisterDto.IdNumber);

        if (string.IsNullOrEmpty(personId))
        {
            _logger.LogWarning("Failed to create account: Person with ID number {IdNumber} not found", accountRegisterDto.IdNumber);
            throw new NotFoundException($"Person with ID number {accountRegisterDto.IdNumber} not found");
        }

        string iban = accountRegisterDto.Iban ?? IbanGenerator.GenerateValidIban();

        var account = new Account
        {
            IBAN = iban,
            Balance = accountRegisterDto.Balance,
            PersonId = personId,
            Currency = accountRegisterDto.Currency
        };

        await _unitOfWork.AccountRepository.CreateAccountAsync(account);

        _logger.LogInformation("Bank account with IBAN {IBAN} created successfully for user {IdNumber}",
            accountRegisterDto.Iban, accountRegisterDto.IdNumber);

        return $"Account created successfully. Your Account IBAN is: {iban}";
    }
}
