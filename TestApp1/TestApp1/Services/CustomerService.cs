using TestApp1.Contracts.Common;
using TestApp1.Contracts.Customers;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;
using TestApp1.Services.Interfaces;

namespace TestApp1.Services;

public sealed class CustomerService(
    ICustomerRepository customerRepository,
    IRoleRepository roleRepository,
    ICodeGenerator codeGenerator,
    IPasswordHasher passwordHasher) : ICustomerService
{
    private async Task<(Role Role, string Code)> ResolveRoleAndCodeAsync(string? roleInput, string? codeInput = null)
    {
        var roleName = string.IsNullOrWhiteSpace(roleInput)
            ? UserRole.Customer
            : roleInput.Trim().ToLowerInvariant();

        var role = await roleRepository.GetByNameAsync(roleName)
            ?? throw new InvalidOperationException("Role not found");

        var code = string.IsNullOrWhiteSpace(codeInput)
            ? await GenerateCustomerCodeAsync(DateTime.UtcNow)
            : codeInput.Trim();

        return (role, code);
    }

    private Customer BuildCustomerEntity(
        Guid id,
        string name,
        string password,
        string address,
        double discount,
        Guid roleId,
        string code)
    {
        return new Customer
        {
            Id = id,
            Name = name.Trim(),
            PasswordHash = passwordHasher.Hash(password),
            Code = code,
            Address = address.Trim(),
            Discount = discount,
            RoleId = roleId
        };
    }

    public async Task<PagedResponse<CustomerResponse>> GetAllAsync(int page, int pageSize)
    {
        var (customers, totalCount) = await customerRepository.GetAllAsync(page, pageSize);
        return PagedResponseFactory.Create(customers, totalCount, page, pageSize, x => MapToResponse(x));
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id)
    {
        var customer = await customerRepository.GetByFilterAsync(customerId: id);
        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        var normalizedName = request.Name.Trim();
        var existingCustomer = await customerRepository.GetByFilterAsync(normalizedName);
        if (existingCustomer is not null)
        {
            return null;
        }

        var (role, code) = await ResolveRoleAndCodeAsync(request.Role);
        var customer = BuildCustomerEntity(
            Guid.NewGuid(),
            normalizedName,
            request.Password,
            request.Address,
            request.Discount,
            role.Id,
            code);

        await customerRepository.AddAsync(customer);
        return MapToResponse(customer, role.Name);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var (role, code) = await ResolveRoleAndCodeAsync(request.Role, request.Code);
        var customer = BuildCustomerEntity(
            id,
            request.Name,
            request.Password,
            request.Address,
            request.Discount,
            role.Id,
            code);
        return await customerRepository.UpdateAsync(customer);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return customerRepository.DeleteAsync(id);
    }

    private async Task<string> GenerateCustomerCodeAsync(DateTime registrationDate)
    {
        var attempts = 0;
        while (attempts < 10)
        {
            var code = codeGenerator.GenerateCustomerCode(registrationDate);
            if ((await customerRepository.GetByFilterAsync(code: code)) != null)
            {
                return code;
            }

            attempts++;
        }

        throw new InvalidOperationException("Unable to generate unique customer code");
    }

    private static CustomerResponse MapToResponse(Customer customer, string? roleName = null)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            Address = customer.Address,
            Discount = customer.Discount,
            Role = roleName ?? customer.Role.Name
        };
    }
}
