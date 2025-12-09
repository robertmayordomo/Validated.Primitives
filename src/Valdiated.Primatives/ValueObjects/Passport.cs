using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated passport number with country-specific format validation.
/// 
/// Validates passport formats according to ICAO Document 9303 and country-specific regulations.
/// Supports 30+ countries with their specific passport number formats:
/// 
/// - United States: 9 digits
/// - Canada: 2 letters + 6 digits
/// - United Kingdom: 9 alphanumeric characters
/// - Germany: 9-10 alphanumeric characters
/// - And many more...
/// 
/// Note: This validates format only, not whether the passport is actually issued.
/// 
/// Reference: ICAO Document 9303 - Machine Readable Travel Documents
/// </summary>
[JsonConverter(typeof(PassportConverter))]
public sealed record Passport : ValidatedPrimitive<string>
{
    /// <summary>
    /// Gets the country that issued this passport.
    /// </summary>
    public CountryCode IssuingCountry { get; }

    private Passport(string value, CountryCode issuingCountry, string propertyName = "Passport") : base(value)
    {
        IssuingCountry = issuingCountry;
        
        Validators.Add(PassportValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(PassportValidators.ValidFormat(propertyName));
        Validators.Add(PassportValidators.ValidCountryFormat(issuingCountry, propertyName));
    }

    /// <summary>
    /// Attempts to create a Passport instance with validation.
    /// </summary>
    /// <param name="value">The passport number.</param>
    /// <param name="issuingCountry">The country that issued the passport.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Passport instance if valid.</returns>
    public static (ValidationResult Result, Passport? Value) TryCreate(
        string value,
        CountryCode issuingCountry,
        string propertyName = "Passport")
    {
        var passport = new Passport(value, issuingCountry, propertyName);
        var validationResult = passport.Validate();
        var result = validationResult.IsValid ? passport : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the passport number without spaces or hyphens in uppercase.
    /// </summary>
    public string ToNormalizedString()
    {
        return Value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Returns the passport number in formatted display format if applicable.
    /// Some countries have standard formatting (e.g., Canada: AB 123456).
    /// </summary>
    public string ToFormattedString()
    {
        var normalized = ToNormalizedString();
        
        return IssuingCountry switch
        {
            // Canada: AB 123456
            CountryCode.Canada when normalized.Length == 8 =>
                $"{normalized.Substring(0, 2)} {normalized.Substring(2)}",
            
            // UK: XXX XXX XXX (groups of 3)
            CountryCode.UnitedKingdom when normalized.Length == 9 =>
                $"{normalized.Substring(0, 3)} {normalized.Substring(3, 3)} {normalized.Substring(6)}",
            
            // Russia: XX XXXXXXX
            CountryCode.Russia when normalized.Length == 9 =>
                $"{normalized.Substring(0, 2)} {normalized.Substring(2)}",
            
            // Default: no formatting
            _ => normalized
        };
    }

    /// <summary>
    /// Returns a masked version showing only the last 4 characters for security.
    /// </summary>
    public string Masked()
    {
        var normalized = ToNormalizedString();
        if (normalized.Length <= 4)
            return new string('*', normalized.Length);

        var lastFour = normalized.Substring(normalized.Length - 4);
        var masked = new string('*', normalized.Length - 4);
        return masked + lastFour;
    }

    /// <summary>
    /// Gets the passport type based on country patterns.
    /// Some countries use prefixes to denote passport type (e.g., 'P' for regular passport).
    /// </summary>
    public string PassportType
    {
        get
        {
            var normalized = ToNormalizedString();
            
            return IssuingCountry switch
            {
                // Countries with letter prefix typically indicating type
                CountryCode.Australia when normalized.Length >= 8 && char.IsLetter(normalized[0]) => 
                    normalized[0] == 'P' ? "Regular Passport" : 
                    normalized[0] == 'D' ? "Diplomatic Passport" :
                    normalized[0] == 'N' ? "Regular Passport" : "Other",
                
                CountryCode.Germany when normalized.Length >= 9 && normalized.StartsWith("C") => "Regular Passport (C-series)",
                
                _ => "Regular Passport"
            };
        }
    }

    /// <summary>
    /// Returns the passport number value in normalized format.
    /// </summary>
    public override string ToString() => ToNormalizedString();

    /// <summary>
    /// Determines whether the specified Passport is equal to the current Passport.
    /// Comparison is case-insensitive and ignores formatting (spaces/hyphens).
    /// </summary>
    public bool Equals(Passport? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ToNormalizedString() == other.ToNormalizedString() && 
               IssuingCountry == other.IssuingCountry;
    }

    /// <summary>
    /// Returns the hash code for this Passport.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(ToNormalizedString(), IssuingCountry);
    }
}
