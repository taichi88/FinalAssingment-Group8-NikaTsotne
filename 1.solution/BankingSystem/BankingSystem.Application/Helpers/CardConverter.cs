using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Helpers
{
    public static class CardConverter
    {
        public static Card DecryptCard(Card encryptedCard)
        {
            return new Card
            {
                Firstname = encryptedCard.Firstname,
                Lastname = encryptedCard.Lastname,
                CardNumber = CardSecurityHelper.Decrypt(encryptedCard.CardNumber),
                ExpirationDate = encryptedCard.ExpirationDate,
                Cvv = CardSecurityHelper.Decrypt(encryptedCard.Cvv),
                PinCode = encryptedCard.PinCode, // PIN code remains hashed
                AccountId = encryptedCard.AccountId
            };
        }

        public static Card EncryptCard(Card decryptedCard)
        {
            return new Card
            {
                Firstname = decryptedCard.Firstname,
                Lastname = decryptedCard.Lastname,
                CardNumber = CardSecurityHelper.Encrypt(decryptedCard.CardNumber),
                ExpirationDate = decryptedCard.ExpirationDate,
                Cvv = CardSecurityHelper.Encrypt(decryptedCard.Cvv),
                PinCode = decryptedCard.PinCode, // PIN code remains hashed
                AccountId = decryptedCard.AccountId
            };
        }
    }
}