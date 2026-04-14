using Microsoft.EntityFrameworkCore;
using TestApp1.Database;
using TestApp1.Database.Models;
using TestApp1.Repositories.Interfaces;

namespace TestApp1.Repositories;

public sealed class RoleRepository(ShopDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByNameAsync(string roleName)
    {
        return dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == roleName);
    }
}
