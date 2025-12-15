# Validated.Primitives.SortCode

A validated bank sort code primitive that enforces UK and Ireland sort code format validation, ensuring valid branch identification codes when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`SortCode` is a validated value object that represents UK and Ireland bank sort codes. It validates the 6-digit format used to identify bank branches in these countries, providing formatting and normalization methods. Once created, a `SortCode` instance is guaranteed to be valid for its specified country.

### Key Features

- **UK/Ireland Validation** - Validates 6-digit sort code format
- **Format Flexibility** - Accepts codes with or without separators
- **Country-Specific** - Tailored validation for UK and Ireland banking
- **Standard Formatting** - Converts to XX-XX-XX display format
- **Normalization** - Standardizes sort code formatting
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating a Sort Code

```csharp
using Validated.Primitives.ValueObjects;

// Create with country validation
var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

if (result.IsValid)
{
    Console.WriteLine(sortCode.Value);                    // "12-34-56"
    Console.WriteLine(sortCode.ToString());              // "12-34-56"
    Console.WriteLine(sortCode.ToDigitsOnly());          // "123456"
    Console.WriteLine(sortCode.ToFormattedString());     // "12-34-56"
    Console.WriteLine(sortCode.CountryCode);             // UnitedKingdom
    Console.WriteLine(sortCode.GetCountryName());        // "United Kingdom"
    
    // Use the validated sort code
    ProcessSortCode(sortCode);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Different Input Formats

```csharp
// All of these create the same validated sort code
var formats = new[]
{
    "123456",        // Plain digits
    "12-34-56",      // With dashes
    "12 34 56"       // With spaces
};

