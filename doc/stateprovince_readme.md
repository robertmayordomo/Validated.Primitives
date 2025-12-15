# Validated.Primitives.StateProvince

A validated state or province name primitive that enforces required field validation and maximum length restrictions when provided, ensuring state/province names are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`StateProvince` is a validated value object that represents a state or province name. It enforces that when provided, the state/province name is not null or empty and has a maximum length of 100 characters. Once created, a `StateProvince` instance is guaranteed to be valid.

### Key Features

- **Required When Provided** - State/province name cannot be null, empty, or whitespace when specified
- **Maximum Length** - Enforces 100 character limit
- **Automatic Trimming** - Removes leading and trailing whitespace
- **Optional Field** - Can be null for addresses that don't require state/province
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ??? Basic Usage

### Creating a State/Province

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, stateProvince) = StateProvince.TryCreate("California");

if (result.IsValid)
{
    Console.WriteLine(stateProvince.Value);      // "California"
    Console.WriteLine(stateProvince.ToString()); // "California"
    
    // Use the validated state/province
    await ProcessAddress(stateProvince);
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
    public City City { get; set; }
    public StateProvince? StateProvince { get; set; }  // Optional field
    public PostalCode PostalCode { get; set; }
}

// Usage - with state/province
var (stateResult, stateProvince) = StateProvince.TryCreate(userInput);
if (stateResult.IsValid)
{
    var address = new Address
    {
        Id = Guid.NewGuid(),
        City = city,
        StateProvince = stateProvince,  // Guaranteed valid when provided
        PostalCode = postalCode
    };
    
    await _addressRepository.SaveAsync(address);
}

// Usage - without state/province (null is valid)
var addressWithoutState = new Address
{
    Id = Guid.NewGuid(),
    City = city,
    StateProvince = null,  // Valid for addresses that don't require state/province
    PostalCode = postalCode
};
```

---

## ? Valid State/Province Names

### US States (Full Names)

```csharp
// Full state names
var (r1, s1) = StateProvince.TryCreate("California");     // ? Valid
var (r2, s2) = StateProvince.TryCreate("New York");       // ? Valid
var (r3, s3) = StateProvince.TryCreate("Texas");          // ? Valid
var (r4, s4) = StateProvince.TryCreate("Florida");        // ? Valid
var (r5, s5) = StateProvince.TryCreate("Illinois");       // ? Valid

foreach (var (result, state) in new[] { (r1, s1), (r2, s2), (r3, s3), (r4, s4), (r5, s5) })
{
    Console.WriteLine($"{state.Value}: {result.IsValid}");  // All true
}
```

### US States (Abbreviations)

```csharp
// State abbreviations
var (r1, s1) = StateProvince.TryCreate("CA");             // ? Valid (California)
var (r2, s2) = StateProvince.TryCreate("NY");             // ? Valid (New York)
var (r3, s3) = StateProvince.TryCreate("TX");             // ? Valid (Texas)
var (r4, s4) = StateProvince.TryCreate("FL");             // ? Valid (Florida)
var (r5, s5) = StateProvince.TryCreate("IL");             // ? Valid (Illinois)
```

### Canadian Provinces

```csharp
// Canadian provinces (full names)
var (r1, s1) = StateProvince.TryCreate("Ontario");        // ? Valid
var (r2, s2) = StateProvince.TryCreate("British Columbia"); // ? Valid
var (r3, s3) = StateProvince.TryCreate("Quebec");         // ? Valid
var (r4, s4) = StateProvince.TryCreate("Alberta");        // ? Valid

