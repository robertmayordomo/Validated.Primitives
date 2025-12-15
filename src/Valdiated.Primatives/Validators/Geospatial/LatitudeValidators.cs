using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators.Geospatial;

/// <summary>
/// Validators for latitude coordinate values.
/// </summary>
public static class LatitudeValidators
{
    /// <summary>
    /// Validates that a latitude is within the valid range (-90 to +90 degrees).
    /// </summary>
    public static ValueValidator<decimal> Range(string fieldName = "Latitude", decimal min = -90m, decimal max = 90m)
        => value =>
        {
            if (value < min)
                return ValidationResult.Failure($"{fieldName} must be at least {min}°.", fieldName, "MinValue");

            if (value > max)
                return ValidationResult.Failure($"{fieldName} must be at most {max}°.", fieldName, "MaxValue");

            return ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a latitude has the correct number of decimal places.
    /// </summary>
    public static ValueValidator<decimal> DecimalPlaces(string fieldName = "Latitude", int maxDecimalPlaces = 6)
        => value =>
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

            return decimalPlaces > maxDecimalPlaces
                ? ValidationResult.Failure($"{fieldName} cannot have more than {maxDecimalPlaces} decimal places.", fieldName, "DecimalPlaces")
                : ValidationResult.Success();
        };
}
