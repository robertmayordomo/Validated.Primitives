# Validated.Primitives.PostalCode

A validated postal code primitive with country-specific format validation, supporting 30+ countries worldwide and ensuring postal codes are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`PostalCode` is a validated value object that represents a postal code with country-specific format validation. It supports 30+ countries and enforces length validation (2-10 characters). Once created, a `PostalCode` instance is guaranteed to be valid for the specified country.

### Key Features

- **Country-Specific Validation** - Validates postal format according to country rules
- **International Support** - Supports 30+ countries worldwide
- **Length Validation** - Enforces 2-10 character limit
- **Country Detection** - Automatically associates with country code
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Postal Code

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, postalCode) = PostalCode.TryCreate(
    CountryCode.UnitedStates,
    "10001"
);

if (result.IsValid)
{
    Console.WriteLine(postalCode.Value);            // "10001"
    Console.WriteLine(postalCode.CountryCode);      // UnitedStates
    Console.WriteLine(postalCode.GetCountryName()); // "United States"
    Console.WriteLine(postalCode.ToString());       // "10001"
    
    // Use the validated postal code
    await ValidateShippingAddress(postalCode);
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
    public PostalCode PostalCode { get; set; }  // Always valid or null
    public string Street { get; set; }
    public string City { get; set; }
}

// Usage
var (result, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, userInput);
if (result.IsValid)
{
    var address = new Address
    {
        Id = Guid.NewGuid(),
        PostalCode = postalCode,  // Guaranteed valid
        Street = street,
        City = city
    };
    
    await _addressRepository.SaveAsync(address);
}
```

---

## ? Valid Postal Codes

### United States

```csharp
// Valid US ZIP codes
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedStates, "10001");     // ? 5-digit ZIP
var (r2, p2) = PostalCode.TryCreate(CountryCode.UnitedStates, "10001-5555"); // ? ZIP+4
var (r3, p3) = PostalCode.TryCreate(CountryCode.UnitedStates, "90210");     // ? Valid ZIP

foreach (var (result, postal) in new[] { (r1, p1), (r2, p2), (r3, p3) })
{
    Console.WriteLine($"{postal.Value}: {result.IsValid}");  // All true
}
```

### United Kingdom

```csharp
// Valid UK postcodes
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "SW1A 2AA"); // ? Central London
var (r2, p2) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "EC1A 1BB"); // ? London
var (r3, p3) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "W1A 0AX");  // ? London
var (r4, p4) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "M1 1AE");  // ? Manchester
```

### Germany

```csharp
// Valid German postcodes
var (r1, p1) = PostalCode.TryCreate(CountryCode.Germany, "10115");          // ? Berlin
var (r2, p2) = PostalCode.TryCreate(CountryCode.Germany, "80331");          // ? Munich
var (r3, p3) = PostalCode.TryCreate(CountryCode.Germany, "50667");          // ? Cologne
```

### Canada

```csharp
// Valid Canadian postcodes
var (r1, p1) = PostalCode.TryCreate(CountryCode.Canada, "M5H 2N2");         // ? Toronto
var (r2, p2) = PostalCode.TryCreate(CountryCode.Canada, "K1A 0B1");         // ? Ottawa
var (r3, p3) = PostalCode.TryCreate(CountryCode.Canada, "V6B 1A1");         // ? Vancouver
```

---

## ? Invalid Postal Codes

### Wrong Format for Country

```csharp
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedStates, "ABC123");    // ? Invalid US format
var (r2, p2) = PostalCode.TryCreate(CountryCode.Germany, "1234");           // ? Too short for Germany
var (r3, p3) = PostalCode.TryCreate(CountryCode.Canada, "12345");           // ? Wrong Canadian format
```

### Too Short

```csharp
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedStates, "1");         // ? Too short (min 2 chars)
var (r2, p2) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "A");        // ? Too short
```

### Too Long

```csharp
var longCode = new string('1', 15);
var (result, postal) = PostalCode.TryCreate(CountryCode.UnitedStates, longCode); // ? Too long (max 10 chars)

