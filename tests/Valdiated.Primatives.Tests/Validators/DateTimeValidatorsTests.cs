using Xunit;
using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.DateRanges;

namespace Validated.Primitives.Tests.Validators;

public class DateTimeValidatorsTests
{
    [Fact]
    public void GivenFromTodayForward_WhenToday_ThenShouldBeValid()
    {
        var today = DateTime.UtcNow.Date;
        var result = DateTimeValidators.FromTodayForward()(today);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenFromTodayForward_WhenFutureDate_ThenShouldBeValid()
    {
        var future = DateTime.UtcNow.Date.AddDays(10);
        var result = DateTimeValidators.FromTodayForward()(future);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenFromTodayForward_WhenPastDate_ThenShouldBeInvalid()
    {
        var past = DateTime.UtcNow.Date.AddDays(-1);
        var result = DateTimeValidators.FromTodayForward()(past);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Message.ShouldContain("must be today or in the future");
    }

    [Fact]
    public void GivenFromTodayForwardWithCustomFieldName_WhenPastDate_ThenShouldUseCustomFieldName()
    {
        var past = DateTime.UtcNow.Date.AddDays(-1);
        var result = DateTimeValidators.FromTodayForward("AppointmentDate")(past);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("AppointmentDate");
    }

    [Fact]
    public void GivenBeforeToday_WhenPastDate_ThenShouldBeValid()
    {
        var past = DateTime.UtcNow.Date.AddDays(-1);
        var result = DateTimeValidators.BeforeToday()(past);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenBeforeToday_WhenToday_ThenShouldBeInvalid()
    {
        var today = DateTime.UtcNow.Date;
        var result = DateTimeValidators.BeforeToday()(today);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Message.ShouldContain("must be before today");
    }

    [Fact]
    public void GivenBeforeToday_WhenFutureDate_ThenShouldBeInvalid()
    {
        var future = DateTime.UtcNow.Date.AddDays(1);
        var result = DateTimeValidators.BeforeToday()(future);
        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GivenBeforeTodayWithCustomFieldName_WhenFutureDate_ThenShouldUseCustomFieldName()
    {
        var future = DateTime.UtcNow.Date.AddDays(1);
        var result = DateTimeValidators.BeforeToday("BirthDate")(future);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("BirthDate");
    }

    [Fact]
    public void GivenBetweenWithDateRange_WhenDateWithinRange_ThenShouldBeValid()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"),
            DateTime.Parse("2020-12-31"));
        var dateInRange = DateTime.Parse("2020-06-15");
        
        var result = DateTimeValidators.Between("TestDate", range)(dateInRange);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenBetweenWithDateRange_WhenDateBeforeRange_ThenShouldBeInvalid()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"),
            DateTime.Parse("2020-12-31"));
        var dateBefore = DateTime.Parse("2019-12-31");
        
        var result = DateTimeValidators.Between("TestDate", range)(dateBefore);
        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GivenBetweenWithDateRange_WhenDateAfterRange_ThenShouldBeInvalid()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"),
            DateTime.Parse("2020-12-31"));
        var dateAfter = DateTime.Parse("2021-01-01");
        
        var result = DateTimeValidators.Between("TestDate", range)(dateAfter);
        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GivenBetweenWithDates_WhenDateWithinRange_ThenShouldBeValid()
    {
        var from = DateTime.Parse("2020-01-01");
        var to = DateTime.Parse("2020-12-31");
        var dateInRange = DateTime.Parse("2020-06-15");
        
        var result = DateTimeValidators.Between("TestDate", from, to)(dateInRange);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenBetweenWithDates_WhenInclusiveFlagSet_ThenShouldRespectFlag()
    {
        var from = DateTime.Parse("2020-01-01");
        var to = DateTime.Parse("2020-12-31");
        
        // Inclusive (default)
        var resultInclusive = DateTimeValidators.Between("TestDate", from, to, inclusive: true)(from);
        resultInclusive.IsValid.ShouldBeTrue();
        
        // Exclusive
        var resultExclusive = DateTimeValidators.Between("TestDate", from, to, inclusive: false)(from);
        resultExclusive.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GivenBetween_WhenDateOutsideRange_ThenShouldReturnCorrectErrorCode()
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"),
            DateTime.Parse("2020-12-31"));
        var dateOutside = DateTime.Parse("2021-06-15");
        
        var result = DateTimeValidators.Between("TestDate", range)(dateOutside);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Between");
    }

    [Theory]
    [InlineData("2020-01-01", "2020-01-01", true)]  // Start boundary, inclusive
    [InlineData("2020-12-31", "2020-12-31", true)]  // End boundary, inclusive
    [InlineData("2020-06-15", "2020-06-15", true)]  // Middle
    public void GivenBetween_WhenBoundaryConditions_ThenShouldValidateCorrectly(string fromDate, string testDate, bool shouldBeValid)
    {
        var range = new DateRange(
            DateTime.Parse("2020-01-01"),
            DateTime.Parse("2020-12-31"));
        var date = DateTime.Parse(testDate);
        
        var result = DateTimeValidators.Between("TestDate", range)(date);
        result.IsValid.ShouldBe(shouldBeValid);
    }
}
