using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects.Geospatial;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class LongitudeSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_Longitude_Should_Write_Value_And_DecimalPlaces()
    {
        // Arrange
        var (_, longitude) = Longitude.TryCreate(-74.0060m, 6);

        // Act
        var json = JsonSerializer.Serialize(longitude, _options);

        // Assert
        json.ShouldContain("\"value\":");
        json.ShouldContain("\"decimalPlaces\":");
        json.ShouldContain("-74.0060");
        json.ShouldContain("6");
    }

    [Fact]
    public void Deserialize_Longitude_Should_Read_From_JSON()
    {
        // Arrange
        var json = "{\"value\":-74.0060,\"decimalPlaces\":6}";

        // Act
        var longitude = JsonSerializer.Deserialize<Longitude>(json, _options);

        // Assert
        longitude.ShouldNotBeNull();
        longitude.Value.ShouldBe(-74.0060m);
        longitude.DecimalPlaces.ShouldBe(6);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = Longitude.TryCreate(151.2093m, 4);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Longitude>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.DecimalPlaces.ShouldBe(original.DecimalPlaces);
        deserialized.ShouldBe(original);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(180, 0)]
    [InlineData(-180, 0)]
    [InlineData(-74.0060, 4)]
    [InlineData(151.2093, 6)]
    public void Deserialize_Should_Preserve_Value_And_DecimalPlaces(decimal value, int decimalPlaces)
    {
        // Arrange
        var json = $"{{\"value\":{value},\"decimalPlaces\":{decimalPlaces}}}";

        // Act
        var longitude = JsonSerializer.Deserialize<Longitude>(json, _options);

        // Assert
        longitude.ShouldNotBeNull();
        longitude.Value.ShouldBe(value);
        longitude.DecimalPlaces.ShouldBe(decimalPlaces);
    }

    [Fact]
    public void Deserialize_Invalid_Longitude_Should_Throw_JsonException()
    {
        // Arrange - longitude value out of range
        var json = "{\"value\":200,\"decimalPlaces\":6}";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Longitude>(json, _options));
    }

    [Fact]
    public void Deserialize_Invalid_DecimalPlaces_Should_Throw_JsonException()
    {
        // Arrange - invalid decimal places
        var json = "{\"value\":-74.0060,\"decimalPlaces\":10}";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Longitude>(json, _options));
    }

    [Fact]
    public void Serialize_Null_Longitude_Should_Write_Null()
    {
        // Arrange
        Longitude? longitude = null;

        // Act
        var json = JsonSerializer.Serialize(longitude, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var longitude = JsonSerializer.Deserialize<Longitude>(json, _options);

        // Assert
        longitude.ShouldBeNull();
    }

    [Fact]
    public void Serialize_Longitude_In_Object_Should_Work()
    {
        // Arrange
        var (_, longitude) = Longitude.TryCreate(-74.0060m, 6);
        var obj = new { LocationLongitude = longitude, Name = "New York" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"locationLongitude\":");
        json.ShouldContain("\"name\":\"New York\"");
        json.ShouldContain("-74.0060");
    }

    [Fact]
    public void Deserialize_Longitude_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"locationLongitude\":{\"value\":-74.0060,\"decimalPlaces\":6},\"name\":\"New York\"}";

        // Act
        var obj = JsonSerializer.Deserialize<LocationObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.LocationLongitude.ShouldNotBeNull();
        obj.LocationLongitude.Value.ShouldBe(-74.0060m);
        obj.LocationLongitude.DecimalPlaces.ShouldBe(6);
        obj.Name.ShouldBe("New York");
    }

    [Fact]
    public void Serialize_Multiple_Longitudes_In_Object_Should_Work()
    {
        // Arrange
        var (_, westLongitude) = Longitude.TryCreate(-74.0060m, 6);
        var (_, eastLongitude) = Longitude.TryCreate(151.2093m, 4);

        var obj = new
        {
            West = westLongitude,
            East = eastLongitude
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var deserialized = JsonSerializer.Deserialize<MultipleLongitudesObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.West.ShouldNotBeNull();
        deserialized.West.Value.ShouldBe(-74.0060m);
        deserialized.West.DecimalPlaces.ShouldBe(6);
        deserialized.East.ShouldNotBeNull();
        deserialized.East.Value.ShouldBe(151.2093m);
        deserialized.East.DecimalPlaces.ShouldBe(4);
    }

    [Fact]
    public void Serialize_Boundary_Values_Should_Work()
    {
        // Arrange
        var (_, minLongitude) = Longitude.TryCreate(-180m, 0);
        var (_, maxLongitude) = Longitude.TryCreate(180m, 0);

        // Act
        var minJson = JsonSerializer.Serialize(minLongitude, _options);
        var maxJson = JsonSerializer.Serialize(maxLongitude, _options);

        var deserializedMin = JsonSerializer.Deserialize<Longitude>(minJson, _options);
        var deserializedMax = JsonSerializer.Deserialize<Longitude>(maxJson, _options);

        // Assert
        deserializedMin.ShouldNotBeNull();
        deserializedMin.Value.ShouldBe(-180m);
        deserializedMax.ShouldNotBeNull();
        deserializedMax.Value.ShouldBe(180m);
    }

    [Fact]
    public void Serialize_Zero_Longitude_Should_Work()
    {
        // Arrange (Prime Meridian)
        var (_, longitude) = Longitude.TryCreate(0m, 2);

        // Act
        var json = JsonSerializer.Serialize(longitude, _options);
        var deserialized = JsonSerializer.Deserialize<Longitude>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(0m);
        deserialized.DecimalPlaces.ShouldBe(2);
    }

    [Fact]
    public void Serialize_Coordinates_With_Latitude_And_Longitude_Should_Work()
    {
        // Arrange
        var (_, latitude) = Latitude.TryCreate(40.7128m, 6);
        var (_, longitude) = Longitude.TryCreate(-74.0060m, 6);
        var coordinates = new { Latitude = latitude, Longitude = longitude, Name = "New York City" };

        // Act
        var json = JsonSerializer.Serialize(coordinates, _options);
        var deserialized = JsonSerializer.Deserialize<CoordinatesObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Latitude.ShouldNotBeNull();
        deserialized.Latitude.Value.ShouldBe(40.7128m);
        deserialized.Longitude.ShouldNotBeNull();
        deserialized.Longitude.Value.ShouldBe(-74.0060m);
        deserialized.Name.ShouldBe("New York City");
    }

    private class LocationObject
    {
        public Longitude LocationLongitude { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    private class MultipleLongitudesObject
    {
        public Longitude West { get; set; } = null!;
        public Longitude East { get; set; } = null!;
    }

    private class CoordinatesObject
    {
        public Latitude Latitude { get; set; } = null!;
        public Longitude Longitude { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
