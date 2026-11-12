namespace Ledger.Logic.Querying;

public sealed record EventView(Guid Id, string EventType, string Message, string Actor, DateTimeOffset OccurredAt);
