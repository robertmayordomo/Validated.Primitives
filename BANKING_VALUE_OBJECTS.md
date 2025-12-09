# Banking Value Objects

This library provides comprehensive validated value objects for banking and financial operations, supporting international standards for bank identification and account numbers.

## Table of Contents
- [Overview](#overview)
- [Supported Banking Fields](#supported-banking-fields)
- [SWIFT Code (BIC)](#swift-code-bic)
- [Routing Number (ABA)](#routing-number-aba)
- [Bank Account Number](#bank-account-number)
- [Sort Code](#sort-code)
- [Usage Examples](#usage-examples)
- [International Wire Transfer Example](#international-wire-transfer-example)

## Overview

Banking value objects provide validated, type-safe representations of banking identifiers used worldwide. Each value object follows international standards and includes:

? **Format Validation** - Ensures correct structure and format  
? **Checksum Validation** - Where applicable (ABA routing numbers, IBANs)  
? **Country-Specific Rules** - Validates against country-specific formats  
? **Component Access** - Extract individual parts (bank code, country code, etc.)  
? **Multiple Input Formats** - Accepts various formatting styles  
? **JSON Serialization** - Full JSON support for APIs  

## Supported Banking Fields

| Value Object | Standard | Countries | Length | Purpose |
|-------------|----------|-----------|--------|---------|
| `SwiftCode` | ISO 9362 | Global | 8 or 11 | International bank identification |
| `RoutingNumber` | ABA | USA | 9 digits | US domestic bank routing |
| `IbanNumber` | ISO 13616 | 70+ countries | Variable (15-34) | International/domestic account number with auto-detection |
| `BankAccountNumber` | IBAN + Local | 20+ countries | Variable | Bank account identification (legacy, use IbanNumber) |
| `SortCode` | UK/Ireland | UK, Ireland | 6 digits | UK/Ireland branch identification |

## SWIFT Code (BIC)

**Standard:** ISO 9362 (Business Identifier Code)  
**Format:** `AAAABBCCXXX`  
**Length:** 8 (BIC8) or 11 (BIC11) characters

### Structure
- **AAAA** - Institution code (4 letters) - identifies the bank
- **BB** - Country code (2 letters, ISO 3166-1 alpha-2)
- **CC** - Location code (2 alphanumeric) - city/location
- **XXX** - Branch code (3 alphanumeric, optional) - 'XXX' = primary office

### Examples

```csharp
using Validated.Primitives.ValueObjects;

// Deutsche Bank, Frankfurt (primary office)
var (result, swift) = SwiftCode.TryCreate("DEUTDEFF");

// Same bank with explicit branch code
var (result2, swift2) = SwiftCode.TryCreate("DEUTDEFFXXX");

// They are equivalent
swift.Equals(swift2); // true

// Access components
Console.WriteLine(swift.InstitutionCode);  // "DEUT"
Console.WriteLine(swift.BankCode);         // "DEUT" (alias)
Console.WriteLine(swift.CountryCode);      // "DE"
Console.WriteLine(swift.LocationCode);     // "FF"
Console.WriteLine(swift.BranchCode);       // "XXX"

// Properties
Console.WriteLine(swift.IsPrimaryOffice);  // true
Console.WriteLine(swift.IsBic8);           // true
Console.WriteLine(swift.IsBic11);          // false (only after ToFullFormat)
Console.WriteLine(swift.IsTestCode);       // false

// Formatting
Console.WriteLine(swift.ToNormalizedString()); // "DEUTDEFF"
Console.WriteLine(swift.ToFullFormat());       // "DEUTDEFFXXX"
```

### Test Codes

ISO 9362 defines test BICs where the location code's second character is '0':

```csharp
// This will fail by default (test code)
var (result, swift) = SwiftCode.TryCreate("DEUTD0FF");
// result.IsValid = false, error code "TestCode"

// Allow test codes for testing environments
var (result2, swift2) = SwiftCode.TryCreate("DEUTD0FF", allowTestCodes: true);
// result2.IsValid = true
Console.WriteLine(swift2.IsTestCode); // true
```

### Real-World Examples

```csharp
// Major international banks
var deutscheBank = SwiftCode.TryCreate("DEUTDEFF");      // Deutsche Bank, Germany
var chase = SwiftCode.TryCreate("CHASUS33");             // Chase Bank, USA
var hsbc = SwiftCode.TryCreate("HSBCHKHH");              // HSBC, Hong Kong
var bnpParibas = SwiftCode.TryCreate("BNPAFRPP");        // BNP Paribas, France
var barclays = SwiftCode.TryCreate("BARCGB22");          // Barclays, UK
var nab = SwiftCode.TryCreate("NATAAU3303M");            // NAB, Melbourne branch
```

## Routing Number (ABA)

**Standard:** ABA Routing Transit Number  
**Format:** `XXXXXXXXX`  
**Length:** 9 digits  
**Country:** United States only

### Structure
- **XXXX** - Federal Reserve routing symbol (first 4 digits)
- **YYYY** - ABA Institution Identifier (middle 4 digits)
- **C** - Check digit (last digit, validated with mod-10 algorithm)

### Validation
- **Checksum:** 3×(d1+d4+d7) + 7×(d2+d5+d8) + (d3+d6+d9) mod 10 = 0
- **Federal Reserve Symbol:** First 2 digits must be in valid ranges (00-12, 21-32, 61-72, or 80)

### Examples

```csharp
using Validated.Primitives.ValueObjects;

// Bank of America, New York
var (result, routing) = RoutingNumber.TryCreate("021000021");

// Accepts formatted input
var (result2, routing2) = RoutingNumber.TryCreate("0210-0002-1");

// Access components
Console.WriteLine(routing.FederalReserveSymbol);    // "0210"
Console.WriteLine(routing.InstitutionIdentifier);   // "0002"
Console.WriteLine(routing.CheckDigit);              // "1"
Console.WriteLine(routing.FederalReserveDistrict); // 2 (Boston Fed)

// Formatting
Console.WriteLine(routing.ToDigitsOnly());          // "021000021"
Console.WriteLine(routing.ToFormattedString());     // "0210-0002-1"
```

### Real-World Examples

```csharp
// Major US banks
var bofa = RoutingNumber.TryCreate("021000021");     // Bank of America, NY
var wellsFargo = RoutingNumber.TryCreate("121000248"); // Wells Fargo, TX
var chase = RoutingNumber.TryCreate("111000025");    // Chase, NY
var citibank = RoutingNumber.TryCreate("021200025"); // Citibank, NY
```

## Bank Account Number

**Standards:** IBAN (Europe), Country-specific formats  
**Format:** Variable by country  
**Supported Countries:** 20+ including all major economies

### Supported Formats

#### IBAN Countries (Europe)
- Germany (DE) - 22 characters
- France (FR) - 27 characters
- UK (GB) - Uses local format, not IBAN
- Netherlands (NL) - 18 characters
- And 15+ more European countries

#### Non-IBAN Countries
- United States - 4-17 digits
- United Kingdom - 8 digits
- Australia - 6-9 digits
- Canada - 7-12 digits
- Japan - 7 digits
- India - 9-18 digits
- China - 16-19 digits

### Examples

```csharp
using Validated.Primitives.ValueObjects;

// UK account number
var (result, account) = BankAccountNumber.TryCreate(
    CountryCode.UnitedKingdom, 
    "12345678"
);

// German IBAN (with checksum validation)
var (result2, account2) = BankAccountNumber.TryCreate(
    CountryCode.Germany,
    "DE89370400440532013000"
);

// Accepts formatted IBAN
var (result3, account3) = BankAccountNumber.TryCreate(
    CountryCode.Germany,
    "DE89 3704 0044 0532 0130 00"
);

// Properties
Console.WriteLine(account2.IsIban);              // true
Console.WriteLine(account2.GetIbanCountryCode()); // "DE"
Console.WriteLine(account2.GetIbanCheckDigits()); // "89"

// Formatting
Console.WriteLine(account2.ToNormalizedString());  // "DE89370400440532013000"
Console.WriteLine(account2.ToFormattedString());   // "DE89 3704 0044 0532 0130 00"
Console.WriteLine(account2.Masked());              // "******************3000"
```

### IBAN Validation

The library validates IBAN checksums using the mod-97 algorithm:

```csharp
// Valid IBAN (checksum correct)
var valid = BankAccountNumber.TryCreate(
    CountryCode.Germany,
    "DE89370400440532013000"
);
// valid.Result.IsValid = true

// Invalid IBAN (checksum incorrect)
var invalid = BankAccountNumber.TryCreate(
    CountryCode.Germany,
    "DE00370400440532013000"  // Wrong checksum
);
// invalid.Result.IsValid = false
// Error: "IBAN checksum is invalid"
```

## Sort Code

**Standard:** UK/Ireland banking  
**Format:** `XX-XX-XX`  
**Length:** 6 digits  
**Countries:** United Kingdom, Ireland

### Structure
- 6 digits identifying the bank branch
- Often displayed as XX-XX-XX format

### Examples

```csharp
using Validated.Primitives.ValueObjects;

// UK sort code
var (result, sortCode) = SortCode.TryCreate(
    CountryCode.UnitedKingdom,
    "12-34-56"
);

// Accepts various formats
var formats = new[] {
    "123456",
    "12-34-56",
    "12 34 56"
};

foreach (var format in formats)
{
    var (r, sc) = SortCode.TryCreate(CountryCode.UnitedKingdom, format);
    Console.WriteLine(sc.ToDigitsOnly());       // "123456"
    Console.WriteLine(sc.ToFormattedString());  // "12-34-56"
}
```

## Usage Examples

### Domestic US Payment

```csharp
using Validated.Primitives.ValueObjects;

// Create US banking details
var (routingResult, routing) = RoutingNumber.TryCreate("021000021");
var (accountResult, account) = BankAccountNumber.TryCreate(
    CountryCode.UnitedStates,
    "123456789"
);

if (routingResult.IsValid && accountResult.IsValid)
{
    Console.WriteLine($"Bank: Federal Reserve District {routing!.FederalReserveDistrict}");
    Console.WriteLine($"Routing: {routing.ToFormattedString()}");
    Console.WriteLine($"Account: {account!.Masked()}");
}
```

### UK Domestic Payment

```csharp
// Create UK banking details
var (sortResult, sortCode) = SortCode.TryCreate(
    CountryCode.UnitedKingdom,
    "12-34-56"
);

var (accountResult, account) = BankAccountNumber.TryCreate(
    CountryCode.UnitedKingdom,
    "12345678"
);

if (sortResult.IsValid && accountResult.IsValid)
{
    Console.WriteLine($"Sort Code: {sortCode!.ToFormattedString()}");
    Console.WriteLine($"Account: {account!.Masked()}");
}
```

## International Wire Transfer Example

```csharp
using Validated.Primitives.ValueObjects;

public class InternationalWireTransfer
{
    public SwiftCode BeneficiaryBank { get; set; }
    public BankAccountNumber BeneficiaryAccount { get; set; }
    public SwiftCode? IntermediaryBank { get; set; }
}

// Create wire transfer to German bank
var (swiftResult, swift) = SwiftCode.TryCreate("DEUTDEFF");
var (accountResult, account) = BankAccountNumber.TryCreate(
    CountryCode.Germany,
    "DE89370400440532013000"
);

if (swiftResult.IsValid && accountResult.IsValid)
{
    var transfer = new InternationalWireTransfer
    {
        BeneficiaryBank = swift!,
        BeneficiaryAccount = account!
    };

    Console.WriteLine($"Beneficiary Bank: {swift.InstitutionCode} ({swift.CountryCode})");
    Console.WriteLine($"Beneficiary Account: {account.ToFormattedString()}");
    Console.WriteLine($"IBAN Valid: {account.IsIban}");
    
    // Validate before processing
    if (!swift.IsTestCode && !transfer.BeneficiaryAccount.ToNormalizedString().Contains("00000"))
    {
        Console.WriteLine("? Ready to process international wire transfer");
    }
}
```

### Complete Banking Information Class

```csharp
using Validated.Primitives.ValueObjects;
using Validated.Primitives.Core;

public class BankingDetails
{
    public CountryCode Country { get; set; }
    public BankAccountNumber AccountNumber { get; set; }
    public SwiftCode? SwiftCode { get; set; }
    public RoutingNumber? RoutingNumber { get; set; }
    public SortCode? SortCode { get; set; }

    public static (ValidationResult Result, BankingDetails? Details) Create(
        CountryCode country,
        string accountNumber,
        string? swiftCode = null,
        string? routingNumber = null,
        string? sortCode = null)
    {
        var errors = new List<ValidationError>();
        
        // Validate account number (required)
        var (accountResult, account) = BankAccountNumber.TryCreate(country, accountNumber);
        if (!accountResult.IsValid)
        {
            errors.AddRange(accountResult.Errors);
        }

        // Validate SWIFT code (optional, for international transfers)
        SwiftCode? swift = null;
        if (!string.IsNullOrWhiteSpace(swiftCode))
        {
            var (swiftResult, swiftValue) = SwiftCode.TryCreate(swiftCode);
            if (!swiftResult.IsValid)
            {
                errors.AddRange(swiftResult.Errors);
            }
            swift = swiftValue;
        }

        // Validate routing number (US only)
        RoutingNumber? routing = null;
        if (country == CountryCode.UnitedStates && !string.IsNullOrWhiteSpace(routingNumber))
        {
            var (routingResult, routingValue) = RoutingNumber.TryCreate(routingNumber);
            if (!routingResult.IsValid)
            {
                errors.AddRange(routingResult.Errors);
            }
            routing = routingValue;
        }

        // Validate sort code (UK/Ireland only)
        SortCode? sort = null;
        if ((country == CountryCode.UnitedKingdom || country == CountryCode.Ireland) 
            && !string.IsNullOrWhiteSpace(sortCode))
        {
            var (sortResult, sortValue) = SortCode.TryCreate(country, sortCode);
            if (!sortResult.IsValid)
            {
                errors.AddRange(sortResult.Errors);
            }
            sort = sortValue;
        }

        if (errors.Any())
        {
            return (ValidationResult.Failure(errors), null);
        }

        var details = new BankingDetails
        {
            Country = country,
            AccountNumber = account!,
            SwiftCode = swift,
            RoutingNumber = routing,
            SortCode = sort
        };

        return (ValidationResult.Success(), details);
    }
}

// Usage
var (result, banking) = BankingDetails.Create(
    country: CountryCode.UnitedStates,
    accountNumber: "123456789",
    routingNumber: "021000021",
    swiftCode: "CHASUS33"
);

if (result.IsValid)
{
    Console.WriteLine("? Banking details validated successfully");
    Console.WriteLine($"Account: {banking!.AccountNumber.Masked()}");
    Console.WriteLine($"Routing: {banking.RoutingNumber?.ToFormattedString()}");
    Console.WriteLine($"SWIFT: {banking.SwiftCode?.ToString()}");
}
```

## Validation Features

### Comprehensive Error Messages

All banking value objects provide detailed validation errors:

```csharp
var (result, swift) = SwiftCode.TryCreate("INVALID");

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"Field: {error.MemberName}");
        Console.WriteLine($"Code: {error.Code}");
        Console.WriteLine($"Message: {error.Message}");
    }
}

// Output:
// Field: SwiftCode
// Code: InvalidLength
// Message: SWIFT code must be either 8 characters (BIC8) or 11 characters (BIC11) according to ISO 9362
```

### JSON Serialization

All banking value objects support JSON serialization:

```csharp
using System.Text.Json;

var (_, swift) = SwiftCode.TryCreate("DEUTDEFF");
var json = JsonSerializer.Serialize(swift);
// Output: "DEUTDEFF"

var deserialized = JsonSerializer.Deserialize<SwiftCode>(json);
// Validation occurs during deserialization
```

## Standards Reference

- **ISO 9362** - SWIFT/BIC codes for international bank identification
- **ISO 3166-1** - Country codes (alpha-2)
- **ISO 13616** - IBAN (International Bank Account Number)
- **ABA** - Routing Transit Numbers for US banks
- **UK/Ireland** - Sort codes for domestic UK/Ireland banking

## See Also

- [Value Objects Documentation](../docs/VALUE_OBJECTS.md)
- [Validation Framework](../docs/VALIDATION.md)
- [Builder Pattern Examples](../docs/BUILDERS.md)
