using Validated.Primitives.Validation;

namespace Validated.Primitives.DateRanges;

public static class TimeOnlyRangeExtensions
{
    /// <summary>
    /// Creates a TimeOnlyRange from the specified TimeOnly until now.
    /// </summary>
    /// <param name="from">The start TimeOnly of the range.</param>
    /// <param name="inclusiveStart">Whether the start is inclusive (default: true).</param>
    /// <param name="inclusiveEnd">Whether the end is inclusive (default: true).</param>
    /// <returns>A tuple containing the validation result and the TimeOnlyRange if successful.</returns>
    public static (ValidationResult Result, TimeOnlyRange? Value) UntilNow(
        this TimeOnly from,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        return TimeOnlyRange.TryCreate(from, TimeOnly.FromDateTime(DateTime.Now), inclusiveStart, inclusiveEnd);
    }

    /// <summary>
    /// Creates a TimeOnlyRange from now until the specified TimeOnly.
    /// </summary>
    /// <param name="to">The end TimeOnly of the range.</param>
    /// <param name="inclusiveStart">Whether the start is inclusive (default: true).</param>
    /// <param name="inclusiveEnd">Whether the end is inclusive (default: true).</param>
    /// <returns>A tuple containing the validation result and the TimeOnlyRange if successful.</returns>
    public static (ValidationResult Result, TimeOnlyRange? Value) FromNowUntil(
        this TimeOnly to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        return TimeOnlyRange.TryCreate(TimeOnly.FromDateTime(DateTime.Now), to, inclusiveStart, inclusiveEnd);
    }
}
