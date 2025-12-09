using System.Text.Json;
using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Serialization;

/// <summary>
/// Abstract base class for JSON converters for ValidatedPrimitive types.
/// Provides conversion between value objects and their underlying primitive types for JSON serialization.
/// </summary>
/// <typeparam name="TValueObject">The value object type derived from ValidatedPrimitive.</typeparam>
/// <typeparam name="TValue">The underlying primitive type.</typeparam>
public abstract class ValidatedPrimitiveConverter<TValueObject, TValue> : JsonConverter<TValueObject>
    where TValueObject : ValidatedPrimitive<TValue>
    where TValue : notnull
{
    private readonly Func<TValue, string, (ValidationResult Result, TValueObject? Value)> _factory;
    private readonly string _propertyName;

    /// <summary>
    /// Initializes a new instance of the ValidatedPrimitiveConverter class.
    /// </summary>
    /// <param name="factory">Factory method to create the value object from the primitive value.</param>
    /// <param name="propertyName">Optional property name for validation error messages.</param>
    protected ValidatedPrimitiveConverter(
        Func<TValue, string, (ValidationResult Result, TValueObject? Value)> factory,
        string propertyName = "Value")
    {
        _factory = factory;
        _propertyName = propertyName;
    }

    /// <summary>
    /// Reads and converts the JSON to a ValidatedPrimitive.
    /// </summary>
    public override TValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        if (value == null)
        {
            return null;
        }

        var (result, valueObject) = _factory(value, _propertyName);

        if (!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Message));
            throw new JsonException(
                $"Failed to deserialize {typeof(TValueObject).Name}: {errors}");
        }

        return valueObject;
    }

    /// <summary>
    /// Writes the ValidatedPrimitive as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
