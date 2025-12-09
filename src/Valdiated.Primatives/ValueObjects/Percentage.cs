using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated percentage value (0-100) with configurable decimal places (max 3).
/// </summary>
[JsonConverter(typeof(PercentageConverter))]
public sealed record Percentage : ValidatedPrimitive<decimal>
{
    /// <summary>
    /// Gets the number of decimal places allowed (0-3).
    /// </summary>
    public int DecimalPlaces { get; }

    private Percentage(decimal value, int decimalPlaces, string propertyName = "Percentage") : base(value)
    {
        if (decimalPlaces < 0 || decimalPlaces > 3)
        {
            throw new ArgumentException("Decimal places must be between 0 and 3.", nameof(decimalPlaces));
        }

        DecimalPlaces = decimalPlaces;

        Validators.Add(PercentageValidators.Range(propertyName, 0, 100));
        Validators.Add(PercentageValidators.DecimalPlaces(propertyName, decimalPlaces));
    }

    /// <summary>
    /// Attempts to create a Percentage instance with validation.
    /// </summary>
    /// <param name="value">The percentage value (0-100).</param>
    /// <param name="decimalPlaces">The maximum number of decimal places (0-3). Default is 0.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Percentage instance if valid.</returns>
    public static (ValidationResult Result, Percentage? Value) TryCreate(
        decimal value,
        int decimalPlaces = 0,
        string propertyName = "Percentage")
    {
        if (decimalPlaces < 0 || decimalPlaces > 3)
        {
            var error = ValidationResult.Failure(
                "Decimal places must be between 0 and 3.",
                propertyName,
                "InvalidDecimalPlaces");
            return (error, null);
        }

        var percentage = new Percentage(value, decimalPlaces, propertyName);
        var validationResult = percentage.Validate();
        var result = validationResult.IsValid ? percentage : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Converts the percentage to a decimal fraction (e.g., 50% becomes 0.5).
    /// </summary>
    public decimal ToFraction() => Value / 100m;

    /// <summary>
    /// Applies this percentage to a given value.
    /// </summary>
    /// <param name="baseValue">The value to calculate the percentage of.</param>
    /// <returns>The calculated percentage amount.</returns>
    public decimal Of(decimal baseValue) => baseValue * ToFraction();

    /// <summary>
    /// Returns a formatted string representation of the percentage.
    /// </summary>
    public override string ToString()
    {
        var format = DecimalPlaces switch
        {
            0 => "F0",
            1 => "F1",
            2 => "F2",
            3 => "F3",
            _ => "F0"
        };

        return $"{Value.ToString(format)}%";
    }

    /// <summary>
    /// Determines whether the specified Percentage is equal to the current Percentage.
    /// </summary>
    public bool Equals(Percentage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && DecimalPlaces == other.DecimalPlaces;
    }

    /// <summary>
    /// Returns the hash code for this Percentage.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, DecimalPlaces);
    }
}
