using Shouldly;
using Validated.Primitives.Validators;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class RoutingNumberValidatorsTests
{
    [Fact]
    public void GivenNotNullOrWhitespace_WhenNullValue_ThenShouldFail()
    {
        // Arrange
        var validator = RoutingNumberValidators.NotNullOrWhitespace("TestField");

        // Act
        var result = validator(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Required");
        result.Errors[0].MemberName.ShouldBe("TestField");
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenEmptyString_ThenShouldFail()
    {
        // Arrange
        var validator = RoutingNumberValidators.NotNullOrWhitespace("TestField");

        // Act
        var result = validator(string.Empty);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Required");
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNoFieldNameProvided_ThenShouldUseDefault()
    {
        // Arrange
        var validator = RoutingNumberValidators.NotNullOrWhitespace();

        // Act
        var result = validator(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("RoutingNumber");
    }

    [Theory]
    [InlineData("021000021")]
    [InlineData("0210-0002-1")]
    [InlineData("0210 0002 1")]
    [InlineData("  021000021  ")]
    public void GivenValidFormat_WhenValidFormats_ThenShouldPass(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("0210A0021")]
    [InlineData("ABC-DEF-GHI")]
    [InlineData("021.000.021")]
    public void GivenValidFormat_WhenInvalidCharacters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidFormat");
    }

    [Theory]
    [InlineData("021000021")]
    [InlineData("0210-0002-1")]
    [InlineData("0210 0002 1")]
    public void GivenOnlyDigits_WhenValidDigits_ThenShouldPass(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.OnlyDigits("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("0210A0021")]
    [InlineData("ABC-DEF-GHI")]
    public void GivenOnlyDigits_WhenContainsLetters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.OnlyDigits("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCharacters");
    }

    [Fact]
    public void GivenValidLength_WhenExactly9Digits_ThenShouldPass()
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidLength("TestField");

        // Act
        var result = validator("021000021");

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("0210-0002-1")]
    [InlineData("0210 0002 1")]
    public void GivenValidLength_When9DigitsWithSeparators_ThenShouldPass(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidLength("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("02100002")]
    [InlineData("0210000211")]
    [InlineData("021")]
    public void GivenValidLength_WhenNotExactly9Digits_ThenShouldFail(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidLength("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidLength");
    }

    [Theory]
    [InlineData("021000021")] // Bank of America, NY
    [InlineData("011401533")] // Wells Fargo, CA
    [InlineData("121000248")] // Wells Fargo, TX
    [InlineData("026009593")] // Bank of America, FL
    [InlineData("111000025")] // Chase, NY
    public void GivenValidChecksum_WhenValidRoutingNumbers_ThenShouldPass(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidChecksum("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("021000020")]
    [InlineData("011401530")]
    [InlineData("121000240")]
    public void GivenValidChecksum_WhenInvalidChecksum_ThenShouldFail(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidChecksum("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidChecksum");
    }

    [Fact]
    public void GivenValidChecksum_WhenFormattedRoutingNumber_ThenShouldPass()
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidChecksum("TestField");

        // Act
        var result = validator("0210-0002-1");

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("021000021")] // 02 - valid
    [InlineData("011401533")] // 01 - valid
    [InlineData("121000248")] // 12 - valid
    [InlineData("261170175")] // 26 - valid
    [InlineData("321270742")] // 32 - valid
    [InlineData("611071975")] // 61 - valid
    [InlineData("800013527")] // 80 - valid
    public void GivenValidFederalReserveSymbol_WhenValidSymbols_ThenShouldPass(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidFederalReserveSymbol("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("131000021")] // 13 - invalid
    [InlineData("201000021")] // 20 - invalid
    [InlineData("331000021")] // 33 - invalid
    [InlineData("501000021")] // 50 - invalid
    [InlineData("731000021")] // 73 - invalid
    [InlineData("811000021")] // 81 - invalid
    [InlineData("991000021")] // 99 - invalid
    public void GivenValidFederalReserveSymbol_WhenInvalidSymbols_ThenShouldFail(string value)
    {
        // Arrange
        var validator = RoutingNumberValidators.ValidFederalReserveSymbol("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidFederalReserveSymbol");
        result.Errors[0].Message.ShouldContain("first two digits");
    }

    [Theory]
    [InlineData("021000021")]
    [InlineData("011401533")]
    [InlineData("121000248")]
    public void GivenAllValidators_WhenValidRoutingNumber_ThenAllShouldPass(string value)
    {
        // Arrange
        var validators = new[]
        {
            RoutingNumberValidators.NotNullOrWhitespace("TestField"),
            RoutingNumberValidators.ValidFormat("TestField"),
            RoutingNumberValidators.OnlyDigits("TestField"),
            RoutingNumberValidators.ValidLength("TestField"),
            RoutingNumberValidators.ValidFederalReserveSymbol("TestField"),
            RoutingNumberValidators.ValidChecksum("TestField")
        };

        // Act & Assert
        foreach (var validator in validators)
        {
            var result = validator(value);
            result.IsValid.ShouldBeTrue();
        }
    }

    [Theory]
    [InlineData("021000020")] // Invalid checksum
    [InlineData("131000021")] // Invalid Federal Reserve symbol
    [InlineData("02100002")]  // Invalid length
    public void GivenAllValidators_WhenInvalidRoutingNumber_ThenShouldFail(string value)
    {
        // Arrange
        var checksumValidator = RoutingNumberValidators.ValidChecksum("TestField");
        var symbolValidator = RoutingNumberValidators.ValidFederalReserveSymbol("TestField");
        var lengthValidator = RoutingNumberValidators.ValidLength("TestField");

        // Act
        var checksumResult = checksumValidator("021000020");
        var symbolResult = symbolValidator("131000021");
        var lengthResult = lengthValidator("02100002");

        // Assert
        checksumResult.IsValid.ShouldBeFalse();
        symbolResult.IsValid.ShouldBeFalse();
        lengthResult.IsValid.ShouldBeFalse();
    }
}
