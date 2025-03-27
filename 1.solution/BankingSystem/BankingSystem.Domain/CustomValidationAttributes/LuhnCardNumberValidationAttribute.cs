using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BankingSystem.Domain.CustomValidationAttributes;

public class LuhnCardNumberValidationAttribute : ValidationAttribute
{
    private const string CardNumberPattern = @"^\d{16}$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        string cardNumber = value.ToString()!.Replace(" ", "").Replace("-", "");

        if (!Regex.IsMatch(cardNumber, CardNumberPattern))
            return new ValidationResult(ErrorMessage ?? "Card number must be 16 digits.");
        if (!IsValidLuhn(cardNumber))
            return new ValidationResult(ErrorMessage ?? "Invalid card number (checksum failed).");

        return ValidationResult.Success;
    }


    private bool IsValidLuhn(string cardNumber)
    {

        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }
}