// Canadian provinces (abbreviations)
var (r5, s5) = StateProvince.TryCreate("ON");             // ? Valid (Ontario)
var (r6, s6) = StateProvince.TryCreate("BC");             // ? Valid (British Columbia)
var (r7, s7) = StateProvince.TryCreate("QC");             // ? Valid (Quebec)
var (r8, s8) = StateProvince.TryCreate("AB");             // ? Valid (Alberta)
```

### International Regions

```csharp
// Other countries' regions
var (r1, s1) = StateProvince.TryCreate("Bavaria");        // ? Valid (Germany)
var (r2, s2) = StateProvince.TryCreate("Catalonia");      // ? Valid (Spain)
var (r3, s3) = StateProvince.TryCreate("Scotland");       // ? Valid (UK)
var (r4, s4) = StateProvince.TryCreate("Queensland");     // ? Valid (Australia)
var (r5, s5) = StateProvince.TryCreate("São Paulo");      // ? Valid (Brazil - state)
var (r6, s6) = StateProvince.TryCreate("Maharashtra");    // ? Valid (India - state)
```

### Whitespace Handling

```csharp
// Automatic trimming
var (r1, s1) = StateProvince.TryCreate("  California  "); // ? Trimmed to "California"
var (r2, s2) = StateProvince.TryCreate("   NY   ");       // ? Trimmed to "NY"
var (r3, s3) = StateProvince.TryCreate("Ontario ");       // ? Trimmed to "Ontario"
var (r4, s4) = StateProvince.TryCreate(" BC");            // ? Trimmed to "BC"
```

---

## ? Invalid State/Province Names

### Empty or Null

```csharp
var (r1, s1) = StateProvince.TryCreate("");               // ? Empty string
var (r2, s2) = StateProvince.TryCreate(null);             // ? Null value
var (r3, s3) = StateProvince.TryCreate("   ");            // ? Whitespace only
var (r4, s4) = StateProvince.TryCreate("\t");             // ? Tab character
var (r5, s5) = StateProvince.TryCreate("\n");             // ? Newline character
```

### Too Long

```csharp
// State/province name exceeds 100 character limit
var longStateName = new string('A', 101);
var (result, stateProvince) = StateProvince.TryCreate(longStateName); // ? Exceeds 100 characters

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
        // Validate required fields
        var (cityResult, city) = City.TryCreate(request.City);
        if (!cityResult.IsValid)
        {
            return ValidationResult.Failed($"Invalid city: {cityResult.ToBulletList()}");
        }

        var (postalResult, postalCode) = PostalCode.TryCreate(request.Country, request.PostalCode);
        if (!postalResult.IsValid)
        {
            return ValidationResult.Failed($"Invalid postal code: {postalResult.ToBulletList()}");
        }

        // Validate optional state/province if provided
        StateProvince? stateProvince = null;
        if (!string.IsNullOrWhiteSpace(request.StateProvince))
        {
            var (stateResult, state) = StateProvince.TryCreate(request.StateProvince);
            if (!stateResult.IsValid)
            {
                return ValidationResult.Failed($"Invalid state/province: {stateResult.ToBulletList()}");
            }
            stateProvince = state;
        }

        // Validate state/province compatibility with country
        if (stateProvince != null)
        {
            var compatibility = await ValidateStateCountryCompatibilityAsync(
                stateProvince.Value,
                request.Country
            );

            if (!compatibility.IsValid)
            {
                return ValidationResult.Failed(
                    $"State/province '{stateProvince.Value}' is not valid for {request.Country}"
                );
            }
        }

        return ValidationResult.Success();
    }

    private async Task<CompatibilityResult> ValidateStateCountryCompatibilityAsync(
        string stateProvince,
        CountryCode country)
    {
        // Check if the state/province is valid for the country
        return await _geoService.ValidateStateProvinceAsync(stateProvince, country);
    }
}
```

### Tax Calculation Service

```csharp
public class TaxCalculationService
{
    public async Task<TaxCalculation> CalculateTax(
        decimal subtotal,
        StateProvince? stateProvince,
        CountryCode country)
    {
        // Get base tax rates for the country
        var countryRates = await _taxService.GetCountryTaxRatesAsync(country);

        // Apply state/province-specific tax rates if available
        decimal stateTaxRate = 0;
        if (stateProvince != null)
        {
            var stateRates = await _taxService.GetStateTaxRatesAsync(
                stateProvince.Value,
                country
            );
            stateTaxRate = stateRates?.TaxRate ?? 0;
        }

        // Calculate taxes
        var countryTax = subtotal * countryRates.TaxRate;
        var stateTax = subtotal * stateTaxRate;
        var totalTax = countryTax + stateTax;

        return new TaxCalculation
        {
            Subtotal = subtotal,
            CountryTax = countryTax,
            StateTax = stateTax,
            TotalTax = totalTax,
            Total = subtotal + totalTax,
            Country = country.ToString(),
            StateProvince = stateProvince?.Value
        };
    }
}
```

### Shipping Calculator

```csharp
public class ShippingCalculator
{
    public async Task<ShippingQuote> CalculateShipping(
        Address origin,
        Address destination)
    {
        // Calculate base shipping cost
        var baseCost = await CalculateBaseCostAsync(origin, destination);

        // Apply state/province-specific surcharges
        var originSurcharge = await CalculateStateSurchargeAsync(origin.StateProvince, origin.Country);
        var destinationSurcharge = await CalculateStateSurchargeAsync(destination.StateProvince, destination.Country);

        var totalCost = baseCost + originSurcharge + destinationSurcharge;

        return new ShippingQuote
        {
            Origin = origin.ToString(),
            Destination = destination.ToString(),
            BaseCost = baseCost,
            OriginSurcharge = originSurcharge,
            DestinationSurcharge = destinationSurcharge,
            TotalCost = totalCost
        };
    }

