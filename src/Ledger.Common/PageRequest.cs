namespace Ledger.Common;

/// <summary>A 1-based page of results.</summary>
public sealed record PageRequest(int Page, int PageSize)
{
    public const int MaxPageSize = 100;

    public int Skip => Page * PageSize;

    public bool IsValid => Page >= 1 && PageSize is >= 1 and <= MaxPageSize;
}
