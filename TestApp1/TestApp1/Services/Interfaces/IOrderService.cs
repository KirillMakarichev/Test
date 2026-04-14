using TestApp1.Contracts.Common;
using TestApp1.Contracts.Orders;
using TestApp1.Database.Models;

namespace TestApp1.Services.Interfaces;

public interface IOrderService
{
    Task<PagedResponse<OrderResponse>> GetAllAsync(int page, int pageSize);
    Task<OrderResponse?> GetByIdAsync(Guid id);
    Task<PagedResponse<OrderResponse>> GetByCustomerAsync(Guid customerId, int page, int pageSize);
    Task<PagedResponse<OrderResponse>> GetByStatusAsync(OrderStatus status, int page, int pageSize);
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid customerId);
    Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus targetStatus, DateTime? shipmentDate = null);
    Task<bool> DeleteAsync(Guid orderId);
}