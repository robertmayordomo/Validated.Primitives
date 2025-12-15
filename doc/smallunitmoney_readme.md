# Validated.Primitives.SmallUnitMoney

A validated monetary value primitive that represents amounts in the smallest currency units (cents, pence, etc.) to prevent floating-point precision issues in financial calculations.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`SmallUnitMoney` is a validated value object that represents monetary amounts using the smallest indivisible currency units (cents, pence, etc.) stored as `uint`. This prevents floating-point precision issues common in financial calculations. Once created, a `SmallUnitMoney` instance is guaranteed to be valid.

### Key Features

- **Smallest Units** - Uses cents, pence, etc. instead of decimal amounts
- **Precision Safe** - Prevents floating-point arithmetic issues
- **Country-Based** - Currency determined by country code
- **Automatic Conversion** - Convert between units and decimal representation
- **Currency Symbols** - Provides currency symbols and codes
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other numeric types

---

## ?? Basic Usage

### Creating SmallUnitMoney

```csharp
using Validated.Primitives.ValueObjects;

// Create with country code and amount in smallest units
var (result, money) = SmallUnitMoney.TryCreate(CountryCode.UnitedStates, 2999u); // 2999 cents = $29.99

if (result.IsValid)
{
    Console.WriteLine(money.Value);              // 2999
    Console.WriteLine(money.CountryCode);        // UnitedStates
    Console.WriteLine(money.GetCurrencyCode());  // "USD"
    Console.WriteLine(money.GetCurrencySymbol()); // "$"
    Console.WriteLine(money.ToDecimal());        // 29.99
    Console.WriteLine(money.ToString());         // "$29.99"
    Console.WriteLine(money.ToStringWithCode()); // "29.99 USD"
    Console.WriteLine(money.ToRawString());      // "2999 cents"
    
    // Use the validated money
    ProcessPayment(money);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Converting Between Representations

```csharp
// Create from smallest units
var (result, money) = SmallUnitMoney.TryCreate(CountryCode.UnitedStates, 1050u); // 1050 cents

if (result.IsValid)
{
    // Convert to decimal for display
    decimal decimalAmount = money.ToDecimal();        // 10.50
    
    // Get currency information
    string currencyCode = money.GetCurrencyCode();    // "USD"
    string symbol = money.GetCurrencySymbol();        // "$"
    string unitName = money.GetSmallestUnitName();    // "cents"
    
    Console.WriteLine($"{symbol}{decimalAmount:N2} ({money.Value} {unitName})");
}
```

### Different Currencies

```csharp
// Different currencies with their smallest units
var examples = new[]
{
    (CountryCode.UnitedStates, 2999u),    // 2999 cents = $29.99 USD
    (CountryCode.UnitedKingdom, 1999u),   // 1999 pence = £19.99 GBP
    (CountryCode.Japan, 3500u),           // 3500 yen = ¥3500 JPY (no cents)
    (CountryCode.Germany, 2499u),         // 2499 cents = €24.99 EUR
};

foreach (var (country, units) in examples)
{
    var (result, money) = SmallUnitMoney.TryCreate(country, units);
    if (result.IsValid)
    {
        Console.WriteLine($"{money.ToStringWithCode()} ({money.ToRawString()})");
    }
}
```

---

## ?? Common Patterns

### Small Units Creation Pattern

```csharp
var (result, money) = SmallUnitMoney.TryCreate(countryCode, smallestUnits);

