using System;

namespace Validated.Primitives.ValueObjects;

public sealed class CreditCardExpiration
{
    public int Month { get; }
    public int Year { get; }

    private CreditCardExpiration(int month, int year)
    {
        Month = month;
        Year = year;
    }

    public static CreditCardExpiration Create(int month, int year)
    {
        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");

        if (year < 0)
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be a positive number.");

        // Normalize two-digit years (e.g., 25 -> 2025)
        var normalizedYear = year < 100 ? (year + 2000) : year;

        var now = DateTime.UtcNow;
        var expiration = new DateTime(normalizedYear, month, DateTime.DaysInMonth(normalizedYear, month));

        if (expiration < new DateTime(now.Year, now.Month, 1))
            throw new ArgumentException("Expiration date must be in the future or current month.", nameof(year));

        return new CreditCardExpiration(month, normalizedYear);
    }

    public override string ToString() => $"{Month:D2}/{Year % 100:D2}";
}
