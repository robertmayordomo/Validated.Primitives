using System;
using System.Collections.Generic;
using System.Text.Json;
using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Domain.Serialization;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Serialization;

public class GeoBoundarySerializationTests
{
    private readonly JsonSerializerOptions _options;

    public GeoBoundarySerializationTests()
    {
        _options = new JsonSerializerOptions
        {
            Converters = { new GeoBoundaryConverter() },
            WriteIndented = true
        };
    }

    [Fact]
    public void Serialize_GeoBoundary_Should_Include_All_Properties()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);

        // Assert
        json.ShouldContain("\"Vertices\"");
        json.ShouldContain("\"AreaSquareKilometers\"");
        json.ShouldContain("\"AreaSquareMiles\"");
        json.ShouldContain("\"AreaSquareMeters\"");
        json.ShouldContain("\"PerimeterKilometers\"");
        json.ShouldContain("\"PerimeterMiles\"");
        json.ShouldContain("\"Center\"");
    }

    [Fact]
    public void Deserialize_GeoBoundary_Should_Recreate_Object()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, original) = GeoBoundary.TryCreate(vertices);
        var json = JsonSerializer.Serialize(original, _options);

        // Act
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Vertices.Count.ShouldBe(original!.Vertices.Count);
        deserialized.AreaSquareKilometers.ShouldBe(original.AreaSquareKilometers, tolerance: 0.001);
        deserialized.PerimeterKilometers.ShouldBe(original.PerimeterKilometers, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_Then_Deserialize_Should_Preserve_Vertices()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.7128m, -74.0060m).Value!,
            Coordinate.TryCreate(40.7484m, -73.9857m).Value!,
            Coordinate.TryCreate(40.7580m, -73.9855m).Value!
        };

        var (_, original) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized!.Vertices.Count.ShouldBe(3);
        deserialized.Vertices[0].Latitude.Value.ShouldBe(40.7128m);
        deserialized.Vertices[0].Longitude.Value.ShouldBe(-74.0060m);
        deserialized.Vertices[1].Latitude.Value.ShouldBe(40.7484m);
        deserialized.Vertices[2].Latitude.Value.ShouldBe(40.7580m);
    }

    [Fact]
    public void Serialize_Then_Deserialize_Should_Recalculate_Area_And_Perimeter()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, original) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert - Values should match (recalculated)
        deserialized!.AreaSquareKilometers.ShouldBe(original!.AreaSquareKilometers, tolerance: 0.001);
        deserialized.AreaSquareMiles.ShouldBe(original.AreaSquareMiles, tolerance: 0.001);
        deserialized.PerimeterKilometers.ShouldBe(original.PerimeterKilometers, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_Triangle_Boundary_Should_Work()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(40.5m, -73.5m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized!.Vertices.Count.ShouldBe(3);
        deserialized.AreaSquareKilometers.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Serialize_Pentagon_Boundary_Should_Work()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(40.5m, -74.5m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(40.8m, -73.0m).Value!,
            Coordinate.TryCreate(40.2m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized!.Vertices.Count.ShouldBe(5);
        deserialized.AreaSquareKilometers.ShouldBe(boundary!.AreaSquareKilometers, tolerance: 0.001);
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized.ShouldBeNull();
    }

    [Fact]
    public void Deserialize_Invalid_Json_Should_Throw()
    {
        // Arrange
        var json = "{ \"Invalid\": true }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeoBoundary>(json, _options));
    }

    [Fact]
    public void Deserialize_Without_Vertices_Should_Throw()
    {
        // Arrange
        var json = @"{ ""AreaSquareKilometers"": 1000 }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeoBoundary>(json, _options));
    }

    [Fact]
    public void Deserialize_With_Less_Than_3_Vertices_Should_Throw()
    {
        // Arrange
        var json = @"{
            ""Vertices"": [
                { ""Latitude"": { ""Value"": 40.0, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0, ""DecimalPlaces"": 6 } },
                { ""Latitude"": { ""Value"": 41.0, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0, ""DecimalPlaces"": 6 } }
            ]
        }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeoBoundary>(json, _options));
    }

    [Fact]
    public void Serialize_Boundary_With_Center_Should_Include_Center()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized!.Center.ShouldNotBeNull();
        deserialized.Center!.Latitude.Value.ShouldBe(boundary!.Center!.Latitude.Value);
        deserialized.Center.Longitude.Value.ShouldBe(boundary.Center.Longitude.Value);
    }

    [Fact]
    public void Serialize_Collection_Of_Boundaries_Should_Work()
    {
        // Arrange
        var boundary1Vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(40.5m, -73.5m).Value!
        };

        var boundary2Vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(45.0m, -70.0m).Value!,
            Coordinate.TryCreate(46.0m, -70.0m).Value!,
            Coordinate.TryCreate(46.0m, -69.0m).Value!,
            Coordinate.TryCreate(45.0m, -69.0m).Value!
        };

        var boundaries = new List<GeoBoundary>
        {
            GeoBoundary.TryCreate(boundary1Vertices).Value!,
            GeoBoundary.TryCreate(boundary2Vertices).Value!
        };

        // Act
        var json = JsonSerializer.Serialize(boundaries, _options);
        var deserialized = JsonSerializer.Deserialize<List<GeoBoundary>>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Count.ShouldBe(2);
        deserialized[0].Vertices.Count.ShouldBe(3);
        deserialized[1].Vertices.Count.ShouldBe(4);
    }

    [Fact]
    public void Serialize_Boundary_Should_Include_All_Area_Units()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);

        // Assert - Verify all units are present in JSON
        var document = JsonSerializer.Deserialize<JsonDocument>(json);
        document!.RootElement.TryGetProperty("AreaSquareKilometers", out _).ShouldBeTrue();
        document.RootElement.TryGetProperty("AreaSquareMiles", out _).ShouldBeTrue();
        document.RootElement.TryGetProperty("AreaSquareMeters", out _).ShouldBeTrue();
        document.RootElement.TryGetProperty("PerimeterKilometers", out _).ShouldBeTrue();
        document.RootElement.TryGetProperty("PerimeterMiles", out _).ShouldBeTrue();
    }

    [Fact]
    public void Serialize_Complex_Polygon_Should_Preserve_All_Vertices()
    {
        // Arrange - Create circular-ish boundary with many points
        var vertices = new List<Coordinate>();
        var centerLat = 40.0m;
        var centerLon = -74.0m;
        var radius = 0.5m; // degrees - reduced to stay within valid ranges

        for (int i = 0; i < 10; i++)
        {
            var angle = (i * 360.0m / 10.0m) * (decimal)Math.PI / 180.0m;
            var lat = Math.Round(centerLat + radius * (decimal)Math.Sin((double)angle), 6);
            var lon = Math.Round(centerLon + radius * (decimal)Math.Cos((double)angle), 6);
            
            var (coordResult, coord) = Coordinate.TryCreate(lat, lon, decimalPlaces: 6);
            coordResult.IsValid.ShouldBeTrue($"Failed to create coordinate at angle {i * 36}°");
            vertices.Add(coord!);
        }

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized!.Vertices.Count.ShouldBe(10);
        for (int i = 0; i < 10; i++)
        {
            deserialized.Vertices[i].Latitude.Value.ShouldBe(vertices[i].Latitude.Value, tolerance: 0.000001m);
            deserialized.Vertices[i].Longitude.Value.ShouldBe(vertices[i].Longitude.Value, tolerance: 0.000001m);
        }
    }

    [Fact]
    public void Serialize_Boundary_With_Altitude_Should_Preserve_Altitude()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m, altitude: 10m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m, altitude: 20m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m, altitude: 30m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var json = JsonSerializer.Serialize(boundary, _options);
        var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json, _options);

        // Assert
        deserialized!.Vertices[0].Altitude.ShouldBe(10m);
        deserialized.Vertices[1].Altitude.ShouldBe(20m);
        deserialized.Vertices[2].Altitude.ShouldBe(30m);
    }
}
