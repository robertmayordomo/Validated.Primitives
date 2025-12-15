using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Domain.Geospatial.Builders;
using Validated.Primitives.Validation;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Geospatial;

public class CoordinateTests
{
    [Fact]
    public void TryCreate_With_Valid_Coordinates_Should_Succeed()
    {
        // Arrange & Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Latitude.Value.ShouldBe(40.7128m);
        coordinate.Longitude.Value.ShouldBe(-74.0060m);
    }

    [Fact]
    public void TryCreate_With_Altitude_Should_Succeed()
    {
        // Arrange & Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 100.5m);

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Altitude.ShouldBe(100.5m);
    }

    [Fact]
    public void TryCreate_With_Accuracy_Should_Succeed()
    {
        // Arrange & Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: 10.5m);

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Accuracy.ShouldBe(10.5m);
    }

    [Theory]
    [InlineData(91, -74.0060)]
    [InlineData(-91, -74.0060)]
    [InlineData(40.7128, 181)]
    [InlineData(40.7128, -181)]
    public void TryCreate_With_Invalid_Coordinates_Should_Fail(decimal latitude, decimal longitude)
    {
        // Act
        var (result, coordinate) = Coordinate.TryCreate(latitude, longitude);

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_With_Invalid_Altitude_Too_Low_Should_Fail()
    {
        // Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: -600m);

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Altitude");
    }

    [Fact]
    public void TryCreate_With_Invalid_Altitude_Too_High_Should_Fail()
    {
        // Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 15000m);

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Altitude");
    }

    [Fact]
    public void TryCreate_With_Negative_Accuracy_Should_Fail()
    {
        // Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: -10m);

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Accuracy");
    }

    [Fact]
    public void TryCreate_With_Excessive_Accuracy_Should_Fail()
    {
        // Act
        var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: 2000000m);

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Accuracy");
    }

    [Fact]
    public void ToCardinalString_Should_Format_Correctly()
    {
        // Arrange
        var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, decimalPlaces: 6);

        // Act
        var result = coordinate!.ToCardinalString();

