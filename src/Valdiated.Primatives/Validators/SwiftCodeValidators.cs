using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for SWIFT codes (also known as BIC - Bank Identifier Code) according to ISO 9362 standard.
/// SWIFT codes are used to identify banks and financial institutions globally for international wire transfers.
/// 
/// ISO 9362 Format: AAAABBCCXXX where:
/// - AAAA: Institution code (4 letters) - identifies the bank or financial institution
/// - BB: Country code (2 letters, ISO 3166-1 alpha-2) - identifies the country
/// - CC: Location code (2 letters or digits) - identifies the location/city
/// - XXX: Branch code (optional, 3 letters or digits) - identifies the specific branch
/// 
/// Valid lengths according to ISO 9362:
/// - 8 characters: BIC8 (Business Identifier Code without branch - represents primary office)
/// - 11 characters: BIC11 (Business Identifier Code with branch)
/// 
/// Reference: ISO 9362:2014 - Banking — Banking telecommunication messages — Business identifier code (BIC)
/// </summary>
public static partial class SwiftCodeValidators
{
    /// <summary>
    /// Validates that the SWIFT code is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "SwiftCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "SWIFT code must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format of a SWIFT code (alphanumeric characters only).
    /// According to ISO 9362, SWIFT codes must contain only uppercase letters (A-Z) and digits (0-9).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "SwiftCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim().ToUpperInvariant();
            
            if (!SwiftCodeFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "SWIFT code must contain only letters and digits (ISO 9362)",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the SWIFT code has valid length according to ISO 9362.
    /// - 8 characters: BIC8 (without branch code, represents primary office)
    /// - 11 characters: BIC11 (with branch code)
    /// </summary>
    public static ValueValidator<string> ValidLength(string propertyName = "SwiftCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim();
            
            if (normalized.Length != 8 && normalized.Length != 11)
            {
                return ValidationResult.Failure(
                    "SWIFT code must be either 8 characters (BIC8) or 11 characters (BIC11) according to ISO 9362",
                    propertyName,
                    "InvalidLength");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the structure of a SWIFT code according to ISO 9362.
    /// Structure: AAAABBCCXXX where:
    /// - AAAA: Institution code (4 letters A-Z)
    /// - BB: Country code (2 letters A-Z, must be valid ISO 3166-1 alpha-2 code)
    /// - CC: Location code (2 characters A-Z or 0-9)
    /// - XXX: Branch code (3 characters A-Z or 0-9, optional)
    /// </summary>
    public static ValueValidator<string> ValidStructure(string propertyName = "SwiftCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim().ToUpperInvariant();
            
            if (normalized.Length != 8 && normalized.Length != 11)
                return ValidationResult.Success(); // Other validators will catch this

            // ISO 9362 structure: AAAABBCCXXX
            // AAAA: Institution code (4 letters)
            // BB: Country code (2 letters)
            // CC: Location code (2 letters or digits)
            // XXX: Branch code (3 letters or digits, optional)

            if (!SwiftCodeStructureRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "SWIFT code structure is invalid. ISO 9362 format: 4 letter institution code + 2 letter country code + 2 alphanumeric location code + optional 3 alphanumeric branch code",
                    propertyName,
                    "InvalidStructure");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the country code is a valid ISO 3166-1 alpha-2 code.
    /// ISO 9362 requires the country code to be a valid two-letter ISO 3166-1 alpha-2 country code.
    /// This is a basic validation - a full implementation would check against the complete ISO 3166-1 list.
    /// </summary>
    public static ValueValidator<string> ValidCountryCode(string propertyName = "SwiftCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim().ToUpperInvariant();
            
            if (normalized.Length < 6)
                return ValidationResult.Success(); // Other validators will catch this

            var countryCode = normalized.Substring(4, 2);
            
            // Basic validation: country code must be two uppercase letters (ISO 3166-1 alpha-2)
            if (!CountryCodeRegex().IsMatch(countryCode))
            {
                return ValidationResult.Failure(
                    "SWIFT code contains invalid country code. Country code must be 2 letters (ISO 3166-1 alpha-2) as required by ISO 9362",
                    propertyName,
                    "InvalidCountryCode");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that test SWIFT codes are flagged.
    /// According to ISO 9362, test BICs have specific patterns to distinguish them from live BICs.
    /// Test BICs typically have a location code where the second character is '0'.
    /// Note: This is informational - test codes are technically valid but used for testing only.
    /// </summary>
    public static ValueValidator<string> NotTestCode(string propertyName = "SwiftCode")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim().ToUpperInvariant();
            
            if (normalized.Length < 8)
                return ValidationResult.Success();

            // ISO 9362: Test codes have location code where second character is '0'
            var locationCode = normalized.Substring(6, 2);
            
            // Test pattern: location code of "X0" where X is any character
            if (locationCode[1] == '0')
            {
                return ValidationResult.Failure(
                    "SWIFT code appears to be a test code (location code second character is '0'). Test codes should not be used for real transactions. (ISO 9362)",
                    propertyName,
                    "TestCode");
            }

            return ValidationResult.Success();
        };
    }

    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex SwiftCodeFormatRegex();

    [GeneratedRegex(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$")]
    private static partial Regex SwiftCodeStructureRegex();

    [GeneratedRegex(@"^[A-Z]{2}$")]
    private static partial Regex CountryCodeRegex();
}
