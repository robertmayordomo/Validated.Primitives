using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Validated.Primitives.DateRanges;

namespace Validated.Primitives.Tests.ValueObjects;

public class BetweenDatesSelectionTests
{
    [Fact]
    public void TryCreate_Fails_For_Invalid_DateString()
    {
        var (res, val) = BetweenDatesSelection.TryCreate("not-a-date", DateTime.Parse("2020-01-01"), DateTime.Parse("2020-01-10"));
        res.IsValid.ShouldBeFalse("Result should be invalid for invalid date string");
        val.ShouldBeNull("Value should be null when validation fails");
    }

    [Fact]
    public void TryCreate_Succeeds_For_Valid()
    {
        var (res, val) = BetweenDatesSelection.TryCreate(DateTime.Parse("2020-01-05"), DateTime.Parse("2020-01-01"), DateTime.Parse("2020-01-10"));
        res.IsValid.ShouldBeTrue("Result should be valid for date within range");
        val.ShouldNotBeNull("Value should not be null when validation succeeds");
        val!.Range.From.ShouldBe(DateTime.Parse("2020-01-01"), "Range.From should be set to the start date");
    }

    [Fact]
    public void Validate_Returns_Failure_When_Out_Of_Range()
    {
        var range = new DateRange(DateTime.Parse("2020-01-01"), DateTime.Parse("2020-01-03"));
        var res = BetweenDatesSelection.Validate(DateTime.Parse("2020-01-05"), range);
        res.IsValid.ShouldBeFalse("Result should be invalid when date is outside the range");
    }
}
