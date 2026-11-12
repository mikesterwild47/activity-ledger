using System.Threading.Channels;

namespace Ledger.Logic.Processing;

/// <summary>In-process stand-in for a message broker.</summary>
public sealed class ChannelEventQueue : IEventQueue
{
    private readonly Channel<QueuedMessage> _channel = Channel.CreateUnbounded<QueuedMessage>();

    public ValueTask EnqueueAsync(QueuedMessage message, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(message, cancellationToken);

    public IAsyncEnumerable<QueuedMessage> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
