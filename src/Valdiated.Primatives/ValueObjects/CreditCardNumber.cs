using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated credit card number.
/// Stores only the digits and validates using the Luhn algorithm.
/// </summary>
public sealed record CreditCardNumber : ValidatedPrimitive<string>
{
    private CreditCardNumber(string input, string propertyName = "CreditCardNumber") 
        : base(ExtractDigits(input))
    {
        Validators.Add(CreditCardValidators.NotEmpty(propertyName));
        Validators.Add(CreditCardValidators.ValidDigitCount(propertyName));
        Validators.Add(CreditCardValidators.NotAllIdenticalDigits(propertyName));
        Validators.Add(CreditCardValidators.LuhnCheck(propertyName));
    }

    /// <summary>
    /// Attempts to create a CreditCardNumber instance with validation.
    /// </summary>
    /// <param name="input">The credit card number input (may contain spaces or hyphens).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the CreditCardNumber instance if valid.</returns>
    public static (ValidationResult Result, CreditCardNumber? Value) TryCreate(
        string? input,
        string propertyName = "CreditCardNumber")
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            var emptyResult = ValidationResult.Failure(
                "Credit card number must be provided",
                propertyName,
                "Required");
            return (emptyResult, null);
        }

        var creditCard = new CreditCardNumber(input, propertyName);
        var validationResult = creditCard.Validate();
        var result = validationResult.IsValid ? creditCard : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the credit card number as digits only.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Returns a masked version of the credit card number showing only the last 4 digits.
    /// </summary>
    public string Masked()
    {
        if (Value.Length <= 4) return Value;
        var shown = Value[^4..];
        return new string('*', Value.Length - 4) + shown;
    }

    private static string ExtractDigits(string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Determines whether the specified CreditCardNumber is equal to the current CreditCardNumber.
    /// </summary>
    public bool Equals(CreditCardNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Returns the hash code for this CreditCardNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
