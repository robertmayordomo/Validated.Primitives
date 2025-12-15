using System.Text.Json;
using System.Text.Json.Serialization;

namespace Validated.Primitives.Domain.Serialization;

/// <summary>
/// JSON converter for GeoDistance domain object.
/// Serializes as an object with From, To coordinates and distance in kilometers.
/// </summary>
public class GeoDistanceConverter : JsonConverter<Geospatial.GeoDistance>
{
    public override Geospatial.GeoDistance? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for GeoDistance");

        Geospatial.Coordinate? from = null;
        Geospatial.Coordinate? to = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "From":
                    from = JsonSerializer.Deserialize<Geospatial.Coordinate>(ref reader, options);
                    break;
                case "To":
                    to = JsonSerializer.Deserialize<Geospatial.Coordinate>(ref reader, options);
                    break;
                case "Kilometers":
                    // Skip - will be recalculated
                    reader.Skip();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (from == null || to == null)
            throw new JsonException("GeoDistance requires both From and To coordinates");

        var (result, geoDistance) = Geospatial.GeoDistance.TryCreate(from, to);

        if (!result.IsValid)
            throw new JsonException($"Failed to create GeoDistance: {result.ToBulletList()}");

        return geoDistance;
    }

    public override void Write(Utf8JsonWriter writer, Geospatial.GeoDistance value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("From");
        JsonSerializer.Serialize(writer, value.From, options);

        writer.WritePropertyName("To");
        JsonSerializer.Serialize(writer, value.To, options);

        writer.WriteNumber("Kilometers", value.Kilometers);
        writer.WriteNumber("Miles", value.Miles);
        writer.WriteNumber("Meters", value.Meters);
        writer.WriteNumber("NauticalMiles", value.NauticalMiles);

        writer.WriteEndObject();
    }
}
