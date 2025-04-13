using BankingSystem.Application.Constants;
using BankingSystem.Application.DTO;
using BankingSystem.Application.Exceptions;
using BankingSystem.Application.Helpers;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.Services;

public class AtmService : IAtmService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AtmService> _logger;
    private readonly AtmConstants _atmConstants;

    public AtmService(IUnitOfWork unitOfWork, ILogger<AtmService> logger, AtmConstants atmConstants)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _atmConstants = atmConstants;
    }

    public async Task<string> AuthorizeCardAsync(CardAuthorizationDto cardAuthorizationDto)
    {
        _logger.LogInformation("Authorizing card {CardNumber}", cardAuthorizationDto.CardNumber);
        var isValidCredentials = await _unitOfWork.CardRepository.ValidateCardAsync(
            cardAuthorizationDto.CardNumber,
            cardAuthorizationDto.PinCode);

        if (!isValidCredentials)
        {
            _logger.LogWarning("Authorization failed for card {CardNumber}: invalid credentials", cardAuthorizationDto.CardNumber);
            throw new ValidationException("Card number or pin code is invalid.");
        }

        var card = await _unitOfWork.CardRepository.GetCardByNumberAsync(cardAuthorizationDto.CardNumber);

        if (card.ExpirationDate < DateTime.Now.Date)
        {
            _logger.LogWarning("Authorization failed for card {CardNumber}: card expired on {ExpirationDate}",
                cardAuthorizationDto.CardNumber, card.ExpirationDate.ToShortDateString());
            throw new ValidationException("Card has expired.");
        }

        _logger.LogInformation("Authorization successful for card {CardNumber}", cardAuthorizationDto.CardNumber);
        return "Card authorized successfully";
    }


    public async Task<string> ViewBalanceAsync(string cardNumber)
    {
        _logger.LogInformation("Viewing balance for card {CardNumber}", cardNumber);
        var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);

        if (account == null)
        {
            _logger.LogWarning("Card not found {CardNumber}", cardNumber);
            throw new NotFoundException("Card not found");
        }

        return $"Your balance is {account.Balance} {account.Currency}";
    }

    public async Task<string> WithdrawMoneyAsync(string cardNumber, WithdrawMoneyDto withdrawMoneyDto)
    {
        try
        {
            _logger.LogInformation("Withdrawing money for card {CardNumber}", cardNumber);
            await _unitOfWork.BeginTransactionAsync();

            var account = await _unitOfWork.CardRepository.GetAccountByCardNumberAsync(cardNumber);
            if (account == null)
            {
                _logger.LogWarning("Card not found {CardNumber}", cardNumber);
                throw new NotFoundException("Card not found");
            }

            var fee = withdrawMoneyDto.Amount * _atmConstants.FeeRate;
            var totalAmount = withdrawMoneyDto.Amount + fee;

            if (totalAmount > account.Balance)
            {
                _logger.LogWarning("Insufficient balance for card {CardNumber}", cardNumber);
                throw new ValidationException("Insufficient balance");
            }

            var transactions = await _unitOfWork.TransactionRepository.GetTransactionsByAccountIdAsync(account.AccountId, DateTime.Now.Date);
            var dailyTotal = transactions.Where(t => t.IsATM).Sum(t => t.Amount);
            if (dailyTotal + withdrawMoneyDto.Amount > _atmConstants.DailyWithdrawalLimit)
            {
                _logger.LogWarning("Daily withdrawal limit exceeded for card {CardNumber}", cardNumber);
                throw new ValidationException("Daily withdrawal limit exceeded");
            }

            account.Balance -= totalAmount;
            await _unitOfWork.AccountRepository.UpdateAccountAsync(account);

            var transaction = new Transaction
            {
                FromAccountId = account.AccountId,
                ToAccountId = account.AccountId,
                Currency = account.Currency,
                Amount = withdrawMoneyDto.Amount,
                TransactionFee = fee,
                TransactionDate = DateTime.Now,
                IsATM = true,
                TransactionType = null
            };

            await _unitOfWork.TransactionRepository.AddAccountTransactionAsync(transaction);
            await _unitOfWork.CommitAsync();

            return $"Withdrawal successful. You withdrew {withdrawMoneyDto.Amount} {account.Currency}. Your remaining balance is {account.Balance} {account.Currency}";
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }


    public async Task<string> ChangePinCodeAsync(string cardNumber, ChangePinCodeDto changePinCodeDto)
    {
        _logger.LogInformation("Changing PIN code for card {CardNumber}", cardNumber);
        var card = await _unitOfWork.CardRepository.GetCardByNumberAsync(cardNumber);
        if (card == null)
        {
            _logger.LogWarning("Card not found {CardNumber}", cardNumber);
            throw new NotFoundException("Card not found");
        }

        bool isPinValid = CardSecurityHelper.VerifyPinCode(changePinCodeDto.OldPinCode, card.PinCode);
        if (!isPinValid)
        {
            _logger.LogWarning("Old PIN code is incorrect for card {CardNumber}", cardNumber);
            throw new ValidationException("Old PIN code is incorrect");
        }


        card.PinCode = CardSecurityHelper.HashPinCode(changePinCodeDto.NewPinCode);
        await _unitOfWork.CardRepository.UpdateCardAsync(card);
        return "PIN code changed successfully";
    }

}
