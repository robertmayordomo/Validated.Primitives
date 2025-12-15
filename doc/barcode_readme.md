# Validated.Primitives.Barcode

A validated barcode primitive that enforces format validation for common barcode standards (EAN-13, UPC-A, Code128, QR payloads, etc.).

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`Barcode` is a value object representing barcode values. It validates format and checksums for common linear barcode standards and supports payload validation for 2D codes like QR when applicable.

### Key Features

- Supports EAN/UPC checksum validation
- Supports common linear and 2D barcode payloads
- Normalizes input and verifies allowed character sets
- Provides masked or shortened display variants
- Immutable and self-validating at creation

---

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, barcode) = Barcode.TryCreate("0123456789012"); // EAN-13
if (result.IsValid)
{
    Console.WriteLine(barcode.Value);
}
```

---

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Normalized barcode string |
| `Symbology` | `string?` | Optional detected symbology (EAN-13, UPC-A, QR, etc.) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, string propertyName = "Barcode")` | `(ValidationResult, Barcode?)` | Static factory method to create validated barcode |
| `ToString()` | `string` | Returns normalized barcode value |

### Validation Rules

- Must not be null or whitespace
- Must match expected pattern for detected or specified symbology
- Must pass checksum validation where applicable (EAN/UPC)

---

## Security Considerations

- Barcode payloads may contain URLs or sensitive data; validate and sanitize before use.

---

## Related Documentation

- [Inventory README](../README.md)
