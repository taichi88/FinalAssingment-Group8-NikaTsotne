using BankingSystem.Domain.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BankingSystem.Domain.Entities;

public class Account
{
    public int AccountId { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [IbanValidation]
    public required string IBAN { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required(ErrorMessage = "Balance is required.")]
    [NonNegativeNumberValidation]
    public decimal Balance { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code in uppercase.")]
    public required string Currency { get; set; }

    [Required(ErrorMessage = "Person ID is required.")]
    public required string PersonId { get; set; }
}