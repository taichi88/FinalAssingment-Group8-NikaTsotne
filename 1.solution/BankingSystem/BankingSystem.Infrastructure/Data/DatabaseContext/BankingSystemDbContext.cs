using BankingSystem.Application.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Data.DatabaseContext;

public class BankingSystemDbContext(DbContextOptions<BankingSystemDbContext> options)
    : IdentityDbContext<IdentityPerson>(options)
{
    public DbSet<IdentityPerson> IdentityPersons { get; set; }
}
