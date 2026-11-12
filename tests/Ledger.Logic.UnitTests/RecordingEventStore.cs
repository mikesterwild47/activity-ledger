using Ledger.Common;
using Ledger.Data;

namespace Ledger.Logic.UnitTests;

internal sealed class RecordingEventStore : IEventStore
{
    public List<ActivityEvent> Added { get; } = [];

    public Task AddAsync(ActivityEvent activityEvent, CancellationToken cancellationToken = default)
    {
        Added.Add(activityEvent);
        return Task.CompletedTask;
    }

    public Task<PagedResult<ActivityEvent>> QueryAsync(
        EventFilter filter, PageRequest page, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}
