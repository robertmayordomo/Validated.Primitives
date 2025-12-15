# Validated Primitives

[![Build & Test](https://github.com/robertmayordomo/Validated.Primitives/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/robertmayordomo/Validated.Primitives/actions/workflows/build-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET library that provides strongly-typed, self-validating primitive value objects to eliminate primitive obsession and enforce domain constraints at compile-time.

## Packages

This repository contains two NuGet packages:

### 📦 Validated.Primitives (Core)
The foundation library providing low-level validated primitive value objects.

[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)

```bash
dotnet add package Validated.Primitives
```

**What it provides:**
- Email addresses, phone numbers, URLs, IP addresses
- Postal codes with country-specific validation (30+ countries)
- Date/time value objects (DateOfBirth, FutureDate, date ranges)
- Financial primitives (Money, Percentage, CurrencyCode)
- Credit card primitives (CreditCardNumber, CreditCardSecurityNumber, CreditCardExpiration)
- **Banking primitives** (SwiftCode/BIC, RoutingNumber, IbanNumber, BankAccountNumber, SortCode, Passport) - [See Banking Documentation](BANKING_VALUE_OBJECTS.md)
- **Identity primitives** (Passport with 30+ country formats)
- Text primitives (HumanName, AddressLine, City, StateProvince)
- Validation framework and error handling
- JSON serialization support

### 📦 Validated.Primitives.Domain (Composition)
Higher-level domain models composed from validated primitives.

[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.Domain.svg)](https://www.nuget.org/packages/Validated.Primitives.Domain/)

```bash
dotnet add package Validated.Primitives.Domain
```

**What it provides:**
- **PersonName** - Complete human name (first, last, middle) with computed properties (FullName, FormalName, Initials)
- **Address** - Physical mailing address with country-specific postal code validation
- **ContactInformation** - Email, primary/secondary phone, optional website
- **CreditCardDetails** - Complete payment card (number, CVV, expiration) with masking and expiration checking
- **BankingDetails** - Complete banking information (account number, SWIFT, routing number, sort code) with country-specific validation

**Think of it as:**
- `Validated.Primitives` = LEGO bricks (EmailAddress, PhoneNumber, PostalCode)
- `Validated.Primitives.Domain` = Complete LEGO models (Address, PersonName, ContactInformation)

## What are Validated Primitives?

Validated Primitives are value objects that encapsulate primitive types (strings, dates, numbers) with built-in validation rules. Instead of passing raw strings or dates throughout your application, you use strongly-typed objects that guarantee their validity.

### The Problem: Primitive Obsession

```csharp
// ❌ Traditional approach - primitive obsession
public class User
{
    public string Email { get; set; }           // Could be null, empty, or invalid
    public string PhoneNumber { get; set; }     // No format validation
    public DateTime DateOfBirth { get; set; }   // Could be in the future!
    public string PostalCode { get; set; }      // No country-specific validation
}

// Validation logic scattered everywhere
if (string.IsNullOrWhiteSpace(user.Email) || !IsValidEmail(user.Email))
{
    throw new ArgumentException("Invalid email");
}
```

### The Solution: Validated Primitives

```csharp
// ✅ With Validated Primitives
public class User
{
    public EmailAddress Email { get; set; }       // Always valid or null
    public PhoneNumber PhoneNumber { get; set; }  // Always properly formatted
    public DateOfBirth DateOfBirth { get; set; }  // Always in the past
    public PostalCode PostalCode { get; set; }    // Country-specific validation
}

// Validation happens at creation - guaranteed valid everywhere else
var (result, email) = EmailAddress.TryCreate(userInput);
if (!result.IsValid)
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
    return;
}

// email is guaranteed to be valid here
user.Email = email;
```

## Key Benefits

- **Type Safety**: Compiler prevents mixing up different string types
- **Self-Validating**: Validation logic lives with the data
- **Immutable**: Value objects are records - thread-safe by default
- **Explicit Intent**: Code clearly communicates business rules
- **Centralized Validation**: No scattered validation logic
- **Rich Error Messages**: Detailed validation feedback

## Installation

```bash
dotnet add package Validated.Primitives
```

## Available Value Objects

### 📧 Email & Communication
- **`EmailAddress`** - RFC 5322 compliant email format, max 256 characters
- **`PhoneNumber`** - International phone number validation with country code support
- **`WebsiteUrl`** - Valid HTTP/HTTPS URLs with proper scheme validation

### 📍 Location & Geography
- **`PostalCode`** - Country-specific postal code validation for 30+ countries
- **`City`** - City names, max 100 characters, letters/spaces/hyphens/apostrophes
- **`StateProvince`** - State or province names, max 100 characters
- **`AddressLine`** - Street address lines, max 200 characters

### 🌐 Network
- **`IpAddress`** - Valid IPv4 or IPv6 addresses
- **`MacAddress`** - MAC address validation supporting multiple formats (colon `AA:BB:CC:DD:EE:FF`, hyphen `AA-BB-CC-DD-EE-FF`, dot-separated Cisco `AABB.CCDD.EEFF`, continuous `AABBCCDDEEFF`), multicast/broadcast/all-zeros detection, OUI/NIC extraction, and address type identification (locally/universally administered, unicast/multicast)
- **`Barcode`** - Barcode validation supporting multiple formats (UPC-A 12-digit, EAN-13 13-digit, EAN-8 8-digit, Code39 alphanumeric with `*` delimiters, Code128 alphanumeric), automatic format detection, checksum validation for numeric formats, and normalized value extraction
- **`TrackingNumber`** - Shipping tracking number validation supporting 17 carrier formats (UPS, FedEx Express/Ground/SmartPost, USPS, DHL Express/eCommerce/Global Mail, Amazon Logistics, Royal Mail, Canada Post, Australia Post, TNT, China Post, Irish Post, LaserShip, OnTrac), automatic carrier detection, and normalized value extraction

### 📍 Geographic Coordinates
- **`Latitude`** - Validated latitude coordinate (-90 to +90 degrees) with configurable decimal places (0-8), hemisphere detection (North/South), and cardinal direction formatting
- **`Longitude`** - Validated longitude coordinate (-180 to +180 degrees) with configurable decimal places (0-8), hemisphere detection (East/West), and cardinal direction formatting

### 📅 Date & Time
- **`DateOfBirth`** - Must be in the past, cannot be future date
- **`FutureDate`** - Must be in the future, cannot be past date
- **`BetweenDatesSelection`** - Date within a specified range
- **`DateRange`** - Represents a range between two DateTimes with duration calculation
- **`DateOnlyRange`** - Represents a range between two DateOnly values
- **`TimeOnlyRange`** - Represents a range between two TimeOnly values

### 💳 Financial & Payment
- **`Money`** - Monetary amounts with currency codes and precision validation
- **`SmallUnitMoney`** - Monetary amounts in smallest currency unit (e.g., cents)
- **`Percentage`** - Percentage values with configurable decimal places
- **`CurrencyCode`** - ISO 4217 currency codes (USD, EUR, GBP, etc.)
- **`CreditCardNumber`** - Luhn-validated card numbers, 13-19 digits, rejects all-same-digit patterns
- **`CreditCardSecurityNumber`** - CVV/CVC security codes, 3-4 digits
- **`CreditCardExpiration`** - Card expiration date with automatic 2-digit year normalization
- **`SwiftCode`** - SWIFT/BIC codes for international bank transfers
- **`RoutingNumber`** - Bank routing numbers (ABA numbers) for US banks
- **`BankAccountNumber`** - International bank account numbers (IBAN) with country-specific validation
- **`SortCode`** - Bank sort codes for UK and Ireland

### 🆔 Identification Numbers
- **`SocialSecurityNumber`** - US Social Security Numbers with format validation (XXX-XX-XXXX), area/group/serial number validation, masking support (XXX-XX-6789), and advertising number detection

### 👤 Personal Information
- **`HumanName`** - Individual name parts (first, middle, last), 1-50 characters, letters/hyphens/apostrophes/spaces

## Barcode Usage Examples

The **`Barcode`** validated primitive supports multiple barcode formats with automatic format detection and checksum validation:

### Supported Formats

- **UPC-A** (12 digits) - Universal Product Code with checksum validation
- **EAN-13** (13 digits) - European Article Number with checksum validation  
- **EAN-8** (8 digits) - Short EAN format with checksum validation
- **Code39** - Alphanumeric with `*` delimiters (e.g., `*PRODUCT123*`)
- **Code128** - Alphanumeric barcodes without asterisks

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Validate a UPC-A barcode
var (result, barcode) = Barcode.TryCreate("042100005264");
if (result.IsValid)
{
    Console.WriteLine($"Valid barcode: {barcode.Value}");
    Console.WriteLine($"Format: {barcode.Format}"); // Output: UpcA
}

// Validate an EAN-13 barcode
var (eanResult, eanBarcode) = Barcode.TryCreate("5901234123457");
if (eanResult.IsValid)
{
    Console.WriteLine($"EAN-13: {eanBarcode.Value}");
    Console.WriteLine($"Format: {eanBarcode.Format}"); // Output: Ean13
}

// Validate a Code39 barcode
var (code39Result, code39) = Barcode.TryCreate("*PRODUCT-123*");
if (code39Result.IsValid)
{
    Console.WriteLine($"Code39: {code39.Value}");
    Console.WriteLine($"Format: {code39.Format}"); // Output: Code39
}
```

### Barcode with Separators

Barcodes with hyphens or spaces are automatically normalized:

```csharp
// Input with separators
var (result, barcode) = Barcode.TryCreate("0421-0000-5264");
if (result.IsValid)
{
    Console.WriteLine($"Original: {barcode.Value}");        // Output: 0421-0000-5264
    Console.WriteLine($"Normalized: {barcode.GetNormalized()}"); // Output: 042100005264
    Console.WriteLine($"Format: {barcode.Format}");         // Output: UpcA
}
```

### E-Commerce Product Example

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Barcode? UpcBarcode { get; set; }
    public Barcode? EanBarcode { get; set; }
    public Barcode? InternalCode { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = new Product { Name = request.Name };

        // Validate UPC barcode
        if (!string.IsNullOrEmpty(request.UpcBarcode))
        {
            var (upcResult, upc) = Barcode.TryCreate(request.UpcBarcode, nameof(request.UpcBarcode));
            if (!upcResult.IsValid)
                return BadRequest(new { errors = upcResult.Errors });
            
            // Ensure it's actually UPC-A format
            if (upc!.Format != BarcodeFormat.UpcA)
                return BadRequest(new { error = "UPC barcode must be in UPC-A format" });
                
            product.UpcBarcode = upc;
        }

        // Validate EAN barcode
        if (!string.IsNullOrEmpty(request.EanBarcode))
        {
            var (eanResult, ean) = Barcode.TryCreate(request.EanBarcode, nameof(request.EanBarcode));
            if (!eanResult.IsValid)
                return BadRequest(new { errors = eanResult.Errors });
                
            product.EanBarcode = ean;
        }

        // Validate internal Code128 barcode
        if (!string.IsNullOrEmpty(request.InternalCode))
        {
            var (codeResult, code) = Barcode.TryCreate(request.InternalCode, nameof(request.InternalCode));
            if (!codeResult.IsValid)
                return BadRequest(new { errors = codeResult.Errors });
                
            product.InternalCode = code;
        }

        // Save product...
        return Ok(product);
    }

    [HttpGet("{barcode}")]
    public IActionResult FindByBarcode(string barcode)
    {
        var (result, validBarcode) = Barcode.TryCreate(barcode);
        if (!result.IsValid)
            return BadRequest(new { error = "Invalid barcode format" });

        // Search by normalized barcode value to match regardless of separators
        var normalizedBarcode = validBarcode!.GetNormalized();
        
        // Search logic here...
        return Ok(new { searchTerm = normalizedBarcode, format = validBarcode.Format });
    }
}
```

### Validation Error Handling

```csharp
var (result, barcode) = Barcode.TryCreate("INVALID-CODE", "ProductBarcode");

if (!result.IsValid)
{
    // Display all validation errors
    Console.WriteLine("Validation failed:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  {error.MemberName}: {error.Message}");
    }
    
    // Or get as bullet list
    Console.WriteLine(result.ToBulletList());
    
    // Or get as dictionary for JSON response
    var errorDict = result.ToDictionary();
}
```

### JSON Serialization

Barcodes serialize as simple strings in JSON:

```csharp
var product = new Product
{
    Id = 1,
    Name = "Sample Product",
    UpcBarcode = Barcode.TryCreate("042100005264").Value,
    InternalCode = Barcode.TryCreate("*PROD-123*").Value
};

var json = JsonSerializer.Serialize(product);
// Output: {"Id":1,"Name":"Sample Product","UpcBarcode":"042100005264","InternalCode":"*PROD-123*"}

var deserialized = JsonSerializer.Deserialize<Product>(json);
Console.WriteLine(deserialized.UpcBarcode?.Format); // Output: UpcA
```

### API Integration Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var (emailResult, email) = EmailAddress.TryCreate(request.Email, nameof(request.Email));
        if (!emailResult.IsValid)
        {
            return BadRequest(emailResult.ToDictionary());
        }
        
        var (phoneResult, phone) = PhoneNumber.TryCreate(request.Phone, nameof(request.Phone));
        if (!phoneResult.IsValid)
        {
            return BadRequest(phoneResult.ToDictionary());
        }
        
        var (postalResult, postalCode) = PostalCode.TryCreate(
            request.CountryCode, 
            request.PostalCode, 
            nameof(request.PostalCode)
        );
        if (!postalResult.IsValid)
        {
            return BadRequest(postalResult.ToDictionary());
        }
        
        // Create user with validated primitives
        var user = new User
        {
            Email = email,
            Phone = phone,
            PostalCode = postalCode
        };
        
        // Save user...
        
        return Ok(user);
    }
}
```

## Using Validated.Primitives.Domain

For more complex scenarios, use the **Validated.Primitives.Domain** package which provides pre-built domain aggregates:

### Complete User Profile Example

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

// Create a complete address
var (addressResult, address) = Address.TryCreate(
    street: "123 Main Street",
    addressLine2: "Apt 4B",
    city: "New York",
    country: CountryCode.UnitedStates,
    postalCode: "10001",
    stateProvince: "NY"
);

// Create a person name
var (nameResult, personName) = PersonName.TryCreate(
    firstName: "John",
    lastName: "Doe",
    middleName: "Michael"
);

// Create contact information
var (contactResult, contact) = ContactInformation.TryCreate(
    countryCode: CountryCode.UnitedStates,
    email: "john.doe@example.com",
    primaryPhone: "+1-555-123-4567",
    secondaryPhone: null,
    website: "https://johndoe.com"
);

// Create credit card details
var (cardResult, card) = CreditCardDetails.TryCreate(
    cardNumber: "4111 1111 1111 1111",
    securityNumber: "123",
    expirationMonth: 12,
    expirationYear: 2025
);

if (addressResult.IsValid && nameResult.IsValid && 
    contactResult.IsValid && cardResult.IsValid)
{
    Console.WriteLine($"Name: {personName.FullName}");
    Console.WriteLine($"Formal: {personName.FormalName}");
    Console.WriteLine($"Initials: {personName.Initials}");
    
    Console.WriteLine($"\nAddress: {address}");
    
    Console.WriteLine($"\nContact: {contact.Email.Value}");
    Console.WriteLine($"Phone: {contact.PrimaryPhone.Value}");
    
    Console.WriteLine($"\nCard: {card.GetMaskedCardNumber()}");
    Console.WriteLine($"Expires: {card.Expiration}");
    Console.WriteLine($"Is Expired: {card.IsExpired()}");
}
```

