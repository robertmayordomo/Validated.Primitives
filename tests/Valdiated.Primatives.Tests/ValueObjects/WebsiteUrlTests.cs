using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class WebsiteUrlTests
{
    public static TheoryData<string> ValidUrls => new()
    {
        "http://example.com",
        "https://example.com/path"
    };

    public static TheoryData<string> InvalidUrls => new()
    {
        "",
        "ftp://example.com",
        "not a url"
    };

    [Theory]
    [MemberData(nameof(ValidUrls))]
    public void TryCreate_Succeeds_For_Valid(string url)
    {
        var (res, val) = WebsiteUrl.TryCreate(url);
        res.IsValid.ShouldBeTrue($"Result should be valid for valid URL: {url}");
        val.ShouldNotBeNull($"Value should not be null when validation succeeds for: {url}");
    }

    [Theory]
    [MemberData(nameof(InvalidUrls))]
    public void TryCreate_Fails_For_Invalid(string url)
    {
        var (res, val) = WebsiteUrl.TryCreate(url);
        res.IsValid.ShouldBeFalse($"Result should be invalid for invalid URL: {url}");
        val.ShouldBeNull($"Value should be null when validation fails for: {url}");
    }
}
