using TestApp1.Contracts.Common;
using TestApp1.Contracts.Customers;

namespace TestApp1.Services.Interfaces;

public interface ICustomerService
{
    Task<PagedResponse<CustomerResponse>> GetAllAsync(int page, int pageSize);
    Task<CustomerResponse?> GetByIdAsync(Guid id);
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request);
    Task<bool> DeleteAsync(Guid id);
}