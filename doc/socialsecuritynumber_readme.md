# Validated.Primitives.SocialSecurityNumber

A validated social security number (SSN) primitive that enforces format and basic structural validation, ensuring SSNs are handled consistently and safely in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`SocialSecurityNumber` is a value object that represents a national social security number. It normalizes input, extracts digits, and performs structural validation appropriate for the target country's expected SSN format. The type is immutable, serializable, and validates on creation.

### Key Features

- Normalizes and stores digits-only representation
- Validates structural rules (length and digit-only content)
- Provides masked output for safe display
- JSON serialization support via converters
- Immutable and self-validating at creation time

---

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, ssn) = SocialSecurityNumber.TryCreate("123-45-6789");
if (result.IsValid)
{
    Console.WriteLine(ssn.Value);       // "123456789"
    Console.WriteLine(ssn.Masked());    // "***-**-6789" or similar
}
```

---

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Normalized digits-only SSN |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, string propertyName = "SocialSecurityNumber")` | `(ValidationResult, SocialSecurityNumber?)` | Static factory method to create validated SSN |
| `ToString()` | `string` | Returns normalized digits-only SSN |
| `Masked()` | `string` | Returns masked representation for display |

### Validation Rules

- Must not be null or whitespace
- Must contain only digits after normalization
- Must meet expected length for the configured format

---

## Security Considerations

- Treat SSNs as sensitive personally identifiable information (PII).
- Avoid logging full SSNs; prefer masked values and store only when necessary and permitted.
- Transmit SSNs only over secure channels (TLS) and follow applicable data protection regulations.
- Limit retention and access; apply encryption and access controls when storing SSNs.

---

## Related Documentation

- [Contact Information README](contactinformation_readme.md)
- [Main README](../README.md)
