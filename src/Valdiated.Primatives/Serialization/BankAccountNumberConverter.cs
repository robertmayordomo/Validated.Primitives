using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for BankAccountNumber value objects.
/// Serializes as an object containing both the account number value and country code.
/// </summary>
public class BankAccountNumberConverter : JsonConverter<BankAccountNumber>
{
    /// <summary>
    /// Reads and converts the JSON to a BankAccountNumber.
    /// </summary>
    public override BankAccountNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            var (result, accountNumber) = BankAccountNumber.TryCreate(countryCode, value, "BankAccountNumber");

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                throw new JsonException($"Failed to deserialize BankAccountNumber: {errors}");
            }

            return accountNumber;
        }

        throw new JsonException("Expected JSON object for BankAccountNumber");
    }

    /// <summary>
    /// Writes the BankAccountNumber as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, BankAccountNumber value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("Value", value.Value);
        writer.WriteString("CountryCode", value.CountryCode.ToString());
        writer.WriteString("IsIban", value.IsIban.ToString());
        writer.WriteEndObject();
    }
}
