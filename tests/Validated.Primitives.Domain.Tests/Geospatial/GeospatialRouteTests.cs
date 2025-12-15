using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Validation;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Geospatial;

public class GeospatialRouteTests
{
    [Fact]
    public void TryCreate_With_Single_Segment_Should_Succeed()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var segments = new[] { segment! };

        // Act
        var (result, route) = GeospatialRoute.TryCreate(segments);

        // Assert
        result.IsValid.ShouldBeTrue();
        route.ShouldNotBeNull();
        route!.Segments.Count.ShouldBe(1);
        route.TotalDistanceKilometers.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void TryCreate_With_Multiple_Contiguous_Segments_Should_Succeed()
    {
        // Arrange - Create a 3-segment route through NYC
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m); // Lower Manhattan
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m); // Empire State
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m); // Times Square
        var (_, point4) = Coordinate.TryCreate(40.7829m, -73.9654m); // Central Park

        var (_, segment1) = RouteSegment.TryCreate(point1, point2, "To Empire State");
        var (_, segment2) = RouteSegment.TryCreate(point2, point3, "To Times Square");
        var (_, segment3) = RouteSegment.TryCreate(point3, point4, "To Central Park");

        var segments = new[] { segment1!, segment2!, segment3! };

        // Act
        var (result, route) = GeospatialRoute.TryCreate(segments);

        // Assert
        result.IsValid.ShouldBeTrue();
        route.ShouldNotBeNull();
        route!.Segments.Count.ShouldBe(3);
        route.TotalDistanceKilometers.ShouldBeGreaterThan(0);
        route.StartingPoint.ShouldBe(point1);
        route.EndingPoint.ShouldBe(point4);
    }

    [Fact]
    public void TryCreate_With_Non_Contiguous_Segments_Should_Fail()
    {
        // Arrange - Create segments with gap
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);
        var (_, point4) = Coordinate.TryCreate(40.7829m, -73.9654m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        // Gap - segment2 should start at point2 but starts at point3
        var (_, segment2) = RouteSegment.TryCreate(point3, point4);

        var segments = new[] { segment1!, segment2! };

        // Act
        var (result, route) = GeospatialRoute.TryCreate(segments);

        // Assert
        result.IsValid.ShouldBeFalse();
        route.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "NonContiguousSegments");
    }

    [Fact]
    public void TryCreate_With_Empty_Segments_Should_Fail()
    {
        // Arrange
        var segments = Array.Empty<RouteSegment>();

        // Act
        var (result, route) = GeospatialRoute.TryCreate(segments);

        // Assert
        result.IsValid.ShouldBeFalse();
        route.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InsufficientSegments");
    }

    [Fact]
    public void TryCreate_With_Null_Segments_Should_Fail()
    {
        // Act
        var (result, route) = GeospatialRoute.TryCreate(null);

        // Assert
        result.IsValid.ShouldBeFalse();
        route.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Fact]
    public void TryCreate_With_Null_Segment_In_Collection_Should_Fail()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment1) = RouteSegment.TryCreate(from, to);

        var segments = new[] { segment1!, null! };

        // Act
        var (result, route) = GeospatialRoute.TryCreate(segments);

        // Assert
        result.IsValid.ShouldBeFalse();
        route.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidSegment");
    }

    [Fact]
    public void TryCreate_With_Name_Should_Set_Name()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var name = "NYC Tour Route";

        // Act
        var (result, route) = GeospatialRoute.TryCreate(new[] { segment! }, name);

        // Assert
        result.IsValid.ShouldBeTrue();
        route!.Name.ShouldBe(name);
    }

    [Fact]
    public void TotalDistanceKilometers_Should_Sum_All_Segment_Distances()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var segments = new[] { segment1!, segment2! };

        // Act
        var (result, route) = GeospatialRoute.TryCreate(segments);

        // Assert
        result.IsValid.ShouldBeTrue();
        var expectedTotal = segment1!.Distance.Kilometers + segment2!.Distance.Kilometers;
        route!.TotalDistanceKilometers.ShouldBe(expectedTotal, tolerance: 0.001);
    }

    [Fact]
    public void TotalDistanceMiles_Should_Convert_Kilometers()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var miles = route!.TotalDistanceMiles;

        // Assert
        (miles / route.TotalDistanceKilometers).ShouldBe(0.621371, tolerance: 0.0001);
    }

    [Fact]
    public void TotalDistanceMeters_Should_Convert_Kilometers()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var meters = route!.TotalDistanceMeters;

        // Assert
        meters.ShouldBe(route.TotalDistanceKilometers * 1000);
    }

    [Fact]
    public void StartingPoint_Should_Be_First_Segment_From()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Assert
        route!.StartingPoint.ShouldBe(point1);
    }

    [Fact]
    public void EndingPoint_Should_Be_Last_Segment_To()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Assert
        route!.EndingPoint.ShouldBe(point3);
    }

    [Fact]
    public void GetWaypoints_Should_Return_All_Unique_Coordinates()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Act
        var waypoints = route!.GetWaypoints();

        // Assert
        waypoints.Count.ShouldBe(3);
        waypoints[0].ShouldBe(point1);
        waypoints[1].ShouldBe(point2);
        waypoints[2].ShouldBe(point3);
    }

    [Fact]
    public void GetSegmentDistance_Should_Return_Correct_Distance()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Act
        var distance0 = route!.GetSegmentDistance(0);
        var distance1 = route.GetSegmentDistance(1);

        // Assert
        distance0.ShouldBe(segment1!.Distance);
        distance1.ShouldBe(segment2!.Distance);
    }

    [Fact]
    public void GetSegmentDistance_With_Invalid_Index_Should_Return_Null()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var distance = route!.GetSegmentDistance(5);

        // Assert
        distance.ShouldBeNull();
    }

    [Fact]
    public void GetCumulativeDistance_Should_Return_Cumulative_Sum()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Act
        var cumulative0 = route!.GetCumulativeDistance(0);
        var cumulative1 = route.GetCumulativeDistance(1);

        // Assert
        cumulative0.ShouldNotBeNull();
        cumulative0.Value.ShouldBe(segment1!.Distance.Kilometers, tolerance: 0.001);
        
        cumulative1.ShouldNotBeNull();
        cumulative1.Value.ShouldBe(
            segment1.Distance.Kilometers + segment2!.Distance.Kilometers,
            tolerance: 0.001);
    }

    [Fact]
    public void GetCumulativeDistance_With_Invalid_Index_Should_Return_Null()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var distance = route!.GetCumulativeDistance(-1);

        // Assert
        distance.ShouldBeNull();
    }

    [Fact]
    public void GetDescription_Should_Include_Segment_Count_And_Distance()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(point1, point2);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! }, "Test Route");

        // Act
        var description = route!.GetDescription();

        // Assert
        description.ShouldContain("Test Route:");
        description.ShouldContain("1 segment");
        description.ShouldContain("km");
        description.ShouldContain("mi");
        description.ShouldContain("From:");
        description.ShouldContain("To:");
    }

    [Fact]
    public void GetDescription_With_Multiple_Segments_Should_Use_Plural()
    {
        // Arrange
        var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

        var (_, segment1) = RouteSegment.TryCreate(point1, point2);
        var (_, segment2) = RouteSegment.TryCreate(point2, point3);

        var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2! });

        // Act
        var description = route!.GetDescription();

        // Assert
        description.ShouldContain("2 segments");
    }

    [Fact]
    public void ToString_Should_Return_Description()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act
        var result = route!.ToString();

        // Assert
        result.ShouldBe(route.GetDescription());
    }

    [Fact]
    public void Route_Should_Be_Immutable()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! }, "Original");

        // Act - Create new route with modified name using with expression
        var modified = route! with { Name = "Modified" };

        // Assert - Original should be unchanged
        route.Name.ShouldBe("Original");
        modified.Name.ShouldBe("Modified");
        route.Segments.ShouldBe(modified.Segments);
    }

    [Fact]
    public void Segments_Collection_Should_Be_ReadOnly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);
        var (_, route) = GeospatialRoute.TryCreate(new[] { segment! });

        // Act & Assert
        route!.Segments.ShouldBeOfType<System.Collections.ObjectModel.ReadOnlyCollection<RouteSegment>>();
    }
}
