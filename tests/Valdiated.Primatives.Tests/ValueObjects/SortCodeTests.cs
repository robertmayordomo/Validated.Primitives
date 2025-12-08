using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class SortCodeTests
{
    [Theory]
    [InlineData(CountryCode.UnitedKingdom, "123456")]
    [InlineData(CountryCode.UnitedKingdom, "12-34-56")]
    [InlineData(CountryCode.UnitedKingdom, "12 34 56")]
    [InlineData(CountryCode.Ireland, "123456")]
    [InlineData(CountryCode.Ireland, "12-34-56")]
    public void TryCreate_WithValidSortCode_ReturnsSuccess(CountryCode countryCode, string sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(countryCode, sortCode);

        result.IsValid.ShouldBeTrue($"Sort code {sortCode} should be valid for {countryCode}");
        sortCodeValue.ShouldNotBeNull();
        sortCodeValue!.CountryCode.ShouldBe(countryCode);
    }

    [Fact]
    public void TryCreate_WithDashes_StoresOriginalFormat()
    {
        var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

        result.IsValid.ShouldBeTrue();
        sortCode.ShouldNotBeNull();
        sortCode!.Value.ShouldBe("12-34-56");
    }

    [Fact]
    public void ToDigitsOnly_ReturnsDigitsWithoutSeparators()
    {
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

        sortCode.ShouldNotBeNull();
        sortCode!.ToDigitsOnly().ShouldBe("123456");
    }

    [Theory]
    [InlineData("12 34 56")]
    [InlineData("12-34-56")]
    public void ToDigitsOnly_WithVariousFormats_ReturnsDigitsOnly(string input)
    {
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, input);

        sortCode.ShouldNotBeNull();
        sortCode!.ToDigitsOnly().ShouldBe("123456");
    }

    [Theory]
    [InlineData(CountryCode.UnitedKingdom, "123456", "12-34-56")]
    [InlineData(CountryCode.UnitedKingdom, "12-34-56", "12-34-56")]
    [InlineData(CountryCode.Ireland, "123456", "12-34-56")]
    public void ToFormattedString_ReturnsFormattedDisplay(CountryCode countryCode, string input, string expected)
    {
        var (_, sortCode) = SortCode.TryCreate(countryCode, input);

        sortCode.ShouldNotBeNull();
        sortCode!.ToFormattedString().ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(CountryCode.UnitedKingdom, sortCode);

        result.IsValid.ShouldBeFalse();
        sortCodeValue.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12-34-567")]
    public void TryCreate_UkWithInvalidLength_ReturnsFailure(string sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(CountryCode.UnitedKingdom, sortCode);

        result.IsValid.ShouldBeFalse();
        sortCodeValue.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidCountrySortCodeFormat");
    }

    [Theory]
    [InlineData("12-34-5A")]
    [InlineData("ABC-DE-FG")]
    public void TryCreate_WithNonNumericCharacters_ReturnsFailure(string sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(CountryCode.UnitedKingdom, sortCode);

        result.IsValid.ShouldBeFalse();
        sortCodeValue.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidCharacters");
    }

    [Theory]
    [InlineData("12.34.56")]
    [InlineData("12/34/56")]
    public void TryCreate_WithInvalidSeparators_ReturnsFailure(string sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(CountryCode.UnitedKingdom, sortCode);

        result.IsValid.ShouldBeFalse();
        sortCodeValue.ShouldBeNull();
    }

    [Fact]
    public void ToString_ReturnsOriginalValue()
    {
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

        sortCode.ShouldNotBeNull();
        sortCode!.ToString().ShouldBe("12-34-56");
    }

    [Fact]
    public void GetCountryName_ReturnsFormattedName()
    {
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");

        sortCode.ShouldNotBeNull();
        sortCode!.GetCountryName().ShouldBe("United Kingdom");
    }

    [Fact]
    public void Equals_WithSameSortCode_ReturnsTrue()
    {
        var (_, sortCode1) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, sortCode2) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

        sortCode1.ShouldNotBeNull();
        sortCode2.ShouldNotBeNull();
        sortCode1!.Equals(sortCode2).ShouldBeTrue();
        sortCode1.ShouldBe(sortCode2);
    }

    [Fact]
    public void Equals_WithDifferentSortCode_ReturnsFalse()
    {
        var (_, sortCode1) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, sortCode2) = SortCode.TryCreate(CountryCode.UnitedKingdom, "654321");

        sortCode1.ShouldNotBeNull();
        sortCode2.ShouldNotBeNull();
        sortCode1!.Equals(sortCode2).ShouldBeFalse();
        sortCode1.ShouldNotBe(sortCode2);
    }

    [Fact]
    public void Equals_WithDifferentCountry_ReturnsFalse()
    {
        var (_, sortCode1) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, sortCode2) = SortCode.TryCreate(CountryCode.Ireland, "123456");

        sortCode1.ShouldNotBeNull();
        sortCode2.ShouldNotBeNull();
        sortCode1!.Equals(sortCode2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");

        sortCode.ShouldNotBeNull();
        sortCode!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameSortCode_ReturnsSameHash()
    {
        var (_, sortCode1) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, sortCode2) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

        sortCode1.ShouldNotBeNull();
        sortCode2.ShouldNotBeNull();
        sortCode1!.GetHashCode().ShouldBe(sortCode2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentSortCode_ReturnsDifferentHash()
    {
        var (_, sortCode1) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, sortCode2) = SortCode.TryCreate(CountryCode.UnitedKingdom, "654321");

        sortCode1.ShouldNotBeNull();
        sortCode2.ShouldNotBeNull();
        sortCode1!.GetHashCode().ShouldNotBe(sortCode2!.GetHashCode());
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12345", "BankSortCode");

        result.IsValid.ShouldBeFalse();
        sortCode.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "BankSortCode");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "123456")]
    [InlineData(CountryCode.Germany, "123456")]
    public void TryCreate_WithUnsupportedCountry_AllowsAnyFormat(CountryCode countryCode, string sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(countryCode, sortCode);

        result.IsValid.ShouldBeTrue();
        sortCodeValue.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_Ireland_WithValidSortCode_ReturnsSuccess()
    {
        var (result, sortCode) = SortCode.TryCreate(CountryCode.Ireland, "90-12-34");

        result.IsValid.ShouldBeTrue();
        sortCode.ShouldNotBeNull();
        sortCode!.ToDigitsOnly().ShouldBe("901234");
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567")]
    public void TryCreate_Ireland_WithInvalidLength_ReturnsFailure(string sortCode)
    {
        var (result, sortCodeValue) = SortCode.TryCreate(CountryCode.Ireland, sortCode);

        result.IsValid.ShouldBeFalse();
        sortCodeValue.ShouldBeNull();
    }
}
