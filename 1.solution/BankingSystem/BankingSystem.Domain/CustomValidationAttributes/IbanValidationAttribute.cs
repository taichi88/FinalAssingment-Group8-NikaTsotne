using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.RegularExpressions;

namespace BankingSystem.Domain.CustomValidationAttributes;
public class IbanValidationAttribute : ValidationAttribute
{
    // Updated pattern for Georgian IBANs: 2 letters + 2 digits + 2 letters + 16 digits = 22 characters total
    private const string IbanPattern = @"^[A-Z]{2}\d{2}[A-Z]{2}\d{16}$";

    public bool AllowNull { get; set; } = false;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return AllowNull ? ValidationResult.Success : new ValidationResult("IBAN is required.");
        }

        string iban = value.ToString()!.Replace(" ", "").ToUpper();

        if (!Regex.IsMatch(iban, IbanPattern))
            return new ValidationResult("Invalid IBAN format. Georgian IBAN must be exactly 22 characters: country code (2), check digits (2), bank code (2), and account number (16).");

        return ValidateIbanChecksum(iban) ? ValidationResult.Success : new ValidationResult("Invalid IBAN checksum.");
    }

    private static bool ValidateIbanChecksum(string iban)
    {
        string rearranged = iban[4..] + iban[..4];
        string numericIban = string.Concat(rearranged.Select(c => char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));
        return BigInteger.Parse(numericIban) % 97 == 1;
    }
}