using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class EmailAddressSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_EmailAddress_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, emailAddress) = EmailAddress.TryCreate("test@example.com");

        // Act
        var json = JsonSerializer.Serialize(emailAddress, _options);

        // Assert
        json.ShouldBe("\"test@example.com\"");
    }

    [Fact]
    public void Deserialize_EmailAddress_Should_Read_From_String()
    {
        // Arrange
        var json = "\"test@example.com\"";

        // Act
        var emailAddress = JsonSerializer.Deserialize<EmailAddress>(json, _options);

        // Assert
        emailAddress.ShouldNotBeNull();
        emailAddress.Value.ShouldBe("test@example.com");
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = EmailAddress.TryCreate("user@domain.com");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<EmailAddress>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void Deserialize_Invalid_EmailAddress_Should_Throw_JsonException()
    {
        // Arrange
        var json = "\"not-an-email\"";

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<EmailAddress>(json, _options));
    }

    [Fact]
    public void Serialize_Null_EmailAddress_Should_Write_Null()
    {
        // Arrange
        EmailAddress? emailAddress = null;

        // Act
        var json = JsonSerializer.Serialize(emailAddress, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var emailAddress = JsonSerializer.Deserialize<EmailAddress>(json, _options);

        // Assert
        emailAddress.ShouldBeNull();
    }

    [Fact]
    public void Serialize_EmailAddress_In_Object_Should_Work()
    {
        // Arrange
        var (_, emailAddress) = EmailAddress.TryCreate("contact@test.com");
        var obj = new { Email = emailAddress, Name = "John" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"email\":\"contact@test.com\"");
        json.ShouldContain("\"name\":\"John\"");
    }

    [Fact]
    public void Deserialize_EmailAddress_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"email\":\"contact@test.com\",\"name\":\"John\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.Email.ShouldNotBeNull();
        obj.Email.Value.ShouldBe("contact@test.com");
        obj.Name.ShouldBe("John");
    }

    private class TestObject
    {
        public EmailAddress Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
