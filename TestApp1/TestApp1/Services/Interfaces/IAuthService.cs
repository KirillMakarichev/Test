using TestApp1.Contracts.Auth;

namespace TestApp1.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}