namespace TestApp1.Database.Models;

public sealed class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product ItemId { get; set; } = null!;

    public int ItemsCount { get; set; }
    public decimal ItemPrice { get; set; }
}
