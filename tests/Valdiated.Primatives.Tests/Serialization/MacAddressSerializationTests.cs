using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class MacAddressSerializationTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public void Serialize_MacAddress_SerializesAsString()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");
        var obj = new { NetworkCard = mac };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"networkCard\":\"AA:BB:CC:DD:EE:FF\"");
    }

    [Fact]
    public void Deserialize_StringToMacAddress_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"networkCard\":\"AA:BB:CC:DD:EE:FF\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.NetworkCard.ShouldNotBeNull();
        obj.NetworkCard.Value.ShouldBe("AA:BB:CC:DD:EE:FF");
    }

    [Fact]
    public void Deserialize_DifferentFormat_NormalizesCorrectly()
    {
        // Arrange
        var json = "{\"networkCard\":\"AA-BB-CC-DD-EE-FF\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.NetworkCard.ShouldNotBeNull();
        obj.NetworkCard.Value.ShouldBe("AA:BB:CC:DD:EE:FF");
    }

    [Fact]
    public void Deserialize_InvalidMacAddress_ThrowsException()
    {
        // Arrange
        var json = "{\"networkCard\":\"INVALID\"}";

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<TestObject>(json, _options));
    }

    [Fact]
    public void Deserialize_NullMacAddress_ReturnsNull()
    {
        // Arrange
        var json = "{\"networkCard\":null}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObjectNullable>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.NetworkCard.ShouldBeNull();
    }

    [Fact]
    public void RoundTrip_PreservesValue()
    {
        // Arrange
        var (_, original) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");
        var obj = new TestObject { NetworkCard = original! };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var deserialized = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.NetworkCard.ShouldBe(original);
    }

    [Fact]
    public void Serialize_Array_SerializesCorrectly()
    {
        // Arrange
        var (_, mac1) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");
        var (_, mac2) = MacAddress.TryCreate("12:34:56:78:9A:BC"); // Changed to valid unicast
        var obj = new { Devices = new[] { mac1, mac2 } };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"devices\":[\"AA:BB:CC:DD:EE:FF\",\"12:34:56:78:9A:BC\"]");
    }

    private class TestObject
    {
        public MacAddress NetworkCard { get; set; } = null!;
    }

    private class TestObjectNullable
    {
        public MacAddress? NetworkCard { get; set; }
    }
}
