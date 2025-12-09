using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated state or province name.
/// Required field with a maximum length of 100 characters.
/// </summary>
[JsonConverter(typeof(StateProvinceConverter))]
public sealed record StateProvince : ValidatedPrimitive<string>
{
    private StateProvince(string value, string propertyName = "StateProvince") : base(value?.Trim() ?? string.Empty)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(CommonValidators.MaxLength(propertyName, 100));
    }

    /// <summary>
    /// Attempts to create a StateProvince instance with validation.
    /// </summary>
    /// <param name="value">The state or province name to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the StateProvince instance if valid.</returns>
    public static (ValidationResult Result, StateProvince? Value) TryCreate(
        string value,
        string propertyName = "StateProvince")
    {
        var stateProvince = new StateProvince(value, propertyName);
        var validationResult = stateProvince.Validate();
        var result = validationResult.IsValid ? stateProvince : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the state or province name as a string.
    /// </summary>
    public override string ToString() => Value;
}
