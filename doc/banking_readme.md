# Banking

The Banking namespace provides validated value objects and domain models for working with international banking information, including account numbers, routing codes, SWIFT/BIC codes, and country-specific banking identifiers. All types include built-in validation, format checking, and secure masking capabilities.

## ?? Package Information

The banking types are split across two packages:

### Core Primitives (`Validated.Primitives`)
- `IbanNumber` - International Bank Account Number (IBAN) and domestic account numbers (BBAN)
- `SwiftCode` - SWIFT/BIC codes for international transfers (ISO 9362)
- `RoutingNumber` - US ABA routing numbers for domestic transfers
- `SortCode` - UK/Ireland bank sort codes
- `BankAccountNumber` - Generic bank account number validation

### Domain Models (`Validated.Primitives.Domain`)
- `BankingDetails` - Complete banking information with country-specific validation
- `BankingDetailsBuilder` - Fluent builder for constructing banking details

---

## ?? Core Primitives

### IbanNumber

Represents a validated bank account number that automatically detects and validates IBAN (International Bank Account Number) or BBAN (Basic Bank Account Number) format according to ISO 13616 standard.

#### Key Features
- Automatic format detection (IBAN vs BBAN)
- ISO 13616 compliant IBAN validation
- Mod-97 checksum validation for IBAN
- Country-specific length validation (70+ countries)
- Country code extraction from IBAN
- Formatted display (groups of 4 for IBAN)
- Account masking for security
- JSON serialization support

#### IBAN Format
- **Structure**: `CC##BBBBBBBBBBBBBBB`
  - CC: 2-letter country code (ISO 3166-1 alpha-2)
  - ##: 2-digit check digits (mod-97 algorithm)
  - B...: Basic Bank Account Number (BBAN) - country-specific

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create IBAN with validation
var (result, iban) = IbanNumber.TryCreate("DE89370400440532013000");

if (result.IsValid)
{
    Console.WriteLine(iban.Value);                  // "DE89370400440532013000"
    Console.WriteLine(iban.ToNormalizedString());   // "DE89370400440532013000"
    Console.WriteLine(iban.ToFormattedString());    // "DE89 3704 0044 0532 0130 00"
    Console.WriteLine(iban.AccountType);            // Iban
    Console.WriteLine(iban.IsIban);                 // true
    Console.WriteLine(iban.CountryCode);            // Germany
}
else
{
    Console.WriteLine(result.ToBulletList());
}
```

#### IBAN Examples

```csharp
// Valid IBANs from different countries
var (r1, iban1) = IbanNumber.TryCreate("DE89370400440532013000");     // ? Germany (22 chars)
var (r2, iban2) = IbanNumber.TryCreate("GB82WEST12345698765432");     // ? UK (22 chars)
var (r3, iban3) = IbanNumber.TryCreate("FR1420041010050500013M02606"); // ? France (27 chars)
var (r4, iban4) = IbanNumber.TryCreate("IT60X0542811101000000123456"); // ? Italy (27 chars)
var (r5, iban5) = IbanNumber.TryCreate("ES9121000418450200051332");    // ? Spain (24 chars)

// With spaces (automatically normalized)
var (r6, iban6) = IbanNumber.TryCreate("DE89 3704 0044 0532 0130 00"); // ? Spaces allowed

// Invalid IBANs
var (r7, iban7) = IbanNumber.TryCreate("DE00000000000000000000");      // ? Invalid checksum
var (r8, iban8) = IbanNumber.TryCreate("XX1234567890");                // ? Invalid country code
```

#### BBAN (Domestic Account Numbers)

```csharp
// US domestic account number (BBAN)
var (result, bban) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);

if (result.IsValid)
{
    Console.WriteLine(bban.AccountType);  // Bban
    Console.WriteLine(bban.IsBban);       // true
    Console.WriteLine(bban.IsIban);       // false
    Console.WriteLine(bban.CountryCode);  // UnitedStates
}
```

#### IBAN Component Extraction

```csharp
var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

Console.WriteLine(iban.GetIbanCountryCode());  // "DE"
Console.WriteLine(iban.GetIbanCheckDigits());  // "89"
Console.WriteLine(iban.GetBbanPart());         // "370400440532013000"
```

#### Account Masking

```csharp
var (_, iban) = IbanNumber.TryCreate("DE89370400440532013000");

