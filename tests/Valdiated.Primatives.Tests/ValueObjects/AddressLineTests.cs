using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class AddressLineTests
{
    [Fact]
    public void TryCreate_WithValidAddressLine_ShouldSucceed()
    {
        var (result, addressLine) = AddressLine.TryCreate("123 Main Street");

        result.IsValid.ShouldBeTrue("Result should be valid for a valid address line");
        addressLine.ShouldNotBeNull("AddressLine should not be null when validation succeeds");
        addressLine!.Value.ShouldBe("123 Main Street");
    }

    [Fact]
    public void TryCreate_WithNullValue_ShouldReturnSuccessWithNullValue()
    {
        var (result, addressLine) = AddressLine.TryCreate(null);

        result.IsValid.ShouldBeTrue("Result should be valid for null value");
        addressLine.ShouldBeNull("AddressLine should be null when input is null");
    }

    [Fact]
    public void TryCreate_WithEmptyString_ShouldReturnSuccessWithNullValue()
    {
        var (result, addressLine) = AddressLine.TryCreate("");

        result.IsValid.ShouldBeTrue("Result should be valid for empty string");
        addressLine.ShouldBeNull("AddressLine should be null when input is empty");
    }

    [Fact]
    public void TryCreate_WithWhitespace_ShouldReturnSuccessWithNullValue()
    {
        var (result, addressLine) = AddressLine.TryCreate("   ");

        result.IsValid.ShouldBeTrue("Result should be valid for whitespace");
        addressLine.ShouldBeNull("AddressLine should be null when input is whitespace");
    }

    [Fact]
    public void TryCreate_WithMaxLength_ShouldSucceed()
    {
        var value = new string('a', 200);
        var (result, addressLine) = AddressLine.TryCreate(value);

        result.IsValid.ShouldBeTrue("Result should be valid for 200 character address line");
        addressLine.ShouldNotBeNull();
        addressLine!.Value.Length.ShouldBe(200);
    }

    [Fact]
    public void TryCreate_ExceedingMaxLength_ShouldFail()
    {
        var value = new string('a', 201);
        var (result, addressLine) = AddressLine.TryCreate(value);

        result.IsValid.ShouldBeFalse("Result should be invalid when address line exceeds 200 characters");
        addressLine.ShouldBeNull("AddressLine should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Code.ShouldBe("MaxLength");
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_ShouldUseInErrorMessage()
    {
        var value = new string('a', 201);
        var (result, addressLine) = AddressLine.TryCreate(value, "StreetAddress");

        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("StreetAddress");
    }

    [Theory]
    [InlineData("123 Main St")]
    [InlineData("Apt 4B, 456 Oak Avenue")]
    [InlineData("Building 7, Floor 3, Room 301")]
    [InlineData("P.O. Box 12345")]
    [InlineData("Rural Route 5")]
    public void TryCreate_WithVariousValidFormats_ShouldSucceed(string value)
    {
        var (result, addressLine) = AddressLine.TryCreate(value);

        result.IsValid.ShouldBeTrue($"Result should be valid for: {value}");
        addressLine.ShouldNotBeNull();
        addressLine!.Value.ShouldBe(value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var (_, addressLine) = AddressLine.TryCreate("123 Main Street");

        addressLine.ShouldNotBeNull();
        addressLine!.ToString().ShouldBe("123 Main Street");
    }

    [Fact]
    public void TryCreate_WithUnicodeCharacters_ShouldSucceed()
    {
        var (result, addressLine) = AddressLine.TryCreate("123 Rue de la Paix, Montréal");

        result.IsValid.ShouldBeTrue("Result should be valid for address with Unicode characters");
        addressLine.ShouldNotBeNull();
        addressLine!.Value.ShouldBe("123 Rue de la Paix, Montréal");
    }

    [Fact]
    public void TryCreate_WithSpecialCharacters_ShouldSucceed()
    {
        var (result, addressLine) = AddressLine.TryCreate("123 Main St. #4-B");

        result.IsValid.ShouldBeTrue("Result should be valid for address with special characters");
        addressLine.ShouldNotBeNull();
        addressLine!.Value.ShouldBe("123 Main St. #4-B");
    }

    [Fact]
    public void TryCreate_WithNumbers_ShouldSucceed()
    {
        var (result, addressLine) = AddressLine.TryCreate("12345");

        result.IsValid.ShouldBeTrue("Result should be valid for numeric address");
        addressLine.ShouldNotBeNull();
        addressLine!.Value.ShouldBe("12345");
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var (_, addressLine1) = AddressLine.TryCreate("123 Main Street");
        var (_, addressLine2) = AddressLine.TryCreate("123 Main Street");

        addressLine1.ShouldBe(addressLine2);
        (addressLine1 == addressLine2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var (_, addressLine1) = AddressLine.TryCreate("123 Main Street");
        var (_, addressLine2) = AddressLine.TryCreate("456 Oak Avenue");

        addressLine1.ShouldNotBe(addressLine2);
        (addressLine1 != addressLine2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_NullValues_AreEqual()
    {
        var (_, addressLine1) = AddressLine.TryCreate(null);
        var (_, addressLine2) = AddressLine.TryCreate("");

        addressLine1.ShouldBe(addressLine2);
        addressLine1.ShouldBeNull();
        addressLine2.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_WithLeadingAndTrailingSpaces_TrimsSpaces()
    {
        var (result, addressLine) = AddressLine.TryCreate("  123 Main Street  ");

        result.IsValid.ShouldBeTrue();
        addressLine.ShouldNotBeNull();
        // Trims leading and trailing spaces
        addressLine!.Value.ShouldBe("123 Main Street");
    }

    [Fact]
    public void AddressLine_ShouldHaveRecordSemantics()
    {
        var (_, addressLine1) = AddressLine.TryCreate("123 Main Street");
        var (_, addressLine2) = AddressLine.TryCreate("123 Main Street");

        // Records should be equal based on value
        addressLine1.ShouldNotBeNull();
        addressLine2.ShouldNotBeNull();
        addressLine1.ShouldBe(addressLine2);
        addressLine1!.GetHashCode().ShouldBe(addressLine2!.GetHashCode());
    }

    [Fact]
    public void TryCreate_WithExactly200Characters_ShouldSucceed()
    {
        var exactValue = new string('X', 200);
        var (result, addressLine) = AddressLine.TryCreate(exactValue);

        result.IsValid.ShouldBeTrue("Exactly 200 characters should be valid");
        addressLine.ShouldNotBeNull();
        addressLine!.Value.Length.ShouldBe(200);
    }

    [Fact]
    public void TryCreate_With201Characters_ShouldFail()
    {
        var tooLong = new string('X', 201);
        var (result, addressLine) = AddressLine.TryCreate(tooLong);

        result.IsValid.ShouldBeFalse("201 characters should exceed maximum");
        addressLine.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }
}
