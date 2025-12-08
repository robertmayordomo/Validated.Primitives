using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class CreditCardSecurityNumberTests
{
    [Theory]
    [InlineData("123")]
    [InlineData(" 123 ")]
    [InlineData("0123")]
    public void TryCreate_Valid_ReturnsInstance(string input)
    {
        var (result, cvv) = CreditCardSecurityNumber.TryCreate(input);
        result.IsValid.ShouldBeTrue();
        cvv.ShouldNotBeNull();

        var expectedDigits = new string(input.Where(char.IsDigit).ToArray());
        cvv!.ToString().ShouldBe(expectedDigits);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("12345")]
    public void TryCreate_Invalid_ReturnsFailure(string input)
    {
        var (result, cvv) = CreditCardSecurityNumber.TryCreate(input);
        result.IsValid.ShouldBeFalse();
        cvv.ShouldBeNull();
    }
}