Console.WriteLine(iban.Masked());  // "******************3000"

var (_, bban) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);
Console.WriteLine(bban.Masked());  // "*****6789"
```

---

### SwiftCode

Represents a validated SWIFT code (also known as BIC - Bank Identifier Code) according to ISO 9362 standard. SWIFT codes identify banks and financial institutions globally for international wire transfers.

#### Key Features
- ISO 9362 standard compliance
- Support for BIC8 (8 characters) and BIC11 (11 characters)
- Institution code, country code, location code, and branch code extraction
- Primary office detection
- Test code detection
- Format normalization (uppercase)
- Country code validation (ISO 3166-1 alpha-2)

#### ISO 9362 Format
- **Structure**: `AAAABBCCXXX`
  - AAAA: Institution code (4 letters A-Z) - identifies the bank
  - BB: Country code (2 letters A-Z, ISO 3166-1 alpha-2)
  - CC: Location code (2 characters A-Z or 0-9) - identifies location/city
  - XXX: Branch code (optional, 3 characters A-Z or 0-9) - identifies branch

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create SWIFT code with validation
var (result, swift) = SwiftCode.TryCreate("DEUTDEFF");

if (result.IsValid)
{
    Console.WriteLine(swift.Value);              // "DEUTDEFF"
    Console.WriteLine(swift.ToNormalizedString()); // "DEUTDEFF"
    Console.WriteLine(swift.ToFullFormat());     // "DEUTDEFFXXX"
    Console.WriteLine(swift.InstitutionCode);    // "DEUT"
    Console.WriteLine(swift.CountryCode);        // "DE"
    Console.WriteLine(swift.LocationCode);       // "FF"
    Console.WriteLine(swift.BranchCode);         // "XXX"
    Console.WriteLine(swift.IsPrimaryOffice);    // true
    Console.WriteLine(swift.IsBic8);             // true
}
```

#### SWIFT Code Examples

```csharp
// Valid SWIFT codes (BIC8 - primary office)
var (r1, swift1) = SwiftCode.TryCreate("DEUTDEFF");    // ? Deutsche Bank, Germany, Frankfurt
var (r2, swift2) = SwiftCode.TryCreate("CHASUS33");    // ? Chase Bank, USA
var (r3, swift3) = SwiftCode.TryCreate("BARCGB22");    // ? Barclays, UK, London

// Valid SWIFT codes (BIC11 - with branch)
var (r4, swift4) = SwiftCode.TryCreate("DEUTDEFFXXX"); // ? Deutsche Bank, primary office (explicit)
var (r5, swift5) = SwiftCode.TryCreate("NATAAU3303M"); // ? National Australia Bank, Melbourne branch

// Case-insensitive
var (r6, swift6) = SwiftCode.TryCreate("deutdeff");    // ? Normalized to "DEUTDEFF"

// Invalid SWIFT codes
var (r7, swift7) = SwiftCode.TryCreate("DEUT");        // ? Too short
var (r8, swift8) = SwiftCode.TryCreate("DEUTDEFFXXXX"); // ? Too long
var (r9, swift9) = SwiftCode.TryCreate("12345678");    // ? Invalid format (must be letters)
```

#### SWIFT Code Components

```csharp
var (_, swift) = SwiftCode.TryCreate("DEUTDEFF500");

Console.WriteLine(swift.BankCode);          // "DEUT" (alias for InstitutionCode)
Console.WriteLine(swift.InstitutionCode);   // "DEUT"
Console.WriteLine(swift.CountryCode);       // "DE"
Console.WriteLine(swift.LocationCode);      // "FF"
Console.WriteLine(swift.BranchCode);        // "500"
Console.WriteLine(swift.IsPrimaryOffice);   // false (has specific branch)
Console.WriteLine(swift.IsBic8);            // false
Console.WriteLine(swift.IsBic11);           // true
```

#### Test Code Detection

```csharp
// Test codes have '0' as second character of location code
var (_, testSwift) = SwiftCode.TryCreate("DEUTDE0F");

Console.WriteLine(testSwift.IsTestCode);  // true (location code is "0F")

// Production code
var (_, prodSwift) = SwiftCode.TryCreate("DEUTDEFF");
Console.WriteLine(prodSwift.IsTestCode);  // false
```

#### BIC8 vs BIC11 Equivalence