### Benefits of Domain Models

1. **Grouped Validation** - Validate all related fields in one call
2. **Rich Behavior** - `FullName`, `FormalName`, `Initials`, `GetMaskedCardNumber()`, `IsExpired()`
3. **Cohesive APIs** - Pass `ContactInformation` instead of Email + Phone + Website separately
4. **Guaranteed Consistency** - If you have an `Address`, all its parts are valid

**Domain Models Available:**
- `PersonName` - FirstName + LastName + MiddleName with computed properties
- `Address` - Complete mailing address with country-specific validation
- `ContactInformation` - Email + Phones + Website
- `CreditCardDetails` - CardNumber + CVV + Expiration with masking
- `BankingDetails` - AccountNumber + SWIFT + RoutingNumber + SortCode with country-specific validation

## ??? Builder Pattern for Complex Types

For easier construction of complex domain models, use the fluent builder pattern:

### AddressBuilder

Build addresses with a fluent interface that handles validation automatically:

```csharp
using Validated.Primitives.Domain.Builders;
using Validated.Primitives.ValueObjects;

// Simple address with required fields
var (result, address) = new AddressBuilder()
    .WithStreet("123 Main Street")
    .WithCity("New York")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("10001")
    .Build();

if (result.IsValid)
{
    Console.WriteLine(address!.ToString());
    // Output: 123 Main Street, New York, 10001, United States
}

// Complete address with all optional fields
var (result, address) = new AddressBuilder()
    .WithStreet("456 Oak Avenue")
    .WithAddressLine2("Apartment 4B")
    .WithCity("Los Angeles")
    .WithStateProvince("California")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("90001")
    .Build();

// Using the convenience method
var (result, address) = new AddressBuilder()
    .WithAddress(
        street: "789 Elm Street",
        city: "Chicago",
        country: CountryCode.UnitedStates,
        postalCode: "60601",
        addressLine2: "Suite 200",
        stateProvince: "Illinois")
    .Build();
```

