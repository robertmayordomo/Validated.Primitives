using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for StateProvince value objects.
/// Converts between StateProvince and string for JSON serialization.
/// </summary>
public class StateProvinceConverter : ValidatedPrimitiveConverter<StateProvince, string>
{
    /// <summary>
    /// Initializes a new instance of the StateProvinceConverter class.
    /// </summary>
    public StateProvinceConverter()
        : base(StateProvince.TryCreate, "StateProvince")
    {
    }
}
