namespace TestApp1.Database.Models;

public sealed class Customer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Discount { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
}
