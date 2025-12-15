# Validated.Primitives.DrivingLicenseNumber

A validated driving license number primitive that enforces normalization and country-aware format validation for driving license identifiers.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`DrivingLicenseNumber` is a value object that represents a driving license number. It normalizes input and applies format validation for supported countries where rules are known. It provides masking for display and is immutable and self-validating at creation.

### Key Features

- Normalizes input (trimming, uppercasing)
- Country-aware validation when a country code is provided
- Masked output for safe display
- Immutable and serializable

---

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, dl) = DrivingLicenseNumber.TryCreate("SAMPLE12345", CountryCode.UnitedKingdom);
if (result.IsValid)
{
    Console.WriteLine(dl.Value);
    Console.WriteLine(dl.Masked());
}
```

---

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Normalized driving license number |
| `CountryCode` | `CountryCode?` | Optional country context used for validation |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, CountryCode? country = null, string propertyName = "DrivingLicenseNumber")` | `(ValidationResult, DrivingLicenseNumber?)` | Static factory method to create validated driving license number |
| `ToString()` | `string` | Returns normalized license number |
| `Masked()` | `string` | Returns masked representation for display |

### Validation Rules

- Must not be null or whitespace
- Must match allowed characters
- Optional country-specific length/structure validation

---

## Security Considerations

- Driving license numbers are sensitive personally identifiable information (PII). Avoid logging full values and store/transmit them securely.

---

## Related Documentation

- [Identity README](../README.md)
