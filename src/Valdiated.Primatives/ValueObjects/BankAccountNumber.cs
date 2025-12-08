using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated bank account number with country-specific format validation.
/// Supports various international formats including:
/// - IBAN (International Bank Account Number) for European countries
/// - Country-specific formats for UK, US, Australia, Canada, Japan, and others
/// </summary>
[JsonConverter(typeof(BankAccountNumberConverter))]
public sealed record BankAccountNumber : ValidatedValueObject<string>
{
    /// <summary>
    /// Gets the country code for which this account number is validated.
    /// </summary>
    public CountryCode CountryCode { get; }

    /// <summary>
    /// Gets whether this account number is in IBAN format.
    /// </summary>
    public bool IsIban => Value.Length >= 2 && char.IsLetter(Value[0]) && char.IsLetter(Value[1]);

    private BankAccountNumber(string value, CountryCode countryCode, string propertyName = "BankAccountNumber") : base(value)
    {
        Validators.Add(BankAccountNumberValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(BankAccountNumberValidators.ValidFormat(propertyName));
        
        CountryCode = countryCode;
        
        if (countryCode != CountryCode.Unknown && countryCode != CountryCode.All)
        {
            Validators.Add(BankAccountNumberValidators.ValidateCountryFormat(propertyName, countryCode));
        }
    }

    /// <summary>
    /// Attempts to create a BankAccountNumber instance with validation.
    /// </summary>
    /// <param name="countryCode">The country code for format validation.</param>
    /// <param name="value">The bank account number value.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the BankAccountNumber instance if valid.</returns>
    public static (ValidationResult Result, BankAccountNumber? Value) TryCreate(
        CountryCode countryCode,
        string value,
        string propertyName = "BankAccountNumber")
    {
        var accountNumber = new BankAccountNumber(value, countryCode, propertyName);
        var validationResult = accountNumber.Validate();
        var result = validationResult.IsValid ? accountNumber : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the account number without spaces or dashes.
    /// </summary>
    public string ToNormalizedString()
    {
        return Value.Replace(" ", "").Replace("-", "").ToUpperInvariant();
    }

    /// <summary>
    /// Returns the account number in a formatted display format.
    /// For IBANs, groups in sets of 4 characters.
    /// </summary>
    public string ToFormattedString()
    {
        var normalized = ToNormalizedString();
        
        if (IsIban && normalized.Length >= 4)
        {
            var formatted = string.Empty;
            for (int i = 0; i < normalized.Length; i += 4)
            {
                if (i > 0) formatted += " ";
                formatted += normalized.Substring(i, Math.Min(4, normalized.Length - i));
            }
            return formatted;
        }

        return normalized;
    }

    /// <summary>
    /// For IBAN accounts, returns the country code (first 2 letters).
    /// </summary>
    public string? GetIbanCountryCode()
    {
        if (IsIban && Value.Length >= 2)
        {
            return Value.Substring(0, 2).ToUpperInvariant();
        }
        return null;
    }

    /// <summary>
    /// For IBAN accounts, returns the check digits (characters 3-4).
    /// </summary>
    public string? GetIbanCheckDigits()
    {
        if (IsIban && Value.Length >= 4)
        {
            var normalized = ToNormalizedString();
            return normalized.Substring(2, 2);
        }
        return null;
    }

    /// <summary>
    /// Returns a masked version of the account number showing only the last 4 characters.
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
    /// Returns the account number value as stored.
    /// </summary>
    public override string ToString() => Value;

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
    /// Determines whether the specified BankAccountNumber is equal to the current BankAccountNumber.
    /// </summary>
    public bool Equals(BankAccountNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare normalized values to handle different formatting
        return ToNormalizedString() == other.ToNormalizedString() && CountryCode == other.CountryCode;
    }

    /// <summary>
    /// Returns the hash code for this BankAccountNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(ToNormalizedString(), CountryCode);
    }
}
