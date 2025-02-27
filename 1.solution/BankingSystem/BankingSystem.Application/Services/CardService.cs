using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.IUnitOfWork;

namespace BankingSystem.Application.Services;

public class CardService(IUnitOfWork unitOfWork) : ICardService
{
    public async Task<bool> CreateCardAsync(CardRegisterDto CardRegisterDto)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var person = await unitOfWork.AccountRepository.GetAccountByIdAsync(CardRegisterDto.AccountId);

            var card = new Card
            {
                CardNumber = CardRegisterDto.CardNumber,
                Cvv = CardRegisterDto.Cvv,
                PinCode = CardRegisterDto.PinCode,
                ExpirationDate = CardRegisterDto.ExpirationDate,
                AccountId = CardRegisterDto.AccountId,
                Firstname = CardRegisterDto.Firstname,
                Lastname = CardRegisterDto.Lastname
            };

            await unitOfWork.CardRepository.CreateCardAsync(card);
            await unitOfWork.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}