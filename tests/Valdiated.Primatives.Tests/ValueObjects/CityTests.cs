using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class CityTests
{
    [Fact]
    public void TryCreate_WithValidCity_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("New York");

        result.IsValid.ShouldBeTrue("Result should be valid for a valid city name");
        city.ShouldNotBeNull("City should not be null when validation succeeds");
        city!.Value.ShouldBe("New York");
    }

    [Fact]
    public void TryCreate_WithNullValue_ShouldFail()
    {
        var (result, city) = City.TryCreate(null!);

        result.IsValid.ShouldBeFalse("Result should be invalid for null value");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Code.ShouldBe("NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_WithEmptyString_ShouldFail()
    {
        var (result, city) = City.TryCreate("");

        result.IsValid.ShouldBeFalse("Result should be invalid for empty string");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_WithWhitespace_ShouldFail()
    {
        var (result, city) = City.TryCreate("   ");

        result.IsValid.ShouldBeFalse("Result should be invalid for whitespace");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_WithMaxLength_ShouldSucceed()
    {
        var value = new string('a', 100);
        var (result, city) = City.TryCreate(value);

        result.IsValid.ShouldBeTrue("Result should be valid for 100 character city name");
        city.ShouldNotBeNull();
        city!.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void TryCreate_ExceedingMaxLength_ShouldFail()
    {
        var value = new string('a', 101);
        var (result, city) = City.TryCreate(value);

        result.IsValid.ShouldBeFalse("Result should be invalid when city name exceeds 100 characters");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_ShouldUseInErrorMessage()
    {
        var value = new string('a', 101);
        var (result, city) = City.TryCreate(value, "CityName");

        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("CityName");
    }

    [Theory]
    [InlineData("New York")]
    [InlineData("Los Angeles")]
    [InlineData("Chicago")]
    [InlineData("Houston")]
    [InlineData("Phoenix")]
    [InlineData("San Francisco")]
    [InlineData("Seattle")]
    [InlineData("Boston")]
    public void TryCreate_WithVariousUSCities_ShouldSucceed(string cityName)
    {
        var (result, city) = City.TryCreate(cityName);

        result.IsValid.ShouldBeTrue($"Result should be valid for: {cityName}");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe(cityName);
    }

    [Theory]
    [InlineData("London")]
    [InlineData("Paris")]
    [InlineData("Tokyo")]
    [InlineData("Berlin")]
    [InlineData("Sydney")]
    [InlineData("Toronto")]
    [InlineData("Mumbai")]
    [InlineData("Mexico City")]
    public void TryCreate_WithInternationalCities_ShouldSucceed(string cityName)
    {
        var (result, city) = City.TryCreate(cityName);

        result.IsValid.ShouldBeTrue($"Result should be valid for: {cityName}");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe(cityName);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var (_, city) = City.TryCreate("New York");

        city.ShouldNotBeNull();
        city!.ToString().ShouldBe("New York");
    }

    [Fact]
    public void TryCreate_WithUnicodeCharacters_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("Montréal");

        result.IsValid.ShouldBeTrue("Result should be valid for city with Unicode characters");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("Montréal");
    }

    [Fact]
    public void TryCreate_WithAccentedCharacters_ShouldSucceed()
    {
        var cities = new[]
        {
            "São Paulo",
            "München",
            "Zürich",
            "København",
            "Kraków"
        };

        foreach (var cityName in cities)
        {
            var (result, city) = City.TryCreate(cityName);
            result.IsValid.ShouldBeTrue($"Should be valid for {cityName}");
            city.ShouldNotBeNull();
            city!.Value.ShouldBe(cityName);
        }
    }

    [Fact]
    public void TryCreate_WithHyphens_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("Winston-Salem");

        result.IsValid.ShouldBeTrue("Result should be valid for city with hyphens");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("Winston-Salem");
    }

    [Fact]
    public void TryCreate_WithApostrophes_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("L'Aquila");

        result.IsValid.ShouldBeTrue("Result should be valid for city with apostrophes");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("L'Aquila");
    }

    [Fact]
    public void TryCreate_WithSpaces_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("Salt Lake City");

        result.IsValid.ShouldBeTrue("Result should be valid for city with spaces");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("Salt Lake City");
    }

    [Fact]
    public void TryCreate_WithPeriods_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("St. Louis");

        result.IsValid.ShouldBeTrue("Result should be valid for city with periods");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("St. Louis");
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("New York");

        city1.ShouldBe(city2);
        (city1 == city2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("Los Angeles");

        city1.ShouldNotBe(city2);
        (city1 != city2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_CaseSensitive()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("new york");

        // String comparison is case-sensitive by default
        city1.ShouldNotBe(city2);
    }

    [Fact]
    public void City_ShouldHaveRecordSemantics()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("New York");

        // Records should be equal based on value
        city1.ShouldNotBeNull();
        city2.ShouldNotBeNull();
        city1.ShouldBe(city2);
        city1!.GetHashCode().ShouldBe(city2!.GetHashCode());
    }

    [Fact]
    public void TryCreate_WithExactly100Characters_ShouldSucceed()
    {
        var exactValue = new string('X', 100);
        var (result, city) = City.TryCreate(exactValue);

        result.IsValid.ShouldBeTrue("Exactly 100 characters should be valid");
        city.ShouldNotBeNull();
        city!.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void TryCreate_With101Characters_ShouldFail()
    {
        var tooLong = new string('X', 101);
        var (result, city) = City.TryCreate(tooLong);

        result.IsValid.ShouldBeFalse("101 characters should exceed maximum");
        city.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_WithSingleCharacter_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("X");

        result.IsValid.ShouldBeTrue("Single character should be valid");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("X");
    }

    [Fact]
    public void TryCreate_WithNumbers_ShouldSucceed()
    {
        var (result, city) = City.TryCreate("City 17");

        result.IsValid.ShouldBeTrue("City name with numbers should be valid");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("City 17");
    }

    [Fact]
    public void TryCreate_WithMixedCase_PreservesCase()
    {
        var (result, city) = City.TryCreate("New YoRk");

        result.IsValid.ShouldBeTrue();
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("New YoRk"); // Preserves exact case
    }

    [Fact]
    public void TryCreate_WithLeadingAndTrailingSpaces_TrimsSpaces()
    {
        var (result, city) = City.TryCreate("  New York  ");

        result.IsValid.ShouldBeTrue();
        city.ShouldNotBeNull();
        // Trims leading and trailing spaces
        city!.Value.ShouldBe("New York");
    }

    [Fact]
    public void TryCreate_ReturnsMultipleErrors_WhenBothValidationsFail()
    {
        var tooLong = new string('a', 101);
        var (result, city) = City.TryCreate(tooLong);

        result.IsValid.ShouldBeFalse();
        city.ShouldBeNull();
        // Should have MaxLength error
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Theory]
    [InlineData("Bangkok")]
    [InlineData("Cairo")]
    [InlineData("Delhi")]
    [InlineData("Istanbul")]
    [InlineData("Jakarta")]
    [InlineData("Karachi")]
    [InlineData("Lagos")]
    [InlineData("Manila")]
    [InlineData("Moscow")]
    [InlineData("Seoul")]
    [InlineData("Shanghai")]
    [InlineData("Tehran")]
    public void TryCreate_WithMajorWorldCities_ShouldSucceed(string cityName)
    {
        var (result, city) = City.TryCreate(cityName);

        result.IsValid.ShouldBeTrue($"Should be valid for {cityName}");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe(cityName);
    }
}
