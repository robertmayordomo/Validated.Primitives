using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for credit card numbers.
/// </summary>
public static class CreditCardValidators
{
    /// <summary>
    /// Validates that the credit card number is not empty or whitespace.
    /// </summary>
    public static ValueValidator<string> NotEmpty(string propertyName = "CreditCardNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure(
                    "Credit card number must be provided",
                    propertyName);

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the credit card number contains only digits after removing spaces and hyphens.
    /// </summary>
    public static ValueValidator<string> ValidDigitCount(string propertyName = "CreditCardNumber")
    {
        return value =>
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length < 13 || digits.Length > 19)
                return ValidationResult.Failure(
                    "Credit card number must contain between 13 and 19 digits",
                    propertyName);

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the credit card number does not consist of all identical digits.
    /// </summary>
    public static ValueValidator<string> NotAllIdenticalDigits(string propertyName = "CreditCardNumber")
    {
        return value =>
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length > 0 && digits.All(c => c == digits[0]))
                return ValidationResult.Failure(
                    "Credit card number cannot consist of all identical digits",
                    propertyName);

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates that the credit card number passes the Luhn algorithm check.
    /// </summary>
    public static ValueValidator<string> LuhnCheck(string propertyName = "CreditCardNumber")
    {
        return value =>
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (!IsLuhnValid(digits))
                return ValidationResult.Failure(
                    "Credit card number is not valid (Luhn check failed)",
                    propertyName);

            return ValidationResult.Success();
        };
    }

    private static bool IsLuhnValid(string digits)
    {
        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int n = digits[i] - '0';
            if (alternate)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }
}
