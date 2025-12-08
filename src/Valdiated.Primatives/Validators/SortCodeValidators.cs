using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for bank sort codes across different countries.
/// Sort codes are used primarily in the UK and Ireland to identify bank branches.
/// </summary>
public static partial class SortCodeValidators
{
    /// <summary>
    /// Validates that the sort code is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "SortCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "Sort code must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format of a sort code (digits and allowed separators).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "SortCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim();
            
            if (!SortCodeFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "Sort code must contain only digits and optional separators (- or space)",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates sort code format specific to the country.
    /// </summary>
    public static ValueValidator<string> ValidateCountryFormat(string propertyName, CountryCode countryCode)
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            // First check if it contains dashes and validate the format structure
            if (value.Contains('-'))
            {
                var parts = value.Split('-');
                if (parts.Length != 3 || parts.Any(p => p.Length != 2))
                {
                    return ValidationResult.Failure(
                        $"Sort code with dashes must be in format XX-XX-XX",
                        propertyName,
                        "InvalidCountrySortCodeFormat");
                }
            }

            var normalized = value.Replace("-", "").Replace(" ", "").Trim();
            var isValid = countryCode switch
            {
                CountryCode.UnitedKingdom => ValidateUkSortCode(normalized),
                CountryCode.Ireland => ValidateIrelandSortCode(normalized),
                _ => true // Other countries don't use sort codes
            };

            if (!isValid)
            {
                return ValidationResult.Failure(
                    $"Sort code is not valid for {countryCode}. Expected format: {GetExpectedFormat(countryCode)}",
                    propertyName,
                    "InvalidCountrySortCodeFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates UK sort code format (6 digits, often displayed as XX-XX-XX).
    /// </summary>
    private static bool ValidateUkSortCode(string sortCode)
    {
        return UkSortCodeRegex().IsMatch(sortCode);
    }

    /// <summary>
    /// Validates Ireland sort code format (6 digits, often displayed as XX-XX-XX).
    /// Irish sort codes follow the same format as UK.
    /// </summary>
    private static bool ValidateIrelandSortCode(string sortCode)
    {
        return IrelandSortCodeRegex().IsMatch(sortCode);
    }

    /// <summary>
    /// Gets the expected format description for a country's sort code.
    /// </summary>
    private static string GetExpectedFormat(CountryCode countryCode)
    {
        return countryCode switch
        {
            CountryCode.UnitedKingdom => "6 digits (e.g., 12-34-56 or 123456)",
            CountryCode.Ireland => "6 digits (e.g., 12-34-56 or 123456)",
            _ => "N/A - this country does not use sort codes"
        };
    }

    /// <summary>
    /// Validates that the sort code contains only digits after removing separators.
    /// </summary>
    public static ValueValidator<string> OnlyDigits(string propertyName = "SortCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = value.Replace("-", "").Replace(" ", "");
            
            if (!digitsOnly.All(char.IsDigit))
            {
                return ValidationResult.Failure(
                    "Sort code must contain only digits (separators like - and spaces are allowed)",
                    propertyName,
                    "InvalidCharacters");
            }

            return ValidationResult.Success();
        };
    }

    [GeneratedRegex(@"^[\d\s\-]+$")]
    private static partial Regex SortCodeFormatRegex();

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex UkSortCodeRegex();

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex IrelandSortCodeRegex();
}
