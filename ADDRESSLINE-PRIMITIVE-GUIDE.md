# AddressLine Primitive - Usage Guide

## Overview

`AddressLine` is a validated primitive type for street addresses, apartment numbers, and other address line components. It's **optional** (can be null) but validates a maximum length of 200 characters when a value is provided.

## Created Files

- **`src/Valdiated.Primatives/ValueObjects/AddressLine.cs`** - The primitive implementation
- **`tests/Valdiated.Primatives.Tests/ValueObjects/AddressLineTests.cs`** - 23 comprehensive tests

## Features

? **Optional** - Can be null or empty  
? **Max Length Validation** - 200 characters maximum when provided  
? **Preserves Formatting** - Keeps original spacing and formatting  
? **Custom Error Messages** - Customize property names in validation errors  
? **Record Semantics** - Value-based equality comparison  
? **Immutable** - Thread-safe by design  

## Usage Examples

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Valid address line
var (result, addressLine) = AddressLine.TryCreate("123 Main Street");
if (result.IsValid)
{
    Console.WriteLine(addressLine.Value); // "123 Main Street"
}

// Null or empty is valid
var (result2, addressLine2) = AddressLine.TryCreate(null);
Console.WriteLine(result2.IsValid); // True
Console.WriteLine(addressLine2);     // null

// Empty string is also valid (returns null)
var (result3, addressLine3) = AddressLine.TryCreate("");
Console.WriteLine(result3.IsValid); // True
Console.WriteLine(addressLine3);     // null

// Whitespace only is valid (returns null)
var (result4, addressLine4) = AddressLine.TryCreate("   ");
Console.WriteLine(result4.IsValid); // True
Console.WriteLine(addressLine4);     // null
```

### Max Length Validation

```csharp
// 200 characters - valid
var valid200 = new string('X', 200);
var (result1, line1) = AddressLine.TryCreate(valid200);
Console.WriteLine(result1.IsValid); // True
Console.WriteLine(line1.Value.Length); // 200

// 201 characters - invalid
var invalid201 = new string('X', 201);
var (result2, line2) = AddressLine.TryCreate(invalid201);
Console.WriteLine(result2.IsValid); // False
Console.WriteLine(line2); // null
Console.WriteLine(result2.Errors[0].Code); // "MaxLength"
Console.WriteLine(result2.Errors[0].Message); 
// "AddressLine must be at most 200 characters."
```

### Custom Property Names

```csharp
var tooLong = new string('a', 201);
var (result, addressLine) = AddressLine.TryCreate(tooLong, "StreetAddress");

if (!result.IsValid)
{
    Console.WriteLine(result.Errors[0].MemberName); // "StreetAddress"
    Console.WriteLine(result.Errors[0].Message);
    // "StreetAddress must be at most 200 characters."
}
```

### Various Valid Formats

```csharp
// All of these are valid:
var examples = new[]
{
    "123 Main St",
    "Apt 4B, 456 Oak Avenue",
    "Building 7, Floor 3, Room 301",
    "P.O. Box 12345",
    "Rural Route 5",
    "123 Rue de la Paix, Montréal", // Unicode
    "123 Main St. #4-B",             // Special characters
    "12345"                          // Numbers only
};

foreach (var example in examples)
{
    var (result, line) = AddressLine.TryCreate(example);
    Console.WriteLine($"{example}: {result.IsValid}"); // All true
}
```

### Using in Domain Models

```csharp
public record ShippingAddress
{
    public required AddressLine? Street { get; init; }
    public AddressLine? AddressLine2 { get; init; }
    public required string City { get; init; }
    public required PostalCode PostalCode { get; init; }
    
    public static (ValidationResult Result, ShippingAddress? Value) TryCreate(
        string? street,
        string? addressLine2,
        string city,
        CountryCode country,
        string postalCode)
    {
        var result = ValidationResult.Success();
        
        // Validate street (optional)
        var (streetResult, streetValue) = AddressLine.TryCreate(street, "Street");
        result.Merge(streetResult);
        
        // Validate address line 2 (optional)
        var (line2Result, line2Value) = AddressLine.TryCreate(addressLine2, "AddressLine2");
        result.Merge(line2Result);
        
        // Validate city (required - not using AddressLine)
        if (string.IsNullOrWhiteSpace(city))
        {
            result.AddError("City is required.", "City", "Required");
        }
        
        // Validate postal code
        var (postalResult, postalValue) = PostalCode.TryCreate(country, postalCode);
        result.Merge(postalResult);
        
        if (!result.IsValid || string.IsNullOrWhiteSpace(city))
        {
            return (result, null);
        }
        
        var address = new ShippingAddress
        {
            Street = streetValue,
            AddressLine2 = line2Value,
            City = city,
            PostalCode = postalValue!
        };
        
        return (result, address);
    }
}

// Usage
var (result, address) = ShippingAddress.TryCreate(
    "123 Main Street",
    "Apt 4B",
    "New York",
    CountryCode.UnitedStates,
    "10001"
);

if (result.IsValid)
{
    Console.WriteLine(address.Street?.Value);      // "123 Main Street"
    Console.WriteLine(address.AddressLine2?.Value); // "Apt 4B"
    Console.WriteLine(address.City);                // "New York"
}
```

### Updating the Existing Address Domain Type

You can update the `Address` domain type to use `AddressLine`:

```csharp
public sealed record Address
{
    public required AddressLine Street { get; init; }
    public AddressLine? AddressLine2 { get; init; } // Optional second line
    public required string City { get; init; }
    public string? StateProvince { get; init; }
    public required PostalCode PostalCode { get; init; }
    
