# Validated.Primitives.Longitude

A validated longitude coordinate primitive that enforces geographic coordinate validation (-180 to +180 degrees) with configurable decimal precision, ensuring longitude values are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`Longitude` is a validated value object that represents a longitude coordinate in degrees. It enforces the valid range of -180 to +180 degrees and supports configurable decimal precision (0-8 decimal places). Once created, a `Longitude` instance is guaranteed to be valid.

### Key Features

- **Geographic Range** - Enforces -180 to +180 degree range
- **Decimal Precision** - Configurable decimal places (0-8)
- **Hemisphere Detection** - East/West hemisphere identification
- **Cardinal Directions** - E/W direction formatting
- **GPS Precision** - Supports up to 8 decimal places for precise coordinates
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other numeric types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Longitude

```csharp
using Validated.Primitives.ValueObjects.Geospatial;

// Create with validation (default 6 decimal places for GPS)
var (result, longitude) = Longitude.TryCreate(-74.0060m);

if (result.IsValid)
{
    Console.WriteLine(longitude.Value);                   // -74.0060
    Console.WriteLine(longitude.ToString());             // "-74.0060?"
    Console.WriteLine(longitude.GetHemisphere());        // "West"
    Console.WriteLine(longitude.GetCardinalDirection()); // "W"
    Console.WriteLine(longitude.ToCardinalString());     // "74.0060? W"
    
    // Use the validated longitude
    ProcessCoordinate(longitude);
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
var (r1, lon1) = Longitude.TryCreate(-74.0060m, 6);     // GPS precision (default)
var (r2, lon2) = Longitude.TryCreate(-74.0060m, 4);     // City-level precision
var (r3, lon3) = Longitude.TryCreate(-74.0060m, 2);     // Regional precision
var (r4, lon4) = Longitude.TryCreate(-74.0060m, 0);     // Degree precision only

Console.WriteLine(lon1.ToString());  // "-74.006000?"
Console.WriteLine(lon2.ToString());  // "-74.0060?"
Console.WriteLine(lon3.ToString());  // "-74.01?"
Console.WriteLine(lon4.ToString());  // "-74?" (rounded)
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

## ? Valid Longitude Values

### Eastern Hemisphere

```csharp
// Valid eastern longitudes (0 to 180 degrees)
var (r1, l1) = Longitude.TryCreate(180.0m);             // ? International Date Line (East)
var (r2, l2) = Longitude.TryCreate(139.6917m);          // ? Tokyo
var (r3, l3) = Longitude.TryCreate(2.3522m);            // ? Paris
var (r4, l4) = Longitude.TryCreate(77.2090m);           // ? New Delhi
var (r5, l5) = Longitude.TryCreate(0.0m);               // ? Prime Meridian

foreach (var (result, lon) in new[] { (r1, l1), (r2, l2), (r3, l3), (r4, l4), (r5, l5) })
{
    Console.WriteLine($"{lon.Value}: {result.IsValid} - {lon.GetHemisphere()}");
}
```

### Western Hemisphere

```csharp
// Valid western longitudes (-180 to 0 degrees)
var (r1, l1) = Longitude.TryCreate(-180.0m);            // ? International Date Line (West)
var (r2, l2) = Longitude.TryCreate(-74.0060m);          // ? New York City
var (r3, l3) = Longitude.TryCreate(-118.2437m);         // ? Los Angeles
var (r4, l4) = Longitude.TryCreate(-87.6298m);          // ? Chicago
var (r5, l5) = Longitude.TryCreate(-0.0001m);           // ? Just west of Prime Meridian
```

### Different Precision Levels

```csharp
// Same coordinate with different precision
var coordinate = -74.0060m;

var (r1, l1) = Longitude.TryCreate(coordinate, 8);      // Maximum precision
var (r2, l2) = Longitude.TryCreate(coordinate, 6);      // GPS precision
var (r3, l3) = Longitude.TryCreate(coordinate, 4);      // City precision
var (r4, l4) = Longitude.TryCreate(coordinate, 2);      // Regional precision
var (r5, l5) = Longitude.TryCreate(coordinate, 0);      // Degree precision

