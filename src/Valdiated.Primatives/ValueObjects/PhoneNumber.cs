using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

public sealed record PhoneNumber : ValidatedValueObject<string>
{
    private PhoneNumber(string value, string propertyName = "PhoneNumber") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(PhoneValidators.PhoneNumber(propertyName));
    }

    public static (ValidationResult Result, PhoneNumber? Value) TryCreate(string value, string propertyName = "PhoneNumber")
    {
        var phoneNumber = new PhoneNumber(value, propertyName);
        var validationResult = phoneNumber.Validate();
        var result = validationResult.IsValid ? phoneNumber : null;
        return (validationResult, result);
    }
}
