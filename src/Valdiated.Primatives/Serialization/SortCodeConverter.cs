using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for SortCode value objects.
/// Serializes as an object containing both the sort code value and country code.
/// </summary>
public class SortCodeConverter : JsonConverter<SortCode>
{
    /// <summary>
    /// Reads and converts the JSON to a SortCode.
    /// </summary>
    public override SortCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            string? value = null;
            CountryCode countryCode = CountryCode.Unknown;

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
                            value = reader.GetString();
                            break;
                        case "CountryCode":
                            if (Enum.TryParse<CountryCode>(reader.GetString(), out var parsed))
                            {
                                countryCode = parsed;
                            }
                            break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var (result, sortCode) = SortCode.TryCreate(countryCode, value, "SortCode");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize SortCode: {errors}");
            }

            return sortCode;
        }

        throw new JsonException("Expected JSON object for SortCode");
    }

    /// <summary>
    /// Writes the SortCode as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, SortCode value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("Value", value.Value);
        writer.WriteString("CountryCode", value.CountryCode.ToString());
        writer.WriteEndObject();
    }
}
