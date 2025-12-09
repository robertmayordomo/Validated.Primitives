# Validated Primitives

[![Build & Test](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml)
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

### 🏦 Banking
- **`BankAccountNumber`** - Country-specific bank account number validation (supports IBAN and domestic formats)
- **`IbanNumber`** - International Bank Account Number (IBAN) with ISO 13616 validation, mod-97 checksum, 70+ countries
- **`RoutingNumber`** - US ABA routing number, 9 digits with checksum validation
- **`SortCode`** - UK/Ireland bank sort codes, 6 digits (XX-XX-XX format)
- **`SwiftCode`** - SWIFT/BIC codes for international wire transfers (ISO 9362), 8 or 11 characters

### 👤 Personal Information
- **`HumanName`** - Individual name parts (first, middle, last), 1-50 characters, letters/hyphens/apostrophes/spaces
- **`SocialSecurityNumber`** - US Social Security Number with format validation and area/group/serial number checks
- **`Passport`** - Country-specific passport number validation for 30+ countries (ICAO Document 9303 standards)
- **`DrivingLicenseNumber`** - Country-specific driving license number validation for 25+ countries

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
