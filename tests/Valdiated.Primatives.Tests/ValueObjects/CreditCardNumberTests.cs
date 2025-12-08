using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class CreditCardNumberTests
{
    [Fact]
    public void TryCreate_WithValidNumber_ReturnsInstance()
    {
        var (result, cc) = CreditCardNumber.TryCreate("4111 1111 1111 1111"); // Visa test number
        result.IsValid.ShouldBeTrue();
        cc.ShouldNotBeNull();
        cc!.ToString().ShouldBe("4111111111111111");
        cc.Masked().ShouldBe("************1111");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("0000 0000 0000 0000")]
    public void TryCreate_Invalid_ReturnsFailure(string input)
    {
        var (result, cc) = CreditCardNumber.TryCreate(input);
        result.IsValid.ShouldBeFalse();
        cc.ShouldBeNull();
    }
}
