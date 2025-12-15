# Validated.Primitives.Latitude

A validated latitude coordinate primitive that enforces geographic coordinate validation (-90 to +90 degrees) with configurable decimal precision, ensuring latitude values are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`Latitude` is a validated value object that represents a latitude coordinate in degrees. It enforces the valid range of -90 to +90 degrees and supports configurable decimal precision (0-8 decimal places). Once created, a `Latitude` instance is guaranteed to be valid.

### Key Features

- **Geographic Range** - Enforces -90 to +90 degree range
- **Decimal Precision** - Configurable decimal places (0-8)
- **Hemisphere Detection** - North/South hemisphere identification
- **Cardinal Directions** - N/S direction formatting
- **GPS Precision** - Supports up to 8 decimal places for precise coordinates
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other numeric types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Latitude

```csharp
using Validated.Primitives.ValueObjects.Geospatial;

// Create with validation (default 6 decimal places for GPS)
var (result, latitude) = Latitude.TryCreate(40.7128m);

if (result.IsValid)
{
    Console.WriteLine(latitude.Value);                    // 40.7128
    Console.WriteLine(latitude.ToString());              // "40.7128?"
    Console.WriteLine(latitude.GetHemisphere());         // "North"
    Console.WriteLine(latitude.GetCardinalDirection());  // "N"
    Console.WriteLine(latitude.ToCardinalString());      // "40.7128? N"
    
    // Use the validated latitude
    ProcessCoordinate(latitude);
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

### Specifying Decimal Precision

```csharp
// Different decimal precision levels
var (r1, lat1) = Latitude.TryCreate(40.7128m, 6);       // GPS precision (default)
var (r2, lat2) = Latitude.TryCreate(40.7128m, 4);       // City-level precision
var (r3, lat3) = Latitude.TryCreate(40.7128m, 2);       // Regional precision
var (r4, lat4) = Latitude.TryCreate(40.7128m, 0);       // Degree precision only

Console.WriteLine(lat1.ToString());  // "40.712800?"
Console.WriteLine(lat2.ToString());  // "40.7128?"
Console.WriteLine(lat3.ToString());  // "40.71?"
Console.WriteLine(lat4.ToString());  // "41?" (rounded)
```

### Using in Domain Models

```csharp
public class GeographicLocation
{
    public Guid Id { get; set; }
    public Latitude Latitude { get; set; }
    public Longitude Longitude { get; set; }
    public string Name { get; set; }
}

// Usage
var (latResult, latitude) = Latitude.TryCreate(40.7128m);
var (lonResult, longitude) = Longitude.TryCreate(-74.0060m);

if (latResult.IsValid && lonResult.IsValid)
{
    var location = new GeographicLocation
    {
        Id = Guid.NewGuid(),
        Latitude = latitude,    // Guaranteed valid
        Longitude = longitude,  // Guaranteed valid
        Name = "New York City"
    };
}
```

---

## ? Valid Latitude Values

### Northern Hemisphere

```csharp
// Valid northern latitudes (0 to 90 degrees)
var (r1, l1) = Latitude.TryCreate(90.0m);              // ? North Pole
var (r2, l2) = Latitude.TryCreate(40.7128m);           // ? New York City
var (r3, l3) = Latitude.TryCreate(51.5074m);           // ? London
var (r4, l4) = Latitude.TryCreate(35.6762m);           // ? Tokyo
var (r5, l5) = Latitude.TryCreate(0.0m);               // ? Equator (Northern)

foreach (var (result, lat) in new[] { (r1, l1), (r2, l2), (r3, l3), (r4, l4), (r5, l5) })
{
    Console.WriteLine($"{lat.Value}: {result.IsValid} - {lat.GetHemisphere()}");
}
```

### Southern Hemisphere

```csharp
// Valid southern latitudes (-90 to 0 degrees)
var (r1, l1) = Latitude.TryCreate(-90.0m);             // ? South Pole
var (r2, l2) = Latitude.TryCreate(-33.8688m);          // ? Sydney
var (r3, l3) = Latitude.TryCreate(-23.5505m);          // ? São Paulo
var (r4, l4) = Latitude.TryCreate(-37.8136m);          // ? Melbourne
var (r5, l5) = Latitude.TryCreate(-0.0001m);           // ? Just south of equator
```

### Different Precision Levels

```csharp
// Same coordinate with different precision
var coordinate = 40.7128m;