### CreditCardBuilder

Build credit card details with flexible input formats:

```csharp
using Validated.Primitives.Domain.Builders;

// Basic credit card details
var (result, card) = new CreditCardBuilder()
    .WithCardNumber("4111 1111 1111 1111")
    .WithSecurityCode("123")
    .WithExpiration(12, 2025)
    .Build();

// Using string expiration format
var (result, card) = new CreditCardBuilder()
    .WithCardNumber("5500 0000 0000 0004")
    .WithSecurityCode(456)  // Can use int or string
    .WithExpiration("12/25")  // Supports MM/YY or MM/YYYY
    .Build();

// Using DateTime
var (result, card) = new CreditCardBuilder()
    .WithCardNumber("3400 0000 0000 009")
    .WithSecurityCode("789")
    .WithExpiration(DateTime.Now.AddYears(2))
    .Build();

if (result.IsValid)
{
    Console.WriteLine($"Card: {card!.GetMaskedCardNumber()}");
    Console.WriteLine($"Expires: {card.Expiration}");
    Console.WriteLine($"Valid: {!card.IsExpired()}");
}
else
{
    // All validation errors collected at once
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.MemberName}: {error.Message}");
    }
}
```

### BankingDetailsBuilder

Build banking details with required fields and optional convenience methods for SWIFT, routing number, and sort code:

```csharp
using Validated.Primitives.Domain.Builders;

// Basic banking details
var (result, bankingDetails) = new BankingDetailsBuilder()
    .WithCountry(CountryCode.UnitedStates)
    .WithAccountNumber("123456789")
    .Build();

// Banking details with SWIFT code (international)
var (result, bankingDetails) = new BankingDetailsBuilder()
    .WithCountry(CountryCode.Germany)
    .WithAccountNumber("DE89370400440532013000")
    .WithSwiftCode("DEUTDEDBFRA")
    .Build();

// Banking details with routing number (US) and sort code (UK/Ireland)
var (result, bankingDetails) = new BankingDetailsBuilder()
    .WithCountry(CountryCode.UnitedKingdom)
    .WithAccountNumber("GB29NWBK60161331926819")
    .WithRoutingNumber("123456789")
    .WithSortCode("601613")
    .Build();

if (result.IsValid)
{
    Console.WriteLine($"Account: {bankingDetails.AccountNumber}");
    Console.WriteLine($"SWIFT: {bankingDetails.SwiftCode}");
    Console.WriteLine($"Routing: {bankingDetails.RoutingNumber}");
    Console.WriteLine($"Sort Code: {bankingDetails.SortCode}");
}
else
{
    // Handle validation errors
    Console.WriteLine(result.ToBulletList());
}
```

### Builder Features

**? Key Benefits:**
- **Fluent Interface** - Method chaining for readable code
- **Flexible Input** - Multiple ways to provide the same data
- **Comprehensive Validation** - All errors collected in one call
- **Nullable Parameters** - Accepts incomplete data and validates properly
- **Reusable** - Reset and reuse builder instances
- **Type Safe** - Compile-time checks for required parameters

**?? Reusing Builders:**

```csharp
var builder = new AddressBuilder();

// Build first address
var (result1, address1) = builder
    .WithStreet("111 First Street")
    .WithCity("Denver")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("80201")
    .Build();

// Reset and build second address
var (result2, address2) = builder
    .Reset()
    .WithStreet("222 Second Avenue")
    .WithCity("Phoenix")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("85001")
    .Build();
```

