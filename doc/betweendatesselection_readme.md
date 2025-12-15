# Validated.Primitives.BetweenDatesSelection

A validated date selection primitive that ensures dates fall within a specified range, guaranteeing valid date selections when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`BetweenDatesSelection` is a validated value object that represents a date that must fall within a specified date range. It ensures the date is between the start and end dates of the range. Once created, a `BetweenDatesSelection` instance is guaranteed to be valid.

### Key Features

- **Range Validation** - Ensures date falls within specified range
- **Inclusive/Exclusive Bounds** - Configurable boundary inclusion
- **DateTime Support** - Accepts DateTime values or parseable strings
- **Range Object** - Maintains reference to the validation range
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other DateTime types

---

## ?? Basic Usage

### Creating with DateRange

```csharp
using Validated.Primitives.ValueObjects;
using Validated.Primitives.DateRanges;

// Define a date range
var (rangeResult, dateRange) = DateRange.TryCreate(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31)
);

if (rangeResult.IsValid && dateRange != null)
{
    // Create date selection within range
    var selectedDate = new DateTime(2024, 6, 15);
    var (result, dateSelection) = BetweenDatesSelection.TryCreate(selectedDate, dateRange);

    if (result.IsValid)
    {
        Console.WriteLine(dateSelection.Value);      // 6/15/2024 12:00:00 AM
        Console.WriteLine(dateSelection.Range);      // DateRange object
        Console.WriteLine(dateSelection.ToString()); // 6/15/2024 12:00:00 AM
        
        // Use the validated date selection
        ProcessDateSelection(dateSelection);
    }
    else
    {
        // Handle validation errors
        Console.WriteLine(result.ToBulletList());
    }
}
```

### Creating with Date Bounds

```csharp
// Create directly with date bounds
var fromDate = new DateTime(2024, 1, 1);
var toDate = new DateTime(2024, 12, 31);
var selectedDate = new DateTime(2024, 6, 15);

var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    selectedDate,
    fromDate,
    toDate,
    inclusive: true  // Include boundary dates
);

if (result.IsValid)
{
    Console.WriteLine($"Selected date {dateSelection.Value:d} is within range");
}
```

### Creating from String

```csharp
// Create from string with date bounds
var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    "2024-06-15",
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31)
);

if (result.IsValid)
{
    Console.WriteLine(dateSelection.Value.ToString("yyyy-MM-dd")); // "2024-06-15"
}
```

### Exclusive Boundaries

```csharp
// Create with exclusive boundaries (range not including start/end dates)
var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    new DateTime(2024, 6, 15),
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31),
    inclusive: false  // Exclude boundary dates
);

if (result.IsValid)
{
    // Date must be > 2024-01-01 and < 2024-12-31
    Console.WriteLine("Date is strictly between boundaries");
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    selectedDate,
    rangeStart,
    rangeEnd
);

if (result.IsValid)
{
    // Use the validated date selection
    ProcessDateSelection(dateSelection);
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

### Range Definition Pattern

```csharp
// Define range first, then validate selections
var (rangeResult, bookingRange) = DateRange.TryCreate(
    DateTime.Today.AddDays(1),      // Tomorrow
    DateTime.Today.AddDays(30)      // 30 days from now
);

if (rangeResult.IsValid && bookingRange != null)
{
    // Now validate user selection
    var (selectionResult, dateSelection) = BetweenDatesSelection.TryCreate(
        userSelectedDate,
        bookingRange
    );
    
    if (selectionResult.IsValid)
    {
        // Valid booking date
    }
}
```

### String Parsing Pattern

```csharp
var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    dateString,
    rangeStart,
    rangeEnd
);

