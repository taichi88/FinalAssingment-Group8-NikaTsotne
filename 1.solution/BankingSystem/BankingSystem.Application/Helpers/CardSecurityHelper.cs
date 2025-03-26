﻿// BankingSystem.Application/Helpers/CardSecurityHelper.cs
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.Helpers;

public static class CardSecurityHelper
{
    private static string EncryptionKey => GetEncryptionKey();
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private static string GetEncryptionKey()
    {
        if (_configuration == null)
        {
            throw new InvalidOperationException("CardSecurityHelper has not been initialized with configuration. Call Initialize method first.");
        }

        string key = _configuration["Security:CardEncryptionKey"];
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("Card encryption key not found in configuration");
        }

        return key;
    }

    // Generate a card number using Luhn algorithm
    /// <summary>
    /// Generates a valid card number that passes the Luhn check
    /// </summary>
    /// <param name="bin">Bank Identification Number (BIN) prefix, defaults to "4000" (Visa test cards)</param>
    /// <returns>A valid 16-digit card number</returns>
// Generate a random 16-digit credit card number that passes the Luhn algorithm.
    public static string GenerateCardNumber()
    {
        Random random = new Random();
        int[] digits = new int[16];

        // Generate the first 15 random digits
        for (int i = 0; i < 15; i++)
        {
            digits[i] = random.Next(0, 10);
        }

        // Calculate the check digit for the first 15 digits.
        // For a 16-digit card, the Luhn algorithm doubles every second digit from the right.
        int sum = 0;
        bool alternate = true; // start doubling from index 14 (the rightmost digit of the first 15)
        for (int i = 14; i >= 0; i--)
        {
            int digit = digits[i];
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
            alternate = !alternate;
        }

        // The check digit is what you need to add to the sum to get a multiple of 10.
        int checkDigit = (10 - (sum % 10)) % 10;
        digits[15] = checkDigit;

        // Convert the digits array to a string.
        return string.Join("", digits);
    }

    // Luhn algorithm to validate a credit card number.
    public static bool IsValidCreditCard(string cardNumber)
    {
        if (cardNumber.Length != 16 || !ulong.TryParse(cardNumber, out _))
        {
            return false;
        }

        int sum = 0;
        bool alternate = false;
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
            alternate = !alternate;
        }
        return (sum % 10 == 0);
    }

    // Generate a random CVV number (3 digits)
    public static string GenerateCvv()
    {
        var random = new Random();
        return random.Next(100, 1000).ToString();
    }

    // Generate a random PIN code (4 digits)
    public static string GeneratePinCode()
    {
        var random = new Random();
        return random.Next(1000, 10000).ToString();
    }

    // Encrypt sensitive data
    // Encrypt sensitive data with a deterministic approach using a derived IV
    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using var aes = Aes.Create();

        // Create a deterministic key
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32, '0').Substring(0, 32));

        // Create a deterministic IV based on plainText and key
        // This approach ensures the same plainText always generates the same ciphertext
        using var hmac = new HMACSHA256(aes.Key);
        byte[] ivHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        byte[] iv = new byte[16]; // AES block size
        Array.Copy(ivHash, iv, iv.Length);
        aes.IV = iv;

        // Perform encryption
        using var ms = new MemoryStream();

        // Still include the IV in the output for verification
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }


    // Decrypt sensitive data
    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32, '0').Substring(0, 32));

        // Extract IV from the beginning of the ciphertext
        byte[] iv = new byte[16]; // AES block size is 16 bytes
        Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
        aes.IV = iv;

        // Decrypt the actual ciphertext (after the IV)
        using var ms = new MemoryStream();
        using var decryptStream = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
        using var cs = new CryptoStream(decryptStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    // Hash PIN code
    public static string HashPinCode(string pinCode)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pinCode));
        return Convert.ToBase64String(bytes);
    }
}
