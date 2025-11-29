using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated human name (first, middle, or last name).
/// Must be non-empty, contain only alphabetic characters, hyphens, and apostrophes,
/// and be no more than 50 characters.
/// </summary>
public sealed record HumanName : ValidatedValueObject<string>
{
    private HumanName(string value, string propertyName = "Name") : base(value?.Trim() ?? string.Empty)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(CommonValidators.MaxLength(propertyName, 50));
        Validators.Add(HumanNameValidators.AlphaWithHyphenAndApostrophe(propertyName));
    }

    /// <summary>
    /// Attempts to create a HumanName instance with validation.
    /// </summary>
    /// <param name="value">The name to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the HumanName instance if valid.</returns>
    public static (ValidationResult Result, HumanName? Value) TryCreate(
        string value,
        string propertyName = "Name")
    {
        var humanName = new HumanName(value, propertyName);
        var validationResult = humanName.Validate();
        var result = validationResult.IsValid ? humanName : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the name as a string.
    /// </summary>
    public override string ToString() => Value;
}
