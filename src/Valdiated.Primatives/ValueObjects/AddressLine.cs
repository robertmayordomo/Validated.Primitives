using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated address line (street address, apartment number, etc.).
/// Maximum length is 200 characters. Can be null or empty.
/// </summary>
[JsonConverter(typeof(AddressLineConverter))]
public sealed record AddressLine : ValidatedPrimitive<string>
{
    private AddressLine(string value, string propertyName = "AddressLine") : base(value?.Trim() ?? string.Empty)
    {
        // Only validate if value is provided (not null or whitespace)
        if (!string.IsNullOrWhiteSpace(value))
        {
            Validators.Add(CommonValidators.MaxLength(propertyName, 200));
        }
    }

    /// <summary>
    /// Attempts to create an AddressLine instance with validation.
    /// </summary>
    /// <param name="value">The address line string to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the AddressLine instance if valid.</returns>
    public static (ValidationResult Result, AddressLine? Value) TryCreate(
        string? value,
        string propertyName = "AddressLine")
    {
        // Allow null or empty values
        if (string.IsNullOrWhiteSpace(value))
        {
            return (ValidationResult.Success(), null);
        }

        var addressLine = new AddressLine(value, propertyName);
        var validationResult = addressLine.Validate();
        var result = validationResult.IsValid ? addressLine : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the address line value as a string.
    /// </summary>
    public override string ToString() => Value;
}
