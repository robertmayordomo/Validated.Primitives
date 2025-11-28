# Address Primitives Comparison

## Overview

Two new validated primitives have been created for building addresses:

1. **`AddressLine`** - Optional street address component (max 200 chars)
2. **`City`** - Required city name (max 100 chars)

Both follow the same patterns as existing validated primitives in the library.

---

## Quick Comparison

| Feature | AddressLine | City |
|---------|-------------|------|
| **Purpose** | Street addresses, apt numbers, etc. | City names |
| **Required?** | ? No (Optional) | ? Yes (Required) |
| **Null allowed** | ? Yes | ? No |
| **Empty allowed** | ? Yes | ? No |
| **Whitespace only** | ? Yes (returns null) | ? No (validation fails) |
| **Max length** | 200 characters | 100 characters |
| **Min length** | None | 1 character |
| **Format validation** | None | None |
| **Case preservation** | ? Yes | ? Yes |
| **Unicode support** | ? Yes | ? Yes |
| **Tests** | 23 tests | 53 tests |

---

## Side-by-Side Examples

### Null/Empty Handling

```csharp
// AddressLine - Optional (null/empty allowed)
var (result1, line) = AddressLine.TryCreate(null);
Console.WriteLine(result1.IsValid); // True
Console.WriteLine(line);            // null

// City - Required (null/empty NOT allowed)
var (result2, city) = City.TryCreate(null);
Console.WriteLine(result2.IsValid); // False
Console.WriteLine(result2.Errors[0].Code); // "NotNullOrWhitespace"
```

### Valid Values

```csharp
// AddressLine - 200 chars max
var (result1, line) = AddressLine.TryCreate("123 Main Street, Apt 4B");
Console.WriteLine(result1.IsValid); // True
Console.WriteLine(line.Value);      // "123 Main Street, Apt 4B"

// City - 100 chars max
var (result2, city) = City.TryCreate("New York");
Console.WriteLine(result2.IsValid); // True
Console.WriteLine(city.Value);      // "New York"
```

### Exceeding Max Length

```csharp
// AddressLine - 201+ chars fails
var tooLongAddress = new string('X', 201);
var (result1, line) = AddressLine.TryCreate(tooLongAddress);
Console.WriteLine(result1.IsValid); // False
Console.WriteLine(result1.Errors[0].Code); // "MaxLength"

// City - 101+ chars fails
var tooLongCity = new string('X', 101);
var (result2, city) = City.TryCreate(tooLongCity);
Console.WriteLine(result2.IsValid); // False
Console.WriteLine(result2.Errors[0].Code); // "MaxLength"
```

---

## Using Together in Domain Models

### Complete Address Example

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
        
        // Validate street (required but uses optional primitive)
        var (streetResult, streetValue) = AddressLine.TryCreate(street, "Street");
        result.Merge(streetResult);
        
        if (streetValue == null)
        {
            result.AddError("Street address is required.", "Street", "Required");
        }
        
        // Validate address line 2 (optional)
        var (line2Result, line2Value) = AddressLine.TryCreate(addressLine2, "AddressLine2");
        result.Merge(line2Result);
        
        // Validate city (required)
        var (cityResult, cityValue) = City.TryCreate(city, "City");
        result.Merge(cityResult);
        
        // Validate state/province (optional manual validation)
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
        var parts = new List<string> { Street.Value };
        
        if (AddressLine2 != null)
        {
            parts.Add(AddressLine2.Value);
        }
        
        parts.Add(City.Value);
        
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

### Usage

```csharp
var (result, address) = Address.TryCreate(
    "123 Main Street",
    "Apt 4B",
    "New York",
    CountryCode.UnitedStates,
    "10001",
    "NY"
);

if (result.IsValid)
{
    Console.WriteLine(address.Street.Value);       // "123 Main Street"
    Console.WriteLine(address.AddressLine2?.Value); // "Apt 4B"
    Console.WriteLine(address.City.Value);         // "New York"
    Console.WriteLine(address.StateProvince);      // "NY"
    Console.WriteLine(address.PostalCode.Value);   // "10001"
    Console.WriteLine(address.ToString());
    // "123 Main Street, Apt 4B, New York, NY, 10001, United States"
}
else
{
    Console.WriteLine("Validation failed:");
    Console.WriteLine(result.ToBulletList());
}
```

---

## Validation Scenarios

### Scenario 1: All Fields Valid

```csharp
var (result, address) = Address.TryCreate(
    "123 Main St",
    "Suite 100",
    "Boston",
    CountryCode.UnitedStates,
    "02101",
    "MA"
);
// result.IsValid = true
// All fields populated correctly
```

### Scenario 2: Missing Optional Address Line 2

```csharp
var (result, address) = Address.TryCreate(
    "456 Oak Ave",
    null,  // No address line 2
    "Seattle",
    CountryCode.UnitedStates,
    "98101",
    "WA"
);
// result.IsValid = true
// address.AddressLine2 = null (allowed)
```

