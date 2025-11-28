using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class IpAddressTests
{
    public static TheoryData<string> ValidIps => new()
    {
        "127.0.0.1",
        "::1",
    };

    public static TheoryData<string> InvalidIps => new()
    {
        "",
        "not-an-ip",
    };

    [Theory]
    [MemberData(nameof(ValidIps))]
    public void TryCreate_Succeeds_For_Valid(string ip)
    {
        var (res, val) = IpAddress.TryCreate(ip);
        res.IsValid.ShouldBeTrue($"Result should be valid for valid IP address: {ip}");
        val.ShouldNotBeNull($"Value should not be null when validation succeeds for: {ip}");
    }

    [Theory]
    [MemberData(nameof(InvalidIps))]
    public void TryCreate_Fails_For_Invalid(string ip)
    {
        var (res, val) = IpAddress.TryCreate(ip);
        res.IsValid.ShouldBeFalse($"Result should be invalid for invalid IP address: {ip}");
        val.ShouldBeNull($"Value should be null when validation fails for: {ip}");
    }
}
