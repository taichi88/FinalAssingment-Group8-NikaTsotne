using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BankingSystem.Domain.CustomValidationAttributes;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTO;

public class TransactionDto
{
    [NegativeNumberValidation(ErrorMessage = "Amount cannot be negative.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "FromAccountId is required.")]
    public int FromAccountId { get; set; }

    [Required(ErrorMessage = "ToAccountId is required.")]
    public int ToAccountId { get; set; }
    public string TransactionType { get; set; }
}