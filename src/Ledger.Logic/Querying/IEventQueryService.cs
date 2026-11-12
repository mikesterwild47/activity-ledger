using Ledger.Common;

namespace Ledger.Logic.Querying;

public interface IEventQueryService
{
    Task<PagedResult<EventView>> QueryAsync(
        Guid tenantId, EventQuery query, CancellationToken cancellationToken = default);
}
