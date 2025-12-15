using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class TrackingNumberTests
{
    public static TheoryData<string> ValidTrackingNumbers => new()
    {
        // UPS (18 characters starting with "1Z")
        "1Z999AA10123456784",
        "1Z12345E0205271688",
        "1Z12345E6605272234",
        
        // FedEx Express (12 digits)
        "986578788855",
        "477179081230",
        "799531274483",
        
        // FedEx Ground (15 digits)
        "100123456789012",
        "987654321098765",
        
        // FedEx SmartPost (22 digits)
        "9261292700768711896765",
        "9400111899223835966083",
        
        // USPS (20-22 alphanumeric)
        "94001116990118965530",
        "9400111699011896653000",
        "EA123456785US",
        
        // DHL Express (10 digits)
        "1234567890",
        "9876543210",
        
        // DHL eCommerce (22 alphanumeric starting with "GM")
        "GM01234567890123456789",
        "GM98765432109876543210",
        
        // DHL Global Mail (13-16 alphanumeric)
        "ABC1234567890",  // 13 alphanumeric (has letters)
        "12345678901234",  // 14 digits
        "ABC123456789012", // 15 alphanumeric (has letters)
        "ABCD123456789012", // 16 alphanumeric (has letters)
        
        // Amazon Logistics ("TBA" + 12 digits)
        "TBA123456789012",
        "TBA987654321098",
        
        // Royal Mail (2 letters + 9 digits + 2 letters)
        "RX123456789GB",
        "EA987654321GB",
        "CP123456789GB",
        
        // Canada Post (16 alphanumeric)
        "1234567890123456",
        "ABCD1234EFGH5678",
        
        // Australia Post (13 digits)
        "1234567890123",
        "9876543210987",
        
        // TNT (9 or 13 digits)
        "123456789",
        "1234567890123",
        
        // China Post (2 letters + 9 digits + 2 letters)
        "RX123456789CN",
        "CP987654321CN",
        
        // Irish Post (2 letters + 9 digits + 2 letters)
        "RX123456789IE",
        "EA987654321IE",
        "CP123456789IE",
        
        // LaserShip ("1LS" + 12 digits)
        "1LS123456789012",
        "1LS987654321098",
        
        // OnTrac ("C" + 14 digits)
        "C12345678901234",
        "C98765432109876"
    };

    public static TheoryData<string> InvalidTrackingNumbers => new()
    {
        "",
        "   ",
        "123", // Too short
        "INVALID", // No valid format
        "1Z123", // UPS too short
        "2Z123456789012345678", // Not starting with 1Z
        "TBA12345", // Amazon too short
        "1LS1234", // LaserShip too short
        "C123", // OnTrac too short
        "GM123", // DHL eCommerce too short
    };

    [Theory]
    [MemberData(nameof(ValidTrackingNumbers))]
    public void TryCreate_Succeeds_For_Valid_TrackingNumbers(string trackingNumber)
    {
        // Act
        var (result, value) = TrackingNumber.TryCreate(trackingNumber);

        // Assert
        result.IsValid.ShouldBeTrue($"Result should be valid for tracking number: {trackingNumber}");
        value.ShouldNotBeNull($"Value should not be null when validation succeeds for: {trackingNumber}");
        value!.Value.ShouldBe(trackingNumber);
    }

    [Theory]
    [MemberData(nameof(InvalidTrackingNumbers))]
    public void TryCreate_Fails_For_Invalid_TrackingNumbers(string trackingNumber)
    {
        // Act
        var (result, value) = TrackingNumber.TryCreate(trackingNumber);

        // Assert
        result.IsValid.ShouldBeFalse($"Result should be invalid for tracking number: {trackingNumber}");
        value.ShouldBeNull($"Value should be null when validation fails for: {trackingNumber}");
    }

    [Fact]
    public void GetNormalized_Removes_Separators_And_Converts_To_UpperCase()
    {
        // Arrange
        var (_, trackingNumber) = TrackingNumber.TryCreate("1z-999aa-1012345-6784");

        // Act
        var normalized = trackingNumber!.GetNormalized();

        // Assert
        normalized.ShouldBe("1Z999AA10123456784");
    }

    [Theory]
    [InlineData("1Z999AA10123456784", TrackingNumberFormat.UPS, "UPS")]
    [InlineData("986578788855", TrackingNumberFormat.FedExExpress, "FedEx Express")]
    [InlineData("100123456789012", TrackingNumberFormat.FedExGround, "FedEx Ground")]
    [InlineData("9261292700768711896765", TrackingNumberFormat.FedExSmartPost, "FedEx SmartPost")]
    [InlineData("94001116990118965530", TrackingNumberFormat.USPS, "USPS")]
    [InlineData("1234567890", TrackingNumberFormat.DHLExpress, "DHL Express")]
    [InlineData("GM01234567890123456789", TrackingNumberFormat.DHLEcommerce, "DHL eCommerce")]
    [InlineData("ABC1234567890", TrackingNumberFormat.DHLGlobalMail, "DHL Global Mail")]
    [InlineData("TBA123456789012", TrackingNumberFormat.AmazonLogistics, "Amazon Logistics")]
    [InlineData("RX123456789GB", TrackingNumberFormat.RoyalMail, "Royal Mail")]
    [InlineData("1234567890123456", TrackingNumberFormat.CanadaPost, "Canada Post")]
    [InlineData("1234567890123", TrackingNumberFormat.AustraliaPost, "Australia Post")]
    [InlineData("123456789", TrackingNumberFormat.TNT, "TNT")]
    [InlineData("RX123456789CN", TrackingNumberFormat.ChinaPost, "China Post")]
    [InlineData("RX123456789IE", TrackingNumberFormat.IrishPost, "Irish Post")]
    [InlineData("1LS123456789012", TrackingNumberFormat.LaserShip, "LaserShip")]
    [InlineData("C12345678901234", TrackingNumberFormat.OnTrac, "OnTrac")]
    public void Format_Is_Detected_Correctly(string trackingNumber, TrackingNumberFormat expectedFormat, string expectedCarrierName)
    {
        // Act
        var (_, value) = TrackingNumber.TryCreate(trackingNumber);

        // Assert
        value.ShouldNotBeNull();
        value!.Format.ShouldBe(expectedFormat);
        value.GetCarrierName().ShouldBe(expectedCarrierName);
    }

    [Fact]
    public void ToString_Returns_Original_Value()
    {
        // Arrange
        var originalValue = "1Z999AA10123456784";
        var (_, trackingNumber) = TrackingNumber.TryCreate(originalValue);

        // Act
        var stringValue = trackingNumber!.ToString();

        // Assert
        stringValue.ShouldBe(originalValue);
    }

    [Fact]
    public void Equal_TrackingNumbers_Should_Be_Equal()
    {
        // Arrange
        var (_, trackingNumber1) = TrackingNumber.TryCreate("1Z999AA10123456784");
        var (_, trackingNumber2) = TrackingNumber.TryCreate("1Z999AA10123456784");

        // Assert
        trackingNumber1.ShouldBe(trackingNumber2);
        trackingNumber1!.GetHashCode().ShouldBe(trackingNumber2!.GetHashCode());
    }

    [Fact]
    public void Different_TrackingNumbers_Should_Not_Be_Equal()
    {
        // Arrange
        var (_, trackingNumber1) = TrackingNumber.TryCreate("1Z999AA10123456784");
        var (_, trackingNumber2) = TrackingNumber.TryCreate("1Z12345E0205271688");

        // Assert
        trackingNumber1.ShouldNotBe(trackingNumber2);
    }

    [Fact]
    public void Custom_PropertyName_Appears_In_Validation_Error()
    {
        // Act
        var (result, _) = TrackingNumber.TryCreate("", "ShipmentTracking");

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.MemberName == "ShipmentTracking");
    }

    [Theory]
    [InlineData("1z999aa10123456784")] // Lowercase
    [InlineData("1Z-999AA-1012345-6784")] // With hyphens
    [InlineData("1Z 999AA 1012345 6784")] // With spaces
    public void Handles_Different_Input_Formats(string trackingNumber)
    {
        // Act
        var (result, value) = TrackingNumber.TryCreate(trackingNumber);

        // Assert
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.GetNormalized().ShouldBe("1Z999AA10123456784");
        value.Format.ShouldBe(TrackingNumberFormat.UPS);
    }

    [Fact]
    public void USPS_20_Digit_Format_Is_Validated()
    {
        // Arrange & Act
        var (result, value) = TrackingNumber.TryCreate("94001116990118965530");

        // Assert
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.Format.ShouldBe(TrackingNumberFormat.USPS);
    }

    [Fact]
    public void USPS_22_Alphanumeric_Format_Is_Validated()
    {
        // Arrange & Act
        var (result, value) = TrackingNumber.TryCreate("EA123456789US");

        // Assert
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.Format.ShouldBe(TrackingNumberFormat.USPS);
    }

    [Fact]
    public void TNT_9_Digit_Format_Is_Validated()
    {
        // Arrange & Act
        var (result, value) = TrackingNumber.TryCreate("123456789");

        // Assert
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.Format.ShouldBe(TrackingNumberFormat.TNT);
    }

    [Fact]
    public void AustraliaPost_And_TNT_13_Digit_Format_Are_Ambiguous()
    {
        // Arrange & Act
        // Both Australia Post and TNT use 13 digits, so we default to Australia Post
        var (result, value) = TrackingNumber.TryCreate("1234567890123");

        // Assert
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        // Note: 13-digit tracking numbers are ambiguous between Australia Post and TNT
        // Without additional context (like carrier hint), we default to Australia Post
        value!.Format.ShouldBe(TrackingNumberFormat.AustraliaPost);
    }
}
