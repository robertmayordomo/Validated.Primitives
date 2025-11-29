using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class PhoneNumberSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_PhoneNumber_Should_Write_Object_With_Value_And_CountryCode()
    {
        // Arrange
        var (_, phoneNumber) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "+1 (555) 123-4567");

        // Act
        var json = JsonSerializer.Serialize(phoneNumber, _options);

        // Assert
        json.ShouldContain("\"Value\":");
        json.ShouldContain("1 (555) 123-4567");  // Without the + which gets escaped
        json.ShouldContain("\"CountryCode\":\"UnitedStates\"");
    }

    [Fact]
    public void Deserialize_PhoneNumber_Should_Read_From_Object()
    {
        // Arrange
        var json = "{\"Value\":\"+1 (555) 123-4567\",\"CountryCode\":\"UnitedStates\"}";

        // Act
        var phoneNumber = JsonSerializer.Deserialize<PhoneNumber>(json, _options);

        // Assert
        phoneNumber.ShouldNotBeNull();
        phoneNumber.Value.ShouldBe("+1 (555) 123-4567");
        phoneNumber.CountryCode.ShouldBe(CountryCode.UnitedStates);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value_And_CountryCode()
    {
        // Arrange
        var (_, original) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "+1 (555) 987-6543");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<PhoneNumber>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.CountryCode.ShouldBe(original.CountryCode);
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Deserialize_Invalid_PhoneNumber_Should_Throw_JsonException()
    {
        // Arrange
        var json = "{\"Value\":\"123\",\"CountryCode\":\"UnitedStates\"}";

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<PhoneNumber>(json, _options));
    }

    [Fact]
    public void Serialize_Null_PhoneNumber_Should_Write_Null()
    {
        // Arrange
        PhoneNumber? phoneNumber = null;

        // Act
        var json = JsonSerializer.Serialize(phoneNumber, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var phoneNumber = JsonSerializer.Deserialize<PhoneNumber>(json, _options);

        // Assert
        phoneNumber.ShouldBeNull();
    }

    [Fact]
    public void Serialize_PhoneNumber_In_Object_Should_Work()
    {
        // Arrange
        var (_, phoneNumber) = PhoneNumber.TryCreate(CountryCode.Canada, "+1 (416) 555-1234");
        var obj = new { Phone = phoneNumber, Name = "John" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"phone\":{");
        json.ShouldContain("\"Value\":");
        json.ShouldContain("1 (416) 555-1234");
        json.ShouldContain("\"CountryCode\":\"Canada\"");
        json.ShouldContain("\"name\":\"John\"");
    }

    [Fact]
    public void Deserialize_PhoneNumber_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"phone\":{\"Value\":\"+1 (416) 555-1234\",\"CountryCode\":\"Canada\"},\"name\":\"John\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.Phone.ShouldNotBeNull();
        obj.Phone.Value.ShouldBe("+1 (416) 555-1234");
        obj.Phone.CountryCode.ShouldBe(CountryCode.Canada);
        obj.Name.ShouldBe("John");
    }

    [Theory]
    [InlineData(CountryCode.UnitedStates, "+1 (555) 123-4567")]
    [InlineData(CountryCode.Canada, "+1 (416) 555-1234")]
    [InlineData(CountryCode.Australia, "+61 2 1234 5678")]
    [InlineData(CountryCode.Germany, "+49 30 12345678")]
    public void RoundTrip_Different_Countries_Should_Preserve_Values(CountryCode countryCode, string phoneValue)
    {
        // Arrange
        var (result, original) = PhoneNumber.TryCreate(countryCode, phoneValue);
        result.IsValid.ShouldBeTrue($"PhoneNumber should be valid for {countryCode}: {phoneValue}. Errors: {string.Join(", ", result.Errors.Select(e => e.Message))}");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<PhoneNumber>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(phoneValue);
        deserialized.CountryCode.ShouldBe(countryCode);
    }

    private class TestObject
    {
        public PhoneNumber Phone { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
