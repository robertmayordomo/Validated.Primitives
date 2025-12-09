using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class IbanNumberTests
{
    #region IBAN Creation Tests

    [Theory]
    [InlineData("DE89370400440532013000")]      // Germany
    [InlineData("GB82WEST12345698765432")]      // UK
    [InlineData("FR1420041010050500013M02606")] // France
    [InlineData("NL91ABNA0417164300")]          // Netherlands
    [InlineData("IT60X0542811101000000123456")] // Italy
    [InlineData("ES9121000418450200051332")]    // Spain
    [InlineData("CH9300762011623852957")]       // Switzerland
    [InlineData("AT611904300234573201")]        // Austria
    [InlineData("BE68539007547034")]            // Belgium
    [InlineData("NO9386011117947")]             // Norway
    public void TryCreate_WithValidIban_ReturnsSuccess(string iban)
    {
        var (result, value) = IbanNumber.TryCreate(iban);

        result.IsValid.ShouldBeTrue($"IBAN {iban} should be valid");
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Iban);
        value.IsIban.ShouldBeTrue();
        value.IsBban.ShouldBeFalse();
    }

    [Theory]
    [InlineData("DE89 3704 0044 0532 0130 00")]
    [InlineData("GB82 WEST 1234 5698 7654 32")]
    [InlineData("FR14 2004 1010 0505 0001 3M02 606")]
    public void TryCreate_WithFormattedIban_ReturnsSuccess(string iban)
    {
        var (result, value) = IbanNumber.TryCreate(iban);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Iban);
    }

    [Fact]
    public void TryCreate_WithIban_AutoDetectsType()
    {
        var (result, value) = IbanNumber.TryCreate("DE89370400440532013000");

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Iban);
        value.IsIban.ShouldBeTrue();
    }

    #endregion

    #region BBAN Creation Tests

    [Theory]
    [InlineData("12345678", CountryCode.UnitedKingdom)]
    [InlineData("123456789", CountryCode.UnitedStates)]
    [InlineData("1234567890", CountryCode.UnitedStates)]
    public void TryCreate_WithValidBban_ReturnsSuccess(string bban, CountryCode countryCode)
    {
        var (result, value) = IbanNumber.TryCreate(bban, countryCode);

        result.IsValid.ShouldBeTrue($"BBAN {bban} should be valid for {countryCode}");
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Bban);
        value.IsBban.ShouldBeTrue();
        value.IsIban.ShouldBeFalse();
        value.CountryCode.ShouldBe(countryCode);
    }

    [Fact]
    public void TryCreate_WithBban_AutoDetectsType()
    {
        var (result, value) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Bban);
        value.IsBban.ShouldBeTrue();
    }

    #endregion

    #region Invalid Creation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? value)
    {
        var (result, ibanNumber) = IbanNumber.TryCreate(value);

        result.IsValid.ShouldBeFalse();
        ibanNumber.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("DE00370400440532013000")]      // Invalid checksum
    [InlineData("GB00WEST12345698765432")]      // Invalid checksum
    [InlineData("FR0020041010050500013M02606")] // Invalid checksum
    public void TryCreate_WithInvalidIbanChecksum_ReturnsFailure(string iban)
    {
        var (result, value) = IbanNumber.TryCreate(iban);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidIbanChecksum");
    }

    [Theory]
    [InlineData("XX89370400440532013000")] // Looks like IBAN but invalid country code
    [InlineData("ZZ82WEST12345698765432")]   // Looks like IBAN but invalid country code
    public void TryCreate_WithInvalidCountryCode_DetectsAsUnknown(string value)
    {
        var (result, ibanValue) = IbanNumber.TryCreate(value);

        // These match IBAN pattern (2 letters + 2 digits) but country code isn't recognized
        // They're detected as Unknown type and pass basic validation
        result.IsValid.ShouldBeTrue();
        ibanValue.ShouldNotBeNull();
        ibanValue!.AccountType.ShouldBe(BankAccountNumberType.Unknown);
    }

    [Theory]
    [InlineData("DE893704004405320130")]        // Too short
    [InlineData("DE8937040044053201300000")]    // Too long
    public void TryCreate_WithInvalidIbanLength_ReturnsFailure(string iban)
    {
        var (result, value) = IbanNumber.TryCreate(iban);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidIbanLength");
    }

    [Theory]
    [InlineData("1234567", CountryCode.UnitedKingdom)]  // UK needs 8 digits
    [InlineData("123", CountryCode.UnitedStates)]        // US needs 4-17 digits
    public void TryCreate_WithInvalidBbanFormat_ReturnsFailure(string bban, CountryCode countryCode)
    {
        var (result, value) = IbanNumber.TryCreate(bban, countryCode);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
    }

    #endregion

    #region IBAN Component Tests

    [Fact]
    public void GetIbanCountryCode_ForIban_ReturnsCountryCode()
    {
        var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

        iban.ShouldNotBeNull();
        iban!.GetIbanCountryCode().ShouldBe("DE");
    }

    [Fact]
    public void GetIbanCountryCode_ForBban_ReturnsNull()
    {
        var (_, bban) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

        bban.ShouldNotBeNull();
        bban!.GetIbanCountryCode().ShouldBeNull();
    }

    [Fact]
    public void GetIbanCheckDigits_ForIban_ReturnsCheckDigits()
    {
        var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

        iban.ShouldNotBeNull();
        iban!.GetIbanCheckDigits().ShouldBe("89");
    }

    [Fact]
    public void GetIbanCheckDigits_ForBban_ReturnsNull()
    {
        var (_, bban) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

        bban.ShouldNotBeNull();
        bban!.GetIbanCheckDigits().ShouldBeNull();
    }

    [Fact]
    public void GetBbanPart_ForIban_ReturnsBbanPart()
    {
        var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

        iban.ShouldNotBeNull();
        iban!.GetBbanPart().ShouldBe("370400440532013000");
    }

    [Fact]
    public void GetBbanPart_ForBban_ReturnsEntireValue()
    {
        var (_, bban) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

        bban.ShouldNotBeNull();
        bban!.GetBbanPart().ShouldBe("12345678");
    }

    [Theory]
    [InlineData("DE89370400440532013000", CountryCode.Germany)]
    [InlineData("FR1420041010050500013M02606", CountryCode.France)]
    [InlineData("GB82WEST12345698765432", CountryCode.UnitedKingdom)]
    [InlineData("IT60X0542811101000000123456", CountryCode.Italy)]
    public void CountryCode_ForIban_ReturnsCorrectCountry(string iban, CountryCode expectedCountry)
    {
        var (_, value) = IbanNumber.TryCreate(iban);

        value.ShouldNotBeNull();
        value!.CountryCode.ShouldBe(expectedCountry);
    }

    #endregion

    #region Formatting Tests

    [Theory]
    [InlineData("de89370400440532013000", "DE89370400440532013000")]
    [InlineData("gb82west12345698765432", "GB82WEST12345698765432")]
    [InlineData("DE89 3704 0044 0532 0130 00", "DE89370400440532013000")]
    public void ToNormalizedString_ReturnsUppercaseWithoutSpaces(string input, string expected)
    {
        var (_, iban) = IbanNumber.TryCreate(input);

        iban.ShouldNotBeNull();
        iban!.ToNormalizedString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("DE89370400440532013000", "DE89 3704 0044 0532 0130 00")]
    [InlineData("GB82WEST12345698765432", "GB82 WEST 1234 5698 7654 32")]
    [InlineData("NO9386011117947", "NO93 8601 1117 947")]
    public void ToFormattedString_ForIban_ReturnsGroupedFormat(string input, string expected)
    {
        var (_, iban) = IbanNumber.TryCreate(input);

        iban.ShouldNotBeNull();
        iban!.ToFormattedString().ShouldBe(expected);
    }

    [Fact]
    public void ToFormattedString_ForBban_ReturnsNormalized()
    {
        var (_, bban) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

        bban.ShouldNotBeNull();
        bban!.ToFormattedString().ShouldBe("12345678");
    }

    [Fact]
    public void ToString_ReturnsNormalizedString()
    {
        var (_, iban) = IbanNumber.TryCreate("DE89 3704 0044 0532 0130 00");

        iban.ShouldNotBeNull();
        iban!.ToString().ShouldBe("DE89370400440532013000");
    }

    #endregion

    #region Masking Tests

    [Fact]
    public void Masked_ShowsOnlyLastFourCharacters()
    {
        var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

        iban.ShouldNotBeNull();
        iban!.Masked().ShouldBe("******************3000");
    }

    [Fact]
    public void Masked_ForShortNumber_ShowsAllAsterisks()
    {
        var (_, bban) = IbanNumber.TryCreate("1234", CountryCode.UnitedStates);

        bban.ShouldNotBeNull();
        bban!.Masked().ShouldBe("****");
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameIban_ReturnsTrue()
    {
        var (_, iban1) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, iban2) = IbanNumber.TryCreate("DE89370400440532013000");

        iban1.ShouldNotBeNull();
        iban2.ShouldNotBeNull();
        iban1!.Equals(iban2).ShouldBeTrue();
        iban1.ShouldBe(iban2);
    }

    [Fact]
    public void Equals_WithDifferentFormatting_ReturnsTrue()
    {
        var (_, iban1) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, iban2) = IbanNumber.TryCreate("DE89 3704 0044 0532 0130 00");

        iban1.ShouldNotBeNull();
        iban2.ShouldNotBeNull();
        iban1!.Equals(iban2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCasing_ReturnsTrue()
    {
        var (_, iban1) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, iban2) = IbanNumber.TryCreate("de89370400440532013000");

        iban1.ShouldNotBeNull();
        iban2.ShouldNotBeNull();
        iban1!.Equals(iban2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentIban_ReturnsFalse()
    {
        var (_, iban1) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, iban2) = IbanNumber.TryCreate("GB82WEST12345698765432");

        iban1.ShouldNotBeNull();
        iban2.ShouldNotBeNull();
        iban1!.Equals(iban2).ShouldBeFalse();
        iban1.ShouldNotBe(iban2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

        iban.ShouldNotBeNull();
        iban!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameIban_ReturnsSameHash()
    {
        var (_, iban1) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, iban2) = IbanNumber.TryCreate("DE89 3704 0044 0532 0130 00");

        iban1.ShouldNotBeNull();
        iban2.ShouldNotBeNull();
        iban1!.GetHashCode().ShouldBe(iban2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentIban_ReturnsDifferentHash()
    {
        var (_, iban1) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, iban2) = IbanNumber.TryCreate("GB82WEST12345698765432");

        iban1.ShouldNotBeNull();
        iban2.ShouldNotBeNull();
        iban1!.GetHashCode().ShouldNotBe(iban2!.GetHashCode());
    }

    #endregion

    #region Real-World IBAN Tests

    [Theory]
    [InlineData("AT611904300234573201", "AT", "61", "1904300234573201")]           // Austria
    [InlineData("BE68539007547034", "BE", "68", "539007547034")]                   // Belgium
    [InlineData("CH9300762011623852957", "CH", "93", "00762011623852957")]         // Switzerland
    [InlineData("DE89370400440532013000", "DE", "89", "370400440532013000")]       // Germany
    [InlineData("DK5000400440116243", "DK", "50", "00400440116243")]               // Denmark
    [InlineData("ES9121000418450200051332", "ES", "91", "21000418450200051332")]   // Spain
    [InlineData("FI2112345600000785", "FI", "21", "12345600000785")]               // Finland
    [InlineData("FR1420041010050500013M02606", "FR", "14", "20041010050500013M02606")] // France
    [InlineData("GB82WEST12345698765432", "GB", "82", "WEST12345698765432")]       // UK
    [InlineData("IE29AIBK93115212345678", "IE", "29", "AIBK93115212345678")]       // Ireland
    [InlineData("IT60X0542811101000000123456", "IT", "60", "X0542811101000000123456")] // Italy
    [InlineData("NL91ABNA0417164300", "NL", "91", "ABNA0417164300")]               // Netherlands
    [InlineData("NO9386011117947", "NO", "93", "86011117947")]                     // Norway
    [InlineData("PL61109010140000071219812874", "PL", "61", "109010140000071219812874")] // Poland
    [InlineData("PT50000201231234567890154", "PT", "50", "000201231234567890154")] // Portugal
    [InlineData("SE4550000000058398257466", "SE", "45", "50000000058398257466")]   // Sweden
    public void TryCreate_WithRealIbans_ParsesComponentsCorrectly(
        string iban,
        string expectedCountryCode,
        string expectedCheckDigits,
        string expectedBban)
    {
        var (result, value) = IbanNumber.TryCreate(iban);

        result.IsValid.ShouldBeTrue($"IBAN {iban} should be valid");
        value.ShouldNotBeNull();
        value!.GetIbanCountryCode().ShouldBe(expectedCountryCode);
        value.GetIbanCheckDigits().ShouldBe(expectedCheckDigits);
        value.GetBbanPart().ShouldBe(expectedBban);
    }

    #endregion

    #region Auto-Detection Edge Cases

    [Fact]
    public void TryCreate_WithAmbiguousFormat_DetectsAsUnknown()
    {
        // Numbers starting with letters but not valid IBAN country codes
        var (result, value) = IbanNumber.TryCreate("AB1234567890");

        // Detected as Unknown type - passes basic validation
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Unknown);
    }

    [Fact]
    public void TryCreate_WithAllDigits_DetectsAsBban()
    {
        var (result, value) = IbanNumber.TryCreate("123456789012", CountryCode.UnitedStates);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Bban);
    }

    #endregion

    #region Custom Property Name Tests

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, value) = IbanNumber.TryCreate("DE00", propertyName: "BankAccount");

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "BankAccount");
    }

    #endregion

    #region Mixed IBAN/BBAN Scenarios

    [Fact]
    public void TryCreate_IbanWithoutCountryCode_AutoDetects()
    {
        var (result, value) = IbanNumber.TryCreate("DE89370400440532013000");

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Iban);
        value.CountryCode.ShouldBe(CountryCode.Germany);
    }

    [Fact]
    public void TryCreate_BbanWithoutCountryCode_AcceptsButRequiresForValidation()
    {
        // BBAN without country code - may pass basic validation but won't have country-specific validation
        var (result, value) = IbanNumber.TryCreate("12345678");

        result.IsValid.ShouldBeTrue(); // Should pass without country-specific validation
        value.ShouldNotBeNull();
        value!.AccountType.ShouldBe(BankAccountNumberType.Bban);
        value.CountryCode.ShouldBeNull();
    }

    #endregion
}
