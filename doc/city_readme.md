# Validated.Primitives.City

A validated city name primitive that enforces required field validation and maximum length restrictions, ensuring city names are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`City` is a validated value object that represents a city name. It enforces that the city name is required (not null or empty) and has a maximum length of 100 characters. Once created, a `City` instance is guaranteed to be valid.

### Key Features

- **Required Field** - City name cannot be null, empty, or whitespace
- **Maximum Length** - Enforces 100 character limit
- **Automatic Trimming** - Removes leading and trailing whitespace
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ??? Basic Usage

### Creating a City

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, city) = City.TryCreate("New York");

if (result.IsValid)
{
    Console.WriteLine(city.Value);      // "New York"
    Console.WriteLine(city.ToString()); // "New York"
    
    // Use the validated city
    await ProcessAddress(city);
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
    public City City { get; set; }  // Always valid or null
    public string Street { get; set; }
    public PostalCode PostalCode { get; set; }
}

// Usage
var (result, city) = City.TryCreate(userInput);
if (result.IsValid)
{
    var address = new Address
    {
        Id = Guid.NewGuid(),
        City = city,  // Guaranteed valid
        Street = street,
        PostalCode = postalCode
    };
    
    await _addressRepository.SaveAsync(address);
}
```

---

## ? Valid City Names

### Simple City Names

```csharp
// Basic city names
var (r1, c1) = City.TryCreate("New York");              // ? Valid
var (r2, c2) = City.TryCreate("Los Angeles");           // ? Valid
var (r3, c3) = City.TryCreate("Chicago");               // ? Valid
var (r4, c4) = City.TryCreate("London");                // ? Valid
var (r5, c5) = City.TryCreate("Paris");                 // ? Valid

foreach (var (result, city) in new[] { (r1, c1), (r2, c2), (r3, c3), (r4, c4), (r5, c5) })
{
    Console.WriteLine($"{city.Value}: {result.IsValid}");  // All true
}
```

### Cities with Special Characters

```csharp
// Cities with hyphens, apostrophes, and spaces
var (r1, c1) = City.TryCreate("Saint Paul");            // ? Valid (space)
var (r2, c2) = City.TryCreate("Winnipeg");              // ? Valid
var (r3, c3) = City.TryCreate("O'Fallon");              // ? Valid (apostrophe)
var (r4, c4) = City.TryCreate("Rio de Janeiro");        // ? Valid (spaces)
var (r5, c5) = City.TryCreate("Port-au-Prince");        // ? Valid (hyphen)
var (r6, c6) = City.TryCreate("München");               // ? Valid (international characters)
```

### International City Names

```csharp
// Cities from various countries
var (r1, c1) = City.TryCreate("São Paulo");             // ? Valid (Brazil)
var (r2, c2) = City.TryCreate("México City");           // ? Valid (Mexico)
var (r3, c3) = City.TryCreate("Montréal");              // ? Valid (Canada)
var (r4, c4) = City.TryCreate("München");               // ? Valid (Germany)
var (r5, c5) = City.TryCreate("???");                 // ? Valid (China)
var (r6, c6) = City.TryCreate("Mumbai");                // ? Valid (India)
```

### Whitespace Handling

```csharp
// Automatic trimming
var (r1, c1) = City.TryCreate("  Boston  ");            // ? Trimmed to "Boston"
var (r2, c2) = City.TryCreate("   New York   ");        // ? Trimmed to "New York"
var (r3, c3) = City.TryCreate("Los Angeles ");          // ? Trimmed to "Los Angeles"
var (r4, c4) = City.TryCreate(" Chicago");              // ? Trimmed to "Chicago"
```

---

## ? Invalid City Names

### Empty or Null

```csharp
var (r1, c1) = City.TryCreate("");                      // ? Empty string
var (r2, c2) = City.TryCreate(null);                    // ? Null value
var (r3, c3) = City.TryCreate("   ");                   // ? Whitespace only
var (r4, c4) = City.TryCreate("\t");                    // ? Tab character
var (r5, c5) = City.TryCreate("\n");                    // ? Newline character
```

### Too Long

```csharp
// City name exceeds 100 character limit
var longCityName = new string('A', 101);
var (result, city) = City.TryCreate(longCityName);      // ? Exceeds 100 characters

