using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated US Social Security Number.
/// Stores the SSN and validates format, area number, group number, and serial number.
/// Supports formatting with or without dashes and provides masking for security.
/// </summary>
[JsonConverter(typeof(SocialSecurityNumberConverter))]
public sealed record SocialSecurityNumber : ValidatedPrimitive<string>
{
    private SocialSecurityNumber(string value, string propertyName = "SocialSecurityNumber") 
        : base(ExtractDigits(value))
    {
        Validators.Add(SocialSecurityNumberValidators.NotEmpty(propertyName));
        Validators.Add(SocialSecurityNumberValidators.ValidFormat(propertyName));
        Validators.Add(SocialSecurityNumberValidators.ValidAreaNumber(propertyName));
        Validators.Add(SocialSecurityNumberValidators.ValidGroupNumber(propertyName));
        Validators.Add(SocialSecurityNumberValidators.ValidSerialNumber(propertyName));
        Validators.Add(SocialSecurityNumberValidators.NotAdvertisingNumber(propertyName));
    }

    /// <summary>
    /// Attempts to create a SocialSecurityNumber instance with validation.
    /// </summary>
    /// <param name="value">The SSN value (with or without dashes).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the SocialSecurityNumber instance if valid.</returns>
    public static (ValidationResult Result, SocialSecurityNumber? Value) TryCreate(
        string? value,
        string propertyName = "SocialSecurityNumber")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var emptyResult = ValidationResult.Failure(
                "Social Security Number must be provided",
                propertyName,
                "Required");
            return (emptyResult, null);
        }

        // Validate format of the original input before extracting digits
        var formatValidationResult = SocialSecurityNumberValidators.ValidateInputFormat(value, propertyName);
        if (!formatValidationResult.IsValid)
        {
            return (formatValidationResult, null);
        }

        var ssn = new SocialSecurityNumber(value, propertyName);
        var validationResult = ssn.Validate();
        var result = validationResult.IsValid ? ssn : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the SSN in XXX-XX-XXXX format.
    /// </summary>
    public override string ToString()
    {
        if (Value.Length != 9)
            return Value;

        return $"{Value.Substring(0, 3)}-{Value.Substring(3, 2)}-{Value.Substring(5, 4)}";
    }

    /// <summary>
    /// Returns the SSN without dashes (9 digits only).
    /// </summary>
    public string ToDigitsOnly() => Value;

    /// <summary>
    /// Returns a masked version of the SSN showing only the last 4 digits (XXX-XX-1234).
    /// </summary>
    public string Masked()
    {
        if (Value.Length != 9)
            return new string('*', Value.Length);

        var lastFour = Value.Substring(5, 4);
        return $"XXX-XX-{lastFour}";
    }

    /// <summary>
    /// Returns a partially masked version showing first 3 and last 4 digits (123-XX-4567).
    /// </summary>
    public string PartiallyMasked()
    {
        if (Value.Length != 9)
            return new string('*', Value.Length);

        var firstThree = Value.Substring(0, 3);
        var lastFour = Value.Substring(5, 4);
        return $"{firstThree}-XX-{lastFour}";
    }

    /// <summary>
    /// Gets the area number (first 3 digits).
    /// </summary>
    public string AreaNumber => Value.Length >= 3 ? Value.Substring(0, 3) : string.Empty;

    /// <summary>
    /// Gets the group number (middle 2 digits).
    /// </summary>
    public string GroupNumber => Value.Length >= 5 ? Value.Substring(3, 2) : string.Empty;

    /// <summary>
    /// Gets the serial number (last 4 digits).
    /// </summary>
    public string SerialNumber => Value.Length >= 9 ? Value.Substring(5, 4) : string.Empty;

    private static string ExtractDigits(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return new string(input.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Determines whether the specified SocialSecurityNumber is equal to the current SocialSecurityNumber.
    /// </summary>
    public bool Equals(SocialSecurityNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Returns the hash code for this SocialSecurityNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
