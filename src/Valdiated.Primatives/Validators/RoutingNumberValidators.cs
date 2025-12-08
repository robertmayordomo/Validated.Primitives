using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for US ABA routing numbers (also known as routing transit numbers).
/// ABA routing numbers are 9-digit codes used to identify financial institutions in the United States.
/// The format includes a checksum digit calculated using a specific algorithm.
/// </summary>
public static partial class RoutingNumberValidators
{
    /// <summary>
    /// Validates that the routing number is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "RoutingNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "Routing number must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format of a routing number (9 digits).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "RoutingNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim();
            
            if (!RoutingNumberFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "Routing number must contain only digits and optional separators (- or space)",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the routing number contains only digits after removing separators.
    /// </summary>
    public static ValueValidator<string> OnlyDigits(string propertyName = "RoutingNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = value.Replace("-", "").Replace(" ", "");
            
            if (!digitsOnly.All(char.IsDigit))
            {
                return ValidationResult.Failure(
                    "Routing number must contain only digits (separators like - and spaces are allowed)",
                    propertyName,
                    "InvalidCharacters");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the routing number has exactly 9 digits.
    /// </summary>
    public static ValueValidator<string> ValidLength(string propertyName = "RoutingNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = value.Replace("-", "").Replace(" ", "");
            
            if (digitsOnly.Length != 9)
            {
                return ValidationResult.Failure(
                    "Routing number must be exactly 9 digits",
                    propertyName,
                    "InvalidLength");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the ABA routing number checksum.
    /// The checksum is calculated using the following formula:
    /// 3*(d1+d4+d7) + 7*(d2+d5+d8) + (d3+d6+d9) mod 10 = 0
    /// where d1-d9 are the nine digits of the routing number.
    /// </summary>
    public static ValueValidator<string> ValidChecksum(string propertyName = "RoutingNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = value.Replace("-", "").Replace(" ", "");
            
            if (digitsOnly.Length != 9 || !digitsOnly.All(char.IsDigit))
                return ValidationResult.Success(); // Other validators will catch this

            var digits = digitsOnly.Select(c => c - '0').ToArray();
            
            // ABA routing number checksum algorithm
            // 3*(d1+d4+d7) + 7*(d2+d5+d8) + (d3+d6+d9) mod 10 = 0
            var checksum = (3 * (digits[0] + digits[3] + digits[6]) +
                           7 * (digits[1] + digits[4] + digits[7]) +
                           (digits[2] + digits[5] + digits[8])) % 10;

            if (checksum != 0)
            {
                return ValidationResult.Failure(
                    "Routing number checksum is invalid. The routing number does not pass ABA validation.",
                    propertyName,
                    "InvalidChecksum");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the first two digits represent a valid Federal Reserve routing symbol.
    /// Valid ranges are: 00-12, 21-32, 61-72, 80
    /// </summary>
    public static ValueValidator<string> ValidFederalReserveSymbol(string propertyName = "RoutingNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = value.Replace("-", "").Replace(" ", "");
            
            if (digitsOnly.Length < 2 || !digitsOnly.All(char.IsDigit))
                return ValidationResult.Success(); // Other validators will catch this

            var firstTwo = int.Parse(digitsOnly.Substring(0, 2));
            
            // Valid Federal Reserve routing symbols
            var isValid = (firstTwo >= 0 && firstTwo <= 12) ||
                         (firstTwo >= 21 && firstTwo <= 32) ||
                         (firstTwo >= 61 && firstTwo <= 72) ||
                         firstTwo == 80;

            if (!isValid)
            {
                return ValidationResult.Failure(
                    "Routing number has an invalid Federal Reserve routing symbol. The first two digits must be in the ranges: 00-12, 21-32, 61-72, or 80.",
                    propertyName,
                    "InvalidFederalReserveSymbol");
            }

            return ValidationResult.Success();
        };
    }

    [GeneratedRegex(@"^[\d\s\-]+$")]
    private static partial Regex RoutingNumberFormatRegex();
}
