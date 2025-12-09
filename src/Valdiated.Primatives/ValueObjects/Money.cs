using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a monetary value with currency code.
/// Uses decimal for representing currency amounts (e.g., 10.50 USD).
/// </summary>
[JsonConverter(typeof(MoneyConverter))]
public sealed record Money : ValidatedPrimitive<decimal>
{
    /// <summary>
    /// Gets the ISO 4217 currency code (e.g., USD, EUR, GBP).
    /// </summary>
    public string CurrencyCode { get; }

    private Money(decimal value, string currencyCode, string propertyName = "Money") : base(value)
    {
        Validators.Add(MoneyValidators.NonNegative(propertyName));
        Validators.Add(MoneyValidators.ValidDecimalPlaces(propertyName, 2));

        CurrencyCode = currencyCode;
    }

    /// <summary>
    /// Attempts to create a Money instance with validation using a specific currency code.
    /// </summary>
    /// <param name="currencyCode">The ISO 4217 currency code (e.g., USD, EUR, GBP).</param>
    /// <param name="value">The monetary amount as a decimal.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Money instance if valid.</returns>
    public static (ValidationResult Result, Money? Value) TryCreate(
        string currencyCode,
        decimal value,
        string propertyName = "Money")
    {
        var money = new Money(value, currencyCode, propertyName);
        var validationResult = money.Validate();
        var result = validationResult.IsValid ? money : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Attempts to create a Money instance with validation using a country code.
    /// The country code is mapped to the corresponding ISO 4217 currency code.
    /// </summary>
    /// <param name="countryCode">The country code for the currency.</param>
    /// <param name="value">The monetary amount as a decimal.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the Money instance if valid.</returns>
    public static (ValidationResult Result, Money? Value) TryCreate(
        CountryCode countryCode,
        decimal value,
        string propertyName = "Money")
    {
        var currencyCode = CurrencyCodeMapper.GetCurrencyCode(countryCode);
        var money = new Money(value, currencyCode, propertyName);
        var validationResult = money.Validate();
        var result = validationResult.IsValid ? money : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets the currency symbol based on the currency code.
    /// </summary>
    public string GetCurrencySymbol() => CurrencyCodeMapper.GetCurrencySymbol(CurrencyCode);

    /// <summary>
    /// Returns a formatted string representation of the money value.
    /// </summary>
    public override string ToString() => $"{GetCurrencySymbol()}{Value:N2}";

    /// <summary>
    /// Returns a formatted string with the currency code.
    /// </summary>
    public string ToStringWithCode() => $"{Value:N2} {CurrencyCode}";

    /// <summary>
    /// Determines whether the specified Money is equal to the current Money.
    /// </summary>
    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && CurrencyCode == other.CurrencyCode;
    }

    /// <summary>
    /// Returns the hash code for this Money.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, CurrencyCode);
    }
}
