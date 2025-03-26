using BankingSystem.Application.Constants;
using BankingSystem.Application.IServices;
using BankingSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSystem.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IPersonAuthService, PersonAuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IAccountTransactionService, AccountTransactionService>();
        services.AddScoped<IAtmService, AtmService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAtmTokenService, AtmTokenService>();
        services.AddScoped<IReportService, ReportService>();
        
        // Register configuration constants
        services.AddSingleton<AtmConstants>();
        services.AddSingleton<TransactionConstants>();

        return services;
    }
}
