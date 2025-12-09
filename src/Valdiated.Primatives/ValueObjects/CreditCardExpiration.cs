using System;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated credit card expiration date.
/// Validates that the month and year are valid and that the date is not expired.
/// </summary>
public sealed record CreditCardExpiration : ValidatedPrimitive<(int Month, int Year)>
{
    /// <summary>
    /// Gets the expiration month (1-12).
    /// </summary>
    public int Month => Value.Month;

    /// <summary>
    /// Gets the expiration year (normalized to four digits).
    /// </summary>
    public int Year => Value.Year;

    private CreditCardExpiration(int month, int year, string propertyName = "Expiration")
        : base((month, NormalizeYear(year)))
    {
        Validators.Add(CreditCardExpirationValidators.ValidMonth(propertyName));
        Validators.Add(CreditCardExpirationValidators.ValidYear(propertyName));
        Validators.Add(CreditCardExpirationValidators.NotExpired(propertyName));
    }

    /// <summary>
    /// Attempts to create a CreditCardExpiration instance with validation.
    /// </summary>
    /// <param name="month">The expiration month (1-12).</param>
    /// <param name="year">The expiration year (can be 2-digit or 4-digit).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the CreditCardExpiration instance if valid.</returns>
    public static (ValidationResult Result, CreditCardExpiration? Value) TryCreate(
        int month,
        int year,
        string propertyName = "Expiration")
    {
        var expiration = new CreditCardExpiration(month, year, propertyName);
        var validationResult = expiration.Validate();
        var result = validationResult.IsValid ? expiration : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the expiration date in MM/YY format.
    /// </summary>
    public override string ToString() => $"{Month:D2}/{Year % 100:D2}";

    /// <summary>
    /// Normalizes a two-digit year to a four-digit year (e.g., 25 -> 2025).
    /// </summary>
    private static int NormalizeYear(int year)
    {
        return year < 100 ? (year + 2000) : year;
    }

    /// <summary>
    /// Determines whether the specified CreditCardExpiration is equal to the current CreditCardExpiration.
    /// </summary>
    public bool Equals(CreditCardExpiration? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Month == other.Month && Year == other.Year;
    }

    /// <summary>
    /// Returns the hash code for this CreditCardExpiration.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Month, Year);
    }
}
