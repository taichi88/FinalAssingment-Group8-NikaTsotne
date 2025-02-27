using BankingSystem.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Infrastructure;
public class TestDataSeeder
{
    private readonly UserManager<IdentityPerson> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public TestDataSeeder(UserManager<IdentityPerson> userManager, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        await SeedInitialDataAsync();
    }

    public async Task SeedInitialDataAsync()
    {
        var roles = new[] { "Operator", "Person" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await _userManager.FindByNameAsync("Operator") is null)
        {
            var identityPerson = new IdentityPerson
            {
                Name = "Operator",
                UserName = "Operator",
                Email = "test@test.com",
                IdNumber = "12312312312",
                Lastname = "Test",
                BirthDate = DateTime.Now
            };

            var test = await _userManager.CreateAsync(identityPerson, "P@ssword!@#$5");
            await _userManager.AddToRoleAsync(identityPerson, "Operator");
        }
    }
}