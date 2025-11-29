using System.Text.RegularExpressions;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Validators for human names (first, middle, last names).
/// </summary>
public static partial class HumanNameValidators
{
    /// <summary>
    /// Validates that a name contains only alphabetic characters, hyphens, and apostrophes.
    /// Supports international characters and Irish names like O'Brien.
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function.</returns>
    public static ValueValidator<string> AlphaWithHyphenAndApostrophe(string fieldName)
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            if (!HumanNamePattern().IsMatch(value))
            {
                return ValidationResult.Failure(
                    $"{fieldName} can only contain letters, hyphens, and apostrophes.",
                    fieldName,
                    "AlphaWithHyphenAndApostrophe");
            }

            return ValidationResult.Success();
        };

    /// <summary>
    /// Regex pattern that matches alphabetic characters (including Unicode letters), hyphens, and apostrophes.
    /// Allows for names like O'Brien, Mary-Jane, José, François, etc.
    /// </summary>
    [GeneratedRegex(@"^[\p{L}\-']+$", RegexOptions.Compiled)]
    private static partial Regex HumanNamePattern();
}
