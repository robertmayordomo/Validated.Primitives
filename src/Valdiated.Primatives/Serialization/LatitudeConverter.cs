using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects.Geospatial;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for Latitude value objects.
/// Serializes as an object containing both the latitude value and decimal places configuration.
/// </summary>
public class LatitudeConverter : JsonConverter<Latitude>
{
    /// <summary>
    /// Reads and converts the JSON to Latitude.
    /// </summary>
    public override Latitude? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            decimal value = 0;
            int decimalPlaces = 6; // Default GPS precision

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

                    switch (propertyName?.ToLowerInvariant())
                    {
                        case "value":
                            value = reader.GetDecimal();
                            break;
                        case "decimalplaces":
                            decimalPlaces = reader.GetInt32();
                            break;
                    }
                }
            }

            var (result, latitude) = Latitude.TryCreate(value, decimalPlaces, "Latitude");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize Latitude: {errors}");
            }

            return latitude;
        }

        throw new JsonException("Expected JSON object for Latitude");
    }

    /// <summary>
    /// Writes the Latitude as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, Latitude value, JsonSerializerOptions options)
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
