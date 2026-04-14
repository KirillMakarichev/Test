namespace TestApp1.Database.Models;

public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Customer> Customers { get; set; } = [];
}
