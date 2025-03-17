using System.Net;
using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.Services;

public class AtmService : IAtmService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AtmService> _logger;

    public AtmService(IUnitOfWork unitOfWork, ILogger<AtmService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto)
    {
        _logger.LogInformation("Authorizing card {CardNumber}", cardAuthorizationDto.CardNumber);
        var authorized = await _unitOfWork.CardRepository.ValidateCardAsync(cardAuthorizationDto.CardNumber, cardAuthorizationDto.PinCode);
        if (!authorized)
        {
            _logger.LogWarning("Authorization failed for card {CardNumber}", cardAuthorizationDto.CardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.BadRequest, "Card number or pin code is invalid.");
        }

        _logger.LogInformation("Authorization successful for card {CardNumber}", cardAuthorizationDto.CardNumber);
        return new ApiResponse { StatusCode = HttpStatusCode.NoContent };
    }

    public async Task<ApiResponse> ViewBalanceAsync(string cardNumber)
    {
        _logger.LogInformation("Viewing balance for card {CardNumber}", cardNumber);
        var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);
        if (account == null)
        {
            _logger.LogWarning("Card not found {CardNumber}", cardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Card not found");
        }

        return new ApiResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            Result = new { Balance = account.Balance }
        };
    }

    public async Task<ApiResponse> WithdrawMoneyAsync(string cardNumber, WithdrawMoneyDto withdrawMoneyDto)
    {
        _logger.LogInformation("Withdrawing money for card {CardNumber}", cardNumber);
        await _unitOfWork.BeginTransactionAsync();
        var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);
        if (account == null)
        {
            _logger.LogWarning("Card not found {CardNumber}", cardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Card not found");
        }

        // Calculate the total amount including the fee
        var fee = withdrawMoneyDto.Amount * 0.02m;
        var totalAmount = withdrawMoneyDto.Amount + fee;

        // Check if the total amount exceeds the account balance
        if (totalAmount > account.Balance)
        {
            _logger.LogWarning("Insufficient balance for card {CardNumber}", cardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.BadRequest, "Insufficient balance");
        }

        // Check if the daily limit is exceeded
        var transactions = await _unitOfWork.TransactionRepository.GetTransactionsByAccountIdAsync(account.AccountId, DateTime.Now.Date);
        var dailyTotal = transactions.Where(t => t.IsATM).Sum(t => t.Amount);
        if (dailyTotal + withdrawMoneyDto.Amount > 10000m)
        {
            _logger.LogWarning("Daily withdrawal limit exceeded for card {CardNumber}", cardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.BadRequest, "Daily withdrawal limit exceeded");
        }

        // Update the account balance
        account.Balance -= totalAmount;
        await _unitOfWork.AccountRepository.UpdateAccountAsync(account);

        // Create the transaction
        var transaction = new Transaction
        {
            FromAccountId = account.AccountId,
            ToAccountId = account.AccountId, // For ATM withdrawal, from and to account are the same
            Currency = account.Currency,
            Amount = withdrawMoneyDto.Amount,
            TransactionDate = DateTime.Now,
            IsATM = true,
            TransactionType = null
        };

        await _unitOfWork.TransactionRepository.AddAccountTransactionAsync(transaction);
        await _unitOfWork.CommitAsync();

        return new ApiResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            Result = new { Balance = account.Balance }
        };
    }

    public async Task<ApiResponse> ChangePinCodeAsync(string cardNumber, ChangePinCodeDto changePinCodeDto)
    {
        _logger.LogInformation("Changing PIN code for card {CardNumber}", cardNumber);
        var card = await _unitOfWork.CardRepository.GetCardByNumberAsync(cardNumber);
        if (card == null)
        {
            _logger.LogWarning("Card not found {CardNumber}", cardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.NotFound, "Card not found");
        }
        else if (card.PinCode != changePinCodeDto.OldPinCode)
        {
            _logger.LogWarning("Old PIN code is incorrect for card {CardNumber}", cardNumber);
            return ApiResponse.CreateErrorResponse(HttpStatusCode.BadRequest, "Old PIN code is incorrect");
        }

        card.PinCode = changePinCodeDto.NewPinCode;
        await _unitOfWork.CardRepository.UpdateCardAsync(card);
        await _unitOfWork.CommitAsync();

        return new ApiResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true
        };
    }
}

