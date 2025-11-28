using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

public static class UrlValidators
{
    public static ValueValidator<string> WebUrl(string fieldName = "Url")
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure("Invalid web URL format.", fieldName, "WebUrl");
            }

            // Reject URLs with leading or trailing whitespace
            if (value != value.Trim())
            {
                return ValidationResult.Failure("Invalid web URL format.", fieldName, "WebUrl");
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return ValidationResult.Failure("Invalid web URL format.", fieldName, "WebUrl");
            }

            return ValidationResult.Success();
        };
}
