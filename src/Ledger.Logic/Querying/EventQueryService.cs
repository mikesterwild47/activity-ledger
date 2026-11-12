using Ledger.Common;
using Ledger.Data;
using Ledger.Logic.Templates;

namespace Ledger.Logic.Querying;

public sealed class EventQueryService(IEventStore store) : IEventQueryService
{
    public async Task<PagedResult<EventView>> QueryAsync(
        Guid tenantId, EventQuery query, CancellationToken cancellationToken = default)
    {
        var (from, toExclusive) = DateRange.ToUtcBounds(query.From, query.To);
        var events = await store.QueryAsync(new EventFilter(tenantId, from, toExclusive), query.Page, cancellationToken);

        return events.Map(e => new EventView(e.Id, e.EventType, EventTemplates.Render(e), e.Actor, e.OccurredAt));
    }
}
