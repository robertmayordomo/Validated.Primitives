using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class SortCodeValidatorsTests
{
    [Theory]
    [InlineData("123456")]
    [InlineData("12-34-56")]
    [InlineData("12 34 56")]
    [InlineData("  123456  ")]
    public void GivenValidFormat_WhenValidFormats_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12-34-5A")]
    [InlineData("ABC-DE-FG")]
    [InlineData("12.34.56")]
    public void GivenValidFormat_WhenInvalidCharacters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe("InvalidFormat");
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNullValue_ThenShouldFail()
    {
        // Arrange
        var validator = SortCodeValidators.NotNullOrWhitespace("TestField");

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
        var validator = SortCodeValidators.NotNullOrWhitespace("TestField");

        // Act
        var result = validator(string.Empty);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Required");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12-34-56")]
    [InlineData("12 34 56")]
    public void GivenOnlyDigits_WhenValidDigits_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SortCodeValidators.OnlyDigits("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12A456")]
    [InlineData("ABC-DEF")]
    public void GivenOnlyDigits_WhenContainsLetters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SortCodeValidators.OnlyDigits("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCharacters");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12-34-56")]
    [InlineData("12 34 56")]
    public void GivenValidateCountryFormat_WhenValidUkSortCode_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidateCountryFormat("TestField", CountryCode.UnitedKingdom);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12-345-6")]
    public void GivenValidateCountryFormat_WhenInvalidUkSortCode_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidateCountryFormat("TestField", CountryCode.UnitedKingdom);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountrySortCodeFormat");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12-34-56")]
    [InlineData("12 34 56")]
    public void GivenValidateCountryFormat_WhenValidIrelandSortCode_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidateCountryFormat("TestField", CountryCode.Ireland);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    public void GivenValidateCountryFormat_WhenInvalidIrelandSortCode_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidateCountryFormat("TestField", CountryCode.Ireland);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountrySortCodeFormat");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("ABC123")]
    public void GivenValidateCountryFormat_WhenUnsupportedCountry_ThenShouldPassThrough(string value)
    {
        // Arrange
        var validator = SortCodeValidators.ValidateCountryFormat("TestField", CountryCode.UnitedStates);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNoFieldNameProvided_ThenShouldUseDefault()
    {
        // Arrange
        var validator = SortCodeValidators.NotNullOrWhitespace();

        // Act
        var result = validator(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("SortCode");
    }
}
