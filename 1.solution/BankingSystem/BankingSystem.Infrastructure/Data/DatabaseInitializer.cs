using BankingSystem.Infrastructure.Data.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data;
using BankingSystem.Infrastructure.DataSeeding;
using Dapper;

namespace BankingSystem.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly TestDataSeeder _seeder;
    private readonly IDbConnection _connection;
    private readonly BankingSystemDbContext _dbContext;

    public DatabaseInitializer(
        BankingSystemDbContext dbContext,
        IConfiguration configuration,
        ILogger<DatabaseInitializer> logger,
        TestDataSeeder seeder,
        IDbConnection connection)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
        _seeder = seeder;
        _connection = connection;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");

            bool databaseExists = await CheckDatabaseExistsAsync();
            _logger.LogInformation("Database exists: {databaseExists}", databaseExists);

            //await _dbContext.Database.EnsureCreatedAsync();

            if (!databaseExists)
            {
                await ApplyMigrationsAsync();
                await ExecuteSqlScriptsAsync();
                await _seeder.SeedAsync();
            }
            _logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }

    private async Task<bool> CheckDatabaseExistsAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        var exists = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName",
            new { dbName = databaseName }) > 0;

        return exists;
    }

    private async Task ApplyMigrationsAsync()
    {
        _logger.LogInformation("Applying pending migrations...");
        await _dbContext.Database.MigrateAsync();
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        _logger.LogInformation("Executing SQL scripts for tables and stored procedures...");

        await ExecuteScriptFromFile("Tables/Accounts.sql");
        await ExecuteScriptFromFile("Tables/Cards.sql");
        await ExecuteScriptFromFile("Tables/Transactions.sql");

        await ExecuteScriptFromFile("Stored Procedures/AtmWithdrawalStatistics/AtmWithdrawalStatistics.sql");
        await ExecuteScriptFromFile("Stored Procedures/MonthlyTransactionBreakdown/MonthlyTransactionBreakdown.sql");

        await ExecuteScriptFromFile("Stored Procedures/TransactionIncomeByType/sp_GetAverageTransactionIncome.sql");
        await ExecuteScriptFromFile("Stored Procedures/TransactionIncomeByType/sp_GetTransactionIncomeLast6Months.sql");
        await ExecuteScriptFromFile("Stored Procedures/TransactionIncomeByType/sp_GetTransactionIncomeLastMonth.sql");
        await ExecuteScriptFromFile("Stored Procedures/TransactionIncomeByType/sp_GetTransactionIncomeLastYear.sql");

        await ExecuteScriptFromFile("Stored Procedures/TransactionStatistics/sp_GetTransactionCountLast6Months.sql");
        await ExecuteScriptFromFile("Stored Procedures/TransactionStatistics/sp_GetTransactionCountLastMonth.sql");
        await ExecuteScriptFromFile("Stored Procedures/TransactionStatistics/sp_GetTransactionCountLastYear.sql");

        await ExecuteScriptFromFile("Stored Procedures/UserStatistics/sp_GetUserCountLast30Days.sql");
        await ExecuteScriptFromFile("Stored Procedures/UserStatistics/sp_GetUserCountLastYear.sql");
        await ExecuteScriptFromFile("Stored Procedures/UserStatistics/sp_GetUserCountThisYear.sql");
    }

    private async Task ExecuteScriptFromFile(string scriptPath)
    {
        try
        {
            var fullPath = Path.Combine(Environment.CurrentDirectory,
                @"..\BankingSystem.Infrastructure\Query", scriptPath);

            if (File.Exists(fullPath))
            {
                var script = await File.ReadAllTextAsync(fullPath);
                await _connection.ExecuteAsync(script);
                _logger.LogInformation("Executed script: {scriptPath}", scriptPath);
            }
            else
            {
                _logger.LogWarning("Script not found: {scriptPath}", scriptPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing script {scriptPath}", scriptPath);
            throw;
        }
    }
}
