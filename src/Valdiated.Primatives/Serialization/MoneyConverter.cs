using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for Money value objects.
/// Serializes as an object containing the monetary value and currency code.
/// </summary>
public class MoneyConverter : JsonConverter<Money>
{
    /// <summary>
    /// Reads and converts the JSON to Money.
    /// </summary>
    public override Money? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            decimal value = 0;
            string? currencyCode = null;

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

                    // Handle both camelCase and PascalCase property names
                    if (string.Equals(propertyName, "value", StringComparison.OrdinalIgnoreCase))
                    {
                        value = reader.GetDecimal();
                    }
                    else if (string.Equals(propertyName, "currencyCode", StringComparison.OrdinalIgnoreCase))
                    {
                        currencyCode = reader.GetString();
                    }
                }
            }

            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new JsonException("CurrencyCode is required for Money deserialization.");
            }

            var (result, money) = Money.TryCreate(currencyCode, value, "Money");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize Money: {errors}");
            }

            return money;
        }

        throw new JsonException("Expected JSON object for Money");
    }

    /// <summary>
    /// Writes the Money as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        
        // Respect the naming policy from options
        var valuePropName = options.PropertyNamingPolicy?.ConvertName("Value") ?? "Value";
        var currencyCodePropName = options.PropertyNamingPolicy?.ConvertName("CurrencyCode") ?? "CurrencyCode";
        
        writer.WriteNumber(valuePropName, value.Value);
        writer.WriteString(currencyCodePropName, value.CurrencyCode);
        writer.WriteEndObject();
    }
}