Console.WriteLine($"8 decimals: {l1.ToString()}");
Console.WriteLine($"6 decimals: {l2.ToString()}");
Console.WriteLine($"4 decimals: {l3.ToString()}");
Console.WriteLine($"2 decimals: {l4.ToString()}");
Console.WriteLine($"0 decimals: {l5.ToString()}");
```

---

## ? Invalid Longitude Values

### Out of Range

```csharp
var (r1, l1) = Longitude.TryCreate(180.1m);             // ? Above maximum (180)
var (r2, l2) = Longitude.TryCreate(-180.1m);            // ? Below minimum (-180)
var (r3, l3) = Longitude.TryCreate(200.0m);             // ? Way above maximum
var (r4, l4) = Longitude.TryCreate(-200.0m);            // ? Way below minimum
```

### Invalid Decimal Places

```csharp
var (r1, l1) = Longitude.TryCreate(-74.0060m, -1);      // ? Negative decimal places
var (r2, l2) = Longitude.TryCreate(-74.0060m, 9);       // ? Too many decimal places (max 8)
var (r3, l3) = Longitude.TryCreate(-74.0060m, 10);      // ? Way too many decimal places
```

### Too Many Decimals for Value

```csharp
var (r1, l1) = Longitude.TryCreate(-74.0060m, 8);       // ? Value has only 4 decimals but asks for 8 precision
// This would fail because the value doesn't have enough decimal places
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, longitude) = Longitude.TryCreate(userInput, 6);

if (result.IsValid)
{
    // Use the validated longitude
    ProcessCoordinate(longitude);
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

var (_, longitude) = Longitude.TryCreate(-74.0060m, 6);

// Serialize
string json = JsonSerializer.Serialize(longitude);
// {"Value":-74.0060,"DecimalPlaces":6}

// Deserialize
var deserialized = JsonSerializer.Deserialize<Longitude>(json);
Console.WriteLine(deserialized.Value);         // -74.0060
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
| `Value` | `decimal` | The validated longitude value in degrees |
| `DecimalPlaces` | `int` | The number of decimal places (0-8) |
| `MinValue` | `decimal` | Constant for minimum valid longitude (-180) |
| `MaxValue` | `decimal` | Constant for maximum valid longitude (180) |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(decimal value, int decimalPlaces = 6, string propertyName = "Longitude")` | `(ValidationResult, Longitude?)` | Static factory method to create validated longitude |
| `GetHemisphere()` | `string` | Returns "East" or "West" |
| `GetCardinalDirection()` | `string` | Returns "E" or "W" |
| `ToCardinalString()` | `string` | Returns formatted string with cardinal direction |
| `ToString()` | `string` | Returns formatted string with degree symbol |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Range** | Must be between -180 and +180 degrees |
| **Decimal Places** | Must match specified precision (0-8) |

---

## ?? Geographic Coordinate Standards

### Longitude Range
- **Valid Range**: -180° (International Date Line West) to +180° (International Date Line East)
- **Prime Meridian**: 0° (Greenwich, London)
- **Eastern Hemisphere**: 0° to +180°
- **Western Hemisphere**: -180° to 0°

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

### Common Longitude Examples

```csharp
// Major cities
var newYork = -74.0060m;      // New York City (Western Hemisphere)
var london = -0.1278m;       // London (Western, near Prime Meridian)
var tokyo = 139.6917m;       // Tokyo (Eastern Hemisphere)
var sydney = 151.2093m;      // Sydney (Eastern Hemisphere)
var dubai = 55.2708m;        // Dubai (Eastern Hemisphere)

// Geographic features
var primeMeridian = 0.0m;    // Prime Meridian
var dateLineEast = 180.0m;    // International Date Line (East)
var dateLineWest = -180.0m;   // International Date Line (West)
```

---

## ??? Security Considerations

### Coordinate Validation

```csharp
// ? DO: Validate before use
var (result, longitude) = Longitude.TryCreate(userInput, 6);
if (!result.IsValid)
{
    return BadRequest("Invalid longitude coordinate");
}

// ? DON'T: Trust user input without validation
var longitude = userInput;  // Dangerous!
ProcessGeographicData(longitude);
```

### Preventing Invalid Coordinates

```csharp
// ? DO: Check for impossible coordinates
var (result, longitude) = Longitude.TryCreate(userInput);
if (result.IsValid)
{
    // Additional business rule validation
    if (longitude.Value == 0 && longitude.DecimalPlaces > 6)
    {
        // Prime Meridian with excessive precision might indicate fake data
        _logger.LogWarning("Suspicious coordinate precision at Prime Meridian");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize coordinate input before validation
public decimal? SanitizeLongitudeInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return null;
    
    // Remove common separators and extra characters
    var sanitized = input.Replace("°", "").Replace("E", "").Replace("W", "").Trim();
    
    // Try to parse as decimal
    if (decimal.TryParse(sanitized, out var value))
    {
        return value;
    }
    
    return null;
}

// Usage
var sanitized = SanitizeLongitudeInput(userInput);
if (sanitized.HasValue)
{
    var (result, longitude) = Longitude.TryCreate(sanitized.Value);
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
