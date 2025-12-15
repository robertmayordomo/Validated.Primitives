# Validated.Primitives.Passport

A validated passport number primitive that enforces normalization and format validation for passport identifiers, ensuring passport numbers are handled consistently.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`Passport` is a value object that represents a passport number. It normalizes input (trims and uppercases), validates allowed characters and length based on common passport formats, and provides masking for safe display.

### Key Features

- Normalizes input to uppercase and removes extraneous whitespace
- Validates allowed character sets and typical length ranges
- Masked output for safe display and logging
- Immutable and self-validating at creation time
- JSON serialization support via converters

---

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, passport) = Passport.TryCreate("C01X12345");
if (result.IsValid)
{
    Console.WriteLine(passport.Value);    // "C01X12345"
    Console.WriteLine(passport.Masked()); // "C01X*****" or similar
}
```

---

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Normalized passport number |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, string propertyName = "Passport")` | `(ValidationResult, Passport?)` | Static factory method to create validated passport number |
| `ToString()` | `string` | Returns normalized passport number |
| `Masked()` | `string` | Returns masked representation for display |

### Validation Rules

- Must not be null or whitespace
- Must contain only allowed characters (letters and digits, some countries allow other characters)
- Must fall within a reasonable length range based on common passport formats

---

## Security Considerations

- Treat passport numbers as sensitive identifiers; avoid logging full values.
- Transmit and store passport numbers securely and in compliance with applicable privacy laws.

---

## Related Documentation

- [Identity README](../README.md)
