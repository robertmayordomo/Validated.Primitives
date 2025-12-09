using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

[JsonConverter(typeof(IpAddressConverter))]
public sealed record IpAddress : ValidatedPrimitive<string>
{
    private IpAddress(string value, string propertyName = "IpAddress") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(IpValidators.IpAddress(propertyName));
    }

    public static (ValidationResult Result, IpAddress? Value) TryCreate(string value, string propertyName = "IpAddress")
    {
        var ipAddress = new IpAddress(value, propertyName);
        var validationResult = ipAddress.Validate();
        var result = validationResult.IsValid ? ipAddress : null;
        return (validationResult, result);
    }
}
