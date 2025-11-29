using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

[JsonConverter(typeof(DateOfBirthConverter))]
public sealed record DateOfBirth : ValidatedValueObject<DateTime>
{
    private DateOfBirth(DateTime value, string propertyName = "DateOfBirth") : base(value)
    {
        Validators.Add(DateTimeValidators.BeforeToday(propertyName));
    }

    public static (ValidationResult Result, DateOfBirth? Value) TryCreate(DateTime value, string propertyName = "DateOfBirth")
    {
        var dateOfBirth = new DateOfBirth(value, propertyName);
        var validationResult = dateOfBirth.Validate();
        var result = validationResult.IsValid ? dateOfBirth : null;
        return (validationResult, result);
    }

    public static (ValidationResult Result, DateOfBirth? Value) TryCreate(string value, string propertyName = "DateOfBirth")
    {
        if (!DateTime.TryParse(value, out var parsed))
        {
            var failure = ValidationResult.Failure(
                "Invalid date format.",
                propertyName,
                "InvalidDateString");

            return (failure, null);
        }

        return TryCreate(parsed, propertyName);
    }
}
