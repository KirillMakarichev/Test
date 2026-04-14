using Microsoft.EntityFrameworkCore;
using TestApp1.Database;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;

namespace TestApp1.Repositories;

public sealed class OrderRepository(ShopDbContext dbContext) : IOrderRepository
{
    private IQueryable<Order> QueryOrdersWithDetails()
    {
        return dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Items);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        if (order.Id == Guid.Empty)
        {
            order.Id = Guid.NewGuid();
        }

        foreach (var item in order.Items)
        {
            if (item.Id == Guid.Empty)
            {
                item.Id = Guid.NewGuid();
            }

            item.OrderId = order.Id;
        }

        if (order.OrderDate == default)
        {
            order.OrderDate = DateTime.UtcNow;
        }

        if (order.ShipmentDate == default)
        {
            order.ShipmentDate = order.OrderDate.AddDays(1);
        }

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();
        return order;
    }

    public Task<Order?> GetByIdAsync(Guid orderId)
    {
        return QueryOrdersWithDetails()
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public Task<(List<Order> Items, int TotalCount)> GetByCustomerAsync(Guid customerId, int page, int pageSize)
    {
        var query = QueryOrdersWithDetails()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.OrderDate);

        return query.ToPagedResultAsync(page, pageSize);
    }

    public Task<(List<Order> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = QueryOrdersWithDetails()
            .OrderByDescending(x => x.OrderDate);

        return query.ToPagedResultAsync(page, pageSize);
    }

    public Task<(List<Order> Items, int TotalCount)> GetByStatusAsync(OrderStatus status, int page, int pageSize)
    {
        var query = QueryOrdersWithDetails()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.OrderDate);

        return query.ToPagedResultAsync(page, pageSize);
    }

    public async Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus targetStatus, DateTime? shipmentDate = null)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order is null)
        {
            return false;
        }

        order.Status = targetStatus;
        if (shipmentDate.HasValue)
        {
            order.ShipmentDate = shipmentDate.Value;
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid orderId)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order is null)
        {
            return false;
        }

        if (order.Status != OrderStatus.Created)
        {
            throw new InvalidOperationException("Order can be deleted only before processing starts.");
        }

        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
