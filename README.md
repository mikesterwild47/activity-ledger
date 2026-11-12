# Activity Ledger

A small multi-tenant service that records user activity ("alice deleted report 'Q3 Forecast'") and lets each tenant page through its own history.

## How it fits together

```
POST /tenants/{id}/events ──► IEventQueue ──► EventProcessingWorker ──► EventProcessor ──► IEventStore
                                                                                                │
GET  /tenants/{id}/events ──► CachedEventQueryService ──► EventQueryService ────────────────────┘
```

| Project | Responsibility |
|---|---|
| `Ledger.Api` | HTTP endpoints, DI wiring, demo data |
| `Ledger.Worker` | Background service that drains the queue and retries transient failures |
| `Ledger.Logic` | Querying, caching, event processing, message templates |
| `Ledger.Data` | Event storage (in memory; the fault-injecting wrapper mimics a flaky database) |
| `Ledger.Common` | Paging primitives shared across layers |

Everything runs in a single process with no external dependencies. The queue and store are in-memory stand-ins for a real broker and database.

## Running it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet test
dotnet run --project src/Ledger.Api
```

The API listens on `http://localhost:5080` and seeds two tenants, Contoso and Fabrikam, with recent activity. `requests.http` has ready-made calls, and `scripts/emit-batch.sh` sends a batch of events.

## Reported issues

Customers have reported the problems below. Work through them in order. For each one, find the root cause, explain it, and make the fix.

Think out loud. How you investigate matters more than how many you finish.

**1. Newest events are missing.**
> "The first page of our activity log never shows the most recent events. They don't appear anywhere."

**2. Date filter drops a day.**
> "When I filter from the 1st to the 7th, nothing from the 7th shows up. Filtering on just today returns nothing at all."

**3. Another company's data.**
> "Our admin opened the activity log and saw users and reports that don't belong to us. It went back to normal a minute later."

**4. Events silently disappear.**
> "Our integration sends every event and gets a 202 back, but a noticeable share of them never show up. Support sees retry warnings in the logs, but no errors."
