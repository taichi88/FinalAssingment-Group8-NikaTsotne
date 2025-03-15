using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.DTO;

public class WithdrawMoneyDto
{

    [Required(ErrorMessage = "Amount is required")]
    public decimal Amount { get; set; }


}
