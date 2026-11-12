namespace Ledger.Logic.Querying;

public static class DateRange
{
    /// <summary>Converts inclusive UTC calendar dates into a half-open <c>[from, toExclusive)</c> range.</summary>
    public static (DateTimeOffset? From, DateTimeOffset? ToExclusive) ToUtcBounds(DateOnly? from, DateOnly? to) =>
        (from is { } start ? StartOfDay(start) : null,
         to is { } end ? StartOfDay(end) : null);

    private static DateTimeOffset StartOfDay(DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}
