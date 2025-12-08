using System;
using Xunit;
using Validated.Primitives.ValueObjects;

namespace Valdiated.Primatives.Tests.ValueObjects;

public class CreditCardNumberTests
{
    [Fact]
    public void Create_WithValidNumber_ReturnsInstance()
    {
        var cc = CreditCardNumber.Create("4111 1111 1111 1111"); // Visa test number
        Assert.Equal("4111111111111111", cc.ToString());
        Assert.Equal("************1111", cc.Masked());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("0000 0000 0000 0000")]
    public void Create_Invalid_Throws(string input)
    {
        Assert.Throws<ArgumentException>(() => CreditCardNumber.Create(input));
    }
}
