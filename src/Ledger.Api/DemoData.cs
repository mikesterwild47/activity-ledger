using Ledger.Data;

namespace Ledger.Api;

/// <summary>Seeds two tenants with recent activity so the API has something to return locally.</summary>
public static class DemoData
{
    public static readonly Guid Contoso = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Fabrikam = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly (string EventType, string? Target, string? TargetType)[] Activities =
    [
        ("user.signed_in", null, null),
        ("record.created", "Q3 Forecast", "report"),
        ("record.updated", "Q3 Forecast", "report"),
        ("role.granted", "Billing Admin", "role"),
        ("record.deleted", "Old Vendors", "list"),
        ("user.signed_out", null, null),
    ];

    public static async Task SeedAsync(IEventStore store, TimeProvider time)
    {
        var now = time.GetUtcNow();

        await SeedTenantAsync(store, Contoso, "contoso", count: 40, spacing: TimeSpan.FromHours(5), now);
        await SeedTenantAsync(store, Fabrikam, "fabrikam", count: 12, spacing: TimeSpan.FromHours(11), now);
    }

    private static async Task SeedTenantAsync(
        IEventStore store, Guid tenantId, string domain, int count, TimeSpan spacing, DateTimeOffset now)
    {
        string[] actors = [$"alice@{domain}.test", $"bob@{domain}.test", $"carol@{domain}.test"];

        for (var i = 0; i < count; i++)
        {
            var (eventType, target, targetType) = Activities[i % Activities.Length];

            await store.AddAsync(new ActivityEvent(
                Guid.NewGuid(),
                tenantId,
                MessageId: $"seed-{domain}-{i}",
                eventType,
                actors[i % actors.Length],
                target,
                targetType,
                OccurredAt: now - spacing * i));
        }
    }
}
