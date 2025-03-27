using BankingSystem.Application;
using BankingSystem.Application.Helpers;
using BankingSystem.Configure;
using BankingSystem.Domain;
using BankingSystem.Infrastructure;
using BankingSystem.Infrastructure.Data;
using BankingSystem.Infrastructure.Data.DatabaseContext;
using BankingSystem.Middleware;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddMemoryCache();

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

CardSecurityHelper.Initialize(app.Configuration);
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
await app.Services.InitializeDatabaseAsync();

app.Run();
