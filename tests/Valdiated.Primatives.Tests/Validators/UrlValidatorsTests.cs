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
    public void GivenWebUrl_WhenValid_ThenShouldSucceed(string url)
    {
        var result = UrlValidators.WebUrl()(url);
        result.IsValid.ShouldBeTrue($"Expected valid URL: {url}");
    }

    [Theory]
    [MemberData(nameof(InvalidUrls))]
    public void GivenWebUrl_WhenInvalid_ThenShouldFail(string url)
    {
        var result = UrlValidators.WebUrl()(url);
        result.IsValid.ShouldBeFalse($"Expected invalid URL: {url}");
    }

    [Fact]
    public void GivenWebUrl_WhenInvalid_ThenShouldReturnCorrectErrorMessage()
    {
        var result = UrlValidators.WebUrl("WebsiteUrl")("invalid");
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Message.ShouldBe("Invalid web URL format.");
        result.Errors[0].MemberName.ShouldBe("WebsiteUrl");
        result.Errors[0].Code.ShouldBe("WebUrl");
    }

    [Fact]
    public void GivenWebUrl_WhenNoFieldNameProvided_ThenShouldUseDefault()
    {
        var result = UrlValidators.WebUrl()("invalid");
        result.Errors[0].MemberName.ShouldBe("Url");
    }

    [Theory]
    [InlineData("https://example.com", "Homepage")]
    [InlineData("http://api.example.com", "ApiEndpoint")]
    public void GivenWebUrlWithCustomFieldName_WhenValid_ThenShouldSucceed(string url, string fieldName)
    {
        var result = UrlValidators.WebUrl(fieldName)(url);
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("http")]
    [InlineData("https")]
    public void GivenWebUrl_WhenHttpOrHttpsScheme_ThenShouldSucceed(string scheme)
    {
        var url = $"{scheme}://example.com";
        var result = UrlValidators.WebUrl()(url);
        result.IsValid.ShouldBeTrue();
    }
}
