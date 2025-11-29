using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

[JsonConverter(typeof(FutureDateConverter))]
public sealed record FutureDate : ValidatedValueObject<DateTime>
{
    private FutureDate(DateTime value, string propertyName = "FutureDate") : base(value)
    {
        Validators.Add(DateTimeValidators.FromTodayForward(propertyName));
    }

    public static (ValidationResult Result, FutureDate? Value) TryCreate(DateTime value, string propertyName = "FutureDate")
    {
        var futureDate = new FutureDate(value, propertyName);
        var validationResult = futureDate.Validate();
        var result = validationResult.IsValid ? futureDate : null;
        return (validationResult, result);
    }

    public static (ValidationResult Result, FutureDate? Value) TryCreate(string value, string propertyName = "FutureDate")
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
