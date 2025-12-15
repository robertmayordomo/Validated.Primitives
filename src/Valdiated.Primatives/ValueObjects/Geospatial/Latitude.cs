using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators.Geospatial;

namespace Validated.Primitives.ValueObjects.Geospatial;

/// <summary>
/// Represents a validated latitude coordinate value (-90 to +90 degrees).
/// Supports up to 8 decimal places for precise GPS coordinates.
/// </summary>
[JsonConverter(typeof(LatitudeConverter))]
public sealed record Latitude : ValidatedPrimitive<decimal>
{
    /// <summary>
    /// The minimum valid latitude value in degrees.
    /// </summary>
    public const decimal MinValue = -90m;

    /// <summary>
    /// The maximum valid latitude value in degrees.
    /// </summary>
    public const decimal MaxValue = 90m;

    /// <summary>
    /// Gets the number of decimal places (0-8).
    /// </summary>
    public int DecimalPlaces { get; }

    private Latitude(decimal value, int decimalPlaces, string propertyName = "Latitude") : base(value)
    {
        if (decimalPlaces < 0 || decimalPlaces > 8)
        {
            throw new ArgumentException("Decimal places must be between 0 and 8.", nameof(decimalPlaces));
        }

        DecimalPlaces = decimalPlaces;

        Validators.Add(LatitudeValidators.Range(propertyName, MinValue, MaxValue));
        Validators.Add(LatitudeValidators.DecimalPlaces(propertyName, decimalPlaces));
    }

    /// <summary>
    /// Attempts to create a Latitude instance with validation.
    /// </summary>
    /// <param name="value">The latitude value in degrees (-90 to +90).</param>
    /// <param name="decimalPlaces">The maximum number of decimal places (0-8). Default is 6 for GPS precision.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Latitude instance if valid.</returns>
    public static (ValidationResult Result, Latitude? Value) TryCreate(
        decimal value,
        int decimalPlaces = 6,
        string propertyName = "Latitude")
    {
        if (decimalPlaces < 0 || decimalPlaces > 8)
        {
            var error = ValidationResult.Failure(
                "Decimal places must be between 0 and 8.",
                propertyName,
                "InvalidDecimalPlaces");
            return (error, null);
        }

        var latitude = new Latitude(value, decimalPlaces, propertyName);
        var validationResult = latitude.Validate();
        var result = validationResult.IsValid ? latitude : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets the hemisphere (North or South).
    /// </summary>
    public string GetHemisphere() => Value >= 0 ? "North" : "South";

    /// <summary>
    /// Gets the cardinal direction abbreviation (N or S).
    /// </summary>
    public string GetCardinalDirection() => Value >= 0 ? "N" : "S";

    /// <summary>
    /// Converts to absolute value with cardinal direction (e.g., "40.7128° N").
    /// </summary>
    public string ToCardinalString()
    {
        var absValue = Math.Abs(Value);
        var format = DecimalPlaces > 0 ? $"F{DecimalPlaces}" : "F0";
        return $"{absValue.ToString(format)}° {GetCardinalDirection()}";
    }

    /// <summary>
    /// Returns a formatted string representation of the latitude.
    /// </summary>
    public override string ToString()
    {
        var format = DecimalPlaces > 0 ? $"F{DecimalPlaces}" : "F0";
        return $"{Value.ToString(format)}°";
    }

    /// <summary>
    /// Determines whether the specified Latitude is equal to the current Latitude.
    /// </summary>
    public bool Equals(Latitude? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && DecimalPlaces == other.DecimalPlaces;
    }

    /// <summary>
    /// Returns the hash code for this Latitude.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, DecimalPlaces);
    }
}
