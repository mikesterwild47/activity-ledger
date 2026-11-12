namespace Ledger.Logic.Processing;

public interface IEventQueue
{
    ValueTask EnqueueAsync(QueuedMessage message, CancellationToken cancellationToken = default);

    IAsyncEnumerable<QueuedMessage> ReadAllAsync(CancellationToken cancellationToken);
}