```csharp
var (_, bic8) = SwiftCode.TryCreate("DEUTDEFF");
var (_, bic11) = SwiftCode.TryCreate("DEUTDEFFXXX");

// These are considered equal (XXX indicates primary office)
bool areEqual = bic8.Equals(bic11);  // true
Console.WriteLine(bic8.ToFullFormat() == bic11.ToFullFormat());  // true
```

---

### RoutingNumber

Represents a validated US ABA routing number (also known as routing transit number). ABA routing numbers are 9-digit codes used to identify financial institutions in the United States.

#### Key Features
- 9-digit format validation
- Federal Reserve routing symbol validation
- ABA checksum validation (weighted algorithm)
- Federal Reserve District extraction
- Formatted display (XXXX-YYYY-C)
- Support for various input formats (with/without separators)

#### Format
- **Structure**: `XXXXYYYYC`
  - XXXX: Federal Reserve routing symbol (first 4 digits)
  - YYYY: ABA Institution Identifier (middle 4 digits)
  - C: Check digit (last digit, calculated using ABA algorithm)

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create routing number with validation
var (result, routing) = RoutingNumber.TryCreate("021000021");

if (result.IsValid)
{
    Console.WriteLine(routing.Value);                   // "021000021"
    Console.WriteLine(routing.ToDigitsOnly());          // "021000021"
    Console.WriteLine(routing.ToFormattedString());     // "0210-0002-1"
    Console.WriteLine(routing.FederalReserveSymbol);    // "0210"
    Console.WriteLine(routing.InstitutionIdentifier);   // "0002"
    Console.WriteLine(routing.CheckDigit);              // "1"
    Console.WriteLine(routing.FederalReserveDistrict);  // 2 (New York)
}
```

#### Routing Number Examples

```csharp
// Valid routing numbers
var (r1, rn1) = RoutingNumber.TryCreate("021000021");       // ? Chase Bank, New York
var (r2, rn2) = RoutingNumber.TryCreate("011401533");       // ? Wells Fargo, California
var (r3, rn3) = RoutingNumber.TryCreate("121000248");       // ? Wells Fargo, Arizona

// With formatting (automatically normalized)
var (r4, rn4) = RoutingNumber.TryCreate("0210-0002-1");     // ? Dashes allowed
var (r5, rn5) = RoutingNumber.TryCreate("0210 0002 1");     // ? Spaces allowed

// Invalid routing numbers
var (r6, rn6) = RoutingNumber.TryCreate("000000000");       // ? Invalid Federal Reserve symbol
var (r7, rn7) = RoutingNumber.TryCreate("021000020");       // ? Invalid checksum
var (r8, rn8) = RoutingNumber.TryCreate("12345678");        // ? Too short (must be 9 digits)
```

#### Routing Number Components

```csharp
var (_, routing) = RoutingNumber.TryCreate("121000248");

Console.WriteLine(routing.FederalReserveSymbol);    // "1210" (Phoenix)
Console.WriteLine(routing.InstitutionIdentifier);   // "0024"
Console.WriteLine(routing.CheckDigit);              // "8"
Console.WriteLine(routing.FederalReserveDistrict);  // 12 (San Francisco District)
```

#### Federal Reserve Districts

The first two digits indicate the Federal Reserve District:
- 01-12: Federal Reserve Districts
  - 01: Boston
  - 02: New York
  - 03: Philadelphia
  - 04: Cleveland
  - 05: Richmond
  - 06: Atlanta
  - 07: Chicago
  - 08: St. Louis
  - 09: Minneapolis
  - 10: Kansas City
  - 11: Dallas
  - 12: San Francisco

```csharp
var (_, routing) = RoutingNumber.TryCreate("021000021");
Console.WriteLine(routing.FederalReserveDistrict);  // 2 (New York)
```

---

### SortCode

Represents a validated bank sort code with country-specific format validation. Sort codes are primarily used in the UK and Ireland to identify bank branches.

#### Key Features
- Country-specific validation (UK and Ireland)
- 6-digit format validation
- Formatted display (XX-XX-XX)
- Support for various input formats (with/without separators)

#### Format
- **UK/Ireland**: 6 digits, displayed as XX-XX-XX
- Example: 12-34-56

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create UK sort code with validation
var (result, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");

if (result.IsValid)
{
    Console.WriteLine(sortCode.Value);              // "123456"
    Console.WriteLine(sortCode.ToDigitsOnly());     // "123456"
    Console.WriteLine(sortCode.ToFormattedString()); // "12-34-56"
    Console.WriteLine(sortCode.CountryCode);        // UnitedKingdom
    Console.WriteLine(sortCode.GetCountryName());   // "United Kingdom"
}
```

