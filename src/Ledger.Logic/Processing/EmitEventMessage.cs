namespace Ledger.Logic.Processing;

public sealed record EmitEventMessage(
    Guid TenantId,
    string MessageId,
    string EventType,
    string Actor,
    string? Target,
    string? TargetType,
    DateTimeOffset OccurredAt);

public sealed record QueuedMessage(EmitEventMessage Message, int Attempt = 1);
