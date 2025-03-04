using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class WithdrawMoneyDto
{

    [Required(ErrorMessage = "Amount is required")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    public string Currency { get; set; }

    public bool IsATM { get; set; } = true; // New property with default value
}