#### Sort Code Examples

```csharp
// Valid UK sort codes
var (r1, sc1) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
var (r2, sc2) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12-34-56");  // ? Dashes allowed
var (r3, sc3) = SortCode.TryCreate(CountryCode.UnitedKingdom, "20-00-00");  // ? Barclays

// Valid Ireland sort codes
var (r4, sc4) = SortCode.TryCreate(CountryCode.Ireland, "123456");

// Invalid sort codes
var (r5, sc5) = SortCode.TryCreate(CountryCode.UnitedKingdom, "12345");     // ? Too short
var (r6, sc6) = SortCode.TryCreate(CountryCode.UnitedKingdom, "1234567");   // ? Too long
var (r7, sc7) = SortCode.TryCreate(CountryCode.UnitedKingdom, "ABCDEF");    // ? Must be digits
```

---

## ?? Domain Models

### BankingDetails

Represents complete banking details including account number and routing/identification codes with country-specific validation rules.

#### Key Features
- Country-specific validation requirements
- Automatic IBAN/BBAN detection
- Support for international banking (SWIFT/BIC + IBAN)
- Support for US domestic banking (Routing Number + Account Number)
- Support for UK/Ireland banking (Sort Code + Account Number)
- International transfer capability detection
- Account number masking for security
- Comprehensive validation with detailed error messages

#### Country-Specific Requirements

| Country | Required | Optional |
|---------|----------|----------|
| **USA** | Routing Number + Account Number | SWIFT Code (for international) |
| **UK** | Sort Code + Account Number | SWIFT Code (for international) |
| **Ireland** | Sort Code + Account Number | SWIFT Code (for international) |
| **International (IBAN)** | IBAN Account Number | SWIFT Code (recommended) |
| **Other** | Account Number | SWIFT Code |

#### Basic Usage - International (IBAN)

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

// German banking details with IBAN and SWIFT
var (result, banking) = BankingDetails.TryCreate(
    country: CountryCode.Germany,
    accountNumber: "DE89370400440532013000",
    swiftCode: "DEUTDEFF"
);

if (result.IsValid)
{
    Console.WriteLine(banking.Country);                        // Germany
    Console.WriteLine(banking.AccountNumber.ToFormattedString()); // "DE89 3704 0044 0532 0130 00"
    Console.WriteLine(banking.SwiftCode);                      // DEUTDEFF
    Console.WriteLine(banking.UsesIban);                       // true
    Console.WriteLine(banking.SupportsInternationalTransfers); // true
    Console.WriteLine(banking.MaskedAccountNumber);            // "******************3000"
}
```

#### Basic Usage - US Banking

```csharp
// US banking details with routing number
var (result, banking) = BankingDetails.TryCreate(
    country: CountryCode.UnitedStates,
    accountNumber: "123456789",
    routingNumber: "021000021",
    swiftCode: "CHASUS33"  // Optional, for international transfers
);

if (result.IsValid)
{
    Console.WriteLine(banking.RoutingNumber.ToFormattedString()); // "0210-0002-1"
    Console.WriteLine(banking.AccountNumber.Value);               // "123456789"
    Console.WriteLine(banking.SwiftCode);                         // CHASUS33
    Console.WriteLine(banking.SupportsInternationalTransfers);    // true
}
```

#### Basic Usage - UK Banking

```csharp
// UK banking details with sort code
var (result, banking) = BankingDetails.TryCreate(
    country: CountryCode.UnitedKingdom,
    accountNumber: "12345678",
    sortCode: "123456",
    swiftCode: "BARCGB22"  // Optional, for international transfers
);

if (result.IsValid)
{
    Console.WriteLine(banking.SortCode.ToFormattedString()); // "12-34-56"
    Console.WriteLine(banking.AccountNumber.Value);          // "12345678"
    Console.WriteLine(banking.SwiftCode);                    // BARCGB22
}
```

#### Validation Examples

```csharp
// ? Valid - US banking with routing number
var (r1, b1) = BankingDetails.TryCreate(
    CountryCode.UnitedStates,
    "123456789",
    routingNumber: "021000021"
);

