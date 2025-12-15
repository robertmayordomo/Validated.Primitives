# Validated.Primitives.TimeOnlyRange

A validated time-only range primitive that represents a range between two times using the TimeOnly type with configurable boundary inclusion, ensuring valid time ranges when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`TimeOnlyRange` is a validated value object that represents a range between two times using the `TimeOnly` type. It supports configurable boundary inclusion (inclusive or exclusive) and provides methods for checking if times fall within the range. Once created, a `TimeOnlyRange` instance is guaranteed to be valid.

### Key Features

- **TimeOnly Type** - Uses .NET TimeOnly for time-only precision
- **Boundary Configuration** - Configurable inclusive/exclusive start and end
- **Time Containment** - Check if times fall within the range
- **Validation** - Ensures 'from' time is not after 'to' time
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other time range types

---

## ?? Basic Usage

### Creating a TimeOnly Range

```csharp
using Validated.Primitives.DateRanges;

// Create with TimeOnly values
var fromTime = new TimeOnly(9, 0, 0);    // 9:00 AM
var toTime = new TimeOnly(17, 0, 0);     // 5:00 PM

var (result, timeRange) = TimeOnlyRange.TryCreate(fromTime, toTime);

if (result.IsValid && timeRange != null)
{
    Console.WriteLine(timeRange.From);           // 9:00 AM
    Console.WriteLine(timeRange.To);             // 5:00 PM
    Console.WriteLine(timeRange.InclusiveStart); // true
    Console.WriteLine(timeRange.InclusiveEnd);   // true
    Console.WriteLine(timeRange.ToString());     // "[09:00:00 .. 17:00:00]"
    
    // Use the validated time range
    ProcessTimeRange(timeRange);
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
var (result, timeRange) = TimeOnlyRange.TryCreate(
    new TimeOnly(9, 0, 0),     // 9:00 AM
    new TimeOnly(17, 0, 0),    // 5:00 PM
    inclusiveStart: false,     // Exclude start time
    inclusiveEnd: false        // Exclude end time
);

if (result.IsValid && timeRange != null)
{
    Console.WriteLine(timeRange.ToString()); // "(09:00:00 .. 17:00:00)"
    
    // Check containment
    var testTime = new TimeOnly(14, 30, 0); // 2:30 PM
    var isContained = timeRange.Contains(testTime);
    Console.WriteLine($"Contains {testTime}: {isContained}");
}
```

### Boundary Combinations

```csharp
// Different boundary combinations
var ranges = new[]
{
    // Inclusive both ends
    TimeOnlyRange.TryCreate(new TimeOnly(9, 0), new TimeOnly(17, 0), true, true),
    // Exclusive start, inclusive end
    TimeOnlyRange.TryCreate(new TimeOnly(9, 0), new TimeOnly(17, 0), false, true),
    // Inclusive start, exclusive end
    TimeOnlyRange.TryCreate(new TimeOnly(9, 0), new TimeOnly(17, 0), true, false),
    // Exclusive both ends
    TimeOnlyRange.TryCreate(new TimeOnly(9, 0), new TimeOnly(17, 0), false, false)
};

foreach (var (result, range) in ranges)
{
    if (result.IsValid && range != null)
    {
        Console.WriteLine(range.ToString());
    }
}
```

### Time Containment Checking

```csharp
var (result, timeRange) = TimeOnlyRange.TryCreate(
    new TimeOnly(9, 0, 0),     // 9:00 AM
    new TimeOnly(17, 0, 0)     // 5:00 PM
);

if (result.IsValid && timeRange != null)
{
    // Test various times
    var testTimes = new[]
    {
        new TimeOnly(8, 0, 0),   // Before range (8:00 AM)
        new TimeOnly(9, 0, 0),   // Start boundary (9:00 AM)
        new TimeOnly(12, 0, 0),  // Within range (12:00 PM)
        new TimeOnly(17, 0, 0),  // End boundary (5:00 PM)
        new TimeOnly(18, 0, 0)   // After range (6:00 PM)
    };

    foreach (var testTime in testTimes)
    {
        var isContained = timeRange.Contains(testTime);
        Console.WriteLine($"{testTime}: {isContained}");
    }
}
```