foreach (var format in formats)
{
    var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {sortCode.ToDigitsOnly()}");
    }
}
```

### Country-Specific Usage

```csharp
// UK sort code
var (ukResult, ukSortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");

// Ireland sort code (same format)
var (ieResult, ieSortCode) = SortCode.TryCreate(CountryCode.Ireland, "654321");

if (ukResult.IsValid && ieResult.IsValid)
{
    Console.WriteLine($"UK: {ukSortCode.ToFormattedString()}");
    Console.WriteLine($"IE: {ieSortCode.ToFormattedString()}");
}
```

---

## ?? Common Patterns

### Sort Code Creation Pattern

```csharp
var (result, sortCode) = SortCode.TryCreate(countryCode, input);

if (result.IsValid)
{
    // Use the validated sort code
    ProcessSortCode(sortCode);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Format Conversion Pattern

```csharp
var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");

if (result.IsValid)
{
    // Convert to different formats
    var digitsOnly = sortCode.ToDigitsOnly();        // "123456"
    var formatted = sortCode.ToFormattedString();    // "12-34-56"
    var stored = sortCode.Value;                     // Original format
    
    // Use appropriate format for different purposes
    var forApi = digitsOnly;
    var forDisplay = formatted;
}
```

### Banking Integration Pattern

```csharp
var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, sortCodeInput);

if (result.IsValid)
{
    // Combine with account number for full banking reference
    var accountNumber = GetAccountNumber();
    var fullReference = $"{sortCode.ToDigitsOnly()}{accountNumber}";
    
    // Validate against banking system requirements
    if (IsValidBankingReference(sortCode, accountNumber))
    {
        // Proceed with banking operation
    }
}
```

---

## ?? Related Documentation

- [Banking README](banking_readme.md) - Complete banking validation including SWIFT and IBAN
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated sort code |
| `CountryCode` | `CountryCode` | The country code for validation |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(CountryCode countryCode, string value, string propertyName = "SortCode")` | `(ValidationResult, SortCode?)` | Static factory method to create validated sort code |
| `ToString()` | `string` | Returns sort code as stored |
| `ToDigitsOnly()` | `string` | Returns sort code without separators |
| `ToFormattedString()` | `string` | Returns formatted as XX-XX-XX |
| `GetCountryName()` | `string` | Returns display-friendly country name |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Sort code cannot be null, empty, or whitespace |
| **Valid Format** | Must match general sort code format requirements |
| **Only Digits** | Must consist only of numeric digits (UK/Ireland) |
| **Country Format** | Must match country-specific format requirements |

---

## ???? UK/Ireland Banking Standards

### Sort Code Structure

UK and Ireland sort codes follow a 6-digit format:

```
XX-XX-XX
????????????
Bank  ?  Branch
Code  ?  Code
      ?
    Location
     Code
```

- **XX-XX-XX**: 6 digits identifying bank and branch
- **First 2 digits**: Bank/institution identifier
- **Middle 2 digits**: Regional location code
- **Last 2 digits**: Specific branch identifier

### Common Sort Code Ranges

| Bank | Sort Code Range | Example |
|------|----------------|---------|
| **Barclays** | 20-00-00 to 20-99-99 | 20-34-56 |
| **HSBC** | 40-00-00 to 40-99-99 | 40-12-34 |
| **Lloyds** | 30-00-00 to 30-99-99 | 30-78-90 |
| **NatWest** | 60-00-00 to 60-99-99 | 60-12-34 |
| **Santander** | 09-00-00 to 09-99-99 | 09-01-23 |

### Display Formats

Sort codes are commonly displayed as:

- **XX-XX-XX**: Standard UK format (12-34-56)
- **XXXXXX**: Digits only for electronic processing
- **12 34 56**: With spaces (less common)

### Banking Integration

Sort codes are used with account numbers:

- **Sort Code**: 6 digits (XX-XX-XX)
- **Account Number**: 8 digits
- **Full Reference**: Sort code + account number

---

## ??? Security Considerations

### Sort Code Validation

```csharp
// ? DO: Validate sort codes before use
var (result, sortCode) = SortCode.TryCreate(countryCode, input);

if (!result.IsValid)
{
    return BadRequest("Invalid sort code");
}

// Additional validation
if (sortCode != null)
{
    // Business rule: validate country is supported
    var supportedCountries = new[] { CountryCode.UnitedKingdom, CountryCode.Ireland };
    if (!supportedCountries.Contains(sortCode.CountryCode))
    {
        return BadRequest("Sort codes only supported for UK and Ireland");
    }
    
    // Business rule: check for test sort codes
    if (sortCode.ToDigitsOnly().StartsWith("99"))
    {
        return BadRequest("Test sort codes not allowed");
    }
    
    // Business rule: validate against known bank ranges
    if (!IsValidBankRange(sortCode))
    {
        return BadRequest("Sort code not in valid bank range");
    }
}

// ? DON'T: Trust user input without validation
var sortCode = input.Replace("-", "").Replace(" ", "");  // Dangerous!
ProcessSortCode(sortCode);
```

### Preventing Banking Fraud

```csharp
// ? DO: Implement additional fraud detection for sort codes
var (result, sortCode) = SortCode.TryCreate(countryCode, sortCodeInput);

if (result.IsValid && sortCode != null)
{
    // Check for sequential or repetitive digits
    if (IsSequentialDigits(sortCode.ToDigitsOnly()))
    {
        return TransactionResult.Suspicious("Suspicious sort code pattern");
    }
    
    // Validate sort code and account number combination
    var accountNumber = GetAccountNumber();
    if (!IsValidSortCodeAccountCombination(sortCode, accountNumber))
    {
        return TransactionResult.Invalid("Sort code and account number combination invalid");
    }
    
    // Check transaction velocity for this sort code
    var transactionCount = GetRecentTransactionCount(sortCode.ToDigitsOnly());
    if (transactionCount > GetMaxTransactionsPerSortCode())
    {
        return TransactionResult.RateLimited("Too many transactions for this sort code");
    }
    
    // Validate against sanctioned banks
    if (IsSanctionedBank(sortCode))
    {
        return TransactionResult.Blocked("Bank is sanctioned");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize sort code input before validation
public string SanitizeSortCodeInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove common separators
    var sanitized = input.Replace("-", "").Replace(" ", "");
    
    // Ensure only digits
    sanitized = new string(sanitized.Where(char.IsDigit).ToArray());
    
    // Validate length for UK/Ireland sort codes
    if (sanitized.Length != 6)
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeSortCodeInput(userInput);
var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, sanitized);
```

### Logging Banking Data

```csharp
// ? DO: Log sort codes appropriately for compliance
public void LogBankingTransaction(SortCode sortCode, string transactionId, decimal amount)
{
    // Log sort code for banking transactions (less sensitive than account numbers)
    _logger.LogInformation(
        "Banking transaction {TransactionId}: Sort code {SortCode} - Amount: {Amount:C}",
        transactionId,
        sortCode.ToFormattedString(),
        amount
    );
}

// For detailed audit trails
public void LogDetailedBankingTransaction(SortCode sortCode, string accountLastFour, string transactionId)
{
    // Log full banking reference for audit purposes
    _logger.LogInformation(
        "Audit: Transaction {TransactionId} with sort code {SortCode} and account ****{LastFour}",
        transactionId,
        sortCode.ToDigitsOnly(),
        accountLastFour
    );
}

// For fraud monitoring
public void LogFraudCheck(SortCode sortCode, string riskLevel)
{
    // Log sort code patterns for fraud analysis
    _logger.LogWarning(
        "Fraud check: Sort code {SortCode} flagged as {RiskLevel}",
        sortCode.ToDigitsOnly(),
        riskLevel
    );
}
```