// ? Invalid - US banking without routing number
var (r2, b2) = BankingDetails.TryCreate(
    CountryCode.UnitedStates,
    "123456789"
);
// Error: "Routing number is required for United States banking details"

// ? Invalid - US banking with sort code (not applicable)
var (r3, b3) = BankingDetails.TryCreate(
    CountryCode.UnitedStates,
    "123456789",
    routingNumber: "021000021",
    sortCode: "123456"
);
// Error: "Sort code is only applicable for United Kingdom or Ireland banking"

// ? Valid - UK banking with sort code
var (r4, b4) = BankingDetails.TryCreate(
    CountryCode.UnitedKingdom,
    "12345678",
    sortCode: "123456"
);

// ? Invalid - UK banking without sort code
var (r5, b5) = BankingDetails.TryCreate(
    CountryCode.UnitedKingdom,
    "12345678"
);
// Error: "Sort code is required for UnitedKingdom banking details"

// ? Valid - International IBAN banking
var (r6, b6) = BankingDetails.TryCreate(
    CountryCode.France,
    "FR1420041010050500013M02606",
    swiftCode: "BNPAFRPP"
);
```

#### Properties and Methods

```csharp
var (_, banking) = BankingDetails.TryCreate(
    CountryCode.Germany,
    "DE89370400440532013000",
    "DEUTDEFF"
);

// Check if supports international transfers
if (banking.SupportsInternationalTransfers)
{
    Console.WriteLine("Can send/receive international wire transfers");
}

// Check if uses IBAN format
if (banking.UsesIban)
{
    Console.WriteLine("Account uses IBAN format");
}

// Get masked account number for display
Console.WriteLine($"Account: {banking.MaskedAccountNumber}");
// Output: "Account: ******************3000"

// Get formatted string representation
Console.WriteLine(banking.ToString());
// Output: "SWIFT: DEUTDEFF | Account: ******************3000 | (Germany)"
```

#### ToString() Formatting

```csharp
// US banking with all components
var (_, usBanking) = BankingDetails.TryCreate(
    CountryCode.UnitedStates,
    "123456789",
    "CHASUS33",
    "021000021"
);
Console.WriteLine(usBanking.ToString());
// "SWIFT: CHASUS33 | Routing: 0210-0002-1 | Account: *****6789 | (UnitedStates)"

// UK banking
var (_, ukBanking) = BankingDetails.TryCreate(
    CountryCode.UnitedKingdom,
    "12345678",
    sortCode: "123456",
    swiftCode: "BARCGB22"
);
Console.WriteLine(ukBanking.ToString());
// "SWIFT: BARCGB22 | Sort Code: 12-34-56 | Account: ****5678 | (UnitedKingdom)"

// International IBAN only
var (_, ibanBanking) = BankingDetails.TryCreate(
    CountryCode.Germany,
    "DE89370400440532013000"
);
Console.WriteLine(ibanBanking.ToString());
// "Account: ******************3000 | (Germany)"
```

---

### BankingDetailsBuilder

Fluent builder for creating validated `BankingDetails` with convenience methods for different banking systems.

#### Key Features
- Fluent interface for step-by-step construction
- Convenience methods for US, UK, and international banking
- Automatic country detection from IBAN
- Comprehensive validation before building
- Reusable with `Reset()` method

#### Basic Builder Usage

```csharp
using Validated.Primitives.Domain.Builders;

var builder = new BankingDetailsBuilder();

// Build step-by-step
var (result, banking) = builder
    .WithCountry(CountryCode.Germany)
    .WithAccountNumber("DE89370400440532013000")
    .WithSwiftCode("DEUTDEFF")
    .Build();
```

#### US Banking Convenience Method

```csharp
var builder = new BankingDetailsBuilder();

// Method 1: Using convenience method
var (result1, banking1) = builder
    .WithUsBanking(
        routingNumber: "021000021",
        accountNumber: "123456789",
        swiftCode: "CHASUS33"  // Optional
    )
    .Build();

// Method 2: Step-by-step (equivalent)
var (result2, banking2) = builder
    .Reset()
    .WithCountry(CountryCode.UnitedStates)
    .WithRoutingNumber("021000021")
    .WithAccountNumber("123456789")
    .WithSwiftCode("CHASUS33")
    .Build();
