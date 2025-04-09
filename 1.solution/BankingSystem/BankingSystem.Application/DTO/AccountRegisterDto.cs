using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.CustomValidationAttributes;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTO;

public class AccountRegisterDto
{
    [Required(ErrorMessage = "ID number is required.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "ID number must be exactly 11 digits.")]
    public required string IdNumber { get; set; }

    [IbanValidation(AllowNull = true)]
    public string? Iban { get; set; }

    [Required(ErrorMessage = "Balance is required.")]
    [NonNegativeNumberValidation(ErrorMessage = "Balance cannot be negative.")]
    public decimal Balance { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [EnumDataType(typeof(CurrencyType), ErrorMessage = "Invalid currency type. Use 'GEL', 'USD' or 'EUR' ")]
    public CurrencyType Currency { get; set; }
}