# Validated.Primitives.DateRange

A validated date range primitive that represents a range between two dates with configurable boundary inclusion, ensuring valid date ranges when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`DateRange` is a validated value object that represents a range between two dates. It supports configurable boundary inclusion (inclusive or exclusive) and provides methods for checking if dates fall within the range. Once created, a `DateRange` instance is guaranteed to be valid.

### Key Features

- **Boundary Configuration** - Configurable inclusive/exclusive start and end
- **Date Containment** - Check if dates fall within the range
- **Validation** - Ensures 'from' date is not after 'to' date
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other date range types

---

## ?? Basic Usage

### Creating a Date Range

```csharp
using Validated.Primitives.DateRanges;

// Create with DateTime values
var fromDate = new DateTime(2024, 1, 1);
var toDate = new DateTime(2024, 12, 31);

var (result, dateRange) = DateRange.TryCreate(fromDate, toDate);

if (result.IsValid && dateRange != null)
{
    Console.WriteLine(dateRange.From);           // 1/1/2024 12:00:00 AM
    Console.WriteLine(dateRange.To);             // 12/31/2024 12:00:00 AM
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
var (result, dateRange) = DateRange.TryCreate(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31),
    inclusiveStart: false,  // Exclude start date
    inclusiveEnd: false     // Exclude end date
);

if (result.IsValid && dateRange != null)
{
    Console.WriteLine(dateRange.ToString()); // "(2024-01-01 .. 2024-12-31)"
    
    // Check containment
    var testDate = new DateTime(2024, 6, 15);
    var isContained = dateRange.Contains(testDate);
    Console.WriteLine($"Contains {testDate:d}: {isContained}");
}
```

### Boundary Combinations

```csharp
// Different boundary combinations
var ranges = new[]
{
    // Inclusive both ends
    DateRange.TryCreate(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), true, true),
    // Exclusive start, inclusive end
    DateRange.TryCreate(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), false, true),
    // Inclusive start, exclusive end
    DateRange.TryCreate(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), true, false),
    // Exclusive both ends
    DateRange.TryCreate(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), false, false)
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
var (result, dateRange) = DateRange.TryCreate(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 12, 31)
);

if (result.IsValid && dateRange != null)
{
    // Test various dates
    var testDates = new[]
    {
        new DateTime(2023, 12, 31),  // Before range
        new DateTime(2024, 1, 1),    // Start boundary
        new DateTime(2024, 6, 15),   // Within range
        new DateTime(2024, 12, 31),  // End boundary
        new DateTime(2025, 1, 1)     // After range
    };

    foreach (var testDate in testDates)
    {
        var isContained = dateRange.Contains(testDate);
        Console.WriteLine($"{testDate:d}: {isContained}");
    }
}
```

---

## ?? Common Patterns

### Range Creation Pattern

```csharp
var (result, dateRange) = DateRange.TryCreate(startDate, endDate);

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
var (result, dateRange) = DateRange.TryCreate(rangeStart, rangeEnd);

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
var weekStart = GetNextMonday(DateTime.Today);
var weekEnd = weekStart.AddDays(4); // Friday

var (result, businessWeek) = DateRange.TryCreate(weekStart, weekEnd);

if (result.IsValid && businessWeek != null)
{
    // Check if today is a business day
    var isBusinessDay = businessWeek.Contains(DateTime.Today) &&
                       DateTime.Today.DayOfWeek != DayOfWeek.Saturday &&
                       DateTime.Today.DayOfWeek != DayOfWeek.Sunday;
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
| `From` | `DateTime` | The start date of the range |
| `To` | `DateTime` | The end date of the range |
| `InclusiveStart` | `bool` | Whether the start date is included in the range |
| `InclusiveEnd` | `bool` | Whether the end date is included in the range |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(DateTime from, DateTime to, bool inclusiveStart = true, bool inclusiveEnd = true)` | `(ValidationResult, DateRange?)` | Static factory method to create validated date range |
| `Contains(DateTime date)` | `bool` | Checks if the specified date falls within the range |
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

### Date Precision

Date ranges work with `DateTime.Date` precision (time component is ignored):

```csharp
var range = DateRange.TryCreate(
    new DateTime(2024, 1, 1, 10, 30, 0),  // 10:30 AM
    new DateTime(2024, 1, 31, 15, 45, 0)  // 3:45 PM
);

// Both dates below are considered within range:
range.Contains(new DateTime(2024, 1, 1, 8, 0, 0));   // true (same date)
range.Contains(new DateTime(2024, 1, 1, 12, 0, 0));  // true (same date)
```

---

## ??? Security Considerations

### Date Range Validation

```csharp
// ? DO: Validate date ranges before use
var (result, dateRange) = DateRange.TryCreate(userStartDate, userEndDate);

if (!result.IsValid)
{
    return BadRequest("Invalid date range");
}

// Additional validation
if (dateRange != null)
{
    var rangeSpan = (dateRange.To - dateRange.From).Days;
    
    // Business rule: range cannot be more than 365 days
    if (rangeSpan > 365)
    {
        return BadRequest("Date range cannot exceed 1 year");
    }
    
    // Business rule: range cannot be in the past
    if (dateRange.To < DateTime.Today)
    {
        return BadRequest("Date range cannot be entirely in the past");
    }
}

// ? DON'T: Trust user input without validation
var startDate = DateTime.Parse(startInput);
var endDate = DateTime.Parse(endInput);
var range = new DateRange(startDate, endDate);  // Dangerous!
```

### Preventing Date Manipulation

```csharp
// ? DO: Enforce business rule constraints on ranges
var (result, dateRange) = DateRange.TryCreate(startDate, endDate);

if (result.IsValid && dateRange != null)
{
    // Business rule: booking ranges must be at least 1 day
    if ((dateRange.To - dateRange.From).Days < 1)
    {
        return BadRequest("Date range must be at least 1 day");
    }
    
    // Business rule: cannot book more than 30 days in advance
    var maxEndDate = DateTime.Today.AddDays(30);
    if (dateRange.To > maxEndDate)
    {
        return BadRequest("Cannot book more than 30 days in advance");
    }
    
    // Business rule: start date cannot be in the past
    if (dateRange.From < DateTime.Today)
    {
        return BadRequest("Start date cannot be in the past");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize date input before creating ranges
public (DateTime?, DateTime?) SanitizeDateRangeInput(string startInput, string endInput)
{
    DateTime? startDate = null;
    DateTime? endDate = null;
    
    if (!string.IsNullOrWhiteSpace(startInput))
    {
        if (DateTime.TryParse(startInput.Trim(), out var parsed))
        {
            startDate = parsed.Date;  // Use date part only
        }
    }
    
    if (!string.IsNullOrWhiteSpace(endInput))
    {
        if (DateTime.TryParse(endInput.Trim(), out var parsed))
        {
            endDate = parsed.Date;  // Use date part only
        }
    }
    
    return (startDate, endDate);
}

// Usage
var (startDate, endDate) = SanitizeDateRangeInput(startInput, endInput);
if (startDate.HasValue && endDate.HasValue)
{
    var (result, dateRange) = DateRange.TryCreate(startDate.Value, endDate.Value);
}
```

### Logging Date Ranges

```csharp
// ? DO: Log date ranges appropriately
public void LogDateRangeOperation(DateRange dateRange, string operation, string userId)
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
public void LogPersonalDateRange(DateRange dateRange, string userId)
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
