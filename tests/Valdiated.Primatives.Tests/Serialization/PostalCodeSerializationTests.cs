using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class PostalCodeSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_PostalCode_Should_Write_Object_With_Value_And_CountryCode()
    {
        // Arrange
        var (_, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, "12345");

        // Act
        var json = JsonSerializer.Serialize(postalCode, _options);

        // Assert
        json.ShouldContain("\"Value\":\"12345\"");
        json.ShouldContain("\"CountryCode\":\"UnitedStates\"");
    }

    [Fact]
    public void Deserialize_PostalCode_Should_Read_From_Object()
    {
        // Arrange
        var json = "{\"Value\":\"12345\",\"CountryCode\":\"UnitedStates\"}";

        // Act
        var postalCode = JsonSerializer.Deserialize<PostalCode>(json, _options);

        // Assert
        postalCode.ShouldNotBeNull();
        postalCode.Value.ShouldBe("12345");
        postalCode.CountryCode.ShouldBe(CountryCode.UnitedStates);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value_And_CountryCode()
    {
        // Arrange
        var (_, original) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "SW1A 1AA");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<PostalCode>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.CountryCode.ShouldBe(original.CountryCode);
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Deserialize_Invalid_PostalCode_Should_Throw_JsonException()
    {
        // Arrange
        var json = "{\"Value\":\"X\",\"CountryCode\":\"UnitedStates\"}";

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<PostalCode>(json, _options));
    }

    [Fact]
    public void Serialize_Null_PostalCode_Should_Write_Null()
    {
        // Arrange
        PostalCode? postalCode = null;

        // Act
        var json = JsonSerializer.Serialize(postalCode, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var postalCode = JsonSerializer.Deserialize<PostalCode>(json, _options);

        // Assert
        postalCode.ShouldBeNull();
    }

    [Fact]
    public void Serialize_PostalCode_In_Object_Should_Work()
    {
        // Arrange
        var (_, postalCode) = PostalCode.TryCreate(CountryCode.Canada, "K1A 0B1");
        var obj = new { Zip = postalCode, City = "Ottawa" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"zip\":{");
        json.ShouldContain("\"Value\":\"K1A 0B1\"");
        json.ShouldContain("\"CountryCode\":\"Canada\"");
        json.ShouldContain("\"city\":\"Ottawa\"");
    }

    [Fact]
    public void Deserialize_PostalCode_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"zip\":{\"Value\":\"K1A 0B1\",\"CountryCode\":\"Canada\"},\"city\":\"Ottawa\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.Zip.ShouldNotBeNull();
        obj.Zip.Value.ShouldBe("K1A 0B1");
        obj.Zip.CountryCode.ShouldBe(CountryCode.Canada);
        obj.City.ShouldBe("Ottawa");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "12345")]
    [InlineData(CountryCode.UnitedKingdom, "SW1A 1AA")]
    [InlineData(CountryCode.Canada, "K1A 0B1")]
    [InlineData(CountryCode.Australia, "2000")]
    [InlineData(CountryCode.Germany, "10115")]
    public void RoundTrip_Different_Countries_Should_Preserve_Values(CountryCode countryCode, string postalValue)
    {
        // Arrange
        var (result, original) = PostalCode.TryCreate(countryCode, postalValue);
        result.IsValid.ShouldBeTrue($"PostalCode should be valid for {countryCode}: {postalValue}. Errors: {string.Join(", ", result.Errors.Select(e => e.Message))}");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<PostalCode>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(postalValue);
        deserialized.CountryCode.ShouldBe(countryCode);
    }

    private class TestObject
    {
        public PostalCode Zip { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}
