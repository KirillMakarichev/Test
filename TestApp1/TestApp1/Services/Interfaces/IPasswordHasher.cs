namespace TestApp1.Services.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
}