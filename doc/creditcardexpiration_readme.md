# Validated.Primitives.CreditCardExpiration

A validated credit card expiration date primitive that enforces proper month/year format and expiration validation, ensuring valid expiration dates when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`CreditCardExpiration` is a validated value object that represents a credit card expiration date. It validates that the month is valid (1-12), the year is reasonable, and the card is not expired. The class automatically normalizes two-digit years to four-digit format. Once created, a `CreditCardExpiration` instance is guaranteed to be valid and not expired.

### Key Features

- **Month Validation** - Ensures month is between 1-12
- **Year Validation** - Validates reasonable year ranges
- **Expiration Checking** - Prevents use of expired cards
- **Year Normalization** - Converts 2-digit years to 4-digit format
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other date types

---

## ?? Basic Usage

### Creating an Expiration Date

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, expiration) = CreditCardExpiration.TryCreate(12, 2025);

if (result.IsValid)
{
    Console.WriteLine(expiration.Month);         // 12
    Console.WriteLine(expiration.Year);          // 2025
    Console.WriteLine(expiration.ToString());    // "12/25"
    
    // Use the validated expiration date
    ProcessExpirationDate(expiration);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Year Normalization

```csharp
// Two-digit years are automatically converted to four-digit
var (result1, exp1) = CreditCardExpiration.TryCreate(12, 25);    // 2025
var (result2, exp2) = CreditCardExpiration.TryCreate(12, 2025);  // 2025

if (result1.IsValid && result2.IsValid)
{
    Console.WriteLine(exp1.ToString());  // "12/25"
    Console.WriteLine(exp2.ToString());  // "12/25"
    Console.WriteLine(exp1.Year);        // 2025
    Console.WriteLine(exp2.Year);        // 2025
}
```

### Expiration Checking

```csharp
// Current date for reference
var currentDate = DateTime.Now;

// This would succeed (future expiration)
var (futureResult, futureExp) = CreditCardExpiration.TryCreate(12, 2025);

// This would fail (expired)
var (expiredResult, expiredExp) = CreditCardExpiration.TryCreate(1, 2020);

Console.WriteLine($"Future expiration valid: {futureResult.IsValid}");
Console.WriteLine($"Expired card valid: {expiredResult.IsValid}");
```

---

## ?? Common Patterns

### Expiration Creation Pattern

```csharp
var (result, expiration) = CreditCardExpiration.TryCreate(month, year);

if (result.IsValid)
{
    // Use the validated expiration date
    ProcessExpirationDate(expiration);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Current Date Validation Pattern

```csharp
var (result, expiration) = CreditCardExpiration.TryCreate(month, year);

if (result.IsValid)
{
    // Check if card expires soon (within 3 months)
    var expiryDate = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1).AddDays(-1);
    var threeMonthsFromNow = DateTime.Now.AddMonths(3);
    
    if (expiryDate <= threeMonthsFromNow)
    {
        // Warn user about upcoming expiration
        Console.WriteLine("Card expires soon");
    }
}
```

### Display Formatting Pattern

```csharp
var (result, expiration) = CreditCardExpiration.TryCreate(month, year);

if (result.IsValid)
{
    // Different display formats
    var shortFormat = expiration.ToString();                    // "12/25"
    var longFormat = $"{expiration.Month:D2}/{expiration.Year}"; // "12/2025"
    var readableFormat = $"{GetMonthName(expiration.Month)} {expiration.Year}"; // "December 2025"
}
```

---

## ?? Related Documentation

- [Credit Card README](creditcard_readme.md) - Complete credit card validation including number and security code
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `(int Month, int Year)` | The validated expiration month and year |
| `Month` | `int` | The expiration month (1-12) |
| `Year` | `int` | The expiration year (4-digit) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(int month, int year, string propertyName = "Expiration")` | `(ValidationResult, CreditCardExpiration?)` | Static factory method to create validated expiration date |
| `ToString()` | `string` | Returns formatted string as MM/YY |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Valid Month** | Month must be between 1 and 12 |
| **Valid Year** | Year must be reasonable (not too far in past/future) |
| **Not Expired** | Card must not be expired |

---

## ?? Expiration Date Standards

### Common Formats

Credit card expiration dates are typically displayed as:

- **MM/YY**: 12/25 (most common)
- **MM/YYYY**: 12/2025 (full year)
- **Month Year**: December 2025 (readable)

### Year Handling

The class automatically handles year normalization:

```csharp
// Input -> Normalized
25 -> 2025
2025 -> 2025
95 -> 2095 (assumes 2000s)
2050 -> 2050
```

### Expiration Logic

Cards expire at the end of the expiration month:

```csharp
// Card expires 12/2025
// Valid for all of December 2025
// Expires on 2025-12-31 at 23:59:59

// For validation, a card is considered expired if:
// Current date > last day of expiration month
```

### Business Rules

