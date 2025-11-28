# Validated.Primitives.Domain - Project Summary

## ?? Overview

The `Validated.Primitives.Domain` project has been successfully created! This package contains complex domain types composed of multiple validated primitives from the `Validated.Primitives` package.

## ?? Package Information

- **Package Name**: `Validated.Primitives.Domain`
- **Version**: Auto-incremented (same as main package)
- **License**: MIT
- **Dependencies**: `Validated.Primitives`

## ??? Project Structure

```
src/Validated.Primitives.Domain/
??? Validated.Primitives.Domain.csproj
??? Address.cs
??? ContactInformation.cs
??? PersonName.cs

tests/Validated.Primitives.Domain.Tests/
??? Validated.Primitives.Domain.Tests.csproj
??? AddressTests.cs (21 tests)
??? ContactInformationTests.cs (20 tests)
??? PersonNameTests.cs (26 tests)
```

**Total: 67 comprehensive tests**

---

## ?? Domain Types

### 1. Address

Represents a validated physical address with country-specific postal code validation.

**Properties:**
- `Street` (required) - Street address, max 200 characters
- `City` (required) - City name, max 100 characters
- `StateProvince` (optional) - State or province, max 100 characters
- `PostalCode` (required) - Validated postal code using `PostalCode` primitive
- `Country` (derived) - Country code from postal code

**Usage:**
```csharp
var (result, address) = Address.TryCreate(
    "123 Main Street",
    "New York",
    CountryCode.UnitedStates,
    "10001",
    "NY");

if (result.IsValid)
{
    Console.WriteLine(address.ToString());
    // Output: 123 Main Street, New York, NY, 10001, United States
    Console.WriteLine(address.Country); // UnitedStates
}
```

**Features:**
- Country-specific postal code validation (30+ countries)
- Automatic trimming of whitespace
- Formatted `ToString()` output
- Immutable record type

---

### 2. ContactInformation

Represents validated contact information with email, phone numbers, and website.

**Properties:**
- `Email` (required) - Validated email using `EmailAddress` primitive
- `PrimaryPhone` (required) - Validated phone using `PhoneNumber` primitive
- `SecondaryPhone` (optional) - Optional secondary phone number
- `Website` (optional) - Validated website using `WebsiteUrl` primitive

**Usage:**
```csharp
var (result, contact) = ContactInformation.TryCreate(
    "john.doe@example.com",
    "+1-555-123-4567",
    "+1-555-987-6543",
    "https://johndoe.com");

if (result.IsValid)
{
    Console.WriteLine(contact.Email.Value);        // john.doe@example.com
    Console.WriteLine(contact.PrimaryPhone.Value); // +1-555-123-4567
    Console.WriteLine(contact.SecondaryPhone?.Value); // +1-555-987-6543
    Console.WriteLine(contact.Website?.Value);     // https://johndoe.com
}
```

**Features:**
- All fields use validated primitives
- Optional fields (secondary phone, website)
- Formatted `ToString()` output
- Comprehensive email and phone validation

---

### 3. PersonName

Represents a validated person's name with formatting helpers.

**Properties:**
- `FirstName` (required) - 1-50 characters
- `LastName` (required) - 1-50 characters
- `MiddleName` (optional) - Max 50 characters
- `FullName` (computed) - "FirstName MiddleName LastName"
- `FormalName` (computed) - "LastName, FirstName MiddleName"
- `Initials` (computed) - "J.M.D." format

**Usage:**
```csharp
var (result, name) = PersonName.TryCreate(
    "John",
    "Doe",
    "Michael");

if (result.IsValid)
{
    Console.WriteLine(name.FullName);    // John Michael Doe
    Console.WriteLine(name.FormalName);  // Doe, John Michael
    Console.WriteLine(name.Initials);    // J.M.D.
    Console.WriteLine(name.ToString());  // John Michael Doe
}
```

**Features:**
- Length validation (1-50 characters)
- Automatic trimming and normalization
- Multiple formatted outputs (FullName, FormalName, Initials)
- Supports international characters, hyphens, apostrophes

---

## ?? Testing

All domain types have comprehensive test coverage:

### AddressTests (21 tests)
- Valid address creation (US, UK, international)
- Street validation (required, max length)
- City validation (required, max length)
- State/province validation (optional, max length)
- Postal code validation (country-specific)
- Whitespace trimming
- Multiple error scenarios
- Formatting tests
- Equality tests

### ContactInformationTests (20 tests)
- Valid contact creation
- Email validation
- Primary/secondary phone validation
- Website validation (optional)
- Empty/whitespace handling for optional fields
- Multiple error scenarios
- Various email formats
- Various phone formats
- Formatting tests
- Equality tests

### PersonNameTests (26 tests)
- Valid name creation
- FirstName validation (required, length)
- LastName validation (required, length)
- MiddleName validation (optional, length)
- Whitespace trimming and normalization
- FullName formatting
- FormalName formatting
- Initials generation
- Single character names
- Hyphenated names
- Names with apostrophes
- International characters
- Equality tests

---

## ?? CI/CD Integration

### Updated Workflows

Both GitHub Actions workflows have been updated to build, test, and publish the domain package:

