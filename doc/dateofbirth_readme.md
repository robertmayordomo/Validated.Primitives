# Validated.Primitives.DateOfBirth

A validated date of birth primitive that ensures dates are in the past and not in the future, guaranteeing valid birth dates when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`DateOfBirth` is a validated value object that represents a date of birth. It ensures the date is in the past (before today) and cannot be a future date. Once created, a `DateOfBirth` instance is guaranteed to be valid.

### Key Features

- **Past Date Validation** - Ensures date is before today
- **Future Date Prevention** - Rejects dates in the future
- **DateTime Support** - Accepts DateTime values or parseable strings
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other DateTime types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Date of Birth

```csharp
using Validated.Primitives.ValueObjects;

// Create with DateTime
var birthDate = new DateTime(1990, 5, 15);
var (result, dateOfBirth) = DateOfBirth.TryCreate(birthDate);

if (result.IsValid)
{
    Console.WriteLine(dateOfBirth.Value);      // 5/15/1990 12:00:00 AM
    Console.WriteLine(dateOfBirth.ToString()); // 5/15/1990 12:00:00 AM
    
    // Use the validated date of birth
    ProcessBirthDate(dateOfBirth);
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
// Create from string (must be parseable)
var (result, dateOfBirth) = DateOfBirth.TryCreate("1990-05-15");

if (result.IsValid)
{
    Console.WriteLine(dateOfBirth.Value.ToString("yyyy-MM-dd")); // "1990-05-15"
}
else
{
    // Handle parsing or validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Age Calculation

```csharp
var (result, dateOfBirth) = DateOfBirth.TryCreate(new DateTime(1990, 5, 15));

if (result.IsValid)
{
    // Calculate age
    var today = DateTime.Today;
    var age = today.Year - dateOfBirth.Value.Year;
    
    // Adjust if birthday hasn't occurred this year
    if (dateOfBirth.Value.Date > today.AddYears(-age))
    {
        age--;
    }
    
    Console.WriteLine($"Age: {age}");
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, dateOfBirth) = DateOfBirth.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated date of birth
    ProcessBirthDate(dateOfBirth);
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
var (result, dateOfBirth) = DateOfBirth.TryCreate(dateString);

if (result.IsValid)
{
    // Successfully parsed and validated
    var formatted = dateOfBirth.Value.ToString("yyyy-MM-dd");
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
        // Handle validation errors (future date, etc.)
    }
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var birthDate = new DateTime(1990, 5, 15);
var (_, dateOfBirth) = DateOfBirth.TryCreate(birthDate);

// Serialize
string json = JsonSerializer.Serialize(dateOfBirth);
// {"Value":"1990-05-15T00:00:00"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<DateOfBirth>(json);
Console.WriteLine(deserialized.Value.ToString("yyyy-MM-dd"));  // "1990-05-15"
```

---

## ?? Related Documentation

- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `DateTime` | The validated date of birth |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(DateTime value, string propertyName = "DateOfBirth")` | `(ValidationResult, DateOfBirth?)` | Static factory method to create validated date of birth from DateTime |
| `TryCreate(string value, string propertyName = "DateOfBirth")` | `(ValidationResult, DateOfBirth?)` | Static factory method to create validated date of birth from string |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Before Today** | Date must be before the current date (no future dates allowed) |

---

## ?? Date Validation Standards

### Date Constraints

- **Past Only**: Date must be before today
- **No Future Dates**: Cannot be today or any future date
- **DateTime Precision**: Supports full DateTime precision but typically used for dates only

### Common Date Formats

The `TryCreate(string)` method accepts any format parseable by `DateTime.TryParse()`:

- `1990-05-15` (ISO format)
- `05/15/1990` (US format)
- `15/05/1990` (UK format)
- `May 15, 1990` (long format)
- `1990-05-15T00:00:00` (ISO with time)

### Age Considerations

```csharp
// Calculate age from DateOfBirth
public int CalculateAge(DateOfBirth dateOfBirth)
{
    var today = DateTime.Today;
    var age = today.Year - dateOfBirth.Value.Year;
    
    // Adjust if birthday hasn't occurred this year
    if (dateOfBirth.Value.Date > today.AddYears(-age))
    {
        age--;
    }
    
    return age;
}

// Check if person is of legal age
public bool IsOfLegalAge(DateOfBirth dateOfBirth, int legalAge = 18)
{
    return CalculateAge(dateOfBirth) >= legalAge;
}
```

---

## ??? Security Considerations

### Date of Birth Validation

```csharp
// ? DO: Validate before use
var (result, dateOfBirth) = DateOfBirth.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid date of birth");
}

// ? DON'T: Trust user input without validation
var dateOfBirth = DateTime.Parse(userInput);  // Dangerous!
ProcessBirthDate(dateOfBirth);
```

### Preventing Age Manipulation

```csharp
// ? DO: Validate age ranges for business rules
var (result, dateOfBirth) = DateOfBirth.TryCreate(userInput);
if (result.IsValid)
{
    var age = CalculateAge(dateOfBirth);
    
    // Business rule: must be at least 13 years old
    if (age < 13)
    {
        return BadRequest("Must be at least 13 years old");
    }
    
    // Business rule: cannot be more than 150 years old
    if (age > 150)
    {
        return BadRequest("Invalid age");
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
var (result, dateOfBirth) = DateOfBirth.TryCreate(sanitized);
```

### Logging Personal Data

```csharp
// ? DO: Log dates of birth appropriately for privacy
public void LogUserRegistration(DateOfBirth dateOfBirth, string userId)
{
    // For privacy, consider logging age instead of exact date
    var age = CalculateAge(dateOfBirth);
    
    _logger.LogInformation(
        "User {UserId} registered with age {Age}",
        userId,
        age
    );
}

// Or mask the date
public void LogUserDetails(DateOfBirth dateOfBirth, string userId)
{
    // Mask day and month: 1990-05-15 -> 1990-XX-XX
    var maskedDob = $"{dateOfBirth.Value.Year}-XX-XX";
    
    _logger.LogInformation(
        "User {UserId} DOB: {MaskedDob}",
        userId,
        maskedDob
    );
}

// ? DON'T: Log full dates of birth
_logger.LogInformation($"User DOB: {fullDateOfBirth}");  // Avoid
```
