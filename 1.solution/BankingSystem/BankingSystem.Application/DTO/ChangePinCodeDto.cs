using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class ChangePinCodeDto
{
    [Required(ErrorMessage = "Old PIN code is required")]
    public string OldPinCode { get; set; }

    [Required(ErrorMessage = "New PIN code is required")]
    public string NewPinCode { get; set; }
}