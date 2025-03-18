using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BankingSystem.Domain.CustomValidationAttributes;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTO;

public class TransactionDto
{
    [Required(ErrorMessage = "Transaction amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "FromAccountId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "FromAccountId must be a positive number.")]
    public int FromAccountId { get; set; }

    [Required(ErrorMessage = "ToAccountId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ToAccountId must be a positive number.")]
    public int ToAccountId { get; set; }

    [Required(ErrorMessage = "Transaction type is required.")]
    [EnumDataType(typeof(TransactionType), ErrorMessage = "Invalid transaction type.")]
    public string TransactionType { get; set; }
}