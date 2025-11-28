using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated city name.
/// Required field with a maximum length of 100 characters.
/// </summary>
public sealed record City : ValidatedValueObject<string>
{
    private City(string value, string propertyName = "City") : base(value?.Trim() ?? string.Empty)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(CommonValidators.MaxLength(propertyName, 100));
    }

    /// <summary>
    /// Attempts to create a City instance with validation.
    /// </summary>
    /// <param name="value">The city name to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the City instance if valid.</returns>
    public static (ValidationResult Result, City? Value) TryCreate(
        string value,
        string propertyName = "City")
    {
        var city = new City(value, propertyName);
        var validationResult = city.Validate();
        var result = validationResult.IsValid ? city : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the city name as a string.
    /// </summary>
    public override string ToString() => Value;
}
