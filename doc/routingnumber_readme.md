# Validated.Primitives.RoutingNumber

A validated US ABA routing number primitive that enforces proper routing transit number format and checksum validation, ensuring valid US banking routing numbers when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`RoutingNumber` is a validated value object that represents a US ABA routing number (also known as routing transit number). It validates the 9-digit format, Federal Reserve routing symbol, and includes checksum validation using the ABA algorithm. The class provides component extraction and formatting methods. Once created, a `RoutingNumber` instance is guaranteed to be valid.

### Key Features

- **ABA Checksum Validation** - Validates using ABA routing number algorithm
- **Federal Reserve Symbol Validation** - Ensures valid Federal Reserve routing symbols
- **Component Extraction** - Extracts district, institution, and check digit
- **Format Flexibility** - Accepts numbers with or without separators
- **US Banking Standard** - Follows US banking industry standards
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating a Routing Number

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, routingNumber) = RoutingNumber.TryCreate("011000015");

if (result.IsValid)
{
    Console.WriteLine(routingNumber.Value);                    // "011000015"
    Console.WriteLine(routingNumber.ToString());              // "011000015"
    Console.WriteLine(routingNumber.ToFormattedString());     // "0110-0001-5"
    Console.WriteLine(routingNumber.FederalReserveSymbol);    // "0110"
    Console.WriteLine(routingNumber.InstitutionIdentifier);   // "0001"
    Console.WriteLine(routingNumber.CheckDigit);              // "5"
    Console.WriteLine(routingNumber.FederalReserveDistrict);  // 1
    
    // Use the validated routing number
    ProcessRoutingNumber(routingNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Different Input Formats

```csharp
// All of these create the same validated routing number
var formats = new[]
{
    "011000015",        // Plain digits
    "0110-0001-5",      // With dashes
    "0110 0001 5"       // With spaces
};

foreach (var format in formats)
{
    var (result, routing) = RoutingNumber.TryCreate(format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {routing.ToDigitsOnly()}");
    }
}
```

### Component Analysis

```csharp
var (result, routingNumber) = RoutingNumber.TryCreate("021000021");

if (result.IsValid)
{
    // Analyze routing number components
    var district = routingNumber.FederalReserveDistrict;     // 2 (New York)
    var institution = routingNumber.InstitutionIdentifier;   // "1000"
    var checkDigit = routingNumber.CheckDigit;               // "1"
    var fedSymbol = routingNumber.FederalReserveSymbol;      // "0210"
    
    // Use components for business logic
    var isNewYorkDistrict = district == 2;
    var formatted = routingNumber.ToFormattedString();       // "0210-0021-1"
}
```

---

## ?? Common Patterns

### Routing Number Creation Pattern

```csharp
var (result, routingNumber) = RoutingNumber.TryCreate(input);

if (result.IsValid)
{
    // Use the validated routing number
    ProcessRoutingNumber(routingNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Format Conversion Pattern

```csharp
var (result, routingNumber) = RoutingNumber.TryCreate("011000015");

if (result.IsValid)
{
    // Convert to different formats
    var digitsOnly = routingNumber.ToDigitsOnly();        // "011000015"
    var formatted = routingNumber.ToFormattedString();    // "0110-0001-5"
    var stored = routingNumber.Value;                     // Original format
    
    // Use appropriate format for different purposes
    var forApi = digitsOnly;
    var forDisplay = formatted;
}
```

### District Analysis Pattern

```csharp
var (result, routingNumber) = RoutingNumber.TryCreate(routingInput);

if (result.IsValid)
{
    // Analyze Federal Reserve district
    var district = routingNumber.FederalReserveDistrict;
    
    // Business logic based on district
    var districtName = GetFederalReserveDistrictName(district);
    var isHeadOffice = routingNumber.InstitutionIdentifier.EndsWith("000");
    
    // Use analysis results
    if (district.HasValue)
    {
        Console.WriteLine($"Bank is in {districtName} district");
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
| `Value` | `string` | The validated routing number |
| `FederalReserveSymbol` | `string` | Federal Reserve routing symbol (first 4 digits) |
| `InstitutionIdentifier` | `string` | ABA institution identifier (middle 4 digits) |
| `CheckDigit` | `string` | Check digit (last digit) |
| `FederalReserveDistrict` | `int?` | Federal Reserve district number (first 2 digits) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, string propertyName = "RoutingNumber")` | `(ValidationResult, RoutingNumber?)` | Static factory method to create validated routing number |
| `ToString()` | `string` | Returns routing number as stored |
| `ToDigitsOnly()` | `string` | Returns routing number without separators |
| `ToFormattedString()` | `string` | Returns formatted as XXXX-YYYY-C |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Routing number cannot be null, empty, or whitespace |
| **Valid Format** | Must contain only digits, spaces, or dashes |
| **Only Digits** | Must consist only of numeric digits |
| **Valid Length** | Must be exactly 9 digits |
| **Valid Federal Reserve Symbol** | First 4 digits must be valid Federal Reserve routing symbol |
| **Valid Checksum** | Must pass ABA checksum algorithm |

---

## ??? ABA Routing Number Standards

### Routing Number Structure

ABA routing numbers follow the format: `XXXXYYYYC`

```
XXXX YYYY C
???? ???? ?
Fed   ABA  Check
Symbol     Digit
```

- **XXXX**: Federal Reserve Routing Symbol (4 digits)
- **YYYY**: ABA Institution Identifier (4 digits)
- **C**: Check Digit (1 digit, calculated)

### Federal Reserve Districts

| District | Location | Routing Symbol Range |
|----------|----------|---------------------|
| 01 | Boston | 0101-0199 |
| 02 | New York | 0201-0299 |
| 03 | Philadelphia | 0301-0399 |
| 04 | Cleveland | 0401-0499 |
| 05 | Richmond | 0501-0599 |
| 06 | Atlanta | 0601-0699 |
| 07 | Chicago | 0701-0799 |
| 08 | St. Louis | 0801-0899 |
| 09 | Minneapolis | 0901-0999 |
| 10 | Kansas City | 1001-1099 |
| 11 | Dallas | 1101-1199 |
| 12 | San Francisco | 1201-1299 |

### Checksum Algorithm

The ABA routing number checksum uses a weighted algorithm:

```
3(d1 + d4 + d7) + 7(d2 + d5 + d8) + (d3 + d6 + d9) ? 0 (mod 10)
```

Where d1-d9 are the 9 digits of the routing number.

### Common Routing Numbers

| Bank | Routing Number | District |
|------|----------------|----------|
| Chase | 021000021 | New York (02) |
| Bank of America | 121000358 | San Francisco (12) |
| Wells Fargo | 121000248 | San Francisco (12) |
| Citibank | 021000089 | New York (02) |

---

## ??? Security Considerations

### Routing Number Validation

```csharp
// ? DO: Validate routing numbers before use
var (result, routingNumber) = RoutingNumber.TryCreate(input);

if (!result.IsValid)
{
    return BadRequest("Invalid routing number");
}

// Additional validation
if (routingNumber != null)
{
    // Business rule: validate district is supported
    var supportedDistricts = new[] { 1, 2, 12 }; // Boston, New York, San Francisco
    if (routingNumber.FederalReserveDistrict.HasValue &&
        !supportedDistricts.Contains(routingNumber.FederalReserveDistrict.Value))
    {
        return BadRequest("Bank district not supported");
    }
    
    // Business rule: check against known fraudulent routing numbers
    if (IsKnownFraudulentRoutingNumber(routingNumber.ToDigitsOnly()))
    {
        return BadRequest("Routing number flagged for security review");
    }
    
    // Business rule: validate institution identifier format
    if (!IsValidInstitutionIdentifier(routingNumber.InstitutionIdentifier))
    {
        return BadRequest("Invalid institution identifier");
    }
}

// ? DON'T: Trust user input without validation
var routingNumber = input.Replace("-", "").Replace(" ", "");  // Dangerous!
ProcessRoutingNumber(routingNumber);
```

### Preventing Banking Fraud

```csharp
// ? DO: Implement additional fraud detection for routing numbers
var (result, routingNumber) = RoutingNumber.TryCreate(routingInput);

if (result.IsValid && routingNumber != null)
{
    // Check for test routing numbers
    if (routingNumber.ToDigitsOnly().StartsWith("9999"))
    {
        return TransactionResult.Blocked("Test routing number not allowed");
    }
    
    // Validate against account number for consistency
    var accountNumber = GetAccountNumber();
    if (!IsValidAccountForRouting(routingNumber, accountNumber))
    {
        return TransactionResult.Suspicious("Account number doesn't match routing number");
    }
    
    // Check transaction velocity for this routing number
    var transactionCount = GetRecentTransactionCount(routingNumber.ToDigitsOnly());
    if (transactionCount > GetMaxTransactionsPerHour())
    {
        return TransactionResult.RateLimited("Too many transactions for this routing number");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize routing number input before validation
public string SanitizeRoutingNumberInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove common separators
    var sanitized = input.Replace("-", "").Replace(" ", "");
    
    // Ensure only digits
    sanitized = new string(sanitized.Where(char.IsDigit).ToArray());
    
    // Validate length
    if (sanitized.Length != 9)
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeRoutingNumberInput(userInput);
var (result, routingNumber) = RoutingNumber.TryCreate(sanitized);
```

### Logging Banking Data

```csharp
// ? DO: Log routing numbers appropriately for compliance
public void LogBankingTransaction(RoutingNumber routingNumber, string transactionId, decimal amount)
{
    // Log district and partial routing number for privacy
    var district = routingNumber.FederalReserveDistrict;
    var partialRouting = $"****{routingNumber.ToDigitsOnly()[4..]}";
    
    _logger.LogInformation(
        "Banking transaction {TransactionId}: District {District}, Routing ****{Partial} - Amount: {Amount:C}",
        transactionId,
        district,
        partialRouting,
        amount
    );
}

// For audit trails (more detailed logging)
public void LogDetailedBankingTransaction(RoutingNumber routingNumber, string transactionId)
{
    // Log full routing number for audit purposes only
    _logger.LogInformation(
        "Audit: Transaction {TransactionId} with routing {RoutingNumber}",
        transactionId,
        routingNumber.ToDigitsOnly()
    );
}

// ? DON'T: Log full routing numbers in regular application logs
_logger.LogInformation($"Transaction with routing: {fullRoutingNumber}");  // Avoid
```
