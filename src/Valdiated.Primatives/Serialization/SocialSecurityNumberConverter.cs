using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for SocialSecurityNumber value object.
/// </summary>
public class SocialSecurityNumberConverter : JsonConverter<SocialSecurityNumber>
{
    public override SocialSecurityNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var (result, ssn) = SocialSecurityNumber.TryCreate(value);
        if (!result.IsValid)
        {
            throw new JsonException($"Invalid Social Security Number: {result.ToSingleMessage()}");
        }

        return ssn;
    }

    public override void Write(Utf8JsonWriter writer, SocialSecurityNumber value, JsonSerializerOptions options)
    {
        // Write in formatted XXX-XX-XXXX format
        writer.WriteStringValue(value.ToString());
    }
}
