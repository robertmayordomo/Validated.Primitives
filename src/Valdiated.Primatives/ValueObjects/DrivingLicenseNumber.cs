using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated driving license number with country-specific format validation.
/// 
/// Validates driving license formats according to national regulations for 25+ countries.
/// Each country has specific format requirements:
/// 
/// - United States: 5-16 alphanumeric (varies by state)
/// - Canada: 8-16 alphanumeric (varies by province)
/// - United Kingdom: 16 characters (specific pattern)
/// - Germany: 11 alphanumeric characters (EU format)
/// - France: 12 digits
/// - And many more...
/// 
/// Note: This validates format only, not whether the license is actually issued or currently valid.
/// </summary>
[JsonConverter(typeof(DrivingLicenseNumberConverter))]
public sealed record DrivingLicenseNumber : ValidatedValueObject<string>
{
    /// <summary>
    /// Gets the country that issued this driving license.
    /// </summary>
    public CountryCode IssuingCountry { get; }

    private DrivingLicenseNumber(string value, CountryCode issuingCountry, string propertyName = "DrivingLicenseNumber") : base(value)
    {
        IssuingCountry = issuingCountry;
        
        Validators.Add(DrivingLicenseNumberValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(DrivingLicenseNumberValidators.ValidFormat(propertyName));
        Validators.Add(DrivingLicenseNumberValidators.ValidCountryFormat(issuingCountry, propertyName));
    }

    /// <summary>
    /// Attempts to create a DrivingLicenseNumber instance with validation.
    /// </summary>
    /// <param name="value">The driving license number.</param>
    /// <param name="issuingCountry">The country that issued the license.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the DrivingLicenseNumber instance if valid.</returns>
    public static (ValidationResult Result, DrivingLicenseNumber? Value) TryCreate(
        string value,
        CountryCode issuingCountry,
        string propertyName = "DrivingLicenseNumber")
    {
        var license = new DrivingLicenseNumber(value, issuingCountry, propertyName);
        var validationResult = license.Validate();
        var result = validationResult.IsValid ? license : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the license number without spaces or hyphens in uppercase.
    /// </summary>
    public string ToNormalizedString()
    {
        return Value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Returns the license number in formatted display format if applicable.
    /// Some countries have standard formatting patterns.
    /// </summary>
    public string ToFormattedString()
    {
        var normalized = ToNormalizedString();
        
        return IssuingCountry switch
        {
            // UK: MORGA 657054 SM9IJ (5 letters + space + 6 digits + space + 5 chars)
            CountryCode.UnitedKingdom when normalized.Length == 16 =>
                $"{normalized.Substring(0, 5)} {normalized.Substring(5, 6)} {normalized.Substring(11)}",
            
            // Spain: 12345678-A (8 digits + dash + letter)
            CountryCode.Spain when normalized.Length == 9 =>
                $"{normalized.Substring(0, 8)}-{normalized.Substring(8)}",
            
            // Poland: 12345 ABCDEF (5 digits + space + 6 letters) - assumes 13 chars total
            CountryCode.Poland when normalized.Length == 13 =>
                $"{normalized.Substring(0, 5)} {normalized.Substring(5)}",
            
            // India: DL-14 20110012345 (state code - region - year - number)
            CountryCode.India when normalized.Length >= 13 && normalized.Length <= 16 =>
                FormatIndiaLicense(normalized),
            
            // Default: no formatting
            _ => normalized
        };
    }

    private static string FormatIndiaLicense(string normalized)
    {
        // Try to format as: DL-14 20110012345 (assumes starts with state code)
        if (normalized.Length >= 13)
        {
            // Basic format: first part (state) + rest
            return normalized.Length switch
            {
                13 => $"{normalized.Substring(0, 4)}-{normalized.Substring(4)}",
                14 => $"{normalized.Substring(0, 4)}-{normalized.Substring(4)}",
                15 => $"{normalized.Substring(0, 4)}-{normalized.Substring(4)}",
                16 => $"{normalized.Substring(0, 4)}-{normalized.Substring(4)}",
                _ => normalized
            };
        }
        return normalized;
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
    /// Gets the license class/category if it can be determined from the license number.
    /// Some countries encode the license class in the number format.
    /// </summary>
    public string LicenseClass
    {
        get
        {
            var normalized = ToNormalizedString();
            
            return IssuingCountry switch
            {
                // UK licenses have class info in specific positions, but not directly in the number
                // Most countries don't encode class in the license number itself
                _ => "Standard"
            };
        }
    }

    /// <summary>
    /// Returns the license number value in normalized format.
    /// </summary>
    public override string ToString() => ToNormalizedString();

    /// <summary>
    /// Determines whether the specified DrivingLicenseNumber is equal to the current DrivingLicenseNumber.
    /// Comparison is case-insensitive and ignores formatting (spaces/hyphens).
    /// </summary>
    public bool Equals(DrivingLicenseNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ToNormalizedString() == other.ToNormalizedString() && 
               IssuingCountry == other.IssuingCountry;
    }

    /// <summary>
    /// Returns the hash code for this DrivingLicenseNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(ToNormalizedString(), IssuingCountry);
    }
}
