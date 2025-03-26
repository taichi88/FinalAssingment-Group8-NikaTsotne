using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.DTO.Response;
public class CardResponseDto
{
    public string CardNumber { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Cvv { get; set; }
    public string PinCode { get; set; }
    public int AccountId { get; set; }
}