using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.CustomValidationAttributes;

namespace BankingSystem.Application.DTO;

public class AccountRegisterDto
{
    [Required(ErrorMessage = "ID number is required.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "ID number must be exactly 11 digits.")]
    public required string IdNumber { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [IbanValidation]
    public string Iban { get; set; }

    [Required(ErrorMessage = "Balance is required.")]
    [NonNegativeNumberValidation(ErrorMessage = "Balance cannot be negative.")]
    public decimal Balance { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [AllowedValues("GEL", "USD", "EUR", ErrorMessage = "Currency must be GEL, USD, or EUR.")]
    public  string Currency { get; set; }
}