// result.IsValid == false
// result.Errors contains MaxLength error
```

---

## ?? Real-World Examples

### Address Validation Service

```csharp
public class AddressValidationService
{
    public async Task<ValidationResult> ValidateAddress(AddressRequest request)
    {
        // Validate city
        var (cityResult, city) = City.TryCreate(request.City);
        if (!cityResult.IsValid)
        {
            return ValidationResult.Failed(
                $"Invalid city: {cityResult.ToBulletList()}"
            );
        }

        // Validate postal code
        var (postalResult, postalCode) = PostalCode.TryCreate(request.Country, request.PostalCode);
        if (!postalResult.IsValid)
        {
            return ValidationResult.Failed(
                $"Invalid postal code: {postalResult.ToBulletList()}"
            );
        }

        // Check if city and postal code are compatible
        var compatibility = await _geoService.ValidateCityPostalAsync(
            city.Value,
            postalCode.Value,
            postalCode.CountryCode
        );

        if (!compatibility.IsValid)
        {
            return ValidationResult.Failed(
                $"City '{city.Value}' is not compatible with postal code '{postalCode.Value}'"
            );
        }

        return ValidationResult.Success();
    }
}
```

### User Profile Management

```csharp
public class UserProfileService
{
    public async Task<UpdateResult> UpdateUserAddress(
        string userId,
        string cityInput,
        string postalCodeInput,
        CountryCode country)
    {
        // Validate city
        var (cityResult, city) = City.TryCreate(cityInput);
        if (!cityResult.IsValid)
        {
            return UpdateResult.ValidationFailed(
                "City",
                cityResult.Errors
            );
        }

        // Validate postal code
        var (postalResult, postalCode) = PostalCode.TryCreate(country, postalCodeInput);
        if (!postalResult.IsValid)
        {
            return UpdateResult.ValidationFailed(
                "PostalCode",
                postalResult.Errors
            );
        }

        // Update user profile
        var user = await _userRepository.GetByIdAsync(userId);
        user.City = city;
        user.PostalCode = postalCode;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return UpdateResult.Success();
    }
}
```

### Shipping Calculator

```csharp
public class ShippingCalculator
{
    public async Task<ShippingQuote> CalculateShipping(
        City originCity,
        City destinationCity,
        PostalCode originPostal,
        PostalCode destinationPostal)
    {
        // Validate cities are in supported countries
        if (!IsSupportedCountry(originPostal.CountryCode) ||
            !IsSupportedCountry(destinationPostal.CountryCode))
        {
            throw new ArgumentException("Unsupported country for shipping");
        }

        // Calculate distance between cities
        var distance = await _geoService.CalculateDistanceAsync(
            originCity.Value,
            destinationCity.Value,
            originPostal.Value,
            destinationPostal.Value
        );

        // Get shipping zones based on cities
        var originZone = await GetShippingZoneAsync(originCity, originPostal);
        var destinationZone = await GetShippingZoneAsync(destinationCity, destinationPostal);

        // Calculate cost based on zones and distance
        var baseRate = CalculateBaseRate(originZone, destinationZone, distance);

        return new ShippingQuote
        {
            OriginCity = originCity.Value,
            DestinationCity = destinationCity.Value,
            Distance = distance,
            BaseRate = baseRate,
            OriginZone = originZone,
            DestinationZone = destinationZone
        };
    }

    private async Task<string> GetShippingZoneAsync(City city, PostalCode postalCode)
    {
        // Determine shipping zone based on city and postal code
        return await _zoneService.GetZoneAsync(city.Value, postalCode.Value, postalCode.CountryCode);
    }
}
```

### Weather API Integration

```csharp
public class WeatherService
{
    public async Task<WeatherData> GetWeatherForecast(City city, CountryCode country)
    {
        // Get weather data for the city
        var weatherData = await _weatherApi.GetForecastAsync(city.Value, country);

        if (weatherData == null)
        {
            throw new ArgumentException($"Weather data not available for city: {city.Value}");
        }

        return new WeatherData
        {
            City = city.Value,
            Country = country.ToString(),
            Temperature = weatherData.Temperature,
            Conditions = weatherData.Conditions,
            Forecast = weatherData.Forecast
        };
    }
}
```

### Address Autocomplete

```csharp
[ApiController]
[Route("api/addresses")]
public class AddressController : ControllerBase
{
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities(
        [FromQuery] string query,
        [FromQuery] CountryCode country,
        [FromQuery] int limit = 10)
    {
        // Validate query is not empty
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required");
        }

