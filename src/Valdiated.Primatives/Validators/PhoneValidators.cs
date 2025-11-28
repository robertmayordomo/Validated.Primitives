using System.Text.RegularExpressions;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

public static partial class PhoneValidators
{
    // Validates international phone numbers (E.164 format)
    // - Optional leading +
    // - Must start with 1-9
    // - Total of 7-15 digits
    [GeneratedRegex(
        @"^\+?[1-9]\d{6,14}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex PhoneNumberRegex();

    public static ValueValidator<string> PhoneNumber(string fieldName = "PhoneNumber")
        => value =>
        {
            return !PhoneNumberRegex().IsMatch(value ?? string.Empty)
                ? ValidationResult.Failure("Invalid phone number format.", fieldName, "PhoneNumber")
                : ValidationResult.Success();
        };
}