    private async Task<decimal> CalculateStateSurchargeAsync(
        StateProvince? stateProvince,
        CountryCode country)
    {
        if (stateProvince == null)
        {
            return 0;
        }

        // Get state-specific shipping surcharges
        var surcharge = await _shippingService.GetStateSurchargeAsync(
            stateProvince.Value,
            country
        );

        return surcharge ?? 0;
    }
}
```

### User Profile Management

```csharp
public class UserProfileService
{
    public async Task<UpdateResult> UpdateUserAddress(
        string userId,
        AddressUpdateRequest request)
    {
        // Validate required fields
        var (cityResult, city) = City.TryCreate(request.City);
        if (!cityResult.IsValid)
        {
            return UpdateResult.ValidationFailed("City", cityResult.Errors);
        }

        var (postalResult, postalCode) = PostalCode.TryCreate(request.Country, request.PostalCode);
        if (!postalResult.IsValid)
        {
            return UpdateResult.ValidationFailed("PostalCode", postalResult.Errors);
        }

        // Validate optional state/province
        StateProvince? stateProvince = null;
        if (!string.IsNullOrWhiteSpace(request.StateProvince))
        {
            var (stateResult, state) = StateProvince.TryCreate(request.StateProvince);
            if (!stateResult.IsValid)
            {
                return UpdateResult.ValidationFailed("StateProvince", stateResult.Errors);
            }
            stateProvince = state;
        }

        // Update user profile
        var user = await _userRepository.GetByIdAsync(userId);
        user.City = city;
        user.StateProvince = stateProvince;  // Can be null
        user.PostalCode = postalCode;
        user.Country = request.Country;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return UpdateResult.Success();
    }
}
```

### Business Registration

```csharp
public class BusinessRegistrationService
{
    public async Task<RegistrationResult> RegisterBusiness(BusinessRegistrationRequest request)
    {
        // Validate business address
        var (cityResult, city) = City.TryCreate(request.City);
        if (!cityResult.IsValid)
        {
            return RegistrationResult.ValidationFailed("City", cityResult.Errors);
        }

        var (postalResult, postalCode) = PostalCode.TryCreate(request.Country, request.PostalCode);
        if (!postalResult.IsValid)
        {
            return RegistrationResult.ValidationFailed("PostalCode", postalResult.Errors);
        }

        // Validate state/province (required for some countries)
        StateProvince? stateProvince = null;
        if (RequiresStateProvince(request.Country))
        {
            if (string.IsNullOrWhiteSpace(request.StateProvince))
            {
                return RegistrationResult.Failed("State/Province is required for this country");
            }

            var (stateResult, state) = StateProvince.TryCreate(request.StateProvince);
            if (!stateResult.IsValid)
            {
                return RegistrationResult.ValidationFailed("StateProvince", stateResult.Errors);
            }
            stateProvince = state;
        }
        else if (!string.IsNullOrWhiteSpace(request.StateProvince))
        {
            // Optional but validate if provided
            var (stateResult, state) = StateProvince.TryCreate(request.StateProvince);
            if (!stateResult.IsValid)
            {
                return RegistrationResult.ValidationFailed("StateProvince", stateResult.Errors);
            }
            stateProvince = state;
        }

        // Register business
        var business = new Business
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            City = city,
            StateProvince = stateProvince,
            PostalCode = postalCode,
            Country = request.Country,
            RegisteredAt = DateTime.UtcNow
        };

