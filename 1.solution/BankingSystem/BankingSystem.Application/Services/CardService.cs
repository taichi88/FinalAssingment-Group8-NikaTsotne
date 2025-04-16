using BankingSystem.Application.DTO;
using BankingSystem.Application.Helpers;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Application.Exceptions;
using Microsoft.Extensions.Logging;
using BankingSystem.Application.DTO.Response;

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

    public async Task<CardResponseDto> CreateCardAsync(CardRegisterDto cardRegisterDto)
    {
        _logger.LogInformation("Creating bank card for account ID {AccountId}", cardRegisterDto.AccountId);

        var account = await _unitOfWork.AccountRepository.GetAccountByIdAsync(cardRegisterDto.AccountId);
        if (account == null)
        {
            _logger.LogWarning("Failed to create card: Account with ID {AccountId} not found", cardRegisterDto.AccountId);
            throw new NotFoundException($"Account with ID {cardRegisterDto.AccountId} not found");
        }

        string cardNumber = cardRegisterDto.CardNumber ?? CardSecurityHelper.GenerateCardNumber();
        string cvv = cardRegisterDto.Cvv ?? CardSecurityHelper.GenerateCvv();
        string pinCode = cardRegisterDto.PinCode ?? CardSecurityHelper.GeneratePinCode();

        var response = new CardResponseDto
        {
            CardNumber = cardNumber,
            Cvv = cvv,
            PinCode = pinCode,
            ExpirationDate = cardRegisterDto.ExpirationDate,
            AccountId = cardRegisterDto.AccountId,
            Firstname = cardRegisterDto.Firstname,
            Lastname = cardRegisterDto.Lastname
        };

        var card = new Card
        {
            CardNumber = CardSecurityHelper.Encrypt(cardNumber),
            Cvv = CardSecurityHelper.Encrypt(cvv),
            PinCode = CardSecurityHelper.HashPinCode(pinCode),
            ExpirationDate = cardRegisterDto.ExpirationDate,
            AccountId = cardRegisterDto.AccountId,
            Firstname = cardRegisterDto.Firstname,
            Lastname = cardRegisterDto.Lastname
        };

        await _unitOfWork.CardRepository.CreateCardAsync(card);

        _logger.LogInformation("Bank card created successfully for account {AccountId}", cardRegisterDto.AccountId);

        return response;
    }
}
