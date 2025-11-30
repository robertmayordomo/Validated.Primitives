using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Validators for monetary values.
/// </summary>
public static class MoneyValidators
{
    /// <summary>
    /// Validates that a monetary value is non-negative.
    /// </summary>
    public static ValueValidator<decimal> NonNegative(string fieldName = "Money")
        => value =>
        {
            return value < 0
                ? ValidationResult.Failure($"{fieldName} cannot be negative.", fieldName, "NonNegative")
                : ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a monetary value has the correct number of decimal places.
    /// </summary>
    public static ValueValidator<decimal> ValidDecimalPlaces(string fieldName = "Money", int maxDecimalPlaces = 2)
        => value =>
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
            
            return decimalPlaces > maxDecimalPlaces
                ? ValidationResult.Failure($"{fieldName} cannot have more than {maxDecimalPlaces} decimal places.", fieldName, "DecimalPlaces")
                : ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a monetary value is within a specified range.
    /// </summary>
    public static ValueValidator<decimal> Range(string fieldName = "Money", decimal min = 0, decimal max = decimal.MaxValue)
        => value =>
        {
            if (value < min)
                return ValidationResult.Failure($"{fieldName} must be at least {min}.", fieldName, "MinValue");

            if (value > max)
                return ValidationResult.Failure($"{fieldName} must be at most {max}.", fieldName, "MaxValue");

            return ValidationResult.Success();
        };
}