```

#### UK Banking Convenience Method

```csharp
var builder = new BankingDetailsBuilder();

// Using convenience method
var (result, banking) = builder
    .WithUkBanking(
        sortCode: "123456",
        accountNumber: "12345678",
        swiftCode: "BARCGB22"  // Optional
    )
    .Build();
```

#### International Banking Convenience Method

```csharp
var builder = new BankingDetailsBuilder();

// Method 1: With explicit country
var (result1, banking1) = builder
    .WithInternationalBanking(
        iban: "DE89370400440532013000",
        swiftCode: "DEUTDEFF",
        country: CountryCode.Germany
    )
    .Build();

// Method 2: Auto-detect country from IBAN
var (result2, banking2) = builder
    .Reset()
    .WithInternationalBanking(
        iban: "FR1420041010050500013M02606",
        swiftCode: "BNPAFRPP"
        // Country auto-detected as France from IBAN
    )
    .Build();
```

#### Builder Reuse

```csharp
var builder = new BankingDetailsBuilder();

// First banking details
var (result1, banking1) = builder
    .WithUsBanking("021000021", "123456789")
    .Build();

// Reuse builder for different banking details
var (result2, banking2) = builder
    .Reset()  // Clear previous values
    .WithUkBanking("123456", "12345678", "BARCGB22")
    .Build();

// Another reuse
var (result3, banking3) = builder
    .Reset()
    .WithInternationalBanking("DE89370400440532013000", "DEUTDEFF")
    .Build();
```

#### Complete Builder Example

```csharp
var builder = new BankingDetailsBuilder();

// Build complex banking details
var (result, banking) = builder
    .WithCountry(CountryCode.UnitedStates)
    .WithAccountNumber("987654321")
    .WithRoutingNumber("121000248")
    .WithSwiftCode("WFBIUS6S")
    .Build();

