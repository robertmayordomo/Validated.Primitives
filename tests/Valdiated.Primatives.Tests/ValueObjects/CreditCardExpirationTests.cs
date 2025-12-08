using System;
using Xunit;
using Validated.Primitives.ValueObjects;

namespace Valdiated.Primatives.Tests.ValueObjects;

public class CreditCardExpirationTests
{
    [Fact]
    public void Create_WithFutureDate_ReturnsInstance()
    {
        var now = DateTime.UtcNow;
        var month = now.Month;
        var year = now.Year;

        var exp = CreditCardExpiration.Create(month, year);

        Assert.Equal(month, exp.Month);
        Assert.Equal(year, exp.Year);
    }

    [Fact]
    public void Create_WithPastDate_Throws()
    {
        var now = DateTime.UtcNow;
        var pastYear = now.Year - 1;
        Assert.Throws<ArgumentException>(() => CreditCardExpiration.Create(now.Month, pastYear));
    }

    [Theory]
    [InlineData(0, 2025)]
    [InlineData(13, 2025)]
    public void Create_InvalidMonth_Throws(int month, int year)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreditCardExpiration.Create(month, year));
    }
}
