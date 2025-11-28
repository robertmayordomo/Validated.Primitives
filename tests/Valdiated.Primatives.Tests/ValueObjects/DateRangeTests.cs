using Xunit;
using Shouldly;
using Validated.Primitives.DateRanges;

namespace Validated.Primitives.Tests.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Ctor_Creates_Valid_Range_With_Same_Dates()
    {
        var date = DateTime.Parse("2020-01-01");
        var range = new DateRange(date, date);

        range.From.ShouldBe(date.Date, "From should be set to the provided date (date part only)");
        range.To.ShouldBe(date.Date, "To should be set to the provided date (date part only)");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void Ctor_Creates_Valid_Range_With_Different_Dates()
    {
        var from = DateTime.Parse("2020-01-01");
        var to = DateTime.Parse("2020-01-31");
        var range = new DateRange(from, to);

        range.From.ShouldBe(from.Date, "From should be set to the start date (date part only)");
        range.To.ShouldBe(to.Date, "To should be set to the end date (date part only)");
    }

    [Fact]
    public void Ctor_Normalizes_DateTime_To_Date()
    {
        var from = DateTime.Parse("2020-01-01 14:30:45");
        var to = DateTime.Parse("2020-01-31 23:59:59");
        var range = new DateRange(from, to);

        range.From.ShouldBe(new DateTime(2020, 1, 1), "From should normalize to date-only value");
        range.To.ShouldBe(new DateTime(2020, 1, 31), "To should normalize to date-only value");
        range.From.TimeOfDay.ShouldBe(TimeSpan.Zero, "From should have no time component");
        range.To.TimeOfDay.ShouldBe(TimeSpan.Zero, "To should have no time component");
    }

    [Fact]
    public void Ctor_Throws_When_From_After_To()
    {
        var ex = Should.Throw<ArgumentException>(() => 
            new DateRange(DateTime.Parse("2020-01-02"), DateTime.Parse("2020-01-01")),
            "Constructor should throw when From is after To");
        
        ex.ParamName.ShouldBe("from", "Exception should indicate 'from' parameter");
        ex.Message.ShouldContain("'From' must be less than or equal to 'To'");
    }

    [Fact]
    public void Ctor_Respects_InclusiveStart_Parameter()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"), 
            inclusiveStart: false);

        range.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void Ctor_Respects_InclusiveEnd_Parameter()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"), 
            inclusiveEnd: false);

        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void Ctor_Respects_Both_Inclusive_Parameters()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"), 
            inclusiveStart: false,
            inclusiveEnd: false);

        range.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Valid_Range()
    {
        var (result, value) = DateRange.TryCreate(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));

        result.IsValid.ShouldBeTrue("Result should be valid for a valid date range");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(new DateTime(2020, 1, 1), "From should be set to the start date");
        value.To.ShouldBe(new DateTime(2020, 1, 31), "To should be set to the end date");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Same_Dates()
    {
        var date = DateTime.Parse("2020-01-01");
        var (result, value) = DateRange.TryCreate(date, date);

        result.IsValid.ShouldBeTrue("Result should be valid when From equals To");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(date.Date, "From should be set to the provided date");
        value.To.ShouldBe(date.Date, "To should be set to the provided date");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_From_After_To()
    {
        var (result, value) = DateRange.TryCreate(
            DateTime.Parse("2020-01-02"), 
            DateTime.Parse("2020-01-01"));

        result.IsValid.ShouldBeFalse("Result should be invalid when From is after To");
        value.ShouldBeNull("Value should be null when validation fails");
    }

    [Fact]
    public void TryCreate_Returns_Correct_Error_Details_When_Invalid()
    {
        var (result, value) = DateRange.TryCreate(
            DateTime.Parse("2020-01-02"), 
            DateTime.Parse("2020-01-01"));

        result.IsValid.ShouldBeFalse("Result should be invalid when From is after To");
        result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
        result.Errors[0].Message.ShouldBe("'From' must be less than or equal to 'To'.", "Error message should explain the validation failure");
        result.Errors[0].MemberName.ShouldBe("DateRange", "Error should be associated with DateRange");
        result.Errors[0].Code.ShouldBe("InvalidRange", "Error code should be InvalidRange");
    }

    [Fact]
    public void TryCreate_Normalizes_DateTime_To_Date()
    {
        var (result, value) = DateRange.TryCreate(
            DateTime.Parse("2020-01-01 14:30:45"),
            DateTime.Parse("2020-01-31 23:59:59"));

        result.IsValid.ShouldBeTrue("Result should be valid for a valid date range");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(new DateTime(2020, 1, 1), "From should normalize to date-only value");
        value.To.ShouldBe(new DateTime(2020, 1, 31), "To should normalize to date-only value");
    }

    [Fact]
    public void TryCreate_Respects_Inclusive_Parameters()
    {
        var (result, value) = DateRange.TryCreate(
            DateTime.Parse("2020-01-01"),
            DateTime.Parse("2020-01-31"),
            inclusiveStart: false,
            inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid for a valid date range");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        value.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void Contains_Returns_True_For_Date_In_Inclusive_Range()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));

        range.Contains(DateTime.Parse("2020-01-01")).ShouldBeTrue("Inclusive range should contain start boundary");
        range.Contains(DateTime.Parse("2020-01-15")).ShouldBeTrue("Range should contain middle date");
        range.Contains(DateTime.Parse("2020-01-31")).ShouldBeTrue("Inclusive range should contain end boundary");
    }

    [Fact]
    public void Contains_Returns_False_For_Date_Outside_Range()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));

        range.Contains(DateTime.Parse("2019-12-31")).ShouldBeFalse("Range should not contain date before start");
        range.Contains(DateTime.Parse("2020-02-01")).ShouldBeFalse("Range should not contain date after end");
    }

    [Fact]
    public void Contains_Respects_InclusiveStart_True()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveStart: true);

        range.Contains(DateTime.Parse("2020-01-01")).ShouldBeTrue("Inclusive start should include the start boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveStart_False()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveStart: false);

        range.Contains(DateTime.Parse("2020-01-01")).ShouldBeFalse("Exclusive start should exclude the start boundary");
        range.Contains(DateTime.Parse("2020-01-02")).ShouldBeTrue("Exclusive start should include date after start boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveEnd_True()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveEnd: true);

        range.Contains(DateTime.Parse("2020-01-31")).ShouldBeTrue("Inclusive end should include the end boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveEnd_False()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveEnd: false);

        range.Contains(DateTime.Parse("2020-01-31")).ShouldBeFalse("Exclusive end should exclude the end boundary");
        range.Contains(DateTime.Parse("2020-01-30")).ShouldBeTrue("Exclusive end should include date before end boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveFlags()
    {
        var dr = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-03"), 
            inclusiveStart: false, 
            inclusiveEnd: false);

        dr.Contains(DateTime.Parse("2020-01-01")).ShouldBeFalse("Exclusive start should exclude start boundary");
        dr.Contains(DateTime.Parse("2020-01-02")).ShouldBeTrue("Exclusive range should contain middle date");
        dr.Contains(DateTime.Parse("2020-01-03")).ShouldBeFalse("Exclusive end should exclude end boundary");
    }

    [Fact]
    public void Contains_Normalizes_DateTime_To_Date()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));

        range.Contains(DateTime.Parse("2020-01-15 14:30:45")).ShouldBeTrue("Range should normalize DateTime to date when checking containment");
        range.Contains(DateTime.Parse("2020-01-01 23:59:59")).ShouldBeTrue("Range should contain start date regardless of time");
        range.Contains(DateTime.Parse("2020-01-31 00:00:01")).ShouldBeTrue("Range should contain end date regardless of time");
    }

    [Fact]
    public void Contains_Single_Day_Range_Inclusive()
    {
        var date = DateTime.Parse("2020-01-01");
        var range = new DateRange(date, date);

        range.Contains(date).ShouldBeTrue("Inclusive single-day range should contain the exact date");
        range.Contains(date.AddDays(-1)).ShouldBeFalse("Single-day range should not contain date before");
        range.Contains(date.AddDays(1)).ShouldBeFalse("Single-day range should not contain date after");
    }

    [Fact]
    public void Contains_Single_Day_Range_Exclusive_Returns_False()
    {
        var date = DateTime.Parse("2020-01-01");
        var range = new DateRange(
            date, 
            date, 
            inclusiveStart: false, 
            inclusiveEnd: false);

        // Exclusive on both ends means the single date is not included
        range.Contains(date).ShouldBeFalse("Exclusive single-day range should not contain any date, including the boundary");
    }

    [Fact]
    public void ToString_Formats_Inclusive_Range_Correctly()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-31]", "Inclusive range should use square brackets");
    }

    [Fact]
    public void ToString_Formats_Exclusive_Start_Correctly()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveStart: false);

        range.ToString().ShouldBe("(2020-01-01 .. 2020-01-31]", "Exclusive start should use opening parenthesis");
    }

    [Fact]
    public void ToString_Formats_Exclusive_End_Correctly()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveEnd: false);

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-31)", "Exclusive end should use closing parenthesis");
    }

    [Fact]
    public void ToString_Formats_Exclusive_Range_Correctly()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveStart: false,
            inclusiveEnd: false);

        range.ToString().ShouldBe("(2020-01-01 .. 2020-01-31)", "Exclusive range should use parentheses on both ends");
    }

    [Fact]
    public void ToString_Formats_Correctly()
    {
        var dr = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-03"), 
            inclusiveStart: true, 
            inclusiveEnd: false);

        dr.ToString().ShouldBe("[2020-01-01 .. 2020-01-03)", "Mixed inclusive/exclusive range should use appropriate brackets");
    }

    [Fact]
    public void ToString_Formats_Single_Day_Range()
    {
        var date = DateTime.Parse("2020-01-01");
        var range = new DateRange(date, date);

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-01]", "Single-day range should format with same date on both ends");
    }

    [Fact]
    public void Equality_Same_Values_Are_Equal()
    {
        var range1 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));
        var range2 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));

        range1.ShouldBe(range2, "Ranges with same values should be equal");
        (range1 == range2).ShouldBeTrue("Equality operator should return true for equal ranges");
    }

    [Fact]
    public void Equality_Different_From_Are_Not_Equal()
    {
        var range1 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));
        var range2 = new DateRange(
            DateTime.Parse("2020-01-02"), 
            DateTime.Parse("2020-01-31"));

        range1.ShouldNotBe(range2, "Ranges with different From dates should not be equal");
    }

    [Fact]
    public void Equality_Different_To_Are_Not_Equal()
    {
        var range1 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"));
        var range2 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-30"));

        range1.ShouldNotBe(range2, "Ranges with different To dates should not be equal");
    }

    [Fact]
    public void Equality_Different_InclusiveStart_Are_Not_Equal()
    {
        var range1 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveStart: true);
        var range2 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveStart: false);

        range1.ShouldNotBe(range2, "Ranges with different InclusiveStart should not be equal");
    }

    [Fact]
    public void Equality_Different_InclusiveEnd_Are_Not_Equal()
    {
        var range1 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveEnd: true);
        var range2 = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.Parse("2020-01-31"),
            inclusiveEnd: false);

        range1.ShouldNotBe(range2, "Ranges with different InclusiveEnd should not be equal");
    }

    [Fact]
    public void Works_With_Min_DateTime()
    {
        var range = new DateRange(
            DateTime.MinValue, 
            DateTime.Parse("2020-01-01"));

        range.From.ShouldBe(DateTime.MinValue.Date, "From should be set to DateTime.MinValue (date part)");
        range.Contains(DateTime.MinValue).ShouldBeTrue("Range should contain DateTime.MinValue as start boundary");
    }

    [Fact]
    public void Works_With_Max_DateTime()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"), 
            DateTime.MaxValue);

        range.To.ShouldBe(DateTime.MaxValue.Date, "To should be set to DateTime.MaxValue (date part)");
        range.Contains(DateTime.MaxValue).ShouldBeTrue("Range should contain DateTime.MaxValue as end boundary");
    }

    [Fact]
    public void Works_With_Leap_Year_Dates()
    {
        var range = new DateRange(
            DateTime.Parse("2020-02-28"), 
            DateTime.Parse("2020-03-01"));

        range.Contains(DateTime.Parse("2020-02-29")).ShouldBeTrue("Range should contain leap day (Feb 29)");
    }

    [Fact]
    public void Works_Across_Year_Boundary()
    {
        var range = new DateRange(
            DateTime.Parse("2019-12-31"), 
            DateTime.Parse("2020-01-02"));

        range.Contains(DateTime.Parse("2019-12-31")).ShouldBeTrue("Range should contain last day of year");
        range.Contains(DateTime.Parse("2020-01-01")).ShouldBeTrue("Range should contain first day of new year");
        range.Contains(DateTime.Parse("2020-01-02")).ShouldBeTrue("Range should contain second day of new year");
    }

    [Fact]
    public void UntilNow_Creates_Range_From_Past_Date_To_Now()
    {
        var from = DateTime.Parse("2020-01-01");
        var (result, range) = from.UntilNow();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to now");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.From.ShouldBe(from.Date, "From should be the specified date");
        range.To.ShouldBe(DateTime.Now.Date, "To should be today's date");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void UntilNow_Respects_Inclusive_Parameters()
    {
        var from = DateTime.Parse("2020-01-01");
        var (result, range) = from.UntilNow(inclusiveStart: false, inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to now");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void UntilNow_Contains_Today()
    {
        var from = DateTime.Parse("2020-01-01");
        var (result, range) = from.UntilNow();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to now");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.Contains(DateTime.Now).ShouldBeTrue("Range should contain today's date");
    }

    [Fact]
    public void FromNowUntil_Creates_Range_From_Now_To_Future_Date()
    {
        var to = DateTime.Parse("2030-12-31");
        var (result, range) = to.FromNowUntil();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from now to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.From.ShouldBe(DateTime.Now.Date, "From should be today's date");
        range.To.ShouldBe(to.Date, "To should be the specified future date");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void FromNowUntil_Respects_Inclusive_Parameters()
    {
        var to = DateTime.Parse("2030-12-31");
        var (result, range) = to.FromNowUntil(inclusiveStart: false, inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from now to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void FromNowUntil_Contains_Today()
    {
        var to = DateTime.Parse("2030-12-31");
        var (result, range) = to.FromNowUntil();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from now to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.Contains(DateTime.Now).ShouldBeTrue("Range should contain today's date");
    }
}
