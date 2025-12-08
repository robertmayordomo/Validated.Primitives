using System;
using System.Linq;

namespace Validated.Primitives.ValueObjects;

public sealed class CreditCardSecurityNumber
{
    private readonly string _value;

    private CreditCardSecurityNumber(string value)
    {
        _value = value;
    }

    public static CreditCardSecurityNumber Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Security number must be provided", nameof(input));

        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.Length < 3 || digits.Length > 4)
            throw new ArgumentException("Security number must contain 3 or 4 digits", nameof(input));

        return new CreditCardSecurityNumber(digits);
    }

    public override string ToString() => _value;
}
