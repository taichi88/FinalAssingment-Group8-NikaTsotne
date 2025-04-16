using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Exceptions;
using BankingSystem.Application.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BankingSystem.Application.Services;

public class AtmTokenService : IAtmTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AtmTokenService> _logger;

    public AtmTokenService(IConfiguration configuration, ILogger<AtmTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<AtmAuthenticationResponse> GenerateAtmTokenAsync(string cardNumber)
    {
        _logger.LogInformation("Generating ATM JWT token for card {CardNumber}", cardNumber);

        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                ?? throw new ValidationException("JWT key is not configured")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("cardNumber", cardNumber),
                new("atmSession", "true")
            };

            var expiration = DateTime.UtcNow.AddMinutes(3);
            var tokenGenerator = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenGenerator);

            _logger.LogInformation("ATM JWT token generated successfully for card {CardNumber}", cardNumber);

            return Task.FromResult(new AtmAuthenticationResponse
            {
                Token = token,
                CardNumber = cardNumber,
                Expiration = expiration,
                Message = "Card authorized successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for card {CardNumber}", cardNumber);
            throw new ValidationException("Failed to generate authentication token");
        }
    }
}
