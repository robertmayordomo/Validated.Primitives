using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class DateTimeValueObjectSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    #region FutureDate Tests

    [Fact]
    public void Serialize_FutureDate_Should_Write_DateTime_Value()
    {
        // Arrange
        var futureDateTime = DateTime.UtcNow.AddDays(10);
        var (_, futureDate) = FutureDate.TryCreate(futureDateTime);

        // Act
        var json = JsonSerializer.Serialize(futureDate, _options);

        // Assert
        json.ShouldNotBeNullOrWhiteSpace();
        json.ShouldContain(futureDateTime.Year.ToString());
    }

    [Fact]
    public void Deserialize_FutureDate_Should_Read_From_DateTime_String()
    {
        // Arrange
        var futureDateTime = DateTime.UtcNow.AddDays(5);
        var json = JsonSerializer.Serialize(futureDateTime);

        // Act
        var futureDate = JsonSerializer.Deserialize<FutureDate>(json, _options);

        // Assert
        futureDate.ShouldNotBeNull();
        futureDate.Value.Date.ShouldBe(futureDateTime.Date);
    }

    [Fact]
    public void RoundTrip_FutureDate_Should_Preserve_Value()
    {
        // Arrange
        var futureDateTime = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        var (_, original) = FutureDate.TryCreate(futureDateTime);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<FutureDate>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
    }

    [Fact]
    public void Deserialize_Past_Date_Should_Throw_JsonException()
    {
        // Arrange
        var pastDateTime = DateTime.UtcNow.AddDays(-10);
        var json = JsonSerializer.Serialize(pastDateTime);

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<FutureDate>(json, _options));
    }

    [Fact]
    public void Serialize_Null_FutureDate_Should_Write_Null()
    {
        // Arrange
        FutureDate? futureDate = null;

        // Act
        var json = JsonSerializer.Serialize(futureDate, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_FutureDate_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var futureDate = JsonSerializer.Deserialize<FutureDate>(json, _options);

        // Assert
        futureDate.ShouldBeNull();
    }

    #endregion

    #region DateOfBirth Tests

    [Fact]
    public void Serialize_DateOfBirth_Should_Write_DateTime_Value()
    {
        // Arrange
        var birthDateTime = new DateTime(1990, 5, 15);
        var (_, dateOfBirth) = DateOfBirth.TryCreate(birthDateTime);

        // Act
        var json = JsonSerializer.Serialize(dateOfBirth, _options);

        // Assert
        json.ShouldNotBeNullOrWhiteSpace();
        json.ShouldContain("1990");
    }

    [Fact]
    public void Deserialize_DateOfBirth_Should_Read_From_DateTime_String()
    {
        // Arrange
        var birthDateTime = new DateTime(1985, 3, 20);
        var json = JsonSerializer.Serialize(birthDateTime);

        // Act
        var dateOfBirth = JsonSerializer.Deserialize<DateOfBirth>(json, _options);

        // Assert
        dateOfBirth.ShouldNotBeNull();
        dateOfBirth.Value.Date.ShouldBe(birthDateTime.Date);
    }

    [Fact]
    public void RoundTrip_DateOfBirth_Should_Preserve_Value()
    {
        // Arrange
        var birthDateTime = new DateTime(1975, 8, 10, 14, 30, 0);
        var (_, original) = DateOfBirth.TryCreate(birthDateTime);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<DateOfBirth>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
    }

    [Fact]
    public void Deserialize_Future_Date_As_DateOfBirth_Should_Throw_JsonException()
    {
        // Arrange
        var futureDateTime = DateTime.UtcNow.AddYears(1);
        var json = JsonSerializer.Serialize(futureDateTime);

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<DateOfBirth>(json, _options));
    }

    [Fact]
    public void Serialize_Null_DateOfBirth_Should_Write_Null()
    {
        // Arrange
        DateOfBirth? dateOfBirth = null;

        // Act
        var json = JsonSerializer.Serialize(dateOfBirth, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_DateOfBirth_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var dateOfBirth = JsonSerializer.Deserialize<DateOfBirth>(json, _options);

        // Assert
        dateOfBirth.ShouldBeNull();
    }

    #endregion

    #region Complex Object Tests

    [Fact]
    public void Serialize_Object_With_DateTime_ValueObjects_Should_Work()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var appointmentDate = DateTime.UtcNow.AddMonths(2);
        var (_, dob) = DateOfBirth.TryCreate(birthDate);
        var (_, futureDate) = FutureDate.TryCreate(appointmentDate);
        var obj = new
        {
            DateOfBirth = dob,
            Appointment = futureDate
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldNotBeNullOrWhiteSpace();
        json.ShouldContain("\"dateOfBirth\":");
        json.ShouldContain("\"appointment\":");
    }

    [Fact]
    public void Deserialize_Object_With_DateTime_ValueObjects_Should_Work()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var appointmentDate = DateTime.UtcNow.AddMonths(2);
        var obj = new
        {
            DateOfBirth = birthDate,
            Appointment = appointmentDate
        };
        var json = JsonSerializer.Serialize(obj, _options);

        // Act
        var result = JsonSerializer.Deserialize<PersonTestObject>(json, _options);

        // Assert
        result.ShouldNotBeNull();
        result.DateOfBirth.ShouldNotBeNull();
        result.DateOfBirth.Value.Date.ShouldBe(birthDate.Date);
        result.Appointment.ShouldNotBeNull();
        result.Appointment.Value.Date.ShouldBe(appointmentDate.Date);
    }

    [Fact]
    public void Serialize_Array_Of_DateOfBirth_Should_Work()
    {
        // Arrange
        var (_, dob1) = DateOfBirth.TryCreate(new DateTime(1990, 1, 1));
        var (_, dob2) = DateOfBirth.TryCreate(new DateTime(1985, 5, 15));
        var (_, dob3) = DateOfBirth.TryCreate(new DateTime(2000, 12, 31));
        var dates = new[] { dob1, dob2, dob3 };

        // Act
        var json = JsonSerializer.Serialize(dates, _options);

        // Assert
        json.ShouldNotBeNullOrWhiteSpace();
        json.ShouldStartWith("[");
        json.ShouldEndWith("]");
    }

    [Fact]
    public void Deserialize_Array_Of_DateOfBirth_Should_Work()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(1990, 1, 1),
            new DateTime(1985, 5, 15),
            new DateTime(2000, 12, 31)
        };
        var json = JsonSerializer.Serialize(dates, _options);

        // Act
        var result = JsonSerializer.Deserialize<DateOfBirth[]>(json, _options);

        // Assert
        result.ShouldNotBeNull();
        result.Length.ShouldBe(3);
        result[0].Value.Date.ShouldBe(dates[0].Date);
        result[1].Value.Date.ShouldBe(dates[1].Date);
        result[2].Value.Date.ShouldBe(dates[2].Date);
    }

    private class PersonTestObject
    {
        public DateOfBirth DateOfBirth { get; set; } = null!;
        public FutureDate Appointment { get; set; } = null!;
    }

    #endregion
}
