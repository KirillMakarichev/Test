using System.ComponentModel.DataAnnotations;

namespace TestApp1.Contracts.Customers;

public sealed class CreateCustomerRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(64, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 100)]
    public double Discount { get; set; }

    [Required]
    [RegularExpression("^(customer|manager)$", ErrorMessage = "Role must be either 'customer' or 'manager'.")]
    public string Role { get; set; } = "customer";
}