**?? Form Integration Example:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateAddressRequest request)
    {
        // Builder handles null/incomplete data gracefully
        var (result, address) = new AddressBuilder()
            .WithStreet(request.Street)
            .WithAddressLine2(request.AddressLine2)
            .WithCity(request.City)
            .WithStateProvince(request.StateProvince)
            .WithCountry(request.Country)
            .WithPostalCode(request.PostalCode)
            .Build();

        if (!result.IsValid)
        {
            return BadRequest(new 
            { 
                errors = result.Errors.Select(e => new 
                { 
                    field = e.MemberName, 
                    message = e.Message,
                    code = e.Code 
                })
            });
        }

        // Save valid address...
        return Ok(new { id = 1, address = address!.ToString() });
    }
}
```

**Available Builders:**
- `AddressBuilder` - Required: street, city, country, postalCode | Optional: addressLine2, stateProvince
- `CreditCardBuilder` - Required: cardNumber, securityCode, expiration | Multiple expiration formats supported
- `BankingDetailsBuilder` - Required: country, accountNumber | Optional: swiftCode, routingNumber (USA), sortCode (UK/Ireland) | Convenience methods for US, UK, and international banking

## Tracking Number Usage Examples

The **`TrackingNumber`** validated primitive supports validation of shipping tracking numbers across multiple carriers:

### Supported Formats

- **UPS** - 18 characters starting with "1Z"
- **FedEx Express** - 12 digits
- **FedEx Ground** - 15 digits  
- **FedEx SmartPost** - 22 digits
- **USPS** - 20-22 alphanumeric, or 13 characters (2 letters + 9 digits + 2 letters ending in "US" or other non-GB/CN/IE codes)
- **DHL Express** - 10 digits
- **DHL eCommerce** - 22 alphanumeric starting with "GM"
- **DHL Global Mail** - 13-16 alphanumeric (use mixed letters/digits for 13-char to avoid ambiguity with Australia Post)
- **Amazon Logistics** - "TBA" followed by 12 digits
- **Royal Mail** - 13 characters (2 letters + 9 digits + 2 letters ending in "GB")
- **Canada Post** - 16 alphanumeric
- **Australia Post** - 13 digits (all numeric)
- **TNT** - 9 digits (13-digit TNT numbers are ambiguous with Australia Post)
- **China Post** - 13 characters (2 letters + 9 digits + 2 letters ending in "CN")
- **Irish Post (An Post)** - 13 characters (2 letters + 9 digits + 2 letters ending in "IE")
- **LaserShip** - "1LS" followed by 12 digits
- **OnTrac** - "C" followed by 14 digits

**Note on Ambiguous Formats:**
- **13-character alphanumeric (2 letters + 9 digits + 2 letters)**: Detected by country suffix
  - Ends with "GB" → Royal Mail
  - Ends with "CN" → China Post
  - Ends with "IE" → Irish Post
  - Ends with "US" or other codes → USPS (international tracking format)
- **13-digit all-numeric**: Australia Post vs TNT vs DHL Global Mail
  - Defaults to **Australia Post** (most common)
  - Use 9-digit format for unambiguous TNT detection
  - Use 13-char alphanumeric (with letters) for unambiguous DHL Global Mail detection
- **14-16 characters**: 
  - All-numeric → Could be Australia Post (extended) or DHL Global Mail (defaults to DHL Global Mail)
  - With letters → DHL Global Mail

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Validate a UPS tracking number
var (result, tracking) = TrackingNumber.TryCreate("1Z999AA10123456784");
if (result.IsValid)
{
    Console.WriteLine($"Valid tracking: {tracking.Value}");
    Console.WriteLine($"Carrier: {tracking.GetCarrierName()}"); // Output: UPS
}

// Validate a FedEx tracking number
var (fedexResult, fedexTracking) = TrackingNumber.TryCreate("986578788855");
if (fedexResult.IsValid)
{
    Console.WriteLine($"FedEx tracking: {fedexTracking.Value}");
    Console.WriteLine($"Carrier: {fedexTracking.GetCarrierName()}"); // Output: FedEx Express
}

// Validate an Amazon tracking number
var (amazonResult, amazonTracking) = TrackingNumber.TryCreate("TBA123456789012");
if (amazonResult.IsValid)
{
    Console.WriteLine($"Amazon tracking: {amazonTracking.Value}");
    Console.WriteLine($"Carrier: {amazonTracking.GetCarrierName()}"); // Output: Amazon Logistics
}

// Validate an Irish Post tracking number
var (irishResult, irishTracking) = TrackingNumber.TryCreate("RX123456789IE");
if (irishResult.IsValid)
{
    Console.WriteLine($"Irish Post tracking: {irishTracking.Value}");
    Console.WriteLine($"Carrier: {irishTracking.GetCarrierName()}"); // Output: Irish Post
}
```

### Tracking Number with Separators

Tracking numbers with hyphens or spaces are automatically normalized:

```csharp
// Input with separators
var (result, tracking) = TrackingNumber.TryCreate("1Z-999AA-1012345-6784");
if (result.IsValid)
{
    Console.WriteLine($"Original: {tracking.Value}");        // Output: 1Z-999AA-1012345-6784
    Console.WriteLine($"Normalized: {tracking.GetNormalized()}"); // Output: 1Z999AA10123456784
    Console.WriteLine($"Carrier: {tracking.GetCarrierName()}");   // Output: UPS
}

// Lowercase input is also handled
var (result2, tracking2) = TrackingNumber.TryCreate("1z999aa10123456784");
if (result2.IsValid)
{
    Console.WriteLine($"Normalized: {tracking2.GetNormalized()}"); // Output: 1Z999AA10123456784
}
```

### E-Commerce Shipment Example

```csharp
public class Shipment
{
    public int Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public TrackingNumber PrimaryTracking { get; set; = null!;
    public TrackingNumber? SecondaryTracking { get; set; }
    public TrackingNumber? InternationalTracking { get; set; }
    public DateTime ShippedDate { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateShipment([FromBody] CreateShipmentRequest request)
    {
        var shipment = new Shipment 
        { 
            OrderId = request.OrderId,
            ShippedDate = DateTime.UtcNow
        };

        // Validate primary tracking number
        var (primaryResult, primaryTracking) = TrackingNumber.TryCreate(
            request.PrimaryTracking, 
            nameof(request.PrimaryTracking));
        
        if (!primaryResult.IsValid)
            return BadRequest(new { errors = primaryResult.Errors });
            
        shipment.PrimaryTracking = primaryTracking!;

        // Validate optional secondary tracking
        if (!string.IsNullOrEmpty(request.SecondaryTracking))
        {
            var (secondaryResult, secondaryTracking) = TrackingNumber.TryCreate(
                request.SecondaryTracking, 
                nameof(request.SecondaryTracking));
            
            if (!secondaryResult.IsValid)
                return BadRequest(new { errors = secondaryResult.Errors });
                
            shipment.SecondaryTracking = secondaryTracking;
        }

        // Validate optional international tracking
        if (!string.IsNullOrEmpty(request.InternationalTracking))
        {
            var (intlResult, intlTracking) = TrackingNumber.TryCreate(
                request.InternationalTracking, 
                nameof(request.InternationalTracking));
            
            if (!intlResult.IsValid)
                return BadRequest(new { errors = intlResult.Errors });
                
            shipment.InternationalTracking = intlTracking;
        }

        // Save shipment...
        return Ok(new 
        { 
            id = 1, 
            orderId = shipment.OrderId,
            primaryCarrier = shipment.PrimaryTracking.GetCarrierName(),
            trackingNumber = shipment.PrimaryTracking.Value
        });
    }

    [HttpGet("track/{trackingNumber}")]
    public IActionResult TrackShipment(string trackingNumber)
    {
        var (result, tracking) = TrackingNumber.TryCreate(trackingNumber);
        if (!result.IsValid)
            return BadRequest(new { error = "Invalid tracking number format" });

        // Search by normalized tracking number
        var normalizedTracking = tracking!.GetNormalized();
        var carrier = tracking.GetCarrierName();
        
        // Lookup shipment logic here...
        return Ok(new 
        { 
            trackingNumber = normalizedTracking, 
            carrier = carrier,
            format = tracking.Format
        });
    }

    [HttpGet("carriers")]
    public IActionResult GetSupportedCarriers()
    {
        var carriers = Enum.GetValues<TrackingNumberFormat>()
            .Where(f => f != TrackingNumberFormat.Unknown)
            .Select(f => new 
            { 
                format = f.ToString(), 
                name = GetCarrierDisplayName(f) 
            });
            
        return Ok(carriers);
    }

    private string GetCarrierDisplayName(TrackingNumberFormat format)
    {
        // Create a dummy tracking number to get the carrier name
        // In practice, you'd have a mapping or use the GetCarrierName method
        return format switch
        {
            TrackingNumberFormat.UPS => "UPS",
            TrackingNumberFormat.FedExExpress => "FedEx Express",
            TrackingNumberFormat.FedExGround => "FedEx Ground",
            TrackingNumberFormat.FedExSmartPost => "FedEx SmartPost",
            TrackingNumberFormat.USPS => "USPS",
            TrackingNumberFormat.DHLExpress => "DHL Express",
            TrackingNumberFormat.DHLEcommerce => "DHL eCommerce",
            TrackingNumberFormat.DHLGlobalMail => "DHL Global Mail",
            TrackingNumberFormat.AmazonLogistics => "Amazon Logistics",
            TrackingNumberFormat.RoyalMail => "Royal Mail",
            TrackingNumberFormat.CanadaPost => "Canada Post",
            TrackingNumberFormat.AustraliaPost => "Australia Post",
            TrackingNumberFormat.TNT => "TNT",
            TrackingNumberFormat.ChinaPost => "China Post",
            TrackingNumberFormat.LaserShip => "LaserShip",
            TrackingNumberFormat.OnTrac => "OnTrac",
            _ => "Unknown"
        };
    }
}
```

