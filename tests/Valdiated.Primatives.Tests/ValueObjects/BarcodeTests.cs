using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class BarcodeTests
{
    public static TheoryData<string> ValidBarcodes => new()
    {
        // UPC-A (12 digits with valid checksum)
        "042100005264", // Valid UPC-A
        "036000291452", // Valid UPC-A
        
        // EAN-13 (13 digits with valid checksum)
        "5901234123457", // This one we know works from earlier tests
        
        // EAN-8 (8 digits with valid checksum)
        "96385074",
        "40123455", // Valid EAN-8
        
        // Code39 (with * delimiters)
        "*HELLO*",
        "*ABC123*",
        "*TEST-CODE*",
        
        // Code128 (alphanumeric)
        "ABC123",
        "TEST1234",
        "Product-Code-456"
    };

    public static TheoryData<string> InvalidBarcodes => new()
    {
        "",
        "   ",
        "123", // Too short for any numeric format
        "1234567890123", // Wrong checksum for EAN-13
        "12345", // Invalid length for numeric formats
        "*INCOMPLETE", // Code39 missing closing *
        "INCOMPLETE*", // Code39 missing opening *
        "**", // Code39 too short (needs content between *)
    };

    [Theory]
    [MemberData(nameof(ValidBarcodes))]
    public void TryCreate_Succeeds_For_Valid_Barcodes(string barcode)
    {
        // Act
        var (result, value) = Barcode.TryCreate(barcode);

        // Assert
        result.IsValid.ShouldBeTrue($"Result should be valid for barcode: {barcode}");
        value.ShouldNotBeNull($"Value should not be null when validation succeeds for: {barcode}");
        value!.Value.ShouldBe(barcode);
    }

    [Theory]
    [MemberData(nameof(InvalidBarcodes))]
    public void TryCreate_Fails_For_Invalid_Barcodes(string barcode)
    {
        // Act
        var (result, value) = Barcode.TryCreate(barcode);

        // Assert
        result.IsValid.ShouldBeFalse($"Result should be invalid for barcode: {barcode}");
        value.ShouldBeNull($"Value should be null when validation fails for: {barcode}");
    }

    [Fact]
    public void GetNormalized_Removes_Separators()
    {
        // Arrange
        var (_, barcode) = Barcode.TryCreate("0421-0000-5264");

        // Act
        var normalized = barcode!.GetNormalized();

        // Assert
        normalized.ShouldBe("042100005264");
    }

    [Theory]
    [InlineData("042100005264", BarcodeFormat.UpcA)]
    [InlineData("5901234123457", BarcodeFormat.Ean13)]
    [InlineData("96385074", BarcodeFormat.Ean8)]
    [InlineData("*HELLO*", BarcodeFormat.Code39)]
    [InlineData("ABC123", BarcodeFormat.Code128)]
    public void Format_Is_Detected_Correctly(string barcode, BarcodeFormat expectedFormat)
    {
        // Act
        var (_, value) = Barcode.TryCreate(barcode);

        // Assert
        value.ShouldNotBeNull();
        value!.Format.ShouldBe(expectedFormat);
    }

    [Fact]
    public void ToString_Returns_Original_Value()
    {
        // Arrange
        var originalValue = "042100005264";
        var (_, barcode) = Barcode.TryCreate(originalValue);

        // Act
        var stringValue = barcode!.ToString();

        // Assert
        stringValue.ShouldBe(originalValue);
    }

    [Fact]
    public void Equal_Barcodes_Should_Be_Equal()
    {
        // Arrange
        var (_, barcode1) = Barcode.TryCreate("042100005264");
        var (_, barcode2) = Barcode.TryCreate("042100005264");

        // Assert
        barcode1.ShouldBe(barcode2);
        barcode1!.GetHashCode().ShouldBe(barcode2!.GetHashCode());
    }

    [Fact]
    public void Different_Barcodes_Should_Not_Be_Equal()
    {
        // Arrange
        var (_, barcode1) = Barcode.TryCreate("042100005264");
        var (_, barcode2) = Barcode.TryCreate("036000291452");

        // Assert
        barcode1.ShouldNotBe(barcode2);
    }

    [Fact]
    public void Custom_PropertyName_Appears_In_Validation_Error()
    {
        // Act
        var (result, _) = Barcode.TryCreate("", "ProductBarcode");

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.MemberName == "ProductBarcode");
    }
}
