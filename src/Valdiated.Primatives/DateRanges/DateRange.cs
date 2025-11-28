using Validated.Primitives.Validation;

namespace Validated.Primitives.DateRanges;

public sealed record DateRange
{
    public DateTime From { get; }
    public DateTime To { get; }
    public bool InclusiveStart { get; }
    public bool InclusiveEnd { get; }

    public DateRange(
        DateTime from,
        DateTime to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        var fromDate = from.Date;
        var toDate = to.Date;

        if (fromDate > toDate)
            throw new ArgumentException("'From' must be less than or equal to 'To'.", nameof(from));

        From = fromDate;
        To = toDate;
        InclusiveStart = inclusiveStart;
        InclusiveEnd = inclusiveEnd;
    }

    public static (ValidationResult Result, DateRange? Value) TryCreate(
        DateTime from,
        DateTime to,
        bool inclusiveStart = true,
        bool inclusiveEnd = true)
    {
        var fromDate = from.Date;
        var toDate = to.Date;

        if (fromDate > toDate)
        {
            var failure = ValidationResult.Failure(
                "'From' must be less than or equal to 'To'.",
                "DateRange",
                "InvalidRange");

            return (failure, null);
        }

        return (ValidationResult.Success(), new DateRange(fromDate, toDate, inclusiveStart, inclusiveEnd));
    }

    public bool Contains(DateTime date)
    {
        var d = date.Date;

        var lowerOk = InclusiveStart ? d >= From : d > From;
        var upperOk = InclusiveEnd   ? d <= To   : d < To;

        return lowerOk && upperOk;
    }

    public override string ToString()
    {
        var startBracket = InclusiveStart ? "[" : "(";
        var endBracket   = InclusiveEnd   ? "]" : ")";
        return $"{startBracket}{From:yyyy-MM-dd} .. {To:yyyy-MM-dd}{endBracket}";
    }
}
