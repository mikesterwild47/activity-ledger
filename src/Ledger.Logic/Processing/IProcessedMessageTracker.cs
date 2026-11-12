namespace Ledger.Logic.Processing;

/// <summary>Remembers which messages have been handled so redeliveries are not stored twice.</summary>
public interface IProcessedMessageTracker
{
    bool HasProcessed(Guid tenantId, string messageId);

    void MarkProcessed(Guid tenantId, string messageId);
}
