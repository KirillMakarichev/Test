using System.ComponentModel.DataAnnotations;

namespace TestApp1.Contracts.Orders;

public sealed class CreateOrderRequest
{
    [Range(typeof(DateTime), "2000-01-01", "9999-12-31")]
    public DateTime OrderDate { get; set; }

    [Range(typeof(DateTime), "2000-01-01", "9999-12-31")]
    public DateTime ShipmentDate { get; set; }

    [Range(1, int.MaxValue)]
    public int OrderNumber { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public sealed class CreateOrderItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, 100000)]
    public int ItemsCount { get; set; }
}
