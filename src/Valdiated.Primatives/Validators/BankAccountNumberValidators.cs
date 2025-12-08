using System.Text.RegularExpressions;
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

/// <summary>
/// Provides validators for bank account numbers across different countries.
/// Supports validation for various international account number formats including
/// IBAN (International Bank Account Number) for European countries and
/// country-specific formats for others.
/// </summary>
public static partial class BankAccountNumberValidators
{
    /// <summary>
    /// Validates that the bank account number is not null or whitespace.
    /// </summary>
    public static ValueValidator<string> NotNullOrWhitespace(string propertyName = "BankAccountNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Failure(
                    "Bank account number must be provided",
                    propertyName,
                    "Required");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates the basic format of a bank account number.
    /// </summary>
    public static ValueValidator<string> ValidFormat(string propertyName = "BankAccountNumber")
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim();
            
            if (!AccountNumberFormatRegex().IsMatch(normalized))
            {
                return ValidationResult.Failure(
                    "Bank account number contains invalid characters",
                    propertyName,
                    "InvalidFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates bank account number format specific to the country.
    /// </summary>
    public static ValueValidator<string> ValidateCountryFormat(string propertyName, CountryCode countryCode)
    {
        return value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Replace(" ", "").Replace("-", "").Trim().ToUpperInvariant();
            var validationResult = countryCode switch
            {
                CountryCode.UnitedKingdom => ValidateUkAccountNumber(normalized),
                CountryCode.UnitedStates => ValidateUsAccountNumber(normalized),
                CountryCode.Germany => ValidateIban(normalized, "DE", 22),
                CountryCode.France => ValidateIban(normalized, "FR", 27),
                CountryCode.Italy => ValidateIban(normalized, "IT", 27),
                CountryCode.Spain => ValidateIban(normalized, "ES", 24),
                CountryCode.Netherlands => ValidateIban(normalized, "NL", 18),
                CountryCode.Belgium => ValidateIban(normalized, "BE", 16),
                CountryCode.Switzerland => ValidateIban(normalized, "CH", 21),
                CountryCode.Austria => ValidateIban(normalized, "AT", 20),
                CountryCode.Sweden => ValidateIban(normalized, "SE", 24),
                CountryCode.Norway => ValidateIban(normalized, "NO", 15),
                CountryCode.Denmark => ValidateIban(normalized, "DK", 18),
                CountryCode.Finland => ValidateIban(normalized, "FI", 18),
                CountryCode.Poland => ValidateIban(normalized, "PL", 28),
                CountryCode.CzechRepublic => ValidateIban(normalized, "CZ", 24),
                CountryCode.Hungary => ValidateIban(normalized, "HU", 28),
                CountryCode.Portugal => ValidateIban(normalized, "PT", 25),
                CountryCode.Ireland => ValidateIban(normalized, "IE", 22),
                CountryCode.Australia => ValidateAustraliaAccountNumber(normalized),
                CountryCode.Canada => ValidateCanadaAccountNumber(normalized),
                CountryCode.Japan => ValidateJapanAccountNumber(normalized),
                CountryCode.Singapore => ValidateSingaporeAccountNumber(normalized),
                CountryCode.India => ValidateIndiaAccountNumber(normalized),
                CountryCode.China => ValidateChinaAccountNumber(normalized),
                CountryCode.Brazil => ValidateBrazilAccountNumber(normalized),
                _ => (true, string.Empty)
            };

            if (!validationResult.Item1)
            {
                return ValidationResult.Failure(
                    validationResult.Item2,
                    propertyName,
                    "InvalidCountryAccountNumberFormat");
            }

            return ValidationResult.Success();
        };
    }

    /// <summary>
    /// Validates UK bank account number (8 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateUkAccountNumber(string accountNumber)
    {
        if (UkAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "UK bank account number must be 8 digits");
    }

    /// <summary>
    /// Validates US bank account number (typically 4-17 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateUsAccountNumber(string accountNumber)
    {
        if (UsAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "US bank account number must be 4-17 digits");
    }

    /// <summary>
    /// Validates IBAN format with country code and length check.
    /// Note: This is a basic format validation, not a full IBAN checksum validation.
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateIban(string iban, string countryCode, int expectedLength)
    {
        if (!iban.StartsWith(countryCode))
        {
            return (false, $"IBAN must start with {countryCode} for this country");
        }

        if (iban.Length != expectedLength)
        {
            return (false, $"IBAN must be {expectedLength} characters long for {countryCode}");
        }

        if (!IbanFormatRegex().IsMatch(iban))
        {
            return (false, "IBAN format is invalid. Expected format: 2 letter country code + 2 check digits + account number");
        }

        // Basic IBAN checksum validation
        if (!ValidateIbanChecksum(iban))
        {
            return (false, "IBAN checksum is invalid");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Validates IBAN checksum using mod-97 algorithm.
    /// </summary>
    private static bool ValidateIbanChecksum(string iban)
    {
        // Move first 4 characters to end
        var rearranged = iban.Substring(4) + iban.Substring(0, 4);
        
        // Convert letters to numbers (A=10, B=11, ..., Z=35)
        var numericString = string.Empty;
        foreach (var c in rearranged)
        {
            if (char.IsLetter(c))
            {
                numericString += (c - 'A' + 10).ToString();
            }
            else
            {
                numericString += c;
            }
        }

        // Calculate mod 97
        var remainder = 0;
        foreach (var c in numericString)
        {
            remainder = (remainder * 10 + (c - '0')) % 97;
        }

        return remainder == 1;
    }

    /// <summary>
    /// Validates Australian BSB + account number format.
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateAustraliaAccountNumber(string accountNumber)
    {
        // Australian account numbers are typically 6-9 digits
        if (AustraliaAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Australian bank account number must be 6-9 digits");
    }

    /// <summary>
    /// Validates Canadian bank account number (7-12 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateCanadaAccountNumber(string accountNumber)
    {
        if (CanadaAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Canadian bank account number must be 7-12 digits");
    }

    /// <summary>
    /// Validates Japanese bank account number (7 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateJapanAccountNumber(string accountNumber)
    {
        if (JapanAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Japanese bank account number must be 7 digits");
    }

    /// <summary>
    /// Validates Singapore bank account number (8-15 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateSingaporeAccountNumber(string accountNumber)
    {
        if (SingaporeAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Singapore bank account number must be 8-15 digits");
    }

    /// <summary>
    /// Validates Indian bank account number (9-18 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateIndiaAccountNumber(string accountNumber)
    {
        if (IndiaAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Indian bank account number must be 9-18 digits");
    }

    /// <summary>
    /// Validates Chinese bank account number (16-19 digits).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateChinaAccountNumber(string accountNumber)
    {
        if (ChinaAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Chinese bank account number must be 16-19 digits");
    }

    /// <summary>
    /// Validates Brazilian bank account number (variable length with optional check digit).
    /// </summary>
    private static (bool IsValid, string ErrorMessage) ValidateBrazilAccountNumber(string accountNumber)
    {
        if (BrazilAccountNumberRegex().IsMatch(accountNumber))
            return (true, string.Empty);
        
        return (false, "Brazilian bank account number format is invalid");
    }

    [GeneratedRegex(@"^[A-Z0-9\s\-]+$")]
    private static partial Regex AccountNumberFormatRegex();

    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex UkAccountNumberRegex();

    [GeneratedRegex(@"^\d{4,17}$")]
    private static partial Regex UsAccountNumberRegex();

    [GeneratedRegex(@"^[A-Z]{2}\d{2}[A-Z0-9]+$")]
    private static partial Regex IbanFormatRegex();

    [GeneratedRegex(@"^\d{6,9}$")]
    private static partial Regex AustraliaAccountNumberRegex();

    [GeneratedRegex(@"^\d{7,12}$")]
    private static partial Regex CanadaAccountNumberRegex();

    [GeneratedRegex(@"^\d{7}$")]
    private static partial Regex JapanAccountNumberRegex();

    [GeneratedRegex(@"^\d{8,15}$")]
    private static partial Regex SingaporeAccountNumberRegex();

    [GeneratedRegex(@"^\d{9,18}$")]
    private static partial Regex IndiaAccountNumberRegex();

    [GeneratedRegex(@"^\d{16,19}$")]
    private static partial Regex ChinaAccountNumberRegex();

    [GeneratedRegex(@"^\d{1,13}[A-Z0-9]?$")]
    private static partial Regex BrazilAccountNumberRegex();
}
