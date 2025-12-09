using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for driving license numbers with country-specific format validation.
/// Supports driving license formats for 20+ countries with their specific regulations.
/// 
/// Driving license formats vary significantly by country:
/// - Length: 5-16 characters (most common: 8-15)
/// - Characters: Alphanumeric, some countries use specific patterns
/// - Patterns: Country-specific prefix/suffix rules, check digits
/// 
/// Note: This validator checks format compliance, not whether a license is actually issued or valid.
/// </summary>
public static partial class DrivingLicenseNumberValidators
{
    /// <summary>
    /// Validates that the driving license number is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "DrivingLicenseNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "Driving license number must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format (alphanumeric characters only, no special characters except spaces/hyphens).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "DrivingLicenseNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            if (!LicenseFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "Driving license number must contain only letters and digits",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates country-specific driving license format.
    /// </summary>
    public static ValueValidator<string> ValidCountryFormat(CountryCode countryCode, string propertyName = "DrivingLicenseNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            var (isValid, errorMessage) = ValidateCountrySpecificFormat(normalized, countryCode);
            
            if (!isValid)
            {
                return ValidationResult.Failure(
                    errorMessage,
                    propertyName,
                    "InvalidCountryFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates country-specific driving license formats.
    /// Based on national regulations and common formats.
    /// </summary>
    private static (bool isValid, string errorMessage) ValidateCountrySpecificFormat(string license, CountryCode countryCode)
    {
        return countryCode switch
        {
            // North America
            CountryCode.UnitedStates => ValidateUsLicense(license),
            CountryCode.Canada => ValidateCanadaLicense(license),
            CountryCode.Mexico => ValidateMexicoLicense(license),

            // Europe
            CountryCode.UnitedKingdom => ValidateUkLicense(license),
            CountryCode.Germany => ValidateGermanyLicense(license),
            CountryCode.France => ValidateFranceLicense(license),
            CountryCode.Italy => ValidateItalyLicense(license),
            CountryCode.Spain => ValidateSpainLicense(license),
            CountryCode.Netherlands => ValidateNetherlandsLicense(license),
            CountryCode.Belgium => ValidateBelgiumLicense(license),
            CountryCode.Sweden => ValidateSwedenLicense(license),
            CountryCode.Norway => ValidateNorwayLicense(license),
            CountryCode.Denmark => ValidateDenmarkLicense(license),
            CountryCode.Finland => ValidateFinlandLicense(license),
            CountryCode.Austria => ValidateAustriaLicense(license),
            CountryCode.Switzerland => ValidateSwitzerlandLicense(license),
            CountryCode.Poland => ValidatePolandLicense(license),
            CountryCode.Ireland => ValidateIrelandLicense(license),

            // Asia-Pacific
            CountryCode.Australia => ValidateAustraliaLicense(license),
            CountryCode.NewZealand => ValidateNewZealandLicense(license),
            CountryCode.Japan => ValidateJapanLicense(license),
            CountryCode.Singapore => ValidateSingaporeLicense(license),
            CountryCode.SouthKorea => ValidateSouthKoreaLicense(license),
            CountryCode.India => ValidateIndiaLicense(license),

            // Other
            CountryCode.Brazil => ValidateBrazilLicense(license),
            CountryCode.SouthAfrica => ValidateSouthAfricaLicense(license),

            _ => (true, string.Empty) // Unknown countries pass validation
        };
    }

    // United States: Varies by state, typically 7-12 alphanumeric characters
    private static (bool, string) ValidateUsLicense(string license)
    {
        if (license.Length < 5 || license.Length > 16)
            return (false, "United States driving license must be 5-16 characters");
        return (true, string.Empty);
    }

    // Canada: Varies by province, typically 12-16 alphanumeric characters
    private static (bool, string) ValidateCanadaLicense(string license)
    {
        if (license.Length < 8 || license.Length > 16)
            return (false, "Canada driving license must be 8-16 characters");
        return (true, string.Empty);
    }

    // Mexico: 16-18 alphanumeric characters
    private static (bool, string) ValidateMexicoLicense(string license)
    {
        if (license.Length < 16 || license.Length > 18)
            return (false, "Mexico driving license must be 16-18 characters");
        return (true, string.Empty);
    }

    // United Kingdom: 16 characters (5 letters, 6 digits, 2 letters, 2 digits, 1 letter)
    // Format: MORGA657054SM9IJ (Surname initials + date info + initials + check digits)
    private static (bool, string) ValidateUkLicense(string license)
    {
        if (license.Length != 16)
            return (false, "United Kingdom driving license must be exactly 16 characters");
        
        if (!char.IsLetter(license[0]) || !char.IsLetter(license[1]) || !char.IsLetter(license[2]) ||
            !char.IsLetter(license[3]) || !char.IsLetter(license[4]))
            return (false, "United Kingdom driving license must start with 5 letters");
        
        return (true, string.Empty);
    }

    // Germany: 11 alphanumeric characters (EU format)
    private static (bool, string) ValidateGermanyLicense(string license)
    {
        if (license.Length != 11)
            return (false, "Germany driving license must be exactly 11 characters");
        return (true, string.Empty);
    }

    // France: 12 digits
    private static (bool, string) ValidateFranceLicense(string license)
    {
        if (license.Length != 12 || !license.All(char.IsDigit))
            return (false, "France driving license must be exactly 12 digits");
        return (true, string.Empty);
    }

    // Italy: 10 alphanumeric characters (2 letters + 7 digits + 1 letter)
    private static (bool, string) ValidateItalyLicense(string license)
    {
        if (license.Length != 10)
            return (false, "Italy driving license must be exactly 10 characters");
        return (true, string.Empty);
    }

    // Spain: 8 digits + 1 letter
    private static (bool, string) ValidateSpainLicense(string license)
    {
        if (license.Length != 9)
            return (false, "Spain driving license must be exactly 9 characters");
        
        if (!license.Substring(0, 8).All(char.IsDigit) || !char.IsLetter(license[8]))
            return (false, "Spain driving license must be 8 digits followed by 1 letter");
        
        return (true, string.Empty);
    }

    // Netherlands: 10 digits
    private static (bool, string) ValidateNetherlandsLicense(string license)
    {
        if (license.Length != 10 || !license.All(char.IsDigit))
            return (false, "Netherlands driving license must be exactly 10 digits");
        return (true, string.Empty);
    }

    // Belgium: 10 digits (EU format)
    private static (bool, string) ValidateBelgiumLicense(string license)
    {
        if (license.Length != 10 || !license.All(char.IsDigit))
            return (false, "Belgium driving license must be exactly 10 digits");
        return (true, string.Empty);
    }

    // Sweden: 8-13 alphanumeric characters
    private static (bool, string) ValidateSwedenLicense(string license)
    {
        if (license.Length < 8 || license.Length > 13)
            return (false, "Sweden driving license must be 8-13 characters");
        return (true, string.Empty);
    }

    // Norway: 11 alphanumeric characters
    private static (bool, string) ValidateNorwayLicense(string license)
    {
        if (license.Length != 11)
            return (false, "Norway driving license must be exactly 11 characters");
        return (true, string.Empty);
    }

    // Denmark: 8-10 digits
    private static (bool, string) ValidateDenmarkLicense(string license)
    {
        if (license.Length < 8 || license.Length > 10)
            return (false, "Denmark driving license must be 8-10 characters");
        return (true, string.Empty);
    }

    // Finland: 12 alphanumeric characters
    private static (bool, string) ValidateFinlandLicense(string license)
    {
        if (license.Length != 12)
            return (false, "Finland driving license must be exactly 12 characters");
        return (true, string.Empty);
    }

    // Austria: 8 digits
    private static (bool, string) ValidateAustriaLicense(string license)
    {
        if (license.Length != 8 || !license.All(char.IsDigit))
            return (false, "Austria driving license must be exactly 8 digits");
        return (true, string.Empty);
    }

    // Switzerland: 8-9 alphanumeric characters
    private static (bool, string) ValidateSwitzerlandLicense(string license)
    {
        if (license.Length < 8 || license.Length > 9)
            return (false, "Switzerland driving license must be 8-9 characters");
        return (true, string.Empty);
    }

    // Poland: 5 digits + 6 letters (e.g., 12345 ABCDEF)
    private static (bool, string) ValidatePolandLicense(string license)
    {
        if (license.Length != 13)
            return (false, "Poland driving license must be exactly 13 characters");
        return (true, string.Empty);
    }

    // Ireland: 8-9 digits
    private static (bool, string) ValidateIrelandLicense(string license)
    {
        if (license.Length < 8 || license.Length > 9)
            return (false, "Ireland driving license must be 8-9 characters");
        return (true, string.Empty);
    }

    // Australia: Varies by state, typically 6-10 alphanumeric characters
    private static (bool, string) ValidateAustraliaLicense(string license)
    {
        if (license.Length < 6 || license.Length > 10)
            return (false, "Australia driving license must be 6-10 characters");
        return (true, string.Empty);
    }

    // New Zealand: 8 alphanumeric characters (2 letters + 6 digits)
    private static (bool, string) ValidateNewZealandLicense(string license)
    {
        if (license.Length != 8)
            return (false, "New Zealand driving license must be exactly 8 characters");
        return (true, string.Empty);
    }

    // Japan: 12 digits
    private static (bool, string) ValidateJapanLicense(string license)
    {
        if (license.Length != 12 || !license.All(char.IsDigit))
            return (false, "Japan driving license must be exactly 12 digits");
        return (true, string.Empty);
    }

    // Singapore: 7-8 alphanumeric characters (1 letter + 7 digits + optional letter)
    private static (bool, string) ValidateSingaporeLicense(string license)
    {
        if (license.Length < 7 || license.Length > 8)
            return (false, "Singapore driving license must be 7-8 characters");
        
        if (!char.IsLetter(license[0]))
            return (false, "Singapore driving license must start with a letter");
        
        return (true, string.Empty);
    }

    // South Korea: 12-14 characters (varies by region)
    private static (bool, string) ValidateSouthKoreaLicense(string license)
    {
        if (license.Length < 12 || license.Length > 14)
            return (false, "South Korea driving license must be 12-14 characters");
        return (true, string.Empty);
    }

    // India: Varies by state, typically 13-16 alphanumeric characters (e.g., DL-1420110012345)
    private static (bool, string) ValidateIndiaLicense(string license)
    {
        if (license.Length < 13 || license.Length > 16)
            return (false, "India driving license must be 13-16 characters");
        return (true, string.Empty);
    }

    // Brazil: 11 digits
    private static (bool, string) ValidateBrazilLicense(string license)
    {
        if (license.Length != 11 || !license.All(char.IsDigit))
            return (false, "Brazil driving license must be exactly 11 digits");
        return (true, string.Empty);
    }

    // South Africa: 8-13 alphanumeric characters
    private static (bool, string) ValidateSouthAfricaLicense(string license)
    {
        if (license.Length < 8 || license.Length > 13)
            return (false, "South Africa driving license must be 8-13 characters");
        return (true, string.Empty);
    }

    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex LicenseFormatRegex();
}
