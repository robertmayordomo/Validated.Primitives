# Validated.Primitives.SwiftCode

A validated SWIFT code (BIC) primitive that enforces ISO 9362 standard format validation for international banking codes, ensuring valid SWIFT codes when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`SwiftCode` is a validated value object that represents a SWIFT code (also known as BIC - Bank Identifier Code) according to the ISO 9362 standard. It supports both BIC8 (8-character) and BIC11 (11-character) formats, validates structure and country codes, and provides comprehensive parsing and formatting methods. Once created, a `SwiftCode` instance is guaranteed to be valid.

### Key Features

- **ISO 9362 Compliance** - Validates according to international banking standards
- **BIC8/BIC11 Support** - Handles both 8-character and 11-character formats
- **Country Validation** - Validates ISO 3166-1 alpha-2 country codes
- **Component Parsing** - Extracts institution, country, location, and branch codes
- **Format Conversion** - Convert between different SWIFT code formats
- **Test Code Detection** - Identifies test SWIFT codes
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating a SWIFT Code

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, swiftCode) = SwiftCode.TryCreate("DEUTDEFF");

if (result.IsValid)
{
    Console.WriteLine(swiftCode.Value);                    // "DEUTDEFF"
    Console.WriteLine(swiftCode.ToString());              // "DEUTDEFF"
    Console.WriteLine(swiftCode.InstitutionCode);         // "DEUT"
    Console.WriteLine(swiftCode.CountryCode);             // "DE"
    Console.WriteLine(swiftCode.LocationCode);            // "FF"
    Console.WriteLine(swiftCode.BranchCode);              // "XXX"
    Console.WriteLine(swiftCode.IsPrimaryOffice);         // true
    Console.WriteLine(swiftCode.IsBic8);                  // true
    
    // Use the validated SWIFT code
    ProcessSwiftCode(swiftCode);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Different Formats

```csharp
// BIC8 format (primary office)
var (bic8Result, bic8Code) = SwiftCode.TryCreate("DEUTDEFF");

// BIC11 format (with branch)
var (bic11Result, bic11Code) = SwiftCode.TryCreate("DEUTDEFF500");

// Both represent the same institution but different branches
if (bic8Result.IsValid && bic11Result.IsValid)
{
    Console.WriteLine(bic8Code.IsPrimaryOffice);   // true
    Console.WriteLine(bic11Code.IsPrimaryOffice);  // false
    Console.WriteLine(bic8Code.Equals(bic11Code)); // false (different branches)
}
```

### Component Extraction

```csharp
var (result, swiftCode) = SwiftCode.TryCreate("CHASUS33");

if (result.IsValid)
{
    // Extract components
    var institution = swiftCode.InstitutionCode;  // "CHAS" (Chase Bank)
    var country = swiftCode.CountryCode;          // "US" (United States)
    var location = swiftCode.LocationCode;        // "33" (New York)
    var branch = swiftCode.BranchCode;            // "XXX" (Primary office)
    
    // Use components for business logic
    var isUSBank = country == "US";
    var isPrimaryOffice = swiftCode.IsPrimaryOffice;
}
```

---

## ?? Common Patterns

### SWIFT Code Creation Pattern

```csharp
var (result, swiftCode) = SwiftCode.TryCreate(input, allowTestCodes: false);

if (result.IsValid)
{
    // Use the validated SWIFT code
    ProcessSwiftCode(swiftCode);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Format Conversion Pattern

```csharp
var (result, swiftCode) = SwiftCode.TryCreate("DEUTDEFF");

if (result.IsValid)
{
    // Convert to different formats
    var normalized = swiftCode.ToNormalizedString();  // "DEUTDEFF"
    var fullFormat = swiftCode.ToFullFormat();        // "DEUTDEFFXXX"
    
    // Use appropriate format for different systems
    var forDisplay = normalized;
    var forStorage = fullFormat;
}
```

### Component Analysis Pattern

```csharp
var (result, swiftCode) = SwiftCode.TryCreate(swiftInput);

if (result.IsValid)
{
    // Analyze SWIFT code components
    var country = swiftCode.CountryCode;
    var isPrimaryOffice = swiftCode.IsPrimaryOffice;
    var isTestCode = swiftCode.IsTestCode;
    
    // Business logic based on analysis
    if (isTestCode)
    {
        // Handle test environment
    }
    else if (isPrimaryOffice)
    {
        // Handle primary office transactions
    }
    else
    {
        // Handle branch office transactions
    }
}
```

---

## ?? Related Documentation

- [Banking README](banking_readme.md) - Complete banking validation including IBAN and routing numbers
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated SWIFT code |
| `InstitutionCode` | `string` | Bank/institution code (first 4 characters) |
| `BankCode` | `string` | Alias for InstitutionCode |
| `CountryCode` | `string` | ISO 3166-1 alpha-2 country code (characters 5-6) |
| `LocationCode` | `string` | Location/city code (characters 7-8) |
| `BranchCode` | `string` | Branch code (characters 9-11, or "XXX" for primary) |
| `IsPrimaryOffice` | `bool` | Whether this represents a primary/head office |
| `IsTestCode` | `bool` | Whether this is a test SWIFT code |
| `IsBic8` | `bool` | Whether this is BIC8 format (8 characters) |
| `IsBic11` | `bool` | Whether this is BIC11 format (11 characters) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, bool allowTestCodes = false, string propertyName = "SwiftCode")` | `(ValidationResult, SwiftCode?)` | Static factory method to create validated SWIFT code |
| `ToString()` | `string` | Returns normalized uppercase SWIFT code |
| `ToNormalizedString()` | `string` | Returns normalized uppercase format |
| `ToFullFormat()` | `string` | Returns BIC11 format (with branch code) |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | SWIFT code cannot be null, empty, or whitespace |
| **Valid Format** | Must match ISO 9362 format (letters A-Z, digits 0-9) |
| **Valid Length** | Must be 8 or 11 characters |
| **Valid Structure** | Must follow AAAA BB CC (XXX) pattern |
| **Valid Country Code** | Country code must be valid ISO 3166-1 alpha-2 |
| **Not Test Code** | Cannot be test SWIFT code (unless allowed) |

---

## ?? SWIFT Code Standards

### ISO 9362 Format Structure

SWIFT codes follow the ISO 9362 standard:

```
AAAA BB CC (XXX)
???? ??? ?? ????
Institution  ?  Branch
Code         ?
             ?
          Location
          Code
       ???????
    Country
     Code
```

- **AAAA**: Institution Code (4 letters) - Bank identifier
- **BB**: Country Code (2 letters) - ISO 3166-1 alpha-2
- **CC**: Location Code (2 alphanumeric) - City/branch location
- **XXX**: Branch Code (3 alphanumeric, optional) - Specific branch

### BIC8 vs BIC11

| Format | Length | Description | Example |
|--------|--------|-------------|---------|
| **BIC8** | 8 chars | Primary office/head office | `DEUTDEFF` |
| **BIC11** | 11 chars | Specific branch | `DEUTDEFF500` |

### Special Branch Codes

| Branch Code | Meaning |
|-------------|---------|
| **XXX** | Primary office/head office |
| **001-999** | Specific branch numbers |
| **AAA-ZZZ** | Branch identifiers |

### Test SWIFT Codes

ISO 9362 defines test BICs where the location code's second character is '0':

- `TESTGB00` - Test code for United Kingdom
- `TESTUS00` - Test code for United States
- `TESTDE00` - Test code for Germany

---

## ??? Security Considerations

### SWIFT Code Validation

```csharp
// ? DO: Validate SWIFT codes before use
var (result, swiftCode) = SwiftCode.TryCreate(input, allowTestCodes: false);

if (!result.IsValid)
{
    return BadRequest("Invalid SWIFT code");
}

// Additional validation
if (swiftCode != null)
{
    // Business rule: validate country is supported
    var supportedCountries = new[] { "US", "GB", "DE", "FR" };
    if (!supportedCountries.Contains(swiftCode.CountryCode))
    {
        return BadRequest("Country not supported for transactions");
    }
    
    // Business rule: reject test codes in production
    if (swiftCode.IsTestCode)
    {
        return BadRequest("Test SWIFT codes not allowed");
    }
    
    // Business rule: validate against known bank database
    if (!IsKnownBank(swiftCode.InstitutionCode, swiftCode.CountryCode))
    {
        return BadRequest("Unknown or inactive bank code");
    }
}

// ? DON'T: Trust user input without validation
var swiftCode = input.ToUpper().Trim();  // Dangerous!
ProcessSwiftCode(swiftCode);
```

### Preventing Fraudulent Transactions

```csharp
// ? DO: Implement additional fraud detection for SWIFT codes
var (result, swiftCode) = SwiftCode.TryCreate(swiftInput);

if (result.IsValid && swiftCode != null)
{
    // Check for suspicious patterns
    if (IsHighRiskCountry(swiftCode.CountryCode))
    {
        // Additional verification required
        _logger.LogWarning("Transaction to high-risk country: {Country}", swiftCode.CountryCode);
    }
    
    // Validate against transaction amount
    var transactionAmount = GetTransactionAmount();
    if (RequiresEnhancedDueDiligence(swiftCode, transactionAmount))
    {
        // Enhanced verification for large transactions
        return TransactionResult.RequiresEnhancedVerification();
    }
    
    // Check for sanctioned entities
    if (IsSanctionedEntity(swiftCode.InstitutionCode, swiftCode.CountryCode))
    {
        return TransactionResult.Blocked("Sanctioned entity");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize SWIFT code input before validation
public string SanitizeSwiftCodeInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove whitespace and convert to uppercase
    var sanitized = input.Replace(" ", "").Replace("-", "").ToUpperInvariant();
    
    // Ensure only valid SWIFT characters (letters A-Z, digits 0-9)
    sanitized = new string(sanitized.Where(c => 
        char.IsLetterOrDigit(c)).ToArray());
    
    // Validate length constraints
    if (sanitized.Length != 8 && sanitized.Length != 11)
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeSwiftCodeInput(userInput);
var (result, swiftCode) = SwiftCode.TryCreate(sanitized);
```

### Logging Banking Data

```csharp
// ? DO: Log SWIFT codes appropriately for compliance
public void LogBankingTransaction(SwiftCode swiftCode, string transactionId, decimal amount)
{
    // Log country and institution without full SWIFT code for privacy
    _logger.LogInformation(
        "Banking transaction {TransactionId}: {Institution} ({Country}) - Amount: {Amount:C}",
        transactionId,
        swiftCode.InstitutionCode,
        swiftCode.CountryCode,
        amount
    );
}

// For audit trails (more detailed logging)
public void LogDetailedBankingTransaction(SwiftCode swiftCode, string transactionId)
{
    // Log full SWIFT code for audit purposes only
    _logger.LogInformation(
        "Audit: Transaction {TransactionId} with SWIFT {SwiftCode}",
        transactionId,
        swiftCode.ToString()
    );
}

// ? DON'T: Log full SWIFT codes in regular application logs
_logger.LogInformation($"Transaction with SWIFT: {fullSwiftCode}");  // Avoid
```
