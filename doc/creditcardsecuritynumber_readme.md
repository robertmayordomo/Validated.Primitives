# Validated.Primitives.CreditCardSecurityNumber

A validated credit card security number (CVV/CVC) primitive that enforces proper security code format and length validation, ensuring valid security codes when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`CreditCardSecurityNumber` is a validated value object that represents a credit card security number (CVV/CVC). It validates the format and ensures proper length (3-4 digits). The class automatically extracts digits from input that may contain formatting. Once created, a `CreditCardSecurityNumber` instance is guaranteed to be valid.

### Key Features

- **Length Validation** - Ensures 3-4 digit security codes
- **Format Flexibility** - Accepts codes with or without formatting
- **Digit Extraction** - Automatically extracts digits from input
- **Security Focused** - Designed for secure handling of sensitive data
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types

---

## ?? Basic Usage

### Creating a Security Number

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate("123");

if (result.IsValid)
{
    Console.WriteLine(securityNumber.Value);      // "123"
    Console.WriteLine(securityNumber.ToString()); // "123"
    
    // Use the validated security number
    ProcessSecurityCode(securityNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Different Input Formats

```csharp
// All of these create the same validated security number
var formats = new[]
{
    "123",      // Plain digits
    " 123 ",    // With spaces
    "123abc",   // Mixed characters (extracts digits)
    "1-2-3"     // With separators (extracts digits)
};

foreach (var format in formats)
{
    var (result, security) = CreditCardSecurityNumber.TryCreate(format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {security.Value}");
    }
}
```

### Card Type Considerations

```csharp
// Different security code lengths for different card types
var visaSecurity = "123";        // 3 digits for Visa
var amexSecurity = "1234";       // 4 digits for American Express

var (visaResult, visaCode) = CreditCardSecurityNumber.TryCreate(visaSecurity);
var (amexResult, amexCode) = CreditCardSecurityNumber.TryCreate(amexSecurity);

if (visaResult.IsValid && amexResult.IsValid)
{
    Console.WriteLine($"Visa CVV: {visaCode.Value}");
    Console.WriteLine($"Amex CID: {amexCode.Value}");
}
```

---

## ?? Common Patterns

### Security Number Creation Pattern

```csharp
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated security number
    ProcessSecurityCode(securityNumber);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Secure Processing Pattern

```csharp
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(securityInput);

if (result.IsValid)
{
    // Process payment with security code
    var paymentResult = ProcessPayment(cardNumber, expiry, securityNumber);
    
    // Clear sensitive data immediately after use
    ClearSecurityCodeFromMemory(securityNumber);
}
```

### Validation Pattern

```csharp
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(securityInput);

if (result.IsValid)
{
    // Additional validation based on card type
    var expectedLength = GetExpectedSecurityCodeLength(cardType);
    if (securityNumber.Value.Length != expectedLength)
    {
        // Handle length mismatch for card type
    }
}
```

---

## ?? Related Documentation

- [Credit Card README](creditcard_readme.md) - Complete credit card validation including number and expiration
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated security number (digits only) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? input, string propertyName = "SecurityNumber")` | `(ValidationResult, CreditCardSecurityNumber?)` | Static factory method to create validated security number |
| `ToString()` | `string` | Returns the security number as digits |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Empty** | Security number must be provided |
| **Valid Length** | Must be 3-4 digits |

---

## ?? Security Code Standards

### Security Code Types

| Card Type | Code Name | Length | Location |
|-----------|-----------|--------|----------|
| **Visa** | CVV2 | 3 digits | Back of card |
| **MasterCard** | CVC2 | 3 digits | Back of card |
| **American Express** | CID | 4 digits | Front of card |
| **Discover** | CID | 3 digits | Back of card |

### Common Lengths

- **3 digits**: Most credit cards (Visa, MasterCard, Discover)
- **4 digits**: American Express and some other cards

### Input Handling

The class automatically handles various input formats:

```csharp
// All of these are accepted and normalized to "123"
var inputs = new[]
{
    "123",          // Plain digits
    " 123 ",        // With whitespace
    "123abc",       // Mixed characters
    "1-2-3",        // With separators
    "CVV: 123",     // With labels
    "123!"          // With special characters
};
```

### Security Best Practices

```csharp
// ? DO: Never store security codes
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(userInput);

if (result.IsValid)
{
    // Use immediately for verification, then discard
    var verificationResult = VerifySecurityCode(cardNumber, securityNumber);
    
    // Clear from memory immediately
    securityNumber = null;
}

// ? DO: Use secure transmission
// Always transmit security codes over HTTPS/TLS

// ? DO: Implement rate limiting
// Limit security code verification attempts
```

---

## ??? Security Considerations

### Security Number Validation

```csharp
// ? DO: Validate security numbers before use
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(userInput);

if (!result.IsValid)
{
    return BadRequest("Invalid security number");
}

// Additional validation
if (securityNumber != null)
{
    // Business rule: validate length matches card type
    var cardType = DetectCardType(cardNumber);
    var expectedLength = GetSecurityCodeLength(cardType);
    
    if (securityNumber.Value.Length != expectedLength)
    {
        return BadRequest($"Security code length invalid for {cardType}");
    }
    
    // Business rule: check for suspicious patterns
    if (IsSuspiciousSecurityCode(securityNumber.Value))
    {
        return BadRequest("Security code flagged for review");
    }
}

// ? DON'T: Trust user input without validation
var securityCode = userInput.Replace(" ", "").Replace("-", "");  // Dangerous!
ProcessSecurityCode(securityCode);
```

### Preventing Security Code Exposure

```csharp
// ? DO: Never store or log security codes
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(securityInput);

if (result.IsValid)
{
    // Use immediately for verification
    var isValid = VerifyPayment(cardNumber, expiry, securityNumber.Value);
    
    // Log success/failure without the security code
    LogPaymentAttempt(cardNumber[^4..], isValid);
    
    // Clear sensitive data immediately
    ClearSensitiveData(securityNumber);
    
    if (isValid)
    {
        return PaymentResult.Success();
    }
}

// ? DO: Implement secure memory handling
public void ClearSensitiveData(CreditCardSecurityNumber securityNumber)
{
    // Clear the object reference
    securityNumber = null;
    
    // Force garbage collection if necessary
    GC.Collect();
    GC.WaitForPendingFinalizers();
}

// ? DON'T: Store security codes in any form
// var storedCode = securityNumber.Value;  // Never do this!

// ? DON'T: Log security codes
_logger.LogInformation($"Payment verification: {securityCode}");  // Dangerous!
```

### Input Sanitization

```csharp
// ? DO: Sanitize security code input before validation
public string SanitizeSecurityCodeInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Extract only digits
    var sanitized = new string(input.Where(char.IsDigit).ToArray());
    
    // Ensure reasonable length (3-4 digits)
    if (sanitized.Length < 3 || sanitized.Length > 4)
    {
        return string.Empty; // Will fail validation
    }
    
    return sanitized;
}

// Usage
var sanitized = SanitizeSecurityCodeInput(userInput);
var (result, securityNumber) = CreditCardSecurityNumber.TryCreate(sanitized);
```

### PCI DSS Compliance

```csharp
// ? DO: Follow PCI DSS guidelines for security code handling
public PaymentResult ProcessSecurePayment(
    CreditCardNumber cardNumber,
    CreditCardExpiration expiry,
    CreditCardSecurityNumber securityNumber)
{
    try
    {
        // 1. Validate all card data
        if (!result.IsValid || !expiryResult.IsValid || !securityResult.IsValid)
        {
            return PaymentResult.ValidationFailed();
        }
        
        // 2. Use security code only for verification
        var verificationResult = VerifyWithPaymentProcessor(
            cardNumber.Value,
            expiry.ToString(),
            securityNumber.Value
        );
        
        // 3. Clear security code immediately after verification
        ClearSecurityCode(securityNumber);
        
        // 4. Never store the security code
        // 5. Use secure transmission (already established)
        
        return verificationResult ? PaymentResult.Success() : PaymentResult.Failed();
    }
    finally
    {
        // Ensure cleanup even if exception occurs
        ClearSecurityCode(securityNumber);
    }
}

private void ClearSecurityCode(CreditCardSecurityNumber securityNumber)
{
    // Clear reference and suggest garbage collection
    securityNumber = null;
    GC.Collect();
}
```
