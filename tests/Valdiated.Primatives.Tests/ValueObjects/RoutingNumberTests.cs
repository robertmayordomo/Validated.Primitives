using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class RoutingNumberTests
{
    [Theory]
    [InlineData("021000021")] // Bank of America, NY
    [InlineData("011401533")] // Wells Fargo, CA
    [InlineData("121000248")] // Wells Fargo, TX
    [InlineData("026009593")] // Bank of America, FL
    [InlineData("111000025")] // Chase, NY
    [InlineData("122000247")] // Wells Fargo, WA
    [InlineData("063100277")] // Bank of America, CA
    [InlineData("031201360")] // Chase, IL
    [InlineData("021200025")] // Citibank, NY
    public void TryCreate_WithValidRoutingNumber_ReturnsSuccess(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeTrue($"Routing number {routingNumber} should be valid");
        value.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("0210-0002-1")]
    [InlineData("0210 0002 1")]
    [InlineData("021-000-021")]
    public void TryCreate_WithFormattedRoutingNumber_ReturnsSuccess(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.ToDigitsOnly().ShouldBe("021000021");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("02100002")]   // Too short
    [InlineData("0210000211")] // Too long
    [InlineData("021")]        // Way too short
    public void TryCreate_WithInvalidLength_ReturnsFailure(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidLength");
    }

    [Theory]
    [InlineData("0210A0021")]
    [InlineData("ABC-DEF-GHI")]
    [InlineData("021-00A-021")]
    public void TryCreate_WithNonDigits_ReturnsFailure(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Theory]
    [InlineData("021000020")] // Invalid checksum
    [InlineData("011401530")] // Invalid checksum
    [InlineData("121000240")] // Invalid checksum
    public void TryCreate_WithInvalidChecksum_ReturnsFailure(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidChecksum");
    }

    [Theory]
    [InlineData("131000021")] // 13 - invalid Federal Reserve symbol
    [InlineData("201000021")] // 20 - invalid
    [InlineData("991000021")] // 99 - invalid
    public void TryCreate_WithInvalidFederalReserveSymbol_ReturnsFailure(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidFederalReserveSymbol");
    }

    [Fact]
    public void ToDigitsOnly_ReturnsNineDigits()
    {
        var (_, routingNumber) = RoutingNumber.TryCreate("0210-0002-1");

        routingNumber.ShouldNotBeNull();
        routingNumber!.ToDigitsOnly().ShouldBe("021000021");
    }

    [Theory]
    [InlineData("0210 0002 1")]
    [InlineData("021-000-021")]
    [InlineData("021000021")]
    public void ToDigitsOnly_WithVariousFormats_ReturnsDigitsOnly(string input)
    {
        var (_, routingNumber) = RoutingNumber.TryCreate(input);

        routingNumber.ShouldNotBeNull();
        routingNumber!.ToDigitsOnly().ShouldBe("021000021");
    }

    [Theory]
    [InlineData("021000021", "0210-0002-1")]
    [InlineData("0210-0002-1", "0210-0002-1")]
    [InlineData("0210 0002 1", "0210-0002-1")]
    public void ToFormattedString_ReturnsFormattedDisplay(string input, string expected)
    {
        var (_, routingNumber) = RoutingNumber.TryCreate(input);

        routingNumber.ShouldNotBeNull();
        routingNumber!.ToFormattedString().ShouldBe(expected);
    }

    [Fact]
    public void ToString_ReturnsOriginalValue()
    {
        var (_, routingNumber) = RoutingNumber.TryCreate("0210-0002-1");

        routingNumber.ShouldNotBeNull();
        routingNumber!.ToString().ShouldBe("0210-0002-1");
    }

    [Fact]
    public void FederalReserveSymbol_ReturnsFirstFourDigits()
    {
        var (_, routingNumber) = RoutingNumber.TryCreate("021000021");

        routingNumber.ShouldNotBeNull();
        routingNumber!.FederalReserveSymbol.ShouldBe("0210");
    }

    [Fact]
    public void InstitutionIdentifier_ReturnsMiddleFourDigits()
    {
        var (_, routingNumber) = RoutingNumber.TryCreate("021000021");

        routingNumber.ShouldNotBeNull();
        routingNumber!.InstitutionIdentifier.ShouldBe("0002");
    }

    [Fact]
    public void CheckDigit_ReturnsLastDigit()
    {
        var (_, routingNumber) = RoutingNumber.TryCreate("021000021");

        routingNumber.ShouldNotBeNull();
        routingNumber!.CheckDigit.ShouldBe("1");
    }

    [Theory]
    [InlineData("021000021", 2)]  // Boston
    [InlineData("011401533", 1)]  // Boston
    [InlineData("111000025", 11)] // Dallas
    [InlineData("121000248", 12)] // San Francisco
    public void FederalReserveDistrict_ReturnsFirstTwoDigits(string input, int expectedDistrict)
    {
        var (_, routingNumber) = RoutingNumber.TryCreate(input);

        routingNumber.ShouldNotBeNull();
        routingNumber!.FederalReserveDistrict.ShouldBe(expectedDistrict);
    }

    [Fact]
    public void Equals_WithSameRoutingNumber_ReturnsTrue()
    {
        var (_, routingNumber1) = RoutingNumber.TryCreate("021000021");
        var (_, routingNumber2) = RoutingNumber.TryCreate("021000021");

        routingNumber1.ShouldNotBeNull();
        routingNumber2.ShouldNotBeNull();
        routingNumber1!.Equals(routingNumber2).ShouldBeTrue();
        routingNumber1.ShouldBe(routingNumber2);
    }

    [Fact]
    public void Equals_WithDifferentFormatting_ReturnsTrue()
    {
        var (_, routingNumber1) = RoutingNumber.TryCreate("021000021");
        var (_, routingNumber2) = RoutingNumber.TryCreate("0210-0002-1");

        routingNumber1.ShouldNotBeNull();
        routingNumber2.ShouldNotBeNull();
        routingNumber1!.Equals(routingNumber2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentRoutingNumber_ReturnsFalse()
    {
        var (_, routingNumber1) = RoutingNumber.TryCreate("021000021");
        var (_, routingNumber2) = RoutingNumber.TryCreate("011401533");

        routingNumber1.ShouldNotBeNull();
        routingNumber2.ShouldNotBeNull();
        routingNumber1!.Equals(routingNumber2).ShouldBeFalse();
        routingNumber1.ShouldNotBe(routingNumber2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, routingNumber) = RoutingNumber.TryCreate("021000021");

        routingNumber.ShouldNotBeNull();
        routingNumber!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameRoutingNumber_ReturnsSameHash()
    {
        var (_, routingNumber1) = RoutingNumber.TryCreate("021000021");
        var (_, routingNumber2) = RoutingNumber.TryCreate("0210-0002-1");

        routingNumber1.ShouldNotBeNull();
        routingNumber2.ShouldNotBeNull();
        routingNumber1!.GetHashCode().ShouldBe(routingNumber2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentRoutingNumber_ReturnsDifferentHash()
    {
        var (_, routingNumber1) = RoutingNumber.TryCreate("021000021");
        var (_, routingNumber2) = RoutingNumber.TryCreate("011401533");

        routingNumber1.ShouldNotBeNull();
        routingNumber2.ShouldNotBeNull();
        routingNumber1!.GetHashCode().ShouldNotBe(routingNumber2!.GetHashCode());
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, routingNumber) = RoutingNumber.TryCreate("02100002", "BankRoutingNumber");

        result.IsValid.ShouldBeFalse();
        routingNumber.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "BankRoutingNumber");
    }

    [Fact]
    public void TryCreate_WithLeadingAndTrailingSpaces_StripsSpaces()
    {
        var (result, routingNumber) = RoutingNumber.TryCreate("  021000021  ");

        result.IsValid.ShouldBeTrue();
        routingNumber.ShouldNotBeNull();
        routingNumber!.ToDigitsOnly().ShouldBe("021000021");
    }

    [Theory]
    [InlineData("021000021")]  // Federal Reserve code 02
    [InlineData("011401533")]  // Federal Reserve code 01
    [InlineData("111000025")]  // Federal Reserve code 11
    [InlineData("321270742")]  // Federal Reserve code 32
    [InlineData("611091479")]  // Federal Reserve code 61
    [InlineData("800013527")]  // Federal Reserve code 80
    public void TryCreate_WithVariousFederalReserveCodes_ReturnsSuccess(string routingNumber)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_WithMultipleErrors_ReturnsAllErrors()
    {
        var (result, routingNumber) = RoutingNumber.TryCreate("ABC");

        result.IsValid.ShouldBeFalse();
        routingNumber.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThan(1);
    }

    [Theory]
    [InlineData("021000021", "Bank of America (NY)")]
    [InlineData("011401533", "Wells Fargo (CA)")]
    [InlineData("121000248", "Wells Fargo (TX)")]
    [InlineData("026009593", "Bank of America (FL)")]
    [InlineData("111000025", "Chase (NY)")]
    [InlineData("122000247", "Wells Fargo (WA)")]
    [InlineData("063100277", "Bank of America (CA)")]
    [InlineData("031201360", "Chase (IL)")]
    [InlineData("021200025", "Citibank (NY)")]
    [InlineData("322271627", "Chase (AZ)")]
    public void TryCreate_WithRealWorldRoutingNumbers_ReturnsSuccess(string routingNumber, string description)
    {
        var (result, value) = RoutingNumber.TryCreate(routingNumber);

        result.IsValid.ShouldBeTrue($"{description} routing number should be valid");
        value.ShouldNotBeNull();
        value!.ToDigitsOnly().ShouldBe(routingNumber);
    }
}
