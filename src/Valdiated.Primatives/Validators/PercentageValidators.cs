using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Validators for percentage values.
/// </summary>
public static class PercentageValidators
{
    /// <summary>
    /// Validates that a percentage is within a specified range.
    /// </summary>
    public static ValueValidator<decimal> Range(string fieldName = "Percentage", decimal min = 0, decimal max = 100)
        => value =>
        {
            if (value < min)
                return ValidationResult.Failure($"{fieldName} must be at least {min}%.", fieldName, "MinValue");

            if (value > max)
                return ValidationResult.Failure($"{fieldName} must be at most {max}%.", fieldName, "MaxValue");

            return ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a percentage has the correct number of decimal places.
    /// </summary>
    public static ValueValidator<decimal> DecimalPlaces(string fieldName = "Percentage", int maxDecimalPlaces = 2)
        => value =>
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

            return decimalPlaces > maxDecimalPlaces
                ? ValidationResult.Failure($"{fieldName} cannot have more than {maxDecimalPlaces} decimal places.", fieldName, "DecimalPlaces")
                : ValidationResult.Success();
        };
}
