using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for IBAN (International Bank Account Number) and BBAN (Basic Bank Account Number) according to ISO 13616.
/// Supports automatic detection of IBAN vs BBAN format and country-specific validation.
/// 
/// ISO 13616 IBAN Format: CCKKbbbbbbbbbbbbbbbbbbbbbbbbbb where:
/// - CC: Country code (2 letters, ISO 3166-1 alpha-2)
/// - KK: Check digits (2 digits, calculated using mod-97)
/// - b: BBAN (Basic Bank Account Number, up to 30 alphanumeric characters, country-specific)
/// 
/// Supported IBAN Countries with lengths:
/// - AD (Andorra): 24, AE (UAE): 23, AL (Albania): 28, AT (Austria): 20
/// - BA (Bosnia): 20, BE (Belgium): 16, BG (Bulgaria): 22, CH (Switzerland): 21
/// - CY (Cyprus): 28, CZ (Czech Republic): 24, DE (Germany): 22, DK (Denmark): 18
/// - EE (Estonia): 20, ES (Spain): 24, FI (Finland): 18, FR (France): 27
/// - GB (UK): 22, GI (Gibraltar): 23, GR (Greece): 27, HR (Croatia): 21
/// - HU (Hungary): 28, IE (Ireland): 22, IL (Israel): 23, IS (Iceland): 26
/// - IT (Italy): 27, LI (Liechtenstein): 21, LT (Lithuania): 20, LU (Luxembourg): 20
/// - LV (Latvia): 21, MC (Monaco): 27, ME (Montenegro): 22, MK (Macedonia): 19
/// - MT (Malta): 31, NL (Netherlands): 18, NO (Norway): 15, PL (Poland): 28
/// - PT (Portugal): 25, RO (Romania): 24, RS (Serbia): 22, SE (Sweden): 24
/// - SI (Slovenia): 19, SK (Slovakia): 24, SM (San Marino): 27, TR (Turkey): 26
/// 
/// Reference: ISO 13616:2020 - Financial services — International bank account number (IBAN)
/// </summary>
public static partial class IbanNumberValidators
{
    private static readonly Dictionary<string, int> IbanCountryLengths = new()
    {
        { "AD", 24 }, { "AE", 23 }, { "AL", 28 }, { "AT", 20 },
        { "AZ", 28 }, { "BA", 20 }, { "BE", 16 }, { "BG", 22 },
        { "BH", 22 }, { "BR", 29 }, { "BY", 28 }, { "CH", 21 },
        { "CR", 22 }, { "CY", 28 }, { "CZ", 24 }, { "DE", 22 },
        { "DK", 18 }, { "DO", 28 }, { "EE", 20 }, { "EG", 29 },
        { "ES", 24 }, { "FI", 18 }, { "FO", 18 }, { "FR", 27 },
        { "GB", 22 }, { "GE", 22 }, { "GI", 23 }, { "GL", 18 },
        { "GR", 27 }, { "GT", 28 }, { "HR", 21 }, { "HU", 28 },
        { "IE", 22 }, { "IL", 23 }, { "IS", 26 }, { "IT", 27 },
        { "JO", 30 }, { "KW", 30 }, { "KZ", 20 }, { "LB", 28 },
        { "LC", 32 }, { "LI", 21 }, { "LT", 20 }, { "LU", 20 },
        { "LV", 21 }, { "MC", 27 }, { "MD", 24 }, { "ME", 22 },
        { "MK", 19 }, { "MR", 27 }, { "MT", 31 }, { "MU", 30 },
        { "NL", 18 }, { "NO", 15 }, { "PK", 24 }, { "PL", 28 },
        { "PS", 29 }, { "PT", 25 }, { "QA", 29 }, { "RO", 24 },
        { "RS", 22 }, { "SA", 24 }, { "SE", 24 }, { "SI", 19 },
        { "SK", 24 }, { "SM", 27 }, { "TN", 24 }, { "TR", 26 },
        { "UA", 29 }, { "VA", 22 }, { "VG", 24 }, { "XK", 20 }
    };

    /// <summary>
    /// Validates that the account number is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "IbanNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "Account number must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format (alphanumeric characters only).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "IbanNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            if (!AccountNumberFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "Account number must contain only letters and digits",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Detects whether the account number is IBAN or BBAN format.
    /// </summary>
    public static BankAccountNumberType DetectAccountType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BankAccountNumberType.Unknown;

        var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();

        // IBAN: starts with 2 letters (country code) followed by 2 digits (check digits)
        if (normalized.Length >= 4 &&
            char.IsLetter(normalized[0]) &&
            char.IsLetter(normalized[1]) &&
            char.IsDigit(normalized[2]) &&
            char.IsDigit(normalized[3]))
        {
            // Check if it's a known IBAN country code
            var countryCode = normalized.Substring(0, 2);
            if (IbanCountryLengths.ContainsKey(countryCode))
            {
                return BankAccountNumberType.Iban;
            }
        }

        // If it's all digits or doesn't match IBAN pattern, it's likely BBAN
        if (normalized.All(char.IsDigit) || !normalized.Take(2).All(char.IsLetter))
        {
            return BankAccountNumberType.Bban;
        }

