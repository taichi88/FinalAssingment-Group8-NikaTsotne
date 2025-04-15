using BankingSystem.Application.IServices;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Identity;
using BankingSystem.Application.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Application.Services;

public class PersonAuthService : IPersonAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityPerson> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<PersonAuthService> _logger;

    public PersonAuthService(
        IConfiguration configuration,
        UserManager<IdentityPerson> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<PersonAuthService> logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<AuthenticationResponse> AuthenticationPersonAsync(PersonLoginDto loginDto)
    {
        _logger.LogInformation("Attempting authentication for user {Email}", loginDto.Email);

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User not found {Email}", loginDto.Email);
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            _logger.LogWarning("Authentication failed: Invalid password for user {Email}", loginDto.Email);
            throw new UnauthorizedException("Invalid email or password");
        }

        _logger.LogInformation("User {Email} authenticated successfully", loginDto.Email);
        var tokenResponse = await GenerateJwtToken(user);
        return tokenResponse;
    }

    public async Task<string> RegisterPersonAsync(PersonRegisterDto registerDto)
    {
        _logger.LogInformation("Starting user registration for {Email}", registerDto.Email);

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: Email {Email} already exists", registerDto.Email);
            throw new ValidationException("User with this email already exists");
        }

        var user = new IdentityPerson
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Name = registerDto.Name,
            Lastname = registerDto.Lastname,
            BirthDate = registerDto.BirthDate,
            IdNumber = registerDto.IdNumber,
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User creation failed for {Email}. Errors: {Errors}", registerDto.Email, errors);
            throw new ValidationException($"User registration failed: {errors}");
        }

        string roleName = registerDto.Role.ToString();
        
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            _logger.LogError("Role does not exist: {Role}", roleName);
            throw new ValidationException($"Invalid role: {roleName}");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            _logger.LogError("Failed to assign role {Role} to user {Email}", roleName, registerDto.Email);
            await _userManager.DeleteAsync(user);
            throw new ValidationException("Failed to assign role to user");
        }

        _logger.LogInformation("User {Email} registered successfully with role {Role}", registerDto.Email, roleName);
        return $"User {registerDto.Email} registered successfully with role {roleName}";
    }

    private async Task<AuthenticationResponse> GenerateJwtToken(IdentityPerson user)
    {
        _logger.LogInformation("Generating JWT token for user {Email}", user.Email);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
            ?? throw new ValidationException("JWT key is not configured")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("userId", user.Id)
        };

        claims.AddRange(roleClaims);
        var expiration = DateTime.UtcNow.AddMinutes(30);

        var tokenGenerator = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenGenerator);

        _logger.LogInformation("JWT token generated successfully for user {Email}", user.Email);

        return new AuthenticationResponse
        {
            Token = token,
            Email = user.Email,
            UserId = user.Id,
            Expiration = expiration,
            Role = string.Join(",", roles)
        };
    }
}
