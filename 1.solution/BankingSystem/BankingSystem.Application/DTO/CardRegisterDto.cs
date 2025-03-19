using System.ComponentModel.DataAnnotations;
using BankingSystem.Domain.CustomValidationAttributes;

namespace BankingSystem.Application.DTO;

public class CardRegisterDto
{

    [Required(ErrorMessage = "Card number is required.")]
    [CardNumberValidation(ErrorMessage = "Invalid card number format.")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Firstname is required.")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Lastname is required.")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Expiration date is required.")]
    public  DateTime ExpirationDate { get; set; }

    [Required(ErrorMessage = "CVV is required.")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be exactly 3 digits.")]
    public string Cvv { get; set; }

    [Required(ErrorMessage = "PIN code is required.")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN code must be 4 digits.")]
    public string PinCode { get; set; }
    [Required(ErrorMessage = "AccountId is required.")]
    public int AccountId { get; set; }
}