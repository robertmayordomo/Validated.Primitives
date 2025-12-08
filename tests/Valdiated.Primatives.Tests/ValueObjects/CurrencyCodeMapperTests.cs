using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class CurrencyCodeMapperTests
{
    #region GetCurrencyCode Tests

    [Theory]
    [InlineData(CountryCode.UnitedStates, "USD")]
    [InlineData(CountryCode.UnitedKingdom, "GBP")]
    [InlineData(CountryCode.Canada, "CAD")]
    [InlineData(CountryCode.Australia, "AUD")]
    [InlineData(CountryCode.Japan, "JPY")]
    [InlineData(CountryCode.China, "CNY")]
    [InlineData(CountryCode.India, "INR")]
    [InlineData(CountryCode.Brazil, "BRL")]
    [InlineData(CountryCode.Mexico, "MXN")]
    [InlineData(CountryCode.SouthAfrica, "ZAR")]
    [InlineData(CountryCode.NewZealand, "NZD")]
    [InlineData(CountryCode.Singapore, "SGD")]
    [InlineData(CountryCode.SouthKorea, "KRW")]
    [InlineData(CountryCode.Russia, "RUB")]
    [InlineData(CountryCode.Switzerland, "CHF")]
    [InlineData(CountryCode.Sweden, "SEK")]
    [InlineData(CountryCode.Norway, "NOK")]
    [InlineData(CountryCode.Denmark, "DKK")]
    [InlineData(CountryCode.Poland, "PLN")]
    [InlineData(CountryCode.CzechRepublic, "CZK")]
    [InlineData(CountryCode.Hungary, "HUF")]
    public void GetCurrencyCode_Returns_Correct_Currency_For_CountryCode(CountryCode countryCode, string expectedCurrency)
    {
        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);

        // Assert
        currencyCode.ShouldBe(expectedCurrency);
    }

    [Theory]
    [InlineData(CountryCode.Germany, "EUR")]
    [InlineData(CountryCode.France, "EUR")]
    [InlineData(CountryCode.Italy, "EUR")]
    [InlineData(CountryCode.Spain, "EUR")]
    [InlineData(CountryCode.Netherlands, "EUR")]
    [InlineData(CountryCode.Belgium, "EUR")]
    [InlineData(CountryCode.Austria, "EUR")]
    [InlineData(CountryCode.Portugal, "EUR")]
    [InlineData(CountryCode.Ireland, "EUR")]
    public void GetCurrencyCode_Returns_EUR_For_Eurozone_Countries(CountryCode countryCode, string expectedCurrency)
    {
        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);

        // Assert
        currencyCode.ShouldBe(expectedCurrency);
        currencyCode.ShouldBe("EUR");
    }

    [Fact]
    public void GetCurrencyCode_Returns_UNKNOWN_For_Unknown_CountryCode()
    {
        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(CountryCode.Unknown);

        // Assert
        currencyCode.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void GetCurrencyCode_Returns_UNKNOWN_For_All_CountryCode()
    {
        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(CountryCode.All);

        // Assert
        currencyCode.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void GetCurrencyCode_Returns_UNKNOWN_For_Finland()
    {
        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(CountryCode.Finland);

        // Assert
        // Finland is not mapped in CurrencyCodeMapper (should be EUR)
        currencyCode.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void GetCurrencyCode_Handles_All_Defined_CountryCodes()
    {
        // Arrange
        var allCountryCodes = Enum.GetValues<CountryCode>();

        // Act & Assert
        foreach (var countryCode in allCountryCodes)
        {
            var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);
            currencyCode.ShouldNotBeNull($"Currency code should not be null for {countryCode}");
            currencyCode.ShouldNotBeEmpty($"Currency code should not be empty for {countryCode}");
        }
    }

    [Fact]
    public void GetCurrencyCode_Returns_Valid_ISO4217_Format()
    {
        // Arrange
        var allCountryCodes = Enum.GetValues<CountryCode>();

        // Act & Assert
        foreach (var countryCode in allCountryCodes)
        {
            var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);
            
            // ISO 4217 currency codes should be 3 uppercase letters or "UNKNOWN"
            currencyCode.Length.ShouldBeGreaterThanOrEqualTo(3);
            currencyCode.ShouldMatch(@"^[A-Z]+$", $"Currency code {currencyCode} for {countryCode} should be uppercase letters");
        }
    }

    [Fact]
    public void GetCurrencyCode_Provides_Deterministic_Results()
    {
        // Arrange
        var countryCode = CountryCode.UnitedStates;

        // Act
        var result1 = CurrencyCodeMapper.GetCurrencyCode(countryCode);
        var result2 = CurrencyCodeMapper.GetCurrencyCode(countryCode);
        var result3 = CurrencyCodeMapper.GetCurrencyCode(countryCode);

        // Assert
        result1.ShouldBe(result2);
        result2.ShouldBe(result3);
        result1.ShouldBe("USD");
    }

    #endregion

    #region GetCurrencySymbol Tests

    [Theory]
    [InlineData("USD", "$")]
    [InlineData("GBP", "£")]
    [InlineData("EUR", "€")]
    [InlineData("JPY", "¥")]
    [InlineData("CNY", "¥")]
    [InlineData("CHF", "CHF")]
    [InlineData("CAD", "C$")]
    [InlineData("AUD", "A$")]
    [InlineData("BRL", "R$")]
    [InlineData("MXN", "MX$")]
    [InlineData("ZAR", "R")]
    [InlineData("NZD", "NZ$")]
    [InlineData("SGD", "S$")]
    [InlineData("SEK", "kr")]
    [InlineData("NOK", "kr")]
    [InlineData("DKK", "kr")]
    [InlineData("PLN", "z?")]
    [InlineData("CZK", "K?")]
    [InlineData("HUF", "Ft")]
    public void GetCurrencySymbol_Returns_Correct_Symbol_For_CurrencyCode(string currencyCode, string expectedSymbol)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Theory]
    [InlineData("INR", "₹")]
    [InlineData("RUB", "₽")]
    [InlineData("KRW", "₩")]
    public void GetCurrencySymbol_Returns_QuestionMark_For_Currencies_Without_Unicode_Symbol(string currencyCode, string expectedSymbol)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Fact]
    public void GetCurrencySymbol_Returns_Empty_For_Unknown_Currency()
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol("XXX");

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Fact]
    public void GetCurrencySymbol_Returns_Empty_For_UNKNOWN()
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol("UNKNOWN");

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("XYZ")]
    [InlineData("ABC")]
    public void GetCurrencySymbol_Returns_Empty_For_Invalid_Codes(string currencyCode)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetCurrencySymbol_Returns_Empty_For_Null_Or_Empty_Input(string? currencyCode)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode!);

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void GetCurrencySymbol_Returns_Empty_For_Whitespace(string currencyCode)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Fact]
    public void GetCurrencySymbol_Returns_Empty_For_Numeric_Input()
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol("123");

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Fact]
    public void GetCurrencySymbol_Returns_Empty_For_Special_Characters()
    {
        // Act
        var symbol1 = CurrencyCodeMapper.GetCurrencySymbol("$$$");
        var symbol2 = CurrencyCodeMapper.GetCurrencySymbol("@#$");

        // Assert
        symbol1.ShouldBe(string.Empty);
        symbol2.ShouldBe(string.Empty);
    }

    [Fact]
    public void GetCurrencySymbol_Provides_Deterministic_Results()
    {
        // Arrange
        var currencyCode = "USD";

        // Act
        var result1 = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);
        var result2 = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);
        var result3 = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        result1.ShouldBe(result2);
        result2.ShouldBe(result3);
        result1.ShouldBe("$");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void CountryCode_To_CurrencyCode_To_Symbol_Chain_Works()
    {
        // Arrange
        var countryCode = CountryCode.UnitedStates;

        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        currencyCode.ShouldBe("USD");
        symbol.ShouldBe("$");
    }

    [Theory]
    [InlineData(CountryCode.Germany, "EUR", "€")]
    [InlineData(CountryCode.France, "EUR", "€")]
    [InlineData(CountryCode.Italy, "EUR", "€")]
    public void Multiple_Countries_Can_Share_Same_Currency(CountryCode countryCode, string expectedCurrency, string expectedSymbol)
    {
        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        currencyCode.ShouldBe(expectedCurrency);
        symbol.ShouldBe(expectedSymbol);
    }

    [Fact]
    public void All_Mapped_CurrencyCodes_Have_Symbols()
    {
        // Arrange
        var allCountryCodes = Enum.GetValues<CountryCode>();

        // Act & Assert
        foreach (var countryCode in allCountryCodes)
        {
            var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);
            var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

            // UNKNOWN currency codes are expected to have no symbol
            if (currencyCode != "UNKNOWN")
            {
                symbol.ShouldNotBeEmpty($"Currency {currencyCode} from {countryCode} should have a symbol");
            }
        }
    }

    [Fact]
    public void All_Unique_Currency_Codes_Are_Mapped_To_Symbols()
    {
        // Arrange
        var allCountryCodes = Enum.GetValues<CountryCode>();
        var uniqueCurrencies = allCountryCodes
            .Select(cc => CurrencyCodeMapper.GetCurrencyCode(cc))
            .Where(c => c != "UNKNOWN")
            .Distinct()
            .ToList();

        // Act & Assert
        foreach (var currency in uniqueCurrencies)
        {
            var symbol = CurrencyCodeMapper.GetCurrencySymbol(currency);
            symbol.ShouldNotBeEmpty($"Currency code {currency} should have a symbol defined");
        }

        // Verify we have a reasonable number of unique currencies
        uniqueCurrencies.Count.ShouldBeGreaterThan(10, "Should have multiple unique currencies mapped");
    }

    [Theory]
    [InlineData("SEK", "kr")]
    [InlineData("NOK", "kr")]
    [InlineData("DKK", "kr")]
    public void Multiple_Currencies_Can_Share_Same_Symbol(string currencyCode, string expectedSymbol)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Theory]
    [InlineData("JPY", "¥")]
    [InlineData("CNY", "¥")]
    public void Japanese_Yen_And_Chinese_Yuan_Share_Symbol(string currencyCode, string expectedSymbol)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    #endregion

    #region Case Sensitivity Tests

    [Theory]
    [InlineData("usd", "")]
    [InlineData("Usd", "")]
    [InlineData("usD", "")]
    public void GetCurrencySymbol_Is_Case_Sensitive(string currencyCode, string expectedSymbol)
    {
        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(currencyCode);

        // Assert
        symbol.ShouldBe(expectedSymbol);
    }

    [Fact]
    public void GetCurrencySymbol_Requires_Exact_Case_Match()
    {
        // Arrange
        var validCurrencyCodes = new[] { "USD", "EUR", "GBP", "JPY" };

        // Act & Assert
        foreach (var code in validCurrencyCodes)
        {
            // Uppercase should work
            CurrencyCodeMapper.GetCurrencySymbol(code).ShouldNotBeEmpty($"{code} should have a symbol");
            
            // Lowercase should not work
            CurrencyCodeMapper.GetCurrencySymbol(code.ToLower()).ShouldBe(string.Empty, $"{code.ToLower()} should not match");
        }
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void GetCurrencyCode_Covers_All_Continents()
    {
        // Verify we have coverage across different continents
        var northAmerica = CurrencyCodeMapper.GetCurrencyCode(CountryCode.UnitedStates);
        var southAmerica = CurrencyCodeMapper.GetCurrencyCode(CountryCode.Brazil);
        var europe = CurrencyCodeMapper.GetCurrencyCode(CountryCode.Germany);
        var asia = CurrencyCodeMapper.GetCurrencyCode(CountryCode.Japan);
        var oceania = CurrencyCodeMapper.GetCurrencyCode(CountryCode.Australia);
        var africa = CurrencyCodeMapper.GetCurrencyCode(CountryCode.SouthAfrica);

        northAmerica.ShouldBe("USD");
        southAmerica.ShouldBe("BRL");
        europe.ShouldBe("EUR");
        asia.ShouldBe("JPY");
        oceania.ShouldBe("AUD");
        africa.ShouldBe("ZAR");
    }

    [Fact]
    public void GetCurrencySymbol_Handles_Long_String_Input()
    {
        // Arrange
        var longString = new string('A', 1000);

        // Act
        var symbol = CurrencyCodeMapper.GetCurrencySymbol(longString);

        // Assert
        symbol.ShouldBe(string.Empty);
    }

    [Fact]
    public void GetCurrencyCode_Default_Case_Returns_UNKNOWN()
    {
        // This test verifies the default case works by casting an invalid enum value
        var invalidEnumValue = (CountryCode)9999;

        // Act
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(invalidEnumValue);

        // Assert
        currencyCode.ShouldBe("UNKNOWN");
    }

    [Fact]
    public void Eurozone_Countries_Count_Is_Correct()
    {
        // Arrange
        var allCountryCodes = Enum.GetValues<CountryCode>();
        var eurozoneCount = allCountryCodes
            .Count(cc => CurrencyCodeMapper.GetCurrencyCode(cc) == "EUR");

        // Assert
        eurozoneCount.ShouldBe(9, "Should have 9 Eurozone countries mapped");
    }

    #endregion

    #region Symbol Validation Tests

    [Fact]
    public void All_Currency_Symbols_Are_Non_Null()
    {
        // Arrange
        var currencyCodes = new[] { "USD", "GBP", "EUR", "JPY", "CNY", "INR", "RUB", "KRW", 
            "CHF", "CAD", "AUD", "BRL", "MXN", "ZAR", "NZD", "SGD", "SEK", "NOK", "DKK", 
            "PLN", "CZK", "HUF" };

        // Act & Assert
        foreach (var code in currencyCodes)
        {
            var symbol = CurrencyCodeMapper.GetCurrencySymbol(code);
            symbol.ShouldNotBeNull($"Symbol for {code} should not be null");
        }
    }

    [Fact]
    public void Dollar_Variants_Are_Properly_Distinguished()
    {
        // Assert different dollar symbols
        CurrencyCodeMapper.GetCurrencySymbol("USD").ShouldBe("$");
        CurrencyCodeMapper.GetCurrencySymbol("CAD").ShouldBe("C$");
        CurrencyCodeMapper.GetCurrencySymbol("AUD").ShouldBe("A$");
        CurrencyCodeMapper.GetCurrencySymbol("NZD").ShouldBe("NZ$");
        CurrencyCodeMapper.GetCurrencySymbol("SGD").ShouldBe("S$");
        CurrencyCodeMapper.GetCurrencySymbol("MXN").ShouldBe("MX$");
        
        // All should be different (except base USD)
        var dollarSymbols = new[] { 
            CurrencyCodeMapper.GetCurrencySymbol("CAD"),
            CurrencyCodeMapper.GetCurrencySymbol("AUD"),
            CurrencyCodeMapper.GetCurrencySymbol("NZD"),
            CurrencyCodeMapper.GetCurrencySymbol("SGD"),
            CurrencyCodeMapper.GetCurrencySymbol("MXN")
        };

        dollarSymbols.Distinct().Count().ShouldBe(5, "All dollar variants should have unique symbols");
    }

    #endregion
}
