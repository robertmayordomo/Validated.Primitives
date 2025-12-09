using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for IpAddress value objects.
/// Converts between IpAddress and string for JSON serialization.
/// </summary>
public class IpAddressConverter : ValidatedPrimitiveConverter<IpAddress, string>
{
    /// <summary>
    /// Initializes a new instance of the IpAddressConverter class.
    /// </summary>
    public IpAddressConverter()
        : base(IpAddress.TryCreate, "IpAddress")
    {
    }
}
