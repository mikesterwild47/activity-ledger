using Ledger.Common;

namespace Ledger.Data;

/// <summary>Stands in for a real database by failing a share of writes with a transient error.</summary>
public sealed class FaultInjectingEventStore(IEventStore inner, double transientFailureRate) : IEventStore
{
    public Task AddAsync(ActivityEvent activityEvent, CancellationToken cancellationToken = default)
    {
        if (Random.Shared.NextDouble() < transientFailureRate)
        {
            throw new TransientStorageException($"Write timed out for message '{activityEvent.MessageId}'.");
        }

        return inner.AddAsync(activityEvent, cancellationToken);
    }

    public Task<PagedResult<ActivityEvent>> QueryAsync(
        EventFilter filter, PageRequest page, CancellationToken cancellationToken = default) =>
        inner.QueryAsync(filter, page, cancellationToken);
}