### Validation Error Handling

```csharp
var (result, tracking) = TrackingNumber.TryCreate("INVALID-TRACKING", "ShipmentTracking");

if (!result.IsValid)
{
    // Display all validation errors
    Console.WriteLine("Validation failed:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  {error.MemberName}: {error.Message}");
    }
    
    // Or get as bullet list
    Console.WriteLine(result.ToBulletList());
    
    // Or get as dictionary for JSON response
    var errorDict = result.ToDictionary();
}
```

### JSON Serialization

Tracking numbers serialize as simple strings in JSON:

```csharp
var shipment = new Shipment
{
    Id = 1,
    OrderId = "ORD-12345",
    PrimaryTracking = TrackingNumber.TryCreate("1Z999AA10123456784").Value!,
    SecondaryTracking = TrackingNumber.TryCreate("986578788855").Value,
    ShippedDate = DateTime.UtcNow
};

var json = JsonSerializer.Serialize(shipment);
// Output: {"Id":1,"OrderId":"ORD-12345","PrimaryTracking":"1Z999AA10123456784","SecondaryTracking":"986578788855",...}

var deserialized = JsonSerializer.Deserialize<Shipment>(json);
Console.WriteLine(deserialized.PrimaryTracking.GetCarrierName()); // Output: UPS
Console.WriteLine(deserialized.SecondaryTracking?.GetCarrierName()); // Output: FedEx Express
```

### Multi-Carrier Shipment Tracking

```csharp
public class MultiCarrierShipment
{
    public string OrderId { get; set; } = string.Empty;
    public List<ShipmentLeg> ShipmentLegs { get; set; } = new();
}

public class ShipmentLeg
{
    public TrackingNumber Tracking { get; set; } = null!;
    public string Carrier => Tracking.GetCarrierName();
    public string Stage { get; set; } = string.Empty;
    public DateTime EstimatedDelivery { get; set; }
}

// Create a multi-carrier shipment (e.g., international with handoff)
var shipment = new MultiCarrierShipment
{
    OrderId = "ORD-99999",
    ShipmentLegs = new List<ShipmentLeg>
    {
        new ShipmentLeg
        {
            Tracking = TrackingNumber.TryCreate("1Z999AA10123456784").Value!, // UPS domestic
            Stage = "Origin",
            EstimatedDelivery = DateTime.UtcNow.AddDays(2)
        },
        new ShipmentLeg
        {
            Tracking = TrackingNumber.TryCreate("RX123456789IE").Value!, // Royal Mail international
            Stage = "Destination",
            EstimatedDelivery = DateTime.UtcNow.AddDays(7)
        }
    }
};

// Display tracking information
foreach (var leg in shipment.ShipmentLegs)
{
    Console.WriteLine($"Stage: {leg.Stage}");
    Console.WriteLine($"Carrier: {leg.Carrier}");
    Console.WriteLine($"Tracking: {leg.Tracking.Value}");
    Console.WriteLine($"Format: {leg.Tracking.Format}");
    Console.WriteLine($"Estimated Delivery: {leg.EstimatedDelivery:yyyy-MM-dd}");
    Console.WriteLine();
}

```

## Latitude Usage Examples

The **`Latitude`** validated primitive represents geographic latitude coordinates with precise validation and formatting.

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Validate a latitude coordinate (New York City)
var (result, latitude) = Latitude.TryCreate(40.7128m, decimalPlaces: 6);
if (result.IsValid)
{
    Console.WriteLine($"Latitude: {latitude.Value}");           // Output: 40.7128
    Console.WriteLine($"Formatted: {latitude.ToString()}");      // Output: 40.712800°
    Console.WriteLine($"Hemisphere: {latitude.GetHemisphere()}"); // Output: North
    Console.WriteLine($"Cardinal: {latitude.ToCardinalString()}"); // Output: 40.712800° N
}

// Validate a southern hemisphere latitude (Sydney)
var (sydneyResult, sydneyLat) = Latitude.TryCreate(-33.8688m, decimalPlaces: 4);
if (sydneyResult.IsValid)
{
    Console.WriteLine($"Sydney: {sydneyLat.ToCardinalString()}"); // Output: 33.8688° S
}

// Equator
var (equatorResult, equator) = Latitude.TryCreate(0m);
if (equatorResult.IsValid)
{
    Console.WriteLine($"Equator: {equator.ToCardinalString()}"); // Output: 0.000000° N
}
```

### Valid Range

Latitude values must be between **-90° (South Pole)** and **+90° (North Pole)**:

```csharp
// Valid latitudes
var (valid1, _) = Latitude.TryCreate(90m);    // North Pole - Valid
var (valid2, _) = Latitude.TryCreate(-90m);   // South Pole - Valid
var (valid3, _) = Latitude.TryCreate(0m);     // Equator - Valid

// Invalid latitudes
var (invalid1, _) = Latitude.TryCreate(91m);   // Invalid: > 90
var (invalid2, _) = Latitude.TryCreate(-100m); // Invalid: < -90
```

### Decimal Places Precision

Configure decimal places (0-8) for different precision needs:

```csharp
// City-level precision (4 decimal places ≈ 11 meters)
var (result, cityLocation) = Latitude.TryCreate(40.7128m, decimalPlaces: 4);
Console.WriteLine(cityLocation.ToString()); // Output: 40.7128°

