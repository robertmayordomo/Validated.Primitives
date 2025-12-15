# Validated.Primitives.AddressLine

A validated address line primitive that enforces maximum length restrictions for street addresses, apartment numbers, and building names, ensuring address lines are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`AddressLine` is a validated value object that represents an address line (street address, apartment number, suite, building name, etc.). It enforces a maximum length of 200 characters when provided, but allows null/empty values for optional address lines. Once created, an `AddressLine` instance is guaranteed to be valid.

### Key Features

- **Optional Field** - Can be null for addresses without second address line
- **Maximum Length** - Enforces 200 character limit when provided
- **Automatic Trimming** - Removes leading and trailing whitespace
- **Flexible Content** - Supports street addresses, apartment numbers, building names
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating an Address Line

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, addressLine) = AddressLine.TryCreate("123 Main Street");

if (result.IsValid)
{
    Console.WriteLine(addressLine.Value);      // "123 Main Street"
    Console.WriteLine(addressLine.ToString()); // "123 Main Street"
    
    // Use the validated address line
    ProcessAddress(addressLine);
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.MemberName}: {error.Message}");
    }
}
```

### Using in Domain Models

```csharp
public class Address
{
    public Guid Id { get; set; }
    public AddressLine Street { get; set; }
    public AddressLine? AddressLine2 { get; set; }  // Optional second line
    public City City { get; set; }
}

// Usage - with second address line
var (streetResult, street) = AddressLine.TryCreate("123 Main Street");
var (line2Result, line2) = AddressLine.TryCreate("Apt 4B");

if (streetResult.IsValid && (line2Result.IsValid || line2 == null))
{
    var address = new Address
    {
        Id = Guid.NewGuid(),
        Street = street,      // Guaranteed valid
        AddressLine2 = line2,  // Can be null
        City = city
    };
}

// Usage - without second address line
var addressSimple = new Address
{
    Id = Guid.NewGuid(),
    Street = street,
    AddressLine2 = null,  // Valid for simple addresses
    City = city
};
```

---

## ? Valid Address Lines

### Street Addresses

```csharp
// Basic street addresses
var (r1, a1) = AddressLine.TryCreate("123 Main Street");              // ? Valid
var (r2, a2) = AddressLine.TryCreate("456 Oak Avenue");               // ? Valid
var (r3, a3) = AddressLine.TryCreate("789 Elm Boulevard");            // ? Valid
var (r4, a4) = AddressLine.TryCreate("1000 Pine Road");               // ? Valid

foreach (var (result, address) in new[] { (r1, a1), (r2, a2), (r3, a3), (r4, a4) })
{
    Console.WriteLine($"{address.Value}: {result.IsValid}");  // All true
}
```

### With Unit/Apartment Numbers

```csharp
var (r1, a1) = AddressLine.TryCreate("123 Main Street, Suite 500");     // ? Valid
var (r2, a2) = AddressLine.TryCreate("456 Oak Ave, Apartment 4B");      // ? Valid
var (r3, a3) = AddressLine.TryCreate("789 Elm Blvd, Unit 12C");         // ? Valid
var (r4, a4) = AddressLine.TryCreate("1000 Pine Rd, Floor 3");          // ? Valid
```

### Building Names and PO Boxes

```csharp
var (r1, a1) = AddressLine.TryCreate("Empire State Building");          // ? Valid
var (r2, a2) = AddressLine.TryCreate("PO Box 1234");                    // ? Valid
var (r3, a3) = AddressLine.TryCreate("One World Trade Center");         // ? Valid
var (r4, a4) = AddressLine.TryCreate("Central Park West");              // ? Valid
```

### International Addresses

```csharp
var (r1, a1) = AddressLine.TryCreate("10 Downing Street");              // ? Valid (UK)
var (r2, a2) = AddressLine.TryCreate("123 Rue de Rivoli");              // ? Valid (France)
var (r3, a3) = AddressLine.TryCreate("1-2-3 Shibuya");                  // ? Valid (Japan)
var (r4, a4) = AddressLine.TryCreate("Platz der Republik 1");           // ? Valid (Germany)
```

### Whitespace Handling

```csharp
// Automatic trimming
var (r1, a1) = AddressLine.TryCreate("  123 Main Street  ");           // ? Trimmed
var (r2, a2) = AddressLine.TryCreate("   456 Oak Avenue   ");          // ? Trimmed
var (r3, a3) = AddressLine.TryCreate("789 Elm Boulevard ");            // ? Trimmed
var (r4, a4) = AddressLine.TryCreate(" 1000 Pine Road");               // ? Trimmed
```

### Null and Empty Values

```csharp
// Null and empty values are allowed (return null)
var (r1, a1) = AddressLine.TryCreate(null);                            // ? Returns null
var (r2, a2) = AddressLine.TryCreate("");                              // ? Returns null
var (r3, a3) = AddressLine.TryCreate("   ");                           // ? Returns null
var (r4, a4) = AddressLine.TryCreate("\t");                            // ? Returns null

// All return ValidationResult.Success() with null value
```

---

## ? Invalid Address Lines

### Too Long

```csharp
// Address line exceeds 200 character limit
var longAddress = new string('A', 201);
var (result, addressLine) = AddressLine.TryCreate(longAddress);          // ? Exceeds 200 characters

