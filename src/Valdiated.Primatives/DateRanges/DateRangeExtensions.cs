using Validated.Primitives.Validation;

namespace Validated.Primitives.DateRanges;

public static class DateRangeExtensions
{
    /// <summary>
    /// Creates a DateRange from the specified DateTime until now.
    /// </summary>
    /// <param name="from">The start DateTime of the range.</param>
    /// <param name="inclusiveStart">Whether the start is inclusive (default: true).</param>
    /// <param name="inclusiveEnd">Whether the end is inclusive (default: true).</param>
    /// <returns>A tuple containing the validation result and the DateRange if successful.</returns>
    public static (ValidationResult Result, DateRange? Value) UntilNow(
        this DateTime from,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        return DateRange.TryCreate(from, DateTime.Now, inclusiveStart, inclusiveEnd);
    }

    /// <summary>
    /// Creates a DateRange from now until the specified DateTime.
    /// </summary>
    /// <param name="to">The end DateTime of the range.</param>
    /// <param name="inclusiveStart">Whether the start is inclusive (default: true).</param>
    /// <param name="inclusiveEnd">Whether the end is inclusive (default: true).</param>
    /// <returns>A tuple containing the validation result and the DateRange if successful.</returns>
    public static (ValidationResult Result, DateRange? Value) FromNowUntil(
        this DateTime to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        return DateRange.TryCreate(DateTime.Now, to, inclusiveStart, inclusiveEnd);
    }
}
