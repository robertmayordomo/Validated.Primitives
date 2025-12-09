using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class PassportValidatorsTests
{
    #region NotNullOrWhitespace Tests

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNullValue_ThenShouldFail()
    {
        // Arrange
        var validator = PassportValidators.NotNullOrWhitespace("TestField");

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
        var validator = PassportValidators.NotNullOrWhitespace();

        // Act
        var result = validator(string.Empty);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Required");
    }

    #endregion

    #region ValidFormat Tests

    [Theory]
    [InlineData("123456789")]
    [InlineData("AB123456")]
    [InlineData("K1234567")]
    [InlineData("AB 123456")]  // With space
    [InlineData("AB-123456")]  // With hyphen
    public void GivenValidFormat_WhenValidFormats_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("AB@123456")]
    [InlineData("AB#123456")]
    [InlineData("AB.123456")]
    public void GivenValidFormat_WhenInvalidCharacters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidFormat");
    }

    #endregion

    #region North America Tests

    [Theory]
    [InlineData("123456789")]   // Valid 9 digits
    [InlineData("987654321")]
    public void GivenValidCountryFormat_WhenValidUsPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.UnitedStates, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("12345678")]    // Too short
    [InlineData("1234567890")]  // Too long
    [InlineData("AB123456")]    // Contains letters
    public void GivenValidCountryFormat_WhenInvalidUsPassport_ThenShouldFail(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.UnitedStates, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryFormat");
    }

    [Theory]
    [InlineData("AB123456")]    // Valid format
    [InlineData("ZY987654")]
    public void GivenValidCountryFormat_WhenValidCanadaPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Canada, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("G123456789")]  // Valid format
    [InlineData("A987654321")]
    public void GivenValidCountryFormat_WhenValidMexicoPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Mexico, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region UK & Ireland Tests

    [Theory]
    [InlineData("123456789")]
    [InlineData("AB1234567")]
    public void GivenValidCountryFormat_WhenValidUkPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.UnitedKingdom, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("AB1234567")]  // Valid format
    [InlineData("ZY9876543")]
    public void GivenValidCountryFormat_WhenValidIrelandPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Ireland, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Western Europe Tests

    [Theory]
    [InlineData("C01X00T47")]   // 9 characters
    [InlineData("C01X00T471")]  // 10 characters
    public void GivenValidCountryFormat_WhenValidGermanyPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Germany, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("12AB12345")]
    [InlineData("99ZY98765")]
    public void GivenValidCountryFormat_WhenValidFrancePassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.France, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("NL123456")]    // 8 characters
    [InlineData("NPABC1234")]   // 9 characters
    public void GivenValidCountryFormat_WhenValidNetherlandsPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Netherlands, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Asia Tests

    [Theory]
    [InlineData("K1234567")]    // Valid format
    [InlineData("Z9876543")]
    public void GivenValidCountryFormat_WhenValidIndiaPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.India, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("1234567")]     // Missing letter
    [InlineData("KK1234567")]   // Too many letters
    public void GivenValidCountryFormat_WhenInvalidIndiaPassport_ThenShouldFail(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.India, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    [Theory]
    [InlineData("K1234567A")]   // Valid format
    [InlineData("Z9876543B")]
    public void GivenValidCountryFormat_WhenValidSingaporePassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Singapore, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("TK1234567")]
    [InlineData("AB9876543")]
    public void GivenValidCountryFormat_WhenValidJapanPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Japan, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("E12345678")]   // 1 letter + 8 digits
    [InlineData("PE1234567")]   // 2 letters + 7 digits
    public void GivenValidCountryFormat_WhenValidChinaPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.China, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Oceania Tests

    [Theory]
    [InlineData("N1234567")]    // 1 letter + 7 digits
    [InlineData("PA1234567")]   // 2 letters + 7 digits
    public void GivenValidCountryFormat_WhenValidAustraliaPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Australia, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("LA123456")]
    [InlineData("NZ987654")]
    public void GivenValidCountryFormat_WhenValidNewZealandPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.NewZealand, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Other Countries Tests

    [Theory]
    [InlineData("A12345678")]
    [InlineData("Z98765432")]
    public void GivenValidCountryFormat_WhenValidSouthAfricaPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.SouthAfrica, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("AB123456")]    // 8 characters
    [InlineData("AB1234567")]   // 9 characters
    public void GivenValidCountryFormat_WhenValidBrazilPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Brazil, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Nordic Countries Tests

    [Theory]
    [InlineData("12345678")]
    [InlineData("98765432")]
    public void GivenValidCountryFormat_WhenValidSwedenPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Sweden, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("987654321")]
    public void GivenValidCountryFormat_WhenValidDenmarkPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Denmark, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("AB1234567")]
    [InlineData("FI9876543")]
    public void GivenValidCountryFormat_WhenValidFinlandPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Finland, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("1234567")]     // 7 characters
    [InlineData("12345678")]    // 8 characters
    public void GivenValidCountryFormat_WhenValidNorwayPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Norway, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Eastern Europe Tests

    [Theory]
    [InlineData("AB1234567")]
    [InlineData("PL9876543")]
    public void GivenValidCountryFormat_WhenValidPolandPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.Poland, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("12345678")]    // 8 digits
    [InlineData("123456789")]   // 9 digits
    public void GivenValidCountryFormat_WhenValidCzechRepublicPassport_ThenShouldPass(string value)
    {
        // Arrange
        var validator = PassportValidators.ValidCountryFormat(CountryCode.CzechRepublic, "TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion
}
