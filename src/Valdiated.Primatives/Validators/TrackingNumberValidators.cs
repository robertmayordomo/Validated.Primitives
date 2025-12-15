using System.Text.RegularExpressions;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validation methods for various tracking number formats.
/// </summary>
public static class TrackingNumberValidators
{
    /// <summary>
    /// Validates that a value is a valid tracking number in one of the supported carrier formats.
    /// Supports UPS, FedEx (Express/Ground/SmartPost), USPS, DHL (Express/eCommerce/Global Mail),
    /// Amazon, Royal Mail, Canada Post, Australia Post, TNT, China Post, LaserShip, OnTrac, and Irish Post.
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function that checks if the value is a valid tracking number.</returns>
    public static ValueValidator<string> TrackingNumber(string fieldName = "TrackingNumber")
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure("Tracking number cannot be empty.", fieldName, "TrackingNumber.Empty");

            // Remove common separators (spaces, hyphens)
            var cleanValue = value.Replace(" ", "").Replace("-", "").ToUpperInvariant();

            // Try to validate against different carrier formats
            if (IsValidUPS(cleanValue) ||
                IsValidFedExExpress(cleanValue) ||
                IsValidFedExGround(cleanValue) ||
                IsValidFedExSmartPost(cleanValue) ||
                IsValidUSPS(cleanValue) ||
                IsValidDHLExpress(cleanValue) ||
                IsValidDHLEcommerce(cleanValue) ||
                IsValidDHLGlobalMail(cleanValue) ||
                IsValidAmazonLogistics(cleanValue) ||
                IsValidRoyalMail(cleanValue) ||
                IsValidCanadaPost(cleanValue) ||
                IsValidAustraliaPost(cleanValue) ||
                IsValidTNT(cleanValue) ||
                IsValidChinaPost(cleanValue) ||
                IsValidLaserShip(cleanValue) ||
                IsValidOnTrac(cleanValue) ||
                IsValidIrishPost(cleanValue))
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Failure(
                "Invalid tracking number format. Supported carriers: UPS, FedEx, USPS, DHL, Amazon, Royal Mail, Canada Post, Australia Post, TNT, China Post, LaserShip, OnTrac, Irish Post.",
                fieldName,
                "TrackingNumber.InvalidFormat");
        };

    /// <summary>
    /// Validates UPS tracking number format (18 characters starting with "1Z").
    /// Format: 1Z + 6 alphanumeric + 2 digits + 6 alphanumeric
    /// </summary>
    private static bool IsValidUPS(string value)
    {
        if (value.Length != 18 || !value.StartsWith("1Z"))
            return false;

        // 1Z + 16 alphanumeric characters
        return value.Substring(2).All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Validates FedEx Express tracking number format (12 digits).
    /// </summary>
    private static bool IsValidFedExExpress(string value)
    {
        return value.Length == 12 && value.All(char.IsDigit);
    }

    /// <summary>
    /// Validates FedEx Ground tracking number format (15 digits).
    /// </summary>
    private static bool IsValidFedExGround(string value)
    {
        return value.Length == 15 && value.All(char.IsDigit);
    }

    /// <summary>
    /// Validates FedEx SmartPost tracking number format (22 digits).
    /// </summary>
    private static bool IsValidFedExSmartPost(string value)
    {
        return value.Length == 22 && value.All(char.IsDigit);
    }

    /// <summary>
    /// Validates USPS tracking number format (20-22 alphanumeric characters).
    /// Common patterns: 20 digits, 22 alphanumeric with specific prefixes
    /// </summary>
    private static bool IsValidUSPS(string value)
    {
        if (value.Length < 20 || value.Length > 22)
            return false;

        // USPS tracking numbers are alphanumeric
        if (!value.All(char.IsLetterOrDigit))
            return false;

        // Common USPS patterns
        // - 20 digits
        // - 9 digits + 2 letters + 9 digits + 2 letters (22 chars)
        // - Starts with specific prefixes: 94, 92, 93, 82, etc.
        if (value.Length == 20 && value.All(char.IsDigit))
            return true;

        if (value.Length == 22)
        {
            // Pattern: 2 letters + 9 digits + 2 letters + 9 digits
            var pattern = @"^[A-Z]{2}\d{9}[A-Z]{2}\d{9}$";
            if (Regex.IsMatch(value, pattern))
                return true;

            // Also accept 22-digit numeric
            if (value.All(char.IsDigit))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Validates DHL Express tracking number format (10 digits).
    /// </summary>
    private static bool IsValidDHLExpress(string value)
    {
        return value.Length == 10 && value.All(char.IsDigit);
    }

    /// <summary>
    /// Validates DHL eCommerce tracking number format (22 alphanumeric starting with "GM").
    /// </summary>
    private static bool IsValidDHLEcommerce(string value)
    {
        if (!value.StartsWith("GM") || value.Length != 22)
            return false;

        return value.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Validates DHL Global Mail tracking number format (13-16 alphanumeric).
    /// </summary>
    private static bool IsValidDHLGlobalMail(string value)
    {
        if (value.Length < 13 || value.Length > 16)
            return false;

        return value.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Validates Amazon Logistics tracking number format ("TBA" followed by 12 digits).
    /// </summary>
    private static bool IsValidAmazonLogistics(string value)
    {
        if (!value.StartsWith("TBA") || value.Length != 15)
            return false;

        return value.Substring(3).All(char.IsDigit);
    }

    /// <summary>
    /// Validates Royal Mail tracking number format (13 characters: 2 letters + 9 digits + 2 letters).
    /// Format: XX123456789GB
    /// </summary>
    private static bool IsValidRoyalMail(string value)
    {
        if (value.Length != 13)
            return false;

        var pattern = @"^[A-Z]{2}\d{9}[A-Z]{2}$";
        return Regex.IsMatch(value, pattern);
    }

    /// <summary>
    /// Validates Canada Post tracking number format (16 alphanumeric).
    /// </summary>
    private static bool IsValidCanadaPost(string value)
    {
        if (value.Length != 16)
            return false;

        return value.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Validates Australia Post tracking number format (13 digits).
    /// </summary>
    private static bool IsValidAustraliaPost(string value)
    {
        return value.Length == 13 && value.All(char.IsDigit);
    }

    /// <summary>
    /// Validates TNT tracking number format (9 or 13 digits).
    /// </summary>
    private static bool IsValidTNT(string value)
    {
        if (value.Length != 9 && value.Length != 13)
            return false;

        return value.All(char.IsDigit);
    }

    /// <summary>
    /// Validates China Post tracking number format (13 characters: 2 letters + 9 digits + 2 letters).
    /// Format: RX123456789CN
    /// </summary>
    private static bool IsValidChinaPost(string value)
    {
        if (value.Length != 13)
            return false;

        var pattern = @"^[A-Z]{2}\d{9}[A-Z]{2}$";
        return Regex.IsMatch(value, pattern);
    }

    /// <summary>
    /// Validates LaserShip tracking number format ("1LS" followed by 12 digits).
    /// </summary>
    private static bool IsValidLaserShip(string value)
    {
        if (!value.StartsWith("1LS") || value.Length != 15)
            return false;

        return value.Substring(3).All(char.IsDigit);
    }

    /// <summary>
    /// Validates OnTrac tracking number format ("C" followed by 14 digits).
    /// </summary>
    private static bool IsValidOnTrac(string value)
    {
        if (!value.StartsWith("C") || value.Length != 15)
            return false;

        return value.Substring(1).All(char.IsDigit);
    }

    /// <summary>
    /// Validates Irish Post tracking number format (13 characters: 2 letters + 9 digits + 2 letters).
    /// Format: XX123456789IE
    /// </summary>
    private static bool IsValidIrishPost(string value)
    {
        if (value.Length != 13)
            return false;

        var pattern = @"^[A-Z]{2}\d{9}[A-Z]{2}$";
        return Regex.IsMatch(value, pattern);
    }
}
