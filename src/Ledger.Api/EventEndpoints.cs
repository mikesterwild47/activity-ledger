using Ledger.Common;
using Ledger.Logic.Processing;
using Ledger.Logic.Querying;
using Ledger.Logic.Templates;

namespace Ledger.Api;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var events = app.MapGroup("/tenants/{tenantId:guid}/events");

        events.MapPost("/", EmitAsync);
        events.MapGet("/", QueryAsync);
    }

    private static async Task<IResult> EmitAsync(
        Guid tenantId,
        EmitEventRequest request,
        IEventQueue queue,
        TimeProvider time,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.MessageId) || string.IsNullOrWhiteSpace(request.Actor))
        {
            return Results.BadRequest("messageId and actor are required.");
        }

        if (!EventTemplates.IsKnown(request.EventType))
        {
            return Results.BadRequest($"Unknown event type '{request.EventType}'.");
        }

        var message = new EmitEventMessage(
            tenantId,
            request.MessageId,
            request.EventType,
            request.Actor,
            request.Target,
            request.TargetType,
            request.OccurredAt ?? time.GetUtcNow());

        await queue.EnqueueAsync(new QueuedMessage(message), cancellationToken);

        return Results.Accepted();
    }

    private static async Task<IResult> QueryAsync(
        Guid tenantId,
        DateOnly? from,
        DateOnly? to,
        IEventQueryService events,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 20)
    {
        var paging = new PageRequest(page, pageSize);

        if (!paging.IsValid)
        {
            return Results.BadRequest($"page must be >= 1 and pageSize between 1 and {PageRequest.MaxPageSize}.");
        }

        if (from > to)
        {
            return Results.BadRequest("from must be on or before to.");
        }

        var result = await events.QueryAsync(tenantId, new EventQuery(from, to, paging), cancellationToken);

        return Results.Ok(result);
    }
}

public sealed record EmitEventRequest(
    string MessageId,
    string EventType,
    string Actor,
    string? Target,
    string? TargetType,
    DateTimeOffset? OccurredAt);
