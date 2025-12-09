using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class SwiftCodeTests
{
    #region Valid Creation Tests

    [Theory]
    [InlineData("DEUTDEFF")]     // Deutsche Bank, Germany (Frankfurt)
    [InlineData("DEUTDEFFXXX")]  // Deutsche Bank, Germany (Frankfurt) - explicit branch
    [InlineData("CHASUS33")]     // Chase Bank, USA
    [InlineData("HSBCHKHH")]     // HSBC, Hong Kong
    [InlineData("BNPAFRPP")]     // BNP Paribas, France (Paris)
    [InlineData("BARCGB22")]     // Barclays, UK
    [InlineData("CHASAU2X")]     // Chase Bank, Australia (Sydney)
    [InlineData("NATAAU3303M")]  // National Australia Bank (Melbourne branch)
    public void TryCreate_WithValidSwiftCode_ReturnsSuccess(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeTrue($"SWIFT code {swiftCode} should be valid");
        value.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("deutdeff")]
    [InlineData("DeutDeff")]
    [InlineData("DEUTDEFF")]
    public void TryCreate_WithVariousCasing_NormalizesToUppercase(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.ToNormalizedString().ShouldBe("DEUTDEFF");
    }

    #endregion

    #region Invalid Creation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("DEUT")]         // Too short
    [InlineData("DEUTDE")]       // Too short
    [InlineData("DEUTDEFFX")]    // 9 characters (invalid)
    [InlineData("DEUTDEFFXX")]   // 10 characters (invalid)
    [InlineData("DEUTDEFFXXXX")] // Too long
    public void TryCreate_WithInvalidLength_ReturnsFailure(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidLength");
    }

    [Theory]
    [InlineData("DEUT-DEFF")]
    [InlineData("DEUT DEFF")]
    [InlineData("DEUT@DEFF")]
    public void TryCreate_WithInvalidCharacters_ReturnsFailure(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidFormat");
    }

    [Theory]
    [InlineData("DEUT1EFF")]     // Digit in bank code
    [InlineData("DEUTD1FF")]     // Digit in country code
    [InlineData("1EUTDEFF")]     // Starts with digit
    public void TryCreate_WithInvalidStructure_ReturnsFailure(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidStructure");
    }

    [Theory]
    [InlineData("DEUTDE00")]     // Test code
    [InlineData("CHASUS30")]     // Test code (location X0)
    public void TryCreate_WithTestCode_WhenNotAllowed_ReturnsFailure(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode, allowTestCodes: false);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "TestCode");
    }

    [Theory]
    [InlineData("DEUTDE00")]
    [InlineData("CHASUS30")]
    public void TryCreate_WithTestCode_WhenAllowed_ReturnsSuccess(string swiftCode)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode, allowTestCodes: true);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
    }

    #endregion

    #region Component Accessor Tests

    [Fact]
    public void BankCode_ReturnsFirstFourCharacters()
    {
        var (_, swiftCode) = SwiftCode.TryCreate("DEUTDEFF");

        swiftCode.ShouldNotBeNull();
        swiftCode!.BankCode.ShouldBe("DEUT");
    }

    [Fact]
    public void CountryCode_ReturnsTwoLetterCountryCode()
    {
        var (_, swiftCode) = SwiftCode.TryCreate("DEUTDEFF");

        swiftCode.ShouldNotBeNull();
        swiftCode!.CountryCode.ShouldBe("DE");
    }

    [Fact]
    public void LocationCode_ReturnsTwoCharacterLocationCode()
    {
        var (_, swiftCode) = SwiftCode.TryCreate("DEUTDEFF");

        swiftCode.ShouldNotBeNull();
        swiftCode!.LocationCode.ShouldBe("FF");
    }

    [Theory]
    [InlineData("DEUTDEFF", "XXX")]       // 8-char defaults to XXX
    [InlineData("DEUTDEFFXXX", "XXX")]    // Explicit XXX
    [InlineData("NATAAU3303M", "03M")]    // Specific branch
    public void BranchCode_ReturnsCorrectBranchCode(string input, string expectedBranch)
    {
        var (_, swiftCode) = SwiftCode.TryCreate(input);

        swiftCode.ShouldNotBeNull();
        swiftCode!.BranchCode.ShouldBe(expectedBranch);
    }

    #endregion

    #region Formatting Tests

    [Theory]
    [InlineData("deutdeff", "DEUTDEFF")]
    [InlineData("DeutDeff", "DEUTDEFF")]
    [InlineData("DEUTDEFF", "DEUTDEFF")]
    public void ToNormalizedString_ReturnsUppercase(string input, string expected)
    {
        var (_, swiftCode) = SwiftCode.TryCreate(input);

        swiftCode.ShouldNotBeNull();
        swiftCode!.ToNormalizedString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("DEUTDEFF", "DEUTDEFFXXX")]     // 8-char expands to 11
    [InlineData("DEUTDEFFXXX", "DEUTDEFFXXX")]  // 11-char stays same
    [InlineData("NATAAU3303M", "NATAAU3303M")]  // 11-char with branch
    public void ToFullFormat_ReturnsElevenCharacters(string input, string expected)
    {
        var (_, swiftCode) = SwiftCode.TryCreate(input);

        swiftCode.ShouldNotBeNull();
        swiftCode!.ToFullFormat().ShouldBe(expected);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var (_, swiftCode) = SwiftCode.TryCreate("deutdeff");

        swiftCode.ShouldNotBeNull();
        swiftCode!.ToString().ShouldBe("DEUTDEFF");
    }

    #endregion

    #region Property Tests

    [Theory]
    [InlineData("DEUTDEFF", true)]       // 8-char is primary office
    [InlineData("DEUTDEFFXXX", true)]    // Explicit XXX is primary office
    [InlineData("NATAAU3303M", false)]   // Specific branch
    public void IsPrimaryOffice_ReturnsCorrectValue(string input, bool expectedIsPrimary)
    {
        var (_, swiftCode) = SwiftCode.TryCreate(input);

        swiftCode.ShouldNotBeNull();
        swiftCode!.IsPrimaryOffice.ShouldBe(expectedIsPrimary);
    }

    [Theory]
    [InlineData("DEUTDEFF", false)]      // Normal code
    [InlineData("DEUTDE00", true)]       // Test code (location 00)
    [InlineData("CHASUS30", true)]       // Test code (location 30 - second char is '0')
    [InlineData("DEUTDE2F", false)]      // Not a test code
    public void IsTestCode_ReturnsCorrectValue(string input, bool expectedIsTest)
    {
        var (_, swiftCode) = SwiftCode.TryCreate(input, allowTestCodes: true);

        swiftCode.ShouldNotBeNull();
        swiftCode!.IsTestCode.ShouldBe(expectedIsTest);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameSwiftCode_ReturnsTrue()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFF");
        var (_, swiftCode2) = SwiftCode.TryCreate("DEUTDEFF");

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.Equals(swiftCode2).ShouldBeTrue();
        swiftCode1.ShouldBe(swiftCode2);
    }

    [Fact]
    public void Equals_WithEightAndElevenCharFormat_ReturnsTrue()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFF");      // 8-char
        var (_, swiftCode2) = SwiftCode.TryCreate("DEUTDEFFXXX");   // 11-char

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.Equals(swiftCode2).ShouldBeTrue();
        swiftCode1.ShouldBe(swiftCode2);
    }

    [Fact]
    public void Equals_WithDifferentCasing_ReturnsTrue()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFF");
        var (_, swiftCode2) = SwiftCode.TryCreate("deutdeff");

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.Equals(swiftCode2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentSwiftCode_ReturnsFalse()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFF");
        var (_, swiftCode2) = SwiftCode.TryCreate("CHASUS33");

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.Equals(swiftCode2).ShouldBeFalse();
        swiftCode1.ShouldNotBe(swiftCode2);
    }

    [Fact]
    public void Equals_WithDifferentBranch_ReturnsFalse()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFFXXX");   // Primary office
        var (_, swiftCode2) = SwiftCode.TryCreate("DEUTDEFF123");   // Specific branch

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.Equals(swiftCode2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, swiftCode) = SwiftCode.TryCreate("DEUTDEFF");

        swiftCode.ShouldNotBeNull();
        swiftCode!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithEquivalentSwiftCodes_ReturnsSameHash()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFF");
        var (_, swiftCode2) = SwiftCode.TryCreate("DEUTDEFFXXX");

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.GetHashCode().ShouldBe(swiftCode2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentSwiftCodes_ReturnsDifferentHash()
    {
        var (_, swiftCode1) = SwiftCode.TryCreate("DEUTDEFF");
        var (_, swiftCode2) = SwiftCode.TryCreate("CHASUS33");

        swiftCode1.ShouldNotBeNull();
        swiftCode2.ShouldNotBeNull();
        swiftCode1!.GetHashCode().ShouldNotBe(swiftCode2!.GetHashCode());
    }

    #endregion

    #region Custom Property Name Tests

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, swiftCode) = SwiftCode.TryCreate("DEUT", propertyName: "BankSwiftCode");

        result.IsValid.ShouldBeFalse();
        swiftCode.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "BankSwiftCode");
    }

    #endregion

    #region Real-World SWIFT Code Tests

    [Theory]
    [InlineData("DEUTDEFF", "DEUT", "DE", "FF", "XXX")]         // Deutsche Bank, Germany (Frankfurt)
    [InlineData("CHASUS33", "CHAS", "US", "33", "XXX")]         // Chase Bank, USA
    [InlineData("HSBCHKHH", "HSBC", "HK", "HH", "XXX")]         // HSBC, Hong Kong
    [InlineData("BNPAFRPP", "BNPA", "FR", "PP", "XXX")]         // BNP Paribas, France (Paris)
    [InlineData("BARCGB22", "BARC", "GB", "22", "XXX")]         // Barclays, UK
    [InlineData("CHASAU2X", "CHAS", "AU", "2X", "XXX")]         // Chase Bank, Australia (Sydney)
    [InlineData("NATAAU3303M", "NATA", "AU", "33", "03M")]      // National Australia Bank (Melbourne branch)
    [InlineData("DEUTDEFFXXX", "DEUT", "DE", "FF", "XXX")]      // Deutsche Bank, explicit branch
    public void TryCreate_WithRealSwiftCodes_ParsesComponentsCorrectly(
        string swiftCode,
        string expectedBank,
        string expectedCountry,
        string expectedLocation,
        string expectedBranch)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeTrue($"SWIFT code {swiftCode} should be valid");
        value.ShouldNotBeNull();
        value!.BankCode.ShouldBe(expectedBank);
        value.CountryCode.ShouldBe(expectedCountry);
        value.LocationCode.ShouldBe(expectedLocation);
        value.BranchCode.ShouldBe(expectedBranch);
    }

    [Theory]
    [InlineData("DEUTDEFF", "Deutsche Bank (Germany, Frankfurt)")]
    [InlineData("CHASUS33", "Chase Bank (USA)")]
    [InlineData("HSBCHKHH", "HSBC (Hong Kong)")]
    [InlineData("BNPAFRPP", "BNP Paribas (France, Paris)")]
    [InlineData("BARCGB22", "Barclays (UK)")]
    [InlineData("CHASAU2X", "Chase Bank (Australia, Sydney)")]
    [InlineData("NATAAU3303M", "National Australia Bank (Melbourne branch)")]
    public void TryCreate_WithKnownBanks_ReturnsSuccess(string swiftCode, string description)
    {
        var (result, value) = SwiftCode.TryCreate(swiftCode);

        result.IsValid.ShouldBeTrue($"{description} SWIFT code should be valid");
        value.ShouldNotBeNull();
        value!.ToNormalizedString().ShouldBe(swiftCode.ToUpperInvariant());
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void TryCreate_WithLeadingAndTrailingSpaces_TrimsSpaces()
    {
        var (result, swiftCode) = SwiftCode.TryCreate("  DEUTDEFF  ");

        result.IsValid.ShouldBeTrue();
        swiftCode.ShouldNotBeNull();
        swiftCode!.ToNormalizedString().ShouldBe("DEUTDEFF");
    }

    [Fact]
    public void TryCreate_WithMultipleErrors_ReturnsAllErrors()
    {
        var (result, swiftCode) = SwiftCode.TryCreate("DE");

        result.IsValid.ShouldBeFalse();
        swiftCode.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion
}