---

## ?? Common Patterns

### Range Creation Pattern

```csharp
var (result, timeRange) = TimeOnlyRange.TryCreate(startTime, endTime);

if (result.IsValid && timeRange != null)
{
    // Use the validated time range
    ProcessTimeRange(timeRange);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Containment Checking Pattern

```csharp
var (result, timeRange) = TimeOnlyRange.TryCreate(rangeStart, rangeEnd);

if (result.IsValid && timeRange != null)
{
    // Check if a time falls within the range
    var isWithinRange = timeRange.Contains(targetTime);
    
    if (isWithinRange)
    {
        // Handle contained time
    }
    else
    {
        // Handle time outside range
    }
}
```

### Business Hours Pattern

```csharp
// Create a range for business hours
var businessStart = new TimeOnly(9, 0, 0);   // 9:00 AM
var businessEnd = new TimeOnly(17, 0, 0);    // 5:00 PM

var (result, businessHours) = TimeOnlyRange.TryCreate(businessStart, businessEnd);

if (result.IsValid && businessHours != null)
{
    // Check if current time is within business hours
    var now = TimeOnly.FromDateTime(DateTime.Now);
    var isBusinessHours = businessHours.Contains(now);
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
| `From` | `TimeOnly` | The start time of the range |
| `To` | `TimeOnly` | The end time of the range |
| `InclusiveStart` | `bool` | Whether the start time is included in the range |
| `InclusiveEnd` | `bool` | Whether the end time is included in the range |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(TimeOnly from, TimeOnly to, bool inclusiveStart = true, bool inclusiveEnd = true)` | `(ValidationResult, TimeOnlyRange?)` | Static factory method to create validated time range |
| `Contains(TimeOnly time)` | `bool` | Checks if the specified time falls within the range |
| `ToString()` | `string` | Returns string representation with bracket notation |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Valid Range** | 'From' time must be less than or equal to 'To' time |

---

## ?? Time Range Standards

### Boundary Notation

The `ToString()` method uses mathematical interval notation:

- **[09:00:00 .. 17:00:00]**: Inclusive start and end
- **(09:00:00 .. 17:00:00)**: Exclusive start and end
- **[09:00:00 .. 17:00:00)**: Inclusive start, exclusive end
- **(09:00:00 .. 17:00:00]**: Exclusive start, inclusive end

### Containment Logic

```csharp
// For inclusive boundaries [from, to]:
time >= from && time <= to

// For exclusive boundaries (from, to):
time > from && time < to

// For mixed boundaries [from, to):
time >= from && time < to

// For mixed boundaries (from, to]:
time > from && time <= to
```

### Time Precision

Time ranges work with `TimeOnly` precision (full time precision):

```csharp
var range = TimeOnlyRange.TryCreate(
    new TimeOnly(9, 0, 0),     // 9:00:00 AM
    new TimeOnly(17, 0, 0)     // 5:00:00 PM
);

// Times are compared with full precision
range.Contains(new TimeOnly(9, 0, 0));    // true (exact start time)
range.Contains(new TimeOnly(9, 0, 1));    // true (1 second after start)
range.Contains(new TimeOnly(17, 0, 0));   // true (exact end time)
```

### Common Time Ranges

```csharp
// Business hours
var businessHours = TimeOnlyRange.TryCreate(
    new TimeOnly(9, 0),    // 9:00 AM
    new TimeOnly(17, 0)    // 5:00 PM
);

// Lunch break
var lunchBreak = TimeOnlyRange.TryCreate(
    new TimeOnly(12, 0),   // 12:00 PM
    new TimeOnly(13, 0)    // 1:00 PM
);

// Night shift
var nightShift = TimeOnlyRange.TryCreate(
    new TimeOnly(22, 0),   // 10:00 PM
    new TimeOnly(6, 0)     // 6:00 AM (next day)
);
```

---

## ??? Security Considerations

### Time Range Validation

```csharp
// ? DO: Validate time ranges before use
var (result, timeRange) = TimeOnlyRange.TryCreate(userStartTime, userEndTime);

if (!result.IsValid)
{
    return BadRequest("Invalid time range");
}

// Additional validation
if (timeRange != null)
{
    var rangeSpan = timeRange.To - timeRange.From;
    
    // Business rule: range cannot be more than 24 hours
    if (rangeSpan.TotalHours > 24)
    {
        return BadRequest("Time range cannot exceed 24 hours");
    }
    
    // Business rule: start time cannot be in the past (for scheduling)
    var now = TimeOnly.FromDateTime(DateTime.Now);
    if (timeRange.From < now && DateTime.Today == DateTime.Today)
    {
        return BadRequest("Start time cannot be in the past");
    }
}

// ? DON'T: Trust user input without validation
var startTime = TimeOnly.Parse(startInput);
var endTime = TimeOnly.Parse(endInput);
var range = new TimeOnlyRange(startTime, endTime);  // Dangerous!
```

### Preventing Time Manipulation

```csharp
// ? DO: Enforce business rule constraints on time ranges
var (result, timeRange) = TimeOnlyRange.TryCreate(startTime, endTime);

if (result.IsValid && timeRange != null)
{
    // Business rule: appointments must be at least 30 minutes
    var duration = timeRange.To - timeRange.From;
    if (duration.TotalMinutes < 30)
    {
        return BadRequest("Time range must be at least 30 minutes");
    }
    
    // Business rule: cannot schedule outside business hours
    var businessStart = new TimeOnly(9, 0);
    var businessEnd = new TimeOnly(17, 0);
    
    if (timeRange.From < businessStart || timeRange.To > businessEnd)
    {
        return BadRequest("Time must be within business hours (9 AM - 5 PM)");
    }
    
    // Business rule: no overlapping appointments
    var hasConflict = await CheckForTimeConflicts(timeRange);
    if (hasConflict)
    {
        return BadRequest("Time slot conflicts with existing appointment");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize time input before creating ranges
public (TimeOnly?, TimeOnly?) SanitizeTimeOnlyRangeInput(string startInput, string endInput)
{
    TimeOnly? startTime = null;
    TimeOnly? endTime = null;
    
    if (!string.IsNullOrWhiteSpace(startInput))
    {
        if (TimeOnly.TryParse(startInput.Trim(), out var parsed))
        {
            startTime = parsed;
        }
    }
    
    if (!string.IsNullOrWhiteSpace(endInput))
    {
        if (TimeOnly.TryParse(endInput.Trim(), out var parsed))
        {
            endTime = parsed;
        }
    }
    
    return (startTime, endTime);
}

// Usage
var (startTime, endTime) = SanitizeTimeOnlyRangeInput(startInput, endInput);
if (startTime.HasValue && endTime.HasValue)
{
    var (result, timeRange) = TimeOnlyRange.TryCreate(startTime.Value, endTime.Value);
}
```

### Logging Time Ranges

```csharp
// ? DO: Log time ranges appropriately
public void LogTimeRangeOperation(TimeOnlyRange timeRange, string operation, string userId)
{
    // Log the time range operation
    _logger.LogInformation(
        "User {UserId} performed {Operation} on time range {TimeRange}",
        userId,
        operation,
        timeRange.ToString()
    );
}

// For privacy-sensitive ranges, consider masking
public void LogPersonalTimeRange(TimeOnlyRange timeRange, string userId)
{
    // Mask the range for privacy
    var maskedRange = $"[{timeRange.From:HH:**:**} .. {timeRange.To:HH:**:**}]";
    
    _logger.LogInformation(
        "Personal time range operation for user {UserId}: {MaskedRange}",
        userId,
        maskedRange
    );
}
```