var (r1, l1) = Latitude.TryCreate(coordinate, 8);      // Maximum precision
var (r2, l2) = Latitude.TryCreate(coordinate, 6);      // GPS precision
var (r3, l3) = Latitude.TryCreate(coordinate, 4);      // City precision
var (r4, l4) = Latitude.TryCreate(coordinate, 2);      // Regional precision
var (r5, l5) = Latitude.TryCreate(coordinate, 0);      // Degree precision

Console.WriteLine($"8 decimals: {l1.ToString()}");
Console.WriteLine($"6 decimals: {l2.ToString()}");
Console.WriteLine($"4 decimals: {l3.ToString()}");
Console.WriteLine($"2 decimals: {l4.ToString()}");
Console.WriteLine($"0 decimals: {l5.ToString()}");
```

---

## ? Invalid Latitude Values

### Out of Range

```csharp
var (r1, l1) = Latitude.TryCreate(90.1m);              // ? Above maximum (90)
var (r2, l2) = Latitude.TryCreate(-90.1m);             // ? Below minimum (-90)
var (r3, l3) = Latitude.TryCreate(100.0m);             // ? Way above maximum
var (r4, l4) = Latitude.TryCreate(-100.0m);            // ? Way below minimum
```

### Invalid Decimal Places

```csharp
var (r1, l1) = Latitude.TryCreate(40.7128m, -1);       // ? Negative decimal places
var (r2, l2) = Latitude.TryCreate(40.7128m, 9);        // ? Too many decimal places (max 8)
var (r3, l3) = Latitude.TryCreate(40.7128m, 10);       // ? Way too many decimal places
```

### Too Many Decimals for Value

```csharp
var (r1, l1) = Latitude.TryCreate(40.7128m, 8);        // ? Value has only 4 decimals but asks for 8 precision
// This would fail because the value doesn't have enough decimal places
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, latitude) = Latitude.TryCreate(userInput, 6);

