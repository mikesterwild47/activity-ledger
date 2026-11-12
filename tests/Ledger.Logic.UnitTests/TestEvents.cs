using Ledger.Data;
using Ledger.Logic.Processing;

namespace Ledger.Logic.UnitTests;

internal static class TestEvents
{
    public static DateTimeOffset At(int year, int month, int day, int hour = 0) =>
        new(year, month, day, hour, 0, 0, TimeSpan.Zero);

    public static ActivityEvent Event(Guid tenantId, DateTimeOffset occurredAt, string eventType = "user.signed_in") =>
        new(Guid.NewGuid(), tenantId, Guid.NewGuid().ToString(), eventType, "alice@example.test", null, null, occurredAt);

    public static EmitEventMessage Message(Guid tenantId, string messageId = "msg-1", string eventType = "user.signed_in") =>
        new(tenantId, messageId, eventType, "alice@example.test", null, null, At(2026, 3, 1));
}
