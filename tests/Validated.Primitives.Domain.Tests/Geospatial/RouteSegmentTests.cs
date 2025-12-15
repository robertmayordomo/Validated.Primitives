using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Validation;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Geospatial;

public class RouteSegmentTests
{
    [Fact]
    public void TryCreate_With_Valid_Coordinates_Should_Succeed()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);

        // Act
        var (result, segment) = RouteSegment.TryCreate(from, to);

        // Assert
        result.IsValid.ShouldBeTrue();
        segment.ShouldNotBeNull();
        segment!.From.ShouldBe(from);
        segment.To.ShouldBe(to);
        segment.Distance.ShouldNotBeNull();
        segment.Distance.Kilometers.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void TryCreate_With_Name_Should_Set_Name()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var name = "Midtown Route";

        // Act
        var (result, segment) = RouteSegment.TryCreate(from, to, name);

        // Assert
        result.IsValid.ShouldBeTrue();
        segment.ShouldNotBeNull();
        segment!.Name.ShouldBe(name);
    }

    [Fact]
    public void TryCreate_With_Null_From_Should_Fail()
    {
        // Arrange
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);

        // Act
        var (result, segment) = RouteSegment.TryCreate(null, to);

        // Assert
        result.IsValid.ShouldBeFalse();
        segment.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "From");
    }

    [Fact]
    public void TryCreate_With_Null_To_Should_Fail()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var (result, segment) = RouteSegment.TryCreate(from, null);

        // Assert
        result.IsValid.ShouldBeFalse();
        segment.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "To");
    }

    [Fact]
    public void TryCreate_With_Both_Null_Should_Fail_With_Multiple_Errors()
    {
        // Act
        var (result, segment) = RouteSegment.TryCreate(null, null);

        // Assert
        result.IsValid.ShouldBeFalse();
        segment.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Distance_Property_Should_Be_Calculated_From_Coordinates()
    {
        // Arrange - New York to Empire State Building
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);

        // Act
        var (result, segment) = RouteSegment.TryCreate(from, to);

        // Assert
        result.IsValid.ShouldBeTrue();
        segment!.Distance.Kilometers.ShouldBeGreaterThan(0);
        segment.Distance.From.ShouldBe(from);
        segment.Distance.To.ShouldBe(to);
    }

    [Fact]
    public void GetDescription_Without_Name_Should_Include_Coordinates_And_Distance()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to);

        // Act
        var description = segment!.GetDescription();

        // Assert
        description.ShouldContain("Segment:");
        description.ShouldContain("?");
        description.ShouldContain("km");
    }

    [Fact]
    public void GetDescription_With_Name_Should_Include_Name()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to, "Downtown to Midtown");

        // Act
        var description = segment!.GetDescription();

        // Assert
        description.ShouldContain("Downtown to Midtown:");
        description.ShouldContain("?");
        description.ShouldContain("km");
    }

    [Fact]
    public void ToString_Should_Return_Description()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to, "Test Route");

        // Act
        var result = segment!.ToString();

        // Assert
        result.ShouldBe(segment.GetDescription());
    }

    [Fact]
    public void Segment_With_Same_Coordinates_Should_Have_Zero_Distance()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var (result, segment) = RouteSegment.TryCreate(from, to);

        // Assert
        result.IsValid.ShouldBeTrue();
        segment!.Distance.Kilometers.ShouldBe(0, tolerance: 0.001);
    }

    [Fact]
    public void Segment_Should_Be_Immutable()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, segment) = RouteSegment.TryCreate(from, to, "Original");

        // Act - Create new segment with modified name using with expression
        var modified = segment! with { Name = "Modified" };

        // Assert - Original should be unchanged
        segment.Name.ShouldBe("Original");
        modified.Name.ShouldBe("Modified");
        segment.From.ShouldBe(modified.From);
        segment.To.ShouldBe(modified.To);
    }
}
