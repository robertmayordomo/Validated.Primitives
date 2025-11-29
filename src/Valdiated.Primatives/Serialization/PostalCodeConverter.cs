using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for PostalCode value objects.
/// Serializes as an object containing both the postal code value and country code.
/// </summary>
public class PostalCodeConverter : JsonConverter<PostalCode>
{
    /// <summary>
    /// Reads and converts the JSON to a PostalCode.
    /// </summary>
    public override PostalCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            var (result, postalCode) = PostalCode.TryCreate(countryCode, value, "PostalCode");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize PostalCode: {errors}");
            }

            return postalCode;
        }

        throw new JsonException("Expected JSON object for PostalCode");
    }

    /// <summary>
    /// Writes the PostalCode as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, PostalCode value, JsonSerializerOptions options)
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
