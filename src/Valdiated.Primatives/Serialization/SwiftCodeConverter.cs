using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for SwiftCode value objects.
/// Serializes as a simple string value containing the SWIFT code.
/// </summary>
public class SwiftCodeConverter : JsonConverter<SwiftCode>
{
    /// <summary>
    /// Reads and converts the JSON to a SwiftCode.
    /// </summary>
    public override SwiftCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            var (result, swiftCode) = SwiftCode.TryCreate(value, allowTestCodes: true); // Allow test codes during deserialization

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize SwiftCode: {errors}");
            }

            return swiftCode;
        }

        throw new JsonException("Expected JSON string for SwiftCode");
    }

    /// <summary>
    /// Writes the SwiftCode as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, SwiftCode value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value);
    }
}
