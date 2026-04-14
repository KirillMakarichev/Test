namespace TestApp1.Contracts.Auth;

public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
