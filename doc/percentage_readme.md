# Validated.Primitives.Percentage

A validated percentage primitive that represents percentage values (0-100) with configurable decimal precision, ensuring valid percentage calculations when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`Percentage` is a validated value object that represents a percentage value between 0 and 100 with configurable decimal precision (0-3 decimal places). It provides methods for converting to fractions and calculating percentage amounts. Once created, a `Percentage` instance is guaranteed to be valid.

### Key Features

- **Range Validation** - Enforces 0-100 percentage range
- **Decimal Precision** - Configurable decimal places (0-3)
- **Fraction Conversion** - Convert percentage to decimal fraction
- **Amount Calculation** - Calculate percentage of a base value
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other numeric types

---

## ?? Basic Usage

### Creating a Percentage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation (default 0 decimal places)
var (result, percentage) = Percentage.TryCreate(25.0m);

if (result.IsValid)
{
    Console.WriteLine(percentage.Value);         // 25.0
    Console.WriteLine(percentage.DecimalPlaces); // 0
    Console.WriteLine(percentage.ToString());    // "25%"
    Console.WriteLine(percentage.ToFraction());  // 0.25
    
    // Calculate percentage of a value
    decimal baseValue = 200.0m;
    decimal percentageAmount = percentage.Of(baseValue); // 50.0
    
    Console.WriteLine($"{percentage} of {baseValue} is {percentageAmount}");
    
    // Use the validated percentage
    ProcessPercentage(percentage);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Specifying Decimal Precision

```csharp
// Different decimal precision levels
var (r1, p1) = Percentage.TryCreate(25.5m, 1);    // 1 decimal place
var (r2, p2) = Percentage.TryCreate(25.55m, 2);   // 2 decimal places
var (r3, p3) = Percentage.TryCreate(25.555m, 3);  // 3 decimal places

if (r1.IsValid && r2.IsValid && r3.IsValid)
{
    Console.WriteLine(p1.ToString());  // "25.5%"
    Console.WriteLine(p2.ToString());  // "25.55%"
    Console.WriteLine(p3.ToString());  // "25.555%"
}
```

### Percentage Calculations

```csharp
var (result, percentage) = Percentage.TryCreate(15.0m);

if (result.IsValid)
{
    // Convert to fraction
    decimal fraction = percentage.ToFraction();  // 0.15
    
    // Calculate percentage of different values
    decimal[] baseValues = { 100.0m, 500.0m, 1000.0m };
    
    foreach (var baseValue in baseValues)
    {
        decimal amount = percentage.Of(baseValue);
        Console.WriteLine($"{percentage} of {baseValue} = {amount}");
    }
}
```

---

## ?? Common Patterns

### Percentage Creation Pattern

```csharp
var (result, percentage) = Percentage.TryCreate(value, decimalPlaces);

if (result.IsValid)
{
    // Use the validated percentage
    ProcessPercentage(percentage);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Fraction Conversion Pattern

```csharp
var (result, percentage) = Percentage.TryCreate(percentageValue);

if (result.IsValid)
{
    // Convert to fraction for calculations
    decimal fraction = percentage.ToFraction();
    
    // Use fraction in mathematical operations
    decimal result = baseValue * fraction;
}
```

### Amount Calculation Pattern

```csharp
var (result, percentage) = Percentage.TryCreate(percentageValue);

if (result.IsValid)
{
    // Calculate percentage amount directly
    decimal percentageAmount = percentage.Of(baseValue);
    
    // Or calculate remaining amount
    decimal remainingAmount = baseValue - percentageAmount;
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
| `Value` | `decimal` | The percentage value (0-100) |
| `DecimalPlaces` | `int` | The number of decimal places (0-3) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(decimal value, int decimalPlaces = 0, string propertyName = "Percentage")` | `(ValidationResult, Percentage?)` | Static factory method to create validated percentage |
| `ToFraction()` | `decimal` | Converts percentage to decimal fraction (e.g., 50% ? 0.5) |
| `Of(decimal baseValue)` | `decimal` | Calculates percentage amount of a base value |
| `ToString()` | `string` | Returns formatted string with percent symbol |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Range** | Must be between 0 and 100 (inclusive) |
| **Decimal Places** | Must match specified precision (0-3) |

---

## ?? Percentage Standards

### Common Percentage Ranges

| Use Case | Typical Range | Example |
|----------|---------------|---------|
| Tax Rates | 0-50% | 8.25%, 20.0% |
| Discount Rates | 0-90% | 10.0%, 25.5% |
| Interest Rates | 0-30% | 4.5%, 12.75% |
| Completion Status | 0-100% | 75.0%, 100.0% |
| Error Rates | 0-10% | 0.5%, 2.1% |

### Decimal Precision Levels

| Decimal Places | Use Case | Example |
|----------------|----------|---------|
| 0 | Whole percentages | 25% |
| 1 | Standard rates | 8.5% |
| 2 | Precise rates | 7.25% |
| 3 | High precision | 4.875% |

### Percentage Calculations

```csharp
// Basic percentage calculations
var (result, percentage) = Percentage.TryCreate(25.0m);

if (result.IsValid)
{
    // Calculate percentage of a value
    decimal amount = percentage.Of(200.0m);  // 50.0
    
    // Calculate what percentage a value is of another
    // (This would require additional calculation)
    decimal part = 50.0m;
    decimal whole = 200.0m;
    decimal calculatedPercentage = (part / whole) * 100;  // 25.0
}
```

---

## ??? Security Considerations

### Percentage Validation

```csharp
// ? DO: Validate percentage values before use
var (result, percentage) = Percentage.TryCreate(userInput, decimalPlaces);

if (!result.IsValid)
{
    return BadRequest("Invalid percentage value");
}

// Additional validation
if (percentage != null)
{
    // Business rule: percentage cannot be negative
    if (percentage.Value < 0)
    {
        return BadRequest("Percentage cannot be negative");
    }
    
    // Business rule: percentage cannot exceed 100%
    if (percentage.Value > 100)
    {
        return BadRequest("Percentage cannot exceed 100%");
    }
    
    // Business rule: restrict decimal precision
    if (percentage.DecimalPlaces > 2)
    {
        return BadRequest("Percentage precision too high");
    }
    
    // Business rule: validate against allowed ranges
    var minAllowed = 0.0m;
    var maxAllowed = GetMaxAllowedPercentage(context);
    if (percentage.Value < minAllowed || percentage.Value > maxAllowed)
    {
        return BadRequest($"Percentage must be between {minAllowed}% and {maxAllowed}%");
    }
}

// ? DON'T: Trust user input without validation
var percentage = decimal.Parse(userInput) / 100;  // Dangerous!
ProcessPercentage(percentage);
```

### Preventing Calculation Manipulation

```csharp
// ? DO: Enforce business rule constraints on percentage calculations
var (result, percentage) = Percentage.TryCreate(percentageValue);

if (result.IsValid && percentage != null)
{
    // Business rule: prevent overflow in calculations
    try
    {
        var baseValue = GetBaseValue();
        var calculatedAmount = percentage.Of(baseValue);
        
        // Check for reasonable bounds
        if (calculatedAmount > GetMaxAllowedAmount())
        {
            return BadRequest("Calculated amount exceeds maximum allowed");
        }
        
        if (calculatedAmount < GetMinAllowedAmount())
        {
            return BadRequest("Calculated amount below minimum allowed");
        }
    }
    catch (OverflowException)
    {
        return BadRequest("Percentage calculation overflow");
    }
    
    // Business rule: validate percentage is reasonable for context
    if (percentage.Value > GetReasonableMaxPercentage(context))
    {
        return BadRequest("Percentage seems unreasonably high");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize percentage input before validation
public (decimal?, int?) SanitizePercentageInput(string valueInput, string precisionInput)
{
    decimal? percentageValue = null;
    int? decimalPlaces = null;
    
    // Sanitize percentage value
    if (!string.IsNullOrWhiteSpace(valueInput))
    {
        // Remove percent symbol and extra characters
        var sanitizedValue = Regex.Replace(valueInput, @"[^\d.,-]", "");
        
        if (decimal.TryParse(sanitizedValue, out var parsedValue))
        {
            // Ensure reasonable bounds
            if (parsedValue >= -100 && parsedValue <= 1000) // Allow some overflow for validation
            {
                percentageValue = parsedValue;
            }
        }
    }
    
    // Sanitize decimal places
    if (!string.IsNullOrWhiteSpace(precisionInput))
    {
        if (int.TryParse(precisionInput.Trim(), out var parsedPrecision))
        {
            // Ensure valid range
            if (parsedPrecision >= 0 && parsedPrecision <= 3)
            {
                decimalPlaces = parsedPrecision;
            }
        }
    }
    
    return (percentageValue, decimalPlaces);
}

// Usage
var (percentageValue, decimalPlaces) = SanitizePercentageInput(valueInput, precisionInput);
if (percentageValue.HasValue)
{
    var precision = decimalPlaces ?? 0;
    var (result, percentage) = Percentage.TryCreate(percentageValue.Value, precision);
}
```

### Logging Percentage Data

```csharp
// ? DO: Log percentage values appropriately
public void LogPercentageOperation(Percentage percentage, string operation, string userId)
{
    // Log the percentage operation
    _logger.LogInformation(
        "Percentage operation {Operation} by user {UserId}: {Percentage}",
        operation,
        userId,
        percentage.ToString()
    );
}

// For sensitive percentage calculations
public void LogSensitivePercentage(Percentage percentage, string context, string userId)
{
    // Round for privacy in logs
    var roundedValue = Math.Round(percentage.Value, 1);
    
    _logger.LogInformation(
        "Sensitive percentage calculation for user {UserId} in {Context}: {RoundedPercentage}%",
        userId,
        context,
        roundedValue
    );
}

// ? DON'T: Log percentage values that might reveal sensitive business data
_logger.LogInformation($"Discount applied: {fullPercentage}%");  // May reveal pricing strategy
```
