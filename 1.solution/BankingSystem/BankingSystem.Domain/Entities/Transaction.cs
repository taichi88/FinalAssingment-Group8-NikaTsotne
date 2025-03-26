using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Entities;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Transaction fee is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Transaction fee cannot be negative.")]
    public decimal TransactionFee { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    public required CurrencyType Currency { get; set; }

    [Required(ErrorMessage = "Transaction date is required.")]
    public DateTime TransactionDate { get; set; }

    [Required(ErrorMessage = "FromAccountId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "FromAccountId must be a positive number.")]
    public int FromAccountId { get; set; }

    public Account? FromAccount { get; set; }


    [Required(ErrorMessage = "ToAccountId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ToAccountId must be a positive number.")]
    public int ToAccountId { get; set; }

    public Account? ToAccount { get; set; }

    public bool IsATM { get; set; } // Added property

    public TransactionType? TransactionType { get; set; }
}
