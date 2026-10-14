using AdvancedEFCore.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AdvancedEFCore.Helpers;

public static class PaginationExtensions
{
    private const int MaxPageSize = 50;

    // The query must already be ordered by the caller.
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}