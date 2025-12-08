using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for credit card security numbers (CVV/CVC).
/// </summary>
public static class CreditCardSecurityNumberValidators
{
    /// <summary>
    /// Validates that the security number is not empty or whitespace.
    /// </summary>
    public static ValueValidator<string> NotEmpty(string propertyName = "SecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure(
                    "Security number must be provided",
                    propertyName);

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the security number contains 3 or 4 digits after removing non-digit characters.
    /// </summary>
    public static ValueValidator<string> ValidLength(string propertyName = "SecurityNumber")
    {
        return value =>
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length < 3 || digits.Length > 4)
                return ValidationResult.Failure(
                    "Security number must contain 3 or 4 digits",
                    propertyName);

            return ValidationResult.Success();
        };
    }
}
