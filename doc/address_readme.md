# Validated.Primitives.Address Namespace

The Address namespace provides validated value objects and domain models for working with physical addresses worldwide. All types include built-in validation, country-specific postal code validation, and formatted display options.

## ?? Package Information

The address types are split across two packages:

### Core Primitives (`Validated.Primitives`)
- `AddressLine` - Street address line validation (max 200 characters)
- `City` - City name validation (max 100 characters)
- `StateProvince` - State or province validation (max 100 characters)
- `PostalCode` - Country-specific postal code validation (30+ countries supported)
- `CountryCode` - ISO country code enumeration

### Domain Models (`Validated.Primitives.Domain`)
- `Address` - Complete physical address with all components
- `AddressBuilder` - Fluent builder for constructing addresses

---

## ?? Core Primitives

### AddressLine

Represents a validated address line for street addresses, apartment numbers, suites, etc.

#### Key Features
- Maximum length: 200 characters
- Automatic whitespace trimming
- Optional (can be null for AddressLine2)
- Suitable for street addresses, apartment numbers, building names
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, addressLine) = AddressLine.TryCreate("123 Main Street");

if (result.IsValid)
{
    Console.WriteLine(addressLine.Value);      // "123 Main Street"
    Console.WriteLine(addressLine.ToString()); // "123 Main Street"
}
```

#### Valid Address Lines

```csharp
// Street addresses
var (r1, line1) = AddressLine.TryCreate("123 Main Street");              // ? Valid
var (r2, line2) = AddressLine.TryCreate("456 Oak Avenue");               // ? Valid
var (r3, line3) = AddressLine.TryCreate("789 Elm Boulevard");            // ? Valid

// With unit/apartment numbers
var (r4, line4) = AddressLine.TryCreate("100 Market St, Suite 500");     // ? Valid
var (r5, line5) = AddressLine.TryCreate("Apartment 4B");                 // ? Valid
var (r6, line6) = AddressLine.TryCreate("Building A, Floor 3");          // ? Valid

// International addresses
var (r7, line7) = AddressLine.TryCreate("10 Downing Street");            // ? Valid (UK)
var (r8, line8) = AddressLine.TryCreate("123 Rue de Rivoli");            // ? Valid (France)
var (r9, line9) = AddressLine.TryCreate("1-2-3 Shibuya");                // ? Valid (Japan)

// Whitespace handling
var (r10, line10) = AddressLine.TryCreate("  123 Main St  ");           // ? Trimmed to "123 Main St"

// Null or empty (for optional fields)
var (r11, line11) = AddressLine.TryCreate(null);                         // ? Returns null
var (r12, line12) = AddressLine.TryCreate("");                           // ? Returns null
```

#### Invalid Address Lines

```csharp
// Too long (over 200 characters)
var longAddress = new string('a', 201);
var (r1, line1) = AddressLine.TryCreate(longAddress);  // ? Exceeds max length
```

---

### City

Represents a validated city name. Required field with maximum length validation.

#### Key Features
- Required field (cannot be null or empty)
- Maximum length: 100 characters
- Automatic whitespace trimming
- Supports international city names
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, city) = City.TryCreate("New York");

if (result.IsValid)
{
    Console.WriteLine(city.Value);      // "New York"
    Console.WriteLine(city.ToString()); // "New York"
}
```

#### Valid City Names

```csharp
// US cities
var (r1, c1) = City.TryCreate("New York");                // ? Valid
var (r2, c2) = City.TryCreate("Los Angeles");             // ? Valid
var (r3, c3) = City.TryCreate("San Francisco");           // ? Valid

// International cities
var (r4, c4) = City.TryCreate("London");                  // ? Valid (UK)
var (r5, c5) = City.TryCreate("Paris");                   // ? Valid (France)
var (r6, c6) = City.TryCreate("Tokyo");                   // ? Valid (Japan)
var (r7, c7) = City.TryCreate("São Paulo");               // ? Valid (Brazil)
var (r8, c8) = City.TryCreate("München");                 // ? Valid (Germany - Munich)

// With special characters
var (r9, c9) = City.TryCreate("Saint-Étienne");           // ? Valid (France)
var (r10, c10) = City.TryCreate("Ålesund");               // ? Valid (Norway)

// Whitespace trimming
var (r11, c11) = City.TryCreate("  Boston  ");            // ? Trimmed to "Boston"
```

