using Ledger.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Ledger.Logic.Querying;

/// <summary>Short-lived cache in front of the query service; dashboards poll the same queries repeatedly.</summary>
public sealed class CachedEventQueryService(IEventQueryService inner, IMemoryCache cache) : IEventQueryService
{
    private static readonly TimeSpan TimeToLive = TimeSpan.FromSeconds(30);

    public async Task<PagedResult<EventView>> QueryAsync(
        Guid tenantId, EventQuery query, CancellationToken cancellationToken = default)
    {
        var result = await cache.GetOrCreateAsync(query, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeToLive;
            return inner.QueryAsync(tenantId, query, cancellationToken);
        });

        return result!;
    }
}
