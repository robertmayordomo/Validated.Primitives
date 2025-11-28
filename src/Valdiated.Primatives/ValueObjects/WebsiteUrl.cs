using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

public sealed record WebsiteUrl : ValidatedValueObject<string>
{
    private WebsiteUrl(string value, string propertyName = "Url") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(UrlValidators.WebUrl(propertyName));
    }

    public static (ValidationResult Result, WebsiteUrl? Value) TryCreate(string value, string propertyName = "Url")
    {
        var websiteUrl = new WebsiteUrl(value, propertyName);
        var validationResult = websiteUrl.Validate();
        var result = validationResult.IsValid ? websiteUrl : null;
        return (validationResult, result);
    }
}
