// BankingSystem.Domain/CustomValidationAttributes/LuhnCardNumberValidationAttribute.cs
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BankingSystem.Domain.CustomValidationAttributes;

/// <summary>
/// Validates that a credit card number follows the Luhn algorithm (mod 10)
/// </summary>
public class LuhnCardNumberValidationAttribute : ValidationAttribute
{
    private const string CardNumberPattern = @"^\d{16}$"; // Pattern for a 16-digit card number

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Allow null values - this is checked by Required attribute if needed
        if (value == null)
            return ValidationResult.Success;

        string cardNumber = value.ToString()!.Replace(" ", "").Replace("-", "");

        // Check basic format first (16 digits)
        if (!Regex.IsMatch(cardNumber, CardNumberPattern))
            return new ValidationResult(ErrorMessage ?? "Card number must be 16 digits.");

        // Apply Luhn algorithm for validation
        if (!IsValidLuhn(cardNumber))
            return new ValidationResult(ErrorMessage ?? "Invalid card number (checksum failed).");

        return ValidationResult.Success;
    }

    /// <summary>
    /// Validates a card number using the Luhn algorithm (mod 10 check)
    /// </summary>
    private bool IsValidLuhn(string cardNumber)
    {
        // 1. Starting from the rightmost digit, double every second digit
        // 2. If doubling results in a two-digit number, add those digits together
        // 3. Sum all digits (doubled and not doubled)
        // 4. If the total sum mod 10 equals 0, the number is valid

        int sum = 0;
        bool alternate = false;

        // Process from right to left
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

        // If the sum is divisible by 10, the card number is valid
        return sum % 10 == 0;
    }
}
