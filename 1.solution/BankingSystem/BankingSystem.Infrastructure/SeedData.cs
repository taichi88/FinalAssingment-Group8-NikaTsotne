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
        var roles = new[] { "Operator", "Person", "Manager" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await _userManager.FindByNameAsync("Manager") is null)
        {
            var identityPerson = new IdentityPerson
            {
                Name = "Manager",
                UserName = "ManagerManager",
                Email = "Manager@test.com",
                IdNumber = "12345678906",
                Lastname = "ManagerManager",
                BirthDate = DateTime.Now,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var test = await _userManager.CreateAsync(identityPerson, "P@ssword!@#$5");
            await _userManager.AddToRoleAsync(identityPerson, "Manager");
        }
    }
}