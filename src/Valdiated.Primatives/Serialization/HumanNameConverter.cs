using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for HumanName value objects.
/// Converts between HumanName and string for JSON serialization.
/// </summary>
public class HumanNameConverter : ValidatedPrimitiveConverter<HumanName, string>
{
    /// <summary>
    /// Initializes a new instance of the HumanNameConverter class.
    /// </summary>
    public HumanNameConverter()
        : base(HumanName.TryCreate, "HumanName")
    {
    }
}
