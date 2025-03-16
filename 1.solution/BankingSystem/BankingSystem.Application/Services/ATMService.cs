using System.Net;
using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.IUnitOfWork;

namespace BankingSystem.Application.Services;

public class AtmService : IAtmService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiResponse _response;

    public AtmService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _response = new();
    }

    public async Task<ApiResponse> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto)
    {
        var authorized = await _unitOfWork.CardRepository.ValidateCardAsync(cardAuthorizationDto.CardNumber,
            cardAuthorizationDto.PinCode);
        if (!authorized)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Card number or pin code is invalid.");
            return _response;
        }

        _response.StatusCode = HttpStatusCode.NoContent;
        return _response;
    }

    public async Task<ApiResponse> ViewBalanceAsync(string cardNumber)
    {
        var response = new ApiResponse();
        var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);
        if (account == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { "Card not found" };
        }
        else
        {
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Result = new { Balance = account.Balance };
        }

        return response;
    }

    public async Task<ApiResponse> WithdrawMoneyAsync(string cardNumber, WithdrawMoneyDto withdrawMoneyDto)
    {
        var response = new ApiResponse();
        await _unitOfWork.BeginTransactionAsync();
        var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);
        if (account == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { "Card not found" };
            return response;
        }

        // Calculate the total amount including the fee
        var fee = withdrawMoneyDto.Amount * 0.02m;
        var totalAmount = withdrawMoneyDto.Amount + fee;

        // Check if the total amount exceeds the account balance
        if (totalAmount > account.Balance)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { "Insufficient balance" };
            return response;
        }

        // Check if the daily limit is exceeded
        var transactions = await _unitOfWork.TransactionRepository.GetTransactionsByAccountIdAsync(account.AccountId, DateTime.Now.Date);
        var dailyTotal = transactions.Where(t => t.IsATM).Sum(t => t.Amount);
        if (dailyTotal + withdrawMoneyDto.Amount > 10000m)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { "Daily withdrawal limit exceeded" };
            return response;
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

        response.StatusCode = HttpStatusCode.OK;
        response.IsSuccess = true;
        response.Result = new { Balance = account.Balance };

        return response;
    }

    public async Task<ApiResponse> ChangePinCodeAsync(string cardNumber, ChangePinCodeDto changePinCodeDto)
    {
        var response = new ApiResponse();
        var card = await _unitOfWork.CardRepository.GetCardByNumberAsync(cardNumber);
        if (card == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { "Card not found" };
        }
        else if (card.PinCode != changePinCodeDto.OldPinCode)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.IsSuccess = false;
            response.ErrorMessages = new List<string> { "Old PIN code is incorrect" };
        }
        else
        {
            card.PinCode = changePinCodeDto.NewPinCode;
            await _unitOfWork.CardRepository.UpdateCardAsync(card);
            await _unitOfWork.CommitAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
        }

        return response;
    }
}