// GPS precision (6 decimal places ≈ 0.11 meters)
var (result, gpsLocation) = Latitude.TryCreate(40.712776m, decimalPlaces: 6);
Console.WriteLine(gpsLocation.ToString()); // Output: 40.712776°

// High precision (8 decimal places ≈ 1.1 millimeters)
var (result, precise) = Latitude.TryCreate(40.71277621m, decimalPlaces: 8);
Console.WriteLine(precise.ToString()); // Output: 40.71277621°

// Integer degrees only
var (result, simple) = Latitude.TryCreate(41m, decimalPlaces: 0);
Console.WriteLine(simple.ToString()); // Output: 41°
```

### Location Tracking Example

```csharp
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Latitude Latitude { get; set; } = null!;
    public decimal Longitude { get; set; } // Or create a Longitude value object
    public DateTime Timestamp { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateLocation([FromBody] CreateLocationRequest request)
    {
        // Validate latitude
        var (latResult, latitude) = Latitude.TryCreate(
            request.Latitude, 
            decimalPlaces: 6, 
            propertyName: nameof(request.Latitude));
        
        if (!latResult.IsValid)
            return BadRequest(new { errors = latResult.Errors });

        var location = new Location
        {
            Name = request.Name,
            Latitude = latitude!,
            Longitude = request.Longitude,
            Timestamp = DateTime.UtcNow
        };

        // Save location...
        return Ok(new
        {
            id = 1,
            name = location.Name,
            coordinates = new
            {
                latitude = location.Latitude.Value,
                latitudeFormatted = location.Latitude.ToCardinalString(),
                hemisphere = location.Latitude.GetHemisphere(),
                longitude = location.Longitude
            }
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetLocation(int id)
    {
        // Retrieve location...
        var location = new Location
        {
            Id = id,
            Name = "New York City",
            Latitude = Latitude.TryCreate(40.7128m, 6).Value!,
            Longitude = -74.0060m,
            Timestamp = DateTime.UtcNow
        };

        return Ok(new
        {
            id = location.Id,
            name = location.Name,
            latitude = location.Latitude.Value,
            latitudeCardinal = location.Latitude.ToCardinalString(),
            hemisphere = location.Latitude.GetHemisphere(),
            longitude = location.Longitude,
            timestamp = location.Timestamp
        });
    }
}
```

### Validation Error Handling

```csharp
var (result, latitude) = Latitude.TryCreate(100m, propertyName: "UserLatitude");

if (!result.IsValid)
{
    // Display all validation errors
    Console.WriteLine("Validation failed:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  {error.MemberName}: {error.Message}");
        // Output: UserLatitude: Value must be between -90 and 90.
    }
    
    // Or get as bullet list
    Console.WriteLine(result.ToBulletList());
    
    // Or get as dictionary for JSON response
    var errorDict = result.ToDictionary();
}
```

### JSON Serialization

Latitude serializes as an object with Value and DecimalPlaces:

```csharp
var location = new Location
{
    Id = 1,
    Name = "New York City",
    Latitude = Latitude.TryCreate(40.7128m, 6).Value!,
    Longitude = -74.0060m,
    Timestamp = DateTime.UtcNow
};

var json = JsonSerializer.Serialize(location);
// Output: {"Id":1,"Name":"New York City","Latitude":{"Value":40.7128,"DecimalPlaces":6},"Longitude":-74.0060,...}

var deserialized = JsonSerializer.Deserialize<Location>(json);
Console.WriteLine(deserialized.Latitude.ToCardinalString()); // Output: 40.712800° N
Console.WriteLine(deserialized.Latitude.GetHemisphere());     // Output: North
```

### Geographic Boundary Checking

```csharp
public class GeographicBounds
{
    public Latitude NorthBound { get; set; } = null!;
    public Latitude SouthBound { get; set; } = null!;
    
    public bool Contains(Latitude latitude)
    {
        return latitude.Value >= SouthBound.Value && 
               latitude.Value <= NorthBound.Value;
    }
}

// Define bounding box for continental United States
var bounds = new GeographicBounds
{
    NorthBound = Latitude.TryCreate(49.3457868m, 6).Value!, // Northern border
    SouthBound = Latitude.TryCreate(24.5465116m, 6).Value!  // Southern border
};

// Check if location is within bounds
var (_, newYork) = Latitude.TryCreate(40.7128m, 6);
var (_, london) = Latitude.TryCreate(51.5074m, 6);

Console.WriteLine(bounds.Contains(newYork!));  // Output: True
Console.WriteLine(bounds.Contains(london!));    // Output: False
```

### Precision Guide

| Decimal Places | Precision | Use Case |
|---|---|---|
| 0 | ~111 km | Country/region level |
| 1 | ~11 km | City level |
| 2 | ~1.1 km | Neighborhood |
| 3 | ~110 m | Street/field |
| 4 | ~11 m | Building/parcel |
| 5 | ~1.1 m | Tree/entrance |
| 6 | ~0.11 m | Standard GPS |
| 7 | ~1.1 cm | Survey grade |
| 8 | ~1.1 mm | Tectonic plates |

## Longitude Usage Examples

The **`Longitude`** validated primitive represents geographic longitude coordinates with precise validation and formatting.

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects.Geospatial;

// Validate a longitude coordinate (New York City)
var (result, longitude) = Longitude.TryCreate(-74.0060m, decimalPlaces: 6);
if (result.IsValid)
{
    Console.WriteLine($"Longitude: {longitude.Value}");            // Output: -74.0060
    Console.WriteLine($"Formatted: {longitude.ToString()}");        // Output: -74.006000°
    Console.WriteLine($"Hemisphere: {longitude.GetHemisphere()}");  // Output: West
    Console.WriteLine($"Cardinal: {longitude.ToCardinalString()}"); // Output: 74.006000° W
}

// Validate an eastern hemisphere longitude (Sydney)
var (sydneyResult, sydneyLon) = Longitude.TryCreate(151.2093m, decimalPlaces: 4);
if (sydneyResult.IsValid)
{
    Console.WriteLine($"Sydney: {sydneyLon.ToCardinalString()}"); // Output: 151.2093° E
}

// Prime Meridian
var (meridianResult, meridian) = Longitude.TryCreate(0m);
if (meridianResult.IsValid)
{
    Console.WriteLine($"Prime Meridian: {meridian.ToCardinalString()}"); // Output: 0.000000° E
}
```

### Valid Range

Longitude values must be between **-180° (West)** and **+180° (East)**:

```csharp
// Valid longitudes
var (valid1, _) = Longitude.TryCreate(180m);   // International Date Line East - Valid
var (valid2, _) = Longitude.TryCreate(-180m);  // International Date Line West - Valid
var (valid3, _) = Longitude.TryCreate(0m);     // Prime Meridian - Valid

// Invalid longitudes
var (invalid1, _) = Longitude.TryCreate(181m);   // Invalid: > 180
var (invalid2, _) = Longitude.TryCreate(-200m);  // Invalid: < -180
```

### Location Tracking System Example

```csharp
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Longitude Longitude { get; set; } = null!;
    public Latitude Latitude { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateLocation([FromBody] CreateLocationRequest request)
    {
        // Validate longitude
        var (lonResult, longitude) = Longitude.TryCreate(
            request.Longitude, 
            decimalPlaces: 6, 
            propertyName: nameof(request.Longitude));
        
        if (!lonResult.IsValid)
            return BadRequest(new { errors = lonResult.Errors });

        // Validate latitude
        var (latResult, latitude) = Latitude.TryCreate(
            request.Latitude, 
            decimalPlaces: 6, 
            propertyName: nameof(request.Latitude));
        
        if (!latResult.IsValid)
            return BadRequest(new { errors = latResult.Errors });

        var location = new Location
        {
            Name = request.Name,
            Longitude = longitude!,
            Latitude = latitude!,
            Timestamp = DateTime.UtcNow
        };

        // Save location...
        return Ok(new
        {
            id = 1,
            name = location.Name,
            coordinates = new
            {
                latitude = location.Latitude.Value,
                latitudeFormatted = location.Latitude.ToCardinalString(),
                hemisphere = location.Latitude.GetHemisphere(),
                longitude = location.Longitude.Value,
                longitudeFormatted = location.Longitude.ToCardinalString(),
                longitudeHemisphere = location.Longitude.GetHemisphere()
            }
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetLocation(int id)
    {
        // Retrieve location...
        var location = new Location
        {
            Id = id,
            Name = "New York City",
            Latitude = Latitude.TryCreate(40.7128m, 6).Value!,
            Longitude = Longitude.TryCreate(-74.0060m, 6).Value!,
            Timestamp = DateTime.UtcNow
        };

        return Ok(new
        {
            id = location.Id,
            name = location.Name,
            latitude = location.Latitude.Value,
            latitudeCardinal = location.Latitude.ToCardinalString(),
            latitudeHemisphere = location.Latitude.GetHemisphere(),
            longitude = location.Longitude.Value,
            longitudeCardinal = location.Longitude.ToCardinalString(),
            longitudeHemisphere = location.Longitude.GetHemisphere(),
            timestamp = location.Timestamp
        });
    }
}
```

### Validation Error Handling

```csharp
var (result, longitude) = Longitude.TryCreate(200m, propertyName: "UserLongitude");

if (!result.IsValid)
{
    // Display all validation errors
    Console.WriteLine("Validation failed:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  {error.MemberName}: {error.Message}");
        // Output: UserLongitude: Value must be between -180 and 180.
    }
    
    // Or get as bullet list
    Console.WriteLine(result.ToBulletList());
    
    // Or get as dictionary for JSON response
    var errorDict = result.ToDictionary();
}
```

### JSON Serialization

Longitude serializes alongside Latitude for complete coordinate data:

```csharp
var location = new Location
{
    Id = 1,
    Name = "New York City",
    Latitude = Latitude.TryCreate(40.7128m, 6).Value!,
    Longitude = Longitude.TryCreate(-74.0060m, 6).Value!,
    Timestamp = DateTime.UtcNow
};

var json = JsonSerializer.Serialize(location);
// Output: {
//   "Id":1,
//   "Name":"New York City",
//   "Latitude":{"Value":40.7128,"DecimalPlaces":6},
//   "Longitude":{"Value":-74.0060,"DecimalPlaces":6},
//   "Timestamp":"2024-01-15T10:30:00Z"
// }

var deserialized = JsonSerializer.Deserialize<Location>(json);
Console.WriteLine(deserialized.Longitude.ToCardinalString()); // Output: 74.006000° W
Console.WriteLine(deserialized.Latitude.ToCardinalString());     // Output: 40.712800° N
```

### Special Longitude Values

```csharp
// Prime Meridian (Greenwich, UK)
var (_, primeMeridian) = Longitude.TryCreate(0m, 2);
Console.WriteLine(primeMeridian!.ToCardinalString()); // Output: 0.00° E

// International Date Line (East)
var (_, dateLine) = Longitude.TryCreate(180m, 0);
Console.WriteLine(dateLine!.ToCardinalString()); // Output: 180° E

// International Date Line (West)
var (_, dateLineWest) = Longitude.TryCreate(-180m, 0);
Console.WriteLine(dateLineWest!.ToCardinalString()); // Output: 180° W
```

## Coordinate Domain Object Usage Examples

The **`Coordinate`** domain object combines Latitude and Longitude for complete GPS positioning with additional metadata.

### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;
using Validated.Primitives.Domain.Geospatial.Builders;

// Create a coordinate using TryCreate
var (result, coordinate) = Coordinate.TryCreate(
    latitude: 40.7128m,
    longitude: -74.0060m,
    decimalPlaces: 6);

if (result.IsValid)
{
    Console.WriteLine($"Location: {coordinate.ToString()}");
    // Output: Location: 40.712800° N, 74.006000° W
    
    Console.WriteLine($"Decimal Degrees: {coordinate.ToDecimalDegreesString()}");
    // Output: Decimal Degrees: 40.7128, -74.0060
    
    Console.WriteLine($"Google Maps: {coordinate.ToGoogleMapsFormat()}");
    // Output: Google Maps: 40.7128,-74.0060
}
```

### With Altitude and Accuracy

```csharp
// Create coordinate with altitude and accuracy
var (result, coordinate) = Coordinate.TryCreate(
    latitude: 40.7128m,
    longitude: -74.0060m,
    decimalPlaces: 6,
    altitude: 10.5m,      // 10.5 meters above sea level
    accuracy: 5.0m);       // ±5 meters accuracy

if (result.IsValid)
{
    Console.WriteLine(coordinate.ToString());
    // Output: 40.712800° N, 74.006000° W, 10.5m (±5m)
    
    Console.WriteLine($"Altitude: {coordinate.Altitude}m");
    Console.WriteLine($"Accuracy: ±{coordinate.Accuracy}m");
}
```

### Using the Builder Pattern

```csharp
var builder = new CoordinateBuilder();

var (result, coordinate) = builder
    .WithLatitude(40.7128m)
    .WithLongitude(-74.0060m)
    .WithAltitude(10.5m)
    .WithAccuracy(5.0m)
    .WithDecimalPlaces(6)
    .Build();

// Or use shorthand methods
var (result2, coordinate2) = new CoordinateBuilder()
    .WithCoordinates(40.7128m, -74.0060m)
    .Build();

// Or set everything at once
var (result3, coordinate3) = new CoordinateBuilder()
    .WithPosition(
        latitude: 40.7128m,
        longitude: -74.0060m,
        altitude: 10.5m,
        accuracy: 5.0m)
    .Build();
```

### Distance Calculations

Calculate distance between two coordinates using the Haversine formula:

```csharp
// Create coordinates for New York and Los Angeles
var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, losAngeles) = Coordinate.TryCreate(34.0522m, -118.2437m);

// Calculate distance in kilometers
var distanceKm = newYork!.DistanceTo(losAngeles!);
Console.WriteLine($"Distance: {distanceKm:F2} km");
// Output: Distance: 3944.42 km

// Convert to miles
var distanceMiles = distanceKm * 0.621371;
Console.WriteLine($"Distance: {distanceMiles:F2} miles");
// Output: Distance: 2451.03 miles
```

### Location Tracking System Example

```csharp
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Coordinate Position { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
}

public class LocationService
{
    public (ValidationResult Result, Location? Location) CreateLocation(
        string name,
        decimal latitude,
        decimal longitude,
        decimal? altitude = null,
        string? description = null)
    {
        var result = ValidationResult.Success();

        // Validate coordinate
        var (coordResult, coordinate) = Coordinate.TryCreate(
            latitude,
            longitude,
            decimalPlaces: 6,
            altitude: altitude);

        if (!coordResult.IsValid)
        {
            return (coordResult, null);
        }

        var location = new Location
        {
            Name = name,
            Position = coordinate!,
            Timestamp = DateTime.UtcNow,
            Description = description
        };

        return (result, location);
    }

    public double CalculateDistance(Location from, Location to)
    {
        return from.Position.DistanceTo(to.Position);
    }

    public string GetDirections(Location from, Location to)
    {
        var distance = CalculateDistance(from, to);
        var fromCoords = from.Position.ToGoogleMapsFormat();
        var toCoords = to.Position.ToGoogleMapsFormat();
        
        return $"From {from.Name} to {to.Name}: {distance:F2} km\n" +
               $"Google Maps: https://www.google.com/maps/dir/{fromCoords}/{toCoords}";
    }
}

// Usage
var service = new LocationService();

var (result1, empireState) = service.CreateLocation(
    "Empire State Building",
    40.7484m,
    -73.9857m,
    altitude: 443.2m,
    description: "Iconic NYC skyscraper");

var (result2, timesSquare) = service.CreateLocation(
    "Times Square",
    40.7580m,
    -73.9855m,
    description: "Commercial intersection in Midtown Manhattan");

if (result1.IsValid && result2.IsValid)
{
    Console.WriteLine(empireState!.Position.ToString());
    // Output: 40.748400° N, 73.985700° W, 443.2m
    
    var distance = service.CalculateDistance(empireState, timesSquare!);
    Console.WriteLine($"Distance: {distance:F2} km");
    // Output: Distance: 1.06 km
    
    Console.WriteLine(service.GetDirections(empireState, timesSquare));
}
```

### Geofencing Example

```csharp
public class Geofence
{
    public Coordinate Center { get; set; } = null!;
    public double RadiusKm { get; set; }
    
    public bool Contains(Coordinate point)
    {
        var distance = Center.DistanceTo(point);
        return distance <= RadiusKm;
    }
}

// Create a geofence around New York City (5 km radius)
var (_, nycCenter) = Coordinate.TryCreate(40.7128m, -74.0060m);
var geofence = new Geofence
{
    Center = nycCenter!,
    RadiusKm = 5.0
};

// Check if locations are within the geofence
var (_, empireState) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, centralPark) = Coordinate.TryCreate(40.7829m, -73.9654m);
var (_, boston) = Coordinate.TryCreate(42.3601m, -71.0589m);

