using Shouldly;
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Validation;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Geospatial;

public class GeoDistanceTests
{
    [Fact]
    public void TryCreate_With_Valid_Coordinates_Should_Succeed()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(from!, to!);

        // Assert
        result.IsValid.ShouldBeTrue();
        distance.ShouldNotBeNull();
        distance!.From.ShouldBe(from);
        distance.To.ShouldBe(to);
        distance.Kilometers.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void TryCreate_Should_Calculate_Distance_Between_New_York_And_Los_Angeles()
    {
        // Arrange - New York to Los Angeles
        var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, losAngeles) = Coordinate.TryCreate(34.0522m, -118.2437m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(newYork!, losAngeles!);

        // Assert
        result.IsValid.ShouldBeTrue();
        distance.ShouldNotBeNull();
        distance!.Kilometers.ShouldBeGreaterThan(3900);
        distance.Kilometers.ShouldBeLessThan(4000);
    }

    [Fact]
    public void TryCreate_With_Same_Coordinates_Should_Return_Zero_Distance()
    {
        // Arrange
        var (_, coord1) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, coord2) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(coord1!, coord2!);

        // Assert
        result.IsValid.ShouldBeTrue();
        distance.ShouldNotBeNull();
        distance!.Kilometers.ShouldBe(0, tolerance: 0.001);
    }

    [Fact]
    public void TryCreate_With_Null_From_Coordinate_Should_Fail()
    {
        // Arrange
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(null!, to!);

        // Assert
        result.IsValid.ShouldBeFalse();
        distance.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "From");
    }

    [Fact]
    public void TryCreate_With_Null_To_Coordinate_Should_Fail()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(from!, null!);

        // Assert
        result.IsValid.ShouldBeFalse();
        distance.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "To");
    }

    [Fact]
    public void Miles_Property_Should_Convert_Kilometers_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var miles = distance!.Miles;

        // Assert
        miles.ShouldBeGreaterThan(2400);
        miles.ShouldBeLessThan(2500);
        (miles / distance.Kilometers).ShouldBe(0.621371, tolerance: 0.0001);
    }

    [Fact]
    public void Meters_Property_Should_Convert_Kilometers_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m); // Empire State Building
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var meters = distance!.Meters;

        // Assert
        meters.ShouldBe(distance.Kilometers * 1000);
    }

    [Fact]
    public void NauticalMiles_Property_Should_Convert_Kilometers_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(51.5074m, -0.1278m); // London
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var nauticalMiles = distance!.NauticalMiles;

        // Assert
        (nauticalMiles / distance.Kilometers).ShouldBe(0.539957, tolerance: 0.0001);
    }

    [Fact]
    public void ToFormattedString_With_Kilometers_Should_Format_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var formatted = distance!.ToFormattedString(DistanceUnit.Kilometers, 2);

        // Assert
        formatted.ShouldEndWith(" km");
        formatted.ShouldContain(".");
    }

    [Fact]
    public void ToFormattedString_With_Miles_Should_Format_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var formatted = distance!.ToFormattedString(DistanceUnit.Miles, 2);

        // Assert
        formatted.ShouldEndWith(" mi");
        formatted.ShouldContain(".");
    }

    [Fact]
    public void ToFormattedString_With_Meters_Should_Format_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var formatted = distance!.ToFormattedString(DistanceUnit.Meters, 0);

        // Assert
        formatted.ShouldEndWith(" m");
    }

    [Fact]
    public void ToFormattedString_With_NauticalMiles_Should_Format_Correctly()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(51.5074m, -0.1278m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var formatted = distance!.ToFormattedString(DistanceUnit.NauticalMiles, 2);

        // Assert
        formatted.ShouldEndWith(" nm");
    }

    [Fact]
    public void GetDescription_Should_Return_Human_Readable_Description()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var description = distance!.GetDescription();

        // Assert
        description.ShouldContain("Distance from");
        description.ShouldContain("to");
        description.ShouldContain("km");
        description.ShouldContain("mi");
    }

    [Fact]
    public void IsWithinRadius_Should_Return_True_When_Within_Radius()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m); // ~1 km away
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var isWithin = distance!.IsWithinRadius(5.0);

        // Assert
        isWithin.ShouldBeTrue();
    }

    [Fact]
    public void IsWithinRadius_Should_Return_False_When_Outside_Radius()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m); // ~3944 km away
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var isWithin = distance!.IsWithinRadius(1000.0);

        // Assert
        isWithin.ShouldBeFalse();
    }

    [Fact]
    public void ToString_Should_Return_Default_Formatted_String()
    {
        // Arrange
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
        var (_, distance) = GeoDistance.TryCreate(from!, to!);

        // Act
        var result = distance!.ToString();

        // Assert
        result.ShouldEndWith(" km");
    }

    [Fact]
    public void Short_Distance_Should_Calculate_Accurately()
    {
        // Arrange - Empire State Building to Times Square (~1 km)
        var (_, empireState) = Coordinate.TryCreate(40.7484m, -73.9857m);
        var (_, timesSquare) = Coordinate.TryCreate(40.7580m, -73.9855m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(empireState!, timesSquare!);

        // Assert
        result.IsValid.ShouldBeTrue();
        distance!.Kilometers.ShouldBeGreaterThan(1.0);
        distance.Kilometers.ShouldBeLessThan(1.5);
    }

    [Fact]
    public void Long_Distance_Should_Calculate_Accurately()
    {
        // Arrange - New York to London (~5570 km)
        var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
        var (_, london) = Coordinate.TryCreate(51.5074m, -0.1278m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(newYork!, london!);

        // Assert
        result.IsValid.ShouldBeTrue();
        distance!.Kilometers.ShouldBeGreaterThan(5500);
        distance.Kilometers.ShouldBeLessThan(5600);
    }

    [Fact]
    public void Distance_Across_International_Date_Line_Should_Calculate_Correctly()
    {
        // Arrange - Coordinates on opposite sides of the date line
        var (_, west) = Coordinate.TryCreate(0m, -179m);
        var (_, east) = Coordinate.TryCreate(0m, 179m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(west!, east!);

        // Assert
        result.IsValid.ShouldBeTrue();
        distance!.Kilometers.ShouldBeGreaterThan(0);
        // The shortest distance should be small, not across the entire globe
        distance.Kilometers.ShouldBeLessThan(500);
    }

    [Fact]
    public void EarthRadiusKm_Constant_Should_Be_Correct()
    {
        // Assert
        GeoDistance.EarthRadiusKm.ShouldBe(6371.0);
    }

    [Fact]
    public void EarthRadiusMiles_Constant_Should_Be_Correct()
    {
        // Assert
        GeoDistance.EarthRadiusMiles.ShouldBe(3958.8);
    }

    [Fact]
    public void Distance_With_Altitude_Should_Only_Use_Lat_Long()
    {
        // Arrange - Same lat/long but different altitudes
        var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 0m);
        var (_, to) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 1000m);

        // Act
        var (result, distance) = GeoDistance.TryCreate(from!, to!);

        // Assert
        result.IsValid.ShouldBeTrue();
        // Distance should be 0 since lat/long are the same (altitude is not considered in horizontal distance)
        distance!.Kilometers.ShouldBe(0, tolerance: 0.001);
    }
}
