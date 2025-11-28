# City Primitive - Usage Guide

## Overview

`City` is a validated primitive type for city names. It's a **required field** with a maximum length of 100 characters. Unlike `AddressLine`, city names cannot be null or empty.

## Created Files

- **`src/Valdiated.Primatives/ValueObjects/City.cs`** - The primitive implementation
- **`tests/Valdiated.Primatives.Tests/ValueObjects/CityTests.cs`** - 53 comprehensive tests

## Features

? **Required** - Cannot be null, empty, or whitespace  
? **Max Length Validation** - 100 characters maximum  
? **Preserves Formatting** - Keeps original spacing and case  
? **Custom Error Messages** - Customize property names in validation errors  
? **Record Semantics** - Value-based equality comparison  
? **Immutable** - Thread-safe by design  
? **International Support** - Supports Unicode, accents, and special characters  

## Usage Examples

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Valid city name
var (result, city) = City.TryCreate("New York");
if (result.IsValid)
{
    Console.WriteLine(city.Value); // "New York"
}

// Null is invalid
var (result2, city2) = City.TryCreate(null);
Console.WriteLine(result2.IsValid); // False
Console.WriteLine(result2.Errors[0].Code); // "NotNullOrWhitespace"

// Empty string is invalid
var (result3, city3) = City.TryCreate("");
Console.WriteLine(result3.IsValid); // False

// Whitespace only is invalid
var (result4, city4) = City.TryCreate("   ");
Console.WriteLine(result4.IsValid); // False
```

### Max Length Validation

```csharp
// 100 characters - valid
var valid100 = new string('X', 100);
var (result1, city1) = City.TryCreate(valid100);
Console.WriteLine(result1.IsValid); // True
Console.WriteLine(city1.Value.Length); // 100

// 101 characters - invalid
var invalid101 = new string('X', 101);
var (result2, city2) = City.TryCreate(invalid101);
Console.WriteLine(result2.IsValid); // False
Console.WriteLine(city2); // null
Console.WriteLine(result2.Errors[0].Code); // "MaxLength"
Console.WriteLine(result2.Errors[0].Message); 
// "City must be at most 100 characters."
```

### Custom Property Names

```csharp
var tooLong = new string('a', 101);
var (result, city) = City.TryCreate(tooLong, "CityName");

if (!result.IsValid)
{
    Console.WriteLine(result.Errors[0].MemberName); // "CityName"
    Console.WriteLine(result.Errors[0].Message);
    // "CityName must be at most 100 characters."
}
```

### International City Names

```csharp
// All of these are valid:
var internationalCities = new[]
{
    "London",
    "Paris",
    "Tokyo",
    "Montréal",        // Accented characters
    "São Paulo",       // Unicode
    "München",         // German umlaut
    "København",       // Danish characters
    "Kraków",          // Polish characters
    "Mexico City",     // With space
    "St. Louis",       // With period
    "Winston-Salem",   // With hyphen
    "L'Aquila"         // With apostrophe
};

foreach (var cityName in internationalCities)
{
    var (result, city) = City.TryCreate(cityName);
    Console.WriteLine($"{cityName}: {result.IsValid}"); // All true
}
```

### Using in Domain Models

```csharp
public record ShippingAddress
{
    public required AddressLine? Street { get; init; }
    public required City City { get; init; }
    public required PostalCode PostalCode { get; init; }
    
    public static (ValidationResult Result, ShippingAddress? Value) TryCreate(
        string? street,
        string city,
        CountryCode country,
        string postalCode)
    {
        var result = ValidationResult.Success();
        
        // Validate street (optional)
        var (streetResult, streetValue) = AddressLine.TryCreate(street, "Street");
        result.Merge(streetResult);
        
        // Validate city (required)
        var (cityResult, cityValue) = City.TryCreate(city, "City");
        result.Merge(cityResult);
        
        // Validate postal code
        var (postalResult, postalValue) = PostalCode.TryCreate(country, postalCode);
        result.Merge(postalResult);
        
        if (!result.IsValid)
        {
            return (result, null);
        }
        
        var address = new ShippingAddress
        {
            Street = streetValue,
            City = cityValue!,
            PostalCode = postalValue!
        };
        
        return (result, address);
    }
}

// Usage
var (result, address) = ShippingAddress.TryCreate(
    "123 Main Street",
    "New York",
    CountryCode.UnitedStates,
    "10001"
);

