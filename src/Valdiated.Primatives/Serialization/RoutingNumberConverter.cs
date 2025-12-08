using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for RoutingNumber value objects.
/// Serializes as a simple string value containing the routing number.
/// </summary>
public class RoutingNumberConverter : JsonConverter<RoutingNumber>
{
    /// <summary>
    /// Reads and converts the JSON to a RoutingNumber.
    /// </summary>
    public override RoutingNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            var (result, routingNumber) = RoutingNumber.TryCreate(value, "RoutingNumber");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize RoutingNumber: {errors}");
            }

            return routingNumber;
        }

        throw new JsonException("Expected JSON string for RoutingNumber");
    }

    /// <summary>
    /// Writes the RoutingNumber as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, RoutingNumber value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value);
    }
}
