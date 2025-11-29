using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

[JsonConverter(typeof(EmailAddressConverter))]
public sealed record EmailAddress : ValidatedValueObject<string>
{
    private EmailAddress(string value, string propertyName = "Email") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(EmailValidators.EmailFormat(propertyName));
        Validators.Add(CommonValidators.MaxLength(propertyName, 256));
    }

    public static (ValidationResult Result, EmailAddress? Value) TryCreate(string value, string propertyName = "Email")
    {
        var emailAddress = new EmailAddress(value, propertyName);
        var validationResult = emailAddress.Validate();
        var result = validationResult.IsValid ? emailAddress : null;
        return (validationResult, result);
    }
}
