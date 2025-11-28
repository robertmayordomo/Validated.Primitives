using Xunit;
using Shouldly;
using Validated.Primitives.Validators;

namespace Validated.Primitives.Tests.Validators;

public class UrlValidatorsTests
{
    public static TheoryData<string> ValidUrls => new()
    {
        "http://example.com",
        "https://example.com/path",
        "https://www.example.com",
        "http://subdomain.example.com",
        "https://example.com/path?query=value",
        "https://example.com/path#anchor",
        "https://example.com:8080",
        "http://192.168.1.1",
        "https://example.com/path/to/resource",
        "https://example.com/path?a=1&b=2",
        "http://localhost:3000"
    };

    public static TheoryData<string> InvalidUrls => new()
    {
        "",
        "   ",
        "ftp://example.com",
        "not a url",
        "example.com",
        "www.example.com",
        "//example.com",
        "http://",
        "https://",
        "http:// example.com",
        "file:///path/to/file",
        "mailto:user@example.com",
        "javascript:alert('xss')",
        "data:text/html,<html>",
        "   http://example.com",
        "http://example.com   "
    };

    [Theory]
    [MemberData(nameof(ValidUrls))]
    public void WebUrl_Allows_Valid(string url)
    {
        var result = UrlValidators.WebUrl()(url);
        result.IsValid.ShouldBeTrue($"Expected valid URL: {url}");
    }

    [Theory]
    [MemberData(nameof(InvalidUrls))]
    public void WebUrl_Fails_Invalid(string url)
    {
        var result = UrlValidators.WebUrl()(url);
        result.IsValid.ShouldBeFalse($"Expected invalid URL: {url}");
    }

    [Fact]
    public void WebUrl_Returns_Correct_Error_Message()
    {
        var result = UrlValidators.WebUrl("WebsiteUrl")("invalid");
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Message.ShouldBe("Invalid web URL format.");
        result.Errors[0].MemberName.ShouldBe("WebsiteUrl");
        result.Errors[0].Code.ShouldBe("WebUrl");
    }

    [Fact]
    public void WebUrl_Uses_Default_FieldName_When_Not_Provided()
    {
        var result = UrlValidators.WebUrl()("invalid");
        result.Errors[0].MemberName.ShouldBe("Url");
    }

    [Theory]
    [InlineData("https://example.com", "Homepage")]
    [InlineData("http://api.example.com", "ApiEndpoint")]
    public void WebUrl_Validates_With_Custom_FieldName(string url, string fieldName)
    {
        var result = UrlValidators.WebUrl(fieldName)(url);
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("http")]
    [InlineData("https")]
    public void WebUrl_Only_Allows_Http_And_Https_Schemes(string scheme)
    {
        var url = $"{scheme}://example.com";
        var result = UrlValidators.WebUrl()(url);
        result.IsValid.ShouldBeTrue();
    }
}
