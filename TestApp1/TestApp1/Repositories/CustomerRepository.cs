using Microsoft.EntityFrameworkCore;
using TestApp1.Database;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;

namespace TestApp1.Repositories;

public sealed class CustomerRepository(ShopDbContext dbContext) : ICustomerRepository
{
    public async Task<(List<Customer> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = dbContext.Customers
            .AsNoTracking()
            .Include(x => x.Role)
            .OrderBy(x => x.Name);

        return await query.ToPagedResultAsync(page, pageSize);
    }
    
    public Task<Customer?> GetByFilterAsync(string? name = null, Guid? customerId = null, string? code = null)
    {
        var customers = dbContext.Customers
            .AsNoTracking()
            .Include(x => x.Role);

        if (!string.IsNullOrWhiteSpace(name))
        {
            return customers.FirstOrDefaultAsync(x => x.Name == name);
        }

        if (customerId.HasValue && customerId != Guid.Empty)
        {
            return customers.FirstOrDefaultAsync(x => x.Id == customerId);
        }
        
        if (!string.IsNullOrWhiteSpace(code))
        {
            return customers.FirstOrDefaultAsync(x => x.Name == name);
        }
        
        return null;
    }

    public Task<bool> ExistsByCodeAsync(string code)
    {
        return dbContext.Customers.AnyAsync(x => x.Code == code);
    }

    public async Task AddAsync(Customer customer)
    {
        if (customer.Id == Guid.Empty)
        {
            customer.Id = Guid.NewGuid();
        }

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var existing = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customer.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Name = customer.Name;
        existing.PasswordHash = customer.PasswordHash;
        existing.Code = customer.Code;
        existing.Address = customer.Address;
        existing.Discount = customer.Discount;
        existing.RoleId = customer.RoleId;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid customerId)
    {
        var existing = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        if (existing is null)
        {
            return false;
        }

        dbContext.Customers.Remove(existing);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
