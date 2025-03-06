using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public DateTime TransactionDate { get; set; }
    public int FromAccountId { get; set; }
    public Account? FromAccount { get; set; }
    public int ToAccountId { get; set; }
    public Account? ToAccount { get; set; }
    public bool IsATM { get; set; } // Added property
    [Required(ErrorMessage = "TransactionType is required.")]
    [EnumDataType(typeof(TransactionType))]
    public TransactionType TransactionType { get; set; }
}