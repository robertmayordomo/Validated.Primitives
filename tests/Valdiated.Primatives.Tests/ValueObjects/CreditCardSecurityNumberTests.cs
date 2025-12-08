using System;
using Xunit;
using Validated.Primitives.ValueObjects;

namespace Valdiated.Primatives.Tests.ValueObjects;

public class CreditCardSecurityNumberTests
{
    [Theory]
    [InlineData("123")]
    [InlineData(" 123 ")]
    [InlineData("0123")]
    public void Create_Valid_ReturnsInstance(string input)
    {
        var cvv = CreditCardSecurityNumber.Create(input);
        Assert.Equal(new string(input.Trim().Replace(" ", string.Empty).ToCharArray()), cvv.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("12345")]
    public void Create_Invalid_Throws(string input)
    {
        Assert.Throws<ArgumentException>(() => CreditCardSecurityNumber.Create(input));
    }
}
