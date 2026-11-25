using CommunityGuard.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityGuard.Shared.Extensions;

public static class QueryablePagingExtensions
{
    // Call this on ANY IQueryable<T> to get server-side paging.
    // Never call .ToList() before this — that would load everything into memory.
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);       // Total before paging.

        var items = await query
            .Skip((page - 1) * pageSize)                              // Skip previous pages.
            .Take(pageSize)                                           // Take current page only.
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(items, total, page, pageSize);
    }
}
