using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.CustomValidationAttributes;

public class NonNegativeNumberValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("Number is required.");

        if (value is decimal decimalValue && decimalValue < 0)
            return new ValidationResult("Number must be 0 or positive.");

        return ValidationResult.Success;
    }
}