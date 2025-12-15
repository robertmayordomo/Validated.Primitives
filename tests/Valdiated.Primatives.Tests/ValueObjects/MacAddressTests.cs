using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class MacAddressTests
{
    #region Format Validation Tests

    [Theory]
    [InlineData("AA:BB:CC:DD:EE:FF")]
    [InlineData("00:11:22:33:44:55")]
    [InlineData("aa:bb:cc:dd:ee:ff")]
    [InlineData("Aa:Bb:Cc:Dd:Ee:Ff")]
    public void TryCreate_WithColonSeparatedFormat_ReturnsSuccess(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        mac!.Value.ShouldBe(macAddress.ToUpperInvariant());
    }

    [Theory]
    [InlineData("AA-BB-CC-DD-EE-FF")]
    [InlineData("00-11-22-33-44-55")]
    [InlineData("aa-bb-cc-dd-ee-ff")]
    [InlineData("Aa-Bb-Cc-Dd-Ee-Ff")]
    public void TryCreate_WithHyphenSeparatedFormat_ReturnsSuccess(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        // Should be normalized to colon-separated uppercase
        mac!.Value.ShouldBe(macAddress.Replace('-', ':').ToUpperInvariant());
    }

    [Theory]
    [InlineData("AABB.CCDD.EEFF")]
    [InlineData("0011.2233.4455")]
    [InlineData("aabb.ccdd.eeff")]
    [InlineData("AaBb.CcDd.EeFf")]
    public void TryCreate_WithDotSeparatedCiscoFormat_ReturnsSuccess(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        // Should be normalized to colon-separated uppercase
        var normalized = macAddress.Replace(".", "").ToUpperInvariant();
        var expected = $"{normalized.Substring(0, 2)}:{normalized.Substring(2, 2)}:{normalized.Substring(4, 2)}:" +
                      $"{normalized.Substring(6, 2)}:{normalized.Substring(8, 2)}:{normalized.Substring(10, 2)}";
        mac!.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("AABBCCDDEEFF")]
    [InlineData("001122334455")]
    [InlineData("aabbccddeeff")]
    [InlineData("AaBbCcDdEeFf")]
    public void TryCreate_WithContinuousFormat_ReturnsSuccess(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        // Should be normalized to colon-separated uppercase
        var normalized = macAddress.ToUpperInvariant();
        var expected = $"{normalized.Substring(0, 2)}:{normalized.Substring(2, 2)}:{normalized.Substring(4, 2)}:" +
                      $"{normalized.Substring(6, 2)}:{normalized.Substring(8, 2)}:{normalized.Substring(10, 2)}";
        mac!.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void TryCreate_WithNullOrWhitespace_ReturnsFailure(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeFalse();
        mac.ShouldBeNull();
    }

    [Theory]
    [InlineData("GG:HH:II:JJ:KK:LL")] // Invalid hex characters
    [InlineData("AA:BB:CC:DD:EE")]    // Too short
    [InlineData("AA:BB:CC:DD:EE:FF:00")] // Too long
    [InlineData("AABB.CCDD")]          // Incomplete dot format
    [InlineData("AA:BB:CC:DD:EE:FG")]  // Invalid hex in last octet
    [InlineData("AA BB CC DD EE FF")]  // Space separator (not supported)
    public void TryCreate_WithInvalidFormat_ReturnsFailure(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeFalse();
        mac.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidFormat" || e.Code == "Required");
    }

    #endregion

    #region Special Address Validation Tests

    [Fact]
    public void TryCreate_WithBroadcastAddress_ReturnsFailure()
    {
        // Arrange
        var broadcastMac = "FF:FF:FF:FF:FF:FF";

        // Act
        var (result, mac) = MacAddress.TryCreate(broadcastMac);

        // Assert
        result.IsValid.ShouldBeFalse();
        mac.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "BroadcastAddress");
    }

    [Fact]
    public void TryCreate_WithBroadcastAddressAllowed_ReturnsSuccess()
    {
        // Arrange
        var broadcastMac = "FF:FF:FF:FF:FF:FF";

        // Act
        var (result, mac) = MacAddress.TryCreate(broadcastMac, allowBroadcast: true);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        mac!.Value.ShouldBe("FF:FF:FF:FF:FF:FF");
    }

    [Fact]
    public void TryCreate_WithAllZerosAddress_ReturnsFailure()
    {
        // Arrange
        var zeroMac = "00:00:00:00:00:00";

        // Act
        var (result, mac) = MacAddress.TryCreate(zeroMac);

        // Assert
        result.IsValid.ShouldBeFalse();
        mac.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "AllZeros");
    }

    [Fact]
    public void TryCreate_WithAllZerosAddressAllowed_ReturnsSuccess()
    {
        // Arrange
        var zeroMac = "00:00:00:00:00:00";

        // Act
        var (result, mac) = MacAddress.TryCreate(zeroMac, allowAllZeros: true);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        mac!.Value.ShouldBe("00:00:00:00:00:00");
    }

    [Theory]
    [InlineData("01:00:5E:00:00:00")] // IPv4 multicast
    [InlineData("33:33:00:00:00:00")] // IPv6 multicast
    [InlineData("FF:FF:FF:FF:FF:FF")] // Broadcast (also multicast)
    public void TryCreate_WithMulticastAddress_ReturnsFailure(string multicastMac)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(multicastMac);

        // Assert
        result.IsValid.ShouldBeFalse();
        mac.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "MulticastAddress" || e.Code == "BroadcastAddress");
    }

    [Fact]
    public void TryCreate_WithMulticastAddressAllowed_ReturnsSuccess()
    {
        // Arrange
        var multicastMac = "01:00:5E:00:00:01";

        // Act
        var (result, mac) = MacAddress.TryCreate(multicastMac, allowMulticast: true);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
        mac!.Value.ShouldBe("01:00:5E:00:00:01");
    }

    [Theory]
    [InlineData("00:11:22:33:44:55")] // Unicast
    [InlineData("AA:BB:CC:DD:EE:FE")] // Unicast
    [InlineData("02:00:00:00:00:02")] // Locally administered unicast
    public void TryCreate_WithUnicastAddress_ReturnsSuccess(string unicastMac)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(unicastMac);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
    }

    #endregion

    #region Format Conversion Tests

    [Fact]
    public void ToString_ReturnsColonSeparatedFormat()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AABBCCDDEEFF");

        // Act
        var result = mac!.ToString();

        // Assert
        result.ShouldBe("AA:BB:CC:DD:EE:FF");
    }

    [Fact]
    public void ToHyphenFormat_ReturnsHyphenSeparatedFormat()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

        // Act
        var result = mac!.ToHyphenFormat();

        // Assert
        result.ShouldBe("AA-BB-CC-DD-EE-FF");
    }

    [Fact]
    public void ToDotFormat_ReturnsCiscoFormat()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

        // Act
        var result = mac!.ToDotFormat();

        // Assert
        result.ShouldBe("AABB.CCDD.EEFF");
    }

    [Fact]
    public void ToContinuousFormat_ReturnsNoSeparators()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

        // Act
        var result = mac!.ToContinuousFormat();

        // Assert
        result.ShouldBe("AABBCCDDEEFF");
    }

    #endregion

    #region OUI and NIC Tests

    [Fact]
    public void GetOUI_ReturnsFirstThreeOctets()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

        // Act
        var oui = mac!.GetOUI();

        // Assert
        oui.ShouldBe("AA:BB:CC");
    }

    [Fact]
    public void GetNIC_ReturnsLastThreeOctets()
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

        // Act
        var nic = mac!.GetNIC();

        // Assert
        nic.ShouldBe("DD:EE:FF");
    }

    #endregion

    #region Address Type Tests

    [Theory]
    [InlineData("02:00:00:00:00:02", true)]  // Locally administered (bit 1 of first octet set)
    [InlineData("06:00:00:00:00:01", true)]  // Locally administered
    [InlineData("00:11:22:33:44:55", false)] // Universally administered
    [InlineData("A8:BB:CC:DD:EE:FE", false)] // Universally administered (0xA8 = 10101000, bit 1 = 0)
    public void IsLocallyAdministered_ReturnsCorrectValue(string macAddress, bool expected)
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate(macAddress);

        // Act & Assert
        mac!.IsLocallyAdministered().ShouldBe(expected);
        mac.IsUniversallyAdministered().ShouldBe(!expected);
    }

    [Theory]
    [InlineData("01:00:5E:00:00:01", true)]  // Multicast (bit 0 of first octet set)
    [InlineData("33:33:00:00:00:01", true)]  // Multicast
    [InlineData("FF:FF:FF:FF:FF:FF", true)]  // Broadcast (also multicast)
    [InlineData("00:11:22:33:44:55", false)] // Unicast
    [InlineData("AA:BB:CC:DD:EE:FE", false)] // Unicast
    public void IsMulticast_ReturnsCorrectValue(string macAddress, bool expected)
    {
        // Arrange
        var (_, mac) = MacAddress.TryCreate(macAddress, allowMulticast: true, allowBroadcast: true);

        // Act & Assert
        mac!.IsMulticast().ShouldBe(expected);
        mac.IsUnicast().ShouldBe(!expected);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameValueDifferentFormat_ReturnsTrue()
    {
        // Arrange
        var (_, mac1) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");
        var (_, mac2) = MacAddress.TryCreate("AA-BB-CC-DD-EE-FF");
        var (_, mac3) = MacAddress.TryCreate("AABBCCDDEEFF");

        // Act & Assert
        mac1.ShouldBe(mac2);
        mac1.ShouldBe(mac3);
        mac2.ShouldBe(mac3);
    }

    [Fact]
    public void Equals_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var (_, mac1) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");
        var (_, mac2) = MacAddress.TryCreate("11:22:33:44:55:66");

        // Act & Assert
        mac1.ShouldNotBe(mac2);
    }

    [Fact]
    public void GetHashCode_WithSameValueDifferentFormat_ReturnsSameHash()
    {
        // Arrange
        var (_, mac1) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");
        var (_, mac2) = MacAddress.TryCreate("AA-BB-CC-DD-EE-FF");

        // Act & Assert
        mac1!.GetHashCode().ShouldBe(mac2!.GetHashCode());
    }

    #endregion

    #region Normalization Tests

    [Theory]
    [InlineData("aa:bb:cc:dd:ee:ff", "AA:BB:CC:DD:EE:FF")]
    [InlineData("AA-bb-CC-dd-EE-ff", "AA:BB:CC:DD:EE:FF")]
    [InlineData("aAbB.cCdD.eEfF", "AA:BB:CC:DD:EE:FF")]
    [InlineData("aabbccddeeff", "AA:BB:CC:DD:EE:FF")]
    public void TryCreate_NormalizesToUppercaseColonFormat(string input, string expected)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(input);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac!.Value.ShouldBe(expected);
    }

    #endregion

    #region Real World Examples

    [Theory]
    [InlineData("00:1A:2B:3C:4D:5E")] // Generic device
    [InlineData("00:0C:29:12:34:56")] // VMware
    [InlineData("08:00:27:AB:CD:EF")] // VirtualBox
    [InlineData("52:54:00:12:34:56")] // QEMU/KVM
    [InlineData("B8:27:EB:11:22:33")] // Raspberry Pi
    public void TryCreate_WithRealWorldMacAddresses_ReturnsSuccess(string macAddress)
    {
        // Act
        var (result, mac) = MacAddress.TryCreate(macAddress);

        // Assert
        result.IsValid.ShouldBeTrue();
        mac.ShouldNotBeNull();
    }

    #endregion
}