if (result.IsValid)
{
    Console.WriteLine(address.Street?.Value);  // "123 Main Street"
    Console.WriteLine(address.City.Value);     // "New York"
    Console.WriteLine(address.PostalCode.Value); // "10001"
}
```

### Updating the Existing Address Domain Type

You can update the `Address` domain type to use `City` primitive:

```csharp
public sealed record Address
{
    public required AddressLine Street { get; init; }
    public AddressLine? AddressLine2 { get; init; }
    public required City City { get; init; }
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
        
        // Validate city using City primitive
        var (cityResult, cityValue) = City.TryCreate(city, "City");
        result.Merge(cityResult);
        
        // Validate state/province if provided
        if (stateProvince != null && stateProvince.Length > 100)
        {
            result.AddError("State/Province cannot exceed 100 characters.", "StateProvince", "MaxLength");
        }
        
        // Validate postal code
        var (postalResult, postalValue) = PostalCode.TryCreate(country, postalCode, "PostalCode");
        result.Merge(postalResult);
        
        if (!result.IsValid)
        {
            return (result, null);
        }
        
        var address = new Address
        {
            Street = streetValue!,
            AddressLine2 = line2Value,
            City = cityValue!,
            StateProvince = stateProvince?.Trim(),
            PostalCode = postalValue!
        };
        
        return (result, address);
    }
    
    public override string ToString()
    {
        var parts = new List<string> { Street.Value, City.Value };
        
        if (AddressLine2 != null)
        {
            parts.Insert(1, AddressLine2.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(StateProvince))
        {
            parts.Add(StateProvince);
        }
        
        parts.Add(PostalCode.Value);
        parts.Add(PostalCode.GetCountryName());

        return string.Join(", ", parts);
    }
}
```

## Test Coverage

### Test Categories

- ? Valid city names (US and international)
- ? Null/empty/whitespace validation
- ? Max length validation (100 chars)
- ? Exceeding max length (101+ chars)
- ? Custom property names
- ? Unicode and accented characters
- ? Special characters (hyphens, apostrophes, periods)
- ? Case preservation
- ? Equality semantics
- ? ToString behavior

### Running Tests

```bash
# Run all City tests
dotnet test --filter "FullyQualifiedName~CityTests"

# Run specific test
dotnet test --filter "FullyQualifiedName~CityTests.TryCreate_WithValidCity_ShouldSucceed"
```

## Key Differences from AddressLine

### Required vs Optional

| Feature | City | AddressLine |
|---------|------|-------------|
| Null allowed | ? No | ? Yes |
| Empty allowed | ? No | ? Yes |
| Whitespace allowed | ? No | ? Yes |
| Max length | 100 chars | 200 chars |

```csharp
// City - Required
var (result1, city) = City.TryCreate(null);
Console.WriteLine(result1.IsValid); // False - validation fails

// AddressLine - Optional
var (result2, line) = AddressLine.TryCreate(null);
Console.WriteLine(result2.IsValid); // True - returns null but valid
```

### Validation Behavior

```csharp
// City requires a value
var (result1, city) = City.TryCreate("");
// result1.IsValid = false
// result1.Errors[0].Code = "NotNullOrWhitespace"

// AddressLine allows empty
var (result2, line) = AddressLine.TryCreate("");
// result2.IsValid = true
// line = null
```

## Integration with Existing Types

The `City` primitive integrates seamlessly with your existing validated primitives:

```csharp
public record FullAddress
{
    public AddressLine? Street { get; init; }
    public City City { get; init; }
    public PostalCode PostalCode { get; init; }
    public EmailAddress? Email { get; init; }
    public PhoneNumber? Phone { get; init; }
}
```

## API Reference

### Constructor
```csharp
private City(string value, string propertyName = "City")
```

### Static Methods

#### TryCreate
```csharp
public static (ValidationResult Result, City? Value) TryCreate(
    string value,
    string propertyName = "City")