Console.WriteLine($"Empire State Building: {geofence.Contains(empireState!)}"); // True
Console.WriteLine($"Central Park: {geofence.Contains(centralPark!)}");          // True
Console.WriteLine($"Boston: {geofence.Contains(boston!)}");                     // False
```

### API Integration Example

```csharp
public class CreateLocationRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? Altitude { get; set; }
    public decimal? Accuracy { get; set; }
    public string? Description { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateLocation([FromBody] CreateLocationRequest request)
    {
        // Validate coordinate using domain object
        var (result, coordinate) = Coordinate.TryCreate(
            request.Latitude,
            request.Longitude,
            decimalPlaces: 6,
            altitude: request.Altitude,
            accuracy: request.Accuracy);

        if (!result.IsValid)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(e => new
                {
                    field = e.MemberName,
                    message = e.Message,
                    code = e.Code
                })
            });
        }

        var location = new Location
        {
            Name = request.Name,
            Position = coordinate!,
            Description = request.Description,
            Timestamp = DateTime.UtcNow
        };

        // Save to database...

        return Ok(new
        {
            id = 1,
            name = location.Name,
            position = new
            {
                latitude = location.Position.Latitude.Value,
                longitude = location.Position.Longitude.Value,
                altitude = location.Position.Altitude,
                accuracy = location.Position.Accuracy,
                formatted = location.Position.ToCardinalString(),
                googleMaps = $"https://www.google.com/maps/search/?api=1&query={location.Position.ToGoogleMapsFormat()}"
            },
            description = location.Description,
            timestamp = location.Timestamp
        });
    }

    [HttpGet("distance")]
    public IActionResult CalculateDistance(
        [FromQuery] decimal lat1,
        [FromQuery] decimal lon1,
        [FromQuery] decimal lat2,
        [FromQuery] decimal lon2)
    {
        var (result1, coord1) = Coordinate.TryCreate(lat1, lon1);
        var (result2, coord2) = Coordinate.TryCreate(lat2, lon2);

        if (!result1.IsValid || !result2.IsValid)
        {
            return BadRequest("Invalid coordinates");
        }

        var distanceKm = coord1!.DistanceTo(coord2!);
        var distanceMiles = distanceKm * 0.621371;

        return Ok(new
        {
            from = coord1.ToCardinalString(),
            to = coord2.ToCardinalString(),
            distance = new
            {
                kilometers = Math.Round(distanceKm, 2),
                miles = Math.Round(distanceMiles, 2)
            }
        });
    }
}
```

### Validation Scenarios

```csharp
// Valid altitude range: -500m to 10,000m
var (result1, _) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: -400m);  // Valid (Dead Sea level)
var (result2, _) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 8848m);  // Valid (Mt. Everest)
var (result3, _) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: -600m);  // Invalid (too low)
var (result4, _) = Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 15000m); // Invalid (too high)

