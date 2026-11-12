using System.Collections.Concurrent;

namespace Ledger.Logic.Processing;

public sealed class InMemoryProcessedMessageTracker : IProcessedMessageTracker
{
    private readonly ConcurrentDictionary<(Guid TenantId, string MessageId), byte> _processed = new();

    public bool HasProcessed(Guid tenantId, string messageId) => _processed.ContainsKey((tenantId, messageId));

    public void MarkProcessed(Guid tenantId, string messageId) => _processed.TryAdd((tenantId, messageId), 0);
}
