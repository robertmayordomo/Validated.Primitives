using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators.Geospatial;

/// <summary>
/// Validators for longitude coordinate values.
/// </summary>
public static class LongitudeValidators
{
    /// <summary>
    /// Validates that a longitude is within the valid range (-180 to +180 degrees).
    /// </summary>
    public static ValueValidator<decimal> Range(string fieldName = "Longitude", decimal min = -180m, decimal max = 180m)
        => value =>
        {
            if (value < min)
                return ValidationResult.Failure($"{fieldName} must be at least {min}°.", fieldName, "MinValue");

            if (value > max)
                return ValidationResult.Failure($"{fieldName} must be at most {max}°.", fieldName, "MaxValue");

            return ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a longitude has the correct number of decimal places.
    /// </summary>
    public static ValueValidator<decimal> DecimalPlaces(string fieldName = "Longitude", int maxDecimalPlaces = 6)
        => value =>
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

            return decimalPlaces > maxDecimalPlaces
                ? ValidationResult.Failure($"{fieldName} cannot have more than {maxDecimalPlaces} decimal places.", fieldName, "DecimalPlaces")
                : ValidationResult.Success();
        };
}
