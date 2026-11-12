using System.Collections.Concurrent;
using Ledger.Common;

namespace Ledger.Data;

public sealed class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentQueue<ActivityEvent> _events = new();

    public Task AddAsync(ActivityEvent activityEvent, CancellationToken cancellationToken = default)
    {
        _events.Enqueue(activityEvent);
        return Task.CompletedTask;
    }

    public Task<PagedResult<ActivityEvent>> QueryAsync(
        EventFilter filter, PageRequest page, CancellationToken cancellationToken = default)
    {
        var matches = _events
            .Where(e => e.TenantId == filter.TenantId)
            .Where(e => filter.From is null || e.OccurredAt >= filter.From)
            .Where(e => filter.ToExclusive is null || e.OccurredAt < filter.ToExclusive)
            .OrderByDescending(e => e.OccurredAt)
            .ToList();

        var items = matches.Skip(page.Skip).Take(page.PageSize).ToList();

        return Task.FromResult(new PagedResult<ActivityEvent>(items, page.Page, page.PageSize, matches.Count));
    }
}
