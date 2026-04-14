using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TestApp1.Contracts.Auth;
using TestApp1.Repositories.Interfaces;
using TestApp1.Services.Interfaces;

namespace TestApp1.Services;

public sealed class AuthService(
    ICustomerRepository customerRepository,
    IPasswordHasher passwordHasher,
    IConfiguration configuration) : IAuthService
{
    private string GetRequiredJwtKey()
    {
        return configuration["Jwt:Key"]
               ?? throw new InvalidOperationException("Missing required configuration key: Jwt:Key");
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var customer = await customerRepository.GetByFilterAsync(request.Name.Trim());
        if (customer is null)
        {
            return null;
        }

        var hash = passwordHasher.Hash(request.Password);
        if (!string.Equals(hash, customer.PasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var key = GetRequiredJwtKey();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new(ClaimTypes.Role, customer.Role.Name)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            Role = customer.Role.Name
        };
    }
}
