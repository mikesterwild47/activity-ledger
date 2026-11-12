using Ledger.Data;
using Ledger.Logic.Templates;

namespace Ledger.Logic.UnitTests;

public class EventTemplatesTests
{
    [Fact]
    public void Renders_actor_and_target()
    {
        var activityEvent = Create("record.deleted", target: "Old Vendors", targetType: "list");

        Assert.Equal("alice@example.test deleted list 'Old Vendors'", EventTemplates.Render(activityEvent));
    }

    [Fact]
    public void Falls_back_to_the_event_type_when_no_template_exists()
    {
        Assert.Equal("made.up", EventTemplates.Render(Create("made.up")));
    }

    private static ActivityEvent Create(string eventType, string? target = null, string? targetType = null) =>
        new(Guid.NewGuid(), Guid.NewGuid(), "msg-1", eventType, "alice@example.test", target, targetType,
            DateTimeOffset.UnixEpoch);
}
