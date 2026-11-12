namespace Ledger.Data;

/// <summary>Matches events where <c>From &lt;= OccurredAt &lt; ToExclusive</c>.</summary>
public sealed record EventFilter(Guid TenantId, DateTimeOffset? From, DateTimeOffset? ToExclusive);
