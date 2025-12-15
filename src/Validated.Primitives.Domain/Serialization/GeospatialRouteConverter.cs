using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Validated.Primitives.Domain.Serialization;

/// <summary>
/// JSON converter for GeospatialRoute domain object.
/// Serializes as an object with Segments array and total distance.
/// </summary>
public class GeospatialRouteConverter : JsonConverter<Geospatial.GeospatialRoute>
{
    public override Geospatial.GeospatialRoute? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for GeospatialRoute");

        List<Geospatial.RouteSegment>? segments = null;
        string? name = null;

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
                case "Segments":
                    segments = JsonSerializer.Deserialize<List<Geospatial.RouteSegment>>(ref reader, options);
                    break;
                case "Name":
                    name = reader.GetString();
                    break;
                case "TotalDistanceKilometers":
                case "TotalDistanceMiles":
                case "TotalDistanceMeters":
                case "StartingPoint":
                case "EndingPoint":
                    // Skip - will be recalculated
                    reader.Skip();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (segments == null || segments.Count == 0)
            throw new JsonException("GeospatialRoute requires at least one segment");

        var (result, route) = Geospatial.GeospatialRoute.TryCreate(segments, name);

        if (!result.IsValid)
            throw new JsonException($"Failed to create GeospatialRoute: {result.ToBulletList()}");

        return route;
    }

    public override void Write(Utf8JsonWriter writer, Geospatial.GeospatialRoute value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (!string.IsNullOrEmpty(value.Name))
        {
            writer.WriteString("Name", value.Name);
        }

        writer.WritePropertyName("Segments");
        JsonSerializer.Serialize(writer, value.Segments, options);

        writer.WriteNumber("TotalDistanceKilometers", value.TotalDistanceKilometers);
        writer.WriteNumber("TotalDistanceMiles", value.TotalDistanceMiles);
        writer.WriteNumber("TotalDistanceMeters", value.TotalDistanceMeters);

        if (value.StartingPoint != null)
        {
            writer.WritePropertyName("StartingPoint");
            JsonSerializer.Serialize(writer, value.StartingPoint, options);
        }

        if (value.EndingPoint != null)
        {
            writer.WritePropertyName("EndingPoint");
            JsonSerializer.Serialize(writer, value.EndingPoint, options);
        }

        writer.WriteEndObject();
    }
}
