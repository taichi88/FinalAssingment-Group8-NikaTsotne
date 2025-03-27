// BankingSystem.Application/Helpers/CardSecurityHelper.cs
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


    public static string GenerateCardNumber()
    {
        Random random = new Random();
        int[] digits = new int[16];

        for (int i = 0; i < 15; i++)
        {
            digits[i] = random.Next(0, 10);
        }

        int sum = 0;
        bool alternate = true; 
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

        int checkDigit = (10 - (sum % 10)) % 10;
        digits[15] = checkDigit;

        return string.Join("", digits);
    }

    public static string GenerateCvv()
    {
        var random = new Random();
        return random.Next(100, 1000).ToString();
    }

    public static string GeneratePinCode()
    {
        var random = new Random();
        return random.Next(1000, 10000).ToString();
    }

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using var aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32, '0').Substring(0, 32));

        using var hmac = new HMACSHA256(aes.Key);
        byte[] ivHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        byte[] iv = new byte[16]; 
        Array.Copy(ivHash, iv, iv.Length);
        aes.IV = iv;

        using var ms = new MemoryStream();

        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string HashPinCode(string pinCode)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pinCode));
        return Convert.ToBase64String(bytes);
    }
}
