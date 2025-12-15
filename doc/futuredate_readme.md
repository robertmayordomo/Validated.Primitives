# Validated.Primitives.FutureDate

A validated future date primitive that ensures dates are today or in the future, guaranteeing valid future dates when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`FutureDate` is a validated value object that represents a date that must be today or in the future. It ensures the date is not in the past. Once created, a `FutureDate` instance is guaranteed to be valid.

### Key Features

- **Future Date Validation** - Ensures date is today or later
- **Past Date Prevention** - Rejects dates in the past
- **DateTime Support** - Accepts DateTime values or parseable strings
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other DateTime types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Future Date

```csharp
using Validated.Primitives.ValueObjects;

// Create with DateTime (must be today or future)
var futureDate = DateTime.Now.AddDays(7); // 7 days from now
var (result, validatedFutureDate) = FutureDate.TryCreate(futureDate);

if (result.IsValid)
{
    Console.WriteLine(validatedFutureDate.Value);      // Future date
    Console.WriteLine(validatedFutureDate.ToString()); // Future date string
    
    // Use the validated future date
    ProcessFutureDate(validatedFutureDate);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.MemberName}: {error.Message}");
    }
}
```

### Creating from String

```csharp
// Create from string (must be parseable and future date)
var (result, futureDate) = FutureDate.TryCreate("2024-12-31");

if (result.IsValid)
{
    Console.WriteLine(futureDate.Value.ToString("yyyy-MM-dd")); // "2024-12-31"
}
else
{
    // Handle parsing or validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Days Until Calculation

```csharp
var (result, futureDate) = FutureDate.TryCreate(DateTime.Now.AddDays(30));

if (result.IsValid)
{
    // Calculate days until the future date
    var daysUntil = (futureDate.Value.Date - DateTime.Today).Days;
    Console.WriteLine($"Days until: {daysUntil}");
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, futureDate) = FutureDate.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated future date
    ProcessFutureDate(futureDate);
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

### String Parsing Pattern

```csharp
var (result, futureDate) = FutureDate.TryCreate(dateString);

if (result.IsValid)
{
    // Successfully parsed and validated
    var formatted = futureDate.Value.ToString("yyyy-MM-dd");
}
else
{
    // Handle parsing or validation errors
    if (result.Errors.Any(e => e.Code == "InvalidDateString"))
    {
        // Handle invalid date format
    }
    else
    {
        // Handle validation errors (past date, etc.)
    }
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var futureDateTime = DateTime.Now.AddDays(30);
var (_, futureDate) = FutureDate.TryCreate(futureDateTime);

// Serialize
string json = JsonSerializer.Serialize(futureDate);
// {"Value":"2024-01-15T10:30:00"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<FutureDate>(json);
Console.WriteLine(deserialized.Value.ToString("yyyy-MM-dd"));  // "2024-01-15"
```

---

## ?? Related Documentation

- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `DateTime` | The validated future date |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(DateTime value, string propertyName = "FutureDate")` | `(ValidationResult, FutureDate?)` | Static factory method to create validated future date from DateTime |
| `TryCreate(string value, string propertyName = "FutureDate")` | `(ValidationResult, FutureDate?)` | Static factory method to create validated future date from string |

### Validation Rules

| Rule | Description |
|------|-------------|
| **From Today Forward** | Date must be today or in the future (no past dates allowed) |

---

## ?? Date Validation Standards

### Date Constraints

- **Today or Future**: Date must be today or later
- **No Past Dates**: Cannot be yesterday or any past date
- **DateTime Precision**: Supports full DateTime precision

### Common Date Formats

The `TryCreate(string)` method accepts any format parseable by `DateTime.TryParse()`:

- `2024-12-31` (ISO format)
- `12/31/2024` (US format)
- `31/12/2024` (UK format)
- `December 31, 2024` (long format)
- `2024-12-31T23:59:59` (ISO with time)

### Time Considerations

```csharp
// Future date includes time component
var now = DateTime.Now;
var oneMinuteAgo = now.AddMinutes(-1);
var oneMinuteFromNow = now.AddMinutes(1);

// This would fail (past date)
var (result1, pastDate) = FutureDate.TryCreate(oneMinuteAgo);

// This would succeed (future date)
var (result2, futureDate) = FutureDate.TryCreate(oneMinuteFromNow);
```

---

## ??? Security Considerations

### Future Date Validation

```csharp
// ? DO: Validate before use
var (result, futureDate) = FutureDate.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid future date");
}

// ? DON'T: Trust user input without validation
var futureDate = DateTime.Parse(userInput);  // Dangerous!
ProcessFutureDate(futureDate);
```

### Preventing Date Manipulation

```csharp
// ? DO: Validate date ranges for business rules
var (result, futureDate) = FutureDate.TryCreate(userInput);
if (result.IsValid)
{
    var daysInFuture = (futureDate.Value.Date - DateTime.Today).Days;
    
    // Business rule: cannot be more than 1 year in the future
    if (daysInFuture > 365)
    {
        return BadRequest("Date cannot be more than 1 year in the future");
    }
    
    // Business rule: must be at least 1 day in the future
    if (daysInFuture < 1)
    {
        return BadRequest("Date must be at least tomorrow");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize date input before validation
public string SanitizeDateInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Trim whitespace
    input = input.Trim();
    
    // Remove common separators that might cause issues
    // But keep valid date separators
    return input;
}

// Usage
var sanitized = SanitizeDateInput(userInput);
var (result, futureDate) = FutureDate.TryCreate(sanitized);
```

### Logging Date Data

```csharp
// ? DO: Log future dates appropriately
public void LogScheduledEvent(FutureDate futureDate, string eventId)
{
    // Log the future date (less sensitive than personal data)
    _logger.LogInformation(
        "Event {EventId} scheduled for {FutureDate}",
        eventId,
        futureDate.Value.ToString("yyyy-MM-dd")
    );
}

// For sensitive future dates, consider masking
public void LogAppointment(FutureDate appointmentDate, string userId)
{
    // Mask exact time for privacy
    var maskedDate = appointmentDate.Value.ToString("yyyy-MM-dd");
    
    _logger.LogInformation(
        "Appointment scheduled for user {UserId} on {Date}",
        userId,
        maskedDate
    );
}
```
