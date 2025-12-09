using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for Passport value objects.
/// Serializes as an object with passportNumber and issuingCountry properties.
/// </summary>
public class PassportConverter : JsonConverter<Passport>
{
    /// <summary>
    /// Reads and converts the JSON to a Passport.
    /// Expects format: { "passportNumber": "value", "issuingCountry": "CountryName" }
    /// </summary>
    public override Passport? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            string? passportNumber = null;
            CountryCode? issuingCountry = null;

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

                    if (propertyName?.Equals("passportNumber", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        passportNumber = reader.GetString();
                    }
                    else if (propertyName?.Equals("issuingCountry", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var countryString = reader.GetString();
                        if (Enum.TryParse<CountryCode>(countryString, true, out var country))
                        {
                            issuingCountry = country;
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(passportNumber) || !issuingCountry.HasValue)
            {
                throw new JsonException("Passport requires both 'passportNumber' and 'issuingCountry' properties");
            }

            var (result, passport) = Passport.TryCreate(passportNumber, issuingCountry.Value);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize Passport: {errors}");
            }

            return passport;
        }

        throw new JsonException("Expected JSON object for Passport");
    }

    /// <summary>
    /// Writes the Passport as JSON.
    /// Format: { "passportNumber": "value", "issuingCountry": "CountryName" }
    /// </summary>
    public override void Write(Utf8JsonWriter writer, Passport value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("passportNumber", value.Value);
        writer.WriteString("issuingCountry", value.IssuingCountry.ToString());
        writer.WriteEndObject();
    }
}
