using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for EmailAddress value objects.
/// Converts between EmailAddress and string for JSON serialization.
/// </summary>
public class EmailAddressConverter : ValidatedPrimitiveConverter<EmailAddress, string>
{
    /// <summary>
    /// Initializes a new instance of the EmailAddressConverter class.
    /// </summary>
    public EmailAddressConverter()
        : base(EmailAddress.TryCreate, "Email")
    {
    }
}
