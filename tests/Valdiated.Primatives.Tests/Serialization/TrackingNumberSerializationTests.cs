using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class TrackingNumberSerializationTests
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void Serialize_TrackingNumber_Should_Write_Value_As_String()
    {
        // Arrange
        var (_, trackingNumber) = TrackingNumber.TryCreate("1Z999AA10123456784");

        // Act
        var json = JsonSerializer.Serialize(trackingNumber, _options);

        // Assert
        json.ShouldBe("\"1Z999AA10123456784\"");
    }

    [Fact]
    public void Deserialize_TrackingNumber_Should_Read_From_String()
    {
        // Arrange
        var json = "\"1Z999AA10123456784\"";

        // Act
        var trackingNumber = JsonSerializer.Deserialize<TrackingNumber>(json, _options);

        // Assert
        trackingNumber.ShouldNotBeNull();
        trackingNumber.Value.ShouldBe("1Z999AA10123456784");
        trackingNumber.Format.ShouldBe(TrackingNumberFormat.UPS);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_Value()
    {
        // Arrange
        var (_, original) = TrackingNumber.TryCreate("986578788855");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<TrackingNumber>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.Format.ShouldBe(original.Format);
        deserialized.ShouldBe(original);
    }

    [Theory]
    [InlineData("1Z999AA10123456784", TrackingNumberFormat.UPS)]
    [InlineData("986578788855", TrackingNumberFormat.FedExExpress)]
    [InlineData("100123456789012", TrackingNumberFormat.FedExGround)]
    [InlineData("9261292700768711896765", TrackingNumberFormat.FedExSmartPost)]
    [InlineData("94001116990118965530", TrackingNumberFormat.USPS)]
    [InlineData("1234567890", TrackingNumberFormat.DHLExpress)]
    [InlineData("GM01234567890123456789", TrackingNumberFormat.DHLEcommerce)]
    [InlineData("TBA123456789012", TrackingNumberFormat.AmazonLogistics)]
    [InlineData("RX123456789GB", TrackingNumberFormat.RoyalMail)]
    [InlineData("1234567890123456", TrackingNumberFormat.CanadaPost)]
    [InlineData("RX123456789IE", TrackingNumberFormat.IrishPost)]
    [InlineData("C12345678901234", TrackingNumberFormat.OnTrac)]
    public void Deserialize_Should_Preserve_Format(string trackingNumberValue, TrackingNumberFormat expectedFormat)
    {
        // Arrange
        var json = $"\"{trackingNumberValue}\"";

        // Act
        var trackingNumber = JsonSerializer.Deserialize<TrackingNumber>(json, _options);

        // Assert
        trackingNumber.ShouldNotBeNull();
        trackingNumber.Format.ShouldBe(expectedFormat);
    }

    [Fact]
    public void Deserialize_Invalid_TrackingNumber_Should_Throw_JsonException()
    {
        // Arrange
        var json = "\"INVALID\"";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<TrackingNumber>(json, _options));
    }

    [Fact]
    public void Serialize_Null_TrackingNumber_Should_Write_Null()
    {
        // Arrange
        TrackingNumber? trackingNumber = null;

        // Act
        var json = JsonSerializer.Serialize(trackingNumber, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var trackingNumber = JsonSerializer.Deserialize<TrackingNumber>(json, _options);

        // Assert
        trackingNumber.ShouldBeNull();
    }

    [Fact]
    public void Serialize_TrackingNumber_In_Object_Should_Work()
    {
        // Arrange
        var (_, trackingNumber) = TrackingNumber.TryCreate("1Z999AA10123456784");
        var obj = new { ShipmentTracking = trackingNumber, OrderId = "ORD-12345" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"shipmentTracking\":\"1Z999AA10123456784\"");
        json.ShouldContain("\"orderId\":\"ORD-12345\"");
    }

    [Fact]
    public void Deserialize_TrackingNumber_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"shipmentTracking\":\"1Z999AA10123456784\",\"orderId\":\"ORD-12345\"}";

        // Act
        var obj = JsonSerializer.Deserialize<ShipmentObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.ShipmentTracking.ShouldNotBeNull();
        obj.ShipmentTracking.Value.ShouldBe("1Z999AA10123456784");
        obj.ShipmentTracking.Format.ShouldBe(TrackingNumberFormat.UPS);
        obj.OrderId.ShouldBe("ORD-12345");
    }

    [Fact]
    public void Serialize_Multiple_Carrier_TrackingNumbers_In_Object_Should_Work()
    {
        // Arrange
        var (_, ups) = TrackingNumber.TryCreate("1Z999AA10123456784");
        var (_, fedex) = TrackingNumber.TryCreate("986578788855");
        var (_, usps) = TrackingNumber.TryCreate("94001116990118965530");

        var obj = new
        {
            UpsTracking = ups,
            FedExTracking = fedex,
            UspsTracking = usps
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var deserialized = JsonSerializer.Deserialize<MultipleTrackingNumbersObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.UpsTracking.ShouldNotBeNull();
        deserialized.UpsTracking.Format.ShouldBe(TrackingNumberFormat.UPS);
        deserialized.FedExTracking.ShouldNotBeNull();
        deserialized.FedExTracking.Format.ShouldBe(TrackingNumberFormat.FedExExpress);
        deserialized.UspsTracking.ShouldNotBeNull();
        deserialized.UspsTracking.Format.ShouldBe(TrackingNumberFormat.USPS);
    }

    [Fact]
    public void Serialize_ShipmentWithMultipleCarriers_Should_Work()
    {
        // Arrange
        var shipment = new ShipmentWithCarriers
        {
            OrderId = "ORD-99999",
            PrimaryTracking = TrackingNumber.TryCreate("1Z999AA10123456784").Value!,
            SecondaryTracking = TrackingNumber.TryCreate("986578788855").Value,
            InternationalTracking = TrackingNumber.TryCreate("RX123456789GB").Value
        };

        // Act
        var json = JsonSerializer.Serialize(shipment, _options);
        var deserialized = JsonSerializer.Deserialize<ShipmentWithCarriers>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.OrderId.ShouldBe("ORD-99999");
        deserialized.PrimaryTracking.Format.ShouldBe(TrackingNumberFormat.UPS);
        deserialized.SecondaryTracking.ShouldNotBeNull();
        deserialized.SecondaryTracking!.Format.ShouldBe(TrackingNumberFormat.FedExExpress);
        deserialized.InternationalTracking.ShouldNotBeNull();
        deserialized.InternationalTracking!.Format.ShouldBe(TrackingNumberFormat.RoyalMail);
    }

    [Fact]
    public void Deserialize_Should_Handle_Lowercase_Input()
    {
        // Arrange
        var json = "\"1z999aa10123456784\"";

        // Act
        var trackingNumber = JsonSerializer.Deserialize<TrackingNumber>(json, _options);

        // Assert
        trackingNumber.ShouldNotBeNull();
        trackingNumber.GetNormalized().ShouldBe("1Z999AA10123456784");
        trackingNumber.Format.ShouldBe(TrackingNumberFormat.UPS);
    }

    [Fact]
    public void Deserialize_Should_Handle_Input_With_Separators()
    {
        // Arrange
        var json = "\"1Z-999AA-1012345-6784\"";

        // Act
        var trackingNumber = JsonSerializer.Deserialize<TrackingNumber>(json, _options);

        // Assert
        trackingNumber.ShouldNotBeNull();
        trackingNumber.GetNormalized().ShouldBe("1Z999AA10123456784");
        trackingNumber.Format.ShouldBe(TrackingNumberFormat.UPS);
    }

    private class ShipmentObject
    {
        public TrackingNumber ShipmentTracking { get; set; } = null!;
        public string OrderId { get; set; } = null!;
    }

    private class MultipleTrackingNumbersObject
    {
        public TrackingNumber UpsTracking { get; set; } = null!;
        public TrackingNumber FedExTracking { get; set; } = null!;
        public TrackingNumber UspsTracking { get; set; } = null!;
    }

    private class ShipmentWithCarriers
    {
        public string OrderId { get; set; } = null!;
        public TrackingNumber PrimaryTracking { get; set; } = null!;
        public TrackingNumber? SecondaryTracking { get; set; }
        public TrackingNumber? InternationalTracking { get; set; }
    }
}
