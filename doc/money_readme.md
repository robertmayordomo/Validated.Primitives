# Validated.Primitives.Money

A validated monetary value primitive that represents amounts with currency codes, ensuring valid financial amounts when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`Money` is a validated value object that represents a monetary amount with an associated currency code. It enforces non-negative values and proper decimal precision (2 decimal places). Once created, a `Money` instance is guaranteed to be valid.

### Key Features

- **Currency Support** - ISO 4217 currency codes (USD, EUR, GBP, etc.)
- **Country Mapping** - Automatic currency mapping from country codes
- **Decimal Precision** - Enforces 2 decimal places for currency amounts
- **Non-negative Values** - Prevents negative monetary amounts
- **Currency Symbols** - Provides currency symbols for display
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other numeric types

---

## ?? Basic Usage

### Creating Money with Currency Code

```csharp
using Validated.Primitives.ValueObjects;

// Create with currency code
var (result, money) = Money.TryCreate("USD", 29.99m);

if (result.IsValid)
{
    Console.WriteLine(money.Value);              // 29.99
    Console.WriteLine(money.CurrencyCode);       // "USD"
    Console.WriteLine(money.GetCurrencySymbol()); // "$"
    Console.WriteLine(money.ToString());         // "$29.99"
    Console.WriteLine(money.ToStringWithCode()); // "29.99 USD"
    
    // Use the validated money
    ProcessPayment(money);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Creating Money with Country Code

```csharp
// Create with country code (automatically maps to currency)
var (result, money) = Money.TryCreate(CountryCode.UnitedStates, 29.99m);

if (result.IsValid)
{
    Console.WriteLine(money.CurrencyCode);       // "USD"
    Console.WriteLine(money.GetCurrencySymbol()); // "$"
}
```

### Different Currencies

```csharp
// Different currency examples
var currencies = new[]
{
    ("USD", 29.99m),    // US Dollar
    ("EUR", 25.50m),    // Euro
    ("GBP", 22.75m),    // British Pound
    ("JPY", 3500m),     // Japanese Yen
    ("CAD", 39.99m)     // Canadian Dollar
};

foreach (var (currencyCode, amount) in currencies)
{
    var (result, money) = Money.TryCreate(currencyCode, amount);
    if (result.IsValid)
    {
        Console.WriteLine($"{money.ToStringWithCode()}");
    }
}
```

---

## ?? Common Patterns

### Currency Code Creation Pattern

```csharp
var (result, money) = Money.TryCreate(currencyCode, amount);

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

### Country Code Creation Pattern

```csharp
var (result, money) = Money.TryCreate(countryCode, amount);

if (result.IsValid)
{
    // Use the validated money with automatically mapped currency
    ProcessMoney(money);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Currency Conversion Preparation Pattern

```csharp
var (result, money) = Money.TryCreate("USD", 100.00m);

if (result.IsValid)
{
    // Prepare for currency conversion
    var baseCurrency = money.CurrencyCode;
    var baseAmount = money.Value;
    
    // Convert to target currency
    var convertedAmount = ConvertCurrency(baseAmount, baseCurrency, targetCurrency);
    var (convertedResult, convertedMoney) = Money.TryCreate(targetCurrency, convertedAmount);
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
| `Value` | `decimal` | The monetary amount |
| `CurrencyCode` | `string` | The ISO 4217 currency code |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string currencyCode, decimal value, string propertyName = "Money")` | `(ValidationResult, Money?)` | Static factory method to create validated money with currency code |
| `TryCreate(CountryCode countryCode, decimal value, string propertyName = "Money")` | `(ValidationResult, Money?)` | Static factory method to create validated money with country code |
| `GetCurrencySymbol()` | `string` | Returns the currency symbol for the currency code |
| `ToString()` | `string` | Returns formatted string with currency symbol |
| `ToStringWithCode()` | `string` | Returns formatted string with currency code |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Non-negative** | Monetary amount cannot be negative |
| **Decimal Places** | Must have exactly 2 decimal places |

---

## ?? Currency Standards

### Supported Currencies

The `Money` type supports all ISO 4217 currency codes:

#### Major World Currencies
- **USD** - United States Dollar ($)
- **EUR** - Euro (€)
- **GBP** - British Pound Sterling (£)
- **JPY** - Japanese Yen (¥)
- **CAD** - Canadian Dollar (C$)
- **AUD** - Australian Dollar (A$)
- **CHF** - Swiss Franc (Fr)
- **CNY** - Chinese Yuan (¥)

#### Country-Specific Currencies
- **BRL** - Brazilian Real (R$)
- **INR** - Indian Rupee (?)
- **KRW** - South Korean Won (?)
- **MXN** - Mexican Peso ($)
- **RUB** - Russian Ruble (?)
- **ZAR** - South African Rand (R)

### Currency Mapping

Country codes are automatically mapped to their primary currencies:

```csharp
// Examples of country to currency mapping
var mappings = new[]
{
    (CountryCode.UnitedStates, "USD"),
    (CountryCode.UnitedKingdom, "GBP"),
    (CountryCode.Germany, "EUR"),
    (CountryCode.Japan, "JPY"),
    (CountryCode.Canada, "CAD"),
    (CountryCode.Australia, "AUD"),
    (CountryCode.Brazil, "BRL"),
    (CountryCode.India, "INR")
};
```

### Decimal Precision

Money values use exactly 2 decimal places:

```csharp
// Valid: exactly 2 decimal places
var (r1, m1) = Money.TryCreate("USD", 29.99m);    // ? Valid
var (r2, m2) = Money.TryCreate("USD", 29.9m);     // Will be validated to 29.90
var (r3, m3) = Money.TryCreate("USD", 30.00m);    // ? Valid

