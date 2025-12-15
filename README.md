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
