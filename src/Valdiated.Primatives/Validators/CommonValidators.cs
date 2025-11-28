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

    public static ValueValidator<string> MinLength(string fieldName, int minLength)
        => value => value != null && value.Length < minLength
            ? ValidationResult.Failure($"{fieldName} must be at least {minLength} characters.", fieldName, "MinLength")
            : ValidationResult.Success();

    public static ValueValidator<string> Length(string fieldName, int minLength, int maxLength)
        => value =>
        {
            if (value == null)
                return ValidationResult.Success();

            if (value.Length < minLength)
                return ValidationResult.Failure($"{fieldName} must be at least {minLength} characters.", fieldName, "MinLength");

            if (value.Length > maxLength)
                return ValidationResult.Failure($"{fieldName} must be at most {maxLength} characters.", fieldName, "MaxLength");

            return ValidationResult.Success();
        };
}
