namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents the type of bank account number format.
/// </summary>
public enum BankAccountNumberType
{
    /// <summary>
    /// Unknown or unidentified format.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// IBAN (International Bank Account Number) - ISO 13616 standard.
    /// Format: 2 letter country code + 2 check digits + up to 30 alphanumeric characters.
    /// Used internationally, primarily in Europe and many other countries.
    /// Example: DE89370400440532013000 (Germany, 22 characters)
    /// </summary>
    Iban = 1,

    /// <summary>
    /// BBAN (Basic Bank Account Number) - Country-specific domestic format.
    /// The national/domestic account number format without IBAN structure.
    /// Format varies by country (e.g., 8 digits in UK, 10-12 digits in US).
    /// Example: 12345678 (UK domestic account number)
    /// </summary>
    Bban = 2
}
