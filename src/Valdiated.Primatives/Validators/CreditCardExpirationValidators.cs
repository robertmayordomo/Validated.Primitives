using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for credit card expiration dates.
/// </summary>
public static class CreditCardExpirationValidators
{
    /// <summary>
    /// Validates that the month is between 1 and 12.
    /// </summary>
    public static ValueValidator<(int Month, int Year)> ValidMonth(string propertyName = "Expiration")
    {
        return value =>
        {
            if (value.Month < 1 || value.Month > 12)
                return ValidationResult.Failure(
                    "Month must be between 1 and 12.",
                    propertyName,
                    "InvalidMonth");

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the year is a positive number.
    /// </summary>
    public static ValueValidator<(int Month, int Year)> ValidYear(string propertyName = "Expiration")
    {
        return value =>
        {
            if (value.Year < 0)
                return ValidationResult.Failure(
                    "Year must be a positive number.",
                    propertyName,
                    "InvalidYear");

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the expiration date is not in the past.
    /// Note: This assumes month and year are already validated to be in valid ranges.
    /// </summary>
    public static ValueValidator<(int Month, int Year)> NotExpired(string propertyName = "Expiration")
    {
        return value =>
        {
            // Skip validation if month or year are invalid (will be caught by other validators)
            if (value.Month < 1 || value.Month > 12 || value.Year < 0)
                return ValidationResult.Success();

            // Normalize two-digit years (e.g., 25 -> 2025)
            var normalizedYear = value.Year < 100 ? (value.Year + 2000) : value.Year;

            var now = DateTime.UtcNow;
            var expiration = new DateTime(normalizedYear, value.Month, DateTime.DaysInMonth(normalizedYear, value.Month));

            if (expiration < new DateTime(now.Year, now.Month, 1))
                return ValidationResult.Failure(
                    "Expiration date must be in the future or current month.",
                    propertyName,
                    "Expired");

            return ValidationResult.Success();
        };
    }
}
