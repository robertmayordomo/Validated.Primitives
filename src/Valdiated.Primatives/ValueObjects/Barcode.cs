using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated barcode supporting multiple formats.
/// Supports UPC-A (12 digits), EAN-13 (13 digits), EAN-8 (8 digits), 
/// Code39 (alphanumeric with * delimiters), and Code128 (alphanumeric).
/// </summary>
[JsonConverter(typeof(BarcodeConverter))]
public sealed record Barcode : ValidatedPrimitive<string>
{
    /// <summary>
    /// Gets the barcode format type.
    /// </summary>
    public BarcodeFormat Format { get; }

    private Barcode(string value, string propertyName = "Barcode") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(BarcodeValidators.Barcode(propertyName));
        
        // Detect format after validation
        Format = DetectFormat(value);
    }

    /// <summary>
    /// Attempts to create a Barcode instance with validation.
    /// </summary>
    /// <param name="value">The barcode string to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Barcode instance if valid.</returns>
    public static (ValidationResult Result, Barcode? Value) TryCreate(string value, string propertyName = "Barcode")
    {
        var barcode = new Barcode(value, propertyName);
        var validationResult = barcode.Validate();
        var result = validationResult.IsValid ? barcode : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets the normalized barcode value without separators.
    /// </summary>
    public string GetNormalized() => Value.Replace(" ", "").Replace("-", "");

    /// <summary>
    /// Detects the barcode format from the value.
    /// </summary>
    private static BarcodeFormat DetectFormat(string value)
    {
        var cleanValue = value.Replace(" ", "").Replace("-", "");

        if (cleanValue.Length == 12 && cleanValue.All(char.IsDigit))
            return BarcodeFormat.UpcA;

        if (cleanValue.Length == 13 && cleanValue.All(char.IsDigit))
            return BarcodeFormat.Ean13;

        if (cleanValue.Length == 8 && cleanValue.All(char.IsDigit))
            return BarcodeFormat.Ean8;

        if (cleanValue.StartsWith("*") && cleanValue.EndsWith("*"))
            return BarcodeFormat.Code39;

        return BarcodeFormat.Code128;
    }

    /// <summary>
    /// Returns the barcode value as a string.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// Represents the format type of a barcode.
/// </summary>
public enum BarcodeFormat
{
    /// <summary>
    /// Unknown barcode format.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// UPC-A format (12 digits).
    /// </summary>
    UpcA = 1,

    /// <summary>
    /// EAN-13 format (13 digits).
    /// </summary>
    Ean13 = 2,

    /// <summary>
    /// EAN-8 format (8 digits).
    /// </summary>
    Ean8 = 3,

    /// <summary>
    /// Code39 format (alphanumeric with * delimiters).
    /// </summary>
    Code39 = 4,

    /// <summary>
    /// Code128 format (alphanumeric).
    /// </summary>
    Code128 = 5
}
