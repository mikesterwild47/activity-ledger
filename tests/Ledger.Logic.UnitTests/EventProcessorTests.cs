using Ledger.Logic.Processing;
using Microsoft.Extensions.Logging.Abstractions;
using static Ledger.Logic.UnitTests.TestEvents;

namespace Ledger.Logic.UnitTests;

public class EventProcessorTests
{
    private static readonly Guid Tenant = Guid.NewGuid();

    private readonly RecordingEventStore _store = new();
    private readonly EventProcessor _sut;

    public EventProcessorTests() =>
        _sut = new EventProcessor(_store, new InMemoryProcessedMessageTracker(), NullLogger<EventProcessor>.Instance);

    [Fact]
    public async Task Stores_a_new_event()
    {
        var result = await _sut.ProcessAsync(Message(Tenant));

        Assert.Equal(ProcessResult.Stored, result);
        Assert.Single(_store.Added);
    }

    [Fact]
    public async Task Ignores_a_redelivered_message()
    {
        await _sut.ProcessAsync(Message(Tenant, "msg-1"));
        var result = await _sut.ProcessAsync(Message(Tenant, "msg-1"));

        Assert.Equal(ProcessResult.Duplicate, result);
        Assert.Single(_store.Added);
    }

    [Fact]
    public async Task Rejects_an_unknown_event_type()
    {
        await Assert.ThrowsAsync<InvalidEventException>(() => _sut.ProcessAsync(Message(Tenant, eventType: "made.up")));

        Assert.Empty(_store.Added);
    }
}
