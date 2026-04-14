namespace TestApp1.Contracts.Customers;

public sealed class CustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Discount { get; set; }
    public string Role { get; set; } = string.Empty;
}