        // Assert
        result.ShouldBe("40.712800° N, 74.006000° W");
    }

    [Fact]
    public void ToCardinalString_With_Altitude_Should_Include_Altitude()
    {
        // Arrange
        var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, decimalPlaces: 6, altitude: 100.5m);

        // Act
        var result = coordinate!.ToCardinalString();

        // Assert
        result.ShouldBe("40.712800° N, 74.006000° W, 100.5m");
    }

    [Fact]
    public void ToDecimalDegreesString_Should_Format_Correctly()
    {
        // Arrange
        var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var result = coordinate!.ToDecimalDegreesString();

        // Assert
        result.ShouldBe("40.7128, -74.0060");
    }

    [Fact]
    public void ToGoogleMapsFormat_Should_Format_Correctly()
    {
        // Arrange
        var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var result = coordinate!.ToGoogleMapsFormat();

        // Assert
        result.ShouldBe("40.7128,-74.0060");
    }

    [Fact]
    public void ToString_Should_Include_Cardinal_Directions()
    {
        // Arrange
        var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, decimalPlaces: 6);

        // Act
        var result = coordinate!.ToString();

        // Assert
        result.ShouldContain("40.712800° N");
        result.ShouldContain("74.006000° W");
    }

    [Fact]
    public void ToString_With_Accuracy_Should_Include_Accuracy()
    {
        // Arrange
        var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: 15.5m);

        // Act
        var result = coordinate!.ToString();

        // Assert
        result.ShouldContain("(±16m)");
    }

    [Fact]
    public void DistanceTo_Should_Calculate_Distance_Between_Coordinates()
    {
        // Arrange - New York to Los Angeles
        var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, losAngeles) = Coordinate.TryCreate(34.0522m, -118.2437m);

        // Act
        var distance = newYork!.DistanceTo(losAngeles!);

        // Assert
        distance.ShouldBeGreaterThan(3900); // Approximately 3944 km
        distance.ShouldBeLessThan(4000);
    }

    [Fact]
    public void DistanceTo_Same_Coordinate_Should_Return_Zero()
    {
        // Arrange
        var (_, coordinate1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, coordinate2) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var distance = coordinate1!.DistanceTo(coordinate2!);

        // Assert
        distance.ShouldBe(0, tolerance: 0.001);
    }

    [Fact]
    public void Builder_With_Complete_Data_Should_Succeed()
    {
        // Arrange
        var builder = new CoordinateBuilder();

        // Act
        var (result, coordinate) = builder
            .WithLatitude(40.7128m)
            .WithLongitude(-74.0060m)
            .WithAltitude(100m)
            .WithAccuracy(10m)
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Latitude.Value.ShouldBe(40.7128m);
        coordinate.Longitude.Value.ShouldBe(-74.0060m);
        coordinate.Altitude.ShouldBe(100m);
        coordinate.Accuracy.ShouldBe(10m);
    }

    [Fact]
    public void Builder_WithCoordinates_Should_Set_Both_Values()
    {
        // Arrange
        var builder = new CoordinateBuilder();

        // Act
        var (result, coordinate) = builder
            .WithCoordinates(40.7128m, -74.0060m)
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Latitude.Value.ShouldBe(40.7128m);
        coordinate!.Longitude.Value.ShouldBe(-74.0060m);
    }

    [Fact]
    public void Builder_WithPosition_Should_Set_All_Values()
    {
        // Arrange
        var builder = new CoordinateBuilder();

        // Act
        var (result, coordinate) = builder
            .WithPosition(40.7128m, -74.0060m, altitude: 100m, accuracy: 10m)
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Latitude.Value.ShouldBe(40.7128m);
        coordinate!.Longitude.Value.ShouldBe(-74.0060m);
        coordinate.Altitude.ShouldBe(100m);
        coordinate.Accuracy.ShouldBe(10m);
    }

    [Fact]
    public void Builder_Without_Latitude_Should_Fail()
    {
        // Arrange
        var builder = new CoordinateBuilder();

        // Act
        var (result, coordinate) = builder
            .WithLongitude(-74.0060m)
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Latitude");
    }

    [Fact]
    public void Builder_Without_Longitude_Should_Fail()
    {
        // Arrange
        var builder = new CoordinateBuilder();

        // Act
        var (result, coordinate) = builder
            .WithLatitude(40.7128m)
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Longitude");
    }

    [Fact]
    public void Builder_Reset_Should_Clear_All_Values()
    {
        // Arrange
        var builder = new CoordinateBuilder()
            .WithLatitude(40.7128m)
            .WithLongitude(-74.0060m)
            .WithAltitude(100m);

        // Act
        builder.Reset();
        var (result, coordinate) = builder.Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        coordinate.ShouldBeNull();
    }

    [Fact]
    public void Builder_WithDecimalPlaces_Should_Set_Precision()
    {
        // Arrange
        var builder = new CoordinateBuilder();

        // Act
        var (result, coordinate) = builder
            .WithLatitude(40.71m)
            .WithLongitude(-74.01m)
            .WithDecimalPlaces(2)
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        coordinate.ShouldNotBeNull();
        coordinate!.Latitude.DecimalPlaces.ShouldBe(2);
        coordinate.Longitude.DecimalPlaces.ShouldBe(2);
    }

    [Fact]
    public void Coordinate_With_Different_Decimal_Places_Should_Work()
    {
        // Arrange & Act
        var (result1, coord1) = Coordinate.TryCreate(40.7m, -74.0m, decimalPlaces: 1);
        var (result2, coord2) = Coordinate.TryCreate(40.7128m, -74.0060m, decimalPlaces: 4);

        // Assert
        result1.IsValid.ShouldBeTrue();
        result2.IsValid.ShouldBeTrue();
        coord1!.Latitude.DecimalPlaces.ShouldBe(1);
        coord2!.Latitude.DecimalPlaces.ShouldBe(4);
    }

    [Fact]
    public void Coordinate_Equality_Should_Work()
    {
        // Arrange
        var (_, coord1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, coord2) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Assert
        coord1.ShouldBe(coord2);
    }

    [Fact]
    public void Coordinate_With_Different_Altitude_Should_Not_Be_Equal()
    {
        // Arrange
        var (_, coord1) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 100m);
        var (_, coord2) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 200m);

        // Assert
        coord1.ShouldNotBe(coord2);
    }
}
