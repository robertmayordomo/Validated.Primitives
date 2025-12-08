using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class BankAccountNumberValidatorsTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("GB12 3456 7890 1234")]
    [InlineData("DE89370400440532013000")]
    public void GivenValidFormat_WhenValidFormats_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12345@78")]
    [InlineData("ABC#DEF")]
    public void GivenValidFormat_WhenInvalidCharacters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidFormat");
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNullValue_ThenShouldFail()
    {
        // Arrange
        var validator = BankAccountNumberValidators.NotNullOrWhitespace("TestField");

        // Act
        var result = validator(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Required");
        result.Errors[0].MemberName.ShouldBe("TestField");
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("00123456")]
    [InlineData("99999999")]
    public void GivenValidateCountryFormat_WhenValidUkAccountNumber_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.UnitedKingdom);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("ABC12345")]
    public void GivenValidateCountryFormat_WhenInvalidUkAccountNumber_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.UnitedKingdom);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
        result.Errors[0].Message.ShouldContain("8 digits");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456789")]
    [InlineData("12345678901234567")]
    public void GivenValidateCountryFormat_WhenValidUsAccountNumber_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.UnitedStates);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012345678")]
    public void GivenValidateCountryFormat_WhenInvalidUsAccountNumber_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.UnitedStates);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("DE89370400440532013000")]
    [InlineData("DE44500105175407324931")]
    public void GivenValidateCountryFormat_WhenValidGermanIban_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Germany);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("GB82WEST12345698765432")]
    [InlineData("DE893704004405320130")]
    [InlineData("DE8937040044053201300000")]
    public void GivenValidateCountryFormat_WhenInvalidGermanIban_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Germany);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("FR1420041010050500013M02606")]
    [InlineData("FR7630006000011234567890189")]
    public void GivenValidateCountryFormat_WhenValidFrenchIban_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.France);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("GB82WEST12345698765432")]
    [InlineData("FR14200410100505000")]
    public void GivenValidateCountryFormat_WhenInvalidFrenchIban_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.France);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("NL91ABNA0417164300")]
    [InlineData("NL39RABO0300065264")]
    public void GivenValidateCountryFormat_WhenValidDutchIban_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Netherlands);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("IE29AIBK93115212345678")]
    [InlineData("IE64IRCE92050112345678")]
    public void GivenValidateCountryFormat_WhenValidIrishIban_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Ireland);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12345678")]
    [InlineData("123456789")]
    public void GivenValidateCountryFormat_WhenValidAustralianAccountNumber_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Australia);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567890")]
    public void GivenValidateCountryFormat_WhenInvalidAustralianAccountNumber_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Australia);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789012")]
    public void GivenValidateCountryFormat_WhenValidCanadianAccountNumber_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Canada);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234567890123")]
    public void GivenValidateCountryFormat_WhenInvalidCanadianAccountNumber_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Canada);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("1234567")]
    public void GivenValidateCountryFormat_WhenValidJapaneseAccountNumber_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Japan);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12345678")]
    public void GivenValidateCountryFormat_WhenInvalidJapaneseAccountNumber_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.Japan);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012345678")]
    public void GivenValidateCountryFormat_WhenValidIndianAccountNumber_ThenShouldPass(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.India);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("1234567890123456789")]
    public void GivenValidateCountryFormat_WhenInvalidIndianAccountNumber_ThenShouldFail(string value)
    {
        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", CountryCode.India);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("GB82WEST12345698765432")]
    [InlineData("DE89370400440532013000")]
    [InlineData("FR1420041010050500013M02606")]
    [InlineData("NL91ABNA0417164300")]
    [InlineData("IT60X0542811101000000123456")]
    [InlineData("ES9121000418450200051332")]
    public void GivenIbanChecksum_WhenValidChecksum_ThenShouldPass(string iban)
    {
        // These are real IBANs with valid checksums
        // Testing against appropriate country validators
        var countryCode = iban.Substring(0, 2) switch
        {
            "GB" => CountryCode.UnitedKingdom,
            "DE" => CountryCode.Germany,
            "FR" => CountryCode.France,
            "NL" => CountryCode.Netherlands,
            "IT" => CountryCode.Italy,
            "ES" => CountryCode.Spain,
            _ => CountryCode.Unknown
        };

        // Note: UK doesn't use IBAN in this validator, so skip it
        if (countryCode == CountryCode.UnitedKingdom)
            return;

        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", countryCode);

        // Act
        var result = validator(iban);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("DE00370400440532013000")]
    [InlineData("FR0020041010050500013M02606")]
    public void GivenIbanChecksum_WhenInvalidChecksum_ThenShouldFail(string iban)
    {
        // These IBANs have invalid checksums (00)
        var countryCode = iban.Substring(0, 2) switch
        {
            "DE" => CountryCode.Germany,
            "FR" => CountryCode.France,
            _ => CountryCode.Unknown
        };

        // Arrange
        var validator = BankAccountNumberValidators.ValidateCountryFormat("TestField", countryCode);

        // Act
        var result = validator(iban);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Message.ShouldContain("checksum");
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNoFieldNameProvided_ThenShouldUseDefault()
    {
        // Arrange
        var validator = BankAccountNumberValidators.NotNullOrWhitespace();

        // Act
        var result = validator(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("BankAccountNumber");
    }
}
