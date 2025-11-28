using Xunit;
using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.Validators;

public class PostalCodeValidatorsTests
{
    [Theory]
    [InlineData("12345")]
    [InlineData("SW1A 1AA")]
    [InlineData("K1A 0B1")]
    [InlineData("100-0001")]
    [InlineData("ABC123")]
    [InlineData("123-ABC")]
    [InlineData("A1B 2C3")]
    public void ValidFormat_WithValidPostalCodes_ShouldReturnSuccess(string postalCode)
    {
        // Arrange
        var validator = PostalCodeValidators.ValidFormat("PostalCode");

        // Act
        var result = validator(postalCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("12345!")]
    [InlineData("ABC@123")]
    [InlineData("12.345")]
    [InlineData("AB_CD")]
    [InlineData("ABC*123")]
    [InlineData("12345#67")]
    [InlineData("AB$CD")]
    public void ValidFormat_WithInvalidCharacters_ShouldReturnFailure(string postalCode)
    {
        // Arrange
        var validator = PostalCodeValidators.ValidFormat("PostalCode");

        // Act
        var result = validator(postalCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeFalse();
        result.ToSingleMessage().ShouldContain("can only contain letters, numbers, spaces, and hyphens");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ValidFormat_WithNullOrWhitespace_ShouldReturnSuccess(string? postalCode)
    {
        // Arrange
        var validator = PostalCodeValidators.ValidFormat("PostalCode");

        // Act
        var result = validator(postalCode!);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ValidFormat_WithCustomFieldName_ShouldIncludeInErrorMessage()
    {
        // Arrange
        var fieldName = "ShippingPostalCode";
        var validator = PostalCodeValidators.ValidFormat(fieldName);

        // Act
        var result = validator("INVALID@CODE");

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeFalse();
        result.ToSingleMessage().ShouldContain(fieldName);
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "12345", true)]
    [InlineData(CountryCode.UnitedStates, "ABCDE", false)]
    [InlineData(CountryCode.UnitedKingdom, "SW1A 1AA", true)]
    [InlineData(CountryCode.UnitedKingdom, "12345", false)]
    [InlineData(CountryCode.Canada, "K1A 0B1", true)]
    [InlineData(CountryCode.Canada, "12345", false)]
    [InlineData(CountryCode.Japan, "123-4567", true)]
    [InlineData(CountryCode.Japan, "12345", false)]
    public void ValidateCountryFormat_ShouldValidateCorrectly(CountryCode countryCode, string postalCode, bool shouldBeValid)
    {
        // Arrange
        var validator = PostalCodeValidators.ValidateCountryFormat("PostalCode", countryCode);

        // Act
        var result = validator(postalCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBe(shouldBeValid);
    }

    [Theory]
    [InlineData(CountryCode.Unknown, "ANYTHING123")]
    [InlineData(CountryCode.All, "XYZ789")]
    public void ValidateCountryFormat_WithUnknownOrAll_ShouldAcceptAnyFormat(CountryCode countryCode, string postalCode)
    {
        // Arrange
        var validator = PostalCodeValidators.ValidateCountryFormat("PostalCode", countryCode);

        // Act
        var result = validator(postalCode);

        // Assert
        result.ShouldNotBeNull();
        result.IsValid.ShouldBeTrue();
    }
}
