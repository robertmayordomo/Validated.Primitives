using Validated.Primitives.Validation;

namespace Validated.Primitives.DateRanges;

public static class DateOnlyRangeExtensions
{
    /// <summary>
    /// Creates a DateOnlyRange from the specified DateOnly until today.
    /// </summary>
    /// <param name="from">The start DateOnly of the range.</param>
    /// <param name="inclusiveStart">Whether the start is inclusive (default: true).</param>
    /// <param name="inclusiveEnd">Whether the end is inclusive (default: true).</param>
    /// <returns>A tuple containing the validation result and the DateOnlyRange if successful.</returns>
    public static (ValidationResult Result, DateOnlyRange? Value) UntilToday(
        this DateOnly from,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        return DateOnlyRange.TryCreate(from, DateOnly.FromDateTime(DateTime.Now), inclusiveStart, inclusiveEnd);
    }

    /// <summary>
    /// Creates a DateOnlyRange from today until the specified DateOnly.
    /// </summary>
    /// <param name="to">The end DateOnly of the range.</param>
    /// <param name="inclusiveStart">Whether the start is inclusive (default: true).</param>
    /// <param name="inclusiveEnd">Whether the end is inclusive (default: true).</param>
    /// <returns>A tuple containing the validation result and the DateOnlyRange if successful.</returns>
    public static (ValidationResult Result, DateOnlyRange? Value) FromTodayUntil(
        this DateOnly to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        return DateOnlyRange.TryCreate(DateOnly.FromDateTime(DateTime.Now), to, inclusiveStart, inclusiveEnd);
    }
}
