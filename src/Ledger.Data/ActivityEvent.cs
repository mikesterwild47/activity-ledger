namespace Ledger.Data;

public sealed record ActivityEvent(
    Guid Id,
    Guid TenantId,
    string MessageId,
    string EventType,
    string Actor,
    string? Target,
    string? TargetType,
    DateTimeOffset OccurredAt);
