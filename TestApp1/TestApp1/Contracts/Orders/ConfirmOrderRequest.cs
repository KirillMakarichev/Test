using System.ComponentModel.DataAnnotations;

namespace TestApp1.Contracts.Orders;

public sealed class ConfirmOrderRequest
{
    [Range(typeof(DateTime), "2000-01-01", "9999-12-31")]
    public DateTime ShipmentDate { get; set; }
}