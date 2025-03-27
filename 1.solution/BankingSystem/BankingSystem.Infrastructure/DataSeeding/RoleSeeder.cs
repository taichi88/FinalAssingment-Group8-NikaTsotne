using BankingSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Infrastructure.DataSeeding;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleSeeder(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        // Get all enum values and convert to string representations
        var roles = Enum.GetValues<RoleType>().Select(r => r.ToString());

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
