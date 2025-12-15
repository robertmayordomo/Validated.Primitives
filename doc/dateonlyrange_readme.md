# Validated.Primitives.DateOnlyRange

A validated date-only range primitive that represents a range between two dates using the DateOnly type with configurable boundary inclusion, ensuring valid date ranges when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`DateOnlyRange` is a validated value object that represents a range between two dates using the `DateOnly` type. It supports configurable boundary inclusion (inclusive or exclusive) and provides methods for checking if dates fall within the range. Once created, a `DateOnlyRange` instance is guaranteed to be valid.

### Key Features

- **DateOnly Type** - Uses .NET DateOnly for date-only precision
- **Boundary Configuration** - Configurable inclusive/exclusive start and end
- **Date Containment** - Check if dates fall within the range
- **Validation** - Ensures 'from' date is not after 'to' date
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with DateTime ranges

---

## ?? Basic Usage

### Creating a DateOnly Range

```csharp
using Validated.Primitives.DateRanges;

// Create with DateOnly values
var fromDate = new DateOnly(2024, 1, 1);
var toDate = new DateOnly(2024, 12, 31);

var (result, dateRange) = DateOnlyRange.TryCreate(fromDate, toDate);

if (result.IsValid && dateRange != null)
{
    Console.WriteLine(dateRange.From);           // 1/1/2024
    Console.WriteLine(dateRange.To);             // 12/31/2024
    Console.WriteLine(dateRange.InclusiveStart); // true
    Console.WriteLine(dateRange.InclusiveEnd);   // true
    Console.WriteLine(dateRange.ToString());     // "[2024-01-01 .. 2024-12-31]"
    
    // Use the validated date range
    ProcessDateRange(dateRange);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Exclusive Boundaries

```csharp
// Create with exclusive boundaries
var (result, dateRange) = DateOnlyRange.TryCreate(
    new DateOnly(2024, 1, 1),
    new DateOnly(2024, 12, 31),
    inclusiveStart: false,  // Exclude start date
    inclusiveEnd: false     // Exclude end date
);

if (result.IsValid && dateRange != null)
{
    Console.WriteLine(dateRange.ToString()); // "(2024-01-01 .. 2024-12-31)"
    
    // Check containment
    var testDate = new DateOnly(2024, 6, 15);
    var isContained = dateRange.Contains(testDate);
    Console.WriteLine($"Contains {testDate}: {isContained}");
}
```

### Boundary Combinations

```csharp
// Different boundary combinations
var ranges = new[]
{
    // Inclusive both ends
    DateOnlyRange.TryCreate(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), true, true),
    // Exclusive start, inclusive end
    DateOnlyRange.TryCreate(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), false, true),
    // Inclusive start, exclusive end
    DateOnlyRange.TryCreate(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), true, false),
    // Exclusive both ends
    DateOnlyRange.TryCreate(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), false, false)
};

foreach (var (result, range) in ranges)
{
    if (result.IsValid && range != null)
    {
        Console.WriteLine(range.ToString());
    }
}
```

### Date Containment Checking

```csharp
var (result, dateRange) = DateOnlyRange.TryCreate(
    new DateOnly(2024, 1, 1),
    new DateOnly(2024, 12, 31)
);

if (result.IsValid && dateRange != null)
{
    // Test various dates
    var testDates = new[]
    {
        new DateOnly(2023, 12, 31),  // Before range
        new DateOnly(2024, 1, 1),    // Start boundary
        new DateOnly(2024, 6, 15),   // Within range
        new DateOnly(2024, 12, 31),  // End boundary
        new DateOnly(2025, 1, 1)     // After range
    };

    foreach (var testDate in testDates)
    {
        var isContained = dateRange.Contains(testDate);
        Console.WriteLine($"{testDate}: {isContained}");
    }
}
```

---

## ?? Common Patterns

### Range Creation Pattern

```csharp
var (result, dateRange) = DateOnlyRange.TryCreate(startDate, endDate);

