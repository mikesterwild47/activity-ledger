using Ledger.Common;
using Ledger.Data;
using Ledger.Logic.Querying;
using static Ledger.Logic.UnitTests.TestEvents;

namespace Ledger.Logic.UnitTests;

public class EventQueryServiceTests
{
    private static readonly Guid Tenant = Guid.NewGuid();

    private readonly InMemoryEventStore _store = new();
    private readonly EventQueryService _sut;

    public EventQueryServiceTests() => _sut = new EventQueryService(_store);

    [Fact]
    public async Task First_page_returns_the_newest_events()
    {
        await AddAsync(Tenant, At(2026, 3, 1), At(2026, 3, 3), At(2026, 3, 2));

        var result = await _sut.QueryAsync(Tenant, new EventQuery(null, null, new PageRequest(1, 2)));

        Assert.Equal([At(2026, 3, 3), At(2026, 3, 2)], result.Items.Select(e => e.OccurredAt));
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task Excludes_events_outside_the_date_range()
    {
        await AddAsync(Tenant, At(2026, 3, 1), At(2026, 3, 5, hour: 12), At(2026, 3, 9));

        var query = new EventQuery(new DateOnly(2026, 3, 4), new DateOnly(2026, 3, 6), new PageRequest(1, 20));
        var result = await _sut.QueryAsync(Tenant, query);

        Assert.Equal([At(2026, 3, 5, hour: 12)], result.Items.Select(e => e.OccurredAt));
    }

    [Fact]
    public async Task Only_returns_events_for_the_requested_tenant()
    {
        var otherTenant = Guid.NewGuid();
        await AddAsync(Tenant, At(2026, 3, 1));
        await AddAsync(otherTenant, At(2026, 3, 2));

        var result = await _sut.QueryAsync(Tenant, new EventQuery(null, null, new PageRequest(1, 20)));

        Assert.Equal([At(2026, 3, 1)], result.Items.Select(e => e.OccurredAt));
    }

    private async Task AddAsync(Guid tenantId, params DateTimeOffset[] occurrences)
    {
        foreach (var occurredAt in occurrences)
        {
            await _store.AddAsync(Event(tenantId, occurredAt));
        }
    }
}
