using BankingSystem.Application.IServices;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BankingSystem.Application.DTO;
using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;


namespace BankingSystem.Application.Services;

public class PersonAuthService : IPersonAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityPerson> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PersonAuthService(IConfiguration configuration, UserManager<IdentityPerson> userManager, RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<AuthenticationResponse> AuthenticationPersonAsync(PersonLoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new AuthenticationResponse
            {
                Token = string.Empty,
                Email = loginDto.Email,
                UserId = string.Empty,
                Expiration = DateTime.MinValue,
                ErrorMessage = "Invalid email or password"
            };
        }
        var tokenResponse = await GenerateJwtToken(user);
        return tokenResponse;
    }

    //აქ ხდება ჩიერ სვაგერში შეყვანილი ინფორმაციის (დტო-ში) დაკავშრება და დაესაინება IdentityPerson -ის მოდელისთვის, რომლის ინიციალიზაციაც ხდება 
    //var user -ში. ამასთან IdentityPerson არის მოდელი რომელიც ინჰერიტანსით იღებს მეთდებს IdentityUser კლასისგან.
    //შესაბამისად ეს მონაცემები გადაეცემა IdentityPerson და IdentityUsers-ს მიაქვს.
    public async Task<bool> RegisterPersonAsync(PersonRegisterDto registerDto)
    {
        var user = new IdentityPerson
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Name = registerDto.Name,
            Lastname = registerDto.Lastname,
            BirthDate = registerDto.BirthDate,
            IdNumber = registerDto.IdNumber
        };
        //ეს არის უზერმენეჯერის მიერ გამოსაყენებელი მეთოდი, და ამ მეთოდი მიდის ეს ინფორმაციები უზერ მენეჯერში და ინახება.
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return false;

        if (string.IsNullOrEmpty(registerDto.Role))
            registerDto.Role = "User";

        if (!await _roleManager.RoleExistsAsync(registerDto.Role))
            return false;

        await _userManager.AddToRoleAsync(user, registerDto.Role);

        return true;
    }

    public async Task<AuthenticationResponse> GenerateJwtToken(IdentityPerson user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id)
        };

        claims.AddRange(roleClaims);
        var expiration = DateTime.UtcNow.AddHours(1);
        
        var tokenGenerator = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        string token = tokenHandler.WriteToken(tokenGenerator);
        
        return new AuthenticationResponse()
        {
            Token = token, 
            Email = user.Email, 
            UserId = user.Id, 
            Expiration = expiration
        };
    }
}
