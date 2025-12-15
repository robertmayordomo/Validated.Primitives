using Shouldly;
using Validated.Primitives.ValueObjects.Geospatial;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class LongitudeTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(74.5)]
    [InlineData(-74.5)]
    [InlineData(180)]
    [InlineData(-180)]
    [InlineData(-74.0060)]
    [InlineData(151.2093)]
    public void TryCreate_Succeeds_For_Valid_Longitudes(decimal value)
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData(181)]
    [InlineData(-181)]
    [InlineData(200)]
    [InlineData(-200)]
    [InlineData(360)]
    [InlineData(-360)]
    public void TryCreate_Fails_For_Invalid_Longitudes(decimal value)
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        longitude.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "MinValue" || e.Code == "MaxValue");
    }

    [Fact]
    public void TryCreate_With_Valid_DecimalPlaces()
    {
        // Arrange
        var value = -74.006012m;

        // Act
        var (result, longitude) = Longitude.TryCreate(value, decimalPlaces: 6);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.Value.ShouldBe(value);
        longitude.DecimalPlaces.ShouldBe(6);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    [InlineData(10)]
    public void TryCreate_Fails_For_Invalid_DecimalPlaces(int decimalPlaces)
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(-74.0060m, decimalPlaces);

        // Assert
        result.IsValid.ShouldBeFalse();
        longitude.ShouldBeNull();
    }

    [Theory]
    [InlineData(-74, 0, "-74°")]
    [InlineData(-74.0, 1, "-74.0°")]
    [InlineData(-74.01, 2, "-74.01°")]
    [InlineData(-74.0060, 4, "-74.0060°")]
    [InlineData(151.21, 2, "151.21°")]
    [InlineData(0, 0, "0°")]
    public void ToString_Formats_Correctly(decimal value, int decimalPlaces, string expected)
    {
        // Arrange
        var (result, longitude) = Longitude.TryCreate(value, decimalPlaces);

        // Act
        result.IsValid.ShouldBeTrue($"Longitude creation should succeed for value {value} with {decimalPlaces} decimal places");
        var formattedResult = longitude!.ToString();

        // Assert
        formattedResult.ShouldBe(expected);
    }

    [Theory]
    [InlineData(-74.0060, "West", "W")]
    [InlineData(151.2093, "East", "E")]
    [InlineData(0, "East", "E")]
    [InlineData(180, "East", "E")]
    [InlineData(-180, "West", "W")]
    public void GetHemisphere_Returns_Correct_Value(decimal value, string expectedHemisphere, string expectedCardinal)
    {
        // Arrange
        var (_, longitude) = Longitude.TryCreate(value);

        // Act
        var hemisphere = longitude!.GetHemisphere();
        var cardinal = longitude.GetCardinalDirection();

        // Assert
        hemisphere.ShouldBe(expectedHemisphere);
        cardinal.ShouldBe(expectedCardinal);
    }

    [Theory]
    [InlineData(-74.0060, 6, "74.006000° W")]
    [InlineData(151.2093, 4, "151.2093° E")]
    [InlineData(0, 2, "0.00° E")]
    [InlineData(180, 0, "180° E")]
    [InlineData(-180, 0, "180° W")]
    public void ToCardinalString_Formats_Correctly(decimal value, int decimalPlaces, string expected)
    {
        // Arrange
        var (_, longitude) = Longitude.TryCreate(value, decimalPlaces);

        // Act
        var result = longitude!.ToCardinalString();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Equal_Longitudes_Should_Be_Equal()
    {
        // Arrange
        var (_, longitude1) = Longitude.TryCreate(-74.0060m, 6);
        var (_, longitude2) = Longitude.TryCreate(-74.0060m, 6);

        // Assert
        longitude1.ShouldBe(longitude2);
        longitude1!.GetHashCode().ShouldBe(longitude2!.GetHashCode());
    }

    [Fact]
    public void Different_Longitudes_Should_Not_Be_Equal()
    {
        // Arrange
        var (_, longitude1) = Longitude.TryCreate(-74.0060m, 6);
        var (_, longitude2) = Longitude.TryCreate(151.2093m, 6);

        // Assert
        longitude1.ShouldNotBe(longitude2);
    }

    [Fact]
    public void Different_DecimalPlaces_Should_Not_Be_Equal()
    {
        // Arrange
        var (_, longitude1) = Longitude.TryCreate(-74.0060m, 4);
        var (_, longitude2) = Longitude.TryCreate(-74.0060m, 6);

        // Assert
        longitude1.ShouldNotBe(longitude2);
    }

    [Fact]
    public void Custom_PropertyName_Appears_In_Validation_Error()
    {
        // Act
        var (result, _) = Longitude.TryCreate(200m, propertyName: "LocationLongitude");

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.MemberName == "LocationLongitude");
    }

    [Theory]
    [InlineData(180)]
    [InlineData(-180)]
    public void Boundary_Values_Are_Valid(decimal value)
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.Value.ShouldBe(value);
    }

    [Fact]
    public void Default_DecimalPlaces_Is_6()
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(-74.0060m);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.DecimalPlaces.ShouldBe(6);
    }

    [Fact]
    public void MinValue_Constant_Is_Negative_180()
    {
        // Assert
        Longitude.MinValue.ShouldBe(-180m);
    }

    [Fact]
    public void MaxValue_Constant_Is_Positive_180()
    {
        // Assert
        Longitude.MaxValue.ShouldBe(180m);
    }

    [Fact]
    public void Prime_Meridian_Is_Zero()
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(0m, 2);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.Value.ShouldBe(0m);
        longitude.GetHemisphere().ShouldBe("East");
        longitude.ToCardinalString().ShouldBe("0.00° E");
    }

    [Fact]
    public void International_Date_Line_West_Is_Negative_180()
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(-180m, 0);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.Value.ShouldBe(-180m);
        longitude.GetHemisphere().ShouldBe("West");
        longitude.ToCardinalString().ShouldBe("180° W");
    }

    [Fact]
    public void International_Date_Line_East_Is_Positive_180()
    {
        // Act
        var (result, longitude) = Longitude.TryCreate(180m, 0);

        // Assert
        result.IsValid.ShouldBeTrue();
        longitude.ShouldNotBeNull();
        longitude!.Value.ShouldBe(180m);
        longitude.GetHemisphere().ShouldBe("East");
        longitude.ToCardinalString().ShouldBe("180° E");
    }
}
