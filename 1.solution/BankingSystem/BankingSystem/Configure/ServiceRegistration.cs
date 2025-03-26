using BankingSystem.Application.Identity;
using BankingSystem.Application.IServices;
using BankingSystem.Application.Services;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.ExternalApiContracts;
using BankingSystem.Domain.IRepository;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Filters;
using BankingSystem.Infrastructure.Data.DatabaseContext;
using BankingSystem.Infrastructure.Data.ExternalApis;
using BankingSystem.Infrastructure.Repository;
using BankingSystem.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

namespace BankingSystem.Configure;

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
        // Add this line in AddApplicationServices method
        services.AddScoped<IAtmTokenService, AtmTokenService>();


        // Register services
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IReportRepository, ReportRepository>();


        services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Disable automatic 400 response for model validation failures
                options.SuppressModelStateInvalidFilter = true;
            });

        services.AddHttpClient();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
    }
}