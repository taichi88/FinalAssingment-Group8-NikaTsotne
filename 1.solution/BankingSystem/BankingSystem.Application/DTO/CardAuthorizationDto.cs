using BankingSystem.Domain.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class CardAuthorizationDto
{
    [Required(ErrorMessage = "Card number is required.")]
    [LuhnCardNumberValidation(ErrorMessage = "Invalid card number format or checksum.")]
    public string CardNumber { get; set; }
    [Required(ErrorMessage = "PIN code is required.")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN code must be 4 digits.")]
    public string PinCode { get; set; }
}