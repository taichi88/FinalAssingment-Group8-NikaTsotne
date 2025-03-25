// BankingSystem.Application/DTO/Response/AtmAuthenticationResponse.cs
namespace BankingSystem.Application.DTO.Response;

public class AtmAuthenticationResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string? Message { get; set; }
}