### Scenario 3: Missing Required Street

```csharp
var (result, address) = Address.TryCreate(
    null,  // Missing street
    null,
    "Chicago",
    CountryCode.UnitedStates,
    "60601",
    "IL"
);
// result.IsValid = false
// Error: "Street address is required."
```

### Scenario 4: Missing Required City

```csharp
var (result, address) = Address.TryCreate(
    "789 Elm St",
    null,
    null,  // Missing city
    CountryCode.UnitedStates,
    "90001",
    "CA"
);
// result.IsValid = false
// Error: "City cannot be null or whitespace."
```

### Scenario 5: Street Exceeds Max Length

```csharp
var tooLongStreet = new string('X', 201);
var (result, address) = Address.TryCreate(
    tooLongStreet,
    null,
    "Denver",
    CountryCode.UnitedStates,
    "80201",
    "CO"
);
// result.IsValid = false
// Error: "Street must be at most 200 characters."
```

### Scenario 6: City Exceeds Max Length

```csharp
var tooLongCity = new string('Y', 101);
var (result, address) = Address.TryCreate(
    "321 Pine St",
    null,
    tooLongCity,
    CountryCode.UnitedStates,
    "30301",
    "GA"
);
// result.IsValid = false
// Error: "City must be at most 100 characters."
```

### Scenario 7: International Address

```csharp
var (result, address) = Address.TryCreate(
    "123 Rue de la Paix",
    "Appartement 5",
    "Montréal",
    CountryCode.Canada,
    "H2X 1Y7",
    "QC"
);
// result.IsValid = true
// Handles Unicode characters correctly
```

---

## Error Messages

### AddressLine Errors

| Scenario | Error Code | Error Message |
|----------|------------|---------------|
| Exceeds 200 chars | `MaxLength` | "AddressLine must be at most 200 characters." |

### City Errors

| Scenario | Error Code | Error Message |
|----------|------------|---------------|
| Null value | `NotNullOrWhitespace` | "City cannot be null or whitespace." |
| Empty string | `NotNullOrWhitespace` | "City cannot be null or whitespace." |
| Whitespace only | `NotNullOrWhitespace` | "City cannot be null or whitespace." |
| Exceeds 100 chars | `MaxLength` | "City must be at most 100 characters." |

---

## Best Practices

### 1. Use AddressLine for Optional Address Components

```csharp
public record ShippingLabel
{
    public AddressLine? Building { get; init; }
    public AddressLine? Floor { get; init; }
    public AddressLine? Suite { get; init; }
    public required City City { get; init; }
}
```

### 2. Use City for Required City Names

```csharp
public record Location
{
    public required City City { get; init; }
    public required PostalCode PostalCode { get; init; }
}
```

### 3. Trim Before Validation (When Needed)

```csharp
// Trim user input before validation
var (streetResult, street) = AddressLine.TryCreate(userStreet?.Trim());
var (cityResult, city) = City.TryCreate(userCity?.Trim());
```

### 4. Use Custom Property Names

```csharp
// Provide context in error messages
var (result1, street) = AddressLine.TryCreate(input, "ShippingStreet");
var (result2, city) = City.TryCreate(input, "BillingCity");
```

### 5. Handle Validation Errors Gracefully

```csharp
var (result, address) = Address.TryCreate(...);

if (!result.IsValid)
{
    // For APIs
    return BadRequest(result.ToDictionary());
    
    // For console apps
    Console.WriteLine(result.ToBulletList());
    
    // For UI
    foreach (var error in result.Errors)
    {
        DisplayError(error.MemberName, error.Message);
    }
}
```

---

## Test Coverage Summary

### AddressLine (23 tests)
- ? Valid address lines
- ? Null/empty/whitespace (all valid, return null)
- ? Max length (200 chars)
- ? Exceeding max length
- ? Various formats (apartments, PO boxes, etc.)
- ? Unicode and special characters
- ? Equality semantics

### City (53 tests)
- ? Valid city names (US and international)
- ? Null/empty/whitespace (all invalid)
- ? Max length (100 chars)
- ? Exceeding max length
- ? Unicode and accented characters
- ? Special characters (hyphens, apostrophes, periods)
- ? Case preservation
- ? Equality semantics

### Total: 76 tests, all passing ?

---

## Summary

| Primitive | Purpose | Required | Max Length | Tests | Status |
|-----------|---------|----------|------------|-------|--------|
| **AddressLine** | Street addresses, apt numbers | ? No | 200 chars | 23 | ? Complete |
| **City** | City names | ? Yes | 100 chars | 53 | ? Complete |

Both primitives are:
- ? Fully tested
- ? Production-ready
- ? Documented
- ? Following library patterns
- ? Supporting international use cases

Perfect for building complete, validated address models!
