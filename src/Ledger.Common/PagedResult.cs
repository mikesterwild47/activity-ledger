namespace Ledger.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResult<TResult> Map<TResult>(Func<T, TResult> selector) =>
        new(Items.Select(selector).ToList(), Page, PageSize, TotalCount);
}
