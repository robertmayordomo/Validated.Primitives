using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for IbanNumber value objects.
/// Serializes as a simple string value containing the account number.
/// Automatically detects IBAN vs BBAN format during deserialization.
/// </summary>
public class IbanNumberConverter : JsonConverter<IbanNumber>
{
    /// <summary>
    /// Reads and converts the JSON to an IbanNumber.
    /// </summary>
    public override IbanNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // Auto-detect will determine if it's IBAN or BBAN
            var (result, ibanNumber) = IbanNumber.TryCreate(value);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize IbanNumber: {errors}");
            }

            return ibanNumber;
        }

        throw new JsonException("Expected JSON string for IbanNumber");
    }

    /// <summary>
    /// Writes the IbanNumber as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, IbanNumber value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value);
    }
}
