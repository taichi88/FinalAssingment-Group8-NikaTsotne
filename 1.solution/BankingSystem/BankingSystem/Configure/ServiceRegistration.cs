using BankingSystem.Application.Identity;
using BankingSystem.Application.Identity;
using BankingSystem.Application.IServices;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.ExternalApiContracts;
using BankingSystem.Domain.IRepository;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Infrastructure.Data.DatabaseContext;
using BankingSystem.Infrastructure.Data.ExternalApis;
using BankingSystem.Infrastructure.Repository;
using BankingSystem.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace InternetBank.UI.Configure;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddIdentity<IdentityPerson, IdentityRole>()
            .AddEntityFrameworkStores<BankingSystemDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IPersonAuthService, PersonAuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IAccountTransactionService, AccountTransactionService>();
        services.AddScoped<IAccountTransactionRepository, TransactionRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IExchangeRateApi, ExchangeRateApi>();
        services.AddScoped<IAtmService, AtmService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddHttpClient();
    }
}