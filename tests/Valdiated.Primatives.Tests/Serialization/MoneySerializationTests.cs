using System.Text.Json;
using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.Serialization;

public class MoneySerializationTests
{
    private readonly JsonSerializerOptions _options;

    public MoneySerializationTests()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    #region Basic Serialization Tests

    [Fact]
    public void Serialize_Money_Should_Include_Value_And_CurrencyCode()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.UnitedStates, 100.50m);

        // Act
        var json = JsonSerializer.Serialize(money, _options);

        // Assert
        json.ShouldContain("\"value\":");
        json.ShouldContain("100.5");
        json.ShouldContain("\"currencyCode\":\"USD\"");
    }

    [Fact]
    public void Deserialize_Money_Should_Restore_All_Properties()
    {
        // Arrange
        var json = "{\"value\":100.50,\"currencyCode\":\"USD\"}";

        // Act
        var money = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        money.ShouldNotBeNull();
        money.Value.ShouldBe(100.50m);
        money.CurrencyCode.ShouldBe("USD");
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_Should_Preserve_All_Values()
    {
        // Arrange
        var (_, original) = Money.TryCreate(CountryCode.UnitedKingdom, 250.75m);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original!.Value);
        deserialized.CurrencyCode.ShouldBe(original.CurrencyCode);
        deserialized.ShouldBe(original);
    }

    #endregion

    #region Currency Code Serialization Tests

    [Fact]
    public void Serialize_Money_With_Custom_CurrencyCode_Should_Preserve_It()
    {
        // Arrange
        var (_, money) = Money.TryCreate("BTC", 0.05m);

        // Act
        var json = JsonSerializer.Serialize(money, _options);

        // Assert
        json.ShouldContain("\"currencyCode\":\"BTC\"");
    }

    [Fact]
    public void Deserialize_Money_With_CurrencyCode_Should_Use_CurrencyCode_Overload()
    {
        // Arrange
        var json = "{\"value\":100,\"currencyCode\":\"EUR\"}";

        // Act
        var money = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        money.ShouldNotBeNull();
        money.CurrencyCode.ShouldBe("EUR");
    }

    #endregion

    #region Null Handling Tests

    [Fact]
    public void Serialize_Null_Money_Should_Write_Null()
    {
        // Arrange
        Money? money = null;

        // Act
        var json = JsonSerializer.Serialize(money, _options);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void Deserialize_Null_Should_Return_Null()
    {
        // Arrange
        var json = "null";

        // Act
        var money = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        money.ShouldBeNull();
    }

    #endregion

    #region Invalid Data Tests

    [Fact]
    public void Deserialize_Invalid_Money_Should_Throw_JsonException()
    {
        // Arrange - Use Pascal case property names as expected by MoneyConverter
        var json = "{\"Value\":-100,\"CurrencyCode\":\"USD\"}";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Money>(json, _options))
            .Message.ShouldContain("Failed to deserialize Money");
    }

    [Fact]
    public void Deserialize_Money_With_Too_Many_Decimal_Places_Should_Throw_JsonException()
    {
        // Arrange
        var json = "{\"value\":100.123,\"currencyCode\":\"USD\"}";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Money>(json, _options));
    }

    [Fact]
    public void Deserialize_Money_Requires_CurrencyCode()
    {
        // Arrange - JSON without currencyCode should fail
        var json = "{\"value\":100}";

        // Act & Assert
        Should.Throw<JsonException>(() => 
            JsonSerializer.Deserialize<Money>(json, _options))
            .Message.ShouldContain("CurrencyCode is required");
    }

    #endregion

    #region Complex Object Tests

    [Fact]
    public void Serialize_Money_In_Object_Should_Work()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.Canada, 150.25m);
        var obj = new { Price = money, ProductName = "Widget" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);

        // Assert
        json.ShouldContain("\"price\":{");
        json.ShouldContain("\"value\":");
        json.ShouldContain("150.25");
        json.ShouldContain("\"currencyCode\":\"CAD\"");
        json.ShouldContain("\"productName\":\"Widget\"");
    }

    [Fact]
    public void Deserialize_Money_In_Object_Should_Work()
    {
        // Arrange
        var json = "{\"price\":{\"value\":150.25,\"currencyCode\":\"CAD\"},\"productName\":\"Widget\"}";

        // Act
        var obj = JsonSerializer.Deserialize<ProductObject>(json, _options);

        // Assert
        obj.ShouldNotBeNull();
        obj.Price.ShouldNotBeNull();
        obj.Price.Value.ShouldBe(150.25m);
        obj.Price.CurrencyCode.ShouldBe("CAD");
        obj.ProductName.ShouldBe("Widget");
    }

    [Fact]
    public void Serialize_Money_In_Collection_Should_Work()
    {
        // Arrange
        var (_, money1) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, money2) = Money.TryCreate(CountryCode.UnitedKingdom, 200m);
        var (_, money3) = Money.TryCreate("EUR", 300m);
        var list = new List<Money> { money1!, money2!, money3! };

        // Act
        var json = JsonSerializer.Serialize(list, _options);

        // Assert
        json.ShouldContain("USD");
        json.ShouldContain("GBP");
        json.ShouldContain("EUR");
        json.ShouldContain("100");
        json.ShouldContain("200");
        json.ShouldContain("300");
    }

    [Fact]
    public void Deserialize_Money_List_Should_Work()
    {
        // Arrange
        var json = "[" +
                   "{\"value\":100,\"currencyCode\":\"USD\"}," +
                   "{\"value\":200,\"currencyCode\":\"GBP\"}" +
                   "]";

        // Act
        var list = JsonSerializer.Deserialize<List<Money>>(json, _options);

        // Assert
        list.ShouldNotBeNull();
        list.Count.ShouldBe(2);
        list[0].CurrencyCode.ShouldBe("USD");
        list[1].CurrencyCode.ShouldBe("GBP");
    }

    #endregion

    #region Multiple Currency Tests

    [Theory]
    [InlineData(CountryCode.UnitedStates, "USD", 100.50)]
    [InlineData(CountryCode.UnitedKingdom, "GBP", 250.75)]
    [InlineData(CountryCode.Germany, "EUR", 500.99)]
    [InlineData(CountryCode.Japan, "JPY", 10000)]
    [InlineData(CountryCode.Canada, "CAD", 150.25)]
    [InlineData(CountryCode.Australia, "AUD", 200.00)]
    public void RoundTrip_Different_Currencies_Should_Preserve_Values(CountryCode countryCode, string expectedCurrency, decimal value)
    {
        // Arrange
        var (result, original) = Money.TryCreate(countryCode, value);
        result.IsValid.ShouldBeTrue($"Money should be valid for {countryCode}: {value}. Errors: {string.Join(", ", result.Errors.Select(e => e.Message))}");

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(value);
        deserialized.CurrencyCode.ShouldBe(expectedCurrency);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Serialize_Zero_Money_Should_Work()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.UnitedStates, 0m);

        // Act
        var json = JsonSerializer.Serialize(money, _options);
        var deserialized = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(0m);
    }

    [Fact]
    public void Serialize_Large_Money_Value_Should_Work()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.UnitedStates, 999999999.99m);

        // Act
        var json = JsonSerializer.Serialize(money, _options);
        var deserialized = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(999999999.99m);
    }

    [Fact]
    public void Deserialize_Money_With_Unknown_CountryCode_Should_Work()
    {
        // Arrange
        var json = "{\"value\":100,\"currencyCode\":\"XXX\"}";

        // Act
        var money = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        money.ShouldNotBeNull();
        money.CurrencyCode.ShouldBe("XXX");
    }

    [Fact]
    public void RoundTrip_Custom_CurrencyCode_Should_Preserve_Values()
    {
        // Arrange
        var (_, original) = Money.TryCreate("BTC", 0.05m);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Money>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.CurrencyCode.ShouldBe("BTC");
        deserialized.Value.ShouldBe(0.05m);
    }

    #endregion

    #region Case Sensitivity Tests

    [Fact]
    public void Deserialize_With_Different_Casing_Should_Work()
    {
        // Arrange - Mixed case property names
        var json = "{\"Value\":100,\"CurrencyCode\":\"USD\"}";

        // Act
        var money = JsonSerializer.Deserialize<Money>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert
        money.ShouldNotBeNull();
        money.Value.ShouldBe(100m);
        money.CurrencyCode.ShouldBe("USD");
    }

    #endregion

    private class ProductObject
    {
        public Money Price { get; set; } = null!;
        public string ProductName { get; set; } = null!;
    }
}