// result.IsValid == false
// result.Errors contains MaxLength error
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, addressLine) = AddressLine.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated address line (can be null)
    ProcessAddress(addressLine);
}
else
{
    // Handle validation errors
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.MemberName}: {error.Message}");
    }
    
    // Or use formatted output
    Console.WriteLine(result.ToBulletList());
}
```

### Optional Field Pattern

```csharp
// AddressLine2 is optional - handle both cases
AddressLine? addressLine2 = null;
if (!string.IsNullOrWhiteSpace(userInput))
{
    var (result, address) = AddressLine.TryCreate(userInput);
    if (result.IsValid)
    {
        addressLine2 = address;
    }
    else
    {
        // Handle validation error for optional field
        _logger.LogWarning("Invalid address line provided: {Input}", userInput);
    }
}

// Use in address (can be null)
var address = new Address
{
    Street = street,
    AddressLine2 = addressLine2,  // Nullable
    City = city
};
```

### Domain Model Usage

```csharp
public class Address
{
    public Guid Id { get; set; }
    public AddressLine Street { get; set; }
    public AddressLine? AddressLine2 { get; set; }
    public City City { get; set; }
}

// Creating an address
var (streetResult, street) = AddressLine.TryCreate(streetInput);
var (line2Result, line2) = AddressLine.TryCreate(line2Input);  // Optional

if (streetResult.IsValid && (line2Result.IsValid || line2 == null))
{
    var address = new Address
    {
        Id = Guid.NewGuid(),
        Street = street,
        AddressLine2 = line2,  // Can be null
        City = city
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, addressLine) = AddressLine.TryCreate("123 Main Street");

// Serialize
string json = JsonSerializer.Serialize(addressLine);
// {"Value":"123 Main Street"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<AddressLine>(json);
Console.WriteLine(deserialized.Value);  // "123 Main Street"
```

---

## ?? Related Documentation

- [Address README](address_readme.md) - Complete address validation including address lines
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated address line string |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string? value, string propertyName = "AddressLine")` | `(ValidationResult, AddressLine?)` | Static factory method to create validated address line |
| `ToString()` | `string` | Returns the address line value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Maximum Length** | Cannot exceed 200 characters when provided |
| **Optional** | Null or empty values are allowed |

---

## ?? Address Line Formats

The `AddressLine` type supports various address line formats:

### Street Addresses
- `123 Main Street`
- `456 Oak Avenue`
- `789 Elm Boulevard`
- `1000 Pine Road`

### With Unit Designators
- `123 Main Street, Suite 500`
- `456 Oak Ave, Apartment 4B`
- `789 Elm Blvd, Unit 12C`
- `1000 Pine Rd, Floor 3`

### Building Names
- `Empire State Building`
- `One World Trade Center`
- `Central Park West`

### PO Boxes
- `PO Box 1234`
- `P.O. Box 5678`

### International Formats
- `10 Downing Street` (UK)
- `123 Rue de Rivoli` (France)
- `1-2-3 Shibuya` (Japan)
- `Platz der Republik 1` (Germany)

---

## ??? Security Considerations

### Address Line Validation

```csharp
// ? DO: Validate before use
var (result, addressLine) = AddressLine.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid address line");
}

// ? DON'T: Trust user input without validation
var addressLine = userInput;  // Dangerous!
ProcessAddress(addressLine);
```

### Preventing Injection Attacks

```csharp
// ? DO: Use validated address line in database queries
var (_, addressLine) = AddressLine.TryCreate(userInput);
var addresses = await _dbContext.Addresses
    .Where(a => a.Street.Value == addressLine.Value)
    .ToListAsync();

// ? DON'T: Concatenate address lines in queries
var query = $"SELECT * FROM Addresses WHERE Street = '{userInput}'";  // Dangerous!
```

### Input Sanitization

```csharp
// ? DO: Sanitize address line input before validation
public string SanitizeAddressLineInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Trim whitespace
    input = input.Trim();
    
    // Allow common address characters
    var sanitized = new string(input.Where(c =>
        char.IsLetterOrDigit(c) ||
        char.IsWhiteSpace(c) ||
        c == '-' ||
        c == ''' ||
        c == ',' ||
        c == '.' ||
        c == '/' ||
        c == '#' ||
        c == '&'
    ).ToArray());
    
    // Normalize multiple spaces to single space
    sanitized = Regex.Replace(sanitized, @"\s+", " ");
    
    return sanitized;
}

// Usage
var sanitized = SanitizeAddressLineInput(userInput);
var (result, addressLine) = AddressLine.TryCreate(sanitized);
```

### Logging Sensitive Information

```csharp
// ? DO: Log address lines appropriately
public void LogAddressUpdate(Address address, string userId)
{
    // Log address update without exposing full address
    _logger.LogInformation(
        "Address updated by user {UserId}: Street starts with '{StreetStart}'",
        userId,
        address.Street.Value?.Substring(0, Math.Min(10, address.Street.Value.Length)) + "..."
    );
}

// ? DON'T: Log full address lines that might contain sensitive data
_logger.LogInformation($"User updated address: {fullAddressLine}");  // Avoid
```
