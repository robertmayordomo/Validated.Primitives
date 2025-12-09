using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated bank account number that automatically detects and validates IBAN or BBAN format.
/// 
/// Supports two formats:
/// 1. IBAN (International Bank Account Number) - ISO 13616 standard
///    - Format: CC##BBBBBBBBBBBBBBB (2 letter country + 2 check digits + BBAN)
///    - 70+ countries supported with country-specific length validation
///    - Checksum validation using mod-97 algorithm
///    - Examples: DE89370400440532013000 (Germany), GB82WEST12345698765432 (UK)
/// 
/// 2. BBAN (Basic Bank Account Number) - Domestic account number
///    - Country-specific domestic format without IBAN structure
///    - Examples: 12345678 (UK 8-digit), 123456789 (US domestic)
/// 
/// The type is automatically detected based on the format:
/// - Starts with 2 letters + 2 digits + recognized country code → IBAN
/// - All digits or doesn't match IBAN pattern → BBAN
/// 
/// Reference: ISO 13616:2020 - Financial services — International bank account number (IBAN)
/// </summary>
[JsonConverter(typeof(IbanNumberConverter))]
public sealed record IbanNumber : ValidatedPrimitive<string>
{
    /// <summary>
    /// Gets the detected account number type (IBAN or BBAN).
    /// </summary>
    public BankAccountNumberType AccountType { get; }

    /// <summary>
    /// Gets the country code for this account number.
    /// For IBAN: extracted from first 2 characters.
    /// For BBAN: must be provided during creation.
    /// </summary>
    public CountryCode? CountryCode { get; }

    private IbanNumber(string value, CountryCode? countryCode, string propertyName = "IbanNumber") : base(value)
    {
        Validators.Add(IbanNumberValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(IbanNumberValidators.ValidFormat(propertyName));
        
        // Auto-detect account type
        AccountType = IbanNumberValidators.DetectAccountType(value);
        
        if (AccountType == BankAccountNumberType.Iban)
        {
            Validators.Add(IbanNumberValidators.ValidIbanFormat(propertyName));
            Validators.Add(IbanNumberValidators.ValidIbanChecksum(propertyName));
            
            // Extract country code from IBAN
            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            if (normalized.Length >= 2)
            {
                var ibanCountryCode = normalized.Substring(0, 2);
                CountryCode = ParseIbanCountryCode(ibanCountryCode);
            }
        }
        else if (AccountType == BankAccountNumberType.Bban)
        {
            Validators.Add(IbanNumberValidators.ValidBbanFormat(propertyName, countryCode));
            CountryCode = countryCode;
        }
        else
        {
            CountryCode = countryCode;
        }
    }

    /// <summary>
    /// Attempts to create an IbanNumber instance with automatic type detection.
    /// </summary>
    /// <param name="value">The account number value (IBAN or BBAN format).</param>
    /// <param name="countryCode">Optional country code for BBAN validation. Required for BBAN, optional for IBAN (auto-detected).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the IbanNumber instance if valid.</returns>
    public static (ValidationResult Result, IbanNumber? Value) TryCreate(
        string value,
        CountryCode? countryCode = null,
        string propertyName = "IbanNumber")
    {
        var ibanNumber = new IbanNumber(value, countryCode, propertyName);
        var validationResult = ibanNumber.Validate();
        var result = validationResult.IsValid ? ibanNumber : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the account number without spaces or dashes in uppercase.
    /// </summary>
    public string ToNormalizedString()
    {
        return Value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Returns the account number in formatted display format.
    /// For IBAN: groups in sets of 4 characters (e.g., "DE89 3704 0044 0532 0130 00").
    /// For BBAN: returns normalized format.
    /// </summary>
    public string ToFormattedString()
    {
        var normalized = ToNormalizedString();
        
        if (AccountType == BankAccountNumberType.Iban && normalized.Length >= 4)
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
    /// For IBAN: Returns the IBAN country code (first 2 letters).
    /// For BBAN: Returns null.
    /// </summary>
    public string? GetIbanCountryCode()
    {
        if (AccountType == BankAccountNumberType.Iban)
        {
            var normalized = ToNormalizedString();
            return normalized.Length >= 2 ? normalized.Substring(0, 2) : null;
        }
        return null;
    }

    /// <summary>
    /// For IBAN: Returns the check digits (characters 3-4).
    /// For BBAN: Returns null.
    /// </summary>
    public string? GetIbanCheckDigits()
    {
        if (AccountType == BankAccountNumberType.Iban)
        {
            var normalized = ToNormalizedString();
            return normalized.Length >= 4 ? normalized.Substring(2, 2) : null;
        }
        return null;
    }

    /// <summary>
    /// For IBAN: Returns the BBAN part (characters after first 4).
    /// For BBAN: Returns the entire value.
    /// </summary>
    public string GetBbanPart()
    {
        var normalized = ToNormalizedString();
        
        if (AccountType == BankAccountNumberType.Iban && normalized.Length > 4)
        {
            return normalized.Substring(4);
        }
        
        return normalized;
    }

    /// <summary>
    /// Gets whether this is an IBAN format.
    /// </summary>
    public bool IsIban => AccountType == BankAccountNumberType.Iban;

    /// <summary>
    /// Gets whether this is a BBAN format.
    /// </summary>
    public bool IsBban => AccountType == BankAccountNumberType.Bban;

    /// <summary>
    /// Returns a masked version showing only the last 4 characters.
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
    /// Returns the account number value in normalized format.
    /// </summary>
    public override string ToString() => ToNormalizedString();

    /// <summary>
    /// Determines whether the specified IbanNumber is equal to the current IbanNumber.
    /// Comparison is case-insensitive and ignores formatting (spaces/dashes).
    /// </summary>
    public bool Equals(IbanNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ToNormalizedString() == other.ToNormalizedString() && AccountType == other.AccountType;
    }

    /// <summary>
    /// Returns the hash code for this IbanNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(ToNormalizedString(), AccountType);
    }

    /// <summary>
    /// Parses IBAN country code to CountryCode enum.
    /// </summary>
    private static CountryCode? ParseIbanCountryCode(string ibanCountryCode)
    {
        return ibanCountryCode switch
        {
            "DE" => ValueObjects.CountryCode.Germany,
            "FR" => ValueObjects.CountryCode.France,
            "GB" => ValueObjects.CountryCode.UnitedKingdom,
            "IT" => ValueObjects.CountryCode.Italy,
            "ES" => ValueObjects.CountryCode.Spain,
            "NL" => ValueObjects.CountryCode.Netherlands,
            "BE" => ValueObjects.CountryCode.Belgium,
            "CH" => ValueObjects.CountryCode.Switzerland,
            "AT" => ValueObjects.CountryCode.Austria,
            "SE" => ValueObjects.CountryCode.Sweden,
            "NO" => ValueObjects.CountryCode.Norway,
            "DK" => ValueObjects.CountryCode.Denmark,
            "FI" => ValueObjects.CountryCode.Finland,
            "PL" => ValueObjects.CountryCode.Poland,
            "CZ" => ValueObjects.CountryCode.CzechRepublic,
            "HU" => ValueObjects.CountryCode.Hungary,
            "PT" => ValueObjects.CountryCode.Portugal,
            "IE" => ValueObjects.CountryCode.Ireland,
            _ => null
        };
    }
}
