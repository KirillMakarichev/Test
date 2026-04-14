using Microsoft.EntityFrameworkCore;

namespace TestApp1.Repositories;

public static class QueryablePagingExtensions
{
    public static async Task<(List<T> Items, int TotalCount)> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}