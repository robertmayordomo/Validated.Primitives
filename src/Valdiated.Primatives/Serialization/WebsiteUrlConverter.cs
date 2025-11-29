using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for WebsiteUrl value objects.
/// Converts between WebsiteUrl and string for JSON serialization.
/// </summary>
public class WebsiteUrlConverter : ValidatedValueObjectConverter<WebsiteUrl, string>
{
    /// <summary>
    /// Initializes a new instance of the WebsiteUrlConverter class.
    /// </summary>
    public WebsiteUrlConverter()
        : base(WebsiteUrl.TryCreate, "Url")
    {
    }
}
