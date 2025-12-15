using System;
using System.Collections.Generic;
using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Validation;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Geospatial;

public class GeoBoundaryTests
{
    [Fact]
    public void TryCreate_With_Valid_Triangle_Should_Succeed()
    {
        // Arrange - Triangle around New York
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.7128m, -74.0060m).Value!, // NYC
            Coordinate.TryCreate(40.7484m, -73.9857m).Value!, // Empire State
            Coordinate.TryCreate(40.7580m, -73.9855m).Value!  // Times Square
        };

        // Act
        var (result, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        result.IsValid.ShouldBeTrue();
        boundary.ShouldNotBeNull();
        boundary!.Vertices.Count.ShouldBe(3);
        boundary.AreaSquareKilometers.ShouldBeGreaterThan(0);
        boundary.PerimeterKilometers.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void TryCreate_With_Valid_Rectangle_Should_Succeed()
    {
        // Arrange - Rectangle
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        // Act
        var (result, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        result.IsValid.ShouldBeTrue();
        boundary.ShouldNotBeNull();
        boundary!.Vertices.Count.ShouldBe(4);
        boundary.AreaSquareKilometers.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void TryCreate_With_Less_Than_3_Vertices_Should_Fail()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.7128m, -74.0060m).Value!,
            Coordinate.TryCreate(40.7484m, -73.9857m).Value!
        };

        // Act
        var (result, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        result.IsValid.ShouldBeFalse();
        boundary.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InsufficientVertices");
    }

    [Fact]
    public void TryCreate_With_Null_Vertices_Should_Fail()
    {
        // Act
        var (result, boundary) = GeoBoundary.TryCreate(null!);

        // Assert
        result.IsValid.ShouldBeFalse();
        boundary.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Vertices");
    }

    [Fact]
    public void TryCreate_With_Null_Vertex_In_Collection_Should_Fail()
    {
        // Arrange
        var vertices = new List<Coordinate?>
        {
            Coordinate.TryCreate(40.7128m, -74.0060m).Value,
            null,
            Coordinate.TryCreate(40.7580m, -73.9855m).Value
        };

        // Act
        var (result, boundary) = GeoBoundary.TryCreate(vertices!);

        // Assert
        result.IsValid.ShouldBeFalse();
        boundary.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidVertex");
    }

    [Fact]
    public void Contains_Should_Return_True_For_Point_Inside_Boundary()
    {
        // Arrange - Square boundary
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Point clearly inside
        var (_, insidePoint) = Coordinate.TryCreate(40.5m, -73.5m);

        // Act
        var isInside = boundary!.Contains(insidePoint!);

        // Assert
        isInside.ShouldBeTrue();
    }

    [Fact]
    public void Contains_Should_Return_False_For_Point_Outside_Boundary()
    {
        // Arrange - Square boundary
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Point clearly outside
        var (_, outsidePoint) = Coordinate.TryCreate(42.0m, -75.0m);

        // Act
        var isOutside = boundary!.Contains(outsidePoint!);

        // Assert
        isOutside.ShouldBeFalse();
    }

    [Fact]
    public void Contains_Should_Return_False_For_Null_Point()
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
        var result = boundary!.Contains(null!);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Center_Should_Be_Calculated_Correctly()
    {
        // Arrange - Square centered around 40.5, -73.5
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!,
            Coordinate.TryCreate(40.0m, -73.0m).Value!
        };

        // Act
        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        boundary!.Center.ShouldNotBeNull();
        boundary.Center!.Latitude.Value.ShouldBe(40.5m);
        boundary.Center.Longitude.Value.ShouldBe(-73.5m);
    }

    [Fact]
    public void GetBoundingBox_Should_Return_Correct_Min_Max()
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
        var (minLat, maxLat, minLon, maxLon) = boundary!.GetBoundingBox();

        // Assert
        minLat.ShouldBe(40.0m);
        maxLat.ShouldBe(41.0m);
        minLon.ShouldBe(-74.0m);
        maxLon.ShouldBe(-73.0m);
    }

    [Fact]
    public void AreaSquareMiles_Should_Convert_Correctly()
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

        // Act & Assert
        (boundary!.AreaSquareMiles / boundary.AreaSquareKilometers).ShouldBe(0.386102, tolerance: 0.0001);
    }

    [Fact]
    public void AreaSquareMeters_Should_Convert_Correctly()
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

        // Act & Assert
        boundary!.AreaSquareMeters.ShouldBe(boundary.AreaSquareKilometers * 1_000_000);
    }

    [Fact]
    public void PerimeterMiles_Should_Convert_Correctly()
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

        // Act & Assert
        (boundary!.PerimeterMiles / boundary.PerimeterKilometers).ShouldBe(0.621371, tolerance: 0.0001);
    }

    [Fact]
    public void DistanceToNearestEdge_Should_Return_Distance_To_Boundary()
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

        // Point outside the boundary
        var (_, outsidePoint) = Coordinate.TryCreate(42.0m, -74.0m);

        // Act
        var distance = boundary!.DistanceToNearestEdge(outsidePoint!);

        // Assert
        distance.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void GetDescription_Should_Return_Formatted_String()
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
        var description = boundary!.GetDescription();

        // Assert
        description.ShouldContain("Polygon");
        description.ShouldContain("4 vertices");
        description.ShouldContain("km²");
        description.ShouldContain("mi²");
    }

    [Fact]
    public void ToString_Should_Return_Description()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Act
        var result = boundary!.ToString();

        // Assert
        result.ShouldBe(boundary.GetDescription());
    }

    [Fact]
    public void Complex_Polygon_Should_Calculate_Area_And_Perimeter()
    {
        // Arrange - Pentagon
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(40.5m, -74.5m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(40.8m, -73.0m).Value!,
            Coordinate.TryCreate(40.2m, -73.0m).Value!
        };

        // Act
        var (result, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        result.IsValid.ShouldBeTrue();
        boundary!.Vertices.Count.ShouldBe(5);
        boundary.AreaSquareKilometers.ShouldBeGreaterThan(0);
        boundary.PerimeterKilometers.ShouldBeGreaterThan(0);
        boundary.Center.ShouldNotBeNull();
    }

    [Fact]
    public void Large_Boundary_Should_Handle_Many_Vertices()
    {
        // Arrange - Create a circular-ish boundary with many points
        var vertices = new List<Coordinate>();
        var centerLat = 40.0m;
        var centerLon = -74.0m;
        var radius = 0.5m; // degrees - reduced to stay within valid lat/long ranges

        for (int i = 0; i < 20; i++)
        {
            var angle = (i * 360.0m / 20.0m) * (decimal)Math.PI / 180.0m;
            var lat = Math.Round(centerLat + radius * (decimal)Math.Sin((double)angle), 6);
            var lon = Math.Round(centerLon + radius * (decimal)Math.Cos((double)angle), 6);
            
            var (coordResult, coord) = Coordinate.TryCreate(lat, lon, decimalPlaces: 6);
            coordResult.IsValid.ShouldBeTrue($"Failed to create coordinate at angle {i * 18}°: {coordResult.ToBulletList()}");
            vertices.Add(coord!);
        }

        // Act
        var (result, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        result.IsValid.ShouldBeTrue();
        boundary!.Vertices.Count.ShouldBe(20);
        boundary.AreaSquareKilometers.ShouldBeGreaterThan(0);
        
        // Center point should be inside
        var (_, centerPoint) = Coordinate.TryCreate(centerLat, centerLon);
        boundary.Contains(centerPoint!).ShouldBeTrue();
    }

    [Fact]
    public void Vertices_Should_Be_ReadOnly()
    {
        // Arrange
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -74.0m).Value!,
            Coordinate.TryCreate(41.0m, -73.0m).Value!
        };

        var (_, boundary) = GeoBoundary.TryCreate(vertices);

        // Assert
        boundary!.Vertices.ShouldBeOfType<System.Collections.ObjectModel.ReadOnlyCollection<Coordinate>>();
    }

    [Fact]
    public void Real_World_Example_NYC_Borough_Should_Work()
    {
        // Arrange - Simplified Manhattan boundary (very rough approximation)
        var vertices = new List<Coordinate>
        {
            Coordinate.TryCreate(40.7009m, -74.0165m).Value!, // Battery Park
            Coordinate.TryCreate(40.7484m, -74.0048m).Value!, // Midtown West
            Coordinate.TryCreate(40.7829m, -73.9654m).Value!, // Central Park North
            Coordinate.TryCreate(40.8088m, -73.9482m).Value!, // Harlem
            Coordinate.TryCreate(40.7614m, -73.9776m).Value!, // Upper East Side
            Coordinate.TryCreate(40.7489m, -73.9680m).Value!, // Midtown East
            Coordinate.TryCreate(40.7061m, -73.9969m).Value!  // Lower East Side
        };

        // Act
        var (result, manhattan) = GeoBoundary.TryCreate(vertices);

        // Assert
        result.IsValid.ShouldBeTrue();
        manhattan.ShouldNotBeNull();

        // Times Square should be inside Manhattan
        var (_, timesSquare) = Coordinate.TryCreate(40.7580m, -73.9855m);
        manhattan!.Contains(timesSquare!).ShouldBeTrue();

        // Brooklyn should be outside
        var (_, brooklyn) = Coordinate.TryCreate(40.6782m, -73.9442m);
        manhattan.Contains(brooklyn!).ShouldBeFalse();
    }
}
