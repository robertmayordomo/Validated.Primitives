using Xunit;
using Shouldly;
using Validated.Primitives.DateRanges;

namespace Validated.Primitives.Tests.ValueObjects;

public class DateOnlyRangeTests
{
    [Fact]
    public void Ctor_Creates_Valid_Range_With_Same_Dates()
    {
        var date = new DateOnly(2020, 1, 1);
        var range = new DateOnlyRange(date, date);

        range.From.ShouldBe(date, "From should be set to the provided date");
        range.To.ShouldBe(date, "To should be set to the provided date");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void Ctor_Creates_Valid_Range_With_Different_Dates()
    {
        var from = new DateOnly(2020, 1, 1);
        var to = new DateOnly(2020, 1, 31);
        var range = new DateOnlyRange(from, to);

        range.From.ShouldBe(from, "From should be set to the start date");
        range.To.ShouldBe(to, "To should be set to the end date");
    }

    [Fact]
    public void Ctor_Throws_When_From_After_To()
    {
        var ex = Should.Throw<ArgumentException>(() => 
            new DateOnlyRange(new DateOnly(2020, 1, 2), new DateOnly(2020, 1, 1)),
            "Constructor should throw when From is after To");
        
        ex.ParamName.ShouldBe("from", "Exception should indicate 'from' parameter");
        ex.Message.ShouldContain("'From' must be less than or equal to 'To'");
    }

    [Fact]
    public void Ctor_Respects_InclusiveStart_Parameter()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31), 
            inclusiveStart: false);

        range.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void Ctor_Respects_InclusiveEnd_Parameter()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31), 
            inclusiveEnd: false);

        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void Ctor_Respects_Both_Inclusive_Parameters()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31), 
            inclusiveStart: false,
            inclusiveEnd: false);

        range.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Valid_Range()
    {
        var (result, value) = DateOnlyRange.TryCreate(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));

        result.IsValid.ShouldBeTrue("Result should be valid for a valid date range");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(new DateOnly(2020, 1, 1), "From should be set to the start date");
        value.To.ShouldBe(new DateOnly(2020, 1, 31), "To should be set to the end date");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Same_Dates()
    {
        var date = new DateOnly(2020, 1, 1);
        var (result, value) = DateOnlyRange.TryCreate(date, date);

        result.IsValid.ShouldBeTrue("Result should be valid when From equals To");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(date, "From should be set to the provided date");
        value.To.ShouldBe(date, "To should be set to the provided date");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_From_After_To()
    {
        var (result, value) = DateOnlyRange.TryCreate(
            new DateOnly(2020, 1, 2), 
            new DateOnly(2020, 1, 1));

        result.IsValid.ShouldBeFalse("Result should be invalid when From is after To");
        value.ShouldBeNull("Value should be null when validation fails");
    }

    [Fact]
    public void TryCreate_Returns_Correct_Error_Details_When_Invalid()
    {
        var (result, value) = DateOnlyRange.TryCreate(
            new DateOnly(2020, 1, 2), 
            new DateOnly(2020, 1, 1));

        result.IsValid.ShouldBeFalse("Result should be invalid when From is after To");
        result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
        result.Errors[0].Message.ShouldBe("'From' must be less than or equal to 'To'.", "Error message should explain the validation failure");
        result.Errors[0].MemberName.ShouldBe("DateOnlyRange", "Error should be associated with DateOnlyRange");
        result.Errors[0].Code.ShouldBe("InvalidRange", "Error code should be InvalidRange");
    }

    [Fact]
    public void TryCreate_Respects_Inclusive_Parameters()
    {
        var (result, value) = DateOnlyRange.TryCreate(
            new DateOnly(2020, 1, 1),
            new DateOnly(2020, 1, 31),
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
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));

        range.Contains(new DateOnly(2020, 1, 1)).ShouldBeTrue("Inclusive range should contain start boundary");
        range.Contains(new DateOnly(2020, 1, 15)).ShouldBeTrue("Range should contain middle date");
        range.Contains(new DateOnly(2020, 1, 31)).ShouldBeTrue("Inclusive range should contain end boundary");
    }

    [Fact]
    public void Contains_Returns_False_For_Date_Outside_Range()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));

        range.Contains(new DateOnly(2019, 12, 31)).ShouldBeFalse("Range should not contain date before start");
        range.Contains(new DateOnly(2020, 2, 1)).ShouldBeFalse("Range should not contain date after end");
    }

    [Fact]
    public void Contains_Respects_InclusiveStart_True()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveStart: true);

        range.Contains(new DateOnly(2020, 1, 1)).ShouldBeTrue("Inclusive start should include the start boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveStart_False()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveStart: false);

        range.Contains(new DateOnly(2020, 1, 1)).ShouldBeFalse("Exclusive start should exclude the start boundary");
        range.Contains(new DateOnly(2020, 1, 2)).ShouldBeTrue("Exclusive start should include date after start boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveEnd_True()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveEnd: true);

        range.Contains(new DateOnly(2020, 1, 31)).ShouldBeTrue("Inclusive end should include the end boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveEnd_False()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveEnd: false);

        range.Contains(new DateOnly(2020, 1, 31)).ShouldBeFalse("Exclusive end should exclude the end boundary");
        range.Contains(new DateOnly(2020, 1, 30)).ShouldBeTrue("Exclusive end should include date before end boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveFlags()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 3), 
            inclusiveStart: false, 
            inclusiveEnd: false);

        range.Contains(new DateOnly(2020, 1, 1)).ShouldBeFalse("Exclusive start should exclude start boundary");
        range.Contains(new DateOnly(2020, 1, 2)).ShouldBeTrue("Exclusive range should contain middle date");
        range.Contains(new DateOnly(2020, 1, 3)).ShouldBeFalse("Exclusive end should exclude end boundary");
    }

    [Fact]
    public void Contains_Single_Day_Range_Inclusive()
    {
        var date = new DateOnly(2020, 1, 1);
        var range = new DateOnlyRange(date, date);

        range.Contains(date).ShouldBeTrue("Inclusive single-day range should contain the exact date");
        range.Contains(date.AddDays(-1)).ShouldBeFalse("Single-day range should not contain date before");
        range.Contains(date.AddDays(1)).ShouldBeFalse("Single-day range should not contain date after");
    }

    [Fact]
    public void Contains_Single_Day_Range_Exclusive_Returns_False()
    {
        var date = new DateOnly(2020, 1, 1);
        var range = new DateOnlyRange(
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
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-31]", "Inclusive range should use square brackets");
    }

    [Fact]
    public void ToString_Formats_Exclusive_Start_Correctly()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveStart: false);

        range.ToString().ShouldBe("(2020-01-01 .. 2020-01-31]", "Exclusive start should use opening parenthesis");
    }

    [Fact]
    public void ToString_Formats_Exclusive_End_Correctly()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveEnd: false);

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-31)", "Exclusive end should use closing parenthesis");
    }

    [Fact]
    public void ToString_Formats_Exclusive_Range_Correctly()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveStart: false,
            inclusiveEnd: false);

        range.ToString().ShouldBe("(2020-01-01 .. 2020-01-31)", "Exclusive range should use parentheses on both ends");
    }

    [Fact]
    public void ToString_Formats_Correctly()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 3), 
            inclusiveStart: true, 
            inclusiveEnd: false);

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-03)", "Mixed inclusive/exclusive range should use appropriate brackets");
    }

    [Fact]
    public void ToString_Formats_Single_Day_Range()
    {
        var date = new DateOnly(2020, 1, 1);
        var range = new DateOnlyRange(date, date);

        range.ToString().ShouldBe("[2020-01-01 .. 2020-01-01]", "Single-day range should format with same date on both ends");
    }

    [Fact]
    public void Equality_Same_Values_Are_Equal()
    {
        var range1 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));
        var range2 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));

        range1.ShouldBe(range2, "Ranges with same values should be equal");
        (range1 == range2).ShouldBeTrue("Equality operator should return true for equal ranges");
    }

    [Fact]
    public void Equality_Different_From_Are_Not_Equal()
    {
        var range1 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));
        var range2 = new DateOnlyRange(
            new DateOnly(2020, 1, 2), 
            new DateOnly(2020, 1, 31));

        range1.ShouldNotBe(range2, "Ranges with different From dates should not be equal");
    }

    [Fact]
    public void Equality_Different_To_Are_Not_Equal()
    {
        var range1 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31));
        var range2 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 30));

        range1.ShouldNotBe(range2, "Ranges with different To dates should not be equal");
    }

    [Fact]
    public void Equality_Different_InclusiveStart_Are_Not_Equal()
    {
        var range1 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveStart: true);
        var range2 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveStart: false);

        range1.ShouldNotBe(range2, "Ranges with different InclusiveStart should not be equal");
    }

    [Fact]
    public void Equality_Different_InclusiveEnd_Are_Not_Equal()
    {
        var range1 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveEnd: true);
        var range2 = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            new DateOnly(2020, 1, 31),
            inclusiveEnd: false);

        range1.ShouldNotBe(range2, "Ranges with different InclusiveEnd should not be equal");
    }

    [Fact]
    public void Works_With_Min_DateOnly()
    {
        var range = new DateOnlyRange(
            DateOnly.MinValue, 
            new DateOnly(2020, 1, 1));

        range.From.ShouldBe(DateOnly.MinValue, "From should be set to DateOnly.MinValue");
        range.Contains(DateOnly.MinValue).ShouldBeTrue("Range should contain DateOnly.MinValue as start boundary");
    }

    [Fact]
    public void Works_With_Max_DateOnly()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 1, 1), 
            DateOnly.MaxValue);

        range.To.ShouldBe(DateOnly.MaxValue, "To should be set to DateOnly.MaxValue");
        range.Contains(DateOnly.MaxValue).ShouldBeTrue("Range should contain DateOnly.MaxValue as end boundary");
    }

    [Fact]
    public void Works_With_Leap_Year_Dates()
    {
        var range = new DateOnlyRange(
            new DateOnly(2020, 2, 28), 
            new DateOnly(2020, 3, 1));

        range.Contains(new DateOnly(2020, 2, 29)).ShouldBeTrue("Range should contain leap day (Feb 29)");
    }

    [Fact]
    public void Works_Across_Year_Boundary()
    {
        var range = new DateOnlyRange(
            new DateOnly(2019, 12, 31), 
            new DateOnly(2020, 1, 2));

        range.Contains(new DateOnly(2019, 12, 31)).ShouldBeTrue("Range should contain last day of year");
        range.Contains(new DateOnly(2020, 1, 1)).ShouldBeTrue("Range should contain first day of new year");
        range.Contains(new DateOnly(2020, 1, 2)).ShouldBeTrue("Range should contain second day of new year");
    }

    [Fact]
    public void Works_Across_Century_Boundary()
    {
        var range = new DateOnlyRange(
            new DateOnly(1999, 12, 31), 
            new DateOnly(2000, 1, 1));

        range.Contains(new DateOnly(1999, 12, 31)).ShouldBeTrue("Range should contain last day of 20th century");
        range.Contains(new DateOnly(2000, 1, 1)).ShouldBeTrue("Range should contain first day of 21st century");
    }

    [Fact]
    public void Works_With_Non_Leap_Year_February()
    {
        var range = new DateOnlyRange(
            new DateOnly(2021, 2, 1), 
            new DateOnly(2021, 2, 28));

        range.Contains(new DateOnly(2021, 2, 15)).ShouldBeTrue("Range should contain mid-February date");
        range.Contains(new DateOnly(2021, 2, 28)).ShouldBeTrue("Range should contain last day of February in non-leap year");
        range.Contains(new DateOnly(2021, 3, 1)).ShouldBeFalse("Range should not contain March dates");
    }

    [Fact]
    public void UntilToday_Creates_Range_From_Past_Date_To_Today()
    {
        var from = new DateOnly(2020, 1, 1);
        var (result, range) = from.UntilToday();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to today");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.From.ShouldBe(from, "From should be the specified date");
        range.To.ShouldBe(DateOnly.FromDateTime(DateTime.Now), "To should be today's date");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void UntilToday_Respects_Inclusive_Parameters()
    {
        var from = new DateOnly(2020, 1, 1);
        var (result, range) = from.UntilToday(inclusiveStart: false, inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to today");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void UntilToday_Contains_Today()
    {
        var from = new DateOnly(2020, 1, 1);
        var (result, range) = from.UntilToday();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to today");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.Contains(DateOnly.FromDateTime(DateTime.Now)).ShouldBeTrue("Range should contain today's date");
    }

    [Fact]
    public void FromTodayUntil_Creates_Range_From_Today_To_Future_Date()
    {
        var to = new DateOnly(2030, 12, 31);
        var (result, range) = to.FromTodayUntil();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from today to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.From.ShouldBe(DateOnly.FromDateTime(DateTime.Now), "From should be today's date");
        range.To.ShouldBe(to, "To should be the specified future date");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void FromTodayUntil_Respects_Inclusive_Parameters()
    {
        var to = new DateOnly(2030, 12, 31);
        var (result, range) = to.FromTodayUntil(inclusiveStart: false, inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from today to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void FromTodayUntil_Contains_Today()
    {
        var to = new DateOnly(2030, 12, 31);
        var (result, range) = to.FromTodayUntil();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from today to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.Contains(DateOnly.FromDateTime(DateTime.Now)).ShouldBeTrue("Range should contain today's date");
    }
}
