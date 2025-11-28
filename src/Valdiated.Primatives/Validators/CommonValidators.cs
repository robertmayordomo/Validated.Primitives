using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

public static class CommonValidators
{
    public static ValueValidator<string> NotNullOrWhitespace(string fieldName)
        => value => string.IsNullOrWhiteSpace(value)
            ? ValidationResult.Failure($"{fieldName} cannot be null or whitespace.", fieldName, "NotNullOrWhitespace")
            : ValidationResult.Success();

    public static ValueValidator<string> MaxLength(string fieldName, int maxLength)
        => value => value != null && value.Length > maxLength
            ? ValidationResult.Failure($"{fieldName} must be at most {maxLength} characters.", fieldName, "MaxLength")
            : ValidationResult.Success();
}
