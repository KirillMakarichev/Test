namespace TestApp1.Contracts.Common;

internal static class PagingRequest
{
    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        return (Math.Max(1, page), Math.Clamp(pageSize, 1, 100));
    }
}