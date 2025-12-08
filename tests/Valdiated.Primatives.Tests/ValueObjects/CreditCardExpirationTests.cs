using System;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class CreditCardExpirationTests
{
    [Fact]
    public void TryCreate_WithFutureDate_ReturnsInstance()
    {
        var now = DateTime.UtcNow;
        var month = now.Month;
        var year = now.Year;

        var (result, exp) = CreditCardExpiration.TryCreate(month, year);

        result.IsValid.ShouldBeTrue();
        exp.ShouldNotBeNull();
        exp!.Month.ShouldBe(month);
        exp.Year.ShouldBe(year);
    }

    [Fact]
    public void TryCreate_WithPastDate_ReturnsFailure()
    {
        var now = DateTime.UtcNow;
        var pastYear = now.Year - 1;
        
        var (result, exp) = CreditCardExpiration.TryCreate(now.Month, pastYear);
        
        result.IsValid.ShouldBeFalse();
        exp.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Expired");
    }

    [Theory]
    [InlineData(0, 2025)]
    [InlineData(13, 2025)]
    public void TryCreate_InvalidMonth_ReturnsFailure(int month, int year)
    {
        var (result, exp) = CreditCardExpiration.TryCreate(month, year);
        
        result.IsValid.ShouldBeFalse();
        exp.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidMonth");
    }

    [Fact]
    public void TryCreate_NormalizesTwoDigitYear()
    {
        var now = DateTime.UtcNow;
        var futureYear = (now.Year + 1) % 100; // Get last two digits of next year
        
        var (result, exp) = CreditCardExpiration.TryCreate(now.Month, futureYear);
        
        result.IsValid.ShouldBeTrue();
        exp.ShouldNotBeNull();
        exp!.Year.ShouldBeGreaterThan(2000);
    }

    [Fact]
    public void ToString_ReturnsFormattedExpiration()
    {
        // Use a future date to ensure the test always passes
        var now = DateTime.UtcNow;
        var futureYear = now.Year + 1;
        
        var (result, exp) = CreditCardExpiration.TryCreate(3, futureYear);
        
        result.IsValid.ShouldBeTrue();
        exp.ShouldNotBeNull();
        exp!.ToString().ShouldBe($"03/{futureYear % 100:D2}");
    }

    [Fact]
    public void TryCreate_WithCurrentMonth_Succeeds()
    {
        var now = DateTime.UtcNow;
        
        var (result, exp) = CreditCardExpiration.TryCreate(now.Month, now.Year);
        
        result.IsValid.ShouldBeTrue();
        exp.ShouldNotBeNull();
    }
}
