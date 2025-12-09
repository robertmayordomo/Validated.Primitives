using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for DrivingLicenseNumber value objects.
/// Serializes as an object with licenseNumber and issuingCountry properties.
/// </summary>
public class DrivingLicenseNumberConverter : JsonConverter<DrivingLicenseNumber>
{
    /// <summary>
    /// Reads and converts the JSON to a DrivingLicenseNumber.
    /// Expects format: { "licenseNumber": "value", "issuingCountry": "CountryName" }
    /// </summary>
    public override DrivingLicenseNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            string? licenseNumber = null;
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

                    if (propertyName?.Equals("licenseNumber", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        licenseNumber = reader.GetString();
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

            if (string.IsNullOrWhiteSpace(licenseNumber) || !issuingCountry.HasValue)
            {
                throw new JsonException("DrivingLicenseNumber requires both 'licenseNumber' and 'issuingCountry' properties");
            }

            var (result, license) = DrivingLicenseNumber.TryCreate(licenseNumber, issuingCountry.Value);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize DrivingLicenseNumber: {errors}");
            }

            return license;
        }

        throw new JsonException("Expected JSON object for DrivingLicenseNumber");
    }

    /// <summary>
    /// Writes the DrivingLicenseNumber as JSON.
    /// Format: { "licenseNumber": "value", "issuingCountry": "CountryName" }
    /// </summary>
    public override void Write(Utf8JsonWriter writer, DrivingLicenseNumber value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("licenseNumber", value.Value);
        writer.WriteString("issuingCountry", value.IssuingCountry.ToString());
        writer.WriteEndObject();
    }
}
