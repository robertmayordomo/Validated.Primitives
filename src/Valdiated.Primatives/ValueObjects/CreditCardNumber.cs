using System;
using System.Linq;

namespace Validated.Primitives.ValueObjects;

public sealed class CreditCardNumber
{
    private readonly string _digits;

    private CreditCardNumber(string digits)
    {
        _digits = digits;
    }

    public static CreditCardNumber Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Credit card number must be provided", nameof(input));

        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.Length < 13 || digits.Length > 19)
            throw new ArgumentException("Credit card number must contain between 13 and 19 digits", nameof(input));

        if (digits.All(c => c == digits[0]))
            throw new ArgumentException("Credit card number cannot consist of all identical digits", nameof(input));

        if (!IsLuhnValid(digits))
            throw new ArgumentException("Credit card number is not valid (Luhn check failed)", nameof(input));

        return new CreditCardNumber(digits);
    }

    public override string ToString() => _digits;

    public string Masked()
    {
        if (_digits.Length <= 4) return _digits;
        var shown = _digits[^4..];
        return new string('*', _digits.Length - 4) + shown;
    }

    private static bool IsLuhnValid(string digits)
    {
        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int n = digits[i] - '0';
            if (alternate)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }
        return sum % 10 == 0;
    }
}
