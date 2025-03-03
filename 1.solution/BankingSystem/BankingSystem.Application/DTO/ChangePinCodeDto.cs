using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class ChangePinCodeDto
{
    [Required(ErrorMessage = "Card number is required")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Old PIN code is required")]
    public string OldPinCode { get; set; }

    [Required(ErrorMessage = "New PIN code is required")]
    public string NewPinCode { get; set; }
}