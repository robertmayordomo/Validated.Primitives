using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated postal code with automatic country code detection.
/// Supports postal code formats from countries worldwide and automatically
/// identifies the country based on format patterns.
/// </summary>
public sealed record PostalCode : ValidatedValueObject<string>
{
    /// <summary>
    /// Gets the detected country code based on the postal code format.
    /// </summary>
    public CountryCode CountryCode { get; }

    private PostalCode(string value, CountryCode countryCode, string propertyName = "PostalCode") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(PostalCodeValidators.ValidFormat(propertyName));
        Validators.Add(CommonValidators.Length(propertyName, 2, 10));
        
        CountryCode = countryCode;
        
        if (countryCode != CountryCode.Unknown && countryCode != CountryCode.All)
        {
            Validators.Add(PostalCodeValidators.ValidateCountryFormat(propertyName, countryCode));
        }
    }

    /// <summary>
    /// Attempts to create a PostalCode instance with validation and automatic country detection.
    /// </summary>
    /// <param name="value">The postal code string to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the PostalCode instance if valid.</returns>
    public static (ValidationResult Result, PostalCode? Value) TryCreate(
        CountryCode countryCode,
        string value, 
        string propertyName = "PostalCode")
    {
        var postalCode = new PostalCode(value, countryCode, propertyName);
        var validationResult = postalCode.Validate();
        var result = validationResult.IsValid ? postalCode : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets a display-friendly name for the detected country.
    /// </summary>
    public string GetCountryName() => CountryCode switch
    {
        CountryCode.All => "All Countries",
        CountryCode.CzechRepublic => "Czech Republic",
        CountryCode.NewZealand => "New Zealand",
        CountryCode.SouthAfrica => "South Africa",
        CountryCode.SouthKorea => "South Korea",
        CountryCode.UnitedKingdom => "United Kingdom",
        CountryCode.UnitedStates => "United States",
        _ => CountryCode.ToString()
    };

    /// <summary>
    /// Returns the postal code value as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether the specified PostalCode is equal to the current PostalCode.
    /// </summary>
    public bool Equals(PostalCode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && CountryCode == other.CountryCode;
    }

    /// <summary>
    /// Returns the hash code for this PostalCode.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, CountryCode);
    }
}
