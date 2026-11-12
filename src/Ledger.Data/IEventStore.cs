using Ledger.Common;

namespace Ledger.Data;

public interface IEventStore
{
    Task AddAsync(ActivityEvent activityEvent, CancellationToken cancellationToken = default);

    /// <summary>Returns matching events, newest first.</summary>
    Task<PagedResult<ActivityEvent>> QueryAsync(
        EventFilter filter, PageRequest page, CancellationToken cancellationToken = default);
}
