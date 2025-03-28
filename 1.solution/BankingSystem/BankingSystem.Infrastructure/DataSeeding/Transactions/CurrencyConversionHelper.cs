namespace BankingSystem.Infrastructure.DataSeeding.Transactions;

public class CurrencyConversionHelper
{
    public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, 
        Dictionary<string, decimal> rates)
    {
        if (fromCurrency == toCurrency)
            return amount;

        const string baseCurrency = "GEL";

        if (fromCurrency == baseCurrency)
            return amount / rates[toCurrency];

        if (toCurrency == baseCurrency)
            return amount * rates[fromCurrency];

        return amount * (rates[fromCurrency] / rates[toCurrency]);
    }
}
