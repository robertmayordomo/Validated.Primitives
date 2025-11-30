namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Maps country codes to their corresponding ISO 4217 currency codes.
/// </summary>
public static class CurrencyCodeMapper
{
    /// <summary>
    /// Gets the ISO 4217 currency code for a given country code.
    /// </summary>
    /// <param name="countryCode">The country code to map.</param>
    /// <returns>The corresponding ISO 4217 currency code, or "UNKNOWN" if no mapping exists.</returns>
    public static string GetCurrencyCode(CountryCode countryCode) => countryCode switch
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
    /// Gets the currency symbol for a given currency code.
    /// </summary>
    /// <param name="currencyCode">The ISO 4217 currency code.</param>
    /// <returns>The currency symbol, or empty string if no symbol is defined.</returns>
    public static string GetCurrencySymbol(string currencyCode) => currencyCode switch
    {
        "USD" => "$",
        "GBP" => "£",
        "EUR" => "€",
        "JPY" => "¥",
        "CNY" => "¥",
        "INR" => "₹",
        "RUB" => "₽",
        "KRW" => "?",
        "CHF" => "CHF",
        "CAD" => "C$",
        "AUD" => "A$",
        "BRL" => "R$",
        "MXN" => "MX$",
        "ZAR" => "R",
        "NZD" => "NZ$",
        "SGD" => "S$",
        "SEK" => "kr",
        "NOK" => "kr",
        "DKK" => "kr",
        "PLN" => "z?",
        "CZK" => "K?",
        "HUF" => "Ft",
        _ => string.Empty
    };
}
