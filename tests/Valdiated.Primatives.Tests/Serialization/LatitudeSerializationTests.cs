using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects.Geospatial;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class LatitudeSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_Latitude_Should_Write_Value_And_DecimalPlaces()
    {
        // Arrange
        var (_, latitude) = Latitude.TryCreate(40.7128m, 6);

        // Act
        var json = JsonSerializer.Serialize(latitude, _options);

        // Assert
        json.ShouldContain("\"value\":");
        json.ShouldContain("\"decimalPlaces\":");
        json.ShouldContain("40.7128");
        json.ShouldContain("6");
    }

    [Fact]
    public void Deserialize_Latitude_Should_Read_From_JSON()
    {
        // Arrange
        var json = "{\"value\":40.7128,\"decimalPlaces\":6}";

        // Act
        var latitude = JsonSerializer.Deserialize<Latitude>(json, _options);

        // Assert
        latitude.ShouldNotBeNull();
        latitude.Value.ShouldBe(40.7128m);
        latitude.DecimalPlaces.ShouldBe(6);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = Latitude.TryCreate(-33.8688m, 4);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Latitude>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.DecimalPlaces.ShouldBe(original.DecimalPlaces);
        deserialized.ShouldBe(original);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 0)]
    [InlineData(-90, 0)]
    [InlineData(40.7128, 4)]
    [InlineData(-33.8688, 6)]
    public void Deserialize_Should_Preserve_Value_And_DecimalPlaces(decimal value, int decimalPlaces)
    {
        // Arrange
        var json = $"{{\"value\":{value},\"decimalPlaces\":{decimalPlaces}}}";

        // Act
        var latitude = JsonSerializer.Deserialize<Latitude>(json, _options);

        // Assert
        latitude.ShouldNotBeNull();
        latitude.Value.ShouldBe(value);
        latitude.DecimalPlaces.ShouldBe(decimalPlaces);
    }

    [Fact]
    public void Deserialize_Invalid_Latitude_Should_Throw_JsonException()
    {
        // Arrange - latitude value out of range
        var json = "{\"value\":100,\"decimalPlaces\":6}";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Latitude>(json, _options));
    }

    [Fact]
    public void Deserialize_Invalid_DecimalPlaces_Should_Throw_JsonException()
    {
        // Arrange - invalid decimal places
        var json = "{\"value\":40.7128,\"decimalPlaces\":10}";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Latitude>(json, _options));
    }

    [Fact]
    public void Serialize_Null_Latitude_Should_Write_Null()
    {
        // Arrange
        Latitude? latitude = null;

        // Act
        var json = JsonSerializer.Serialize(latitude, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var latitude = JsonSerializer.Deserialize<Latitude>(json, _options);

        // Assert
        latitude.ShouldBeNull();
    }

    [Fact]
    public void Serialize_Latitude_In_Object_Should_Work()
    {
        // Arrange
        var (_, latitude) = Latitude.TryCreate(40.7128m, 6);
        var obj = new { LocationLatitude = latitude, Name = "New York" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"locationLatitude\":");
        json.ShouldContain("\"name\":\"New York\"");
        json.ShouldContain("40.7128");
    }

    [Fact]
    public void Deserialize_Latitude_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"locationLatitude\":{\"value\":40.7128,\"decimalPlaces\":6},\"name\":\"New York\"}";

        // Act
        var obj = JsonSerializer.Deserialize<LocationObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.LocationLatitude.ShouldNotBeNull();
        obj.LocationLatitude.Value.ShouldBe(40.7128m);
        obj.LocationLatitude.DecimalPlaces.ShouldBe(6);
        obj.Name.ShouldBe("New York");
    }

    [Fact]
    public void Serialize_Multiple_Latitudes_In_Object_Should_Work()
    {
        // Arrange
        var (_, northLatitude) = Latitude.TryCreate(40.7128m, 6);
        var (_, southLatitude) = Latitude.TryCreate(-33.8688m, 4);

        var obj = new
        {
            North = northLatitude,
            South = southLatitude
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var deserialized = JsonSerializer.Deserialize<MultipleLatitudesObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.North.ShouldNotBeNull();
        deserialized.North.Value.ShouldBe(40.7128m);
        deserialized.North.DecimalPlaces.ShouldBe(6);
        deserialized.South.ShouldNotBeNull();
        deserialized.South.Value.ShouldBe(-33.8688m);
        deserialized.South.DecimalPlaces.ShouldBe(4);
    }

    [Fact]
    public void Serialize_Boundary_Values_Should_Work()
    {
        // Arrange
        var (_, minLatitude) = Latitude.TryCreate(-90m, 0);
        var (_, maxLatitude) = Latitude.TryCreate(90m, 0);

        // Act
        var minJson = JsonSerializer.Serialize(minLatitude, _options);
        var maxJson = JsonSerializer.Serialize(maxLatitude, _options);

        var deserializedMin = JsonSerializer.Deserialize<Latitude>(minJson, _options);
        var deserializedMax = JsonSerializer.Deserialize<Latitude>(maxJson, _options);

        // Assert
        deserializedMin.ShouldNotBeNull();
        deserializedMin.Value.ShouldBe(-90m);
        deserializedMax.ShouldNotBeNull();
        deserializedMax.Value.ShouldBe(90m);
    }

    [Fact]
    public void Serialize_Zero_Latitude_Should_Work()
    {
        // Arrange
        var (_, latitude) = Latitude.TryCreate(0m, 2);

        // Act
        var json = JsonSerializer.Serialize(latitude, _options);
        var deserialized = JsonSerializer.Deserialize<Latitude>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(0m);
        deserialized.DecimalPlaces.ShouldBe(2);
    }

    private class LocationObject
    {
        public Latitude LocationLatitude { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    private class MultipleLatitudesObject
    {
        public Latitude North { get; set; } = null!;
        public Latitude South { get; set; } = null!;
    }
}