if (result.IsValid)
{
    // Successfully parsed and validated
    var formatted = dateSelection.Value.ToString("yyyy-MM-dd");
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
        // Handle range validation errors
    }
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
| `Value` | `DateTime` | The validated date within the range |
| `Range` | `DateRange` | The date range used for validation |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(DateTime value, DateRange range, string propertyName = "BetweenDatesSelection")` | `(ValidationResult, BetweenDatesSelection?)` | Static factory method to create validated date selection with DateRange |
| `TryCreate(string value, DateRange range, string propertyName = "BetweenDatesSelection")` | `(ValidationResult, BetweenDatesSelection?)` | Static factory method to create validated date selection from string with DateRange |
| `TryCreate(DateTime value, DateTime from, DateTime to, bool inclusive = true, string propertyName = "BetweenDatesSelection")` | `(ValidationResult, BetweenDatesSelection?)` | Static factory method to create validated date selection with date bounds |
| `TryCreate(string value, DateTime from, DateTime to, bool inclusive = true, string propertyName = "BetweenDatesSelection")` | `(ValidationResult, BetweenDatesSelection?)` | Static factory method to create validated date selection from string with date bounds |
| `Validate(DateTime value, DateRange range, string propertyName = "BetweenDatesSelection")` | `ValidationResult` | Static validation method without creating instance |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Between** | Date must fall within the specified date range (inclusive or exclusive based on configuration) |

---

## ?? Date Range Standards

### Boundary Inclusion

- **Inclusive (default)**: `from <= date <= to`
- **Exclusive**: `from < date < to`

### Range Validation

```csharp
// Inclusive range: date can be on boundaries
var range = DateRange.Create(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));
// Valid: 2024-01-01, 2024-06-15, 2024-12-31

// Exclusive range: date must be strictly between boundaries
var exclusiveRange = DateRange.Create(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31), false, false);
// Valid: 2024-01-02 through 2024-12-30
// Invalid: 2024-01-01, 2024-12-31
```

### Common Date Formats

The `TryCreate(string)` method accepts any format parseable by `DateTime.TryParse()`:

- `2024-06-15` (ISO format)
- `06/15/2024` (US format)
- `15/06/2024` (UK format)
- `June 15, 2024` (long format)
- `2024-06-15T14:30:00` (ISO with time)

---

## ??? Security Considerations

### Date Range Validation

```csharp
// ? DO: Validate date selections within allowed ranges
var allowedStart = DateTime.Today.AddDays(1);
var allowedEnd = DateTime.Today.AddDays(90);

var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    userSelectedDate,
    allowedStart,
    allowedEnd
);

if (!result.IsValid)
{
    return BadRequest("Selected date is outside allowed range");
}

// ? DON'T: Trust user input without range validation
var selectedDate = DateTime.Parse(userInput);  // Dangerous!
ProcessDateSelection(selectedDate);
```

### Preventing Date Manipulation

```csharp
// ? DO: Enforce business rule date ranges
var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    userInput,
    businessStartDate,
    businessEndDate
);

if (result.IsValid)
{
    // Additional business rule validation
    var daysFromStart = (dateSelection.Value - businessStartDate).Days;
    
    // Business rule: cannot select dates too close to start
    if (daysFromStart < 7)
    {
        return BadRequest("Date must be at least 7 days from start");
    }
    
    // Business rule: cannot select weekend dates
    if (dateSelection.Value.DayOfWeek == DayOfWeek.Saturday ||
        dateSelection.Value.DayOfWeek == DayOfWeek.Sunday)
    {
        return BadRequest("Weekend dates not allowed");
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
var (result, dateSelection) = BetweenDatesSelection.TryCreate(
    sanitized,
    rangeStart,
    rangeEnd
);
```

### Logging Date Selections

```csharp
// ? DO: Log date selections appropriately
public void LogDateSelection(BetweenDatesSelection dateSelection, string userId)
{
    // Log the selected date and range context
    _logger.LogInformation(
        "User {UserId} selected date {SelectedDate} within range {StartDate} to {EndDate}",
        userId,
        dateSelection.Value.ToString("yyyy-MM-dd"),
        dateSelection.Range.Start.ToString("yyyy-MM-dd"),
        dateSelection.Range.End.ToString("yyyy-MM-dd")
    );
}

// For privacy-sensitive selections, consider masking
public void LogAppointmentSelection(BetweenDatesSelection dateSelection, string userId)
{
    // Mask exact date for privacy
    var maskedDate = dateSelection.Value.ToString("yyyy-MM-**");
    
    _logger.LogInformation(
        "Appointment slot selected for user {UserId} on {MaskedDate}",
        userId,
        maskedDate
    );
}
```