        // Search for cities matching the query
        var cities = await _cityService.SearchCitiesAsync(query, country, limit);

        // Validate each city name before returning
        var validCities = new List<string>();
        foreach (var cityName in cities)
        {
            var (result, city) = City.TryCreate(cityName);
            if (result.IsValid)
            {
                validCities.Add(city.Value);
            }
        }

        return Ok(new
        {
            Query = query,
            Country = country.ToString(),
            Cities = validCities,
            Count = validCities.Count
        });
    }
}
```

### Business Directory

```csharp
public class BusinessDirectoryService
{
    public async Task<ListingResult> CreateBusinessListing(BusinessListingRequest request)
    {
        // Validate city
        var (cityResult, city) = City.TryCreate(request.City);
        if (!cityResult.IsValid)
        {
            return ListingResult.ValidationFailed(
                "City",
                cityResult.Errors
            );
        }

        // Validate other address components
        var (postalResult, postalCode) = PostalCode.TryCreate(request.Country, request.PostalCode);
        if (!postalResult.IsValid)
        {
            return ListingResult.ValidationFailed(
                "PostalCode",
                postalResult.Errors
            );
        }

        // Create business listing
        var listing = new BusinessListing
        {
            Id = Guid.NewGuid(),
            BusinessName = request.BusinessName,
            Street = request.Street,
            City = city,  // Type-safe
            PostalCode = postalCode,  // Type-safe
            Country = request.Country,
            CreatedAt = DateTime.UtcNow
        };

        await _listingRepository.SaveAsync(listing);

        return ListingResult.Success(listing.Id);
    }

    public async Task<List<BusinessListing>> SearchByCity(City city, CountryCode country)
    {
        // Search businesses in the specified city
        var listings = await _listingRepository.FindByCityAsync(
            city.Value,
            country
        );

        return listings.Select(l => new BusinessListing
        {
            Id = l.Id,
            BusinessName = l.BusinessName,
            City = city,  // Reuse validated city
            PostalCode = l.PostalCode
        }).ToList();
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, city) = City.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated city
    await ProcessAddress(city);
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
    public City City { get; set; }
    public string Description { get; set; }
}

