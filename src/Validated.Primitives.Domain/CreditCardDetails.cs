using Validated.Primitives;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain;

/// <summary>
/// Represents validated credit card details including card number, security code, and expiration date.
/// </summary>
public sealed record CreditCardDetails
{
    /// <summary>
    /// Gets the validated credit card number.
    /// </summary>
    public required CreditCardNumber CardNumber { get; init; }

    /// <summary>
    /// Gets the validated security number (CVV/CVC).
    /// </summary>
    public required CreditCardSecurityNumber SecurityNumber { get; init; }

    /// <summary>
    /// Gets the validated expiration date.
    /// </summary>
    public required CreditCardExpiration Expiration { get; init; }

    /// <summary>
    /// Creates new CreditCardDetails with validation.
    /// </summary>
    /// <param name="cardNumber">The credit card number string to validate.</param>
    /// <param name="securityNumber">The security number (CVV/CVC) string to validate.</param>
    /// <param name="expirationMonth">The expiration month (1-12).</param>
    /// <param name="expirationYear">The expiration year.</param>
    /// <returns>A tuple containing the validation result and the CreditCardDetails if valid.</returns>
    public static (ValidationResult Result, CreditCardDetails? Value) TryCreate(
        string cardNumber,
        string securityNumber,
        int expirationMonth,
        int expirationYear)
    {
        var result = ValidationResult.Success();

        // Validate card number
        CreditCardNumber? cardNumberValue = null;
        try
        {
            cardNumberValue = CreditCardNumber.Create(cardNumber);
        }
        catch (ArgumentException ex)
        {
            result.AddError(ex.Message, "CardNumber", "Invalid");
        }

        // Validate security number
        CreditCardSecurityNumber? securityNumberValue = null;
        try
        {
            securityNumberValue = CreditCardSecurityNumber.Create(securityNumber);
        }
        catch (ArgumentException ex)
        {
            result.AddError(ex.Message, "SecurityNumber", "Invalid");
        }

        // Validate expiration
        CreditCardExpiration? expirationValue = null;
        try
        {
            expirationValue = CreditCardExpiration.Create(expirationMonth, expirationYear);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            result.AddError(ex.Message, "Expiration", "Invalid");
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        var creditCardDetails = new CreditCardDetails
        {
            CardNumber = cardNumberValue!,
            SecurityNumber = securityNumberValue!,
            Expiration = expirationValue!
        };

        return (result, creditCardDetails);
    }

    /// <summary>
    /// Returns a masked string representation of the credit card details.
    /// </summary>
    public override string ToString()
    {
        return $"Card: {CardNumber.Masked()}, Expires: {Expiration}, CVV: ***";
    }

    /// <summary>
    /// Gets the masked card number for display purposes.
    /// </summary>
    public string GetMaskedCardNumber() => CardNumber.Masked();

    /// <summary>
    /// Checks if the card has expired.
    /// </summary>
    public bool IsExpired()
    {
        var now = DateTime.UtcNow;
        var expirationDate = new DateTime(Expiration.Year, Expiration.Month, DateTime.DaysInMonth(Expiration.Year, Expiration.Month));
        return expirationDate < new DateTime(now.Year, now.Month, 1);
    }
}
