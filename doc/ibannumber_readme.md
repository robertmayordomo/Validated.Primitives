# Validated.Primitives.IbanNumber

A validated international bank account number primitive that supports both IBAN and BBAN formats with automatic detection and ISO 13616 compliance, ensuring valid international banking account numbers when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`IbanNumber` is a validated value object that represents international bank account numbers. It automatically detects and validates both IBAN (International Bank Account Number) and BBAN (Basic Bank Account Number) formats according to ISO 13616 standards. The class provides comprehensive parsing, formatting, and component extraction methods. Once created, an `IbanNumber` instance is guaranteed to be valid.

### Key Features

- **Automatic Format Detection** - Detects IBAN vs BBAN automatically
- **ISO 13616 Compliance** - Validates according to international banking standards
- **IBAN Checksum Validation** - Uses mod-97 algorithm for IBAN validation
- **Country-Specific Validation** - Validates country-specific lengths and formats
- **Component Extraction** - Extracts country code, check digits, and BBAN parts
- **Format Conversion** - Convert between different display formats
- **Security Masking** - Provides masked display for privacy
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating an IBAN Number

```csharp
using Validated.Primitives.ValueObjects;

// Create with automatic format detection
var (result, ibanNumber) = IbanNumber.TryCreate("DE89370400440532013000");

if (result.IsValid)
{
    Console.WriteLine(ibanNumber.Value);                    // "DE89370400440532013000"
    Console.WriteLine(ibanNumber.ToString());              // "DE89370400440532013000"
    Console.WriteLine(ibanNumber.ToFormattedString());     // "DE89 3704 0044 0532 0130 00"
    Console.WriteLine(ibanNumber.AccountType);             // Iban
    Console.WriteLine(ibanNumber.CountryCode);             // Germany
    Console.WriteLine(ibanNumber.GetIbanCountryCode());    // "DE"
    Console.WriteLine(ibanNumber.GetIbanCheckDigits());    // "89"
    Console.WriteLine(ibanNumber.GetBbanPart());           // "370400440532013000"
    Console.WriteLine(ibanNumber.Masked());                // "*****************13000"
    
    // Use the validated IBAN number
    ProcessIbanNumber(ibanNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Creating a BBAN Number

```csharp
// Create BBAN with country specification
var (result, bbanNumber) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

if (result.IsValid)
{
    Console.WriteLine(bbanNumber.AccountType);             // Bban
    Console.WriteLine(bbanNumber.CountryCode);             // UnitedKingdom
    Console.WriteLine(bbanNumber.IsBban);                  // true
    Console.WriteLine(bbanNumber.GetBbanPart());           // "12345678"
}
```

### Different Input Formats

```csharp
// All of these create the same validated IBAN
var formats = new[]
{
    "DE89370400440532013000",        // Plain format
    "DE89 3704 0044 0532 0130 00",   // With spaces
    "DE89-3704-0044-0532-0130-00"    // With dashes
};

foreach (var format in formats)
{
    var (result, iban) = IbanNumber.TryCreate(format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {iban.ToNormalizedString()}");
    }
}
```

---

## ?? Common Patterns

### IBAN/BBAN Creation Pattern

```csharp
var (result, accountNumber) = IbanNumber.TryCreate(input, countryCode);

