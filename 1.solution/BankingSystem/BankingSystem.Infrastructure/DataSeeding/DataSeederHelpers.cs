namespace BankingSystem.Infrastructure.DataSeeding;

public static class DataSeederHelpers
{
    public static string GenerateIban() => 
        $"GE{new Random().Next(100000000, 999999999)}{new Random().Next(100000000, 999999999)}";
    
    public static string GenerateCardNumber() =>
        $"{new Random().Next(10000000, 99999999)}{new Random().Next(10000000, 99999999)}";
    
    public static string GenerateCvv() => 
        new Random().Next(100, 999).ToString();
    
    public static string GenerateIdNumber() => 
        $"{new Random().Next(100000000, 999999999)}{new Random().Next(10, 99)}";
}
