using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class PostalCodeTests
{
    [Theory]
    [InlineData(CountryCode.UnitedStates, "12345")]
    [InlineData(CountryCode.UnitedStates, "90210")]
    [InlineData(CountryCode.UnitedStates, "12345-6789")]
    [InlineData(CountryCode.UnitedKingdom, "SW1A 1AA")]
    [InlineData(CountryCode.UnitedKingdom, "EC1A1BB")]
    [InlineData(CountryCode.Canada, "K1A 0B1")]
    [InlineData(CountryCode.Canada, "M5W1E6")]
    [InlineData(CountryCode.France, "75001")]
    [InlineData(CountryCode.Germany, "10115")]
    [InlineData(CountryCode.Austria, "1010")]
    [InlineData(CountryCode.Japan, "100-0001")]
    [InlineData(CountryCode.Japan, "1234567")]
    [InlineData(CountryCode.Australia, "2000")]
    [InlineData(CountryCode.Netherlands, "1234 AB")]
    [InlineData(CountryCode.Netherlands, "1234AB")]
    [InlineData(CountryCode.Hungary, "H-1075")]
    [InlineData(CountryCode.Hungary, "1075")]
    [InlineData(CountryCode.Poland, "00-001")]
    [InlineData(CountryCode.Ireland, "A65 F4E2")]
    [InlineData(CountryCode.Brazil, "12345-678")]
    public void TryCreate_WithValidPostalCodes_ShouldSucceed(CountryCode countryCode, string postalCode)
    {
        // Act
        var (result, value) = PostalCode.TryCreate(countryCode, postalCode);

        // Assert
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value.Value.ShouldBe(postalCode);
        value.CountryCode.ShouldBe(countryCode);
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "")]
    [InlineData(CountryCode.UnitedStates, " ")]
    [InlineData(CountryCode.UnitedStates, null)]
    public void TryCreate_WithNullOrWhitespace_ShouldFail(CountryCode countryCode, string? postalCode)
    {
        // Act
        var (result, value) = PostalCode.TryCreate(countryCode, postalCode!);

        // Assert
        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.ToSingleMessage().ShouldContain("cannot be null or whitespace");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "1")]
    [InlineData(CountryCode.UnitedStates, "12345678901")]
    public void TryCreate_WithInvalidLength_ShouldFail(CountryCode countryCode, string postalCode)
    {
        // Act
        var (result, value) = PostalCode.TryCreate(countryCode, postalCode);

        // Assert
        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.ToSingleMessage().ShouldContain("characters");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "12345!")]
    [InlineData(CountryCode.UnitedStates, "ABC@123")]
    [InlineData(CountryCode.UnitedStates, "12345#")]
    [InlineData(CountryCode.UnitedStates, "ABC*123")]
    [InlineData(CountryCode.UnitedStates, "12.345")]
    public void TryCreate_WithInvalidCharacters_ShouldFail(CountryCode countryCode, string postalCode)
    {
        // Act
        var (result, value) = PostalCode.TryCreate(countryCode, postalCode);

        // Assert
        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.ToSingleMessage().ShouldContain("can only contain letters, numbers, spaces, and hyphens");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "ABCDE")]
    [InlineData(CountryCode.UnitedKingdom, "12345")]
    [InlineData(CountryCode.Canada, "12345")]
    [InlineData(CountryCode.Japan, "12345")]
    public void TryCreate_WithInvalidFormatForCountry_ShouldFail(CountryCode countryCode, string postalCode)
    {
        // Act
        var (result, value) = PostalCode.TryCreate(countryCode, postalCode);

        // Assert
        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.ToSingleMessage().ShouldContain($"not a valid postal code format for {countryCode}");
    }

    [Fact]
    public void GetCountryName_ShouldReturnCorrectName()
    {
        // Arrange
        var (_, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, "12345");

        // Act
        var countryName = postalCode!.GetCountryName();

        // Assert
        countryName.ShouldBe("United States");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "12345", "United States")]
    [InlineData(CountryCode.UnitedKingdom, "SW1A 1AA", "United Kingdom")]
    [InlineData(CountryCode.Canada, "K1A 0B1", "Canada")]
    [InlineData(CountryCode.Japan, "100-0001", "Japan")]
    [InlineData(CountryCode.France, "75001", "France")]
    [InlineData(CountryCode.All, "12345", "All Countries")]
    public void GetCountryName_ForVariousCountries_ShouldReturnCorrectNames(CountryCode countryCode, string postalCode, string expectedCountry)
    {
        // Arrange
        var (_, value) = PostalCode.TryCreate(countryCode, postalCode);

        // Act
        var countryName = value!.GetCountryName();

        // Assert
        countryName.ShouldBe(expectedCountry);
    }

    [Fact]
    public void CountryCode_Unknown_ShouldAcceptAnyFormat()
    {
        // Arrange & Act
        var (result, postalCode) = PostalCode.TryCreate(CountryCode.Unknown, "ABC123");

        // Assert
        result.IsValid.ShouldBeTrue();
        postalCode!.CountryCode.ShouldBe(CountryCode.Unknown);
        postalCode.GetCountryName().ShouldBe("Unknown");
    }

    [Fact]
    public void CountryCode_All_ShouldAcceptAnyFormat()
    {
        // Arrange & Act
        var (result, postalCode) = PostalCode.TryCreate(CountryCode.All, "XYZ789");

        // Assert
        result.IsValid.ShouldBeTrue();
        postalCode!.CountryCode.ShouldBe(CountryCode.All);
        postalCode.GetCountryName().ShouldBe("All Countries");
    }

    [Fact]
    public void PostalCode_ShouldHaveRecordSemantics()
    {
        // Arrange
        var (_, postalCode1) = PostalCode.TryCreate(CountryCode.UnitedStates, "12345");
        var (_, postalCode2) = PostalCode.TryCreate(CountryCode.UnitedStates, "12345");
        var (_, postalCode3) = PostalCode.TryCreate(CountryCode.UnitedStates, "54321");

        // Assert
        postalCode1!.Value.ShouldBe(postalCode2!.Value);
        postalCode1.CountryCode.ShouldBe(postalCode2.CountryCode);
        postalCode1.Value.ShouldNotBe(postalCode3!.Value);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var postalCodeString = "SW1A 1AA";
        var (_, postalCode) = PostalCode.TryCreate(CountryCode.UnitedKingdom, postalCodeString);

        // Act
        var result = postalCode!.ToString();

        // Assert
        result.ShouldBe(postalCodeString);
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_ShouldUseInErrorMessage()
    {
        // Arrange
        var customPropertyName = "ShippingPostalCode";

        // Act
        var (result, value) = PostalCode.TryCreate(CountryCode.UnitedStates, "", customPropertyName);

        // Assert
        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.ToSingleMessage().ShouldContain(customPropertyName);
    }
}
