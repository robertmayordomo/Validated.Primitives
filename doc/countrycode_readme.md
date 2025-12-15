# Validated.Primitives.CountryCode

A compact enumeration of supported country identifiers used by the validated primitives library. `CountryCode` provides a typed way to reference countries for format or locale-sensitive operations.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`CountryCode` is an enum that lists countries used by various value objects to apply country-specific validation, formatting, or presentation. The enum values are intentionally simple identifiers suitable for switch-based logic and mapping.

### Key Points

- Strongly-typed representation of country context
- Useful as a switch target for mapping to currency codes, formats, or validation rules
- Designed for use in value object factories and validators where country context is required

---

## API Reference

### Enum Members

The enum includes entries such as `UnitedStates`, `UnitedKingdom`, `Germany`, `France`, `Japan`, `China`, and others. Use the enum value directly when calling factory methods that accept a country context.

### Typical Usage

- Pass `CountryCode` into factory or validation methods that require country-specific behavior.
- Map `CountryCode` to other domain values (currency code, locale, display name) in a single centralized mapper.

---

## Security and Privacy

Country identifiers are not sensitive data by themselves, but when combined with other PII they should be handled according to applicable data protection policies.

---

## Related Documentation

- [Money README](money_readme.md) — shows where country context is used for currency mapping
- [IbanNumber README](ibannumber_readme.md) — demonstrates country parsing for IBANs
