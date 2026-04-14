using Microsoft.EntityFrameworkCore;
using TestApp1.Database;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;

namespace TestApp1.Repositories;

public sealed class ProductRepository(ShopDbContext dbContext) : IProductRepository
{
    public async Task<(List<Product> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name);

        return await query.ToPagedResultAsync(page, pageSize);
    }
    
    public Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds)
    {
        return dbContext.Products
            .AsNoTracking()
            .Where(x => productIds.Contains(x.Id))
            .ToListAsync();
    }

    public Task<bool> ExistsByCodeAsync(string code)
    {
        return dbContext.Products.AnyAsync(x => x.Code == code);
    }

    public async Task AddAsync(Product product)
    {
        if (product.Id == Guid.Empty)
        {
            product.Id = Guid.NewGuid();
        }

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Category = product.Category;
        existing.Code = product.Code;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid productId)
    {
        var existing = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (existing is null)
        {
            return false;
        }

        dbContext.Products.Remove(existing);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
