using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.Services;

public class CardService : ICardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CardService> _logger;

    public CardService(IUnitOfWork unitOfWork, ILogger<CardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> CreateCardAsync(CardRegisterDto cardRegisterDto)
    {
        try
        {
            _logger.LogInformation("Creating bank card for account ID {AccountId}", cardRegisterDto.AccountId);
            await _unitOfWork.BeginTransactionAsync();

            // Check if the account exists
            var account = await _unitOfWork.AccountRepository.GetAccountByIdAsync(cardRegisterDto.AccountId);
            if (account == null)
            {
                _logger.LogWarning("Failed to create card: Account with ID {AccountId} not found", cardRegisterDto.AccountId);
                throw new NotFoundException($"Account with ID {cardRegisterDto.AccountId} not found");
            }

            var card = new Card
            {
                CardNumber = cardRegisterDto.CardNumber,
                Cvv = cardRegisterDto.Cvv,
                PinCode = cardRegisterDto.PinCode,
                ExpirationDate = cardRegisterDto.ExpirationDate,
                AccountId = cardRegisterDto.AccountId,
                Firstname = cardRegisterDto.Firstname,
                Lastname = cardRegisterDto.Lastname
            };

            await _unitOfWork.CardRepository.CreateCardAsync(card);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Bank card {CardNumber} created successfully for account {AccountId}",
                cardRegisterDto.CardNumber, cardRegisterDto.AccountId);

            return "Card created successfully";
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw; // Let middleware handle the exception
        }
    }
}