// result.IsValid == false
// result.Errors contains Length error
```

### Empty or Null

```csharp
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedStates, "");          // ? Empty string
var (r2, p2) = PostalCode.TryCreate(CountryCode.UnitedStates, null);        // ? Null value
var (r3, p3) = PostalCode.TryCreate(CountryCode.UnitedStates, "   ");       // ? Whitespace only
```

---

## ?? Real-World Examples

### Address Validation Service

```csharp
public class AddressValidationService
{
    public async Task<ValidationResult> ValidateAddress(AddressRequest request)
    {
        // Validate postal code
        var (postalResult, postalCode) = PostalCode.TryCreate(request.Country, request.PostalCode);
        if (!postalResult.IsValid)
        {
            return ValidationResult.Failed(
                $"Invalid postal code: {postalResult.ToBulletList()}"
            );
        }

        // Validate other address components
        var (cityResult, city) = City.TryCreate(request.City);
        if (!cityResult.IsValid)
        {
            return ValidationResult.Failed($"Invalid city: {cityResult.ToBulletList()}");
        }

        // Check address deliverability
        var deliverability = await _postalService.ValidateAddressAsync(
            postalCode.Value,
            city.Value,
            request.Country
        );

        if (!deliverability.IsDeliverable)
        {
            return ValidationResult.Failed("Address is not deliverable");
        }

        return ValidationResult.Success();
    }
}
```

### Shipping Calculator

```csharp
public class ShippingCalculator
{
    public async Task<ShippingQuote> CalculateShipping(
        PostalCode originPostal,
        PostalCode destinationPostal,
        decimal weight)
    {
        // Validate postal codes are from supported countries
        if (!IsSupportedCountry(originPostal.CountryCode) ||
            !IsSupportedCountry(destinationPostal.CountryCode))
        {
            throw new ArgumentException("Unsupported country for shipping");
        }

        // Calculate distance based on postal codes
        var distance = await _geoService.CalculateDistanceAsync(
            originPostal.Value,
            destinationPostal.Value,
            originPostal.CountryCode,
            destinationPostal.CountryCode
        );

        // Calculate shipping cost
        var baseRate = CalculateBaseRate(distance, weight);
        var fuelSurcharge = CalculateFuelSurcharge(distance);

        return new ShippingQuote
        {
            OriginPostal = originPostal.Value,
            DestinationPostal = destinationPostal.Value,
            Distance = distance,
            BaseRate = baseRate,
            FuelSurcharge = fuelSurcharge,
            TotalCost = baseRate + fuelSurcharge
        };
    }

