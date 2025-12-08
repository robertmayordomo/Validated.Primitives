using System.Text.RegularExpressions;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for US Social Security Numbers.
/// </summary>
public static class SocialSecurityNumberValidators
{
    private static readonly Regex SsnFormatRegex = new Regex(@"^\d{3}-?\d{2}-?\d{4}$", RegexOptions.Compiled);
    private static readonly Regex DigitsOnlyRegex = new Regex(@"^\d{9}$", RegexOptions.Compiled);

    /// <summary>
    /// Validates that the SSN is not empty or whitespace.
    /// </summary>
    public static ValueValidator<string> NotEmpty(string propertyName = "SocialSecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure(
                    "Social Security Number must be provided",
                    propertyName,
                    "Required");

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the format of the Social Security Number (XXX-XX-XXXX or XXXXXXXXX).
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "SocialSecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
            
            // Must have exactly 9 digits
            if (digitsOnly.Length != 9)
                return ValidationResult.Failure(
                    "Social Security Number must be in format XXX-XX-XXXX or 9 digits",
                    propertyName,
                    "InvalidFormat");

            // If input contains dashes, validate the format more strictly
            if (value.Contains('-'))
            {
                // Remove common prefixes and whitespace to get to the SSN part
                var cleaned = value.Trim();
                
                // Find the first digit
                var firstDigitIdx = -1;
                for (int i = 0; i < cleaned.Length; i++)
                {
                    if (char.IsDigit(cleaned[i]))
                    {
                        firstDigitIdx = i;
                        break;
                    }
                }
                
                if (firstDigitIdx >= 0)
                {
                    // Extract the portion from first digit onward, stopping at first non-SSN character
                    var ssnPortion = cleaned.Substring(firstDigitIdx);
                    var ssnEnd = ssnPortion.Length;
                    for (int i = 0; i < ssnPortion.Length; i++)
                    {
                        char c = ssnPortion[i];
                        if (!char.IsDigit(c) && c != '-' && c != ' ')
                        {
                            ssnEnd = i;
                            break;
                        }
                    }
                    
                    var ssnLike = ssnPortion.Substring(0, ssnEnd).Replace(" ", "").Trim();
                    
                    // If this contains dashes and has 9 digits, validate strict format
                    if (ssnLike.Contains('-'))
                    {
                        var ssnDigits = new string(ssnLike.Where(char.IsDigit).ToArray());
                        if (ssnDigits.Length == 9)
                        {
                            // Must match XXX-XX-XXXX exactly
                            if (!Regex.IsMatch(ssnLike, @"^\d{3}-\d{2}-\d{4}$"))
                                return ValidationResult.Failure(
                                    "Social Security Number must be in format XXX-XX-XXXX or 9 digits",
                                    propertyName,
                                    "InvalidFormat");
                        }
                    }
                }
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the area number (first three digits) of the SSN.
    /// Invalid area numbers: 000, 666, 900-999
    /// </summary>
    public static ValueValidator<string> ValidAreaNumber(string propertyName = "SocialSecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
            
            if (digitsOnly.Length < 3)
                return ValidationResult.Success(); // Let format validator handle this

            var areaNumber = int.Parse(digitsOnly.Substring(0, 3));

            if (areaNumber == 0)
                return ValidationResult.Failure(
                    "Social Security Number cannot have area number 000",
                    propertyName,
                    "InvalidAreaNumber");

            if (areaNumber == 666)
                return ValidationResult.Failure(
                    "Social Security Number cannot have area number 666",
                    propertyName,
                    "InvalidAreaNumber");

            if (areaNumber >= 900 && areaNumber <= 999)
                return ValidationResult.Failure(
                    "Social Security Number cannot have area number 900-999",
                    propertyName,
                    "InvalidAreaNumber");

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the group number (middle two digits) of the SSN.
    /// Invalid group number: 00
    /// </summary>
    public static ValueValidator<string> ValidGroupNumber(string propertyName = "SocialSecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
            
            if (digitsOnly.Length < 5)
                return ValidationResult.Success(); // Let format validator handle this

            var groupNumber = int.Parse(digitsOnly.Substring(3, 2));

            if (groupNumber == 0)
                return ValidationResult.Failure(
                    "Social Security Number cannot have group number 00",
                    propertyName,
                    "InvalidGroupNumber");

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the serial number (last four digits) of the SSN.
    /// Invalid serial number: 0000
    /// </summary>
    public static ValueValidator<string> ValidSerialNumber(string propertyName = "SocialSecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
            
            if (digitsOnly.Length < 9)
                return ValidationResult.Success(); // Let format validator handle this

            var serialNumber = int.Parse(digitsOnly.Substring(5, 4));

            if (serialNumber == 0)
                return ValidationResult.Failure(
                    "Social Security Number cannot have serial number 0000",
                    propertyName,
                    "InvalidSerialNumber");

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates against known invalid/test SSN patterns.
    /// Advertising SSN: 987-65-4320 through 987-65-4329
    /// </summary>
    public static ValueValidator<string> NotAdvertisingNumber(string propertyName = "SocialSecurityNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());
            
            if (digitsOnly.Length != 9)
                return ValidationResult.Success();

            // Check for known advertising numbers (987-65-4320 through 987-65-4329)
            if (digitsOnly.StartsWith("987654") && digitsOnly.Length == 9)
            {
                var lastThreeDigits = digitsOnly.Substring(6, 3);
                if (lastThreeDigits.StartsWith("32") && digitsOnly[8] >= '0' && digitsOnly[8] <= '9')
                {
                    return ValidationResult.Failure(
                        "Social Security Number cannot be a known advertising/test number",
                        propertyName,
                        "AdvertisingNumber");
                }
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the format of the input SSN string before digit extraction.
    /// This ensures that if dashes are present, they are in the correct positions.
    /// </summary>
    public static ValidationResult ValidateInputFormat(string value, string propertyName = "SocialSecurityNumber")
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Success();

        var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

        // Must have exactly 9 digits
        if (digitsOnly.Length != 9)
            return ValidationResult.Failure(
                "Social Security Number must be in format XXX-XX-XXXX or 9 digits",
                propertyName,
                "InvalidFormat");

        // If input contains dashes, validate the format more strictly
        if (value.Contains('-'))
        {
            // Remove common prefixes and whitespace to get to the SSN part
            var cleaned = value.Trim();

            // Find the first digit
            var firstDigitIdx = -1;
            for (int i = 0; i < cleaned.Length; i++)
            {
                if (char.IsDigit(cleaned[i]))
                {
                    firstDigitIdx = i;
                    break;
                }
            }

            if (firstDigitIdx >= 0)
            {
                // Extract the portion from first digit onward, stopping at first non-SSN character
                var ssnPortion = cleaned.Substring(firstDigitIdx);
                var ssnEnd = ssnPortion.Length;
                for (int i = 0; i < ssnPortion.Length; i++)
                {
                    char c = ssnPortion[i];
                    if (!char.IsDigit(c) && c != '-' && c != ' ')
                    {
                        ssnEnd = i;
                        break;
                    }
                }

                var ssnLike = ssnPortion.Substring(0, ssnEnd).Replace(" ", "").Trim();

                // If this contains dashes and has 9 digits, validate strict format
                if (ssnLike.Contains('-'))
                {
                    var ssnDigits = new string(ssnLike.Where(char.IsDigit).ToArray());
                    if (ssnDigits.Length == 9)
                    {
                        // Must match XXX-XX-XXXX exactly
                        if (!Regex.IsMatch(ssnLike, @"^\d{3}-\d{2}-\d{4}$"))
                            return ValidationResult.Failure(
                                "Social Security Number must be in format XXX-XX-XXXX or 9 digits",
                                propertyName,
                                "InvalidFormat");
                    }
                }
            }
        }

        return ValidationResult.Success();
    }
}
