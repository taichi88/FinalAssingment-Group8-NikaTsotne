using System.Text.Json;
using BankingSystem.Domain.IExternalApi;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Infrastructure.Data.ExternalApis;

public class ExchangeRateApi : IExchangeRateApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly string _baseUrl;
    private readonly TimeSpan _cacheExpirationTime;

    public ExchangeRateApi(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _memoryCache = memoryCache;
        _baseUrl = _configuration["ExchangeRateApi:BaseUrl"];
        
        // Default to 1 hour if not specified in configuration
        int cacheMinutes = _configuration.GetValue<int>("ExchangeRateApi:CacheExpirationMinutes", 60);
        _cacheExpirationTime = TimeSpan.FromMinutes(cacheMinutes);
    }

    public async Task<decimal> GetExchangeRate(string currency)
    {
        // Generate cache key based on currency
        string cacheKey = $"ExchangeRate_{currency}";

        // Try to get the rate from cache
        if (_memoryCache.TryGetValue(cacheKey, out decimal cachedRate))
        {
            return cachedRate;
        }

        // If not in cache, fetch from API
        var rate = await FetchExchangeRateFromApiAsync(currency);

        // Store in cache with expiration
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheExpirationTime);
        
        _memoryCache.Set(cacheKey, rate, cacheEntryOptions);

        return rate;
    }

    private async Task<decimal> FetchExchangeRateFromApiAsync(string currency)
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_baseUrl}{currency}");

        var client = _httpClientFactory.CreateClient();
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