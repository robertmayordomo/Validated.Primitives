using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class SocialSecurityNumberTests
{
    [Theory]
    [InlineData("123-45-6789")]
    [InlineData("123456789")]
    [InlineData("567-65-4319")]
    [InlineData("001-01-0001")]
    public void TryCreate_WithValidSSN_ReturnsSuccess(string ssn)
    {
        var (result, ssnValue) = SocialSecurityNumber.TryCreate(ssn);

        result.IsValid.ShouldBeTrue($"SSN {ssn} should be valid");
        ssnValue.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_WithDashes_StoresDigitsOnly()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        result.IsValid.ShouldBeTrue();
        ssn.ShouldNotBeNull();
        ssn!.ToDigitsOnly().ShouldBe("123456789");
    }

    [Fact]
    public void TryCreate_WithoutDashes_AcceptsFormat()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("123456789");

        result.IsValid.ShouldBeTrue();
        ssn.ShouldNotBeNull();
        ssn!.ToDigitsOnly().ShouldBe("123456789");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? ssn)
    {
        var (result, ssnValue) = SocialSecurityNumber.TryCreate(ssn);

        result.IsValid.ShouldBeFalse();
        ssnValue.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("12-34-5678")]    // Too few digits
    [InlineData("1234-56-789")]   // Wrong format
    [InlineData("123-456-789")]   // Wrong format
    [InlineData("12345678")]      // Only 8 digits
    [InlineData("1234567890")]    // 10 digits
    [InlineData("ABC-DE-FGHI")]   // Letters
    public void TryCreate_WithInvalidFormat_ReturnsFailure(string ssn)
    {
        var (result, ssnValue) = SocialSecurityNumber.TryCreate(ssn);

        result.IsValid.ShouldBeFalse();
        ssnValue.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_WithAreaNumber000_ReturnsFailure()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("000-12-3456");

        result.IsValid.ShouldBeFalse();
        ssn.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidAreaNumber");
        result.Errors.ShouldContain(e => e.Message.Contains("000"));
    }

    [Fact]
    public void TryCreate_WithAreaNumber666_ReturnsFailure()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("666-12-3456");

        result.IsValid.ShouldBeFalse();
        ssn.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidAreaNumber");
        result.Errors.ShouldContain(e => e.Message.Contains("666"));
    }

    [Theory]
    [InlineData("900-12-3456")]
    [InlineData("950-12-3456")]
    [InlineData("999-12-3456")]
    public void TryCreate_WithAreaNumber900To999_ReturnsFailure(string ssn)
    {
        var (result, ssnValue) = SocialSecurityNumber.TryCreate(ssn);

        result.IsValid.ShouldBeFalse();
        ssnValue.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidAreaNumber");
        result.Errors.ShouldContain(e => e.Message.Contains("900-999"));
    }

    [Fact]
    public void TryCreate_WithGroupNumber00_ReturnsFailure()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("123-00-4567");

        result.IsValid.ShouldBeFalse();
        ssn.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidGroupNumber");
        result.Errors.ShouldContain(e => e.Message.Contains("00"));
    }

    [Fact]
    public void TryCreate_WithSerialNumber0000_ReturnsFailure()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("123-45-0000");

        result.IsValid.ShouldBeFalse();
        ssn.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidSerialNumber");
        result.Errors.ShouldContain(e => e.Message.Contains("0000"));
    }

    [Theory]
    [InlineData("987-65-4320")]
    [InlineData("987-65-4321")]
    [InlineData("987-65-4329")]
    public void TryCreate_WithAdvertisingNumber_ReturnsFailure(string ssn)
    {
        var (result, ssnValue) = SocialSecurityNumber.TryCreate(ssn);

        result.IsValid.ShouldBeFalse();
        ssnValue.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "AdvertisingNumber");
    }

    [Fact]
    public void ToString_ReturnsFormattedSSN()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123456789");

        ssn.ShouldNotBeNull();
        ssn!.ToString().ShouldBe("123-45-6789");
    }

    [Fact]
    public void ToDigitsOnly_ReturnsNineDigits()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.ToDigitsOnly().ShouldBe("123456789");
    }

    [Fact]
    public void Masked_ShowsOnlyLastFourDigits()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.Masked().ShouldBe("XXX-XX-6789");
    }

    [Fact]
    public void PartiallyMasked_ShowsAreaAndSerialNumbers()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.PartiallyMasked().ShouldBe("123-XX-6789");
    }

    [Fact]
    public void AreaNumber_ReturnsFirstThreeDigits()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.AreaNumber.ShouldBe("123");
    }

    [Fact]
    public void GroupNumber_ReturnsMiddleTwoDigits()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.GroupNumber.ShouldBe("45");
    }

    [Fact]
    public void SerialNumber_ReturnsLastFourDigits()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.SerialNumber.ShouldBe("6789");
    }

    [Fact]
    public void Equals_WithSameSSN_ReturnsTrue()
    {
        var (_, ssn1) = SocialSecurityNumber.TryCreate("123-45-6789");
        var (_, ssn2) = SocialSecurityNumber.TryCreate("123456789");

        ssn1.ShouldNotBeNull();
        ssn2.ShouldNotBeNull();
        ssn1!.Equals(ssn2).ShouldBeTrue();
        ssn1.ShouldBe(ssn2);
    }

    [Fact]
    public void Equals_WithDifferentSSN_ReturnsFalse()
    {
        var (_, ssn1) = SocialSecurityNumber.TryCreate("123-45-6789");
        var (_, ssn2) = SocialSecurityNumber.TryCreate("567-65-4319");

        ssn1.ShouldNotBeNull();
        ssn2.ShouldNotBeNull();
        ssn1!.Equals(ssn2).ShouldBeFalse();
        ssn1.ShouldNotBe(ssn2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");

        ssn.ShouldNotBeNull();
        ssn!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameSSN_ReturnsSameHash()
    {
        var (_, ssn1) = SocialSecurityNumber.TryCreate("123-45-6789");
        var (_, ssn2) = SocialSecurityNumber.TryCreate("123456789");

        ssn1.ShouldNotBeNull();
        ssn2.ShouldNotBeNull();
        ssn1!.GetHashCode().ShouldBe(ssn2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentSSN_ReturnsDifferentHash()
    {
        var (_, ssn1) = SocialSecurityNumber.TryCreate("123-45-6789");
        var (_, ssn2) = SocialSecurityNumber.TryCreate("567-65-4319");

        ssn1.ShouldNotBeNull();
        ssn2.ShouldNotBeNull();
        ssn1!.GetHashCode().ShouldNotBe(ssn2!.GetHashCode());
    }

    [Fact]
    public void TryCreate_WithSpaces_ExtractsDigits()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("123 45 6789");

        result.IsValid.ShouldBeTrue();
        ssn.ShouldNotBeNull();
        ssn!.ToDigitsOnly().ShouldBe("123456789");
    }

    [Fact]
    public void TryCreate_WithExtraCharacters_IgnoresNonDigits()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("SSN: 123-45-6789");

        result.IsValid.ShouldBeTrue();
        ssn.ShouldNotBeNull();
        ssn!.ToDigitsOnly().ShouldBe("123456789");
    }

    [Fact]
    public void TryCreate_WithValidEdgeCases_AcceptsAll()
    {
        // Valid edge cases: 001-01-0001 is the lowest valid SSN
        var validCases = new[]
        {
            "001-01-0001",
            "665-99-9999",  // Just before 666
            "667-01-0001",  // Just after 666
            "899-99-9999"   // Just before 900
        };

        foreach (var ssn in validCases)
        {
            var (result, ssnValue) = SocialSecurityNumber.TryCreate(ssn);
            result.IsValid.ShouldBeTrue($"SSN {ssn} should be valid");
            ssnValue.ShouldNotBeNull();
        }
    }

    [Fact]
    public void TryCreate_WithMultipleErrors_ReturnsAllErrors()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("000-00-0000");

        result.IsValid.ShouldBeFalse();
        ssn.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThan(1);
        result.Errors.ShouldContain(e => e.Code == "InvalidAreaNumber");
        result.Errors.ShouldContain(e => e.Code == "InvalidGroupNumber");
        result.Errors.ShouldContain(e => e.Code == "InvalidSerialNumber");
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, ssn) = SocialSecurityNumber.TryCreate("000-12-3456", "EmployeeSSN");

        result.IsValid.ShouldBeFalse();
        ssn.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "EmployeeSSN");
    }
}
