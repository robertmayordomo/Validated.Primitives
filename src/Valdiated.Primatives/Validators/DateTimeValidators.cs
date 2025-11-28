using Validated.Primitives.DateRanges;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

public static class DateTimeValidators
{
    public static ValueValidator<DateTime> FromTodayForward(string fieldName = "Date")
        => value =>
        {
            var today = DateTime.UtcNow.Date;

            if (value.Date < today)
            {
                return ValidationResult.Failure(
                    $"{fieldName} must be today or in the future.",
                    fieldName,
                    "FromTodayForward");
            }

            return ValidationResult.Success();
        };

    public static ValueValidator<DateTime> BeforeToday(string fieldName = "Date")
        => value =>
        {
            var today = DateTime.UtcNow.Date;

            if (value.Date >= today)
            {
                return ValidationResult.Failure(
                    $"{fieldName} must be before today.",
                    fieldName,
                    "BeforeToday");
            }

            return ValidationResult.Success();
        };

    public static ValueValidator<DateTime> Between(
        string fieldName,
        DateTime from,
        DateTime to,
        bool inclusive = true)
        => Between(fieldName,
            new DateRange(from, to, inclusiveStart: inclusive, inclusiveEnd: inclusive));

    public static ValueValidator<DateTime> Between(
        string fieldName,
        DateRange range)
        => value =>
        {
            if (range.Contains(value))
                return ValidationResult.Success();

            var message = range.InclusiveStart && range.InclusiveEnd
                ? $"{fieldName} must be between {range.From:yyyy-MM-dd} and {range.To:yyyy-MM-dd}."
                : $"{fieldName} must be within the range {range}.";

            return ValidationResult.Failure(
                message,
                fieldName,
                "Between");
        };
}
