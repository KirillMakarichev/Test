namespace TestApp1.Contracts.Orders;

public sealed class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int ItemsCount { get; set; }
    public decimal ItemPrice { get; set; }
}

public sealed class OrderResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ShipmentDate { get; set; }
    public int OrderNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = [];
}
