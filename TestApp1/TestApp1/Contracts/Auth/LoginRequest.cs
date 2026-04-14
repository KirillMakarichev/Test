using System.ComponentModel.DataAnnotations;

namespace TestApp1.Contracts.Auth;

public sealed class LoginRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