if (result.IsValid)
{
    // Use the validated account number
    ProcessAccountNumber(accountNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Format Conversion Pattern

```csharp
var (result, ibanNumber) = IbanNumber.TryCreate("DE89370400440532013000");

if (result.IsValid)
{
    // Convert to different formats
    var normalized = ibanNumber.ToNormalizedString();     // "DE89370400440532013000"
    var formatted = ibanNumber.ToFormattedString();       // "DE89 3704 0044 0532 0130 00"
    var masked = ibanNumber.Masked();                     // "*****************13000"
    
    // Use appropriate format for different purposes
    var forStorage = normalized;
    var forDisplay = formatted;
    var forLogs = masked;
}
```

### Component Analysis Pattern

```csharp
var (result, accountNumber) = IbanNumber.TryCreate(accountInput);

if (result.IsValid)
{
    // Analyze account number components
    var isIban = accountNumber.IsIban;
    var country = accountNumber.CountryCode;
    var accountType = accountNumber.AccountType;
    
    // Business logic based on analysis
    if (isIban)
    {
        var ibanCountry = accountNumber.GetIbanCountryCode();
        var checkDigits = accountNumber.GetIbanCheckDigits();
        // Handle IBAN-specific logic
    }
    else
    {
        // Handle BBAN-specific logic
    }
}
```

---

## ?? Related Documentation

- [Banking README](banking_readme.md) - Complete banking validation including SWIFT and routing numbers
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated account number |
| `AccountType` | `BankAccountNumberType` | IBAN or BBAN |
| `CountryCode` | `CountryCode?` | Country code for the account |
| `IsIban` | `bool` | Whether this is IBAN format |
| `IsBban` | `bool` | Whether this is BBAN format |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, CountryCode? countryCode = null, string propertyName = "IbanNumber")` | `(ValidationResult, IbanNumber?)` | Static factory method to create validated account number |
| `ToString()` | `string` | Returns normalized account number |
| `ToNormalizedString()` | `string` | Returns normalized format without separators |
| `ToFormattedString()` | `string` | Returns formatted display format |
| `GetIbanCountryCode()` | `string?` | Returns IBAN country code (first 2 chars) |
| `GetIbanCheckDigits()` | `string?` | Returns IBAN check digits (chars 3-4) |
| `GetBbanPart()` | `string?` | Returns BBAN portion of the account number |
| `Masked()` | `string` | Returns masked version showing last 4 digits |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Account number cannot be null, empty, or whitespace |
| **Valid Format** | Must match IBAN or BBAN format requirements |
| **Valid IBAN Format** | For IBAN: must follow CC##BBBBBBBBBBBBBBB pattern |
| **Valid IBAN Checksum** | For IBAN: must pass mod-97 checksum validation |
| **Valid BBAN Format** | For BBAN: must match country-specific format |

---

## ?? International Banking Standards

### IBAN Structure (ISO 13616)

IBAN follows the format: `CC##BBBBBBBBBBBBBBB`

```
CC ## BBBBBBBBBBBBBBBB
?? ?? ?????????????????
Country  Check  BBAN
Code    Digits  Part
```

- **CC**: Country Code (2 letters, ISO 3166-1 alpha-2)
- **##**: Check Digits (2 digits, calculated)
- **BBBBBBBBBBBBBBBB**: BBAN (Basic Bank Account Number, up to 30 chars)

### IBAN Checksum Algorithm

IBAN uses mod-97 checksum validation:

1. Move first 4 characters to end
2. Convert letters to numbers (A=10, B=11, ..., Z=35)
3. Interpret as big integer
4. Check if divisible by 97

### Supported Countries (IBAN)

| Country | Code | IBAN Length | Example |
|---------|------|-------------|---------|
| Germany | DE | 22 | DE89 3704 0044 0532 0130 00 |
| United Kingdom | GB | 22 | GB82 WEST 1234 5698 7654 32 |
| France | FR | 27 | FR14 2004 1010 0505 0001 3M02 606 |
| Italy | IT | 27 | IT60 X054 2811 1010 0000 0123 456 |
| Spain | ES | 24 | ES91 2100 0418 4502 0005 1332 |
| Netherlands | NL | 18 | NL91 ABNA 0417 1643 00 |

### BBAN vs IBAN

| Type | Description | Example |
|------|-------------|---------|
| **IBAN** | International standard with country code and checksum | DE89370400440532013000 |
| **BBAN** | Domestic account number without IBAN structure | 12345678 (UK domestic) |

---

## ??? Security Considerations

### Account Number Validation

```csharp
// ? DO: Validate account numbers before use
var (result, accountNumber) = IbanNumber.TryCreate(input, countryCode);

if (!result.IsValid)
{
    return BadRequest("Invalid account number");
}

// Additional validation
if (accountNumber != null)
{
    // Business rule: validate country is supported
    var supportedCountries = new[] { CountryCode.Germany, CountryCode.UnitedKingdom };
    if (!supportedCountries.Contains(accountNumber.CountryCode))
    {
        return BadRequest("Country not supported for transactions");
    }
    
    // Business rule: check account type requirements
    if (accountNumber.IsIban && !RequiresIban(accountNumber.CountryCode))
    {
        return BadRequest("IBAN format required for this country");
    }
    
    // Business rule: validate against known fraudulent patterns
    if (IsSuspiciousAccountNumber(accountNumber.ToNormalizedString()))
    {
        return BadRequest("Account number flagged for security review");
    }
}

// ? DON'T: Trust user input without validation
var accountNumber = input.Replace(" ", "").Replace("-", "");  // Dangerous!
ProcessAccountNumber(accountNumber);
```

### Preventing Banking Fraud

```csharp
// ? DO: Implement additional fraud detection for account numbers
var (result, accountNumber) = IbanNumber.TryCreate(accountInput, countryCode);

if (result.IsValid && accountNumber != null)
{
    // Check for test account numbers
    if (accountNumber.ToNormalizedString().Contains("000000"))
    {
        return TransactionResult.Blocked("Test account number not allowed");
    }
    
    // Validate IBAN checksum strength
    if (accountNumber.IsIban)
    {
        var checkDigits = accountNumber.GetIbanCheckDigits();
        if (checkDigits == "00" || checkDigits == "99")
        {
            return TransactionResult.Suspicious("Suspicious check digits");
        }
    }
    
    // Check transaction velocity for this account
    var transactionCount = GetRecentTransactionCount(accountNumber.ToNormalizedString());
    if (transactionCount > GetMaxTransactionsPerAccount())
    {
        return TransactionResult.RateLimited("Too many transactions for this account");
    }
    
    // Validate account number format consistency
    if (!IsValidFormatForCountry(accountNumber))
    {
        return TransactionResult.Invalid("Account format doesn't match country");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize account number input before validation
public string SanitizeAccountNumberInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove common separators and convert to uppercase
    var sanitized = input.Replace(" ", "").Replace("-", "").ToUpperInvariant();
    
    // Ensure only valid IBAN characters (letters A-Z, digits 0-9)
    sanitized = new string(sanitized.Where(c => 
        char.IsLetterOrDigit(c)).ToArray());
    
    // Validate reasonable length (IBAN max 34 chars, BBAN varies)
    if (sanitized.Length < 8 || sanitized.Length > 34)
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeAccountNumberInput(userInput);
var (result, accountNumber) = IbanNumber.TryCreate(sanitized, countryCode);
```

### Logging Banking Data

```csharp
// ? DO: Log account numbers appropriately for compliance
public void LogBankingTransaction(IbanNumber accountNumber, string transactionId, decimal amount)
{
    // Log masked account number for privacy
    var maskedAccount = accountNumber.Masked();
    var country = accountNumber.CountryCode?.ToString() ?? "Unknown";
    
    _logger.LogInformation(
        "Banking transaction {TransactionId}: {Country} account {MaskedAccount} - Amount: {Amount:C}",
        transactionId,
        country,
        maskedAccount,
        amount
    );
}

// For audit trails (more detailed logging)
public void LogDetailedBankingTransaction(IbanNumber accountNumber, string transactionId)
{
    // Log full account number for audit purposes only
    _logger.LogInformation(
        "Audit: Transaction {TransactionId} with account {AccountNumber} ({AccountType})",
        transactionId,
        accountNumber.ToNormalizedString(),
        accountNumber.AccountType
    );
}

// ? DON'T: Log full account numbers in regular application logs
_logger.LogInformation($"Transaction with account: {fullAccountNumber}");  // Avoid
```
