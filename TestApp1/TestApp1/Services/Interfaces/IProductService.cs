using TestApp1.Contracts.Common;
using TestApp1.Contracts.Products;

namespace TestApp1.Services.Interfaces;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetAllAsync(int page, int pageSize);
    Task<ProductResponse?> GetByIdAsync(Guid id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteAsync(Guid id);
}