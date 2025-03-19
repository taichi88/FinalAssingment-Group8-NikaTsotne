using BankingSystem.Domain.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BankingSystem.Domain.Entities;

public class Account
{
    [Key]
    [Required(ErrorMessage = "Account ID is required.")]
    public int AccountId { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [IbanValidation]
    public required string IBAN { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required(ErrorMessage = "Balance is required.")]
    [NonNegativeNumberValidation]
    public decimal Balance { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [AllowedValues("GEL", "USD", "EUR", ErrorMessage = "Currency must be GEL, USD, or EUR.")]
    public required string Currency { get; set; }

    [Required(ErrorMessage = "Person ID is required.")]
    public required string PersonId { get; set; }
}