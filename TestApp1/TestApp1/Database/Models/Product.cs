namespace TestApp1.Database.Models;

public sealed class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public List<OrderItem> OrderItems { get; set; } = [];
}
