using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for Percentage value objects.
/// Serializes as an object containing both the percentage value and decimal places configuration.
/// </summary>
public class PercentageConverter : JsonConverter<Percentage>
{
    /// <summary>
    /// Reads and converts the JSON to Percentage.
    /// </summary>
    public override Percentage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            decimal value = 0;
            int decimalPlaces = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "Value":
                            value = reader.GetDecimal();
                            break;
                        case "DecimalPlaces":
                            decimalPlaces = reader.GetInt32();
                            break;
                    }
                }
            }

            var (result, percentage) = Percentage.TryCreate(value, decimalPlaces, "Percentage");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize Percentage: {errors}");
            }

            return percentage;
        }

        throw new JsonException("Expected JSON object for Percentage");
    }

    /// <summary>
    /// Writes the Percentage as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, Percentage value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("Value", value.Value);
        writer.WriteNumber("DecimalPlaces", value.DecimalPlaces);
        writer.WriteEndObject();
    }
}