#### Invalid City Names

```csharp
// Null or empty
var (r1, c1) = City.TryCreate(null);                      // ? Required
var (r2, c2) = City.TryCreate("");                        // ? Required
var (r3, c3) = City.TryCreate("   ");                     // ? Required (whitespace only)

// Too long
var longCity = new string('a', 101);
var (r4, c4) = City.TryCreate(longCity);                  // ? Exceeds 100 characters
```

---

### StateProvince

Represents a validated state or province name. Optional field with maximum length validation.

#### Key Features
- Optional field (can be null)
- Maximum length: 100 characters when provided
- Automatic whitespace trimming
- Supports state/province abbreviations and full names
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, stateProvince) = StateProvince.TryCreate("California");

if (result.IsValid)
{
    Console.WriteLine(stateProvince.Value);      // "California"
    Console.WriteLine(stateProvince.ToString()); // "California"
}
```

#### Valid State/Province Names

```csharp
// US states (full names)
var (r1, s1) = StateProvince.TryCreate("California");     // ? Valid
var (r2, s2) = StateProvince.TryCreate("New York");       // ? Valid
var (r3, s3) = StateProvince.TryCreate("Texas");          // ? Valid

// US states (abbreviations)
var (r4, s4) = StateProvince.TryCreate("CA");             // ? Valid
var (r5, s5) = StateProvince.TryCreate("NY");             // ? Valid
var (r6, s6) = StateProvince.TryCreate("TX");             // ? Valid

// Canadian provinces
var (r7, s7) = StateProvince.TryCreate("Ontario");        // ? Valid
var (r8, s8) = StateProvince.TryCreate("ON");             // ? Valid
var (r9, s9) = StateProvince.TryCreate("British Columbia"); // ? Valid
var (r10, s10) = StateProvince.TryCreate("BC");           // ? Valid

// Other international regions
var (r11, s11) = StateProvince.TryCreate("Bavaria");      // ? Valid (Germany)
var (r12, s12) = StateProvince.TryCreate("Queensland");   // ? Valid (Australia)

// Whitespace trimming
var (r13, s13) = StateProvince.TryCreate("  CA  ");       // ? Trimmed to "CA"
```

#### Invalid State/Province Names

```csharp
// Too long
var longState = new string('a', 101);
var (r1, s1) = StateProvince.TryCreate(longState);        // ? Exceeds 100 characters
```

---

### PostalCode

Represents a validated postal code with country-specific format validation. Supports 30+ countries with automatic country detection and format validation.

#### Key Features
- Country-specific validation for 30+ countries
- Format validation for each country
- Country code extraction
- Length validation (2-10 characters)
- Supports ZIP codes, postal codes, postcodes worldwide
- JSON serialization support

#### Supported Countries

United States, Canada, United Kingdom, Germany, France, Italy, Spain, Netherlands, Belgium, Switzerland, Austria, Sweden, Norway, Denmark, Finland, Poland, Czech Republic, Hungary, Portugal, Ireland, Australia, New Zealand, Japan, South Korea, Brazil, Mexico, India, China, South Africa, and more.

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, postalCode) = PostalCode.TryCreate(CountryCode.UnitedStates, "10001");

if (result.IsValid)
{
    Console.WriteLine(postalCode.Value);            // "10001"
    Console.WriteLine(postalCode.CountryCode);      // UnitedStates
    Console.WriteLine(postalCode.GetCountryName()); // "United States"
    Console.WriteLine(postalCode.ToString());       // "10001"
}
```

