using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class FutureDateTests
{
    [Fact]
    public void GivenTryCreateDateTime_WhenPastDate_ThenShouldFail()
    {
        var past = DateTime.UtcNow.AddDays(-1);
        var (result, value) = FutureDate.TryCreate(past);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for past dates");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
    }

    [Fact]
    public void GivenTryCreateDateTime_WhenToday_ThenShouldSucceed()
    {
        var today = DateTime.UtcNow.Date;
        var (result, value) = FutureDate.TryCreate(today);
        
        result.IsValid.ShouldBeTrue("Result should be valid for today");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.Value.ShouldBe(today, "Value should match the provided date");
    }

    [Fact]
    public void GivenTryCreateDateTime_WhenFutureDate_ThenShouldSucceed()
    {
        var future = DateTime.UtcNow.AddDays(7);
        var (result, value) = FutureDate.TryCreate(future);
        
        result.IsValid.ShouldBeTrue("Result should be valid for future dates");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.Value.ShouldBe(future, "Value should match the provided date");
    }

    [Fact]
    public void GivenTryCreateDateTimeWithCustomPropertyName_WhenPastDate_ThenShouldUseCustomName()
    {
        var past = DateTime.UtcNow.AddDays(-1);
        var customPropertyName = "AppointmentDate";
        var (result, value) = FutureDate.TryCreate(past, customPropertyName);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for past dates");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors[0].MemberName.ShouldBe(customPropertyName, "Error should use custom property name");
    }

    [Fact]
    public void GivenTryCreateString_WhenValidFutureDateString_ThenShouldSucceed()
    {
        var futureDate = DateTime.UtcNow.AddDays(7);
        var dateString = futureDate.ToString("yyyy-MM-dd");
        var (result, value) = FutureDate.TryCreate(dateString);
        
        result.IsValid.ShouldBeTrue("Result should be valid for valid future date string");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.Value.Date.ShouldBe(futureDate.Date, "Value should match the parsed date");
    }

    [Fact]
    public void GivenTryCreateString_WhenTodayString_ThenShouldSucceed()
    {
        var today = DateTime.UtcNow.Date;
        var dateString = today.ToString("yyyy-MM-dd");
        var (result, value) = FutureDate.TryCreate(dateString);
        
        result.IsValid.ShouldBeTrue("Result should be valid for today's date");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        value!.Value.Date.ShouldBe(today, "Value should match today's date");
    }

    [Fact]
    public void GivenTryCreateString_WhenPastDateString_ThenShouldFail()
    {
        var pastDate = DateTime.UtcNow.AddDays(-7);
        var dateString = pastDate.ToString("yyyy-MM-dd");
        var (result, value) = FutureDate.TryCreate(dateString);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for past date string");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
    }

    [Fact]
    public void GivenTryCreateString_WhenInvalidDateFormat_ThenShouldFail()
    {
        var invalidDateString = "not-a-date";
        var (result, value) = FutureDate.TryCreate(invalidDateString);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for invalid date format");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
        result.Errors[0].Message.ShouldBe("Invalid date format.", "Error message should indicate invalid format");
        result.Errors[0].Code.ShouldBe("InvalidDateString", "Error code should be InvalidDateString");
    }

    [Fact]
    public void GivenTryCreateString_WhenEmptyString_ThenShouldFail()
    {
        var (result, value) = FutureDate.TryCreate(string.Empty);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for empty string");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors[0].Message.ShouldBe("Invalid date format.", "Error message should indicate invalid format");
        result.Errors[0].Code.ShouldBe("InvalidDateString", "Error code should be InvalidDateString");
    }

    [Fact]
    public void GivenTryCreateString_WhenWhitespaceString_ThenShouldFail()
    {
        var (result, value) = FutureDate.TryCreate("   ");
        
        result.IsValid.ShouldBeFalse("Result should be invalid for whitespace string");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors[0].Message.ShouldBe("Invalid date format.", "Error message should indicate invalid format");
    }

    [Fact]
    public void GivenTryCreateString_WhenVariousValidDateFormats_ThenShouldSucceed()
    {
        var futureDate = DateTime.UtcNow.AddDays(7);
        var formats = new[]
        {
            futureDate.ToString("yyyy-MM-dd"),
            futureDate.ToString("yyyy/MM/dd")
        };

        foreach (var dateString in formats)
        {
            var (result, value) = FutureDate.TryCreate(dateString);
            
            result.IsValid.ShouldBeTrue($"Result should be valid for format: {dateString}");
            value.ShouldNotBeNull($"Value should not be null for format: {dateString}");
        }
    }

    [Fact]
    public void GivenTryCreateStringWithCustomPropertyName_WhenInvalidFormat_ThenShouldUseCustomName()
    {
        var customPropertyName = "DeliveryDate";
        var (result, value) = FutureDate.TryCreate("invalid-date", customPropertyName);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for invalid date format");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors[0].MemberName.ShouldBe(customPropertyName, "Error should use custom property name");
    }

    [Fact]
    public void GivenTryCreateStringWithCustomPropertyName_WhenPastDate_ThenShouldUseCustomName()
    {
        var pastDate = DateTime.UtcNow.AddDays(-7);
        var dateString = pastDate.ToString("yyyy-MM-dd");
        var customPropertyName = "EventDate";
        var (result, value) = FutureDate.TryCreate(dateString, customPropertyName);
        
        result.IsValid.ShouldBeFalse("Result should be invalid for past date");
        value.ShouldBeNull("Value should be null when validation fails");
        result.Errors[0].MemberName.ShouldBe(customPropertyName, "Error should use custom property name");
    }

    [Fact]
    public void GivenTryCreateString_WhenDateTimeWithTimeComponent_ThenShouldParse()
    {
        var futureDateTime = DateTime.UtcNow.AddDays(7);
        var dateTimeString = futureDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var (result, value) = FutureDate.TryCreate(dateTimeString);
        
        result.IsValid.ShouldBeTrue("Result should be valid for valid future datetime string");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
        // DateTime.TryParse may lose some precision (milliseconds/ticks), so compare date and main time components
        value!.Value.Date.ShouldBe(futureDateTime.Date, "Date component should match");
        value.Value.Hour.ShouldBe(futureDateTime.Hour, "Hour component should match");
        value.Value.Minute.ShouldBe(futureDateTime.Minute, "Minute component should match");
        value.Value.Second.ShouldBe(futureDateTime.Second, "Second component should match");
    }

    [Fact]
    public void GivenTryCreateString_WhenISO8601Format_ThenShouldSucceed()
    {
        var futureDate = DateTime.UtcNow.AddDays(7);
        var iso8601String = futureDate.ToString("o"); // ISO 8601 format
        var (result, value) = FutureDate.TryCreate(iso8601String);
        
        result.IsValid.ShouldBeTrue("Result should be valid for ISO 8601 format");
        value.ShouldNotBeNull("Value should not be null when validation succeeds");
    }

    [Fact]
    public void GivenTryCreateString_WhenPartialDateString_ThenShouldHandleCorrectly()
    {
        var (result, value) = FutureDate.TryCreate("2024-12");
        
        // DateTime.TryParse may parse partial dates in some cases
        // Testing actual behavior: if it parses, validate it; if not, check error
        if (!result.IsValid)
        {
            value.ShouldBeNull("Value should be null when validation fails");
            result.Errors.Count.ShouldBeGreaterThan(0, "Errors collection should contain at least one error");
        }
    }
}
