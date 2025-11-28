using Validated.Primitives.Validation;

namespace Validated.Primitives.DateRanges;

public sealed record TimeOnlyRange
{
    public TimeOnly From { get; }
    public TimeOnly To { get; }
    public bool InclusiveStart { get; }
    public bool InclusiveEnd { get; }

    public TimeOnlyRange(
        TimeOnly from,
        TimeOnly to,
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

    public static (ValidationResult Result, TimeOnlyRange? Value) TryCreate(
        TimeOnly from,
        TimeOnly to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        if (from > to)
        {
            var failure = ValidationResult.Failure(
                "'From' must be less than or equal to 'To'.",
                "TimeOnlyRange",
                "InvalidRange");

            return (failure, null);
        }

        return (ValidationResult.Success(), new TimeOnlyRange(from, to, inclusiveStart, inclusiveEnd));
    }

    public bool Contains(TimeOnly time)
    {
        var lowerOk = InclusiveStart ? time >= From : time > From;
        var upperOk = InclusiveEnd   ? time <= To   : time < To;

        return lowerOk && upperOk;
    }

    public override string ToString()
    {
        var startBracket = InclusiveStart ? "[" : "(";
        var endBracket   = InclusiveEnd   ? "]" : ")";
        return $"{startBracket}{From:HH:mm:ss} .. {To:HH:mm:ss}{endBracket}";
    }
}
