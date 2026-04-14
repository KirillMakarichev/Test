using TestApp1.Database.Models;

namespace TestApp1.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<(List<Order> Items, int TotalCount)> GetByCustomerAsync(Guid customerId, int page, int pageSize);
    Task<(List<Order> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<(List<Order> Items, int TotalCount)> GetByStatusAsync(OrderStatus status, int page, int pageSize);
    Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus targetStatus, DateTime? shipmentDate = null);
    Task<bool> DeleteAsync(Guid orderId);
}
