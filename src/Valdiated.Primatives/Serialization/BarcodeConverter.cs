using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for Barcode value objects.
/// Converts between Barcode and string for JSON serialization.
/// </summary>
public class BarcodeConverter : ValidatedPrimitiveConverter<Barcode, string>
{
    /// <summary>
    /// Initializes a new instance of the BarcodeConverter class.
    /// </summary>
    public BarcodeConverter()
        : base(Barcode.TryCreate, "Barcode")
    {
    }
}
