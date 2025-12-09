using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated phone number with country code support.
/// Supports phone number formats from countries worldwide.
/// </summary>
[JsonConverter(typeof(PhoneNumberConverter))]
public sealed record PhoneNumber : ValidatedPrimitive<string>
{
    /// <summary>
    /// Gets the country code for the phone number.
    /// </summary>
    public CountryCode CountryCode { get; }

    private PhoneNumber(string value, CountryCode countryCode, string propertyName = "PhoneNumber") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(PhoneValidators.ValidFormat(propertyName));
        Validators.Add(CommonValidators.Length(propertyName, 7, 20));

        CountryCode = countryCode;

        if (countryCode != CountryCode.Unknown && countryCode != CountryCode.All)
        {
            Validators.Add(PhoneValidators.ValidateCountryFormat(propertyName, countryCode));
        }
    }

    /// <summary>
    /// Attempts to create a PhoneNumber instance with validation.
    /// </summary>
    /// <param name="countryCode">The country code for the phone number.</param>
    /// <param name="value">The phone number string to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the PhoneNumber instance if valid.</returns>
    public static (ValidationResult Result, PhoneNumber? Value) TryCreate(
        CountryCode countryCode,
        string value,
        string propertyName = "PhoneNumber")
    {
        var phoneNumber = new PhoneNumber(value, countryCode, propertyName);
        var validationResult = phoneNumber.Validate();
        var result = validationResult.IsValid ? phoneNumber : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets a display-friendly name for the country.
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
    /// Returns the phone number value as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether the specified PhoneNumber is equal to the current PhoneNumber.
    /// </summary>
    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && CountryCode == other.CountryCode;
    }

    /// <summary>
    /// Returns the hash code for this PhoneNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, CountryCode);
    }
}