// Creating a location
var (cityResult, city) = City.TryCreate(input);
if (cityResult.IsValid)
{
    var location = new Location
    {
        Id = Guid.NewGuid(),
        City = city,  // Type-safe and validated
        Description = description
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, city) = City.TryCreate("New York");

// Serialize
string json = JsonSerializer.Serialize(city);
// {"Value":"New York"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<City>(json);
Console.WriteLine(deserialized.Value);  // "New York"
```

---

## ?? Related Documentation

- [Address README](address_readme.md) - Complete address validation including cities
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated city name string |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, string propertyName = "City")` | `(ValidationResult, City?)` | Static factory method to create validated city |
| `ToString()` | `string` | Returns the city name value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | City name cannot be null, empty, or whitespace only |
| **Maximum Length** | Cannot exceed 100 characters |

---

## ?? International City Names

The `City` type supports city names from all countries and regions:

### North America
- **United States** - New York, Los Angeles, Chicago, Houston, Phoenix
- **Canada** - Toronto, Montreal, Vancouver, Calgary, Edmonton
- **Mexico** - Mexico City, Guadalajara, Monterrey, Puebla, Tijuana

### Europe
- **United Kingdom** - London, Birmingham, Manchester, Liverpool, Bristol
- **Germany** - Berlin, Munich, Hamburg, Cologne, Frankfurt
- **France** - Paris, Marseille, Lyon, Toulouse, Nice
- **Italy** - Rome, Milan, Naples, Turin, Palermo
- **Spain** - Madrid, Barcelona, Valencia, Seville, Zaragoza

### Asia-Pacific
- **Japan** - Tokyo, Yokohama, Osaka, Nagoya, Sapporo
- **China** - Beijing, Shanghai, Guangzhou, Shenzhen, Chengdu
- **India** - Mumbai, Delhi, Bangalore, Hyderabad, Chennai
- **Australia** - Sydney, Melbourne, Brisbane, Perth, Adelaide
- **South Korea** - Seoul, Busan, Incheon, Daegu, Daejeon

### Other Regions
- **Brazil** - São Paulo, Rio de Janeiro, Salvador, Brasília, Fortaleza
- **Russia** - Moscow, Saint Petersburg, Novosibirsk, Yekaterinburg, Kazan
- **South Africa** - Johannesburg, Cape Town, Durban, Pretoria, Port Elizabeth

### Special Characters in City Names

```csharp
// Cities with various special characters
var internationalCities = new[]
{
    "São Paulo",      // Brazil (tilde)
    "Montréal",       // Canada (accent)
    "München",        // Germany (umlaut)
    "México City",    // Mexico (accent)
    "Nizhny Novgorod", // Russia (Cyrillic support)
    "Xi'an",          // China (apostrophe-like)
    "O'Fallon",       // USA (apostrophe)
    "Port-au-Prince", // Haiti (hyphen)
    "Saint-Étienne",  // France (hyphen)
    "Rio de Janeiro"  // Brazil (spaces and accent)
};

foreach (var cityName in internationalCities)
{
    var (result, city) = City.TryCreate(cityName);
    Console.WriteLine($"{cityName}: {result.IsValid}");  // All valid
}
```

---

## ??? Security Considerations

### City Name Validation

```csharp
// ? DO: Validate before use
var (result, city) = City.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid city name");
}

// ? DON'T: Trust user input without validation
var city = userInput;  // Dangerous!
await ProcessAddress(city);
```

### Preventing Injection Attacks

```csharp
// ? DO: Use validated city in database queries
var (_, city) = City.TryCreate(userInput);
var addresses = await _dbContext.Addresses
    .Where(a => a.City.Value == city.Value)
    .ToListAsync();

// ? DON'T: Concatenate city names in queries
var query = $"SELECT * FROM Addresses WHERE City = '{userInput}'";  // Dangerous!
```

### Input Sanitization

```csharp
// ? DO: Sanitize city input before validation
public string SanitizeCityInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Trim whitespace
    input = input.Trim();
    
    // Allow letters, spaces, hyphens, apostrophes, and international characters
    var sanitized = new string(input.Where(c =>
        char.IsLetter(c) ||
        char.IsWhiteSpace(c) ||
        c == '-' ||
        c == ''' ||
        c == '’'  // Alternative apostrophe
    ).ToArray());
    
    // Normalize multiple spaces to single space
    sanitized = Regex.Replace(sanitized, @"\s+", " ");
    
    return sanitized;
}

// Usage
var sanitized = SanitizeCityInput(userInput);
var (result, city) = City.TryCreate(sanitized);
```

### Rate Limiting for City Searches

```csharp
// ? DO: Implement rate limiting for city searches
public async Task<SearchResult> SearchCities(string query, CountryCode country)
{
    var rateLimitKey = $"city_search:{country}";
    var count = await _cache.GetAsync<int>(rateLimitKey);
    
    if (count >= 100)  // Max 100 searches per hour per country
    {
        return SearchResult.RateLimited();
    }
    
    var cities = await _cityService.SearchAsync(query, country);
    await _cache.SetAsync(rateLimitKey, count + 1, TimeSpan.FromHours(1));
    
    return SearchResult.Success(cities);
}
```

### Logging Sensitive Information

```csharp
// ? DO: Log city searches appropriately
public void LogCitySearch(City city, CountryCode country, string userId)
{
    // Log successful searches without exposing sensitive patterns
    _logger.LogInformation(
        "City search by user {UserId}: '{City}' in {Country}",
        userId,
        city.Value,
        country
    );
}

// ? DON'T: Log raw user input that might contain sensitive data
_logger.LogInformation($"User searched for: {rawUserInput}");  // Avoid
```
