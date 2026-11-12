using Ledger.Data;
using Ledger.Logic.Processing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ledger.Worker;

public sealed class EventProcessingWorker(
    IEventQueue queue,
    EventProcessor processor,
    ILogger<EventProcessingWorker> logger) : BackgroundService
{
    public const int MaxAttempts = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var queued in queue.ReadAllAsync(stoppingToken))
        {
            await HandleAsync(queued, stoppingToken);
        }
    }

    private async Task HandleAsync(QueuedMessage queued, CancellationToken cancellationToken)
    {
        var message = queued.Message;

        try
        {
            await processor.ProcessAsync(message, cancellationToken);
        }
        catch (TransientStorageException ex) when (queued.Attempt < MaxAttempts)
        {
            logger.LogWarning(ex, "Attempt {Attempt} failed for message {MessageId}; retrying",
                queued.Attempt, message.MessageId);

            await queue.EnqueueAsync(queued with { Attempt = queued.Attempt + 1 }, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Dropping message {MessageId} after {Attempt} attempt(s)",
                message.MessageId, queued.Attempt);
        }
    }
}
