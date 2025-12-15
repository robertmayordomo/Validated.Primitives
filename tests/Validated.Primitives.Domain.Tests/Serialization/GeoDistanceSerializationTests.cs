using System.Collections.Generic;
using System.Text.Json;
using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Domain.Serialization;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Serialization;

public class GeoDistanceSerializationTests
{
    private readonly JsonSerializerOptions _options;

    public GeoDistanceSerializationTests()
    {
        _options = new JsonSerializerOptions
        {
            Converters = { new GeoDistanceConverter() },
            WriteIndented = true
        };
    }

    [Fact]
    public void Serialize_GeoDistance_Should_Include_All_Properties()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(distance, _options);

        // Assert
        json.ShouldContain("\"From\"");
        json.ShouldContain("\"To\"");
        json.ShouldContain("\"Kilometers\"");
        json.ShouldContain("\"Miles\"");
        json.ShouldContain("\"Meters\"");
        json.ShouldContain("\"NauticalMiles\"");
    }

    [Fact]
    public void Deserialize_GeoDistance_Should_Recreate_Object()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, original) = GeoDistance.TryCreate(from!, to!);
        var json = JsonSerializer.Serialize(original, _options);

        // Act
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.From.Latitude.Value.ShouldBe(original!.From.Latitude.Value);
        deserialized.From.Longitude.Value.ShouldBe(original.From.Longitude.Value);
        deserialized.To.Latitude.Value.ShouldBe(original.To.Latitude.Value);
        deserialized.To.Longitude.Value.ShouldBe(original.To.Longitude.Value);
        deserialized.Kilometers.ShouldBe(original.Kilometers, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_Then_Deserialize_Should_Preserve_Distance()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, original) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized!.Kilometers.ShouldBe(original!.Kilometers, tolerance: 0.001);
        deserialized.Miles.ShouldBe(original.Miles, tolerance: 0.001);
        deserialized.Meters.ShouldBe(original.Meters, tolerance: 1.0);
        deserialized.NauticalMiles.ShouldBe(original.NauticalMiles, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_GeoDistance_With_Short_Distance_Should_Work()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, to) = Coordinate.TryCreate(40.7580m, -73.9855m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(distance, _options);
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Kilometers.ShouldBe(distance!.Kilometers, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_GeoDistance_With_Long_Distance_Should_Work()
    {
        // Arrange - New York to London
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(51.5074m, -0.1278m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(distance, _options);
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Kilometers.ShouldBe(distance!.Kilometers, tolerance: 0.001);
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized.ShouldBeNull();
    }

    [Fact]
    public void Deserialize_Invalid_Json_Should_Throw()
    {
        // Arrange
        var json = "{ \"Invalid\": true }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeoDistance>(json, _options));
    }

    [Fact]
    public void Deserialize_Without_From_Should_Throw()
    {
        // Arrange
        var json = @"{ ""To"": { ""Latitude"": { ""Value"": 34.0522, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -118.2437, ""DecimalPlaces"": 6 } } }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeoDistance>(json, _options));
    }

    [Fact]
    public void Deserialize_Without_To_Should_Throw()
    {
        // Arrange
        var json = @"{ ""From"": { ""Latitude"": { ""Value"": 40.7128, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0060, ""DecimalPlaces"": 6 } } }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeoDistance>(json, _options));
    }

    [Fact]
    public void Serialize_GeoDistance_Should_Include_All_Unit_Conversions()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(distance, _options);

        // Assert - Verify all units are present in JSON
        var deserialized = JsonSerializer.Deserialize<JsonDocument>(json);
        deserialized!.RootElement.TryGetProperty("Kilometers", out _).ShouldBeTrue();
        deserialized.RootElement.TryGetProperty("Miles", out _).ShouldBeTrue();
        deserialized.RootElement.TryGetProperty("Meters", out _).ShouldBeTrue();
        deserialized.RootElement.TryGetProperty("NauticalMiles", out _).ShouldBeTrue();
    }

    [Fact]
    public void Serialize_GeoDistance_With_Altitude_Should_Preserve_Coordinates()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 10m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m, altitude: 100m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(distance, _options);
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized!.From.Altitude.ShouldBe(10m);
        deserialized.To.Altitude.ShouldBe(100m);
    }

    [Fact]
    public void Serialize_Collection_Of_GeoDistances_Should_Work()
    {
        // Arrange
        var (_, ny) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, la) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, chicago) = Coordinate.TryCreate(41.8781m, -87.6298m);

        var distances = new List<GeoDistance>
        {
            GeoDistance.TryCreate(ny!, la!).Value!,
            GeoDistance.TryCreate(ny!, chicago!).Value!,
            GeoDistance.TryCreate(la!, chicago!).Value!
        };

        // Act
        var json = JsonSerializer.Serialize(distances, _options);
        var deserialized = JsonSerializer.Deserialize<List<GeoDistance>>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Count.ShouldBe(3);
        deserialized[0].Kilometers.ShouldBe(distances[0].Kilometers, tolerance: 0.001);
        deserialized[1].Kilometers.ShouldBe(distances[1].Kilometers, tolerance: 0.001);
        deserialized[2].Kilometers.ShouldBe(distances[2].Kilometers, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_GeoDistance_With_Zero_Distance_Should_Work()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var json = JsonSerializer.Serialize(distance, _options);
        var deserialized = JsonSerializer.Deserialize<GeoDistance>(json, _options);

        // Assert
        deserialized!.Kilometers.ShouldBe(0, tolerance: 0.001);
        deserialized.Miles.ShouldBe(0, tolerance: 0.001);
    }
}
