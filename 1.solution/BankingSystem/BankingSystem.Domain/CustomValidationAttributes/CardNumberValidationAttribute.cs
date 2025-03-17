using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BankingSystem.Domain.CustomValidationAttributes;

public class CardNumberValidationAttribute : ValidationAttribute
{
    private const string CardNumberPattern = @"^\d{16}$"; // Example pattern for a 16-digit card number

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("Card number is required.");

        string cardNumber = value.ToString()!.Replace(" ", "");

        if (!Regex.IsMatch(cardNumber, CardNumberPattern))
            return new ValidationResult("Invalid card number format.");

        return ValidationResult.Success;
    }
}
