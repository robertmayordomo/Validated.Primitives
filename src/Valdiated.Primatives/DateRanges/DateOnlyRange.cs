using Validated.Primitives.Validation;

namespace Validated.Primitives.DateRanges;

public sealed record DateOnlyRange
{
    public DateOnly From { get; }
    public DateOnly To { get; }
    public bool InclusiveStart { get; }
    public bool InclusiveEnd { get; }

    public DateOnlyRange(
        DateOnly from,
        DateOnly to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        if (from > to)
            throw new ArgumentException("'From' must be less than or equal to 'To'.", nameof(from));

        From = from;
        To = to;
        InclusiveStart = inclusiveStart;
        InclusiveEnd = inclusiveEnd;
    }

    public static (ValidationResult Result, DateOnlyRange? Value) TryCreate(
        DateOnly from,
        DateOnly to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        if (from > to)
        {
            var failure = ValidationResult.Failure(
                "'From' must be less than or equal to 'To'.",
                "DateOnlyRange",
                "InvalidRange");

            return (failure, null);
        }

        return (ValidationResult.Success(), new DateOnlyRange(from, to, inclusiveStart, inclusiveEnd));
    }

    public bool Contains(DateOnly date)
    {
        var lowerOk = InclusiveStart ? date >= From : date > From;
        var upperOk = InclusiveEnd   ? date <= To   : date < To;

        return lowerOk && upperOk;
    }

    public override string ToString()
    {
        var startBracket = InclusiveStart ? "[" : "(";
        var endBracket   = InclusiveEnd   ? "]" : ")";
        return $"{startBracket}{From:yyyy-MM-dd} .. {To:yyyy-MM-dd}{endBracket}";
    }
}
