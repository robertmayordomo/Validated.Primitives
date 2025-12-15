# Validated.Primitives.BankAccountNumber

A validated bank account number primitive that enforces country-specific format validation for international banking account numbers, ensuring valid domestic and international account formats when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`BankAccountNumber` is a validated value object that represents bank account numbers with country-specific validation. It supports both IBAN and domestic account number formats for various countries, providing comprehensive validation and formatting methods. Once created, a `BankAccountNumber` instance is guaranteed to be valid for its specified country.

### Key Features

- **Country-Specific Validation** - Validates formats for specific countries
- **IBAN Support** - Handles international bank account numbers
- **Format Flexibility** - Accepts numbers with or without separators
- **Component Extraction** - Extracts country codes and check digits for IBANs
- **Security Masking** - Provides masked display for privacy
- **Normalization** - Standardizes account number formatting
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating a Bank Account Number

```csharp
using Validated.Primitives.ValueObjects;

// Create with country-specific validation
var (result, accountNumber) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

if (result.IsValid)
{
    Console.WriteLine(accountNumber.Value);                    // "12345678"
    Console.WriteLine(accountNumber.ToString());              // "12345678"
    Console.WriteLine(accountNumber.ToNormalizedString());    // "12345678"
    Console.WriteLine(accountNumber.CountryCode);             // UnitedKingdom
    Console.WriteLine(accountNumber.GetCountryName());        // "United Kingdom"
    Console.WriteLine(accountNumber.IsIban);                  // false
    Console.WriteLine(accountNumber.Masked());                // "****5678"
    
    // Use the validated account number
    ProcessAccountNumber(accountNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Creating an IBAN Account Number

```csharp
// Create IBAN with country validation
var (result, ibanAccount) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

if (result.IsValid)
{
    Console.WriteLine(ibanAccount.IsIban);                    // true
    Console.WriteLine(ibanAccount.GetIbanCountryCode());     // "DE"
    Console.WriteLine(ibanAccount.GetIbanCheckDigits());     // "89"
    Console.WriteLine(ibanAccount.ToFormattedString());      // "DE89 3704 0044 0532 0130 00"
}
```

### Different Input Formats

```csharp
// All of these create the same validated account number
var formats = new[]
{
    "12345678",        // Plain digits
    "1234 5678",       // With spaces
    "1234-5678"        // With dashes
};

foreach (var format in formats)
{
    var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {account.ToNormalizedString()}");
    }
}
```

---

## ?? Common Patterns

### Account Number Creation Pattern

```csharp
var (result, accountNumber) = BankAccountNumber.TryCreate(countryCode, input);

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
var (result, accountNumber) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

if (result.IsValid)
{
    // Convert to different formats
    var normalized = accountNumber.ToNormalizedString();     // "DE89370400440532013000"
    var formatted = accountNumber.ToFormattedString();       // "DE89 3704 0044 0532 0130 00"
    var masked = accountNumber.Masked();                     // "*****************13000"
    
    // Use appropriate format for different purposes
    var forStorage = normalized;
    var forDisplay = formatted;
    var forLogs = masked;
}
```

### Country-Specific Validation Pattern

```csharp
var (result, accountNumber) = BankAccountNumber.TryCreate(countryCode, accountInput);

