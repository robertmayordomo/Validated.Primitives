using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class SimpleValueObjectSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_AddressLine_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, addressLine) = AddressLine.TryCreate("123 Main Street");

        // Act
        var json = JsonSerializer.Serialize(addressLine, _options);

        // Assert
        json.ShouldBe("\"123 Main Street\"");
    }

    [Fact]
    public void Deserialize_AddressLine_Should_Read_From_String()
    {
        // Arrange
        var json = "\"456 Oak Avenue\"";

        // Act
        var addressLine = JsonSerializer.Deserialize<AddressLine>(json, _options);

        // Assert
        addressLine.ShouldNotBeNull();
        addressLine.Value.ShouldBe("456 Oak Avenue");
    }

    [Fact]
    public void RoundTrip_AddressLine_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = AddressLine.TryCreate("789 Elm Boulevard");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<AddressLine>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Serialize_HumanName_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, humanName) = HumanName.TryCreate("Mary-Jane");

        // Act
        var json = JsonSerializer.Serialize(humanName, _options);

        // Assert
        json.ShouldBe("\"Mary-Jane\"");
    }

    [Fact]
    public void Deserialize_HumanName_Should_Read_From_String()
    {
        // Arrange
        var json = "\"O'Brien\"";

        // Act
        var humanName = JsonSerializer.Deserialize<HumanName>(json, _options);

        // Assert
        humanName.ShouldNotBeNull();
        humanName.Value.ShouldBe("O'Brien");
    }

    [Fact]
    public void RoundTrip_HumanName_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = HumanName.TryCreate("François");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<HumanName>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Serialize_IpAddress_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, ipAddress) = IpAddress.TryCreate("192.168.1.1");

        // Act
        var json = JsonSerializer.Serialize(ipAddress, _options);

        // Assert
        json.ShouldBe("\"192.168.1.1\"");
    }

    [Fact]
    public void Deserialize_IpAddress_Should_Read_From_String()
    {
        // Arrange
        var json = "\"10.0.0.1\"";

        // Act
        var ipAddress = JsonSerializer.Deserialize<IpAddress>(json, _options);

        // Assert
        ipAddress.ShouldNotBeNull();
        ipAddress.Value.ShouldBe("10.0.0.1");
    }

    [Fact]
    public void RoundTrip_IpAddress_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = IpAddress.TryCreate("172.16.0.1");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<IpAddress>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void RoundTrip_IpAddress_IPv6_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = IpAddress.TryCreate("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<IpAddress>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
    }

    [Fact]
    public void Serialize_WebsiteUrl_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, websiteUrl) = WebsiteUrl.TryCreate("https://www.example.com");

        // Act
        var json = JsonSerializer.Serialize(websiteUrl, _options);

        // Assert
        json.ShouldBe("\"https://www.example.com\"");
    }

    [Fact]
    public void Deserialize_WebsiteUrl_Should_Read_From_String()
    {
        // Arrange
        var json = "\"https://github.com\"";

        // Act
        var websiteUrl = JsonSerializer.Deserialize<WebsiteUrl>(json, _options);

        // Assert
        websiteUrl.ShouldNotBeNull();
        websiteUrl.Value.ShouldBe("https://github.com");
    }

    [Fact]
    public void RoundTrip_WebsiteUrl_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = WebsiteUrl.TryCreate("https://www.microsoft.com");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<WebsiteUrl>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Serialize_City_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, city) = City.TryCreate("New York");

        // Act
        var json = JsonSerializer.Serialize(city, _options);

        // Assert
        json.ShouldBe("\"New York\"");
    }

    [Fact]
    public void Deserialize_City_Should_Read_From_String()
    {
        // Arrange
        var json = "\"London\"";

        // Act
        var city = JsonSerializer.Deserialize<City>(json, _options);

        // Assert
        city.ShouldNotBeNull();
        city.Value.ShouldBe("London");
    }

    [Fact]
    public void RoundTrip_City_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = City.TryCreate("Tokyo");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<City>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Serialize_StateProvince_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, stateProvince) = StateProvince.TryCreate("California");

        // Act
        var json = JsonSerializer.Serialize(stateProvince, _options);

        // Assert
        json.ShouldBe("\"California\"");
    }

    [Fact]
    public void Deserialize_StateProvince_Should_Read_From_String()
    {
        // Arrange
        var json = "\"Ontario\"";

        // Act
        var stateProvince = JsonSerializer.Deserialize<StateProvince>(json, _options);

        // Assert
        stateProvince.ShouldNotBeNull();
        stateProvince.Value.ShouldBe("Ontario");
    }

    [Fact]
    public void RoundTrip_StateProvince_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = StateProvince.TryCreate("Texas");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<StateProvince>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Serialize_Object_With_Multiple_ValueObjects_Should_Work()
    {
        // Arrange
        var (_, addressLine) = AddressLine.TryCreate("123 Main St");
        var (_, city) = City.TryCreate("Springfield");
        var (_, state) = StateProvince.TryCreate("Illinois");
        var obj = new
        {
            Street = addressLine,
            City = city,
            State = state
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"street\":\"123 Main St\"");
        json.ShouldContain("\"city\":\"Springfield\"");
        json.ShouldContain("\"state\":\"Illinois\"");
    }

    [Fact]
    public void Deserialize_Object_With_Multiple_ValueObjects_Should_Work()
    {
        // Arrange
        var json = "{\"street\":\"123 Main St\",\"city\":\"Springfield\",\"state\":\"Illinois\"}";

        // Act
        var obj = JsonSerializer.Deserialize<AddressTestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.Street.Value.ShouldBe("123 Main St");
        obj.City.Value.ShouldBe("Springfield");
        obj.State.Value.ShouldBe("Illinois");
    }

    private class AddressTestObject
    {
        public AddressLine Street { get; set; } = null!;
        public City City { get; set; } = null!;
        public StateProvince State { get; set; } = null!;
    }
}
