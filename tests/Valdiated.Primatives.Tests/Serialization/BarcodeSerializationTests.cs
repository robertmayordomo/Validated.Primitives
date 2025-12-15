using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class BarcodeSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_Barcode_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, barcode) = Barcode.TryCreate("042100005264");

        // Act
        var json = JsonSerializer.Serialize(barcode, _options);

        // Assert
        json.ShouldBe("\"042100005264\"");
    }

    [Fact]
    public void Deserialize_Barcode_Should_Read_From_String()
    {
        // Arrange
        var json = "\"042100005264\"";

        // Act
        var barcode = JsonSerializer.Deserialize<Barcode>(json, _options);

        // Assert
        barcode.ShouldNotBeNull();
        barcode.Value.ShouldBe("042100005264");
        barcode.Format.ShouldBe(BarcodeFormat.UpcA);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = Barcode.TryCreate("5901234123457");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Barcode>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.Format.ShouldBe(original.Format);
        deserialized.ShouldBe(original);
    }

    [Theory]
    [InlineData("042100005264", BarcodeFormat.UpcA)]
    [InlineData("5901234123457", BarcodeFormat.Ean13)]
    [InlineData("96385074", BarcodeFormat.Ean8)]
    [InlineData("*HELLO*", BarcodeFormat.Code39)]
    [InlineData("ABC123", BarcodeFormat.Code128)]
    public void Deserialize_Should_Preserve_Format(string barcodeValue, BarcodeFormat expectedFormat)
    {
        // Arrange
        var json = $"\"{barcodeValue}\"";

        // Act
        var barcode = JsonSerializer.Deserialize<Barcode>(json, _options);

        // Assert
        barcode.ShouldNotBeNull();
        barcode.Format.ShouldBe(expectedFormat);
    }

    [Fact]
    public void Deserialize_Invalid_Barcode_Should_Throw_JsonException()
    {
        // Arrange - use a string that cannot be any valid barcode format
        var json = "\"1234567890123\""; // 13 digits but wrong checksum for EAN-13

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<Barcode>(json, _options));
    }

    [Fact]
    public void Serialize_Null_Barcode_Should_Write_Null()
    {
        // Arrange
        Barcode? barcode = null;

        // Act
        var json = JsonSerializer.Serialize(barcode, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var barcode = JsonSerializer.Deserialize<Barcode>(json, _options);

        // Assert
        barcode.ShouldBeNull();
    }

    [Fact]
    public void Serialize_Barcode_In_Object_Should_Work()
    {
        // Arrange
        var (_, barcode) = Barcode.TryCreate("042100005264");
        var obj = new { ProductBarcode = barcode, Name = "Product A" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"productBarcode\":\"042100005264\"");
        json.ShouldContain("\"name\":\"Product A\"");
    }

    [Fact]
    public void Deserialize_Barcode_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"productBarcode\":\"042100005264\",\"name\":\"Product A\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.ProductBarcode.ShouldNotBeNull();
        obj.ProductBarcode.Value.ShouldBe("042100005264");
        obj.ProductBarcode.Format.ShouldBe(BarcodeFormat.UpcA);
        obj.Name.ShouldBe("Product A");
    }

    [Fact]
    public void Serialize_Multiple_Barcode_Formats_In_Object_Should_Work()
    {
        // Arrange
        var (_, upcA) = Barcode.TryCreate("042100005264");
        var (_, ean13) = Barcode.TryCreate("5901234123457");
        var (_, code39) = Barcode.TryCreate("*TEST*");
        
        var obj = new 
        { 
            UpcA = upcA, 
            Ean13 = ean13, 
            Code39 = code39 
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var deserialized = JsonSerializer.Deserialize<MultipleBarcodesObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.UpcA.ShouldNotBeNull();
        deserialized.UpcA.Format.ShouldBe(BarcodeFormat.UpcA);
        deserialized.Ean13.ShouldNotBeNull();
        deserialized.Ean13.Format.ShouldBe(BarcodeFormat.Ean13);
        deserialized.Code39.ShouldNotBeNull();
        deserialized.Code39.Format.ShouldBe(BarcodeFormat.Code39);
    }

    private class TestObject
    {
        public Barcode ProductBarcode { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    private class MultipleBarcodesObject
    {
        public Barcode UpcA { get; set; } = null!;
        public Barcode Ean13 { get; set; } = null!;
        public Barcode Code39 { get; set; } = null!;
    }
}
