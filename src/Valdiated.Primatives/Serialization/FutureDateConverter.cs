using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for FutureDate value objects.
/// Converts between FutureDate and DateTime for JSON serialization.
/// </summary>
public class FutureDateConverter : ValidatedValueObjectConverter<FutureDate, DateTime>
{
    /// <summary>
    /// Initializes a new instance of the FutureDateConverter class.
    /// </summary>
    public FutureDateConverter()
        : base(FutureDate.TryCreate, "FutureDate")
    {
    }
}