#### Valid Postal Codes by Country

```csharp
// United States (ZIP codes)
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedStates, "10001");     // ? 5-digit ZIP
var (r2, p2) = PostalCode.TryCreate(CountryCode.UnitedStates, "10001-5555"); // ? ZIP+4
var (r3, p3) = PostalCode.TryCreate(CountryCode.UnitedStates, "90210");     // ? Valid

// Canada
var (r4, p4) = PostalCode.TryCreate(CountryCode.Canada, "M5H 2N2");         // ? Valid
var (r5, p5) = PostalCode.TryCreate(CountryCode.Canada, "K1A 0B1");         // ? Valid

// United Kingdom
var (r6, p6) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "SW1A 2AA"); // ? Valid
var (r7, p7) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "EC1A 1BB"); // ? Valid
var (r8, p8) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "W1A 0AX");  // ? Valid

// Germany
var (r9, p9) = PostalCode.TryCreate(CountryCode.Germany, "10115");          // ? 5-digit
var (r10, p10) = PostalCode.TryCreate(CountryCode.Germany, "80331");        // ? Valid (Munich)

// France
var (r11, p11) = PostalCode.TryCreate(CountryCode.France, "75001");         // ? 5-digit
var (r12, p12) = PostalCode.TryCreate(CountryCode.France, "69001");         // ? Valid (Lyon)

// Japan
var (r13, p13) = PostalCode.TryCreate(CountryCode.Japan, "100-0001");       // ? 7-digit with hyphen
var (r14, p14) = PostalCode.TryCreate(CountryCode.Japan, "150-0002");       // ? Valid (Tokyo)

// Australia
var (r15, p15) = PostalCode.TryCreate(CountryCode.Australia, "2000");       // ? 4-digit
var (r16, p16) = PostalCode.TryCreate(CountryCode.Australia, "3000");       // ? Valid (Melbourne)
```

#### Invalid Postal Codes

```csharp
// Wrong format for country
var (r1, p1) = PostalCode.TryCreate(CountryCode.UnitedStates, "ABC123");    // ? Invalid format
var (r2, p2) = PostalCode.TryCreate(CountryCode.Germany, "1234");           // ? Too short (needs 5)
var (r3, p3) = PostalCode.TryCreate(CountryCode.Canada, "12345");           // ? Wrong format

// Empty or null
var (r4, p4) = PostalCode.TryCreate(CountryCode.UnitedStates, null);        // ? Required
var (r5, p5) = PostalCode.TryCreate(CountryCode.UnitedStates, "");          // ? Required

// Too long
var (r6, p6) = PostalCode.TryCreate(CountryCode.UnitedStates, "12345678901"); // ? Exceeds 10 characters
```

#### Country Name Display

```csharp
var (_, postal) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "SW1A 2AA");

Console.WriteLine(postal.CountryCode);      // UnitedKingdom
Console.WriteLine(postal.GetCountryName()); // "United Kingdom"
```

---

## ?? Domain Model

### Address

Represents a complete physical address with validated components.

#### Key Features
- Validates all address components
- Country-specific postal code validation
- Optional second address line
- Optional state/province
- Automatic country detection from postal code
- Formatted display
- Comprehensive validation with detailed error messages
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, address) = Address.TryCreate(
    street: "123 Main Street",
    addressLine2: null,
    city: "New York",
    country: CountryCode.UnitedStates,
    postalCode: "10001"
);

if (result.IsValid)
{
    Console.WriteLine(address.Street.Value);        // "123 Main Street"
    Console.WriteLine(address.City.Value);          // "New York"
    Console.WriteLine(address.PostalCode.Value);    // "10001"
    Console.WriteLine(address.Country);             // UnitedStates
    Console.WriteLine(address.ToString());          
    // "123 Main Street, New York, 10001, United States"
}
```

#### Complete Address with All Fields

```csharp
var (result, address) = Address.TryCreate(
    street: "1600 Pennsylvania Avenue",
    addressLine2: "The White House",
    city: "Washington",
    country: CountryCode.UnitedStates,
    postalCode: "20500",
    stateProvince: "DC"
);

