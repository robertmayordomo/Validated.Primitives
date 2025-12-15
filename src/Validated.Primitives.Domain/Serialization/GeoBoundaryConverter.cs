using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Validated.Primitives.Domain.Serialization;

/// <summary>
/// JSON converter for GeoBoundary domain object.
/// Serializes as an object with Vertices array and calculated properties.
/// </summary>
public class GeoBoundaryConverter : JsonConverter<Geospatial.GeoBoundary>
{
    public override Geospatial.GeoBoundary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for GeoBoundary");

        List<Geospatial.Coordinate>? vertices = null;

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
                case "Vertices":
                    vertices = JsonSerializer.Deserialize<List<Geospatial.Coordinate>>(ref reader, options);
                    break;
                case "AreaSquareKilometers":
                case "PerimeterKilometers":
                case "Center":
                case "AreaSquareMiles":
                case "AreaSquareMeters":
                case "PerimeterMiles":
                    // Skip - will be recalculated
                    reader.Skip();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (vertices == null || vertices.Count == 0)
            throw new JsonException("GeoBoundary requires at least 3 vertices");

        var (result, geoBoundary) = Geospatial.GeoBoundary.TryCreate(vertices);

        if (!result.IsValid)
            throw new JsonException($"Failed to create GeoBoundary: {result.ToBulletList()}");

        return geoBoundary;
    }

    public override void Write(Utf8JsonWriter writer, Geospatial.GeoBoundary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Vertices");
        JsonSerializer.Serialize(writer, value.Vertices, options);

        writer.WriteNumber("AreaSquareKilometers", value.AreaSquareKilometers);
        writer.WriteNumber("AreaSquareMiles", value.AreaSquareMiles);
        writer.WriteNumber("AreaSquareMeters", value.AreaSquareMeters);
        writer.WriteNumber("PerimeterKilometers", value.PerimeterKilometers);
        writer.WriteNumber("PerimeterMiles", value.PerimeterMiles);

        if (value.Center != null)
        {
            writer.WritePropertyName("Center");
            JsonSerializer.Serialize(writer, value.Center, options);
        }

        writer.WriteEndObject();
    }
}
