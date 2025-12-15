using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for TrackingNumber value objects.
/// Converts between TrackingNumber and string for JSON serialization.
/// </summary>
public class TrackingNumberConverter : ValidatedPrimitiveConverter<TrackingNumber, string>
{
    /// <summary>
    /// Initializes a new instance of the TrackingNumberConverter class.
    /// </summary>
    public TrackingNumberConverter()
        : base(TrackingNumber.TryCreate, "TrackingNumber")
    {
    }
}