// Invalid: more or less than 2 decimal places
var (r4, m4) = Money.TryCreate("USD", 29.999m);   // ? Too many decimals
var (r5, m5) = Money.TryCreate("USD", 29.9m);     // ? Too few decimals (will be padded)
```

---

## ??? Security Considerations

### Money Validation

```csharp
// ? DO: Validate money amounts before use
var (result, money) = Money.TryCreate(currencyCode, amount);

if (!result.IsValid)
{
    return BadRequest("Invalid money amount");
}

// Additional validation
if (money != null)
{
    // Business rule: maximum transaction amount
    if (money.Value > 10000.00m)
    {
        return BadRequest("Transaction amount cannot exceed $10,000");
    }
    
    // Business rule: minimum transaction amount
    if (money.Value < 0.01m)
    {
        return BadRequest("Transaction amount must be at least $0.01");
    }
    
    // Business rule: only allow specific currencies
    var allowedCurrencies = new[] { "USD", "EUR", "GBP" };
    if (!allowedCurrencies.Contains(money.CurrencyCode))
    {
        return BadRequest("Currency not supported");
    }
}

// ? DON'T: Trust user input without validation
var amount = decimal.Parse(amountInput);
var money = new Money(amount, currencyCode);  // Dangerous!
```

### Preventing Financial Manipulation

```csharp
// ? DO: Enforce business rule constraints on money values
var (result, money) = Money.TryCreate(currencyCode, amount);

if (result.IsValid && money != null)
{
    // Business rule: prevent negative amounts
    if (money.Value < 0)
    {
        return BadRequest("Amount cannot be negative");
    }
    
    // Business rule: prevent amounts that are too large
    var maxAmount = GetMaxAllowedAmount(money.CurrencyCode);
    if (money.Value > maxAmount)
    {
        return BadRequest($"Amount cannot exceed {maxAmount} {money.CurrencyCode}");
    }
    
    // Business rule: prevent amounts that are too small
    var minAmount = GetMinAllowedAmount(money.CurrencyCode);
    if (money.Value < minAmount)
    {
        return BadRequest($"Amount must be at least {minAmount} {money.CurrencyCode}");
    }
    
    // Business rule: validate currency is active/tradeable
    if (!IsCurrencyActive(money.CurrencyCode))
    {
        return BadRequest("Currency is not currently supported");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize money input before validation
public (string?, decimal?) SanitizeMoneyInput(string currencyInput, string amountInput)
{
    string? currencyCode = null;
    decimal? amount = null;
    
    // Sanitize currency code
    if (!string.IsNullOrWhiteSpace(currencyInput))
    {
        currencyCode = currencyInput.Trim().ToUpperInvariant();
        // Ensure only valid characters for currency codes
        if (!Regex.IsMatch(currencyCode, @"^[A-Z]{3}$"))
        {
            currencyCode = null;
        }
    }
    
    // Sanitize amount
    if (!string.IsNullOrWhiteSpace(amountInput))
    {
        // Remove currency symbols and extra characters
        var sanitizedAmount = Regex.Replace(amountInput, @"[^\d.,-]", "");
        
        if (decimal.TryParse(sanitizedAmount, out var parsedAmount))
        {
            // Ensure reasonable bounds
            if (parsedAmount >= -1000000 && parsedAmount <= 1000000)
            {
                amount = parsedAmount;
            }
        }
    }
    
    return (currencyCode, amount);
}

// Usage
var (currencyCode, amount) = SanitizeMoneyInput(currencyInput, amountInput);
if (currencyCode != null && amount.HasValue)
{
    var (result, money) = Money.TryCreate(currencyCode, amount.Value);
}
```

### Logging Financial Data

```csharp
// ? DO: Log money amounts appropriately for privacy and security
public void LogFinancialTransaction(Money money, string transactionId, string userId)
{
    // For security, consider masking or rounding amounts in logs
    var roundedAmount = Math.Round(money.Value, 0); // Round to whole dollars
    
    _logger.LogInformation(
        "Financial transaction {TransactionId} for user {UserId}: {RoundedAmount} {CurrencyCode}",
        transactionId,
        userId,
        roundedAmount,
        money.CurrencyCode
    );
}

// For detailed audit logs (more secure environment)
public void LogDetailedFinancialTransaction(Money money, string transactionId, string userId)
{
    // Only log detailed amounts in secure, audited environments
    _logger.LogInformation(
        "Detailed financial transaction {TransactionId} for user {UserId}: {Amount} {CurrencyCode}",
        transactionId,
        userId,
        money.Value,
        money.CurrencyCode
    );
}

// ? DON'T: Log full financial amounts in insecure logs
_logger.LogInformation($"Transaction: {fullAmount} {currencyCode}");  // Avoid
```