if (result.IsValid)
{
    Console.WriteLine("Banking details created successfully!");
    Console.WriteLine(banking.ToString());
}
else
{
    Console.WriteLine("Validation errors:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error.MemberName}: {error.Message}");
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

All banking types follow the same validation pattern:

```csharp
var (result, value) = Type.TryCreate(...);

if (result.IsValid)
{
    // Use the validated value
    ProcessBankingInfo(value);
}
else
{
    // Handle validation errors
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.MemberName}: {error.Message}");
    }
    
    // Or use formatted output
    Console.WriteLine(result.ToBulletList());
}
```

### Builder Pattern

Use builders for complex construction:

```csharp
var builder = new BankingDetailsBuilder();

var (result, value) = builder
    .WithProperty1(value1)
    .WithProperty2(value2)
    .Build();

// Reuse the builder
builder.Reset();
var (result2, value2) = builder
    .WithProperty1(newValue1)
    .Build();
```

### JSON Serialization

All types support JSON serialization:

```csharp
using System.Text.Json;

var (_, banking) = BankingDetails.TryCreate(
    CountryCode.Germany,
    "DE89370400440532013000",
    "DEUTDEFF"
);

// Serialize
string json = JsonSerializer.Serialize(banking);

// Deserialize
var deserialized = JsonSerializer.Deserialize<BankingDetails>(json);
```

### Security - Account Masking

Always use masking when displaying account numbers:

```csharp
var (_, banking) = BankingDetails.TryCreate(...);

// DON'T: Display full account number
Console.WriteLine($"Account: {banking.AccountNumber.Value}");

// DO: Use masked version
Console.WriteLine($"Account: {banking.MaskedAccountNumber}");

// DO: Use ToString() which includes masking
Console.WriteLine(banking.ToString());
```

---

## ?? Real-World Examples

### International Wire Transfer

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.Domain.Builders;
using Validated.Primitives.ValueObjects;

// Sender's US bank account
var (senderResult, senderBanking) = new BankingDetailsBuilder()
    .WithUsBanking(
        routingNumber: "021000021",
        accountNumber: "123456789",
        swiftCode: "CHASUS33"
    )
    .Build();

// Recipient's German bank account
var (recipientResult, recipientBanking) = new BankingDetailsBuilder()
    .WithInternationalBanking(
        iban: "DE89370400440532013000",
        swiftCode: "DEUTDEFF"
    )
    .Build();

if (senderResult.IsValid && recipientResult.IsValid)
{
    // Verify international transfer capability
    if (senderBanking.SupportsInternationalTransfers && 
        recipientBanking.SupportsInternationalTransfers)
    {
        Console.WriteLine("International wire transfer is possible!");
        Console.WriteLine($"From: {senderBanking.SwiftCode}");
        Console.WriteLine($"To: {recipientBanking.SwiftCode}");
    }
    else
    {
        Console.WriteLine("International transfer not supported");
    }
}
```

### Payment Gateway Integration

```csharp
public class PaymentProcessor
{
    public async Task<PaymentResult> ProcessPayment(
        decimal amount,
        string accountNumber,
        string routingNumber,
        string swiftCode = null)
    {
        // Validate banking details before processing
        var (validationResult, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            accountNumber,
            swiftCode,
            routingNumber
        );

        if (!validationResult.IsValid)
        {
            return PaymentResult.Failed(
                $"Invalid banking details: {validationResult.ToBulletList()}"
            );
        }

        // Log with masked account number for security
        _logger.LogInformation(
            "Processing payment: Amount={Amount}, Account={MaskedAccount}, Routing={Routing}",
            amount,
            banking.MaskedAccountNumber,
            banking.RoutingNumber.ToFormattedString()
        );

        // Process payment with validated banking details
        return await _paymentGateway.ProcessAsync(amount, banking);
    }
}
```

### Multi-Country Payment System

```csharp
public class BankingDetailsFactory
{
    public (ValidationResult Result, BankingDetails? Banking) CreateBankingDetails(
        CountryCode country,
        string accountNumber,
        string? identifier1 = null,  // Routing/Sort Code
        string? identifier2 = null)  // SWIFT Code
    {
        var builder = new BankingDetailsBuilder();

        return country switch
        {
            CountryCode.UnitedStates => builder.WithUsBanking(
                routingNumber: identifier1 ?? throw new ArgumentNullException(nameof(identifier1)),
                accountNumber: accountNumber,
                swiftCode: identifier2
            ).Build(),

            CountryCode.UnitedKingdom or CountryCode.Ireland => builder.WithUkBanking(
                sortCode: identifier1 ?? throw new ArgumentNullException(nameof(identifier1)),
                accountNumber: accountNumber,
                swiftCode: identifier2
            ).Build(),

            _ => builder.WithInternationalBanking(
                iban: accountNumber,
                swiftCode: identifier1 ?? throw new ArgumentNullException(nameof(identifier1)),
                country: country
            ).Build()
        };
    }
}

// Usage
var factory = new BankingDetailsFactory();

// US customer
var (r1, usBanking) = factory.CreateBankingDetails(
    CountryCode.UnitedStates,
    accountNumber: "123456789",
    identifier1: "021000021",  // Routing number
    identifier2: "CHASUS33"     // SWIFT (optional)
);

// UK customer
var (r2, ukBanking) = factory.CreateBankingDetails(
    CountryCode.UnitedKingdom,
    accountNumber: "12345678",
    identifier1: "123456",      // Sort code
    identifier2: "BARCGB22"     // SWIFT (optional)
);

// German customer
var (r3, deBanking) = factory.CreateBankingDetails(
    CountryCode.Germany,
    accountNumber: "DE89370400440532013000",  // IBAN
    identifier1: "DEUTDEFF"     // SWIFT
);
```

### Account Verification Service

```csharp
public class BankAccountVerifier
{
    public async Task<VerificationResult> VerifyAccount(BankingDetails banking)
    {
        // Check account number format
        if (!banking.UsesIban && banking.Country == CountryCode.UnitedStates)
        {
            // Verify routing number is valid and active
            var routingValid = await _routingNumberService.ValidateAsync(
                banking.RoutingNumber.ToDigitsOnly()
            );

            if (!routingValid)
            {
                return VerificationResult.Failed("Invalid or inactive routing number");
            }
        }

        // Verify SWIFT code for international transfers
        if (banking.SupportsInternationalTransfers)
        {
            var swiftValid = await _swiftCodeService.ValidateAsync(
                banking.SwiftCode.ToNormalizedString()
            );

            if (!swiftValid)
            {
                return VerificationResult.Failed("Invalid or inactive SWIFT code");
            }
        }

        // Perform micro-deposit verification
        var microDepositResult = await _microDepositService.InitiateAsync(banking);

        return VerificationResult.PendingVerification(microDepositResult.TransactionId);
    }
}
```

### Secure Display and Logging

```csharp
public class BankingDetailsDisplay
{
    public string GetDisplayText(BankingDetails banking)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"Country: {banking.Country}");
        sb.AppendLine($"Account: {banking.MaskedAccountNumber}");

        if (banking.RoutingNumber != null)
        {
            sb.AppendLine($"Routing: {banking.RoutingNumber.ToFormattedString()}");
        }

        if (banking.SortCode != null)
        {
            sb.AppendLine($"Sort Code: {banking.SortCode.ToFormattedString()}");
        }

        if (banking.SwiftCode != null)
        {
            sb.AppendLine($"SWIFT/BIC: {banking.SwiftCode.ToNormalizedString()}");
        }

        sb.AppendLine($"International Transfers: {(banking.SupportsInternationalTransfers ? "Yes" : "No")}");
        sb.AppendLine($"IBAN Format: {(banking.UsesIban ? "Yes" : "No")}");

        return sb.ToString();
    }

    public void LogBankingDetails(ILogger logger, BankingDetails banking, string userId)
    {
        // Always use masked account number in logs
        logger.LogInformation(
            "User {UserId} banking details: Country={Country}, Account={MaskedAccount}, IBAN={IsIban}",
            userId,
            banking.Country,
            banking.MaskedAccountNumber,
            banking.UsesIban
        );
    }
}
```

### Form Validation

```csharp
public class BankingDetailsFormValidator
{
    public ValidationResult ValidateForm(
        string country,
        string accountNumber,
        string routingNumber,
        string sortCode,
        string swiftCode)
    {
        // Parse country code
        if (!Enum.TryParse<CountryCode>(country, out var countryCode))
        {
            return ValidationResult.Failure(
                "Invalid country code",
                nameof(country),
                "InvalidCountry"
            );
        }

        // Create banking details with validation
        var (result, banking) = BankingDetails.TryCreate(
            countryCode,
            accountNumber,
            swiftCode,
            routingNumber,
            sortCode
        );

        if (!result.IsValid)
        {
            // Return detailed validation errors for form display
            return result;
        }

        // Additional business rules
        if (IsHighRiskCountry(countryCode) && !banking.SupportsInternationalTransfers)
        {
            result.AddError(
                "SWIFT code is required for this country",
                nameof(swiftCode),
                "SwiftRequired"
            );
        }

        return result;
    }

