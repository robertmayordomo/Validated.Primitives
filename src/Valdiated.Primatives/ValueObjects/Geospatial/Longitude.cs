using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators.Geospatial;

namespace Validated.Primitives.ValueObjects.Geospatial;

/// <summary>
/// Represents a validated longitude coordinate value (-180 to +180 degrees).
/// Supports up to 8 decimal places for precise GPS coordinates.
/// </summary>
[JsonConverter(typeof(LongitudeConverter))]
public sealed record Longitude : ValidatedPrimitive<decimal>
{
    /// <summary>
    /// The minimum valid longitude value in degrees.
    /// </summary>
    public const decimal MinValue = -180m;

    /// <summary>
    /// The maximum valid longitude value in degrees.
    /// </summary>
    public const decimal MaxValue = 180m;

    /// <summary>
    /// Gets the number of decimal places (0-8).
    /// </summary>
    public int DecimalPlaces { get; }

    private Longitude(decimal value, int decimalPlaces, string propertyName = "Longitude") : base(value)
    {
        if (decimalPlaces < 0 || decimalPlaces > 8)
        {
            throw new ArgumentException("Decimal places must be between 0 and 8.", nameof(decimalPlaces));
        }

        DecimalPlaces = decimalPlaces;

        Validators.Add(LongitudeValidators.Range(propertyName, MinValue, MaxValue));
        Validators.Add(LongitudeValidators.DecimalPlaces(propertyName, decimalPlaces));
    }

    /// <summary>
    /// Attempts to create a Longitude instance with validation.
    /// </summary>
    /// <param name="value">The longitude value in degrees (-180 to +180).</param>
    /// <param name="decimalPlaces">The maximum number of decimal places (0-8). Default is 6 for GPS precision.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Longitude instance if valid.</returns>
    public static (ValidationResult Result, Longitude? Value) TryCreate(
        decimal value,
        int decimalPlaces = 6,
        string propertyName = "Longitude")
    {
        if (decimalPlaces < 0 || decimalPlaces > 8)
        {
            var error = ValidationResult.Failure(
                "Decimal places must be between 0 and 8.",
                propertyName,
                "InvalidDecimalPlaces");
            return (error, null);
        }

        var longitude = new Longitude(value, decimalPlaces, propertyName);
        var validationResult = longitude.Validate();
        var result = validationResult.IsValid ? longitude : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets the hemisphere (East or West).
    /// </summary>
    public string GetHemisphere() => Value >= 0 ? "East" : "West";

    /// <summary>
    /// Gets the cardinal direction abbreviation (E or W).
    /// </summary>
    public string GetCardinalDirection() => Value >= 0 ? "E" : "W";

    /// <summary>
    /// Converts to absolute value with cardinal direction (e.g., "74.0060° W").
    /// </summary>
    public string ToCardinalString()
    {
        var absValue = Math.Abs(Value);
        var format = DecimalPlaces > 0 ? $"F{DecimalPlaces}" : "F0";
        return $"{absValue.ToString(format)}° {GetCardinalDirection()}";
    }

    /// <summary>
    /// Returns a formatted string representation of the longitude.
    /// </summary>
    public override string ToString()
    {
        var format = DecimalPlaces > 0 ? $"F{DecimalPlaces}" : "F0";
        return $"{Value.ToString(format)}°";
    }

    /// <summary>
    /// Determines whether the specified Longitude is equal to the current Longitude.
    /// </summary>
    public bool Equals(Longitude? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && DecimalPlaces == other.DecimalPlaces;
    }

    /// <summary>
    /// Returns the hash code for this Longitude.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, DecimalPlaces);
    }
}
