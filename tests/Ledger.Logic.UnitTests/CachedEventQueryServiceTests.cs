using Ledger.Common;
using Ledger.Logic.Querying;
using Microsoft.Extensions.Caching.Memory;

namespace Ledger.Logic.UnitTests;

public class CachedEventQueryServiceTests
{
    [Fact]
    public async Task Repeated_query_is_served_from_the_cache()
    {
        var inner = new CountingQueryService();
        var sut = new CachedEventQueryService(inner, new MemoryCache(new MemoryCacheOptions()));
        var tenantId = Guid.NewGuid();
        var query = new EventQuery(null, null, new PageRequest(1, 20));

        await sut.QueryAsync(tenantId, query);
        await sut.QueryAsync(tenantId, query);

        Assert.Equal(1, inner.Calls);
    }

    private sealed class CountingQueryService : IEventQueryService
    {
        public int Calls { get; private set; }

        public Task<PagedResult<EventView>> QueryAsync(
            Guid tenantId, EventQuery query, CancellationToken cancellationToken = default)
        {
            Calls++;
            return Task.FromResult(new PagedResult<EventView>([], query.Page.Page, query.Page.PageSize, 0));
        }
    }
}