    private bool IsHighRiskCountry(CountryCode country)
    {
        // Business logic for high-risk countries
        return false;
    }
}
```

---

## ?? Related Documentation

- [Builder Examples](builders_examples.md) - General builder pattern usage
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

For complete API documentation, see:
- XML documentation comments in source code
- IntelliSense in Visual Studio
- [Validated.Primitives Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)
- [Validated.Primitives.Domain Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)

---

## ?? Security Best Practices

### 1. Always Mask Account Numbers

```csharp
// ? DON'T: Log or display full account numbers
_logger.LogInformation($"Processing account: {banking.AccountNumber.Value}");

// ? DO: Use masked version
_logger.LogInformation($"Processing account: {banking.MaskedAccountNumber}");
```

### 2. Validate Before Processing

```csharp
// ? DO: Always validate before processing payments
var (result, banking) = BankingDetails.TryCreate(...);
if (!result.IsValid)
{
    return Error("Invalid banking details");
}
ProcessPayment(banking);
```

### 3. Use HTTPS for Transmission

```csharp
// ? DO: Only transmit banking details over HTTPS
// ? DO: Use TLS 1.2 or higher
// ? DO: Consider additional encryption for sensitive data
```

### 4. Secure Storage

```csharp
// ? DO: Encrypt account numbers in database
// ? DO: Use separate encryption keys for different data types
// ? DO: Implement key rotation policies
// ? DO: Follow PCI DSS compliance if handling card data
```

### 5. Audit Logging

```csharp
// ? DO: Log all access to banking details
_auditLogger.LogAccess(
    userId: currentUser.Id,
    action: "ViewBankingDetails",
    maskedAccount: banking.MaskedAccountNumber,
    timestamp: DateTime.UtcNow
);
```