if (result.IsValid)
{
    Console.WriteLine(address.Street.Value);            // "1600 Pennsylvania Avenue"
    Console.WriteLine(address.AddressLine2.Value);      // "The White House"
    Console.WriteLine(address.City.Value);              // "Washington"
    Console.WriteLine(address.StateProvince.Value);     // "DC"
    Console.WriteLine(address.PostalCode.Value);        // "20500"
    Console.WriteLine(address.Country);                 // UnitedStates
    
    Console.WriteLine(address.ToString());
    // "1600 Pennsylvania Avenue, The White House, Washington, DC, 20500, United States"
}
```

#### Validation Examples

```csharp
// ? Valid - Minimal address (no state, no address line 2)
var (r1, a1) = Address.TryCreate(
    "123 Main Street",
    null,
    "New York",
    CountryCode.UnitedStates,
    "10001"
);
// r1.IsValid == true

// ? Valid - With apartment number
var (r2, a2) = Address.TryCreate(
    "456 Oak Avenue",
    "Apt 4B",
    "Los Angeles",
    CountryCode.UnitedStates,
    "90001"
);
// r2.IsValid == true

// ? Valid - With state
var (r3, a3) = Address.TryCreate(
    "789 Elm Street",
    null,
    "Chicago",
    CountryCode.UnitedStates,
    "60601",
    "IL"
);
// r3.IsValid == true

// ? Invalid - Missing street
var (r4, a4) = Address.TryCreate(
    "",
    null,
    "New York",
    CountryCode.UnitedStates,
    "10001"
);
// r4.IsValid == false, error on Street

// ? Invalid - Missing city
var (r5, a5) = Address.TryCreate(
    "123 Main Street",
    null,
    "",
    CountryCode.UnitedStates,
    "10001"
);
// r5.IsValid == false, error on City

// ? Invalid - Invalid postal code
var (r6, a6) = Address.TryCreate(
    "123 Main Street",
    null,
    "New York",
    CountryCode.UnitedStates,
    "INVALID"
);
// r6.IsValid == false, error on PostalCode

// ? Invalid - Multiple errors
var (r7, a7) = Address.TryCreate(
    "",           // Missing street
    null,
    "",           // Missing city
    CountryCode.UnitedStates,
    "INVALID"     // Invalid postal code
);
// r7.IsValid == false, multiple errors
```

#### International Addresses

```csharp
// UK Address
var (r1, ukAddress) = Address.TryCreate(
    "10 Downing Street",
    null,
    "London",
    CountryCode.UnitedKingdom,
    "SW1A 2AA"
);
// "10 Downing Street, London, SW1A 2AA, United Kingdom"

// German Address
var (r2, deAddress) = Address.TryCreate(
    "Platz der Republik 1",
    null,
    "Berlin",
    CountryCode.Germany,
    "11011"
);
// "Platz der Republik 1, Berlin, 11011, Germany"

// French Address
var (r3, frAddress) = Address.TryCreate(
    "55 Rue du Faubourg Saint-Honoré",
    null,
    "Paris",
    CountryCode.France,
    "75008"
);
// "55 Rue du Faubourg Saint-Honoré, Paris, 75008, France"

// Japanese Address
var (r4, jpAddress) = Address.TryCreate(
    "1-2-3 Shibuya",
    null,
    "Tokyo",
    CountryCode.Japan,
    "150-0002"
);
// "1-2-3 Shibuya, Tokyo, 150-0002, Japan"