        await _businessRepository.SaveAsync(business);

        return RegistrationResult.Success(business.Id);
    }

    private bool RequiresStateProvince(CountryCode country)
    {
        // Some countries require state/province for business registration
        return country == CountryCode.UnitedStates ||
               country == CountryCode.Canada ||
               country == CountryCode.Australia;
    }
}
```

### Address Autocomplete API

```csharp
[ApiController]
[Route("api/addresses")]
public class AddressController : ControllerBase
{
    [HttpGet("states")]
    public async Task<IActionResult> GetStatesProvinces(
        [FromQuery] CountryCode country,
        [FromQuery] string? query = null)
    {
        // Get states/provinces for the country
        var regions = await _geoService.GetStatesProvincesAsync(country);

        // Filter by query if provided
        if (!string.IsNullOrWhiteSpace(query))
        {
            regions = regions.Where(r =>
                r.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        // Validate each region name
        var validRegions = new List<string>();
        foreach (var regionName in regions)
        {
            var (result, region) = StateProvince.TryCreate(regionName);
            if (result.IsValid)
            {
                validRegions.Add(region.Value);
            }
        }

        return Ok(new
        {
            Country = country.ToString(),
            Query = query,
            StatesProvinces = validRegions,
            Count = validRegions.Count
        });
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, stateProvince) = StateProvince.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated state/province
    await ProcessAddress(stateProvince);
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
// State/province is optional - handle both cases
StateProvince? stateProvince = null;
if (!string.IsNullOrWhiteSpace(userInput))
{
    var (result, state) = StateProvince.TryCreate(userInput);
    if (result.IsValid)
    {
        stateProvince = state;
    }
    else
    {
        // Handle validation error for optional field
        _logger.LogWarning("Invalid state/province provided: {Input}", userInput);
    }
}

// Use in address (can be null)
var address = new Address
{
    City = city,
    StateProvince = stateProvince,  // Nullable
    PostalCode = postalCode
};
```

### Domain Model Usage

```csharp
public class Location
{
    public Guid Id { get; set; }
    public City City { get; set; }
    public StateProvince? StateProvince { get; set; }
    public CountryCode Country { get; set; }
}

// Creating a location
var (cityResult, city) = City.TryCreate(cityInput);
var (stateResult, stateProvince) = StateProvince.TryCreate(stateInput);  // Optional

if (cityResult.IsValid && (stateResult.IsValid || string.IsNullOrWhiteSpace(stateInput)))
{
    var location = new Location
    {
        Id = Guid.NewGuid(),
        City = city,
        StateProvince = stateResult.IsValid ? stateProvince : null,
        Country = country
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, stateProvince) = StateProvince.TryCreate("California");

// Serialize
string json = JsonSerializer.Serialize(stateProvince);
// {"Value":"California"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<StateProvince>(json);
Console.WriteLine(deserialized.Value);  // "California"
```

---

## ?? Related Documentation

- [Address README](address_readme.md) - Complete address validation including state/province
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated state/province name string |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, string propertyName = "StateProvince")` | `(ValidationResult, StateProvince?)` | Static factory method to create validated state/province |
| `ToString()` | `string` | Returns the state/province name value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | State/province name cannot be null, empty, or whitespace only when provided |
| **Maximum Length** | Cannot exceed 100 characters |

---

## ?? International State/Province Names

The `StateProvince` type supports state and province names from all countries:

### North America

#### United States (50 States + DC)
- **States**: Alabama, Alaska, Arizona, Arkansas, California, Colorado, Connecticut, Delaware, Florida, Georgia, Hawaii, Idaho, Illinois, Indiana, Iowa, Kansas, Kentucky, Louisiana, Maine, Maryland, Massachusetts, Michigan, Minnesota, Mississippi, Missouri, Montana, Nebraska, Nevada, New Hampshire, New Jersey, New Mexico, New York, North Carolina, North Dakota, Ohio, Oklahoma, Oregon, Pennsylvania, Rhode Island, South Carolina, South Dakota, Tennessee, Texas, Utah, Vermont, Virginia, Washington, West Virginia, Wisconsin, Wyoming
- **District**: Washington, D.C.
- **Abbreviations**: AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID, IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS, MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV, WI, WY, DC

#### Canada (10 Provinces + 3 Territories)
- **Provinces**: Alberta, British Columbia, Manitoba, New Brunswick, Newfoundland and Labrador, Nova Scotia, Ontario, Prince Edward Island, Quebec, Saskatchewan
- **Territories**: Northwest Territories, Nunavut, Yukon
- **Abbreviations**: AB, BC, MB, NB, NL, NS, ON, PE, QC, SK, NT, NU, YT

#### Mexico (32 States)
- **States**: Aguascalientes, Baja California, Baja California Sur, Campeche, Chiapas, Chihuahua, Coahuila, Colima, Durango, Guanajuato, Guerrero, Hidalgo, Jalisco, México, Michoacán, Morelos, Nayarit, Nuevo León, Oaxaca, Puebla, Querétaro, Quintana Roo, San Luis Potosí, Sinaloa, Sonora, Tabasco, Tamaulipas, Tlaxcala, Veracruz, Yucatán, Zacatecas

### Europe

#### Germany (16 States)
- **States**: Baden-Württemberg, Bavaria, Berlin, Brandenburg, Bremen, Hamburg, Hesse, Lower Saxony, Mecklenburg-Vorpommern, North Rhine-Westphalia, Rhineland-Palatinate, Saarland, Saxony, Saxony-Anhalt, Schleswig-Holstein, Thuringia

#### United Kingdom (Countries/Regions)
- **Countries**: England, Scotland, Wales
- **Regions**: East Midlands, East of England, Greater London, North East England, North West England, South East England, South West England, West Midlands, Yorkshire and the Humber

#### France (18 Regions)
- **Regions**: Auvergne-Rhône-Alpes, Bourgogne-Franche-Comté, Brittany, Centre-Val de Loire, Corsica, Grand Est, Hauts-de-France, Île-de-France, Normandy, Nouvelle-Aquitaine, Occitanie, Pays de la Loire, Provence-Alpes-Côte d'Azur

#### Spain (17 Autonomous Communities)
- **Communities**: Andalusia, Aragon, Asturias, Balearic Islands, Basque Country, Canary Islands, Cantabria, Castile and León, Castile-La Mancha, Catalonia, Extremadura, Galicia, La Rioja, Madrid, Murcia, Navarre, Valencia

### Asia-Pacific

#### Australia (6 States + 2 Territories)
- **States**: New South Wales, Queensland, South Australia, Tasmania, Victoria, Western Australia
- **Territories**: Australian Capital Territory, Northern Territory
- **Abbreviations**: NSW, QLD, SA, TAS, VIC, WA, ACT, NT

#### India (28 States + 8 Union Territories)
- **States**: Andhra Pradesh, Arunachal Pradesh, Assam, Bihar, Chhattisgarh, Goa, Gujarat, Haryana, Himachal Pradesh, Jharkhand, Karnataka, Kerala, Madhya Pradesh, Maharashtra, Manipur, Meghalaya, Mizoram, Nagaland, Odisha, Punjab, Rajasthan, Sikkim, Tamil Nadu, Telangana, Tripura, Uttar Pradesh, Uttarakhand, West Bengal

### Other Regions

#### Brazil (26 States + 1 Federal District)
- **States**: Acre, Alagoas, Amapá, Amazonas, Bahia, Ceará, Espírito Santo, Goiás, Maranhão, Mato Grosso, Mato Grosso do Sul, Minas Gerais, Pará, Paraíba, Paraná, Pernambuco, Piauí, Rio de Janeiro, Rio Grande do Norte, Rio Grande do Sul, Rondônia, Roraima, Santa Catarina, São Paulo, Sergipe, Tocantins
- **Federal District**: Distrito Federal

#### South Africa (9 Provinces)
- **Provinces**: Eastern Cape, Free State, Gauteng, KwaZulu-Natal, Limpopo, Mpumalanga, North West, Northern Cape, Western Cape

---

## ??? Security Considerations

### State/Province Validation

```csharp
// ? DO: Validate before use
var (result, stateProvince) = StateProvince.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid state/province name");
}

// ? DON'T: Trust user input without validation
var stateProvince = userInput;  // Dangerous!
await ProcessAddress(stateProvince);
```

### Preventing Injection Attacks

```csharp
// ? DO: Use validated state/province in database queries
var (_, stateProvince) = StateProvince.TryCreate(userInput);
var addresses = await _dbContext.Addresses
    .Where(a => a.StateProvince.Value == stateProvince.Value)
    .ToListAsync();

// ? DON'T: Concatenate state/province names in queries
var query = $"SELECT * FROM Addresses WHERE StateProvince = '{userInput}'";  // Dangerous!
```

### Input Sanitization

```csharp
// ? DO: Sanitize state/province input before validation
public string SanitizeStateProvinceInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Trim whitespace
    input = input.Trim();
    
    // Allow letters, spaces, hyphens, apostrophes, and periods (for abbreviations)
    var sanitized = new string(input.Where(c =>
        char.IsLetter(c) ||
        char.IsWhiteSpace(c) ||
        c == '-' ||
        c == ''' ||
        c == '’' ||  // Alternative apostrophe
        c == '.'     // For abbreviations like "N.Y."
    ).ToArray());
    
    // Normalize multiple spaces to single space
    sanitized = Regex.Replace(sanitized, @"\s+", " ");
    
    return sanitized;
}

// Usage
var sanitized = SanitizeStateProvinceInput(userInput);
var (result, stateProvince) = StateProvince.TryCreate(sanitized);
```

### Country-Specific Validation

```csharp
// ? DO: Validate state/province compatibility with country
public async Task<bool> IsValidStateForCountry(string stateProvince, CountryCode country)
{
    var (result, validatedState) = StateProvince.TryCreate(stateProvince);
    if (!result.IsValid)
    {
        return false;
    }

    // Check if the state/province exists in the country
    return await _geoService.StateExistsInCountryAsync(
        validatedState.Value,
        country
    );
}

// Usage
if (!await IsValidStateForCountry(userInput, CountryCode.UnitedStates))
{
    return BadRequest("Invalid state for the selected country");
}
```

### Logging Sensitive Information

```csharp
// ? DO: Log state/province information appropriately
public void LogAddressUpdate(Address address, string userId)
{
    // Log state/province without exposing sensitive patterns
    var stateInfo = address.StateProvince != null
        ? address.StateProvince.Value
        : "Not specified";

    _logger.LogInformation(
        "Address updated by user {UserId}: City='{City}', State='{State}', Country={Country}",
        userId,
        address.City.Value,
        stateInfo,
        address.Country
    );
}

// ? DON'T: Log raw user input that might contain sensitive data
_logger.LogInformation($"User updated address with state: {rawUserInput}");  // Avoid
```
