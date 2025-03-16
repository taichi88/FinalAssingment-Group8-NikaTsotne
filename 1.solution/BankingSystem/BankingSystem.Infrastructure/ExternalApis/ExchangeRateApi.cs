using System.Text.Json;
using BankingSystem.Domain.ExternalApiContracts;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Infrastructure.Data.ExternalApis;

public class ExchangeRateApi(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IExchangeRateApi
{
    private readonly string _baseUrl = configuration["ExchangeRateApi:BaseUrl"];

    public async Task<decimal> GetExchangeRate(string currency)
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_baseUrl}{currency}");

        var client = httpClientFactory.CreateClient();
        var httpResponseMessage = await client.SendAsync(httpRequestMessage);

        IEnumerable<CurrencyResponse>? currencyResponses = new List<CurrencyResponse>();

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            currencyResponses = await JsonSerializer.DeserializeAsync<IEnumerable<CurrencyResponse>>(contentStream);
        }

        return currencyResponses!.First().Currencies!.First().Rate;
    }
}