// Canadian Address
var (r5, caAddress) = Address.TryCreate(
    "350 Sparks Street",
    null,
    "Ottawa",
    CountryCode.Canada,
    "K1A 0N5",
    "ON"
);
// "350 Sparks Street, Ottawa, ON, K1A 0N5, Canada"
```

#### Properties and Methods

```csharp
var (_, address) = Address.TryCreate(
    "123 Main Street",
    "Apt 4B",
    "New York",
    CountryCode.UnitedStates,
    "10001",
    "NY"
);

// Access components
Console.WriteLine(address.Street.Value);            // "123 Main Street"
Console.WriteLine(address.AddressLine2.Value);      // "Apt 4B"
Console.WriteLine(address.City.Value);              // "New York"
Console.WriteLine(address.StateProvince.Value);     // "NY"
Console.WriteLine(address.PostalCode.Value);        // "10001"
Console.WriteLine(address.Country);                 // UnitedStates (from PostalCode)

// Formatted output
Console.WriteLine(address.ToString());
// "123 Main Street, Apt 4B, New York, NY, 10001, United States"
```

#### ToString() Formatting

```csharp
// Without optional fields
var (_, addr1) = Address.TryCreate(
    "10 Downing Street",
    null,
    "London",
    CountryCode.UnitedKingdom,
    "SW1A 2AA"
);
Console.WriteLine(addr1.ToString());
// "10 Downing Street, London, SW1A 2AA, United Kingdom"

// With state/province
var (_, addr2) = Address.TryCreate(
    "1600 Pennsylvania Avenue",
    null,
    "Washington",
    CountryCode.UnitedStates,
    "20500",
    "DC"
);
Console.WriteLine(addr2.ToString());
// "1600 Pennsylvania Avenue, Washington, DC, 20500, United States"

// With address line 2
var (_, addr3) = Address.TryCreate(
    "123 Main Street",
    "Suite 100",
    "Boston",
    CountryCode.UnitedStates,
    "02101",
    "MA"
);
Console.WriteLine(addr3.ToString());
// "123 Main Street, Suite 100, Boston, MA, 02101, United States"
```

---

### AddressBuilder

Fluent builder for creating validated `Address` with step-by-step construction.

#### Key Features
- Fluent interface for readable address construction
- Convenience methods for common patterns
- Automatic validation before building
- Reusable with `Reset()` method

#### Basic Builder Usage

```csharp
using Validated.Primitives.Domain.Builders;

var builder = new AddressBuilder();

// Build step-by-step
var (result, address) = builder
    .WithStreet("123 Main Street")
    .WithCity("New York")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("10001")
    .Build();
```

#### Complete Address Construction

```csharp
var builder = new AddressBuilder();

var (result, address) = builder
    .WithStreet("1600 Pennsylvania Avenue")
    .WithAddressLine2("The White House")
    .WithCity("Washington")
    .WithStateProvince("DC")
    .WithPostalCode("20500")
    .WithCountry(CountryCode.UnitedStates)
    .Build();

if (result.IsValid)
{
    Console.WriteLine("Address created successfully!");
    Console.WriteLine(address.ToString());
}
```

#### Convenience Methods

```csharp
var builder = new AddressBuilder();

// Method 1: Set minimal address all at once
var (r1, a1) = builder
    .WithAddress(
        street: "123 Main Street",
        city: "New York",
        country: CountryCode.UnitedStates,
        postalCode: "10001"
    )
    .Build();

// Method 2: Set complete address all at once
var (r2, a2) = builder
    .Reset()
    .WithAddress(
        street: "456 Oak Avenue",
        city: "Los Angeles",
        country: CountryCode.UnitedStates,
        postalCode: "90001",
        addressLine2: "Apt 4B",
        stateProvince: "CA"
    )
    .Build();
```

#### Builder Reuse

```csharp
var builder = new AddressBuilder();

// First address
var (result1, address1) = builder
    .WithStreet("123 Main Street")
    .WithCity("New York")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("10001")
    .Build();

// Reuse builder for different address
var (result2, address2) = builder
    .Reset()  // Clear previous values
    .WithStreet("10 Downing Street")
    .WithCity("London")
    .WithCountry(CountryCode.UnitedKingdom)
    .WithPostalCode("SW1A 2AA")
    .Build();