```

**Parameters:**
- `value` - The city name to validate (required, cannot be null)
- `propertyName` - Optional custom property name for error messages (default: "City")

**Returns:**
- `ValidationResult` - Contains validation errors if any
- `City?` - The validated city, or null if validation failed

**Behavior:**
- Returns `(Failure, null)` if value is null, empty, or whitespace
- Returns `(Failure, null)` if value exceeds 100 characters
- Returns `(Success, City)` if value is 1-100 characters

### Instance Methods

#### ToString
```csharp
public override string ToString()
```
Returns the city name as a string.

## Error Codes

| Code | Description | Example |
|------|-------------|---------|
| `NotNullOrWhitespace` | City name is null, empty, or whitespace | "City cannot be null or whitespace." |
| `MaxLength` | City name exceeds 100 characters | "City must be at most 100 characters." |

## Best Practices

1. **Always Validate**: City is required, so always check validation result:
   ```csharp
   var (result, city) = City.TryCreate(userInput);
   if (!result.IsValid)
   {
       // Handle validation errors
       Console.WriteLine(result.ToBulletList());
       return;
   }
   // Safe to use city here
   ```

2. **Use Custom Property Names**: Provide meaningful property names for better error messages:
   ```csharp
   City.TryCreate(input, "ShippingCity");
   City.TryCreate(input, "BillingCity");
   ```

3. **Trim Before Validation**: If you want to trim whitespace, do it before passing to TryCreate:
   ```csharp
   var (result, city) = City.TryCreate(userInput?.Trim());
   ```

4. **Handle Special Characters**: City names can contain hyphens, apostrophes, periods, etc.:
   ```csharp
   // All valid:
   City.TryCreate("Winston-Salem");   // Hyphen
   City.TryCreate("L'Aquila");        // Apostrophe
   City.TryCreate("St. Louis");       // Period
   ```

5. **International Names**: Support Unicode for international city names:
   ```csharp
   City.TryCreate("Montréal");  // French
   City.TryCreate("São Paulo"); // Portuguese
   City.TryCreate("München");   // German
   City.TryCreate("København"); // Danish
   ```

## Comparison with Manual Validation

### Before (Manual Validation)
```csharp
public static (ValidationResult, Address?) TryCreate(string city, ...)
{
    var result = ValidationResult.Success();
    
    if (string.IsNullOrWhiteSpace(city))
    {
        result.AddError("City is required.", "City", "Required");
    }
    else if (city.Length > 100)
    {
        result.AddError("City cannot exceed 100 characters.", "City", "MaxLength");
    }
    
    // ... rest of validation
}
```

### After (Using City Primitive)
```csharp
public static (ValidationResult, Address?) TryCreate(string city, ...)
{
    var result = ValidationResult.Success();
    
    var (cityResult, cityValue) = City.TryCreate(city, "City");
    result.Merge(cityResult);
    
    // ... rest of validation
}
```

**Benefits:**
- ? Less code duplication
- ? Consistent validation logic
- ? Type safety
- ? Reusable across domain models
- ? Centralized validation rules

## Common Patterns

### Pattern 1: Required City with Trimming
```csharp
var input = "  New York  ";
var (result, city) = City.TryCreate(input.Trim());
// city.Value = "New York" (trimmed)
```

### Pattern 2: City with Custom Error Context
```csharp
var (result, city) = City.TryCreate(userInput, "ShippingCity");
if (!result.IsValid)
{
    // Error will reference "ShippingCity" instead of "City"
    return BadRequest(result.ToDictionary());
}
```

### Pattern 3: Multiple Cities in One Model
```csharp
public record MultiLocationOrder
{
    public City OriginCity { get; init; }
    public City DestinationCity { get; init; }
    
    public static (ValidationResult, MultiLocationOrder?) TryCreate(
        string originCity,
        string destinationCity)
    {
        var result = ValidationResult.Success();
        
        var (originResult, originValue) = City.TryCreate(originCity, "OriginCity");
        result.Merge(originResult);
        
        var (destResult, destValue) = City.TryCreate(destinationCity, "DestinationCity");
        result.Merge(destResult);
        
        if (!result.IsValid)
            return (result, null);
            
        return (result, new MultiLocationOrder
        {
            OriginCity = originValue!,
            DestinationCity = destValue!
        });
    }
}
```

---

## Summary

? **Created**: `City` primitive for required city names  
? **Validated**: Required field with 100 character maximum  
? **Tested**: 53 comprehensive tests, all passing  
? **Ready**: For use in Address and other domain types  

The `City` primitive provides:
- **Type Safety**: Compile-time guarantees that city names are valid
- **Consistency**: Same validation logic everywhere
- **Reusability**: Use across multiple domain models
- **International Support**: Handles Unicode and special characters
- **Clear Errors**: Detailed validation feedback with custom property names

Perfect for use with `AddressLine` and `PostalCode` to build complete, validated addresses!
