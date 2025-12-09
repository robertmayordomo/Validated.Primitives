using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated bank sort code with country-specific format validation.
/// Sort codes are primarily used in the UK and Ireland to identify bank branches.
/// UK/Ireland format: 6 digits, often displayed as XX-XX-XX.
/// </summary>
[JsonConverter(typeof(SortCodeConverter))]
public sealed record SortCode : ValidatedPrimitive<string>
{
    /// <summary>
    /// Gets the country code for which this sort code is validated.
    /// </summary>
    public CountryCode CountryCode { get; }

    private SortCode(string value, CountryCode countryCode, string propertyName = "SortCode") : base(value)
    {
        Validators.Add(SortCodeValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(SortCodeValidators.ValidFormat(propertyName));
        
        CountryCode = countryCode;
        
        // Only apply digit and country-specific validation for countries that use sort codes
        if (countryCode == CountryCode.UnitedKingdom || countryCode == CountryCode.Ireland)
        {
            Validators.Add(SortCodeValidators.OnlyDigits(propertyName));
            Validators.Add(SortCodeValidators.ValidateCountryFormat(propertyName, countryCode));
        }
    }

    /// <summary>
    /// Attempts to create a SortCode instance with validation.
    /// </summary>
    /// <param name="countryCode">The country code for format validation.</param>
    /// <param name="value">The sort code value.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the SortCode instance if valid.</returns>
    public static (ValidationResult Result, SortCode? Value) TryCreate(
        CountryCode countryCode,
        string value,
        string propertyName = "SortCode")
    {
        var sortCode = new SortCode(value, countryCode, propertyName);
        var validationResult = sortCode.Validate();
        var result = validationResult.IsValid ? sortCode : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the sort code in standardized format (digits only).
    /// </summary>
    public string ToDigitsOnly()
    {
        return Value.Replace("-", "").Replace(" ", "");
    }

    /// <summary>
    /// Returns the sort code in formatted display format (XX-XX-XX for UK/Ireland).
    /// </summary>
    public string ToFormattedString()
    {
        var digits = ToDigitsOnly();
        
        return CountryCode switch
        {
            CountryCode.UnitedKingdom when digits.Length == 6 => $"{digits.Substring(0, 2)}-{digits.Substring(2, 2)}-{digits.Substring(4, 2)}",
            CountryCode.Ireland when digits.Length == 6 => $"{digits.Substring(0, 2)}-{digits.Substring(2, 2)}-{digits.Substring(4, 2)}",
            _ => digits
        };
    }

    /// <summary>
    /// Returns the sort code value as stored.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Gets a display-friendly name for the country.
    /// </summary>
    public string GetCountryName() => CountryCode switch
    {
        CountryCode.All => "All Countries",
        CountryCode.UnitedKingdom => "United Kingdom",
        CountryCode.Ireland => "Ireland",
        _ => CountryCode.ToString()
    };

    /// <summary>
    /// Determines whether the specified SortCode is equal to the current SortCode.
    /// </summary>
    public bool Equals(SortCode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare digits only to handle different formatting
        return ToDigitsOnly() == other.ToDigitsOnly() && CountryCode == other.CountryCode;
    }

    /// <summary>
    /// Returns the hash code for this SortCode.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(ToDigitsOnly(), CountryCode);
    }
}
