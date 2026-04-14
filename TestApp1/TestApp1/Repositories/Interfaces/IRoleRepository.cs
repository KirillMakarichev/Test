using TestApp1.Database.Models;

namespace TestApp1.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);
}
