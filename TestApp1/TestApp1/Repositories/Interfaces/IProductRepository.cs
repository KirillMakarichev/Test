using TestApp1.Database.Models;

namespace TestApp1.Repositories.Interfaces;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds);
    Task<bool> ExistsByCodeAsync(string code);
    Task AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid productId);
}
