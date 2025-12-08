using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Builders;

/// <summary>
/// Builder for creating validated CreditCardDetails with a fluent interface.
/// </summary>
public sealed class CreditCardBuilder
{
    private string? _cardNumber;
    private string? _securityCode;
    private int? _expirationMonth;
    private int? _expirationYear;

    /// <summary>
    /// Sets the credit card number.
    /// </summary>
    /// <param name="cardNumber">The credit card number string.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder WithCardNumber(string cardNumber)
    {
        _cardNumber = cardNumber;
        return this;
    }

    /// <summary>
    /// Sets the security code (CVV/CVC) as a string.
    /// </summary>
    /// <param name="securityCode">The security code string.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder WithSecurityCode(string securityCode)
    {
        _securityCode = securityCode;
        return this;
    }

    /// <summary>
    /// Sets the security code (CVV/CVC) as an integer.
    /// </summary>
    /// <param name="securityCode">The security code integer.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder WithSecurityCode(int securityCode)
    {
        _securityCode = securityCode.ToString();
        return this;
    }

    /// <summary>
    /// Sets the expiration date using a DateTime.
    /// </summary>
    /// <param name="expiration">The expiration DateTime.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder WithExpiration(DateTime expiration)
    {
        _expirationMonth = expiration.Month;
        _expirationYear = expiration.Year;
        return this;
    }

    /// <summary>
    /// Sets the expiration date using a string in MM/YY or MM/YYYY format.
    /// </summary>
    /// <param name="expiration">The expiration string (e.g., "12/25" or "12/2025").</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder WithExpiration(string expiration)
    {
        if (string.IsNullOrWhiteSpace(expiration))
        {
            return this;
        }

        var parts = expiration.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 2 &&
            int.TryParse(parts[0], out var month) &&
            int.TryParse(parts[1], out var year))
        {
            _expirationMonth = month;
            _expirationYear = year;
        }

        return this;
    }

    /// <summary>
    /// Sets the expiration date using month and year values.
    /// </summary>
    /// <param name="month">The expiration month (1-12).</param>
    /// <param name="year">The expiration year.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder WithExpiration(int month, int year)
    {
        _expirationMonth = month;
        _expirationYear = year;
        return this;
    }

    /// <summary>
    /// Builds the CreditCardDetails with validation.
    /// </summary>
    /// <returns>A tuple containing the validation result and the CreditCardDetails if valid.</returns>
    public (ValidationResult Result, CreditCardDetails? Value) Build() 
        => CreditCardDetails.TryCreate(_cardNumber, _securityCode, _expirationMonth, _expirationYear);

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CreditCardBuilder Reset()
    {
        _cardNumber = null;
        _securityCode = null;
        _expirationMonth = null;
        _expirationYear = null;
        return this;
    }
}