result1.IsValid.ShouldBeTrue();
result2.IsValid.ShouldBeTrue();
result3.IsValid.ShouldBeFalse();
result4.IsValid.ShouldBeFalse();

// Valid accuracy range: 0m to 1,000,000m
var (result5, _) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: 5m);       // Valid (GPS)
var (result6, _) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: -10m);     // Invalid (negative)
var (result7, _) = Coordinate.TryCreate(40.7128m, -74.0060m, accuracy: 2000000m); // Invalid (too large)

result5.IsValid.ShouldBeTrue();
result6.IsValid.ShouldBeFalse();
result7.IsValid.ShouldBeFalse();
```

### Famous Locations Examples

```csharp
// Create coordinates for famous world locations
var locations = new []
{
    ("Statue of Liberty", 40.6892m, -74.0445m, 93m),
    ("Eiffel Tower", 48.8584m, 2.2945m, 330m),
    ("Sydney Opera House", -33.8568m, 151.2153m, 0m),
    ("Great Pyramid of Giza", 29.9792m, 31.1342m, 138.8m),
    ("Mount Everest", 27.9881m, 86.9250m, 8848.86m)
};

foreach (var (name, lat, lon, alt) in locations)
{
    var (result, coord) = Coordinate.TryCreate(lat, lon, altitude: alt, decimalPlaces: 4);
    
    if (result.IsValid)
    {
        Console.WriteLine($"{name}:");
        Console.WriteLine($"  Position: {coord!.ToCardinalString()}");
        Console.WriteLine($"  Google Maps: https://maps.google.com/?q={coord.ToGoogleMapsFormat()}");
    }
}

// Output:
// Statue of Liberty:
//   Position: 40.6892° N, 74.0445° W, 93.0m
//   Google Maps: https://maps.google.com/?q=40.6892,-74.0445
// ...
```
