using BankingSystem.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Infrastructure.DataSeeding;

public class UserSeeder
{
    private readonly UserManager<IdentityPerson> _userManager;

    public UserSeeder(UserManager<IdentityPerson> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityPerson> SeedManagerUserAsync()
    {
        if (await _userManager.FindByNameAsync("ManagerManager") is null)
        {
            var manager = new IdentityPerson
            {
                Name = "Manager",
                UserName = "ManagerManager",
                Email = "manager@test.com",
                IdNumber = "12345678906",
                Lastname = "ManagerSurname",
                BirthDate = new DateTime(1980, 1, 1),
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _userManager.CreateAsync(manager, "P@ssword!@#$5");
            await _userManager.AddToRoleAsync(manager, "Manager");
            return manager;
        }

        return await _userManager.FindByNameAsync("ManagerManager")!;
    }

    public async Task<IdentityPerson> SeedOperatorUserAsync()
    {
        if (await _userManager.FindByNameAsync("OperatorUser") is null)
        {
            var operatorUser = new IdentityPerson
            {
                Name = "Operator",
                UserName = "OperatorUser",
                Email = "operator@test.com",
                IdNumber = "12345678907",
                Lastname = "OperatorSurname",
                BirthDate = new DateTime(1985, 2, 15),
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _userManager.CreateAsync(operatorUser, "P@ssword!@#$5");
            await _userManager.AddToRoleAsync(operatorUser, "Operator");
            return operatorUser;
        }

        return await _userManager.FindByNameAsync("OperatorUser")!;
    }

    public async Task<List<IdentityPerson>> SeedPersonUsersAsync(int count)
    {
        var persons = new List<IdentityPerson>();

        for (int i = 1; i <= count; i++)
        {
            var username = $"Person{i}User";

            if (await _userManager.FindByNameAsync(username) is null)
            {
                var person = new IdentityPerson
                {
                    Name = $"Person{i}",
                    UserName = username,
                    Email = $"person{i}@test.com",
                    IdNumber = DataSeederHelpers.GenerateIdNumber(),
                    Lastname = $"Person{i}Surname",
                    BirthDate = new DateTime(1990, i, 10),
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
                };

                await _userManager.CreateAsync(person, "P@ssword!@#$5");
                await _userManager.AddToRoleAsync(person, "Person");
                persons.Add(person);
            }
            else
            {
                persons.Add(await _userManager.FindByNameAsync(username)!);
            }
        }

        return persons;
    }
}
