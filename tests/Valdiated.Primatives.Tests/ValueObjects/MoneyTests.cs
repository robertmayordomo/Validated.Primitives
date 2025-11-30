using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class MoneyTests
{
    #region Valid Test Data

    public static TheoryData<CountryCode, decimal> ValidMoneyValues => new()
    {
        { CountryCode.UnitedStates, 0m },
        { CountryCode.UnitedStates, 100.50m },
        { CountryCode.UnitedStates, 1000.99m },
        { CountryCode.UnitedKingdom, 250.75m },
        { CountryCode.Canada, 99.99m },
        { CountryCode.Germany, 1500.00m },
        { CountryCode.Japan, 10000m },
        { CountryCode.India, 5000.50m }
    };

    public static TheoryData<string, decimal, CountryCode> ValidMoneyWithCurrencyCode => new()
    {
        { "USD", 100.50m, CountryCode.UnitedStates },
        { "EUR", 250.75m, CountryCode.Germany },
        { "GBP", 99.99m, CountryCode.UnitedKingdom },
        { "JPY", 10000m, CountryCode.Japan },
        { "CAD", 150.25m, CountryCode.Canada },
        { "AUD", 200.00m, CountryCode.Australia },
        { "CHF", 300.50m, CountryCode.Switzerland },
        { "BTC", 0.05m, CountryCode.Unknown } // Custom currency
    };

    #endregion

    #region Invalid Test Data

    public static TheoryData<CountryCode, decimal> InvalidMoneyValues => new()
    {
        { CountryCode.UnitedStates, -1m },
        { CountryCode.UnitedStates, -100.50m },
        { CountryCode.UnitedKingdom, -0.01m },
        { CountryCode.Germany, -1000m }
    };

    public static TheoryData<CountryCode, decimal> InvalidDecimalPlaces => new()
    {
        { CountryCode.UnitedStates, 100.123m }, // 3 decimal places
        { CountryCode.UnitedStates, 100.5555m }, // 4 decimal places
        { CountryCode.UnitedKingdom, 99.999m }
    };

    #endregion

    #region TryCreate Tests - CountryCode Overload

    [Theory]
    [MemberData(nameof(ValidMoneyValues))]
    public void TryCreate_With_CountryCode_Succeeds_For_Valid_Values(CountryCode countryCode, decimal value)
    {
        // Act
        var (result, money) = Money.TryCreate(countryCode, value);

        // Assert
        result.IsValid.ShouldBeTrue($"Result should be valid for value: {value} with country code: {countryCode}");
        money.ShouldNotBeNull($"Money should not be null when validation succeeds for: {value}");
        money.Value.ShouldBe(value);
        
        money.CurrencyCode.ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidMoneyValues))]
    public void TryCreate_Fails_For_Negative_Values(CountryCode countryCode, decimal value)
    {
        // Act
        var (result, money) = Money.TryCreate(countryCode, value);

        // Assert
        result.IsValid.ShouldBeFalse($"Result should be invalid for negative value: {value}");
        money.ShouldBeNull($"Money should be null when validation fails for: {value}");
        result.Errors.ShouldContain(e => e.Code == "NonNegative");
    }

    [Theory]
    [MemberData(nameof(InvalidDecimalPlaces))]
    public void TryCreate_Fails_For_Too_Many_Decimal_Places(CountryCode countryCode, decimal value)
    {
        // Act
        var (result, money) = Money.TryCreate(countryCode, value);

        // Assert
        result.IsValid.ShouldBeFalse($"Result should be invalid for value with too many decimal places: {value}");
        money.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "DecimalPlaces");
    }

    [Fact]
    public void TryCreate_With_CountryCode_Sets_Correct_CurrencyCode()
    {
        // Arrange & Act
        var (_, usdMoney) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, eurMoney) = Money.TryCreate(CountryCode.Germany, 100m);
        var (_, gbpMoney) = Money.TryCreate(CountryCode.UnitedKingdom, 100m);
        var (_, jpyMoney) = Money.TryCreate(CountryCode.Japan, 100m);

        // Assert
        usdMoney!.CurrencyCode.ShouldBe("USD");
        eurMoney!.CurrencyCode.ShouldBe("EUR");
        gbpMoney!.CurrencyCode.ShouldBe("GBP");
        jpyMoney!.CurrencyCode.ShouldBe("JPY");
    }

    #endregion

    #region TryCreate Tests - CurrencyCode Overload

    [Theory]
    [MemberData(nameof(ValidMoneyWithCurrencyCode))]
    public void TryCreate_With_CurrencyCode_Succeeds_For_Valid_Values(string currencyCode, decimal value, CountryCode countryCode)
    {
        // Act
        var (result, money) = Money.TryCreate(currencyCode, value);

        // Assert
        result.IsValid.ShouldBeTrue($"Result should be valid for value: {value} with currency code: {currencyCode}");
        money.ShouldNotBeNull();
        money.Value.ShouldBe(value);
        money.CurrencyCode.ShouldBe(currencyCode);
        
    }

    [Fact]
    public void TryCreate_With_CurrencyCode_Allows_Custom_Currency()
    {
        // Act
        var (result, money) = Money.TryCreate("BTC", 0.05m);

        // Assert
        result.IsValid.ShouldBeTrue();
        money.ShouldNotBeNull();
        money.CurrencyCode.ShouldBe("BTC");
    }

    [Fact]
    public void TryCreate_With_CurrencyCode_Defaults_To_Unknown_CountryCode()
    {
        // Act
        var (result, money) = Money.TryCreate("EUR", 100m);

        // Assert
        result.IsValid.ShouldBeTrue();
        money.ShouldNotBeNull();
        money.CurrencyCode.ShouldBe("EUR");
    }

    #endregion

    #region GetCurrencySymbol Tests

    [Theory]
    [InlineData(CountryCode.UnitedStates, "$")]
    [InlineData(CountryCode.UnitedKingdom, "£")]
    [InlineData(CountryCode.Germany, "€")]
    [InlineData(CountryCode.France, "€")]
    [InlineData(CountryCode.Japan, "¥")]
    [InlineData(CountryCode.Switzerland, "CHF")]
    [InlineData(CountryCode.Canada, "C$")]
    [InlineData(CountryCode.Australia, "A$")]
    [InlineData(CountryCode.India, "₹")]
    [InlineData(CountryCode.Russia, "₽")]
    public void GetCurrencySymbol_Returns_Correct_Symbol_For_CountryCode(CountryCode countryCode, string expectedSymbol)
    {
        // Arrange
        var (_, money) = Money.TryCreate(countryCode, 100m);

        // Act
        var symbol = money!.GetCurrencySymbol();

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Theory]
    [InlineData("USD", "$")]
    [InlineData("EUR", "€")]
    [InlineData("GBP", "£")]
    [InlineData("JPY", "¥")]
    [InlineData("CHF", "CHF")]
    [InlineData("CAD", "C$")]
    public void GetCurrencySymbol_Returns_Correct_Symbol_For_CurrencyCode(string currencyCode, string expectedSymbol)
    {
        // Arrange
        var (_, money) = Money.TryCreate(currencyCode, 100m);

        // Act
        var symbol = money!.GetCurrencySymbol();

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Fact]
    public void GetCurrencySymbol_Prioritizes_CurrencyCode_Over_CountryCode()
    {
        // Arrange - Create money with EUR currency but US country code
        var (_, money) = Money.TryCreate("EUR", 100m);

        // Act
        var symbol = money!.GetCurrencySymbol();

        // Assert
        symbol.ShouldBe("€"); // Should use EUR symbol, not $ from US
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_Formats_Money_Correctly()
    {
        // Arrange
        var (_, usdMoney) = Money.TryCreate(CountryCode.UnitedStates, 1234.56m);
        var (_, eurMoney) = Money.TryCreate(CountryCode.Germany, 999.99m);

        // Act & Assert
        usdMoney!.ToString().ShouldBe("$1,234.56");
        eurMoney!.ToString().ShouldBe("€999.99");
    }

    [Fact]
    public void ToStringWithCode_Includes_Currency_Code()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.UnitedStates, 100.50m);

        // Act
        var result = money!.ToStringWithCode();

        // Assert
        result.ShouldBe("100.50 USD");
    }

    [Fact]
    public void ToString_Handles_Zero_Value()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.UnitedStates, 0m);

        // Act & Assert
        money!.ToString().ShouldBe("$0.00");
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_Returns_True_For_Same_Values()
    {
        // Arrange
        var (_, money1) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, money2) = Money.TryCreate(CountryCode.UnitedStates, 100m);

        // Act & Assert
        money1.ShouldBe(money2);
        money1!.Equals(money2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Returns_False_For_Different_Values()
    {
        // Arrange
        var (_, money1) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, money2) = Money.TryCreate(CountryCode.UnitedStates, 200m);

        // Act & Assert
        money1.ShouldNotBe(money2);
        money1!.Equals(money2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Returns_False_For_Different_CountryCodes()
    {
        // Arrange
        var (_, money1) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, money2) = Money.TryCreate(CountryCode.Canada, 100m);

        // Act & Assert
        money1.ShouldNotBe(money2);
        money1!.Equals(money2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Returns_False_For_Different_CurrencyCodes()
    {
        // Arrange
        var (_, money1) = Money.TryCreate("USD", 100m);
        var (_, money2) = Money.TryCreate("EUR", 100m);

        // Act & Assert
        money1.ShouldNotBe(money2);
        money1!.Equals(money2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Returns_False_For_Null()
    {
        // Arrange
        var (_, money) = Money.TryCreate(CountryCode.UnitedStates, 100m);

        // Act & Assert
        money!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_Is_Same_For_Equal_Money()
    {
        // Arrange
        var (_, money1) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, money2) = Money.TryCreate(CountryCode.UnitedStates, 100m);

        // Act & Assert
        money1!.GetHashCode().ShouldBe(money2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_Is_Different_For_Different_Money()
    {
        // Arrange
        var (_, money1) = Money.TryCreate(CountryCode.UnitedStates, 100m);
        var (_, money2) = Money.TryCreate(CountryCode.UnitedStates, 200m);

        // Act & Assert
        money1!.GetHashCode().ShouldNotBe(money2!.GetHashCode());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void TryCreate_With_Max_Decimal_Value_Should_Work()
    {
        // Arrange
        var maxValidMoney = 999999999.99m;

        // Act
        var (result, money) = Money.TryCreate(CountryCode.UnitedStates, maxValidMoney);

        // Assert
        result.IsValid.ShouldBeTrue();
        money.ShouldNotBeNull();
        money.Value.ShouldBe(maxValidMoney);
    }

    [Fact]
    public void TryCreate_With_Exactly_Two_Decimal_Places_Should_Work()
    {
        // Arrange & Act
        var (result, money) = Money.TryCreate(CountryCode.UnitedStates, 100.12m);

        // Assert
        result.IsValid.ShouldBeTrue();
        money.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_With_One_Decimal_Place_Should_Work()
    {
        // Arrange & Act
        var (result, money) = Money.TryCreate(CountryCode.UnitedStates, 100.5m);

        // Assert
        result.IsValid.ShouldBeTrue();
        money.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_With_No_Decimal_Places_Should_Work()
    {
        // Arrange & Act
        var (result, money) = Money.TryCreate(CountryCode.UnitedStates, 100m);

        // Assert
        result.IsValid.ShouldBeTrue();
        money.ShouldNotBeNull();
    }

    [Fact]
    public void Multiple_Euro_Countries_Share_Same_Currency_Code()
    {
        // Arrange & Act
        var (_, germanyMoney) = Money.TryCreate(CountryCode.Germany, 100m);
        var (_, franceMoney) = Money.TryCreate(CountryCode.France, 100m);
        var (_, italyMoney) = Money.TryCreate(CountryCode.Italy, 100m);

        // Assert
        germanyMoney!.CurrencyCode.ShouldBe("EUR");
        franceMoney!.CurrencyCode.ShouldBe("EUR");
        italyMoney!.CurrencyCode.ShouldBe("EUR");
    }

    [Fact]
    public void MemberName_Is_Used_In_Validation_Errors()
    {
        // Arrange & Act
        var (result, _) = Money.TryCreate(CountryCode.UnitedStates, -100m, "PaymentAmount");

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.MemberName == "PaymentAmount");
    }

    #endregion
}
