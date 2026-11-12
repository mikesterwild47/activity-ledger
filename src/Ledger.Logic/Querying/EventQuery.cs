using Ledger.Common;

namespace Ledger.Logic.Querying;

/// <summary><paramref name="From"/> and <paramref name="To"/> are inclusive UTC calendar dates.</summary>
public sealed record EventQuery(DateOnly? From, DateOnly? To, PageRequest Page);
