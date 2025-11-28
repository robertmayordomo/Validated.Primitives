using Validated.Primitives.Core;
using Validated.Primitives.DateRanges;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

public sealed record BetweenDatesSelection : ValidatedValueObject<DateTime>
{
    public DateRange Range { get; }

    private BetweenDatesSelection(DateTime value, DateRange range, string propertyName = "BetweenDatesSelection") : base(value)
    {
        Range = range;
        Validators.Add(DateTimeValidators.Between(propertyName, range));
    }

    public static ValidationResult Validate(DateTime value, DateRange range, string propertyName = "BetweenDatesSelection")
    {
        var temp = new BetweenDatesSelection(value, range, propertyName);
        return temp.Validate();
    }

    public static (ValidationResult Result, BetweenDatesSelection? Value) TryCreate(
        DateTime value,
        DateRange range,
        string propertyName = "BetweenDatesSelection")
    {
        var betweenDatesSelection = new BetweenDatesSelection(value, range, propertyName);
        var validationResult = betweenDatesSelection.Validate();
        var result = validationResult.IsValid ? betweenDatesSelection : null;
        return (validationResult, result);
    }

    public static (ValidationResult Result, BetweenDatesSelection? Value) TryCreate(
        string value,
        DateRange range,
        string propertyName = "BetweenDatesSelection")
    {
        if (!DateTime.TryParse(value, out var parsed))
        {
            var failure = ValidationResult.Failure(
                "Invalid date format.",
                propertyName,
                "InvalidDateString");

            return (failure, null);
        }

        return TryCreate(parsed, range, propertyName);
    }

    public static (ValidationResult Result, BetweenDatesSelection? Value) TryCreate(
        DateTime value,
        DateTime from,
        DateTime to,
        bool inclusive = true,
        string propertyName = "BetweenDatesSelection")
    {
        var (rangeResult, range) = DateRange.TryCreate(from, to, inclusive, inclusive);
        if (!rangeResult.IsValid || range is null)
            return (rangeResult, null);

        return TryCreate(value, range, propertyName);
    }

    public static (ValidationResult Result, BetweenDatesSelection? Value) TryCreate(
        string value,
        DateTime from,
        DateTime to,
        bool inclusive = true,
        string propertyName = "BetweenDatesSelection")
    {
        var (rangeResult, range) = DateRange.TryCreate(from, to, inclusive, inclusive);
        if (!rangeResult.IsValid || range is null)
            return (rangeResult, null);

        return TryCreate(value, range, propertyName);
    }
}
