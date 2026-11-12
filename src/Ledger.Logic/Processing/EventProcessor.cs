using Ledger.Data;
using Ledger.Logic.Templates;
using Microsoft.Extensions.Logging;

namespace Ledger.Logic.Processing;

public enum ProcessResult
{
    Stored,
    Duplicate,
}

public sealed class EventProcessor(
    IEventStore store,
    IProcessedMessageTracker tracker,
    ILogger<EventProcessor> logger)
{
    public async Task<ProcessResult> ProcessAsync(EmitEventMessage message, CancellationToken cancellationToken = default)
    {
        if (!EventTemplates.IsKnown(message.EventType))
        {
            throw new InvalidEventException($"Unknown event type '{message.EventType}'.");
        }

        if (tracker.HasProcessed(message.TenantId, message.MessageId))
        {
            logger.LogDebug("Skipping duplicate message {MessageId}", message.MessageId);
            return ProcessResult.Duplicate;
        }

        tracker.MarkProcessed(message.TenantId, message.MessageId);
        await store.AddAsync(ToActivityEvent(message), cancellationToken);

        return ProcessResult.Stored;
    }

    private static ActivityEvent ToActivityEvent(EmitEventMessage message) => new(
        Guid.NewGuid(),
        message.TenantId,
        message.MessageId,
        message.EventType,
        message.Actor,
        message.Target,
        message.TargetType,
        message.OccurredAt);
}
