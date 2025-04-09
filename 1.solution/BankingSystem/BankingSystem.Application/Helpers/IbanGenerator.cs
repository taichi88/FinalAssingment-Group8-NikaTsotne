using System;
using System.Numerics;
using System.Text;

namespace BankingSystem.Application.Helpers;

public static class IbanGenerator
{
    private static readonly Random _random = new();

    /// <summary>
    /// Generates a valid IBAN that passes the IbanValidationAttribute check.
    /// For Georgia (GE), the format is: GE + 2-digit check + 2-letter bank code + 16 digits
    /// </summary>
    /// <param name="countryCode">Optional country code (default: GE for Georgia)</param>
    /// <returns>A valid IBAN string</returns>
    public static string GenerateValidIban(string countryCode = "GE")
    {
        // Validate country code
        if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
        {
            throw new ArgumentException("Country code must be exactly 2 letters", nameof(countryCode));
        }

        countryCode = countryCode.ToUpper();

        // Generate BBAN (Basic Bank Account Number)
        // For Georgia (GE), the format is: 2-letter bank code + 16 digits (total 18 characters)
        StringBuilder bban = new();

        // Add 2 letters for bank code (e.g., BG for Bank of Georgia)
        string[] commonBankCodes = { "BG", "TB", "LB", "CR", "PC" }; // Common Georgian bank codes
        bban.Append(commonBankCodes[_random.Next(commonBankCodes.Length)]);

        // Add 16 digits for the account number
        for (int i = 0; i < 16; i++)
        {
            bban.Append(_random.Next(0, 10));
        }

        // Calculate proper check digits
        string checkDigits = CalculateIbanCheckDigits(countryCode, bban.ToString());

        // Combine all parts for the final valid IBAN
        return countryCode + checkDigits + bban.ToString();
    }

    /// <summary>
    /// Calculates the check digits for an IBAN
    /// </summary>
    /// <param name="countryCode">Two-letter country code</param>
    /// <param name="bban">Basic Bank Account Number</param>
    /// <returns>Two-digit check string</returns>
    private static string CalculateIbanCheckDigits(string countryCode, string bban)
    {
        // Move country code and placeholder check digits to the end
        string rearranged = bban + countryCode + "00";

        // Convert letters to numbers (A=10, B=11, ..., Z=35)
        StringBuilder numericIban = new();
        foreach (char c in rearranged)
        {
            if (char.IsLetter(c))
                numericIban.Append((c - 'A' + 10).ToString());
            else
                numericIban.Append(c);
        }

        // Calculate the MOD 97 operation
        // Subtract remainder from 98 to get the check digits
        BigInteger numeric = BigInteger.Parse(numericIban.ToString());
        int remainder = (int)(98 - numeric % 97);

        // Format check digits as two digits with leading zero if needed
        return remainder.ToString("D2");
    }
}
