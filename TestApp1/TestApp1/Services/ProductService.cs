using TestApp1.Contracts.Common;
using TestApp1.Contracts.Products;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;
using TestApp1.Services.Interfaces;

namespace TestApp1.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    ICodeGenerator codeGenerator) : IProductService
{
    public async Task<PagedResponse<ProductResponse>> GetAllAsync(int page, int pageSize)
    {
        var (products, totalCount) = await productRepository.GetAllAsync(page, pageSize);
        return PagedResponseFactory.Create(products, totalCount, page, pageSize, MapToResponse);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var products = await productRepository.GetByIdsAsync([id]);
        var product = products.FirstOrDefault();
        return product is null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var code = await GenerateProductCodeAsync();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Price = decimal.Round(request.Price, 2),
            Category = request.Category.Trim(),
            Code = code
        };

        await productRepository.AddAsync(product);
        return MapToResponse(product);
    }

    public Task<bool> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        return productRepository.UpdateAsync(new Product
        {
            Id = id,
            Name = request.Name.Trim(),
            Price = decimal.Round(request.Price, 2),
            Category = request.Category.Trim(),
            Code = request.Code.Trim()
        });
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return productRepository.DeleteAsync(id);
    }

    private async Task<string> GenerateProductCodeAsync()
    {
        var attempts = 0;
        while (attempts < 10)
        {
            var code = codeGenerator.GenerateProductCode();
            if (!await productRepository.ExistsByCodeAsync(code))
            {
                return code;
            }

            attempts++;
        }

        throw new InvalidOperationException("Unable to generate unique product code");
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Code = product.Code
        };
    }
}
