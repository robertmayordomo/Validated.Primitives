using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated US ABA routing number (also known as routing transit number).
/// ABA routing numbers are 9-digit codes used to identify financial institutions in the United States.
/// The routing number includes a checksum digit for validation and Federal Reserve routing symbol validation.
/// Format: XXXXYYYYC where:
/// - XXXX: Federal Reserve routing symbol (first 4 digits, with specific valid ranges for first 2)
/// - YYYY: ABA Institution Identifier (middle 4 digits)
/// - C: Check digit (last digit, calculated using ABA algorithm)
/// </summary>
[JsonConverter(typeof(RoutingNumberConverter))]
public sealed record RoutingNumber : ValidatedPrimitive<string>
{
    private RoutingNumber(string value, string propertyName = "RoutingNumber") : base(value)
    {
        Validators.Add(RoutingNumberValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(RoutingNumberValidators.ValidFormat(propertyName));
        Validators.Add(RoutingNumberValidators.OnlyDigits(propertyName));
        Validators.Add(RoutingNumberValidators.ValidLength(propertyName));
        Validators.Add(RoutingNumberValidators.ValidFederalReserveSymbol(propertyName));
        Validators.Add(RoutingNumberValidators.ValidChecksum(propertyName));
    }

    /// <summary>
    /// Attempts to create a RoutingNumber instance with validation.
    /// </summary>
    /// <param name="value">The routing number value (9 digits, with optional separators).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the RoutingNumber instance if valid.</returns>
    public static (ValidationResult Result, RoutingNumber? Value) TryCreate(
        string value,
        string propertyName = "RoutingNumber")
    {
        var routingNumber = new RoutingNumber(value, propertyName);
        var validationResult = routingNumber.Validate();
        var result = validationResult.IsValid ? routingNumber : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the routing number without spaces or dashes (9 digits only).
    /// </summary>
    public string ToDigitsOnly()
    {
        return Value.Replace("-", "").Replace(" ", "");
    }

    /// <summary>
    /// Returns the routing number in formatted display format (XXXX-YYYY-C).
    /// </summary>
    public string ToFormattedString()
    {
        var digits = ToDigitsOnly();
        
        if (digits.Length == 9)
        {
            return $"{digits.Substring(0, 4)}-{digits.Substring(4, 4)}-{digits[8]}";
        }

        return digits;
    }

    /// <summary>
    /// Gets the Federal Reserve routing symbol (first 4 digits).
    /// </summary>
    public string FederalReserveSymbol
    {
        get
        {
            var digits = ToDigitsOnly();
            return digits.Length >= 4 ? digits.Substring(0, 4) : string.Empty;
        }
    }

    /// <summary>
    /// Gets the ABA Institution Identifier (middle 4 digits).
    /// </summary>
    public string InstitutionIdentifier
    {
        get
        {
            var digits = ToDigitsOnly();
            return digits.Length >= 8 ? digits.Substring(4, 4) : string.Empty;
        }
    }

    /// <summary>
    /// Gets the check digit (last digit).
    /// </summary>
    public string CheckDigit
    {
        get
        {
            var digits = ToDigitsOnly();
            return digits.Length == 9 ? digits[8].ToString() : string.Empty;
        }
    }

    /// <summary>
    /// Gets the Federal Reserve District (first two digits).
    /// This indicates which Federal Reserve Bank serves the financial institution.
    /// </summary>
    public int? FederalReserveDistrict
    {
        get
        {
            var digits = ToDigitsOnly();
            if (digits.Length >= 2 && int.TryParse(digits.Substring(0, 2), out var district))
            {
                return district;
            }
            return null;
        }
    }

    /// <summary>
    /// Returns the routing number value as stored.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether the specified RoutingNumber is equal to the current RoutingNumber.
    /// </summary>
    public bool Equals(RoutingNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare digits only to handle different formatting
        return ToDigitsOnly() == other.ToDigitsOnly();
    }

    /// <summary>
    /// Returns the hash code for this RoutingNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return ToDigitsOnly().GetHashCode();
    }
}
