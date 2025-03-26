using BankingSystem.Application.Identity;
using BankingSystem.Domain.ExternalApiContracts;
using BankingSystem.Domain.IRepository;
using BankingSystem.Domain.IUnitOfWork;
using BankingSystem.Infrastructure.Data.DatabaseContext;
using BankingSystem.Infrastructure.Data.ExternalApis;
using BankingSystem.Infrastructure.DataSeeding;
using BankingSystem.Infrastructure.Repository;
using BankingSystem.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using BankingSystem.Infrastructure.Data;

namespace BankingSystem.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration
        services.AddDbContext<BankingSystemDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly("BankingSystem.Infrastructure")));

        // Identity configuration
        services.AddIdentity<IdentityPerson, IdentityRole>()
            .AddEntityFrameworkStores<BankingSystemDbContext>()
            .AddDefaultTokenProviders();

        // Database connection
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

        // Register repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<IAccountTransactionRepository, TransactionRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        
        // Register unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        
        // Register external services
        services.AddScoped<IExchangeRateApi, ExchangeRateApi>();
        
        // Register data seeders and initializers
        services.AddTransient<TestDataSeeder>();
        services.AddTransient<DatabaseInitializer>();

        // Register HTTP client for external APIs
        services.AddHttpClient();

        // Configure JWT Authentication
        ConfigureJwtAuthentication(services, configuration);

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        // Create a new scope to retrieve scoped services
        using var scope = serviceProvider.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await dbInitializer.InitializeAsync();
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        var jwtIssuer = configuration["Jwt:Issuer"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(3)
            };
        });
    }
}
