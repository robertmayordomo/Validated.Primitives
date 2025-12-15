using System.Collections.Generic;
using System.Text.Json;
using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Domain.Serialization;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Serialization;

public class GeospatialRouteSerializationTests
{
    private readonly JsonSerializerOptions _options;

    public GeospatialRouteSerializationTests()
    {
        _options = new JsonSerializerOptions
        {
            Converters = 
            { 
                new GeospatialRouteConverter(),
                new GeoDistanceConverter()
            },
            WriteIndented = true
        };
    }

    [Fact]
    public void Serialize_GeospatialRoute_Should_Include_All_Properties()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(point1, point2);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! }, "Test Route");

        // Act
        var json = JsonSerializer.Serialize(route, _options);

        // Assert
        json.ShouldContain("\"Name\"");
        json.ShouldContain("\"Segments\"");
        json.ShouldContain("\"TotalDistanceKilometers\"");
        json.ShouldContain("\"TotalDistanceMiles\"");
        json.ShouldContain("\"TotalDistanceMeters\"");
        json.ShouldContain("\"StartingPoint\"");
        json.ShouldContain("\"EndingPoint\"");
    }

    [Fact]
    public void Deserialize_GeospatialRoute_Should_Recreate_Object()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(point1, point2, "Segment 1");
        var (_, original) = GeospatialRoute.TryCreate(new[] { segment! }, "NYC Route");
        var json = JsonSerializer.Serialize(original, _options);

        // Act
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Name.ShouldBe("NYC Route");
        deserialized.Segments.Count.ShouldBe(1);
        deserialized.TotalDistanceKilometers.ShouldBe(original!.TotalDistanceKilometers, tolerance: 0.001);
    }

    [Fact]
    public void Serialize_Then_Deserialize_Should_Preserve_All_Distances()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, original) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized!.TotalDistanceKilometers.ShouldBe(original!.TotalDistanceKilometers, tolerance: 0.001);
        deserialized.TotalDistanceMiles.ShouldBe(original.TotalDistanceMiles, tolerance: 0.001);
        deserialized.TotalDistanceMeters.ShouldBe(original.TotalDistanceMeters, tolerance: 1.0);
    }

    [Fact]
    public void Deserialize_Should_Validate_Segment_Contiguity()
    {
        // Arrange - Create JSON with non-contiguous segments
        var json = @"{
            ""Name"": ""Invalid Route"",
            ""Segments"": [
                {
                    ""From"": { ""Latitude"": { ""Value"": 40.7128, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0060, ""DecimalPlaces"": 6 } },
                    ""To"": { ""Latitude"": { ""Value"": 40.7484, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -73.9857, ""DecimalPlaces"": 6 } },
                    ""Distance"": {
                        ""From"": { ""Latitude"": { ""Value"": 40.7128, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0060, ""DecimalPlaces"": 6 } },
                        ""To"": { ""Latitude"": { ""Value"": 40.7484, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -73.9857, ""DecimalPlaces"": 6 } },
                        ""Kilometers"": 4.5
                    }
                },
                {
                    ""From"": { ""Latitude"": { ""Value"": 41.0000, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0000, ""DecimalPlaces"": 6 } },
                    ""To"": { ""Latitude"": { ""Value"": 41.1000, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.1000, ""DecimalPlaces"": 6 } },
                    ""Distance"": {
                        ""From"": { ""Latitude"": { ""Value"": 41.0000, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.0000, ""DecimalPlaces"": 6 } },
                        ""To"": { ""Latitude"": { ""Value"": 41.1000, ""DecimalPlaces"": 6 }, ""Longitude"": { ""Value"": -74.1000, ""DecimalPlaces"": 6 } },
                        ""Kilometers"": 3.0
                    }
                }
            ]
        }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeospatialRoute>(json, _options));
    }

    [Fact]
    public void Serialize_Route_Without_Name_Should_Work()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(point1, point2);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var json = JsonSerializer.Serialize(route, _options);
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Name.ShouldBeNull();
        deserialized.Segments.Count.ShouldBe(1);
    }

    [Fact]
    public void Serialize_Multi_Segment_Route_Should_Preserve_All_Segments()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);
        var (_, point4) = Coordinate.TryCreate(40.7829m, -73.9654m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2, "Leg 1");
        var (_, segment2) = RouteSegment.TryCreate(point2, point3, "Leg 2");
        var (_, segment3) = RouteSegment.TryCreate(point3, point4, "Leg 3");

        var (_, original) = GeospatialRoute.TryCreate(new[] { segment1!, segment2!, segment3! });

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized!.Segments.Count.ShouldBe(3);
        deserialized.Segments[0].Name.ShouldBe("Leg 1");
        deserialized.Segments[1].Name.ShouldBe("Leg 2");
        deserialized.Segments[2].Name.ShouldBe("Leg 3");
    }

    [Fact]
    public void Serialize_Route_Should_Preserve_Starting_And_Ending_Points()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(point1, point2);
        var (_, original) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized!.StartingPoint.ShouldNotBeNull();
        deserialized.StartingPoint!.Latitude.Value.ShouldBe(40.7128m);
        deserialized.StartingPoint.Longitude.Value.ShouldBe(-74.0060m);

        deserialized.EndingPoint.ShouldNotBeNull();
        deserialized.EndingPoint!.Latitude.Value.ShouldBe(40.7484m);
        deserialized.EndingPoint.Longitude.Value.ShouldBe(-73.9857m);
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized.ShouldBeNull();
    }

    [Fact]
    public void Deserialize_Without_Segments_Should_Throw()
    {
        // Arrange
        var json = @"{ ""Name"": ""Empty Route"" }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeospatialRoute>(json, _options));
    }

    [Fact]
    public void Deserialize_With_Empty_Segments_Array_Should_Throw()
    {
        // Arrange
        var json = @"{ ""Name"": ""Empty Route"", ""Segments"": [] }";

        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<GeospatialRoute>(json, _options));
    }

    [Fact]
    public void Serialize_Collection_Of_Routes_Should_Work()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);
        var (_, point4) = Coordinate.TryCreate(40.7829m, -73.9654m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point3, point4);

        var routes = new List<GeospatialRoute>
        {
            GeospatialRoute.TryCreate(new[] { segment1! }, "Route 1").Value!,
            GeospatialRoute.TryCreate(new[] { segment2! }, "Route 2").Value!
        };

        // Act
        var json = JsonSerializer.Serialize(routes, _options);
        var deserialized = JsonSerializer.Deserialize<List<GeospatialRoute>>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Count.ShouldBe(2);
        deserialized[0].Name.ShouldBe("Route 1");
        deserialized[1].Name.ShouldBe("Route 2");
    }

    [Fact]
    public void Serialize_Route_With_Coordinates_With_Altitude_Should_Preserve_Altitude()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 10m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m, altitude: 443m);
        var (_, segment) = RouteSegment.TryCreate(point1, point2);
        var (_, original) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert
        deserialized!.Segments[0].From.Altitude.ShouldBe(10m);
        deserialized.Segments[0].To.Altitude.ShouldBe(443m);
    }

    [Fact]
    public void Serialize_Route_Should_Recalculate_Total_Distance_On_Deserialize()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, original) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json, _options);

        // Assert - Total should be recalculated from segments
        var expectedTotal = deserialized!.Segments[0].Distance.Kilometers + 
                           deserialized.Segments[1].Distance.Kilometers;
        deserialized.TotalDistanceKilometers.ShouldBe(expectedTotal, tolerance: 0.001);
    }
}
