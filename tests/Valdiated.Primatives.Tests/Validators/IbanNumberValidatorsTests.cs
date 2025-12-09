using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class IbanNumberValidatorsTests
{
    #region NotNullOrWhitespace Tests

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNullValue_ThenShouldFail()
    {
        // Arrange
        var validator = IbanNumberValidators.NotNullOrWhitespace("TestField");

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
        var validator = IbanNumberValidators.NotNullOrWhitespace();

        // Act
        var result = validator(string.Empty);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("Required");
    }

    #endregion

    #region ValidFormat Tests

    [Theory]
    [InlineData("DE89370400440532013000")]
    [InlineData("GB82WEST12345698765432")]
    [InlineData("12345678")]
    [InlineData("DE89 3704 0044 0532 0130 00")]
    public void GivenValidFormat_WhenValidFormats_ThenShouldPass(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DE89@3704")]
    [InlineData("GB82-WEST#123")]
    public void GivenValidFormat_WhenInvalidCharacters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidFormat");
    }

    #endregion

    #region DetectAccountType Tests

    [Theory]
    [InlineData("DE89370400440532013000", BankAccountNumberType.Iban)]
    [InlineData("GB82WEST12345698765432", BankAccountNumberType.Iban)]
    [InlineData("FR1420041010050500013M02606", BankAccountNumberType.Iban)]
    [InlineData("NL91ABNA0417164300", BankAccountNumberType.Iban)]
    public void GivenDetectAccountType_WhenIban_ThenShouldReturnIban(string value, BankAccountNumberType expected)
    {
        // Act
        var result = IbanNumberValidators.DetectAccountType(value);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("12345678", BankAccountNumberType.Bban)]
    [InlineData("123456789012", BankAccountNumberType.Bban)]
    [InlineData("00123456", BankAccountNumberType.Bban)]
    public void GivenDetectAccountType_WhenBban_ThenShouldReturnBban(string value, BankAccountNumberType expected)
    {
        // Act
        var result = IbanNumberValidators.DetectAccountType(value);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenDetectAccountType_WhenNullOrEmpty_ThenShouldReturnUnknown(string? value)
    {
        // Act
        var result = IbanNumberValidators.DetectAccountType(value);

        // Assert
        result.ShouldBe(BankAccountNumberType.Unknown);
    }

    #endregion

    #region ValidIbanFormat Tests

    [Theory]
    [InlineData("DE89370400440532013000")]     // Germany
    [InlineData("GB82WEST12345698765432")]      // UK
    [InlineData("FR1420041010050500013M02606")] // France
    [InlineData("NL91ABNA0417164300")]          // Netherlands
    [InlineData("IT60X0542811101000000123456")] // Italy
    [InlineData("ES9121000418450200051332")]    // Spain
    public void GivenValidIbanFormat_WhenValidIbans_ThenShouldPass(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("1234567890")]          // Starts with digit
    [InlineData("DEAA370400440532013000")] // Non-digit check digits
    public void GivenValidIbanFormat_WhenInvalidStructure_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidIbanStructure");
    }

    [Theory]
    [InlineData("XX89370400440532013000")] // Invalid country code
    [InlineData("ZZ82WEST12345698765432")]
    public void GivenValidIbanFormat_WhenInvalidCountryCode_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidIbanCountryCode");
    }

    [Theory]
    [InlineData("DE893704004405320130")]        // Too short for Germany
    [InlineData("DE8937040044053201300000")]    // Too long for Germany
    [InlineData("GB82WEST123456987654")]        // Too short for UK
    public void GivenValidIbanFormat_WhenInvalidLength_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidIbanLength");
    }

    #endregion

    #region ValidIbanChecksum Tests

    [Theory]
    [InlineData("DE89370400440532013000")]      // Valid checksum
    [InlineData("GB82WEST12345698765432")]      // Valid checksum
    [InlineData("FR1420041010050500013M02606")] // Valid checksum
    [InlineData("NL91ABNA0417164300")]          // Valid checksum
    [InlineData("IT60X0542811101000000123456")] // Valid checksum
    public void GivenValidIbanChecksum_WhenValidChecksums_ThenShouldPass(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanChecksum("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DE00370400440532013000")]      // Invalid checksum (00)
    [InlineData("GB00WEST12345698765432")]      // Invalid checksum (00)
    [InlineData("FR0020041010050500013M02606")] // Invalid checksum (00)
    public void GivenValidIbanChecksum_WhenInvalidChecksums_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanChecksum("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidIbanChecksum");
    }

    #endregion

    #region ValidBbanFormat Tests

    [Theory]
    [InlineData("12345678", null)]
    [InlineData("123456789012", null)]
    [InlineData("ABCD1234EFGH5678", null)]
    public void GivenValidBbanFormat_WhenValidBbans_ThenShouldPass(string value, CountryCode? countryCode)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidBbanFormat("TestField", countryCode);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("123")]                      // Too short
    [InlineData("12345678901234567890123456789012345")] // Too long (>34)
    public void GivenValidBbanFormat_WhenInvalidLength_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidBbanFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidBbanLength");
    }

    [Fact]
    public void GivenValidBbanFormat_WhenUkValid_ThenShouldPass()
    {
        // Arrange
        var validator = IbanNumberValidators.ValidBbanFormat("TestField", CountryCode.UnitedKingdom);

        // Act
        var result = validator("12345678");

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenValidBbanFormat_WhenUkInvalid_ThenShouldFail()
    {
        // Arrange
        var validator = IbanNumberValidators.ValidBbanFormat("TestField", CountryCode.UnitedKingdom);

        // Act
        var result = validator("1234567"); // Only 7 digits

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidBbanFormat");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456789")]
    [InlineData("12345678901234567")]
    public void GivenValidBbanFormat_WhenUsValid_ThenShouldPass(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidBbanFormat("TestField", CountryCode.UnitedStates);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("123")]          // Too short
    [InlineData("123456789012345678")] // Too long
    public void GivenValidBbanFormat_WhenUsInvalid_ThenShouldFail(string value)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidBbanFormat("TestField", CountryCode.UnitedStates);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    #endregion

    #region Integration Tests

    [Theory]
    [InlineData("DE89370400440532013000")]
    [InlineData("GB82WEST12345698765432")]
    [InlineData("FR1420041010050500013M02606")]
    public void GivenAllValidators_WhenValidIban_ThenAllShouldPass(string value)
    {
        // Arrange
        var validators = new[]
        {
            IbanNumberValidators.NotNullOrWhitespace("TestField"),
            IbanNumberValidators.ValidFormat("TestField"),
            IbanNumberValidators.ValidIbanFormat("TestField"),
            IbanNumberValidators.ValidIbanChecksum("TestField")
        };

        // Act & Assert
        foreach (var validator in validators)
        {
            var result = validator(value);
            result.IsValid.ShouldBeTrue($"Validator failed for IBAN {value}");
        }
    }

    [Theory]
    [InlineData("12345678", CountryCode.UnitedKingdom)]
    [InlineData("123456789", CountryCode.UnitedStates)]
    public void GivenAllValidators_WhenValidBban_ThenAllShouldPass(string value, CountryCode countryCode)
    {
        // Arrange
        var validators = new[]
        {
            IbanNumberValidators.NotNullOrWhitespace("TestField"),
            IbanNumberValidators.ValidFormat("TestField"),
            IbanNumberValidators.ValidBbanFormat("TestField", countryCode)
        };

        // Act & Assert
        foreach (var validator in validators)
        {
            var result = validator(value);
            result.IsValid.ShouldBeTrue($"Validator failed for BBAN {value}");
        }
    }

    #endregion

    #region Country-Specific IBAN Length Tests

    [Theory]
    [InlineData("AD", 24)]  // Andorra
    [InlineData("AE", 23)]  // UAE
    [InlineData("AT", 20)]  // Austria
    [InlineData("BE", 16)]  // Belgium
    [InlineData("CH", 21)]  // Switzerland
    [InlineData("DE", 22)]  // Germany
    [InlineData("DK", 18)]  // Denmark
    [InlineData("ES", 24)]  // Spain
    [InlineData("FI", 18)]  // Finland
    [InlineData("FR", 27)]  // France
    [InlineData("GB", 22)]  // UK
    [InlineData("IE", 22)]  // Ireland
    [InlineData("IT", 27)]  // Italy
    [InlineData("NL", 18)]  // Netherlands
    [InlineData("NO", 15)]  // Norway
    [InlineData("PL", 28)]  // Poland
    [InlineData("PT", 25)]  // Portugal
    [InlineData("SE", 24)]  // Sweden
    public void GivenValidIbanFormat_WhenCorrectLengthForCountry_ThenShouldPass(string countryCode, int expectedLength)
    {
        // Arrange
        var validator = IbanNumberValidators.ValidIbanFormat("TestField");
        var iban = countryCode + "00" + new string('0', expectedLength - 4); // Dummy IBAN with correct length

        // Act
        var result = validator(iban);

        // Assert - should pass length check (checksum will fail but that's a different validator)
        result.IsValid.ShouldBeTrue($"IBAN length validation failed for {countryCode} with length {expectedLength}");
    }

    #endregion
}
