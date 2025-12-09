using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for passport numbers with country-specific format validation.
/// Supports passport formats for 30+ countries according to ICAO Document 9303 standards
/// and country-specific regulations.
/// 
/// Passport formats vary significantly by country:
/// - Length: 6-12 characters (most common: 7-9)
/// - Characters: Alphanumeric, some countries letters only, some digits only
/// - Patterns: Country-specific prefix/suffix rules
/// 
/// Note: This validator checks format compliance, not whether a passport number is actually issued.
/// </summary>
public static partial class PassportValidators
{
    /// <summary>
    /// Validates that the passport number is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "Passport")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "Passport number must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format (alphanumeric characters only, no special characters except spaces/hyphens).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "Passport")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            
            if (!PassportFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "Passport number must contain only letters and digits",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates country-specific passport format.
    /// </summary>
    public static ValueValidator<string> ValidCountryFormat(CountryCode countryCode, string propertyName = "Passport")
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
    /// Validates country-specific passport formats.
    /// Based on ICAO Document 9303 and country-specific regulations.
    /// </summary>
    private static (bool isValid, string errorMessage) ValidateCountrySpecificFormat(string passport, CountryCode countryCode)
    {
        return countryCode switch
        {
            // North America
            CountryCode.UnitedStates => ValidateUsPassport(passport),
            CountryCode.Canada => ValidateCanadaPassport(passport),
            CountryCode.Mexico => ValidateMexicoPassport(passport),

            // UK & Ireland
            CountryCode.UnitedKingdom => ValidateUkPassport(passport),
            CountryCode.Ireland => ValidateIrelandPassport(passport),

            // Western Europe
            CountryCode.Germany => ValidateGermanyPassport(passport),
            CountryCode.France => ValidateFrancePassport(passport),
            CountryCode.Netherlands => ValidateNetherlandsPassport(passport),
            CountryCode.Belgium => ValidateBelgiumPassport(passport),
            CountryCode.Switzerland => ValidateSwitzerlandPassport(passport),
            CountryCode.Austria => ValidateAustriaPassport(passport),

            // Southern Europe
            CountryCode.Italy => ValidateItalyPassport(passport),
            CountryCode.Spain => ValidateSpainPassport(passport),
            CountryCode.Portugal => ValidatePortugalPassport(passport),

            // Northern Europe
            CountryCode.Sweden => ValidateSwedenPassport(passport),
            CountryCode.Norway => ValidateNorwayPassport(passport),
            CountryCode.Denmark => ValidateDenmarkPassport(passport),
            CountryCode.Finland => ValidateFinlandPassport(passport),

            // Eastern Europe
            CountryCode.Poland => ValidatePolandPassport(passport),
            CountryCode.CzechRepublic => ValidateCzechRepublicPassport(passport),
            CountryCode.Hungary => ValidateHungaryPassport(passport),
            CountryCode.Russia => ValidateRussiaPassport(passport),

            // Oceania
            CountryCode.Australia => ValidateAustraliaPassport(passport),
            CountryCode.NewZealand => ValidateNewZealandPassport(passport),

            // Asia
            CountryCode.Japan => ValidateJapanPassport(passport),
            CountryCode.China => ValidateChinaPassport(passport),
            CountryCode.India => ValidateIndiaPassport(passport),
            CountryCode.Singapore => ValidateSingaporePassport(passport),
            CountryCode.SouthKorea => ValidateSouthKoreaPassport(passport),

            // Other
            CountryCode.Brazil => ValidateBrazilPassport(passport),
            CountryCode.SouthAfrica => ValidateSouthAfricaPassport(passport),

            // Unknown or All - use generic validation
            _ => (passport.Length >= 6 && passport.Length <= 12, 
                  $"Passport number for {countryCode} must be 6-12 alphanumeric characters")
        };
    }

    // United States: 9 digits
    private static (bool, string) ValidateUsPassport(string passport)
    {
        if (passport.Length != 9 || !passport.All(char.IsDigit))
            return (false, "US passport must be 9 digits");
        return (true, string.Empty);
    }

    // Canada: 2 letters + 6 digits (e.g., AB123456)
    private static (bool, string) ValidateCanadaPassport(string passport)
    {
        if (passport.Length != 8 || 
            !passport.Substring(0, 2).All(char.IsLetter) ||
            !passport.Substring(2).All(char.IsDigit))
            return (false, "Canada passport must be 2 letters followed by 6 digits");
        return (true, string.Empty);
    }

    // Mexico: 1 letter + 9 digits (e.g., G12345678)
    private static (bool, string) ValidateMexicoPassport(string passport)
    {
        if (passport.Length != 10 ||
            !char.IsLetter(passport[0]) ||
            !passport.Substring(1).All(char.IsDigit))
            return (false, "Mexico passport must be 1 letter followed by 9 digits");
        return (true, string.Empty);
    }

    // UK: 9 characters - 2 digits + 7 alphanumeric (e.g., 123456789)
    private static (bool, string) ValidateUkPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "UK passport must be 9 characters");
        return (true, string.Empty);
    }

    // Ireland: 2 letters + 7 digits (e.g., AB1234567)
    private static (bool, string) ValidateIrelandPassport(string passport)
    {
        if (passport.Length != 9 ||
            !passport.Substring(0, 2).All(char.IsLetter) ||
            !passport.Substring(2).All(char.IsDigit))
            return (false, "Ireland passport must be 2 letters followed by 7 digits");
        return (true, string.Empty);
    }

    // Germany: 10 characters - letter C + 8 digits + check digit (e.g., C01X00T47)
    private static (bool, string) ValidateGermanyPassport(string passport)
    {
        if (passport.Length != 9 && passport.Length != 10)
            return (false, "Germany passport must be 9-10 alphanumeric characters");
        return (true, string.Empty);
    }

    // France: 2 digits + 2 letters + 5 digits (e.g., 12AB12345)
    private static (bool, string) ValidateFrancePassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "France passport must be 9 characters");
        return (true, string.Empty);
    }

    // Netherlands: 2 letters + 6 alphanumeric (e.g., NL123456 or NPABC1234)
    private static (bool, string) ValidateNetherlandsPassport(string passport)
    {
        if (passport.Length != 8 && passport.Length != 9)
            return (false, "Netherlands passport must be 8-9 alphanumeric characters");
        return (true, string.Empty);
    }

    // Belgium: 2 letters + 6 digits (e.g., AB123456)
    private static (bool, string) ValidateBelgiumPassport(string passport)
    {
        if (passport.Length != 8)
            return (false, "Belgium passport must be 8 characters");
        return (true, string.Empty);
    }

    // Switzerland: 1 letter + 7 digits (e.g., X1234567)
    private static (bool, string) ValidateSwitzerlandPassport(string passport)
    {
        if (passport.Length != 8)
            return (false, "Switzerland passport must be 8 characters");
        return (true, string.Empty);
    }

    // Austria: 1 letter + 7 digits (e.g., P1234567)
    private static (bool, string) ValidateAustriaPassport(string passport)
    {
        if (passport.Length != 8)
            return (false, "Austria passport must be 8 characters");
        return (true, string.Empty);
    }

    // Italy: 2 letters + 5 digits + 2 letters (e.g., AA1234567)
    private static (bool, string) ValidateItalyPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "Italy passport must be 9 alphanumeric characters");
        return (true, string.Empty);
    }

    // Spain: 3 letters + 6 digits (e.g., AAA123456)
    private static (bool, string) ValidateSpainPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "Spain passport must be 9 characters");
        return (true, string.Empty);
    }

    // Portugal: 1 letter + 6 digits (e.g., N123456)
    private static (bool, string) ValidatePortugalPassport(string passport)
    {
        if (passport.Length != 7 && passport.Length != 8)
            return (false, "Portugal passport must be 7-8 characters");
        return (true, string.Empty);
    }

    // Sweden: 8 digits
    private static (bool, string) ValidateSwedenPassport(string passport)
    {
        if (passport.Length != 8 || !passport.All(char.IsDigit))
            return (false, "Sweden passport must be 8 digits");
        return (true, string.Empty);
    }

    // Norway: 7 or 8 alphanumeric characters
    private static (bool, string) ValidateNorwayPassport(string passport)
    {
        if (passport.Length < 7 || passport.Length > 8)
            return (false, "Norway passport must be 7-8 alphanumeric characters");
        return (true, string.Empty);
    }

    // Denmark: 9 digits
    private static (bool, string) ValidateDenmarkPassport(string passport)
    {
        if (passport.Length != 9 || !passport.All(char.IsDigit))
            return (false, "Denmark passport must be 9 digits");
        return (true, string.Empty);
    }

    // Finland: 2 letters + 7 digits (e.g., AB1234567)
    private static (bool, string) ValidateFinlandPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "Finland passport must be 9 characters");
        return (true, string.Empty);
    }

    // Poland: 2 letters + 7 digits (e.g., AB1234567)
    private static (bool, string) ValidatePolandPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "Poland passport must be 9 characters");
        return (true, string.Empty);
    }

    // Czech Republic: 8 or 9 digits
    private static (bool, string) ValidateCzechRepublicPassport(string passport)
    {
        if ((passport.Length != 8 && passport.Length != 9) || !passport.All(char.IsDigit))
            return (false, "Czech Republic passport must be 8-9 digits");
        return (true, string.Empty);
    }

    // Hungary: 2 letters + 6 or 7 digits
    private static (bool, string) ValidateHungaryPassport(string passport)
    {
        if (passport.Length < 8 || passport.Length > 9)
            return (false, "Hungary passport must be 8-9 characters");
        return (true, string.Empty);
    }

    // Russia: 2 digits + 7 digits (e.g., 12 1234567)
    private static (bool, string) ValidateRussiaPassport(string passport)
    {
        if (passport.Length != 9 && passport.Length != 10)
            return (false, "Russia passport must be 9-10 digits");
        return (true, string.Empty);
    }

    // Australia: 1 or 2 letters + 7 digits (e.g., N1234567 or PA1234567)
    private static (bool, string) ValidateAustraliaPassport(string passport)
    {
        if (passport.Length < 8 || passport.Length > 9)
            return (false, "Australia passport must be 8-9 characters");
        return (true, string.Empty);
    }

    // New Zealand: 2 letters + 6 digits (e.g., LA123456)
    private static (bool, string) ValidateNewZealandPassport(string passport)
    {
        if (passport.Length != 8)
            return (false, "New Zealand passport must be 8 characters");
        return (true, string.Empty);
    }

    // Japan: 2 letters + 7 digits (e.g., TK1234567)
    private static (bool, string) ValidateJapanPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "Japan passport must be 9 characters");
        return (true, string.Empty);
    }

    // China: 1 letter + 8 digits (e.g., E12345678) or 2 letters + 7 digits (e.g., PE1234567)
    private static (bool, string) ValidateChinaPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "China passport must be 9 characters");
        return (true, string.Empty);
    }

    // India: 1 letter + 7 digits (e.g., K1234567)
    private static (bool, string) ValidateIndiaPassport(string passport)
    {
        if (passport.Length != 8 ||
            !char.IsLetter(passport[0]) ||
            !passport.Substring(1).All(char.IsDigit))
            return (false, "India passport must be 1 letter followed by 7 digits");
        return (true, string.Empty);
    }

    // Singapore: 1 letter + 7 digits + 1 letter (e.g., K1234567A)
    private static (bool, string) ValidateSingaporePassport(string passport)
    {
        if (passport.Length != 9 ||
            !char.IsLetter(passport[0]) ||
            !char.IsLetter(passport[8]))
            return (false, "Singapore passport must be 1 letter + 7 digits + 1 letter");
        return (true, string.Empty);
    }

    // South Korea: 2 letters + 7 digits (e.g., M12345678) or 9 characters
    private static (bool, string) ValidateSouthKoreaPassport(string passport)
    {
        if (passport.Length != 9)
            return (false, "South Korea passport must be 9 characters");
        return (true, string.Empty);
    }

    // Brazil: 2 letters + 6 digits (e.g., AB123456)
    private static (bool, string) ValidateBrazilPassport(string passport)
    {
        if (passport.Length != 8 && passport.Length != 9)
            return (false, "Brazil passport must be 8-9 characters");
        return (true, string.Empty);
    }

    // South Africa: 1 letter + 8 digits (e.g., A12345678)
    private static (bool, string) ValidateSouthAfricaPassport(string passport)
    {
        if (passport.Length != 9 ||
            !char.IsLetter(passport[0]) ||
            !passport.Substring(1).All(char.IsDigit))
            return (false, "South Africa passport must be 1 letter followed by 8 digits");
        return (true, string.Empty);
    }

    [GeneratedRegex(@"^[A-Z0-9]+$")]
    private static partial Regex PassportFormatRegex();
}
