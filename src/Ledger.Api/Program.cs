using Ledger.Api;
using Ledger.Data;
using Ledger.Logic.Processing;
using Ledger.Logic.Querying;
using Ledger.Worker;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<InMemoryEventStore>();
builder.Services.AddSingleton<IEventStore>(sp => new FaultInjectingEventStore(
    sp.GetRequiredService<InMemoryEventStore>(),
    builder.Configuration.GetValue<double>("Storage:TransientFailureRate")));

builder.Services.AddSingleton<EventQueryService>();
builder.Services.AddSingleton<IEventQueryService>(sp => new CachedEventQueryService(
    sp.GetRequiredService<EventQueryService>(),
    sp.GetRequiredService<IMemoryCache>()));

builder.Services.AddSingleton<IEventQueue, ChannelEventQueue>();
builder.Services.AddSingleton<IProcessedMessageTracker, InMemoryProcessedMessageTracker>();
builder.Services.AddSingleton<EventProcessor>();
builder.Services.AddHostedService<EventProcessingWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await DemoData.SeedAsync(
        app.Services.GetRequiredService<InMemoryEventStore>(),
        app.Services.GetRequiredService<TimeProvider>());
}

app.MapEventEndpoints();

app.Run();