if (result.IsValid && dateRange != null)
{
    // Use the validated date range
    ProcessDateRange(dateRange);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Containment Checking Pattern

```csharp
var (result, dateRange) = DateOnlyRange.TryCreate(rangeStart, rangeEnd);

if (result.IsValid && dateRange != null)
{
    // Check if a date falls within the range
    var isWithinRange = dateRange.Contains(targetDate);
    
    if (isWithinRange)
    {
        // Handle contained date
    }
    else
    {
        // Handle date outside range
    }
}
```

### Business Days Range Pattern

```csharp
// Create a range for business days only
var weekStart = GetNextMonday(DateOnly.FromDateTime(DateTime.Today));
var weekEnd = weekStart.AddDays(4); // Friday

var (result, businessWeek) = DateOnlyRange.TryCreate(weekStart, weekEnd);

if (result.IsValid && businessWeek != null)
{
    // Check if today is a business day
    var today = DateOnly.FromDateTime(DateTime.Today);
    var isBusinessDay = businessWeek.Contains(today) &&
                       today.DayOfWeek != DayOfWeek.Saturday &&
                       today.DayOfWeek != DayOfWeek.Sunday;
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
| `From` | `DateOnly` | The start date of the range |
| `To` | `DateOnly` | The end date of the range |
| `InclusiveStart` | `bool` | Whether the start date is included in the range |
| `InclusiveEnd` | `bool` | Whether the end date is included in the range |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(DateOnly from, DateOnly to, bool inclusiveStart = true, bool inclusiveEnd = true)` | `(ValidationResult, DateOnlyRange?)` | Static factory method to create validated date range |
| `Contains(DateOnly date)` | `bool` | Checks if the specified date falls within the range |
| `ToString()` | `string` | Returns string representation with bracket notation |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Valid Range** | 'From' date must be less than or equal to 'To' date |

---

## ?? Date Range Standards

### Boundary Notation

The `ToString()` method uses mathematical interval notation:

- **[2024-01-01 .. 2024-12-31]**: Inclusive start and end
- **(2024-01-01 .. 2024-12-31)**: Exclusive start and end
- **[2024-01-01 .. 2024-12-31)**: Inclusive start, exclusive end
- **(2024-01-01 .. 2024-12-31]**: Exclusive start, inclusive end

### Containment Logic

```csharp
// For inclusive boundaries [from, to]:
date >= from && date <= to

// For exclusive boundaries (from, to):
date > from && date < to

// For mixed boundaries [from, to):
date >= from && date < to

// For mixed boundaries (from, to]:
date > from && date <= to
```

### DateOnly vs DateTime

`DateOnlyRange` works with `DateOnly` precision (no time component):

```csharp
// DateOnly range ignores time completely
var range = DateOnlyRange.TryCreate(
    new DateOnly(2024, 1, 1),
    new DateOnly(2024, 1, 31)
);

// All DateTime values on January 15, 2024 are considered within range
range.Contains(new DateOnly(2024, 1, 15));  // true (same date)
```

### Conversion Between Types

```csharp
// Convert DateTime to DateOnly for range operations
DateTime dateTimeValue = DateTime.Now;
DateOnly dateOnlyValue = DateOnly.FromDateTime(dateTimeValue);

// Convert DateOnly to DateTime (with time set to midnight)
DateOnly dateOnly = new DateOnly(2024, 6, 15);
DateTime dateTime = dateOnly.ToDateTime(TimeOnly.MinValue);
```

---

## ??? Security Considerations

### Date Range Validation

```csharp
// ? DO: Validate date ranges before use
var (result, dateRange) = DateOnlyRange.TryCreate(userStartDate, userEndDate);

if (!result.IsValid)
{
    return BadRequest("Invalid date range");
}

// Additional validation
if (dateRange != null)
{
    var rangeSpan = dateRange.To.DayNumber - dateRange.From.DayNumber;
    
    // Business rule: range cannot be more than 365 days
    if (rangeSpan > 365)
    {
        return BadRequest("Date range cannot exceed 1 year");
    }
    
    // Business rule: range cannot be in the past
    if (dateRange.To < DateOnly.FromDateTime(DateTime.Today))
    {
        return BadRequest("Date range cannot be entirely in the past");
    }
}

// ? DON'T: Trust user input without validation
var startDate = DateOnly.Parse(startInput);
var endDate = DateOnly.Parse(endInput);
var range = new DateOnlyRange(startDate, endDate);  // Dangerous!
```

### Preventing Date Manipulation

```csharp
// ? DO: Enforce business rule constraints on ranges
var (result, dateRange) = DateOnlyRange.TryCreate(startDate, endDate);

if (result.IsValid && dateRange != null)
{
    // Business rule: booking ranges must be at least 1 day
    var rangeSpan = dateRange.To.DayNumber - dateRange.From.DayNumber;
    if (rangeSpan < 1)
    {
        return BadRequest("Date range must be at least 1 day");
    }
    
    // Business rule: cannot book more than 30 days in advance
    var maxEndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
    if (dateRange.To > maxEndDate)
    {
        return BadRequest("Cannot book more than 30 days in advance");
    }
    
    // Business rule: start date cannot be in the past
    var today = DateOnly.FromDateTime(DateTime.Today);
    if (dateRange.From < today)
    {
        return BadRequest("Start date cannot be in the past");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize date input before creating ranges
public (DateOnly?, DateOnly?) SanitizeDateOnlyRangeInput(string startInput, string endInput)
{
    DateOnly? startDate = null;
    DateOnly? endDate = null;
    
    if (!string.IsNullOrWhiteSpace(startInput))
    {
        if (DateOnly.TryParse(startInput.Trim(), out var parsed))
        {
            startDate = parsed;
        }
    }
    
    if (!string.IsNullOrWhiteSpace(endInput))
    {
        if (DateOnly.TryParse(endInput.Trim(), out var parsed))
        {
            endDate = parsed;
        }
    }
    
    return (startDate, endDate);
}

// Usage
var (startDate, endDate) = SanitizeDateOnlyRangeInput(startInput, endInput);
if (startDate.HasValue && endDate.HasValue)
{
    var (result, dateRange) = DateOnlyRange.TryCreate(startDate.Value, endDate.Value);
}
```

### Logging Date Ranges

```csharp
// ? DO: Log date ranges appropriately
public void LogDateOnlyRangeOperation(DateOnlyRange dateRange, string operation, string userId)
{
    // Log the date range operation
    _logger.LogInformation(
        "User {UserId} performed {Operation} on date range {DateRange}",
        userId,
        operation,
        dateRange.ToString()
    );
}

// For privacy-sensitive ranges, consider masking
public void LogPersonalDateOnlyRange(DateOnlyRange dateRange, string userId)
{
    // Mask the range for privacy
    var maskedRange = $"[{dateRange.From:yyyy-**-**} .. {dateRange.To:yyyy-**-**}]";
    
    _logger.LogInformation(
        "Personal date range operation for user {UserId}: {MaskedRange}",
        userId,
        maskedRange
    );
}
```
