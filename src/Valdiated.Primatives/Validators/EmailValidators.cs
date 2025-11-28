using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Validated.Primitives.Validation;
using ValidationResult = Validated.Primitives.Validation.ValidationResult;

namespace Validated.Primitives.Validators;

public static partial class EmailValidators
{
    // Stricter regex that validates:
    // - Local part: starts with alphanumeric, no leading/trailing dots, no consecutive dots
    // - Domain: proper labels (alphanumeric start/end), valid TLD
    // Using source generator for better performance
    [GeneratedRegex(
        @"^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex EmailRegex();

    public static ValueValidator<string> EmailFormat(string fieldName = "Email")
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure("Invalid email address format.", fieldName, "EmailFormat");

            // First try built-in attribute for broad validation
            var emailAttr = new EmailAddressAttribute();
            if (!emailAttr.IsValid(value))
                return ValidationResult.Failure("Invalid email address format.", fieldName, "EmailFormat");

            // Then enforce stricter rules:
            // - No leading/trailing dots in local part
            // - No consecutive dots
            // - Valid domain structure
            if (!EmailRegex().IsMatch(value))
                return ValidationResult.Failure("Invalid email address format.", fieldName, "EmailFormat");

            return ValidationResult.Success();
        };
}