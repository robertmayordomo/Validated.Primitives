using Shouldly;
using Validated.Primitives.Validators;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class SwiftCodeValidatorsTests
{
    #region NotNullOrWhitespace Tests

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNullValue_ThenShouldFail()
    {
        // Arrange
        var validator = SwiftCodeValidators.NotNullOrWhitespace("TestField");

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
        var validator = SwiftCodeValidators.NotNullOrWhitespace("TestField");

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
        var validator = SwiftCodeValidators.NotNullOrWhitespace();

        // Act
        var result = validator(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("SwiftCode");
    }

    #endregion

    #region ValidFormat Tests

    [Theory]
    [InlineData("DEUTDEFF")]
    [InlineData("DEUTDEFFXXX")]
    [InlineData("CHASUS33")]
    [InlineData("HSBCHKHH")]
    [InlineData("deutdeff")]  // lowercase should pass format check
    public void GivenValidFormat_WhenValidFormats_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DEUT-DEFF")]
    [InlineData("DEUT DEFF")]
    [InlineData("DEUT@DEFF")]
    public void GivenValidFormat_WhenInvalidCharacters_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidFormat("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidFormat");
    }

    #endregion

    #region ValidLength Tests

    [Theory]
    [InlineData("DEUTDEFF")]     // 8 characters
    [InlineData("DEUTDEFFXXX")]  // 11 characters
    public void GivenValidLength_WhenValidLengths_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidLength("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DEUT")]         // Too short
    [InlineData("DEUTDE")]       // Too short
    [InlineData("DEUTDEFFX")]    // 9 characters (invalid)
    [InlineData("DEUTDEFFXX")]   // 10 characters (invalid)
    [InlineData("DEUTDEFFXXXX")] // Too long
    public void GivenValidLength_WhenInvalidLengths_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidLength("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidLength");
    }

    #endregion

    #region ValidStructure Tests

    [Theory]
    [InlineData("DEUTDEFF")]     // Valid 8-char structure
    [InlineData("DEUTDEFFXXX")]  // Valid 11-char structure
    [InlineData("CHASUS33")]     // Valid with digits in location
    [InlineData("HSBCHK2H")]     // Valid with digit in location
    [InlineData("HSBCHKHH123")]  // Valid with digits in branch
    public void GivenValidStructure_WhenValidStructures_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidStructure("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DEUT1EFF")]     // Digit in bank code (should be letters only)
    [InlineData("DEUTD1FF")]     // Digit in country code (should be letters only)
    [InlineData("1EUTDEFF")]     // Starts with digit
    public void GivenValidStructure_WhenInvalidStructures_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidStructure("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidStructure");
    }

    #endregion

    #region ValidCountryCode Tests

    [Theory]
    [InlineData("DEUTDEFF")]     // DE - Germany
    [InlineData("CHASUS33")]     // US - United States
    [InlineData("HSBCHKHH")]     // HK - Hong Kong
    [InlineData("BNPAFRPP")]     // FR - France
    [InlineData("BARCGB22")]     // GB - United Kingdom
    public void GivenValidCountryCode_WhenValidCountryCodes_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidCountryCode("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DEUT1EFF")]     // Country code contains digit
    [InlineData("DEUTD1FF")]     // Country code contains digit
    public void GivenValidCountryCode_WhenInvalidCountryCodes_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.ValidCountryCode("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("InvalidCountryCode");
    }

    #endregion

    #region NotTestCode Tests

    [Theory]
    [InlineData("DEUTDEFF")]     // Normal code
    [InlineData("CHASUS33")]     // Normal code
    [InlineData("HSBCHKHH")]     // Normal code
    [InlineData("DEUTDE2F")]     // Location starts with 2, not 0
    public void GivenNotTestCode_WhenProductionCodes_ThenShouldPass(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.NotTestCode("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("DEUTDE00")]     // Test code (location 00 - second char '0')
    [InlineData("DEUTDE10")]     // Test code (location 10 - second char '0')
    [InlineData("CHASUS30")]     // Test code (location 30 - second char '0')
    public void GivenNotTestCode_WhenTestCodes_ThenShouldFail(string value)
    {
        // Arrange
        var validator = SwiftCodeValidators.NotTestCode("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("TestCode");
    }

    #endregion

    #region Integration Tests

    [Theory]
    [InlineData("DEUTDEFF")]
    [InlineData("DEUTDEFFXXX")]
    [InlineData("CHASUS33")]
    [InlineData("HSBCHKHH")]
    public void GivenAllValidators_WhenValidSwiftCode_ThenAllShouldPass(string value)
    {
        // Arrange
        var validators = new[]
        {
            SwiftCodeValidators.NotNullOrWhitespace("TestField"),
            SwiftCodeValidators.ValidFormat("TestField"),
            SwiftCodeValidators.ValidLength("TestField"),
            SwiftCodeValidators.ValidStructure("TestField"),
            SwiftCodeValidators.ValidCountryCode("TestField"),
            SwiftCodeValidators.NotTestCode("TestField")
        };

        // Act & Assert
        foreach (var validator in validators)
        {
            var result = validator(value);
            result.IsValid.ShouldBeTrue($"Validator failed for {value}");
        }
    }

    [Fact]
    public void GivenAllValidators_WhenInvalidSwiftCode_ThenShouldFail()
    {
        // Arrange
        var lengthValidator = SwiftCodeValidators.ValidLength("TestField");
        var structureValidator = SwiftCodeValidators.ValidStructure("TestField");
        var testCodeValidator = SwiftCodeValidators.NotTestCode("TestField");

        // Act
        var lengthResult = lengthValidator("DEUT");
        var structureResult = structureValidator("DEUT1EFF");
        var testCodeResult = testCodeValidator("DEUTDE00");

        // Assert
        lengthResult.IsValid.ShouldBeFalse();
        structureResult.IsValid.ShouldBeFalse();
        testCodeResult.IsValid.ShouldBeFalse();
    }

    #endregion

    #region Real SWIFT Code Tests

    [Theory]
    [InlineData("DEUTDEFF")]     // Deutsche Bank, Germany (Frankfurt)
    [InlineData("DEUTDEFFXXX")]  // Deutsche Bank, Germany (Frankfurt) - explicit branch
    [InlineData("CHASUS33")]     // Chase Bank, USA
    [InlineData("HSBCHKHH")]     // HSBC, Hong Kong
    [InlineData("BNPAFRPP")]     // BNP Paribas, France (Paris)
    [InlineData("BARCGB22")]     // Barclays, UK
    [InlineData("CHASAU2X")]     // Chase Bank, Australia (Sydney)
    [InlineData("NATAAU3303M")]  // National Australia Bank (Melbourne branch)
    public void GivenRealSwiftCodes_WhenValidated_ThenShouldPass(string value)
    {
        // Arrange
        var validators = new[]
        {
            SwiftCodeValidators.ValidFormat("TestField"),
            SwiftCodeValidators.ValidLength("TestField"),
            SwiftCodeValidators.ValidStructure("TestField"),
            SwiftCodeValidators.ValidCountryCode("TestField")
        };

        // Act & Assert
        foreach (var validator in validators)
        {
            var result = validator(value);
            result.IsValid.ShouldBeTrue($"Validator failed for {value}");
        }
    }

    #endregion
}
