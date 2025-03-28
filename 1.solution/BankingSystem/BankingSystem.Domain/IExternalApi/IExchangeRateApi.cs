namespace BankingSystem.Domain.IExternalApi;

public interface IExchangeRateApi
{
    Task<decimal> GetExchangeRate(string currency);
}