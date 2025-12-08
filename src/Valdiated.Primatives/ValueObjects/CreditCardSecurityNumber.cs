using System;
using System.Linq;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated credit card security number (CVV/CVC).
/// Stores only the digits and validates the length.
/// </summary>
public sealed record CreditCardSecurityNumber : ValidatedValueObject<string>
{
    private CreditCardSecurityNumber(string input, string propertyName = "SecurityNumber")
        : base(ExtractDigits(input))
    {
        Validators.Add(CreditCardSecurityNumberValidators.NotEmpty(propertyName));
        Validators.Add(CreditCardSecurityNumberValidators.ValidLength(propertyName));
    }

    /// <summary>
    /// Attempts to create a CreditCardSecurityNumber instance with validation.
    /// </summary>
    /// <param name="input">The security number input (may contain non-digit characters).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the CreditCardSecurityNumber instance if valid.</returns>
    public static (ValidationResult Result, CreditCardSecurityNumber? Value) TryCreate(
        string? input,
        string propertyName = "SecurityNumber")
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            var emptyResult = ValidationResult.Failure(
                "Security number must be provided",
                propertyName,
                "Required");
            return (emptyResult, null);
        }

        var securityNumber = new CreditCardSecurityNumber(input, propertyName);
        var validationResult = securityNumber.Validate();
        var result = validationResult.IsValid ? securityNumber : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the security number as digits only.
    /// </summary>
    public override string ToString() => Value;

    private static string ExtractDigits(string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Determines whether the specified CreditCardSecurityNumber is equal to the current CreditCardSecurityNumber.
    /// </summary>
    public bool Equals(CreditCardSecurityNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Returns the hash code for this CreditCardSecurityNumber.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
