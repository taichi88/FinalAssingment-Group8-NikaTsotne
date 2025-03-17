using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.RegularExpressions;

namespace BankingSystem.Domain.CustomValidationAttributes;
public class IbanValidationAttribute : ValidationAttribute
{
    private const string IbanPattern = @"^[A-Z]{2}\d{2}[A-Z0-9]{11,30}$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("IBAN is required.");

        string iban = value.ToString()!.Replace(" ", "").ToUpper();

        if (!Regex.IsMatch(iban, IbanPattern))
            return new ValidationResult("Invalid IBAN format.");

        return ValidateIbanChecksum(iban) ? ValidationResult.Success : new ValidationResult("Invalid IBAN checksum.");
    }

    private static bool ValidateIbanChecksum(string iban)
    {
        string rearranged = iban[4..] + iban[..4];
        string numericIban = string.Concat(rearranged.Select(c => char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));
        return BigInteger.Parse(numericIban) % 97 == 1;
    }
}