#### 1. Simple Workflow (`build-test-publish.yml`)
```yaml
env:
  PROJECT_PATH: 'src/Valdiated.Primatives/Validated.Primitives.csproj'
  DOMAIN_PROJECT_PATH: 'src/Validated.Primitives.Domain/Validated.Primitives.Domain.csproj'
  TEST_PROJECT_PATH: 'tests/Valdiated.Primatives.Tests/Validated.Primitives.Tests.csproj'
  DOMAIN_TEST_PROJECT_PATH: 'tests/Validated.Primitives.Domain.Tests/Validated.Primitives.Domain.Tests.csproj'
```

#### 2. GitVersion Workflow (`build-test-publish-gitversion.yml`)
- Same environment variables
- Same versioning strategy for both packages
- Both packages published to NuGet.org

### Build Scripts

Both local build scripts updated:

**PowerShell (Windows):**
```powershell
.\build-local.ps1
```

**Bash (Linux/Mac):**
```bash
./build-local.sh
```

Both packages will be created in `./artifacts/`:
- `Validated.Primitives.{version}.nupkg`
- `Validated.Primitives.Domain.{version}.nupkg`
- Symbol packages (`.snupkg`)

---

## ?? NuGet Package Metadata

```xml
<PackageId>Validated.Primitives.Domain</PackageId>
<Description>
  Domain models and complex types built from Validated.Primitives value objects. 
  Provides strongly-typed, self-validating domain entities for common use cases 
  like Address, ContactInformation, and PersonName.
</Description>
<PackageTags>
  validation;value-objects;ddd;domain-driven-design;domain-models;
  aggregates;composites;address;contact-information;strongly-typed;
  self-validating;type-safety;immutable;csharp;dotnet
</PackageTags>
```

---

## ?? Installation

Once published to NuGet:

```bash
# Install both packages
dotnet add package Validated.Primitives
dotnet add package Validated.Primitives.Domain
```

---

## ?? Usage Examples

### Complete Customer Example

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

public record Customer
{
    public Guid Id { get; init; }
    public PersonName Name { get; init; }
    public ContactInformation Contact { get; init; }
    public Address BillingAddress { get; init; }
    public Address? ShippingAddress { get; init; }
    public DateOfBirth DateOfBirth { get; init; }
    
    public static (ValidationResult Result, Customer? Value) TryCreate(
        string firstName,
        string lastName,
        string? middleName,
        string email,
        string phone,
        string street,
        string city,
        CountryCode country,
        string postalCode,
        string? stateProvince,
        DateTime dateOfBirth)
    {
        var result = ValidationResult.Success();
        
        // Validate person name
        var (nameResult, nameValue) = PersonName.TryCreate(
            firstName, lastName, middleName);
        result.Merge(nameResult);
        
        // Validate contact information
        var (contactResult, contactValue) = ContactInformation.TryCreate(
            email, phone);
        result.Merge(contactResult);
        
        // Validate billing address
        var (addressResult, addressValue) = Address.TryCreate(
            street, city, country, postalCode, stateProvince);
        result.Merge(addressResult);
        
        // Validate date of birth
        var (dobResult, dobValue) = DateOfBirth.TryCreate(dateOfBirth);
        result.Merge(dobResult);
        
        if (!result.IsValid)
            return (result, null);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = nameValue!,
            Contact = contactValue!,
            BillingAddress = addressValue!,
            DateOfBirth = dobValue!
        };
        
        return (result, customer);
    }
}

// Usage
var (result, customer) = Customer.TryCreate(
    "John", "Doe", "Michael",
    "john.doe@example.com",
    "+1-555-123-4567",
    "123 Main Street",
    "New York",
    CountryCode.UnitedStates,
    "10001",
    "NY",
    new DateTime(1990, 5, 15));

if (result.IsValid)
{
    Console.WriteLine($"Customer: {customer.Name.FullName}");
    Console.WriteLine($"Email: {customer.Contact.Email.Value}");
    Console.WriteLine($"Address: {customer.BillingAddress}");
}
else
{
    Console.WriteLine("Validation failed:");
    Console.WriteLine(result.ToBulletList());
}
```

---

## ? Build Verification

Build successful! ?

All tests pass:
- Validated.Primitives.Tests: All tests passing
- Validated.Primitives.Domain.Tests: All 67 tests passing

---

## ?? Next Steps

1. **Commit the changes:**
```bash
git add .
git commit -m "Add Validated.Primitives.Domain project with Address, ContactInformation, and PersonName"
git push
```

2. **Create a release:**
- Both packages will be automatically published to NuGet.org
- Use the same version number for consistency

3. **Update README.md** (optional):
- Add domain package installation instructions
- Add usage examples for domain types

4. **Expand domain types** (future):
Consider adding more domain types like:
- `CreditCard` - Card number, expiration, CVV
- `BankAccount` - Account number, routing number
- `Money` - Amount, currency
- `Credentials` - Username, password strength
- `GeoCoordinate` - Latitude, longitude

---

## ?? Documentation

All types include XML documentation comments for IntelliSense support and API documentation generation.

**Happy coding! ??**
