using BankingSystem.Domain.CustomValidationAttributes;
using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystem.Domain.Entities;

public class Account
{
    [Key]
    [Required(ErrorMessage = "Account ID is required.")]
    public int AccountId { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [IbanValidation(AllowNull = false)]
    public required string IBAN { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Required(ErrorMessage = "Balance is required.")]
    [NonNegativeNumberValidation]
    public decimal Balance { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    public required CurrencyType Currency { get; set; }

    [Required(ErrorMessage = "Person ID is required.")]
    public required string? PersonId { get; set; }
}