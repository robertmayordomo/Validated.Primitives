# Validated Primitives

[![Build & Test](https://github.com/robertmayordomo/Validated.Primitives/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/robertmayordomo/Validated.Primitives/actions/workflows/build-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET library that provides strongly-typed, self-validating primitive value objects to eliminate primitive obsession and enforce domain constraints at compile-time.

This repository contains two NuGet packages: `Validated.Primitives` (core primitives) and `Validated.Primitives.Domain` (composed domain models).

### 📦 Validated.Primitives (Core)
The foundation library providing low-level validated primitive value objects.

[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)

```bash
dotnet add package Validated.Primitives
```

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


**What it provides:**
- **Communication primitives** - EmailAddress, PhoneNumber, WebsiteUrl
- **Network primitives** - IpAddress, MacAddress
- **Postal/Geographic primitives** - PostalCode (with 30+ country-specific validation), City, StateProvince, AddressLine, CountryCode
- **Geospatial primitives** - Latitude, Longitude
- **Date/Time primitives** - DateOfBirth, FutureDate, DateRange, DateOnlyRange, TimeOnlyRange, BetweenDatesSelection
- **Financial primitives** - Money, SmallUnitMoney, Percentage, CurrencyCode
- **Credit Card primitives** - CreditCardNumber, CreditCardSecurityNumber, CreditCardExpiration
- **Banking primitives** - SwiftCode/BIC, RoutingNumber, IbanNumber, BankAccountNumber, SortCode
- **Identity primitives** - Passport (30+ country formats), SocialSecurityNumber, DrivingLicenseNumber
- **Logistics primitives** - Barcode, TrackingNumber
- **Text primitives** - HumanName
- **Core features** - Validation framework, error handling, JSON serialization support

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
- **`Latitude`** - Validated latitude coordinate (-90 to +90 degrees) with configurable decimal places (0-8), hemisphere detection (North/South), and cardinal direction formatting
- **`Longitude`** - Validated longitude coordinate (-180 to +180 degrees) with configurable decimal places (0-8), hemisphere detection (East/West), and cardinal direction formatting
- 
### 🌐 Network
- **`IpAddress`** - Valid IPv4 or IPv6 addresses
- **`MacAddress`** - MAC address validation supporting multiple formats (colon `AA:BB:CC:DD:EE:FF`, hyphen `AA-BB-CC-DD-EE-FF`, dot-separated Cisco `AABB.CCDD.EEFF`, continuous `AABBCCDDEEFF`), multicast/broadcast/all-zeros detection, OUI/NIC extraction, and address type identification (locally/universally administered, unicast/multicast)
- [**`Barcode`**](doc/barcode_examples.md) - Barcode validation supporting multiple formats (UPC-A 12-digit, EAN-13 13-digit, EAN-8 8-digit, Code39 alphanumeric with `*` delimiters, Code128 alphanumeric), automatic format detection, checksum validation for numeric formats, and normalized value extraction. 
- [**`TrackingNumber`**](doc/tracking_examples.md) - Shipping tracking number validation supporting 17 carrier formats (UPS, FedEx Express/Ground/SmartPost, USPS, DHL Express/eCommerce/Global Mail, Amazon Logistics, Royal Mail, Canada Post, Australia Post, TNT, China Post, Irish Post, LaserShip, OnTrac), automatic carrier detection, and normalized value extraction 

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

### 📦 Validated.Primitives.Domain (Composed Models)
Higher-level domain models built from core primitives.

[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.Domain.svg)](https://www.nuget.org/packages/Validated.Primitives.Domain/)

```bash
dotnet add package Validated.Primitives.Domain
```

**What it provides:**
- **Personal information** - PersonName, ContactInformation
- **Address models** - Address (with AddressBuilder)
- **Financial models** - CreditCardDetails (with CreditCardBuilder), BankingDetails (with BankingDetailsBuilder)
- [**Geospatial models**](doc/geospatial_readme.md) - Coordinate, GeoDistance, GeoBoundary, GeospatialRoute, RouteSegment (with builders)
- **JSON serialization** support for all domain models

## Available Domain Objects
*Available in `Validated.Primitives.Domain` package*

### 👤 Personal Information
- **`PersonName`** - Complete person name with first, middle (optional), and last name components, built from validated `HumanName` primitives
- [**`ContactInformation`**](doc/contactinformation_readme.md) - Contact details including email address, phone number, and website URL, composed from validated communication primitives

### 🏠 Address
- [**`Address`**](doc/address_readme.md) - Complete postal address with street address lines, city, state/province, postal code, and country code
  - **`AddressBuilder`** - Fluent builder for constructing validated addresses step-by-step

### 💳 Financial
- [**`CreditCardDetails`**](doc/creditcard_readme.md) - Complete credit card information including card number, security code, and expiration date
  - **`CreditCardBuilder`** - Fluent builder for constructing validated credit card details
- [**`BankingDetails`**](doc/banking_readme.md) - Complete banking information including account number, routing number, SWIFT/BIC code, IBAN, and sort code
  - **`BankingDetailsBuilder`** - Fluent builder for constructing validated banking details

### 🌍 Geospatial [examples](doc/geospatial_examples.md) 
- **`Coordinate`** - Geographic coordinate combining validated latitude and longitude with elevation (optional), coordinate system support, and distance calculation methods
  - **`CoordinateBuilder`** - Fluent builder for constructing validated coordinates
- **`GeoDistance`** - Represents distance between two coordinates with multiple unit support (meters, kilometers, miles, nautical miles), includes distance calculation using Haversine formula
- **`GeoBoundary`** - Geographic boundary/bounding box defined by northeast and southwest coordinates, includes containment checks and boundary validation
- **`GeospatialRoute`** - Complete route with multiple segments, total distance calculation, and waypoint management
  - **`GeospatialRouteBuilder`** - Fluent builder for constructing multi-segment routes
- **`RouteSegment`** - Individual route segment with start/end coordinates, distance, and duration
  - **`RouteSegmentBuilder`** - Fluent builder for constructing route segments

## Where to find API reference

See the XML docs, IntelliSense and the source code for full API reference and available value objects.
