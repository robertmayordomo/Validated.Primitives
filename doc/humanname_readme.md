# Validated.Primitives.HumanName

A validated human name primitive that enforces normalization and common validation rules for personal names to ensure consistent usage across applications.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`HumanName` is a value object representing a person's name. It normalizes whitespace, trims, and optionally validates length and allowed characters. It can store separate components (given, middle, family) or a single combined full name depending on usage.

### Key Features

- Normalizes whitespace and trims input
- Optional componentized storage (Given, Middle, Family)
- Validation for length and disallowed characters
- Immutable and self-validating at creation
- JSON serialization support

---

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, name) = HumanName.TryCreate("John A. Doe");
if (result.IsValid)
{
    Console.WriteLine(name.Value); // "John A. Doe"
}
```

---

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | Normalized full name |
| `GivenName` | `string?` | Optional given name component |
| `MiddleName` | `string?` | Optional middle name component |
| `FamilyName` | `string?` | Optional family name component |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, string propertyName = "HumanName")` | `(ValidationResult, HumanName?)` | Static factory method to create validated full name |
| `ToString()` | `string` | Returns normalized full name |

### Validation Rules

- Must not be null or whitespace
- Must be within reasonable length limits
- Should not contain control characters

---

## Security Considerations

- Names are PII; handle with appropriate access controls and avoid unnecessary logging.

---

## Related Documentation

- [Contact Information README](contactinformation_readme.md)
