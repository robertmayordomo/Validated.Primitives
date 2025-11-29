using Xunit;
using Shouldly;
using Validated.Primitives.DateRanges;

namespace Validated.Primitives.Tests.ValueObjects;

public class TimeOnlyRangeTests
{
    [Fact]
    public void Ctor_Creates_Valid_Range_With_Same_Times()
    {
        var time = new TimeOnly(14, 30, 0);
        var range = new TimeOnlyRange(time, time);

        range.From.ShouldBe(time, "From should be set to the provided time");
        range.To.ShouldBe(time, "To should be set to the provided time");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void Ctor_Creates_Valid_Range_With_Different_Times()
    {
        var from = new TimeOnly(9, 0, 0);
        var to = new TimeOnly(17, 0, 0);
        var range = new TimeOnlyRange(from, to);

        range.From.ShouldBe(from, "From should be set to the start time");
        range.To.ShouldBe(to, "To should be set to the end time");
    }

    [Fact]
    public void Ctor_Throws_When_From_After_To()
    {
        var ex = Should.Throw<ArgumentException>(() => 
            new TimeOnlyRange(new TimeOnly(17, 0, 0), new TimeOnly(9, 0, 0)),
            "Constructor should throw when From is after To");
        
        ex.ParamName.ShouldBe("from", "Exception should indicate 'from' parameter");
        ex.Message.ShouldContain("'From' must be less than or equal to 'To'");
    }

    [Fact]
    public void Ctor_Respects_InclusiveStart_Parameter()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0), 
            inclusiveStart: false);

        range.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void Ctor_Respects_InclusiveEnd_Parameter()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0), 
            inclusiveEnd: false);

        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void Ctor_Respects_Both_Inclusive_Parameters()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0), 
            inclusiveStart: false,
            inclusiveEnd: false);

        range.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Valid_Range()
    {
        var (result, value) = TimeOnlyRange.TryCreate(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));

        result.IsValid.ShouldBeTrue("Result should be valid for a valid time range");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(new TimeOnly(9, 0, 0), "From should be set to the start time");
        value.To.ShouldBe(new TimeOnly(17, 0, 0), "To should be set to the end time");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Same_Times()
    {
        var time = new TimeOnly(14, 30, 0);
        var (result, value) = TimeOnlyRange.TryCreate(time, time);

        result.IsValid.ShouldBeTrue("Result should be valid when From equals To");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.From.ShouldBe(time, "From should be set to the provided time");
        value.To.ShouldBe(time, "To should be set to the provided time");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_From_After_To()
    {
        var (result, value) = TimeOnlyRange.TryCreate(
            new TimeOnly(17, 0, 0), 
            new TimeOnly(9, 0, 0));

        result.IsValid.ShouldBeFalse("Result should be invalid when From is after To");
        value.ShouldBeNull("Value should be null when validation fails");
    }

    [Fact]
    public void TryCreate_Returns_Correct_Error_Details_When_Invalid()
    {
        var (result, value) = TimeOnlyRange.TryCreate(
            new TimeOnly(17, 0, 0), 
            new TimeOnly(9, 0, 0));

        result.IsValid.ShouldBeFalse("Result should be invalid when From is after To");
        result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
        result.Errors[0].Message.ShouldBe("'From' must be less than or equal to 'To'.", "Error message should explain the validation failure");
        result.Errors[0].MemberName.ShouldBe("TimeOnlyRange", "Error should be associated with TimeOnlyRange");
        result.Errors[0].Code.ShouldBe("InvalidRange", "Error code should be InvalidRange");
    }

    [Fact]
    public void TryCreate_Respects_Inclusive_Parameters()
    {
        var (result, value) = TimeOnlyRange.TryCreate(
            new TimeOnly(9, 0, 0),
            new TimeOnly(17, 0, 0),
            inclusiveStart: false,
            inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid for a valid time range");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        value.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void Contains_Returns_True_For_Time_In_Inclusive_Range()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));

        range.Contains(new TimeOnly(9, 0, 0)).ShouldBeTrue("Inclusive range should contain start boundary");
        range.Contains(new TimeOnly(12, 30, 0)).ShouldBeTrue("Range should contain middle time");
        range.Contains(new TimeOnly(17, 0, 0)).ShouldBeTrue("Inclusive range should contain end boundary");
    }

    [Fact]
    public void Contains_Returns_False_For_Time_Outside_Range()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));

        range.Contains(new TimeOnly(8, 59, 59)).ShouldBeFalse("Range should not contain time before start");
        range.Contains(new TimeOnly(17, 0, 1)).ShouldBeFalse("Range should not contain time after end");
    }

    [Fact]
    public void Contains_Respects_InclusiveStart_True()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveStart: true);

        range.Contains(new TimeOnly(9, 0, 0)).ShouldBeTrue("Inclusive start should include the start boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveStart_False()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveStart: false);

        range.Contains(new TimeOnly(9, 0, 0)).ShouldBeFalse("Exclusive start should exclude the start boundary");
        range.Contains(new TimeOnly(9, 0, 1)).ShouldBeTrue("Exclusive start should include time after start boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveEnd_True()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveEnd: true);

        range.Contains(new TimeOnly(17, 0, 0)).ShouldBeTrue("Inclusive end should include the end boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveEnd_False()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveEnd: false);

        range.Contains(new TimeOnly(17, 0, 0)).ShouldBeFalse("Exclusive end should exclude the end boundary");
        range.Contains(new TimeOnly(16, 59, 59)).ShouldBeTrue("Exclusive end should include time before end boundary");
    }

    [Fact]
    public void Contains_Respects_InclusiveFlags()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(11, 0, 0), 
            inclusiveStart: false, 
            inclusiveEnd: false);

        range.Contains(new TimeOnly(9, 0, 0)).ShouldBeFalse("Exclusive start should exclude start boundary");
        range.Contains(new TimeOnly(10, 0, 0)).ShouldBeTrue("Exclusive range should contain middle time");
        range.Contains(new TimeOnly(11, 0, 0)).ShouldBeFalse("Exclusive end should exclude end boundary");
    }

    [Fact]
    public void Contains_Single_Time_Range_Inclusive()
    {
        var time = new TimeOnly(14, 30, 0);
        var range = new TimeOnlyRange(time, time);

        range.Contains(time).ShouldBeTrue("Inclusive single-time range should contain the exact time");
        range.Contains(new TimeOnly(14, 29, 59)).ShouldBeFalse("Single-time range should not contain time before");
        range.Contains(new TimeOnly(14, 30, 1)).ShouldBeFalse("Single-time range should not contain time after");
    }

    [Fact]
    public void Contains_Single_Time_Range_Exclusive_Returns_False()
    {
        var time = new TimeOnly(14, 30, 0);
        var range = new TimeOnlyRange(
            time, 
            time, 
            inclusiveStart: false, 
            inclusiveEnd: false);

        // Exclusive on both ends means the single time is not included
        range.Contains(time).ShouldBeFalse("Exclusive single-time range should not contain any time, including the boundary");
    }

    [Fact]
    public void ToString_Formats_Inclusive_Range_Correctly()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));

        range.ToString().ShouldBe("[09:00:00 .. 17:00:00]", "Inclusive range should use square brackets");
    }

    [Fact]
    public void ToString_Formats_Exclusive_Start_Correctly()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveStart: false);

        range.ToString().ShouldBe("(09:00:00 .. 17:00:00]", "Exclusive start should use opening parenthesis");
    }

    [Fact]
    public void ToString_Formats_Exclusive_End_Correctly()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveEnd: false);

        range.ToString().ShouldBe("[09:00:00 .. 17:00:00)", "Exclusive end should use closing parenthesis");
    }

    [Fact]
    public void ToString_Formats_Exclusive_Range_Correctly()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveStart: false,
            inclusiveEnd: false);

        range.ToString().ShouldBe("(09:00:00 .. 17:00:00)", "Exclusive range should use parentheses on both ends");
    }

    [Fact]
    public void ToString_Formats_Correctly()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(11, 0, 0), 
            inclusiveStart: true, 
            inclusiveEnd: false);

        range.ToString().ShouldBe("[09:00:00 .. 11:00:00)", "Mixed inclusive/exclusive range should use appropriate brackets");
    }

    [Fact]
    public void ToString_Formats_Single_Time_Range()
    {
        var time = new TimeOnly(14, 30, 0);
        var range = new TimeOnlyRange(time, time);

        range.ToString().ShouldBe("[14:30:00 .. 14:30:00]", "Single-time range should format with same time on both ends");
    }

    [Fact]
    public void Equality_Same_Values_Are_Equal()
    {
        var range1 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));
        var range2 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));

        range1.ShouldBe(range2, "Ranges with same values should be equal");
        (range1 == range2).ShouldBeTrue("Equality operator should return true for equal ranges");
    }

    [Fact]
    public void Equality_Different_From_Are_Not_Equal()
    {
        var range1 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));
        var range2 = new TimeOnlyRange(
            new TimeOnly(10, 0, 0), 
            new TimeOnly(17, 0, 0));

        range1.ShouldNotBe(range2, "Ranges with different From times should not be equal");
    }

    [Fact]
    public void Equality_Different_To_Are_Not_Equal()
    {
        var range1 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0));
        var range2 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(16, 0, 0));

        range1.ShouldNotBe(range2, "Ranges with different To times should not be equal");
    }

    [Fact]
    public void Equality_Different_InclusiveStart_Are_Not_Equal()
    {
        var range1 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveStart: true);
        var range2 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveStart: false);

        range1.ShouldNotBe(range2, "Ranges with different InclusiveStart should not be equal");
    }

    [Fact]
    public void Equality_Different_InclusiveEnd_Are_Not_Equal()
    {
        var range1 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveEnd: true);
        var range2 = new TimeOnlyRange(
            new TimeOnly(9, 0, 0), 
            new TimeOnly(17, 0, 0),
            inclusiveEnd: false);

        range1.ShouldNotBe(range2, "Ranges with different InclusiveEnd should not be equal");
    }

    [Fact]
    public void Works_With_Min_TimeOnly()
    {
        var range = new TimeOnlyRange(
            TimeOnly.MinValue, 
            new TimeOnly(12, 0, 0));

        range.From.ShouldBe(TimeOnly.MinValue, "From should be set to TimeOnly.MinValue");
        range.Contains(TimeOnly.MinValue).ShouldBeTrue("Range should contain TimeOnly.MinValue as start boundary");
    }

    [Fact]
    public void Works_With_Max_TimeOnly()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(12, 0, 0), 
            TimeOnly.MaxValue);

        range.To.ShouldBe(TimeOnly.MaxValue, "To should be set to TimeOnly.MaxValue");
        range.Contains(TimeOnly.MaxValue).ShouldBeTrue("Range should contain TimeOnly.MaxValue as end boundary");
    }

    [Fact]
    public void Works_With_Midnight_Times()
    {
        var range = new TimeOnlyRange(
            new TimeOnly(0, 0, 0), 
            new TimeOnly(0, 0, 1));

        range.Contains(new TimeOnly(0, 0, 0)).ShouldBeTrue("Range should contain midnight (start boundary)");
        range.Contains(new TimeOnly(0, 0, 1)).ShouldBeTrue("Range should contain one second past midnight (end boundary)");
    }

    [Fact]
    public void Works_With_Microsecond_Precision()
    {
        var from = new TimeOnly(14, 30, 0, 123, 456);
        var to = new TimeOnly(14, 30, 0, 123, 789);
        var range = new TimeOnlyRange(from, to);

        range.From.ShouldBe(from, "From should preserve microsecond precision");
        range.To.ShouldBe(to, "To should preserve microsecond precision");
        range.Contains(new TimeOnly(14, 30, 0, 123, 500)).ShouldBeTrue("Range should work with microsecond precision values");
    }

    [Fact]
    public void UntilNow_Creates_Range_From_Past_Time_To_Now()
    {
        // Use TimeOnly.MinValue to ensure 'from' is always before 'to' regardless of current time
        var from = TimeOnly.MinValue;
        var beforeCreate = TimeOnly.FromDateTime(DateTime.Now);
        var (result, range) = from.UntilNow();
        var afterCreate = TimeOnly.FromDateTime(DateTime.Now);

        result.IsValid.ShouldBeTrue($"Result should be valid when creating range from past to now: {result.ToSingleMessage()}");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.From.ShouldBe(from, "From should be the specified time");
        range.To.ShouldBeGreaterThanOrEqualTo(beforeCreate, "To should be at or after the time when method was called");
        range.To.ShouldBeLessThanOrEqualTo(afterCreate, "To should be at or before the time after method call");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void UntilNow_Respects_Inclusive_Parameters()
    {
        // Use TimeOnly.MinValue to ensure 'from' is always before 'to' regardless of current time
        var from = TimeOnly.MinValue;
        var (result, range) = from.UntilNow(inclusiveStart: false, inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from past to now");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void UntilNow_Contains_Current_Time()
    {
        // Use TimeOnly.MinValue to ensure 'from' is always before 'to' regardless of current time
        var from = TimeOnly.MinValue;
        var (result, range) = from.UntilNow();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from midnight to now");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        // The range should contain a time that was current when we created it
        range!.Contains(range.To).ShouldBeTrue("Range should contain its end boundary (current time when created)");
    }

    [Fact]
    public void FromNowUntil_Creates_Range_From_Now_To_Future_Time()
    {
        // Use TimeOnly.MaxValue to ensure 'to' is always after 'from' regardless of current time
        var to = TimeOnly.MaxValue;
        var beforeCreate = TimeOnly.FromDateTime(DateTime.Now);
        var (result, range) = to.FromNowUntil();
        var afterCreate = TimeOnly.FromDateTime(DateTime.Now);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from now to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.From.ShouldBeGreaterThanOrEqualTo(beforeCreate, "From should be at or after the time when method was called");
        range.From.ShouldBeLessThanOrEqualTo(afterCreate, "From should be at or before the time after method call");
        range.To.ShouldBe(to, "To should be the specified time");
        range.InclusiveStart.ShouldBeTrue("InclusiveStart should default to true");
        range.InclusiveEnd.ShouldBeTrue("InclusiveEnd should default to true");
    }

    [Fact]
    public void FromNowUntil_Respects_Inclusive_Parameters()
    {
        // Use TimeOnly.MaxValue to ensure 'to' is always after 'from' regardless of current time
        var to = TimeOnly.MaxValue;
        var (result, range) = to.FromNowUntil(inclusiveStart: false, inclusiveEnd: false);

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from now to future");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        range!.InclusiveStart.ShouldBeFalse("InclusiveStart should be false when explicitly set");
        range.InclusiveEnd.ShouldBeFalse("InclusiveEnd should be false when explicitly set");
    }

    [Fact]
    public void FromNowUntil_Contains_Current_Time()
    {
        // Use TimeOnly.MaxValue to ensure 'to' is always after 'from' regardless of current time
        var to = TimeOnly.MaxValue;
        var (result, range) = to.FromNowUntil();

        result.IsValid.ShouldBeTrue("Result should be valid when creating range from now to end of day");
        range.ShouldNotBeNull("Range should not be null when validation succeeds");
        // The range should contain a time that was current when we created it
        range!.Contains(range.From).ShouldBeTrue("Range should contain its start boundary (current time when created)");
    }
}
