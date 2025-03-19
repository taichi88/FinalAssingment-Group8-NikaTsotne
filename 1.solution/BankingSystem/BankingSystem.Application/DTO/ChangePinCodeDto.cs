using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class ChangePinCodeDto
{
    [Required(ErrorMessage = "Old PIN code is required")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN code must be 4 digits.")]

    public string OldPinCode { get; set; }

    [Required(ErrorMessage = "New PIN code is required")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN code must be 4 digits.")]

    public string NewPinCode { get; set; }
}