    public static (ValidationResult Result, Address? Value) TryCreate(
        string street,
        string? addressLine2,
        string city,
        CountryCode country,
        string postalCode,
        string? stateProvince = null)
    {
        var result = ValidationResult.Success();
        
        // Validate street using AddressLine primitive
        var (streetResult, streetValue) = AddressLine.TryCreate(street, "Street");
        result.Merge(streetResult);
        
        // Ensure street is not null (it's required)
        if (streetValue == null)
        {
            result.AddError("Street address is required.", "Street", "Required");
        }
        
        // Validate optional address line 2
        var (line2Result, line2Value) = AddressLine.TryCreate(addressLine2, "AddressLine2");
        result.Merge(line2Result);
        
        // ... rest of validation ...
        
        if (!result.IsValid)
        {
            return (result, null);
        }
        
        var address = new Address
        {
            Street = streetValue!,
            AddressLine2 = line2Value,
            City = city.Trim(),
            StateProvince = stateProvince?.Trim(),
            PostalCode = postalValue!
        };
        
        return (result, address);
    }
}
```

## Test Coverage

### Test Categories

- ? Valid address lines
- ? Null/empty/whitespace handling
- ? Max length validation (200 chars)
- ? Exceeding max length (201+ chars)
- ? Custom property names
- ? Various formats (apartments, PO boxes, etc.)
- ? Unicode characters
- ? Special characters
- ? Equality semantics
- ? ToString behavior

### Running Tests

```bash
# Run all AddressLine tests
dotnet test --filter "FullyQualifiedName~AddressLineTests"

# Run specific test
dotnet test --filter "FullyQualifiedName~AddressLineTests.TryCreate_WithValidAddressLine_ShouldSucceed"
```

## Key Differences from Other Primitives

### Optional by Design
Unlike `EmailAddress` or `PhoneNumber` which require a value, `AddressLine` allows null:

```csharp
// These all return success with null value:
AddressLine.TryCreate(null);
AddressLine.TryCreate("");
AddressLine.TryCreate("   ");
```

### Preserves Formatting
Unlike some primitives that trim whitespace, `AddressLine` preserves the original string:

```csharp
var (result, line) = AddressLine.TryCreate("  123 Main St  ");
Console.WriteLine(line.Value); // "  123 Main St  " (spaces preserved)
```

### No Format Validation
`AddressLine` only validates length, not format. This allows flexibility for different address formats worldwide.

## Integration with Existing Types

The `AddressLine` primitive integrates seamlessly with your existing validated primitives:

```csharp
public record FullAddress
{
    public AddressLine? Street { get; init; }
    public AddressLine? Unit { get; init; }
    public AddressLine? Building { get; init; }
    public PostalCode PostalCode { get; init; }
    public EmailAddress? Email { get; init; }
    public PhoneNumber? Phone { get; init; }
}
```

## API Reference

### Constructor
```csharp
private AddressLine(string value, string propertyName = "AddressLine")
```

### Static Methods

#### TryCreate
```csharp
public static (ValidationResult Result, AddressLine? Value) TryCreate(
    string? value,
    string propertyName = "AddressLine")
```

**Parameters:**
- `value` - The address line string to validate (can be null)
- `propertyName` - Optional custom property name for error messages (default: "AddressLine")

**Returns:**
- `ValidationResult` - Contains validation errors if any
- `AddressLine?` - The validated address line, or null if input was null/empty/whitespace or validation failed

**Behavior:**
- Returns `(Success, null)` if value is null, empty, or whitespace
- Returns `(Failure, null)` if value exceeds 200 characters
- Returns `(Success, AddressLine)` if value is 1-200 characters

### Instance Methods

#### ToString
```csharp
public override string ToString()
```
Returns the address line value as a string.

## Error Codes

| Code | Description | Example |
|------|-------------|---------|
| `MaxLength` | Address line exceeds 200 characters | "AddressLine must be at most 200 characters." |

## Best Practices

1. **Use for Optional Address Components**: Perfect for optional address lines, apartment numbers, building names, etc.

2. **Combine with Required Validation**: If an address line is required, add separate validation:
   ```csharp
   var (result, line) = AddressLine.TryCreate(input);
   if (line == null)
   {
       result.AddError("Street is required.", "Street", "Required");
   }
   ```

3. **Use Custom Property Names**: Always provide meaningful property names for better error messages:
   ```csharp
   AddressLine.TryCreate(input, "StreetAddress");
   AddressLine.TryCreate(input, "ApartmentNumber");
   ```

4. **Don't Trim User Input**: `AddressLine` preserves spacing, so trim before passing if needed:
   ```csharp
   var (result, line) = AddressLine.TryCreate(userInput?.Trim());
   ```

---

## Summary

? **Created**: `AddressLine` primitive for optional street addresses  
? **Validated**: Maximum 200 characters when value is provided  
? **Tested**: 23 comprehensive tests, all passing  
? **Ready**: For use in Address and other domain types  

The `AddressLine` primitive follows the same patterns as other validated primitives in your library while providing the flexibility needed for optional address components!
