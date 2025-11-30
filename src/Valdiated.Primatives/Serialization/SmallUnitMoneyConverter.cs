using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for SmallUnitMoney value objects.
/// Serializes as an object containing both the smallest unit value and country code.
/// </summary>
public class SmallUnitMoneyConverter : JsonConverter<SmallUnitMoney>
{
    /// <summary>
    /// Reads and converts the JSON to SmallUnitMoney.
    /// </summary>
    public override SmallUnitMoney? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            uint value = 0;
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
                            value = reader.GetUInt32();
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

            var (result, money) = SmallUnitMoney.TryCreate(countryCode, value, "SmallUnitMoney");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize SmallUnitMoney: {errors}");
            }

            return money;
        }

        throw new JsonException("Expected JSON object for SmallUnitMoney");
    }

    /// <summary>
    /// Writes the SmallUnitMoney as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, SmallUnitMoney value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("Value", value.Value);
        writer.WriteString("CountryCode", value.CountryCode.ToString());
        writer.WriteEndObject();
    }
}
