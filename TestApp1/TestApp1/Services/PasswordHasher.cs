using System.Security.Cryptography;
using System.Text;
using TestApp1.Services.Interfaces;

namespace TestApp1.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
