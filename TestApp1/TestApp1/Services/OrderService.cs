using TestApp1.Contracts.Common;
using TestApp1.Contracts.Orders;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;
using TestApp1.Services.Interfaces;

namespace TestApp1.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository) : IOrderService
{
    public async Task<PagedResponse<OrderResponse>> GetAllAsync(int page, int pageSize)
    {
        var (items, totalCount) = await orderRepository.GetAllAsync(page, pageSize);
        return PagedResponseFactory.Create(items, totalCount, page, pageSize, MapToResponse);
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            return null;
        }

        return MapToResponse(order);
    }

    public async Task<PagedResponse<OrderResponse>> GetByCustomerAsync(Guid customerId, int page, int pageSize)
    {
        var (items, totalCount) = await orderRepository.GetByCustomerAsync(customerId, page, pageSize);
        return PagedResponseFactory.Create(items, totalCount, page, pageSize, MapToResponse);
    }

    public async Task<PagedResponse<OrderResponse>> GetByStatusAsync(OrderStatus status, int page, int pageSize)
    {
        var (items, totalCount) = await orderRepository.GetByStatusAsync(status, page, pageSize);
        return PagedResponseFactory.Create(items, totalCount, page, pageSize, MapToResponse);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid customerId)
    {
        var customer = await customerRepository.GetByFilterAsync(customerId: customerId)
            ?? throw new InvalidOperationException("Customer not found");

        var discount = Math.Clamp(customer.Discount, 0, 100);
        var discountRate = (decimal)discount / 100m;

        var items = new List<OrderItem>();
        var productIds = request.Items.Select(x => x.ProductId).ToHashSet();
        var products =
            (await productRepository.GetByIdsAsync(productIds))
            .ToDictionary(x => x.Id, x => x);
        
        foreach (var requestItem in request.Items)
        {
            if (!products.TryGetValue(requestItem.ProductId, out var product))
            {
                continue;
            }
            
            items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ItemsCount = requestItem.ItemsCount,
                ItemPrice = decimal.Round(product.Price * (1m - discountRate), 2, MidpointRounding.AwayFromZero)
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Created,
            OrderDate = request.OrderDate == default ? DateTime.UtcNow : request.OrderDate,
            ShipmentDate = request.ShipmentDate == default ? DateTime.UtcNow.AddDays(1) : request.ShipmentDate,
            OrderNumber = request.OrderNumber,
            Items = items
        };

        var created = await orderRepository.CreateAsync(order);
        return MapToResponse(created);
    }

    public Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus targetStatus, DateTime? shipmentDate = null)
    {
        if (targetStatus == OrderStatus.Processing)
        {
            var normalizedShipmentDate = shipmentDate is null || shipmentDate == default
                ? DateTime.UtcNow.AddDays(1)
                : shipmentDate;

            return orderRepository.UpdateStatusAsync(orderId, targetStatus, normalizedShipmentDate);
        }

        return orderRepository.UpdateStatusAsync(orderId, targetStatus);
    }

    public Task<bool> DeleteAsync(Guid orderId)
    {
        return orderRepository.DeleteAsync(orderId);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            ShipmentDate = order.ShipmentDate,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            Items = order.Items.Select(x => new OrderItemResponse
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ItemsCount = x.ItemsCount,
                ItemPrice = x.ItemPrice
            }).ToList()
        };
    }
}