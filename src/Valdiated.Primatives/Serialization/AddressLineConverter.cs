using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for AddressLine value objects.
/// Converts between AddressLine and string for JSON serialization.
/// </summary>
public class AddressLineConverter : ValidatedPrimitiveConverter<AddressLine, string>
{
    /// <summary>
    /// Initializes a new instance of the AddressLineConverter class.
    /// </summary>
    public AddressLineConverter()
        : base(AddressLine.TryCreate, "AddressLine")
    {
    }
}