        return BankAccountNumberType.Unknown;
    }

    /// <summary>
    /// Validates IBAN format and structure according to ISO 13616.
    /// </summary>
    public static ValueValidator<string> ValidIbanFormat(string propertyName = "IbanNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            // Must start with 2 letters (country code) and 2 digits (check digits)
            if (!IbanStructureRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "IBAN must start with 2 letter country code and 2 check digits (ISO 13616)",
                    propertyName,
                    "InvalidIbanStructure");
            }

            var countryCode = normalized.Substring(0, 2);
            
            // Validate country code exists
            if (!IbanCountryLengths.ContainsKey(countryCode))
            {
                return ValidationResult.Failure(
                    $"IBAN country code '{countryCode}' is not recognized (ISO 13616)",
                    propertyName,
                    "InvalidIbanCountryCode");
            }

            // Validate length for country
            var expectedLength = IbanCountryLengths[countryCode];
            if (normalized.Length != expectedLength)
            {
                return ValidationResult.Failure(
                    $"IBAN for country {countryCode} must be {expectedLength} characters long, but was {normalized.Length}",
                    propertyName,
                    "InvalidIbanLength");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates IBAN checksum using mod-97 algorithm (ISO 13616).
    /// </summary>
    public static ValueValidator<string> ValidIbanChecksum(string propertyName = "IbanNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            if (normalized.Length < 4)
                return ValidationResult.Success(); // Other validators will catch this

            if (!CalculateIbanChecksum(normalized))
            {
                return ValidationResult.Failure(
                    "IBAN checksum is invalid (ISO 13616 mod-97 validation failed)",
                    propertyName,
                    "InvalidIbanChecksum");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates BBAN format (country-specific domestic account number).
    /// </summary>
    public static ValueValidator<string> ValidBbanFormat(string propertyName = "IbanNumber", CountryCode? countryCode = null)
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            // Basic BBAN validation - typically 4-34 characters, mostly digits
            if (normalized.Length < 4 || normalized.Length > 34)
            {
                return ValidationResult.Failure(
                    "BBAN must be between 4 and 34 characters",
                    propertyName,
                    "InvalidBbanLength");
            }

            // Country-specific BBAN validation
            if (countryCode.HasValue)
            {
                var result = ValidateCountrySpecificBban(normalized, countryCode.Value);
                if (!result.isValid)
                {
                    return ValidationResult.Failure(
                        result.errorMessage,
                        propertyName,
                        "InvalidBbanFormat");
                }
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Calculates IBAN checksum using mod-97 algorithm.
    /// </summary>
    private static bool CalculateIbanChecksum(string iban)
    {
        // Move first 4 characters to end
        var rearranged = iban.Substring(4) + iban.Substring(0, 4);
        
        // Convert letters to numbers (A=10, B=11, ..., Z=35)
        var numericString = string.Empty;
        foreach (var c in rearranged)
        {
            if (char.IsLetter(c))
            {
                numericString += (c - 'A' + 10).ToString();
            }
            else
            {
                numericString += c;
            }
        }

        // Calculate mod 97
        var remainder = 0;
        foreach (var c in numericString)
        {
            remainder = (remainder * 10 + (c - '0')) % 97;
        }

        return remainder == 1;
    }

    /// <summary>
    /// Validates country-specific BBAN formats.
    /// </summary>
    private static (bool isValid, string errorMessage) ValidateCountrySpecificBban(string bban, CountryCode countryCode)
    {
        return countryCode switch
        {
            CountryCode.UnitedKingdom => ValidateUkBban(bban),
            CountryCode.UnitedStates => ValidateUsBban(bban),
            CountryCode.Germany => (bban.Length == 18, "German BBAN must be 18 characters"),
            CountryCode.France => (bban.Length == 23, "French BBAN must be 23 characters"),
            CountryCode.Netherlands => (bban.Length == 14, "Dutch BBAN must be 14 characters"),
            CountryCode.Spain => (bban.Length == 20, "Spanish BBAN must be 20 characters"),
            CountryCode.Italy => (bban.Length == 23, "Italian BBAN must be 23 characters"),
            _ => (true, string.Empty) // Other countries - allow any valid format
        };
    }

    private static (bool isValid, string errorMessage) ValidateUkBban(string bban)
    {
        // UK: 8 digit account number
        if (bban.Length != 8 || !bban.All(char.IsDigit))
        {
            return (false, "UK BBAN must be 8 digits");
        }
        return (true, string.Empty);
    }

    private static (bool isValid, string errorMessage) ValidateUsBban(string bban)
    {
        // US: typically 4-17 digits
        if (bban.Length < 4 || bban.Length > 17 || !bban.All(char.IsDigit))
        {
            return (false, "US BBAN must be 4-17 digits");
        }
        return (true, string.Empty);
    }

    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex AccountNumberFormatRegex();

    [GeneratedRegex(@"^[A-Z]{2}\d{2}[A-Z0-9]+$")]
    private static partial Regex IbanStructureRegex();
}
