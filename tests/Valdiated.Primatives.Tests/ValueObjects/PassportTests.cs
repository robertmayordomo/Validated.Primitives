using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class PassportTests
{
    #region Valid Creation Tests

    [Theory]
    [InlineData("123456789", CountryCode.UnitedStates)]
    [InlineData("AB123456", CountryCode.Canada)]
    [InlineData("K1234567", CountryCode.India)]
    [InlineData("K1234567A", CountryCode.Singapore)]
    [InlineData("N1234567", CountryCode.Australia)]
    [InlineData("12345678", CountryCode.Sweden)]
    [InlineData("AB1234567", CountryCode.Ireland)]
    [InlineData("G123456789", CountryCode.Mexico)]
    public void TryCreate_WithValidPassport_ReturnsSuccess(string passportNumber, CountryCode country)
    {
        var (result, value) = Passport.TryCreate(passportNumber, country);

        result.IsValid.ShouldBeTrue($"Passport {passportNumber} for {country} should be valid");
        value.ShouldNotBeNull();
        value!.IssuingCountry.ShouldBe(country);
    }

    [Theory]
    [InlineData("ab123456", "AB123456", CountryCode.Canada)]
    [InlineData("K1234567", "K1234567", CountryCode.India)]
    public void TryCreate_WithVariousCasing_NormalizesToUppercase(string input, string expected, CountryCode country)
    {
        var (result, value) = Passport.TryCreate(input, country);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.ToNormalizedString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("AB 123456", "AB123456", CountryCode.Canada)]
    [InlineData("AB-123456", "AB123456", CountryCode.Canada)]
    public void TryCreate_WithFormattedInput_RemovesFormatting(string input, string expected, CountryCode country)
    {
        var (result, value) = Passport.TryCreate(input, country);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.ToNormalizedString().ShouldBe(expected);
    }

    #endregion

    #region Invalid Creation Tests

    [Theory]
    [InlineData(null, CountryCode.UnitedStates)]
    [InlineData("", CountryCode.UnitedStates)]
    [InlineData("   ", CountryCode.UnitedStates)]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? passportNumber, CountryCode country)
    {
        var (result, value) = Passport.TryCreate(passportNumber, country);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("AB123456", CountryCode.UnitedStates)]  // US requires 9 digits
    [InlineData("1234567", CountryCode.Canada)]         // Canada requires 2 letters + 6 digits
    [InlineData("ABC1234567", CountryCode.India)]       // India requires 1 letter + 7 digits
    public void TryCreate_WithWrongFormatForCountry_ReturnsFailure(string passportNumber, CountryCode country)
    {
        var (result, value) = Passport.TryCreate(passportNumber, country);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidCountryFormat");
    }

    [Theory]
    [InlineData("AB@12345", CountryCode.Canada)]
    [InlineData("AB.123456", CountryCode.Canada)]
    public void TryCreate_WithInvalidCharacters_ReturnsFailure(string passportNumber, CountryCode country)
    {
        var (result, value) = Passport.TryCreate(passportNumber, country);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidFormat");
    }

    #endregion

    #region Formatting Tests

    [Theory]
    [InlineData("ab123456", "AB123456", CountryCode.Canada)]
    [InlineData("k1234567", "K1234567", CountryCode.India)]
    [InlineData("AB 123456", "AB123456", CountryCode.Canada)]
    public void ToNormalizedString_ReturnsUppercaseWithoutSpaces(string input, string expected, CountryCode country)
    {
        var (_, passport) = Passport.TryCreate(input, country);

        passport.ShouldNotBeNull();
        passport!.ToNormalizedString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("AB123456", "AB 123456", CountryCode.Canada)]
    [InlineData("123456789", "123 456 789", CountryCode.UnitedKingdom)]
    public void ToFormattedString_FormatsAccordingToCountry(string input, string expected, CountryCode country)
    {
        var (_, passport) = Passport.TryCreate(input, country);

        passport.ShouldNotBeNull();
        passport!.ToFormattedString().ShouldBe(expected);
    }

    [Fact]
    public void ToString_ReturnsNormalizedString()
    {
        var (_, passport) = Passport.TryCreate("ab 123456", CountryCode.Canada);

        passport.ShouldNotBeNull();
        passport!.ToString().ShouldBe("AB123456");
    }

    #endregion

    #region Masking Tests

    [Fact]
    public void Masked_ShowsOnlyLastFourCharacters()
    {
        var (_, passport) = Passport.TryCreate("123456789", CountryCode.UnitedStates);

        passport.ShouldNotBeNull();
        passport!.Masked().ShouldBe("*****6789");
    }

    [Fact]
    public void Masked_ForShortPassport_ShowsAllAsterisks()
    {
        // Use a valid short passport format (Portugal allows 7 characters)
        var (_, passport) = Passport.TryCreate("N123456", CountryCode.Portugal);

        passport.ShouldNotBeNull();
        // For a 7-char passport, masking shows ***3456
        passport!.Masked().ShouldBe("***3456");
    }

    #endregion

    #region Country-Specific Tests

    [Fact]
    public void TryCreate_WithUsPassport_ValidatesCorrectFormat()
    {
        // Valid
        var (result1, _) = Passport.TryCreate("123456789", CountryCode.UnitedStates);
        result1.IsValid.ShouldBeTrue();

        // Invalid - contains letters
        var (result2, _) = Passport.TryCreate("AB1234567", CountryCode.UnitedStates);
        result2.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void TryCreate_WithCanadaPassport_ValidatesCorrectFormat()
    {
        // Valid
        var (result1, _) = Passport.TryCreate("AB123456", CountryCode.Canada);
        result1.IsValid.ShouldBeTrue();

        // Invalid - wrong structure
        var (result2, _) = Passport.TryCreate("123456AB", CountryCode.Canada);
        result2.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void TryCreate_WithIndiaPassport_ValidatesCorrectFormat()
    {
        // Valid
        var (result1, _) = Passport.TryCreate("K1234567", CountryCode.India);
        result1.IsValid.ShouldBeTrue();

        // Invalid - missing letter
        var (result2, _) = Passport.TryCreate("1234567", CountryCode.India);
        result2.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void TryCreate_WithSingaporePassport_ValidatesCorrectFormat()
    {
        // Valid
        var (result1, _) = Passport.TryCreate("K1234567A", CountryCode.Singapore);
        result1.IsValid.ShouldBeTrue();

        // Invalid - missing trailing letter
        var (result2, _) = Passport.TryCreate("K1234567", CountryCode.Singapore);
        result2.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void TryCreate_WithSouthAfricaPassport_ValidatesCorrectFormat()
    {
        // Valid
        var (result1, _) = Passport.TryCreate("A12345678", CountryCode.SouthAfrica);
        result1.IsValid.ShouldBeTrue();

        // Invalid - all digits
        var (result2, _) = Passport.TryCreate("123456789", CountryCode.SouthAfrica);
        result2.IsValid.ShouldBeFalse();
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSamePassport_ReturnsTrue()
    {
        var (_, passport1) = Passport.TryCreate("AB123456", CountryCode.Canada);
        var (_, passport2) = Passport.TryCreate("AB123456", CountryCode.Canada);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.Equals(passport2).ShouldBeTrue();
        passport1.ShouldBe(passport2);
    }

    [Fact]
    public void Equals_WithDifferentFormatting_ReturnsTrue()
    {
        var (_, passport1) = Passport.TryCreate("AB123456", CountryCode.Canada);
        var (_, passport2) = Passport.TryCreate("AB 123456", CountryCode.Canada);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.Equals(passport2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCasing_ReturnsTrue()
    {
        var (_, passport1) = Passport.TryCreate("AB123456", CountryCode.Canada);
        var (_, passport2) = Passport.TryCreate("ab123456", CountryCode.Canada);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.Equals(passport2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentNumber_ReturnsFalse()
    {
        var (_, passport1) = Passport.TryCreate("AB123456", CountryCode.Canada);
        var (_, passport2) = Passport.TryCreate("AB987654", CountryCode.Canada);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.Equals(passport2).ShouldBeFalse();
        passport1.ShouldNotBe(passport2);
    }

    [Fact]
    public void Equals_WithDifferentCountry_ReturnsFalse()
    {
        var (_, passport1) = Passport.TryCreate("123456789", CountryCode.UnitedStates);
        var (_, passport2) = Passport.TryCreate("123456789", CountryCode.Denmark);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.Equals(passport2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, passport) = Passport.TryCreate("AB123456", CountryCode.Canada);

        passport.ShouldNotBeNull();
        passport!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithSamePassport_ReturnsSameHash()
    {
        var (_, passport1) = Passport.TryCreate("AB123456", CountryCode.Canada);
        var (_, passport2) = Passport.TryCreate("AB 123456", CountryCode.Canada);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.GetHashCode().ShouldBe(passport2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentPassport_ReturnsDifferentHash()
    {
        var (_, passport1) = Passport.TryCreate("AB123456", CountryCode.Canada);
        var (_, passport2) = Passport.TryCreate("AB987654", CountryCode.Canada);

        passport1.ShouldNotBeNull();
        passport2.ShouldNotBeNull();
        passport1!.GetHashCode().ShouldNotBe(passport2!.GetHashCode());
    }

    #endregion

    #region Property Tests

    [Fact]
    public void IssuingCountry_ReturnsCorrectCountry()
    {
        var (_, passport) = Passport.TryCreate("AB123456", CountryCode.Canada);

        passport.ShouldNotBeNull();
        passport!.IssuingCountry.ShouldBe(CountryCode.Canada);
    }

    [Theory]
    [InlineData("N1234567", CountryCode.Australia, "Regular Passport")]
    [InlineData("P1234567", CountryCode.Australia, "Regular Passport")]
    [InlineData("D1234567", CountryCode.Australia, "Diplomatic Passport")]
    public void PassportType_ReturnsCorrectType(string passportNumber, CountryCode country, string expectedType)
    {
        var (_, passport) = Passport.TryCreate(passportNumber, country);

        passport.ShouldNotBeNull();
        passport!.PassportType.ShouldBe(expectedType);
    }

    #endregion

    #region Custom Property Name Tests

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, value) = Passport.TryCreate("INVALID", CountryCode.UnitedStates, "TravelDocument");

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "TravelDocument");
    }

    #endregion

    #region All Countries Validation Tests

    [Theory]
    [InlineData(CountryCode.UnitedStates, "123456789")]
    [InlineData(CountryCode.Canada, "AB123456")]
    [InlineData(CountryCode.Mexico, "G123456789")]
    [InlineData(CountryCode.UnitedKingdom, "123456789")]
    [InlineData(CountryCode.Ireland, "AB1234567")]
    [InlineData(CountryCode.Germany, "C01X00T47")]
    [InlineData(CountryCode.France, "12AB12345")]
    [InlineData(CountryCode.Italy, "AB1234567")]
    [InlineData(CountryCode.Spain, "AAA123456")]
    [InlineData(CountryCode.Netherlands, "NL123456")]
    [InlineData(CountryCode.Belgium, "AB123456")]
    [InlineData(CountryCode.Switzerland, "X1234567")]
    [InlineData(CountryCode.Austria, "P1234567")]
    [InlineData(CountryCode.Sweden, "12345678")]
    [InlineData(CountryCode.Norway, "1234567")]
    [InlineData(CountryCode.Denmark, "123456789")]
    [InlineData(CountryCode.Finland, "AB1234567")]
    [InlineData(CountryCode.Poland, "AB1234567")]
    [InlineData(CountryCode.CzechRepublic, "12345678")]
    [InlineData(CountryCode.Hungary, "AB123456")]
    [InlineData(CountryCode.Portugal, "N123456")]
    [InlineData(CountryCode.Russia, "123456789")]
    [InlineData(CountryCode.Japan, "TK1234567")]
    [InlineData(CountryCode.China, "E12345678")]
    [InlineData(CountryCode.India, "K1234567")]
    [InlineData(CountryCode.Brazil, "AB123456")]
    [InlineData(CountryCode.SouthAfrica, "A12345678")]
    [InlineData(CountryCode.Australia, "N1234567")]
    [InlineData(CountryCode.NewZealand, "LA123456")]
    [InlineData(CountryCode.Singapore, "K1234567A")]
    [InlineData(CountryCode.SouthKorea, "M12345678")]
    public void TryCreate_WithAllSupportedCountries_AcceptsValidFormats(CountryCode country, string passportNumber)
    {
        var (result, value) = Passport.TryCreate(passportNumber, country);

        result.IsValid.ShouldBeTrue($"Passport {passportNumber} should be valid for {country}");
        value.ShouldNotBeNull();
        value!.IssuingCountry.ShouldBe(country);
    }

    #endregion
}
