# Validated.Primitives.TrackingNumber

A validated tracking number primitive that enforces normalization and basic format validation for shipment and logistics tracking numbers (UPS, USPS, FedEx, DHL, etc.).

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`TrackingNumber` is a value object representing shipment tracking numbers. It normalizes input, detects common carriers when possible, and validates basic structure (length, allowed characters) and checksums where available.

### Key Features

- Carrier detection for popular shippers (UPS, FedEx, USPS, DHL)
- Normalization (remove spaces/dashes) and allowed character validation
- Optional checksum validation for carriers that support it
- Immutable and self-validating at creation

---

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, tracking) = TrackingNumber.TryCreate("1Z999AA10123456784");
if (result.IsValid)
{
    Console.WriteLine(tracking.Value);
    Console.WriteLine(tracking.Shipper); // e.g., "UPS"
}
```

---

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Normalized tracking number |
| `Shipper` | `string?` | Optional detected shipper/carrier |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, string propertyName = "TrackingNumber")` | `(ValidationResult, TrackingNumber?)` | Static factory method to create validated tracking number |
| `ToString()` | `string` | Returns normalized tracking number |

### Validation Rules

- Must not be null or whitespace
- Must match expected characters for carriers
- Optional checksum validation

---

## Security Considerations

- Tracking numbers are generally not highly sensitive but avoid exposing them unnecessarily in logs or public interfaces.

---

## Related Documentation

- [Shipping README](../README.md)