    private bool IsSupportedCountry(CountryCode countryCode)
    {
        return countryCode == CountryCode.UnitedStates ||
               countryCode == CountryCode.Canada ||
               countryCode == CountryCode.UnitedKingdom;
    }
}
```

### Tax Calculation Service

```csharp
public class TaxCalculationService
{
    public async Task<TaxCalculation> CalculateTax(
        PostalCode postalCode,
        decimal subtotal)
    {
        // Get tax rates based on postal code
        var taxRates = await _taxService.GetTaxRatesAsync(
            postalCode.Value,
            postalCode.CountryCode
        );

        var provincialTax = subtotal * taxRates.ProvincialRate;
        var federalTax = subtotal * taxRates.FederalRate;

        return new TaxCalculation
        {
            Subtotal = subtotal,
            ProvincialTax = provincialTax,
            FederalTax = federalTax,
            TotalTax = provincialTax + federalTax,
            Total = subtotal + provincialTax + federalTax,
            TaxRegion = $"{postalCode.GetCountryName()} - {postalCode.Value}"
        };
    }
}
```

### Address Autocomplete API

```csharp
[ApiController]
[Route("api/addresses")]
public class AddressController : ControllerBase
{
    [HttpGet("lookup")]
    public async Task<IActionResult> LookupAddress(
        [FromQuery] string postalCode,
        [FromQuery] CountryCode country)
    {
        // Validate postal code
        var (result, validatedPostal) = PostalCode.TryCreate(country, postalCode);
        if (!result.IsValid)
        {
            return BadRequest(new
            {
                Field = "PostalCode",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        // Lookup address suggestions
        var suggestions = await _addressService.GetAddressSuggestionsAsync(
            validatedPostal.Value,
            validatedPostal.CountryCode
        );

        return Ok(new
        {
            PostalCode = validatedPostal.Value,
            Country = validatedPostal.GetCountryName(),
            Suggestions = suggestions.Select(s => new
            {
                Street = s.Street,
                City = s.City,
                Region = s.Region
            })
        });
    }
}
```

### E-Commerce Checkout

```csharp
public class CheckoutService
{
    public async Task<CheckoutResult> ProcessCheckout(CheckoutRequest request)
    {
        // Validate shipping postal code
        var (shippingPostalResult, shippingPostal) = PostalCode.TryCreate(
            request.ShippingCountry,
            request.ShippingPostalCode
        );

        if (!shippingPostalResult.IsValid)
        {
            return CheckoutResult.ValidationFailed(
                "Shipping Address",
                shippingPostalResult.Errors
            );
        }

        // Validate billing postal code if different
        PostalCode billingPostal = shippingPostal;
        if (request.BillingPostalCode != request.ShippingPostalCode ||
            request.BillingCountry != request.ShippingCountry)
        {
            var (billingPostalResult, billingPostalValue) = PostalCode.TryCreate(
                request.BillingCountry,
                request.BillingPostalCode
            );

            if (!billingPostalResult.IsValid)
            {
                return CheckoutResult.ValidationFailed(
                    "Billing Address",
                    billingPostalResult.Errors
                );
            }

            billingPostal = billingPostalValue;
        }

        // Calculate shipping cost
        var shippingCost = await _shippingService.CalculateCostAsync(
            shippingPostal,
            request.Items.Sum(i => i.Weight)
        );

        // Process payment and create order
        var order = await CreateOrderAsync(request, shippingPostal, billingPostal, shippingCost);

        return CheckoutResult.Success(order.Id);
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, userInput);

if (result.IsValid)
{
    // Use the validated postal code
    await ProcessAddress(postalCode);
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

### Domain Model Usage

```csharp
public class Location
{
    public Guid Id { get; set; }
    public PostalCode PostalCode { get; set; }
    public string Description { get; set; }
}

// Creating a location
var (postalResult, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, input);
if (postalResult.IsValid)
{
    var location = new Location
    {
        Id = Guid.NewGuid(),
        PostalCode = postalCode,  // Type-safe and validated
        Description = description
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, "10001");

// Serialize
string json = JsonSerializer.Serialize(postalCode);
// {"Value":"10001","CountryCode":"UnitedStates"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<PostalCode>(json);
Console.WriteLine(deserialized.Value);       // "10001"
Console.WriteLine(deserialized.CountryCode); // UnitedStates
```

---

## ?? Related Documentation

- [Address README](address_readme.md) - Complete address validation including postal codes
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated postal code string |
| `CountryCode` | `CountryCode` | The country code for the postal code |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(CountryCode countryCode, string value, string propertyName = "PostalCode")` | `(ValidationResult, PostalCode?)` | Static factory method to create validated postal code |
| `GetCountryName()` | `string` | Returns display-friendly country name |
| `ToString()` | `string` | Returns the postal code value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Postal code cannot be null, empty, or whitespace only |
| **Valid Format** | Must conform to valid postal code format |
| **Length** | Must be between 2 and 10 characters |
| **Country Format** | Must conform to country-specific format (when country is specified) |

---

## ?? Supported Countries

The `PostalCode` type supports country-specific validation for 30+ countries:

### North America
- **United States** - 5-digit ZIP or ZIP+4 (12345 or 12345-6789)
- **Canada** - A1A 1A1 format
- **Mexico** - 5-digit postal codes

### Europe
- **United Kingdom** - Various formats (SW1A 2AA, EC1A 1BB, etc.)
- **Germany** - 5-digit postal codes
- **France** - 5-digit postal codes
- **Italy** - 5-digit postal codes
- **Spain** - 5-digit postal codes
- **Netherlands** - 4 digits + 2 letters (1234 AB)
- **Belgium** - 4-digit postal codes
- **Switzerland** - 4-digit postal codes
- **Austria** - 4-digit postal codes
- **Sweden** - 5-digit postal codes (123 45)
- **Norway** - 4-digit postal codes
- **Denmark** - 4-digit postal codes
- **Finland** - 5-digit postal codes
- **Poland** - 5-digit postal codes with hyphen (12-345)
- **Czech Republic** - 5-digit postal codes with space (123 45)
- **Hungary** - 4-digit postal codes
- **Portugal** - 7-digit postal codes with hyphen (1234-567)
- **Ireland** - Various formats (A65 F4E2, D02 AF30, etc.)

### Asia-Pacific
- **Japan** - 7-digit postal codes with hyphen (123-4567)
- **South Korea** - 5-digit postal codes
- **Australia** - 4-digit postal codes
- **New Zealand** - 4-digit postal codes
- **China** - 6-digit postal codes
- **India** - 6-digit postal codes

### Other Regions
- **Brazil** - 8-digit postal codes with hyphen (12345-678)
- **South Africa** - 4-digit postal codes

---

## ??? Security Considerations

### Postal Code Validation

```csharp
// ? DO: Validate before use
var (result, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid postal code");
}

// ? DON'T: Trust user input without validation
var postalCode = userInput;  // Dangerous!
await ProcessAddress(postalCode);
```

### Preventing Address Enumeration

```csharp
// ? DO: Use same response for existing and non-existing postal codes
public async Task<Result> LookupAddress(string postalCodeInput, CountryCode country)
{
    var (result, postalCode) = PostalCode.TryCreate(country, postalCodeInput);
    if (!result.IsValid)
    {
        // Generic message
        return Result.Success("Address lookup completed");
    }

    var addresses = await _addressService.FindByPostalCodeAsync(postalCode.Value);
    if (addresses.Any())
    {
        // Return addresses
        return Result.Success(addresses);
    }

    // Same response whether addresses exist or not
    return Result.Success("Address lookup completed");
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize postal code input
public string SanitizePostalCode(string input, CountryCode country)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove all non-alphanumeric characters except spaces and hyphens
    var sanitized = new string(input.Where(c => 
        char.IsLetterOrDigit(c) || c == ' ' || c == '-').ToArray());
    
    // For some countries, convert to uppercase
    if (country == CountryCode.Canada || country == CountryCode.UnitedKingdom)
    {
        sanitized = sanitized.ToUpperInvariant();
    }
    
    return sanitized.Trim();
}

// Usage
var sanitized = SanitizePostalCode(userInput, CountryCode.Canada);
var (result, postalCode) = PostalCode.TryCreate(CountryCode.Canada, sanitized);
```

### Rate Limiting for Address Lookups

```csharp
// ? DO: Implement rate limiting for postal code lookups
public async Task<LookupResult> LookupByPostalCode(PostalCode postalCode)
{
    var rateLimitKey = $"postal_lookup:{postalCode.Value}";
    var count = await _cache.GetAsync<int>(rateLimitKey);
    
    if (count >= 10)  // Max 10 lookups per hour per postal code
    {
        return LookupResult.RateLimited();
    }
    
    var addresses = await _addressService.FindByPostalCodeAsync(postalCode.Value);
    await _cache.SetAsync(rateLimitKey, count + 1, TimeSpan.FromHours(1));
    
    return LookupResult.Success(addresses);
}
```

### Data Privacy

```csharp
// ? DO: Mask postal codes in logs and external communications
public void LogAddressLookup(PostalCode postalCode, string userId)
{
    // Only log first 3 digits for privacy
    var maskedPostal = postalCode.Value.Length >= 3 
        ? postalCode.Value.Substring(0, 3) + "***"
        : "***";
    
    _logger.LogInformation(
        "Address lookup by user {UserId} for postal code {MaskedPostal} in {Country}",
        userId,
        maskedPostal,
        postalCode.GetCountryName()
    );
}
```
