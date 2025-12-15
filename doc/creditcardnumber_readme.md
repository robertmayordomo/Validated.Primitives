# Validated.Primitives.CreditCardNumber

A validated credit card number primitive that enforces proper credit card format validation using the Luhn algorithm, ensuring valid credit card numbers when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`CreditCardNumber` is a validated value object that represents a credit card number. It validates the format, digit count, and uses the Luhn algorithm to verify authenticity. The class provides masking functionality for secure display. Once created, a `CreditCardNumber` instance is guaranteed to be valid.

### Key Features

- **Luhn Algorithm** - Validates credit card number authenticity
- **Format Flexibility** - Accepts numbers with spaces, hyphens, or plain digits
- **Digit Count Validation** - Ensures proper length for credit card numbers
- **Security Masking** - Provides masked display showing only last 4 digits
- **Duplicate Prevention** - Rejects numbers with all identical digits
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating a Credit Card Number

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, cardNumber) = CreditCardNumber.TryCreate("4111-1111-1111-1111");

if (result.IsValid)
{
    Console.WriteLine(cardNumber.Value);         // "4111111111111111"
    Console.WriteLine(cardNumber.ToString());    // "4111111111111111"
    Console.WriteLine(cardNumber.Masked());      // "************1111"
    
    // Use the validated credit card number
    ProcessCardNumber(cardNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Different Input Formats

```csharp
// All of these create the same validated credit card number
var formats = new[]
{
    "4111111111111111",      // Plain digits
    "4111 1111 1111 1111",   // Spaces
    "4111-1111-1111-1111",   // Hyphens
    "4111.1111.1111.1111"    // Dots
};

foreach (var format in formats)
{
    var (result, card) = CreditCardNumber.TryCreate(format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {card.Value}");
    }
}
```

### Security Masking

```csharp
var (result, cardNumber) = CreditCardNumber.TryCreate("4111-1111-1111-1111");

if (result.IsValid)
{
    // For secure display
    var masked = cardNumber.Masked();  // "************1111"
    
    // For logging (avoid logging full numbers)
    Console.WriteLine($"Card ending in: {cardNumber.Value[^4..]}");
    
    // For storage (use tokenized version, not the actual number)
    // var token = TokenizeCardNumber(cardNumber.Value);
}
```

---

## ?? Common Patterns

### Credit Card Creation Pattern

```csharp
var (result, cardNumber) = CreditCardNumber.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated credit card number
    ProcessCardNumber(cardNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Secure Display Pattern

```csharp
var (result, cardNumber) = CreditCardNumber.TryCreate(cardInput);

if (result.IsValid)
{
    // Display masked version for user confirmation
    var maskedDisplay = cardNumber.Masked();
    
    // Show last 4 digits for identification
    var lastFour = cardNumber.Value[^4..];
    
    Console.WriteLine($"Card: {maskedDisplay}");
    Console.WriteLine($"Ending in: {lastFour}");
}
```

### Validation Pattern

```csharp
var (result, cardNumber) = CreditCardNumber.TryCreate(cardInput);

if (result.IsValid)
{
    // Additional business validation
    var cardType = DetectCardType(cardNumber.Value);
    
    if (!IsSupportedCardType(cardType))
    {
        // Handle unsupported card type
    }
    
    // Proceed with validated card
}
```

---

## ?? Related Documentation

- [Credit Card README](creditcard_readme.md) - Complete credit card validation including security number and expiration
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated credit card number (digits only) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? input, string propertyName = "CreditCardNumber")` | `(ValidationResult, CreditCardNumber?)` | Static factory method to create validated credit card number |
| `ToString()` | `string` | Returns the credit card number as digits |
| `Masked()` | `string` | Returns masked version showing only last 4 digits |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Empty** | Credit card number must be provided |
| **Valid Digit Count** | Must have valid number of digits (typically 13-19) |
| **Not All Identical** | Cannot have all identical digits |
| **Luhn Check** | Must pass Luhn algorithm validation |

---

## ?? Credit Card Standards

### Supported Card Types

| Card Type | Digit Range | Example |
|-----------|-------------|---------|
| **Visa** | 13, 16, 19 | 4111-1111-1111-1111 |
| **MasterCard** | 16 | 5555-5555-5555-4444 |
| **American Express** | 15 | 3782-822463-10005 |
| **Discover** | 16, 19 | 6011-1111-1111-1117 |
| **JCB** | 15, 16 | 3530-1111-1111-1110 |
| **Diners Club** | 14 | 3056-930902-5904 |

### Luhn Algorithm

The Luhn algorithm (mod 10) is used to validate credit card numbers:

1. Starting from the rightmost digit, double every second digit
2. If doubling results in a number > 9, subtract 9
3. Sum all digits
4. If sum is divisible by 10, the number is valid

```csharp
// Example: 4111-1111-1111-1111
// 4 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
// × × × × × × × × (double every second from right)
// 8 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1
// Sum: 8+1+2+1+2+1+2+1+2+1+2+1+2+1+2+1 = 40 (divisible by 10 ?)
```

### Input Format Handling

The class automatically handles common input formats:

```csharp
// All of these are accepted and normalized to "4111111111111111"
var inputs = new[]
{
    "4111111111111111",      // Plain
    "4111 1111 1111 1111",   // Spaces
    "4111-1111-1111-1111",   // Hyphens
    "4111.1111.1111.1111",   // Dots
    "4111/1111/1111/1111"    // Slashes
};
```

---

## ??? Security Considerations

### Credit Card Validation

```csharp
// ? DO: Validate credit card numbers before use
var (result, cardNumber) = CreditCardNumber.TryCreate(userInput);

if (!result.IsValid)
{
    return BadRequest("Invalid credit card number");
}

// Additional validation
if (cardNumber != null)
{
    // Business rule: check card type support
    var cardType = DetectCardType(cardNumber.Value);
    if (!IsSupportedCardType(cardType))
    {
        return BadRequest("Card type not supported");
    }
    
    // Business rule: validate expiration (would need separate validation)
    // Business rule: check against fraud patterns
    if (IsSuspiciousCardNumber(cardNumber.Value))
    {
        return BadRequest("Card number flagged for security review");
    }
}

// ? DON'T: Trust user input without validation
var cardNumber = userInput.Replace(" ", "").Replace("-", "");  // Dangerous!
ProcessCardNumber(cardNumber);
```

### Preventing Card Number Exposure

```csharp
// ? DO: Never store full credit card numbers
var (result, cardNumber) = CreditCardNumber.TryCreate(cardInput);

if (result.IsValid)
{
    // Use tokenization or encryption instead of storing raw number
    var token = TokenizeCardNumber(cardNumber.Value);
    var lastFour = cardNumber.Value[^4..];
    
    // Store token and last four digits only
    SaveCardToken(token, lastFour, cardType);
    
    // Never store: cardNumber.Value
}

// ? DO: Use masking for display and logging
public void LogCardTransaction(CreditCardNumber cardNumber, string transactionId)
{
    // Only log last 4 digits and card type
    var lastFour = cardNumber.Value[^4..];
    var cardType = DetectCardType(cardNumber.Value);
    
    _logger.LogInformation(
        "Card transaction {TransactionId}: {CardType} ending in {LastFour}",
        transactionId,
        cardType,
        lastFour
    );
}

// ? DON'T: Log full credit card numbers
_logger.LogInformation($"Transaction: {fullCardNumber}");  // Dangerous!
```

### Input Sanitization

```csharp
// ? DO: Sanitize credit card input before validation
public string SanitizeCardNumberInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove all non-digit characters except spaces and hyphens initially
    // But ultimately extract only digits
    var sanitized = new string(input.Where(c => 
        char.IsDigit(c) || c == ' ' || c == '-').ToArray());
    
    // Ensure reasonable length (credit cards are 13-19 digits)
    if (sanitized.Length < 13 || sanitized.Length > 23) // Allow for separators
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeCardNumberInput(userInput);
var (result, cardNumber) = CreditCardNumber.TryCreate(sanitized);
```

### PCI DSS Compliance

```csharp
// ? DO: Follow PCI DSS guidelines for card number handling
public void ProcessCardPayment(CreditCardNumber cardNumber, PaymentRequest request)
{
    // 1. Validate card number format
    if (!result.IsValid)
    {
        return PaymentResult.InvalidCard();
    }
    
    // 2. Never store full card number
    var token = TokenizeForPayment(cardNumber.Value);
    
    // 3. Use secure transmission (HTTPS/TLS)
    var paymentResult = ProcessPaymentWithToken(token, request.Amount);
    
    // 4. Log minimally (last 4 digits only)
    LogPaymentAttempt(cardNumber.Value[^4..], paymentResult.Success);
    
    // 5. Clear sensitive data from memory
    ClearSensitiveData(cardNumber);
}

// ? DO: Implement proper data retention policies
public void CleanupExpiredCardData()
{
    // Remove any temporarily stored card data after processing
    // Implement data retention policies per PCI DSS
    RemoveTemporaryCardData();
}
```
