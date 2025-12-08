using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class CityTests
{
    [Fact]
    public void GivenTryCreate_WhenValidCity_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("New York");

        result.IsValid.ShouldBeTrue("Result should be valid for a valid city name");
        city.ShouldNotBeNull("City should not be null when validation succeeds");
        city!.Value.ShouldBe("New York");
    }

    [Fact]
    public void GivenTryCreate_WhenNullValue_ThenShouldFail()
    {
        var (result, city) = City.TryCreate(null!);

        result.IsValid.ShouldBeFalse("Result should be invalid for null value");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Code.ShouldBe("NotNullOrWhitespace");
    }

    [Fact]
    public void GivenTryCreate_WhenEmptyString_ThenShouldFail()
    {
        var (result, city) = City.TryCreate("");

        result.IsValid.ShouldBeFalse("Result should be invalid for empty string");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void GivenTryCreate_WhenWhitespace_ThenShouldFail()
    {
        var (result, city) = City.TryCreate("   ");

        result.IsValid.ShouldBeFalse("Result should be invalid for whitespace");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void GivenTryCreate_WhenMaxLength_ThenShouldSucceed()
    {
        var value = new string('a', 100);
        var (result, city) = City.TryCreate(value);

        result.IsValid.ShouldBeTrue("Result should be valid for 100 character city name");
        city.ShouldNotBeNull();
        city!.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void GivenTryCreate_WhenExceedingMaxLength_ThenShouldFail()
    {
        var value = new string('a', 101);
        var (result, city) = City.TryCreate(value);

        result.IsValid.ShouldBeFalse("Result should be invalid when city name exceeds 100 characters");
        city.ShouldBeNull("City should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Fact]
    public void GivenTryCreateWithCustomPropertyName_WhenExceedsMaxLength_ThenShouldUseInErrorMessage()
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
    public void GivenTryCreate_WhenVariousUSCities_ThenShouldSucceed(string cityName)
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
    public void GivenTryCreate_WhenInternationalCities_ThenShouldSucceed(string cityName)
    {
        var (result, city) = City.TryCreate(cityName);

        result.IsValid.ShouldBeTrue($"Result should be valid for: {cityName}");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe(cityName);
    }

    [Fact]
    public void GivenCity_WhenToString_ThenShouldReturnValue()
    {
        var (_, city) = City.TryCreate("New York");

        city.ShouldNotBeNull();
        city!.ToString().ShouldBe("New York");
    }

    [Fact]
    public void GivenTryCreate_WhenUnicodeCharacters_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("Montréal");

        result.IsValid.ShouldBeTrue("Result should be valid for city with Unicode characters");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("Montréal");
    }

    [Fact]
    public void GivenTryCreate_WhenAccentedCharacters_ThenShouldSucceed()
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
    public void GivenTryCreate_WhenHyphens_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("Winston-Salem");

        result.IsValid.ShouldBeTrue("Result should be valid for city with hyphens");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("Winston-Salem");
    }

    [Fact]
    public void GivenTryCreate_WhenApostrophes_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("L'Aquila");

        result.IsValid.ShouldBeTrue("Result should be valid for city with apostrophes");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("L'Aquila");
    }

    [Fact]
    public void GivenTryCreate_WhenSpaces_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("Salt Lake City");

        result.IsValid.ShouldBeTrue("Result should be valid for city with spaces");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("Salt Lake City");
    }

    [Fact]
    public void GivenTryCreate_WhenPeriods_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("St. Louis");

        result.IsValid.ShouldBeTrue("Result should be valid for city with periods");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("St. Louis");
    }

    [Fact]
    public void GivenEquality_WhenSameValues_ThenShouldBeEqual()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("New York");

        city1.ShouldBe(city2);
        (city1 == city2).ShouldBeTrue();
    }

    [Fact]
    public void GivenEquality_WhenDifferentValues_ThenShouldNotBeEqual()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("Los Angeles");

        city1.ShouldNotBe(city2);
        (city1 != city2).ShouldBeTrue();
    }

    [Fact]
    public void GivenEquality_WhenDifferentCase_ThenShouldBeCaseSensitive()
    {
        var (_, city1) = City.TryCreate("New York");
        var (_, city2) = City.TryCreate("new york");

        // String comparison is case-sensitive by default
        city1.ShouldNotBe(city2);
    }

    [Fact]
    public void GivenCity_WhenComparing_ThenShouldHaveRecordSemantics()
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
    public void GivenTryCreate_WhenExactly100Characters_ThenShouldSucceed()
    {
        var exactValue = new string('X', 100);
        var (result, city) = City.TryCreate(exactValue);

        result.IsValid.ShouldBeTrue("Exactly 100 characters should be valid");
        city.ShouldNotBeNull();
        city!.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void GivenTryCreate_When101Characters_ThenShouldFail()
    {
        var tooLong = new string('X', 101);
        var (result, city) = City.TryCreate(tooLong);

        result.IsValid.ShouldBeFalse("101 characters should exceed maximum");
        city.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Fact]
    public void GivenTryCreate_WhenSingleCharacter_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("X");

        result.IsValid.ShouldBeTrue("Single character should be valid");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("X");
    }

    [Fact]
    public void GivenTryCreate_WhenNumbers_ThenShouldSucceed()
    {
        var (result, city) = City.TryCreate("City 17");

        result.IsValid.ShouldBeTrue("City name with numbers should be valid");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("City 17");
    }

    [Fact]
    public void GivenTryCreate_WhenMixedCase_ThenShouldPreserveCase()
    {
        var (result, city) = City.TryCreate("New YoRk");

        result.IsValid.ShouldBeTrue();
        city.ShouldNotBeNull();
        city!.Value.ShouldBe("New YoRk"); // Preserves exact case
    }

    [Fact]
    public void GivenTryCreate_WhenLeadingAndTrailingSpaces_ThenShouldTrimSpaces()
    {
        var (result, city) = City.TryCreate("  New York  ");

        result.IsValid.ShouldBeTrue();
        city.ShouldNotBeNull();
        // Trims leading and trailing spaces
        city!.Value.ShouldBe("New York");
    }

    [Fact]
    public void GivenTryCreate_WhenBothValidationsFail_ThenShouldReturnMultipleErrors()
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
    public void GivenTryCreate_WhenMajorWorldCities_ThenShouldSucceed(string cityName)
    {
        var (result, city) = City.TryCreate(cityName);

        result.IsValid.ShouldBeTrue($"Should be valid for {cityName}");
        city.ShouldNotBeNull();
        city!.Value.ShouldBe(cityName);
    }
}