if (result.IsValid)
{
    // Use the validated money
    ProcessMoney(money);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Decimal to Small Units Conversion Pattern

```csharp
// Convert decimal amount to smallest units
decimal decimalAmount = 29.99m;
uint smallestUnits = (uint)(decimalAmount * 100); // For currencies with 2 decimal places

var (result, money) = SmallUnitMoney.TryCreate(CountryCode.UnitedStates, smallestUnits);

if (result.IsValid)
{
    // Now you have precision-safe money
    Console.WriteLine($"Precise amount: {money.Value} cents");
}
```

### Currency-Aware Conversion Pattern

```csharp
var (result, money) = SmallUnitMoney.TryCreate(countryCode, units);

if (result.IsValid)
{
    // Get decimal places for the currency
    int decimalPlaces = money.GetDecimalPlaces();
    
    // Convert to decimal safely
    decimal decimalAmount = money.Value / (decimal)Math.Pow(10, decimalPlaces);
    
    // Use for calculations or display
    Console.WriteLine($"{money.GetCurrencySymbol()}{decimalAmount:N{decimalPlaces}}");
}
```

---

## ?? Related Documentation

- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `uint` | The amount in smallest currency units |
| `CountryCode` | `CountryCode` | The country code for currency determination |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(CountryCode countryCode, uint value, string propertyName = "SmallUnitMoney")` | `(ValidationResult, SmallUnitMoney?)` | Static factory method to create validated small unit money |
| `GetCurrencySymbol()` | `string` | Returns the currency symbol for the country |
| `GetCurrencyCode()` | `string` | Returns the ISO currency code for the country |
| `GetDecimalPlaces()` | `int` | Returns decimal places for the currency (0 or 2) |
| `GetSmallestUnitName()` | `string` | Returns name of smallest unit (cents, pence, etc.) |
| `ToDecimal()` | `decimal` | Converts to decimal representation |
| `ToString()` | `string` | Returns formatted string with currency symbol |
| `ToStringWithCode()` | `string` | Returns formatted string with currency code |
| `ToRawString()` | `string` | Returns raw value with unit name |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Unsigned Integer** | Value is inherently non-negative (uint) |

---

## ?? Currency Standards

### Smallest Units by Currency

| Currency | Code | Smallest Unit | Units per Main Unit |
|----------|------|---------------|-------------------|
| US Dollar | USD | Cent | 100 |
| Euro | EUR | Cent | 100 |
| British Pound | GBP | Pence | 100 |
| Canadian Dollar | CAD | Cent | 100 |
| Australian Dollar | AUD | Cent | 100 |
| Japanese Yen | JPY | Yen | 1 (no cents) |
| South Korean Won | KRW | Won | 1 (no cents) |
| Swiss Franc | CHF | Rappen | 100 |
| Swedish Krona | SEK | Öre | 100 |
| Indian Rupee | INR | Paisa | 100 |
| Brazilian Real | BRL | Centavo | 100 |

### Precision Handling

Different currencies have different decimal precision:

```csharp
// Currencies with 2 decimal places (most common)
var usd = SmallUnitMoney.TryCreate(CountryCode.UnitedStates, 2999u); // $29.99
var eur = SmallUnitMoney.TryCreate(CountryCode.Germany, 2499u);      // €24.99

// Currencies with 0 decimal places (no cents)
var jpy = SmallUnitMoney.TryCreate(CountryCode.Japan, 3500u);        // ¥3500
var krw = SmallUnitMoney.TryCreate(CountryCode.SouthKorea, 50000u);  // ?50000
```

### Unit Names by Country

```csharp
var unitNames = new[]
{
    (CountryCode.UnitedStates, "cents"),
    (CountryCode.UnitedKingdom, "pence"),
    (CountryCode.Japan, "yen"),
    (CountryCode.Germany, "cents"),
    (CountryCode.India, "paise"),
    (CountryCode.Brazil, "centavos"),
    (CountryCode.SouthAfrica, "cents")
};
```

---

## ??? Security Considerations

### SmallUnitMoney Validation

```csharp
// ? DO: Validate small unit money before use
var (result, money) = SmallUnitMoney.TryCreate(countryCode, units);

if (!result.IsValid)
{
    return BadRequest("Invalid money amount");
}

// Additional validation
if (money != null)
{
    // Business rule: maximum transaction amount in smallest units
    var maxUnits = GetMaxAllowedUnits(money.CountryCode);
    if (money.Value > maxUnits)
    {
        return BadRequest($"Transaction amount cannot exceed {money.ToDecimal():C}");
    }
    
    // Business rule: minimum transaction amount
    var minUnits = GetMinAllowedUnits(money.CountryCode);
    if (money.Value < minUnits)
    {
        return BadRequest($"Transaction amount must be at least {money.ToDecimal():C}");
    }
    
    // Business rule: validate country code is supported
    if (!IsCountrySupported(money.CountryCode))
    {
        return BadRequest("Country not supported for transactions");
    }
}

// ? DON'T: Trust user input without validation
var units = uint.Parse(unitsInput);
var money = new SmallUnitMoney(units, countryCode);  // Dangerous!
```

### Preventing Financial Manipulation

```csharp
// ? DO: Enforce business rule constraints on small unit values
var (result, money) = SmallUnitMoney.TryCreate(countryCode, units);

if (result.IsValid && money != null)
{
    // Business rule: prevent overflow in decimal conversion
    try
    {
        var decimalAmount = money.ToDecimal();
        if (decimalAmount > 1000000.00m)
        {
            return BadRequest("Amount cannot exceed maximum allowed");
        }
    }
    catch (OverflowException)
    {
        return BadRequest("Amount calculation overflow");
    }
    
    // Business rule: validate currency decimal places
    var expectedDecimals = money.GetDecimalPlaces();
    var actualValue = money.Value;
    
    // For currencies with 2 decimal places, value should be reasonable
    if (expectedDecimals == 2 && actualValue > 100000000) // 1M in smallest units
    {
        return BadRequest("Amount too large for currency");
    }
    
    // Business rule: prevent zero amounts
    if (money.Value == 0)
    {
        return BadRequest("Amount must be greater than zero");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize small unit money input before validation
public (CountryCode?, uint?) SanitizeSmallUnitMoneyInput(string countryInput, string unitsInput)
{
    CountryCode? countryCode = null;
    uint? units = null;
    
    // Sanitize country code
    if (!string.IsNullOrWhiteSpace(countryInput))
    {
        if (Enum.TryParse<CountryCode>(countryInput.Trim(), out var parsedCountry))
        {
            countryCode = parsedCountry;
        }
    }
    
    // Sanitize units
    if (!string.IsNullOrWhiteSpace(unitsInput))
    {
        // Remove non-numeric characters
        var sanitizedUnits = Regex.Replace(unitsInput, @"[^\d]", "");
        
        if (uint.TryParse(sanitizedUnits, out var parsedUnits))
        {
            // Ensure reasonable bounds to prevent overflow
            if (parsedUnits <= 100000000) // 1M in smallest units
            {
                units = parsedUnits;
            }
        }
    }
    
    return (countryCode, units);
}

// Usage
var (countryCode, units) = SanitizeSmallUnitMoneyInput(countryInput, unitsInput);
if (countryCode.HasValue && units.HasValue)
{
    var (result, money) = SmallUnitMoney.TryCreate(countryCode.Value, units.Value);
}
```

### Logging Financial Data

```csharp
// ? DO: Log small unit money amounts appropriately for privacy and security
public void LogFinancialTransaction(SmallUnitMoney money, string transactionId, string userId)
{
    // For security, consider rounding or masking amounts in logs
    var roundedAmount = Math.Round(money.ToDecimal(), 0); // Round to whole units
    
    _logger.LogInformation(
        "Financial transaction {TransactionId} for user {UserId}: {RoundedAmount} {CurrencyCode}",
        transactionId,
        userId,
        roundedAmount,
        money.GetCurrencyCode()
    );
}

// For detailed audit logs (more secure environment)
public void LogDetailedFinancialTransaction(SmallUnitMoney money, string transactionId, string userId)
{
    // Only log detailed amounts in secure, audited environments
    _logger.LogInformation(
        "Detailed financial transaction {TransactionId} for user {UserId}: {Amount} {CurrencyCode} ({RawUnits} {UnitName})",
        transactionId,
        userId,
        money.ToDecimal(),
        money.GetCurrencyCode(),
        money.Value,
        money.GetSmallestUnitName()
    );
}

// ? DON'T: Log full financial amounts in insecure logs
_logger.LogInformation($"Transaction: {fullAmount} {currencyCode}");  // Avoid
```
