namespace BankingSystem.Infrastructure.DataSeeding.Transactions;

public class CurrencyConversionHelper
{
    public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, 
        Dictionary<string, decimal> rates)
    {
        // No conversion needed if currencies are the same
        if (fromCurrency == toCurrency)
            return amount;

        // Base currency for the system is GEL
        const string baseCurrency = "GEL";

        // Conversion logic - simplified version of what's in AccountTransactionService
        // 1. Converting from base currency to another
        if (fromCurrency == baseCurrency)
            return amount / rates[toCurrency];

        // 2. Converting to base currency
        if (toCurrency == baseCurrency)
            return amount * rates[fromCurrency];

        // 3. Cross-currency conversion (through base currency)
        return amount * (rates[fromCurrency] / rates[toCurrency]);
    }
}
