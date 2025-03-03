using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class WithdrawMoneyDto
{
    [Required(ErrorMessage = "Card number is required")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    public string Currency { get; set; }
}