```csharp
// Common business rules for expiration dates
var (result, expiration) = CreditCardExpiration.TryCreate(month, year);

if (result.IsValid)
{
    var expiryDate = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1).AddDays(-1);
    var now = DateTime.Now;
    
    // Rule 1: Card must be valid for at least 1 month
    if (expiryDate <= now.AddMonths(1))
    {
        // Reject cards expiring too soon
    }
    
    // Rule 2: Card should not expire more than 10 years from now
    if (expiryDate > now.AddYears(10))
    {
        // Reject cards with too distant expiration
    }
    
    // Rule 3: Warn for cards expiring within 3 months
    if (expiryDate <= now.AddMonths(3))
    {
        // Show warning to user
    }
}
```

---

## ??? Security Considerations

### Expiration Date Validation

```csharp
// ? DO: Validate expiration dates before use
var (result, expiration) = CreditCardExpiration.TryCreate(month, year);

if (!result.IsValid)
{
    return BadRequest("Invalid expiration date");
}

// Additional validation
if (expiration != null)
{
    // Business rule: check expiration timeframe
    var expiryDate = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1).AddDays(-1);
    var now = DateTime.Now;
    
    // Reject cards that expire too soon
    if (expiryDate <= now.AddDays(30))
    {
        return BadRequest("Card expires too soon");
    }
    
    // Reject cards with unrealistic expiration
    if (expiryDate > now.AddYears(20))
    {
        return BadRequest("Expiration date too far in future");
    }
    
    // Check for suspicious patterns
    if (IsSuspiciousExpiration(expiration))
    {
        return BadRequest("Expiration date flagged for review");
    }
}

// ? DON'T: Trust user input without validation
var expiryMonth = int.Parse(monthInput);
var expiryYear = int.Parse(yearInput);
ProcessExpiration(expiryMonth, expiryYear);  // Dangerous!
```

### Preventing Expired Card Usage

```csharp
// ? DO: Always check expiration before processing payments
var (result, expiration) = CreditCardExpiration.TryCreate(month, year);

if (result.IsValid)
{
    // Double-check expiration hasn't occurred during processing
    var now = DateTime.Now;
    var expiryDate = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1).AddDays(-1);
    
    if (now > expiryDate)
    {
        return PaymentResult.CardExpired();
    }
    
    // Proceed with payment processing
    return ProcessPayment(cardNumber, expiration, securityCode);
}

// ? DO: Implement grace period handling
public bool IsExpiredWithGracePeriod(CreditCardExpiration expiration, int graceDays = 7)
{
    var now = DateTime.Now;
    var expiryDate = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1).AddDays(-1);
    
    // Allow grace period after official expiration
    return now > expiryDate.AddDays(graceDays);
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize expiration date input before validation
public (int?, int?) SanitizeExpirationInput(string monthInput, string yearInput)
{
    int? month = null;
    int? year = null;
    
    // Sanitize month
    if (!string.IsNullOrWhiteSpace(monthInput))
    {
        if (int.TryParse(monthInput.Trim(), out var parsedMonth))
        {
            if (parsedMonth >= 1 && parsedMonth <= 12)
            {
                month = parsedMonth;
            }
        }
    }
    
    // Sanitize year
    if (!string.IsNullOrWhiteSpace(yearInput))
    {
        if (int.TryParse(yearInput.Trim(), out var parsedYear))
        {
            // Allow 2-digit or 4-digit years
            if (parsedYear >= 0 && parsedYear <= 9999)
            {
                year = parsedYear;
            }
        }
    }
    
    return (month, year);
}

// Usage
var (month, year) = SanitizeExpirationInput(monthInput, yearInput);
if (month.HasValue && year.HasValue)
{
    var (result, expiration) = CreditCardExpiration.TryCreate(month.Value, year.Value);
}
```

### Logging Expiration Data

```csharp
// ? DO: Log expiration dates appropriately
public void LogCardValidation(CreditCardExpiration expiration, string cardLastFour, bool isValid)
{
    // Log expiration month/year without full card details
    _logger.LogInformation(
        "Card validation for ****-****-****-{LastFour}: expires {Expiration}, valid: {IsValid}",
        cardLastFour,
        expiration.ToString(),
        isValid
    );
}

// For security monitoring
public void LogExpirationAnalytics(CreditCardExpiration expiration)
{
    // Track expiration patterns for fraud detection
    var monthsUntilExpiry = GetMonthsUntilExpiry(expiration);
    
    if (monthsUntilExpiry < 1)
    {
        _logger.LogWarning("Attempted use of expired card");
    }
    else if (monthsUntilExpiry < 3)
    {
        _logger.LogInformation("Card expiring soon used in transaction");
    }
}

private int GetMonthsUntilExpiry(CreditCardExpiration expiration)
{
    var expiryDate = new DateTime(expiration.Year, expiration.Month, 1).AddMonths(1).AddDays(-1);
    var now = DateTime.Now;
    return ((expiryDate.Year - now.Year) * 12) + expiryDate.Month - now.Month;
}
```
