using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Helpers
{
    public static class CardConverter
    {
        public static Card DecryptCard(Card encryptedCard)
        {
            return new Card
            {
                CardId = encryptedCard.CardId,
                Firstname = encryptedCard.Firstname,
                Lastname = encryptedCard.Lastname,
                CardNumber = CardSecurityHelper.Decrypt(encryptedCard.CardNumber),
                ExpirationDate = encryptedCard.ExpirationDate,
                Cvv = CardSecurityHelper.Decrypt(encryptedCard.Cvv),
                PinCode = encryptedCard.PinCode,
                AccountId = encryptedCard.AccountId
            };
        }

        public static Card EncryptCard(Card decryptedCard)
        {
            return new Card
            {
                CardId = decryptedCard.CardId,
                Firstname = decryptedCard.Firstname,
                Lastname = decryptedCard.Lastname,
                CardNumber = CardSecurityHelper.Encrypt(decryptedCard.CardNumber),
                ExpirationDate = decryptedCard.ExpirationDate,
                Cvv = CardSecurityHelper.Encrypt(decryptedCard.Cvv),
                PinCode = decryptedCard.PinCode,
                AccountId = decryptedCard.AccountId
            };
        }
    }
}