if (result.IsValid)
{
    // Validate country-specific requirements
    var isIbanRequired = RequiresIbanFormat(countryCode);
    var isIbanProvided = accountNumber.IsIban;
    
    if (isIbanRequired && !isIbanProvided)
    {
        // Handle IBAN requirement not met
    }
    else if (!isIbanRequired && isIbanProvided)
    {
        // Handle unexpected IBAN format
    }
    
    // Proceed with validated account
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
| `CountryCode` | `CountryCode` | The country code for validation |
| `IsIban` | `bool` | Whether this is IBAN format |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(CountryCode countryCode, string value, string propertyName = "BankAccountNumber")` | `(ValidationResult, BankAccountNumber?)` | Static factory method to create validated account number |
| `ToString()` | `string` | Returns account number as stored |
| `ToNormalizedString()` | `string` | Returns normalized format without separators |
| `ToFormattedString()` | `string` | Returns formatted display format |
| `GetIbanCountryCode()` | `string?` | Returns IBAN country code (first 2 chars) |
| `GetIbanCheckDigits()` | `string?` | Returns IBAN check digits (chars 3-4) |
| `GetCountryName()` | `string` | Returns display-friendly country name |
| `Masked()` | `string` | Returns masked version showing last 4 digits |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Account number cannot be null, empty, or whitespace |
| **Valid Format** | Must match general account number format requirements |
| **Country Format** | Must match country-specific format requirements |

---

## ?? International Account Formats

### Supported Country Formats

| Country | Format Type | Length | Example |
|---------|-------------|--------|---------|
| **United Kingdom** | Domestic | 8 digits | 12345678 |
| **United States** | Domestic | 8-17 digits | 123456789 |
| **Germany** | IBAN | 22 chars | DE89 3704 0044 0532 0130 00 |
| **France** | IBAN | 27 chars | FR14 2004 1010 0505 0001 3M02 606 |
| **Canada** | Domestic | 7-12 digits | 123456789 |
| **Australia** | Domestic | 6-10 digits | 123456789 |
| **Japan** | Domestic | 7 digits | 1234567 |

### IBAN Structure

For countries using IBAN format:

```
CC ## BBBBBBBBBBBBBBBB
?? ?? ?????????????????
Country  Check  Domestic
Code    Digits  Account
```

- **CC**: Country Code (2 letters)
- **##**: Check Digits (2 digits)
- **BBBBBBBBBBBBBBBB**: Domestic account number

### Domestic Formats

Country-specific domestic account formats:

- **UK**: 8-digit sort code + account number
- **US**: Routing number + account number (8-17 digits total)
- **Canada**: Transit number + institution number + account number
- **Australia**: BSB code + account number
- **Japan**: 7-digit account number

---

## ??? Security Considerations

### Account Number Validation

```csharp
// ? DO: Validate account numbers before use
var (result, accountNumber) = BankAccountNumber.TryCreate(countryCode, input);

if (!result.IsValid)
{
    return BadRequest("Invalid account number");
}

// Additional validation
if (accountNumber != null)
{
    // Business rule: validate country is supported
    var supportedCountries = new[] { CountryCode.UnitedStates, CountryCode.UnitedKingdom };
    if (!supportedCountries.Contains(accountNumber.CountryCode))
    {
        return BadRequest("Country not supported for transactions");
    }
    
    // Business rule: check format consistency
    var expectedFormat = GetExpectedFormat(accountNumber.CountryCode);
    if (!MatchesExpectedFormat(accountNumber, expectedFormat))
    {
        return BadRequest("Account format doesn't match country requirements");
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
var (result, accountNumber) = BankAccountNumber.TryCreate(countryCode, accountInput);

if (result.IsValid && accountNumber != null)
{
    // Check for test account numbers
    if (accountNumber.ToNormalizedString().Contains("000000"))
    {
        return TransactionResult.Blocked("Test account number not allowed");
    }
    
    // Validate account number length for country
    var expectedLength = GetExpectedLength(accountNumber.CountryCode);
    if (accountNumber.ToNormalizedString().Length != expectedLength)
    {
        return TransactionResult.Suspicious("Account length doesn't match country");
    }
    
    // Check transaction velocity for this account
    var transactionCount = GetRecentTransactionCount(accountNumber.ToNormalizedString());
    if (transactionCount > GetMaxTransactionsPerAccount())
    {
        return TransactionResult.RateLimited("Too many transactions for this account");
    }
    
    // Validate IBAN checksum if applicable
    if (accountNumber.IsIban && !ValidateIbanChecksum(accountNumber))
    {
        return TransactionResult.Invalid("IBAN checksum failed");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize account number input before validation
public string SanitizeAccountNumberInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove common separators
    var sanitized = input.Replace(" ", "").Replace("-", "");
    
    // For IBAN, convert to uppercase
    sanitized = sanitized.ToUpperInvariant();
    
    // Ensure only valid characters (letters A-Z for IBAN, digits for domestic)
    sanitized = new string(sanitized.Where(c => 
        char.IsLetterOrDigit(c)).ToArray());
    
    // Validate reasonable length
    if (sanitized.Length < 6 || sanitized.Length > 34)
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeAccountNumberInput(userInput);
var (result, accountNumber) = BankAccountNumber.TryCreate(countryCode, sanitized);
```

### Logging Banking Data

```csharp
// ? DO: Log account numbers appropriately for compliance
public void LogBankingTransaction(BankAccountNumber accountNumber, string transactionId, decimal amount)
{
    // Log masked account number for privacy
    var maskedAccount = accountNumber.Masked();
    var country = accountNumber.GetCountryName();
    
    _logger.LogInformation(
        "Banking transaction {TransactionId}: {Country} account {MaskedAccount} - Amount: {Amount:C}",
        transactionId,
        country,
        maskedAccount,
        amount
    );
}

// For audit trails (more detailed logging)
public void LogDetailedBankingTransaction(BankAccountNumber accountNumber, string transactionId)
{
    // Log full account number for audit purposes only
    _logger.LogInformation(
        "Audit: Transaction {TransactionId} with account {AccountNumber} ({Country})",
        transactionId,
        accountNumber.ToNormalizedString(),
        accountNumber.GetCountryName()
    );
}

// ? DON'T: Log full account numbers in regular application logs
_logger.LogInformation($"Transaction with account: {fullAccountNumber}");  // Avoid
```
