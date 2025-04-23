using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.CustomValidationAttributes;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Application.DTO;

public class TransactionDto
{
    [Required(ErrorMessage = "Transaction amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [IbanValidation(AllowNull = false)]
    public string FromAccountIban { get; set; }

    [Required(ErrorMessage = "IBAN is required.")]
    [IbanValidation(AllowNull = false)]
    public string ToAccountIban { get; set; }

    [Required(ErrorMessage = "Transaction type is required.")]
    [EnumDataType(typeof(TransactionType), ErrorMessage = $"Invalid transaction type. Use 'ToMyAccount' or 'TransferToOthers'.")]
    public TransactionType TransactionType { get; set; }
}