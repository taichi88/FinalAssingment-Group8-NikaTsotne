using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BankingSystem.Domain.CustomValidationAttributes;

public class PasswordValidationAttribute : ValidationAttribute
{
    private const int MinLength = 6;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("Password is required.");

        var password = value.ToString()!;

        // Check minimum length
        if (password.Length < MinLength)
            return new ValidationResult($"Password must be at least {MinLength} characters.");

        // Check for at least one uppercase letter
        if (!Regex.IsMatch(password, @"[A-Z]"))
            return new ValidationResult("Password must contain at least one uppercase letter.");

        // Check for at least one lowercase letter
        if (!Regex.IsMatch(password, @"[a-z]"))
            return new ValidationResult("Password must contain at least one lowercase letter.");

        // Check for at least one digit
        if (!Regex.IsMatch(password, @"[0-9]"))
            return new ValidationResult("Password must contain at least one number.");

        // Check for at least one special character
        if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
            return new ValidationResult("Password must contain at least one special character.");

        return ValidationResult.Success;
    }
}