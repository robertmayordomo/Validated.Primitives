using System.Text.RegularExpressions;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validation methods for various barcode formats.
/// </summary>
public static class BarcodeValidators
{
    /// <summary>
    /// Validates that a value is a valid barcode in one of the supported formats.
    /// Supports UPC-A, EAN-13, EAN-8, Code39, and Code128 formats.
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function that checks if the value is a valid barcode.</returns>
    public static ValueValidator<string> Barcode(string fieldName = "Barcode")
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure("Barcode cannot be empty.", fieldName, "Barcode.Empty");

            // Remove common separators (spaces, hyphens)
            var cleanValue = value.Replace(" ", "").Replace("-", "");

            // Try to validate against different barcode formats
            if (IsValidUpcA(cleanValue) || 
                IsValidEan13(cleanValue) || 
                IsValidEan8(cleanValue) ||
                IsValidCode39(cleanValue) ||
                IsValidCode128(cleanValue))
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Failure(
                "Invalid barcode format. Supported formats: UPC-A (12 digits), EAN-13 (13 digits), EAN-8 (8 digits), Code39 (alphanumeric with *), Code128 (alphanumeric).",
                fieldName,
                "Barcode.InvalidFormat");
        };

    /// <summary>
    /// Validates UPC-A format (12 digits).
    /// </summary>
    private static bool IsValidUpcA(string value)
    {
        if (value.Length != 12 || !value.All(char.IsDigit))
            return false;

        return ValidateChecksum(value, 12);
    }

    /// <summary>
    /// Validates EAN-13 format (13 digits).
    /// </summary>
    private static bool IsValidEan13(string value)
    {
        if (value.Length != 13 || !value.All(char.IsDigit))
            return false;

        return ValidateChecksum(value, 13);
    }

    /// <summary>
    /// Validates EAN-8 format (8 digits).
    /// </summary>
    private static bool IsValidEan8(string value)
    {
        if (value.Length != 8 || !value.All(char.IsDigit))
            return false;

        return ValidateChecksum(value, 8);
    }

    /// <summary>
    /// Validates Code39 format (alphanumeric with asterisk delimiters).
    /// </summary>
    private static bool IsValidCode39(string value)
    {
        // Code39 pattern: starts and ends with *, contains allowed characters
        if (!value.StartsWith("*") || !value.EndsWith("*"))
            return false;
            
        if (value.Length < 3)
            return false;
            
        var pattern = @"^\*[0-9A-Z\-\.\s\$\/\+\%]+\*$";
        return Regex.IsMatch(value, pattern);
    }

    /// <summary>
    /// Validates Code128 format (alphanumeric, length 1-48).
    /// Code128 should NOT contain asterisks (those are Code39).
    /// </summary>
    private static bool IsValidCode128(string value)
    {
        // Code128 can encode all 128 ASCII characters
        // For simplicity, we'll accept alphanumeric and common symbols
        // Minimum realistic length is 2 characters for a barcode
        if (value.Length < 2 || value.Length > 48)
            return false;

        // Reject if it contains asterisks (that would be Code39)
        if (value.Contains('*'))
            return false;

        // Must be printable ASCII and not all digits (that would be UPC/EAN)
        if (value.All(char.IsDigit))
            return false;

        return value.All(c => c >= 32 && c <= 126); // Printable ASCII
    }

    /// <summary>
    /// Validates checksum for UPC/EAN barcodes using the standard algorithm.
    /// UPC/EAN standard: odd positions (1,3,5...) multiplied by 3, even positions (2,4,6...) by 1.
    /// Position counting starts at 1 from the left, excluding the check digit.
    /// </summary>
    private static bool ValidateChecksum(string value, int length)
    {
        if (value.Length != length)
            return false;

        var digits = value.Select(c => c - '0').ToArray();
        var sum = 0;

        // For UPC/EAN: odd positions (1-indexed) multiplied by 3, even by 1
        // Last digit is the check digit, so we process length-1 digits
        for (var i = 0; i < length - 1; i++)
        {
            // Position is 1-indexed: position = i + 1
            // Odd positions (1,3,5,7,9,11) -> multiply by 3
            // Even positions (2,4,6,8,10) -> multiply by 1
            var multiplier = ((i + 1) % 2 == 1) ? 3 : 1;
            sum += digits[i] * multiplier;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == digits[length - 1];
    }
}
