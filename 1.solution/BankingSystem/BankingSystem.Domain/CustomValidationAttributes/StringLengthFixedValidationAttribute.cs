using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.CustomValidationAttributes;

public class StringLengthFixedValidationAttribute(int fixedLength) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is string stringValue)
            return stringValue.Length == fixedLength
                ? ValidationResult.Success
                : new ValidationResult($"The field {validationContext.DisplayName} must be exactly {fixedLength} characters.");

        return new ValidationResult("Invalid data type; must be a string.");
    }
}