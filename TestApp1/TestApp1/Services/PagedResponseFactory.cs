using TestApp1.Contracts.Common;

namespace TestApp1.Services;

public static class PagedResponseFactory
{
    public static PagedResponse<TResponse> Create<TEntity, TResponse>(
        IEnumerable<TEntity> items,
        int totalCount,
        int page,
        int pageSize,
        Func<TEntity, TResponse> map)
    {
        return new PagedResponse<TResponse>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items.Select(map).ToList()
        };
    }
}