using BankingSystem.Application.DTO;
using BankingSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperatorController : ControllerBase
{
    private readonly IPersonAuthService _authService;
    private readonly IAccountService _accountService;
    private readonly ICardService _cardService;
    public OperatorController(IPersonAuthService authService, IAccountService accountService, ICardService cardService)
    {
        _authService = authService;
        _accountService = accountService;
        _cardService = cardService;
    }

    [Authorize(Roles = "Operator")]
    [HttpPost("register-user")]

    //From Body აქ საჭირო არ არის, ვინაიდან ის მხოლოდ მაშინ გამოიყენება როდესაც ხდება ორი კომპლექსური პარამეტრის გადაცემა.
    //მაგალთად ROute, Query, ვინაიდან ასპ.ნეტ ქორი ვერ ხვდება საიდან უნდა წამოიღოს ეს მონაცემი. ამ დროს ვუთითებთ. 
    //ეხლა ვერ მოვხსნი არ მინდა რამე ავრიო...
    public async Task<IActionResult> RegisterUser([FromBody] PersonRegisterDto registerModel)
    {
        if (!await _authService.RegisterPersonAsync(registerModel))
        {
            return BadRequest("Invalid operation.");
        }

        return Created();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] PersonLoginDto loginModel)
    {
        var result = await _authService.AuthenticationPersonAsync(loginModel);

        if (result == null || string.IsNullOrEmpty(result.Token))
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    //[Authorize(Roles = "Operator")]
    [HttpPost("create-bank-account")]
    public async Task<IActionResult> CreateAccount(AccountRegisterDto AccountRegisterDto)
    {
        var result = await _accountService.CreateAccountAsync(AccountRegisterDto);
        if (result == false)
        {
            return BadRequest();
        }

        return Ok(new { message = "Account created successfully" });
    }

    [Authorize(Roles = "Operator")]
    [HttpPost("create-bank-card")]
    public async Task<IActionResult> CreateCard(CardRegisterDto cardRegisterDto)
    {
        
        await _cardService.CreateCardAsync(cardRegisterDto);
        return Ok(new { message = "Card created successfully" });
    }
}