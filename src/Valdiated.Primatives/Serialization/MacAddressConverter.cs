using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for MacAddress value objects.
/// Converts between MacAddress and string for JSON serialization.
/// </summary>
public class MacAddressConverter : ValidatedPrimitiveConverter<MacAddress, string>
{
    /// <summary>
    /// Initializes a new instance of the MacAddressConverter class.
    /// </summary>
    public MacAddressConverter()
        : base((value, propertyName) => MacAddress.TryCreate(value, propertyName: propertyName), "MacAddress")
    {
    }
}
