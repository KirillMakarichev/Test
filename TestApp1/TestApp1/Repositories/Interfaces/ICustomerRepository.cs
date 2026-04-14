using TestApp1.Database.Models;

namespace TestApp1.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<(List<Customer> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Customer?> GetByFilterAsync(string? name = null, Guid? customerId = null, string? code = null);
    Task AddAsync(Customer customer);
    Task<bool> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(Guid customerId);
}
