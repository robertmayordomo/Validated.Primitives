using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for City value objects.
/// Converts between City and string for JSON serialization.
/// </summary>
public class CityConverter : ValidatedPrimitiveConverter<City, string>
{
    /// <summary>
    /// Initializes a new instance of the CityConverter class.
    /// </summary>
    public CityConverter()
        : base(City.TryCreate, "City")
    {
    }
}
