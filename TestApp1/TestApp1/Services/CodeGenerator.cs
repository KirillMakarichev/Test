using TestApp1.Services.Interfaces;

namespace TestApp1.Services;

public sealed class CodeGenerator : ICodeGenerator
{
    private readonly Random _random = new Random();
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";

    public string GenerateCustomerCode(DateTime registrationDate)
    {
        return $"{GenerateString(Digits, 4)}-{registrationDate.Year}";
    }

    public string GenerateProductCode()
    {
        return $"{GenerateString(Digits, 2)}-{GenerateString(Digits, 4)}-{GenerateString(Letters, 2)}{GenerateString(Digits, 2)}";
    }

    private string GenerateString(string alphabet, int length)
    {
        return new string(Enumerable.Repeat(alphabet, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}