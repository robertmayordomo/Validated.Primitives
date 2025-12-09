using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a monetary value in the smallest currency units (cents, pennies, etc.) with currency code.
/// Uses uint to represent the smallest indivisible unit (e.g., 1050 cents = $10.50).
/// This prevents floating-point precision issues in financial calculations.
/// </summary>
[JsonConverter(typeof(SmallUnitMoneyConverter))]
public sealed record SmallUnitMoney : ValidatedPrimitive<uint>
{
    /// <summary>
    /// Gets the country code associated with the currency.
    /// </summary>
    public CountryCode CountryCode { get; }

    private SmallUnitMoney(uint value, CountryCode countryCode, string propertyName = "SmallUnitMoney") : base(value)
    {
        // No additional validators needed - uint is inherently non-negative
        CountryCode = countryCode;
    }

    /// <summary>
    /// Attempts to create a SmallUnitMoney instance with validation.
    /// </summary>
    /// <param name="countryCode">The country code for the currency.</param>
    /// <param name="value">The amount in smallest currency units (cents, pennies, etc.).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the SmallUnitMoney instance if valid.</returns>
    public static (ValidationResult Result, SmallUnitMoney? Value) TryCreate(
        CountryCode countryCode,
        uint value,
        string propertyName = "SmallUnitMoney")
    {
        var money = new SmallUnitMoney(value, countryCode, propertyName);
        var validationResult = money.Validate();
        var result = validationResult.IsValid ? money : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets the currency symbol based on the country code.
    /// </summary>
    public string GetCurrencySymbol() => CountryCode switch
    {
        CountryCode.UnitedStates => "$",
        CountryCode.UnitedKingdom => "£",
        CountryCode.Canada => "C$",
        CountryCode.Australia => "A$",
        CountryCode.Germany => "€",
        CountryCode.France => "€",
        CountryCode.Italy => "€",
        CountryCode.Spain => "€",
        CountryCode.Netherlands => "€",
        CountryCode.Belgium => "€",
        CountryCode.Austria => "€",
        CountryCode.Portugal => "€",
        CountryCode.Ireland => "€",
        CountryCode.Switzerland => "CHF",
        CountryCode.Sweden => "kr",
        CountryCode.Norway => "kr",
        CountryCode.Denmark => "kr",
        CountryCode.Poland => "z?",
        CountryCode.CzechRepublic => "K?",
        CountryCode.Hungary => "Ft",
        CountryCode.Japan => "¥",
        CountryCode.China => "¥",
        CountryCode.India => "?",
        CountryCode.Brazil => "R$",
        CountryCode.Mexico => "MX$",
        CountryCode.SouthAfrica => "R",
        CountryCode.NewZealand => "NZ$",
        CountryCode.Singapore => "S$",
        CountryCode.SouthKorea => "?",
        CountryCode.Russia => "?",
        _ => string.Empty
    };

    /// <summary>
    /// Gets the ISO currency code based on the country code.
    /// </summary>
    public string GetCurrencyCode() => CountryCode switch
    {
        CountryCode.UnitedStates => "USD",
        CountryCode.UnitedKingdom => "GBP",
        CountryCode.Canada => "CAD",
        CountryCode.Australia => "AUD",
        CountryCode.Germany => "EUR",
        CountryCode.France => "EUR",
        CountryCode.Italy => "EUR",
        CountryCode.Spain => "EUR",
        CountryCode.Netherlands => "EUR",
        CountryCode.Belgium => "EUR",
        CountryCode.Austria => "EUR",
        CountryCode.Portugal => "EUR",
        CountryCode.Ireland => "EUR",
        CountryCode.Switzerland => "CHF",
        CountryCode.Sweden => "SEK",
        CountryCode.Norway => "NOK",
        CountryCode.Denmark => "DKK",
        CountryCode.Poland => "PLN",
        CountryCode.CzechRepublic => "CZK",
        CountryCode.Hungary => "HUF",
        CountryCode.Japan => "JPY",
        CountryCode.China => "CNY",
        CountryCode.India => "INR",
        CountryCode.Brazil => "BRL",
        CountryCode.Mexico => "MXN",
        CountryCode.SouthAfrica => "ZAR",
        CountryCode.NewZealand => "NZD",
        CountryCode.Singapore => "SGD",
        CountryCode.SouthKorea => "KRW",
        CountryCode.Russia => "RUB",
        _ => "UNKNOWN"
    };

    /// <summary>
    /// Gets the number of decimal places for the currency (usually 2, but some currencies like JPY use 0).
    /// </summary>
    public int GetDecimalPlaces() => CountryCode switch
    {
        CountryCode.Japan => 0,
        CountryCode.SouthKorea => 0,
        _ => 2
    };

    /// <summary>
    /// Converts the smallest units to a decimal representation.
    /// </summary>
    public decimal ToDecimal()
    {
        var decimalPlaces = GetDecimalPlaces();
        if (decimalPlaces == 0)
            return Value;

        return Value / (decimal)Math.Pow(10, decimalPlaces);
    }

    /// <summary>
    /// Returns a formatted string representation of the money value.
    /// </summary>
    public override string ToString() => $"{GetCurrencySymbol()}{ToDecimal():N2}";

    /// <summary>
    /// Returns a formatted string with the currency code.
    /// </summary>
    public string ToStringWithCode() => $"{ToDecimal():N2} {GetCurrencyCode()}";

    /// <summary>
    /// Returns the raw value in smallest units (e.g., "1050 cents").
    /// </summary>
    public string ToRawString() => $"{Value} {GetSmallestUnitName()}";

    /// <summary>
    /// Gets the name of the smallest currency unit.
    /// </summary>
    public string GetSmallestUnitName() => CountryCode switch
    {
        CountryCode.UnitedStates => "cents",
        CountryCode.UnitedKingdom => "pence",
        CountryCode.Canada => "cents",
        CountryCode.Australia => "cents",
        CountryCode.Germany => "cents",
        CountryCode.France => "cents",
        CountryCode.Italy => "cents",
        CountryCode.Spain => "cents",
        CountryCode.Netherlands => "cents",
        CountryCode.Belgium => "cents",
        CountryCode.Austria => "cents",
        CountryCode.Portugal => "cents",
        CountryCode.Ireland => "cents",
        CountryCode.Switzerland => "rappen",
        CountryCode.Sweden => "öre",
        CountryCode.Norway => "øre",
        CountryCode.Denmark => "øre",
        CountryCode.Poland => "groszy",
        CountryCode.CzechRepublic => "halé??",
        CountryCode.Hungary => "fillér",
        CountryCode.Japan => "yen",
        CountryCode.China => "fen",
        CountryCode.India => "paise",
        CountryCode.Brazil => "centavos",
        CountryCode.Mexico => "centavos",
        CountryCode.SouthAfrica => "cents",
        CountryCode.NewZealand => "cents",
        CountryCode.Singapore => "cents",
        CountryCode.SouthKorea => "won",
        CountryCode.Russia => "kopeks",
        _ => "units"
    };

    /// <summary>
    /// Determines whether the specified SmallUnitMoney is equal to the current SmallUnitMoney.
    /// </summary>
    public bool Equals(SmallUnitMoney? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && CountryCode == other.CountryCode;
    }

    /// <summary>
    /// Returns the hash code for this SmallUnitMoney.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, CountryCode);
    }
}
