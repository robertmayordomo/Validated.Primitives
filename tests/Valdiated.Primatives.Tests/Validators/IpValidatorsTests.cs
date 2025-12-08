using Xunit;
using Shouldly;
using Validated.Primitives.Validators;

namespace Validated.Primitives.Tests.Validators;

public class IpValidatorsTests
{
    public static TheoryData<string> ValidIps => new()
    {
        "127.0.0.1",
        "::1",
        "192.168.1.1",
        "0.0.0.0",
        "255.255.255.255",
        "10.0.0.1",
        "172.16.0.1",
        "2001:0db8:85a3:0000:0000:8a2e:0370:7334",
        "2001:db8:85a3::8a2e:370:7334",
        "fe80::1",
        "::ffff:192.0.2.1"
    };

    public static TheoryData<string> InvalidIps => new()
    {
        "",
        "   ",
        "not-an-ip",
        "256.256.256.256",
        "192.168.1",           // Shorthand - only 3 octets
        "192.168",             // Shorthand - only 2 octets
        "192",                 // Shorthand - only 1 octet
        "192.168.1.1.1",       // Too many octets
        "192.168.-1.1",        // Negative number
        "192.168.1.256",       // Octet out of range
        "gggg::1",             // Invalid hex
        ":::",                 // Invalid format
        "192.168.1.1.",        // Trailing dot
        ".192.168.1.1",        // Leading dot
        "192 168 1 1",         // Spaces
        "192.168.1.1/24",      // CIDR notation
        "192.168.001.1",       // Leading zeros
        "192.168.01.1"         // Leading zeros
    };

    [Theory]
    [MemberData(nameof(ValidIps))]
    public void GivenIpAddress_WhenValidIp_ThenShouldSucceed(string ip)
    {
        var result = IpValidators.IpAddress()(ip);
        result.IsValid.ShouldBeTrue($"Expected valid IP: {ip}");
    }

    [Theory]
    [MemberData(nameof(InvalidIps))]
    public void GivenIpAddress_WhenInvalidIp_ThenShouldFail(string ip)
    {
        var result = IpValidators.IpAddress()(ip);
        result.IsValid.ShouldBeFalse($"Expected invalid IP: {ip}");
    }

    [Fact]
    public void GivenIpAddress_WhenInvalid_ThenShouldReturnCorrectErrorMessage()
    {
        var result = IpValidators.IpAddress("ServerIp")("invalid");
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Message.ShouldBe("Invalid IP address format.");
        result.Errors[0].MemberName.ShouldBe("ServerIp");
        result.Errors[0].Code.ShouldBe("IpAddress");
    }

    [Fact]
    public void GivenIpAddress_WhenNoFieldNameProvided_ThenShouldUseDefault()
    {
        var result = IpValidators.IpAddress()("invalid");
        result.Errors[0].MemberName.ShouldBe("IpAddress");
    }

    [Theory]
    [InlineData("192.168.1.1", "ServerIp")]
    [InlineData("::1", "ClientIp")]
    public void GivenIpAddressWithCustomFieldName_WhenValid_ThenShouldSucceed(string ip, string fieldName)
    {
        var result = IpValidators.IpAddress(fieldName)(ip);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenIpAddress_WhenIPv4OrIPv6_ThenShouldHandleBoth()
    {
        var ipv4Result = IpValidators.IpAddress()("192.168.1.1");
        ipv4Result.IsValid.ShouldBeTrue();

        var ipv6Result = IpValidators.IpAddress()("2001:db8::1");
        ipv6Result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("192.168.1")]
    [InlineData("10.0")]
    [InlineData("127")]
    public void GivenIpAddress_WhenShorthandIPv4Notation_ThenShouldReject(string ip)
    {
        var result = IpValidators.IpAddress()(ip);
        result.IsValid.ShouldBeFalse($"Shorthand IPv4 should be rejected: {ip}");
    }

    [Theory]
    [InlineData("192.168.001.1")]
    [InlineData("192.168.01.1")]
    [InlineData("192.168.1.001")]
    public void GivenIpAddress_WhenLeadingZerosInIPv4_ThenShouldReject(string ip)
    {
        var result = IpValidators.IpAddress()(ip);
        result.IsValid.ShouldBeFalse($"IPv4 with leading zeros should be rejected: {ip}");
    }
}