if (result.IsValid)
{
    // Use the validated latitude
    ProcessCoordinate(latitude);
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

### Coordinate Pair Pattern

```csharp
public class Coordinate
{
    public Latitude Latitude { get; set; }
    public Longitude Longitude { get; set; }
}

var (latResult, latitude) = Latitude.TryCreate(latValue);
var (lonResult, longitude) = Longitude.TryCreate(lonValue);

if (latResult.IsValid && lonResult.IsValid)
{
    var coordinate = new Coordinate
    {
        Latitude = latitude,
        Longitude = longitude
    };
}
```

### Domain Model Usage

```csharp
public class GeographicPoint
{
    public Guid Id { get; set; }
    public Latitude Latitude { get; set; }
    public Longitude Longitude { get; set; }
    public int Precision { get; set; }
}

// Creating a geographic point
var (latResult, latitude) = Latitude.TryCreate(latValue, precision);
var (lonResult, longitude) = Longitude.TryCreate(lonValue, precision);

if (latResult.IsValid && lonResult.IsValid)
{
    var point = new GeographicPoint
    {
        Id = Guid.NewGuid(),
        Latitude = latitude,
        Longitude = longitude,
        Precision = precision
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, latitude) = Latitude.TryCreate(40.7128m, 6);

// Serialize
string json = JsonSerializer.Serialize(latitude);
// {"Value":40.7128,"DecimalPlaces":6}

// Deserialize
var deserialized = JsonSerializer.Deserialize<Latitude>(json);
Console.WriteLine(deserialized.Value);         // 40.7128
Console.WriteLine(deserialized.DecimalPlaces); // 6
```

---

## ?? Related Documentation

- [Geospatial README](geospatial_readme.md) - Complete geospatial coordinate validation
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `decimal` | The validated latitude value in degrees |
| `DecimalPlaces` | `int` | The number of decimal places (0-8) |
| `MinValue` | `decimal` | Constant for minimum valid latitude (-90) |
| `MaxValue` | `decimal` | Constant for maximum valid latitude (90) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(decimal value, int decimalPlaces = 6, string propertyName = "Latitude")` | `(ValidationResult, Latitude?)` | Static factory method to create validated latitude |
| `GetHemisphere()` | `string` | Returns "North" or "South" |
| `GetCardinalDirection()` | `string` | Returns "N" or "S" |
| `ToCardinalString()` | `string` | Returns formatted string with cardinal direction |
| `ToString()` | `string` | Returns formatted string with degree symbol |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Range** | Must be between -90 and +90 degrees |
| **Decimal Places** | Must match specified precision (0-8) |

---

## ?? Geographic Coordinate Standards

### Latitude Range
- **Valid Range**: -90° (South Pole) to +90° (North Pole)
- **Equator**: 0°
- **Northern Hemisphere**: 0° to +90°
- **Southern Hemisphere**: -90° to 0°

### Decimal Precision Levels

| Decimal Places | Precision | Use Case |
|----------------|-----------|----------|
| 0 | 1 degree (~111 km) | Very rough location |
| 1 | 0.1 degree (~11 km) | Regional area |
| 2 | 0.01 degree (~1 km) | City-level |
| 3 | 0.001 degree (~100 m) | Town/neighborhood |
| 4 | 0.0001 degree (~10 m) | Street-level |
| 5 | 0.00001 degree (~1 m) | Building-level |
| 6 | 0.000001 degree (~10 cm) | GPS precision |
| 7 | 0.0000001 degree (~1 cm) | Survey precision |
| 8 | 0.00000001 degree (~1 mm) | Maximum precision |

### Common Latitude Examples

```csharp
// Major cities
var newYork = 40.7128m;      // New York City
var london = 51.5074m;       // London
var tokyo = 35.6762m;        // Tokyo
var sydney = -33.8688m;      // Sydney (Southern Hemisphere)
var saoPaulo = -23.5505m;    // São Paulo (Southern Hemisphere)

// Geographic features
var northPole = 90.0m;       // North Pole
var southPole = -90.0m;      // South Pole
var equator = 0.0m;          // Equator
```

---

## ??? Security Considerations

### Coordinate Validation

```csharp
// ? DO: Validate before use
var (result, latitude) = Latitude.TryCreate(userInput, 6);
if (!result.IsValid)
{
    return BadRequest("Invalid latitude coordinate");
}

// ? DON'T: Trust user input without validation
var latitude = userInput;  // Dangerous!
ProcessGeographicData(latitude);
```

### Preventing Invalid Coordinates

```csharp
// ? DO: Check for impossible coordinates
var (result, latitude) = Latitude.TryCreate(userInput);
if (result.IsValid)
{
    // Additional business rule validation
    if (latitude.Value == 0 && latitude.DecimalPlaces > 6)
    {
        // Equator with excessive precision might indicate fake data
        _logger.LogWarning("Suspicious coordinate precision at equator");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize coordinate input before validation
public decimal? SanitizeLatitudeInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return null;
    
    // Remove common separators and extra characters
    var sanitized = input.Replace("°", "").Replace("N", "").Replace("S", "").Trim();
    
    // Try to parse as decimal
    if (decimal.TryParse(sanitized, out var value))
    {
        return value;
    }
    
    return null;
}

// Usage
var sanitized = SanitizeLatitudeInput(userInput);
if (sanitized.HasValue)
{
    var (result, latitude) = Latitude.TryCreate(sanitized.Value);
}
```

### Logging Geographic Data

```csharp
// ? DO: Log coordinates appropriately for privacy
public void LogLocationUpdate(Latitude latitude, Longitude longitude, string userId)
{
    // Round to reduce precision for privacy
    var roundedLat = Math.Round(latitude.Value, 2);
    var roundedLon = Math.Round(longitude.Value, 2);
    
    _logger.LogInformation(
        "Location updated by user {UserId}: {RoundedLat}, {RoundedLon}",
        userId,
        roundedLat,
        roundedLon
    );
}

// ? DON'T: Log full precision coordinates
_logger.LogInformation($"User location: {fullPrecisionLat}, {fullPrecisionLon}");  // Avoid
```