// Another reuse
var (result3, address3) = builder
    .Reset()
    .WithAddress(
        "350 Sparks Street",
        "Ottawa",
        CountryCode.Canada,
        "K1A 0N5",
        stateProvince: "ON"
    )
    .Build();
```

#### Error Handling with Builder

```csharp
var builder = new AddressBuilder();

// Build with invalid/missing data
var (result, address) = builder
    .WithStreet("")  // Invalid - empty
    .WithCity("")    // Invalid - empty
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("INVALID")  // Invalid format
    .Build();

if (!result.IsValid)
{
    Console.WriteLine($"Found {result.Errors.Count} validation errors:");
    
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  [{error.Code}] {error.MemberName}: {error.Message}");
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

All address types follow the same validation pattern:

```csharp
var (result, value) = Type.TryCreate(...);

if (result.IsValid)
{
    // Use the validated value
    SaveAddress(value);
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

### Builder Pattern

Use builders for step-by-step construction:

```csharp
var builder = new AddressBuilder();

var (result, value) = builder
    .WithStreet(street)
    .WithCity(city)
    .WithCountry(country)
    .WithPostalCode(postalCode)
    .Build();

// Reuse the builder
builder.Reset();
var (result2, value2) = builder
    .WithAddress(newStreet, newCity, newCountry, newPostalCode)
    .Build();
```

### JSON Serialization

All types support JSON serialization:

```csharp
using System.Text.Json;

var (_, address) = Address.TryCreate(
    "123 Main Street",
    null,
    "New York",
    CountryCode.UnitedStates,
    "10001"
);

// Serialize
string json = JsonSerializer.Serialize(address);

// Deserialize
var deserialized = JsonSerializer.Deserialize<Address>(json);
```

---

## ?? Real-World Examples

### E-Commerce Checkout

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.Domain.Builders;
using Validated.Primitives.ValueObjects;

public class CheckoutService
{
    public async Task<CheckoutResult> CreateOrder(OrderRequest request)
    {
        // Validate shipping address
        var (shippingResult, shippingAddress) = new AddressBuilder()
            .WithStreet(request.ShippingStreet)
            .WithAddressLine2(request.ShippingApt)
            .WithCity(request.ShippingCity)
            .WithStateProvince(request.ShippingState)
            .WithPostalCode(request.ShippingZip)
            .WithCountry(request.ShippingCountry)
            .Build();

        if (!shippingResult.IsValid)
        {
            return CheckoutResult.ValidationFailed(
                "Shipping Address",
                shippingResult.Errors
            );
        }

        // Validate billing address (if different)
        Address billingAddress;
        if (request.BillingIsSameAsShipping)
        {
            billingAddress = shippingAddress;
        }
        else
        {
            var (billingResult, billing) = new AddressBuilder()
                .WithAddress(
                    request.BillingStreet,
                    request.BillingCity,
                    request.BillingCountry,
                    request.BillingZip,
                    request.BillingApt,
                    request.BillingState
                )
                .Build();

            if (!billingResult.IsValid)
            {
                return CheckoutResult.ValidationFailed(
                    "Billing Address",
                    billingResult.Errors
                );
            }

            billingAddress = billing;
        }

        // Create order with validated addresses
        var order = await CreateOrder(shippingAddress, billingAddress, request.Items);
        
        return CheckoutResult.Success(order.Id);
    }
}
```

### User Profile Management

```csharp
public class UserProfileService
{
    public async Task<UpdateResult> UpdateAddress(string userId, AddressUpdateRequest request)
    {
        // Validate new address
        var (result, address) = Address.TryCreate(
            request.Street,
            request.AddressLine2,
            request.City,
            request.Country,
            request.PostalCode,
            request.StateProvince
        );

        if (!result.IsValid)
        {
            return UpdateResult.ValidationFailed(result.Errors);
        }

        // Verify address is deliverable (external service)
        var deliverabilityResult = await _addressVerificationService.VerifyAsync(address);
        if (!deliverabilityResult.IsDeliverable)
        {
            return UpdateResult.Failed(
                "Address cannot be verified. Please check your address details."
            );
        }

        // Update user profile
        await _userRepository.UpdateAddress(userId, address);

        return UpdateResult.Success();
    }
}
```

### Shipping Label Generator

```csharp
public class ShippingLabelService
{
    public async Task<ShippingLabel> GenerateLabel(Order order)
    {
        var fromAddress = await GetWarehouseAddress(order.FulfillmentCenter);
        var toAddress = order.ShippingAddress;

        // Validate both addresses
        if (fromAddress == null || toAddress == null)
        {
            throw new InvalidOperationException("Valid addresses required for shipping label");
        }

        // Calculate shipping zone based on postal codes
        var zone = CalculateShippingZone(
            fromAddress.PostalCode.Value,
            toAddress.PostalCode.Value,
            fromAddress.Country,
            toAddress.Country
        );

        // Determine if international shipping
        var isInternational = fromAddress.Country != toAddress.Country;

        // Generate label
        var label = new ShippingLabel
        {
            OrderId = order.Id,
            FromAddress = FormatAddressForLabel(fromAddress),
            ToAddress = FormatAddressForLabel(toAddress),
            ShippingZone = zone,
            IsInternational = isInternational,
            ServiceType = DetermineServiceType(zone, isInternational),
            GeneratedAt = DateTime.UtcNow
        };

        return label;
    }

    private string FormatAddressForLabel(Address address)
    {
        var lines = new List<string>();
        
        lines.Add(address.Street.Value);
        
        if (address.AddressLine2 != null)
        {
            lines.Add(address.AddressLine2.Value);
        }

        var cityLine = address.City.Value;
        if (address.StateProvince != null)
        {
            cityLine += $", {address.StateProvince.Value}";
        }
        cityLine += $" {address.PostalCode.Value}";
        lines.Add(cityLine);

        lines.Add(address.PostalCode.GetCountryName());

        return string.Join("\n", lines);
    }
}
```

### Address Autocomplete API

```csharp
[ApiController]
[Route("api/addresses")]
public class AddressController : ControllerBase
{
    [HttpPost("validate")]
    public IActionResult ValidateAddress([FromBody] AddressValidationRequest request)
    {
        var (result, address) = Address.TryCreate(
            request.Street,
            request.AddressLine2,
            request.City,
            request.Country,
            request.PostalCode,
            request.StateProvince
        );

        if (!result.IsValid)
        {
            return BadRequest(new
            {
                Valid = false,
                Errors = result.Errors.Select(e => new
                {
                    Field = e.MemberName,
                    Message = e.Message,
                    Code = e.Code
                })
            });
        }

        return Ok(new
        {
            Valid = true,
            FormattedAddress = address.ToString(),
            Components = new
            {
                Street = address.Street.Value,
                AddressLine2 = address.AddressLine2?.Value,
                City = address.City.Value,
                StateProvince = address.StateProvince?.Value,
                PostalCode = address.PostalCode.Value,
                Country = address.Country.ToString(),
                CountryName = address.PostalCode.GetCountryName()
            }
        });
    }

    [HttpGet("countries/{country}/postal-codes/{postalCode}")]
    public IActionResult ValidatePostalCode(string country, string postalCode)
    {
        if (!Enum.TryParse<CountryCode>(country, true, out var countryCode))
        {
            return BadRequest("Invalid country code");
        }

        var (result, postal) = PostalCode.TryCreate(countryCode, postalCode);

        if (!result.IsValid)
        {
            return BadRequest(new
            {
                Valid = false,
                Errors = result.Errors
            });
        }

        return Ok(new
        {
            Valid = true,
            PostalCode = postal.Value,
            Country = postal.CountryCode.ToString(),
            CountryName = postal.GetCountryName()
        });
    }
}
```

### Address Book Management

```csharp
public class AddressBookService
{
    public async Task<AddressBook> GetUserAddressBook(string userId)
    {
        var addresses = await _repository.GetUserAddresses(userId);

        return new AddressBook
        {
            UserId = userId,
            Addresses = addresses.Select(a => new AddressEntry
            {
                Id = a.Id,
                Label = a.Label,  // "Home", "Work", "Billing", etc.
                Address = a.Address,
                IsDefault = a.IsDefault,
                FormattedAddress = a.Address.ToString()
            }).ToList()
        };
    }

    public async Task<SaveResult> SaveAddress(
        string userId,
        string label,
        string street,
        string? addressLine2,
        string city,
        CountryCode country,
        string postalCode,
        string? stateProvince)
    {
        // Validate address
        var (result, address) = new AddressBuilder()
            .WithStreet(street)
            .WithAddressLine2(addressLine2)
            .WithCity(city)
            .WithCountry(country)
            .WithPostalCode(postalCode)
            .WithStateProvince(stateProvince)
            .Build();

        if (!result.IsValid)
        {
            return SaveResult.ValidationFailed(result.Errors);
        }

        // Check for duplicates
        var existingAddresses = await _repository.GetUserAddresses(userId);
        var isDuplicate = existingAddresses.Any(a => AddressesAreEqual(a.Address, address));

        if (isDuplicate)
        {
            return SaveResult.Failed("This address already exists in your address book");
        }

        // Save address
        var entry = new AddressBookEntry
        {
            UserId = userId,
            Label = label,
            Address = address,
            IsDefault = !existingAddresses.Any(),  // First address is default
            CreatedAt = DateTime.UtcNow
        };

        await _repository.SaveAddress(entry);

        return SaveResult.Success(entry.Id);
    }

    private bool AddressesAreEqual(Address a1, Address a2)
    {
        return a1.Street.Value == a2.Street.Value &&
               a1.AddressLine2?.Value == a2.AddressLine2?.Value &&
               a1.City.Value == a2.City.Value &&
               a1.StateProvince?.Value == a2.StateProvince?.Value &&
               a1.PostalCode.Value == a2.PostalCode.Value &&
               a1.Country == a2.Country;
    }
}
```

### International Shipping Calculator

```csharp
public class ShippingCalculator
{
    public async Task<ShippingQuote> CalculateShipping(
        Address origin,
        Address destination,
        decimal weight,
        PackageSize size)
    {
        // Determine if domestic or international
        var isDomestic = origin.Country == destination.Country;

        // Calculate distance (approximate based on postal codes)
        var distance = await _geoService.CalculateDistance(
            origin.PostalCode.Value,
            destination.PostalCode.Value
        );

        // Get base rate
        var baseRate = GetBaseRate(weight, size, distance, isDomestic);

        // Apply country-specific surcharges
        var surcharges = CalculateSurcharges(origin.Country, destination.Country);

        // Calculate delivery time estimate
        var deliveryDays = EstimateDeliveryTime(distance, isDomestic);

        return new ShippingQuote
        {
            Origin = origin.ToString(),
            Destination = destination.ToString(),
            BaseRate = baseRate,
            Surcharges = surcharges,
            TotalCost = baseRate + surcharges.Sum(s => s.Amount),
            EstimatedDeliveryDays = deliveryDays,
            IsDomestic = isDomestic,
            Distance = distance,
            QuoteDate = DateTime.UtcNow
        };
    }
}
```

---

## ?? Related Documentation

- [Builder Examples](builders_examples.md) - General builder pattern usage
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

For complete API documentation, see:
- XML documentation comments in source code
- IntelliSense in Visual Studio
- [Validated.Primitives Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)
- [Validated.Primitives.Domain Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)

---

## ?? Supported Countries for Postal Codes

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
