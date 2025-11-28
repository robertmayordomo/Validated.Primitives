# Validated Primitives

A .NET library that provides strongly-typed, self-validating primitive value objects to eliminate primitive obsession and enforce domain constraints at compile-time.

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

### Email & Communication
- **`EmailAddress`** - Valid email format, max 256 characters
- **`PhoneNumber`** - Valid phone number format
- **`WebsiteUrl`** - Valid HTTP/HTTPS URLs

### Location
- **`PostalCode`** - Country-specific postal code validation for 30+ countries

### Network
- **`IpAddress`** - Valid IPv4 or IPv6 addresses

### Date & Time
- **`DateOfBirth`** - Must be in the past
- **`FutureDate`** - Must be in the future
- **`BetweenDatesSelection`** - Date within a specified range
- **`DateRange`** - Represents a range between two DateTimes
- **`DateOnlyRange`** - Represents a range between two DateOnly values
- **`TimeOnlyRange`** - Represents a range between two TimeOnly values

## Usage Examples

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create a validated email address
var (result, email) = EmailAddress.TryCreate("user@example.com");

if (result.IsValid)
{
    Console.WriteLine($"Email: {email.Value}");
    // Use email.Value to access the underlying string
}
else
{
    // Handle validation errors
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}
```

### Postal Codes with Country Validation

```csharp
using Validated.Primitives.ValueObjects;

// Validate a US ZIP code
var (result, usZip) = PostalCode.TryCreate(CountryCode.UnitedStates, "12345");
if (result.IsValid)
{
    Console.WriteLine($"Valid US ZIP: {usZip.Value}");
    Console.WriteLine($"Country: {usZip.GetCountryName()}"); // "United States"
}

// Validate a UK postal code
var (result2, ukCode) = PostalCode.TryCreate(CountryCode.UnitedKingdom, "SW1A 1AA");
if (result2.IsValid)
{
    Console.WriteLine($"Valid UK postcode: {ukCode.Value}");
}

// Accept any format with Unknown or All
var (result3, genericCode) = PostalCode.TryCreate(CountryCode.All, "XYZ-123");
if (result3.IsValid)
{
    Console.WriteLine($"Generic postal code: {genericCode.Value}");
}

// Country-specific validation will fail for wrong formats
var (result4, invalid) = PostalCode.TryCreate(CountryCode.UnitedStates, "ABCDE");
if (!result4.IsValid)
{
    Console.WriteLine(result4.ToSingleMessage()); 
    // "PostalCode is not a valid postal code format for UnitedStates."
}
```

**Supported Countries** (30+):
- United States, United Kingdom, Canada, Australia
- Germany, France, Italy, Spain, Netherlands, Belgium, Switzerland, Austria
- Sweden, Norway, Denmark, Finland, Poland, Czech Republic, Hungary, Portugal, Ireland
- Japan, China, India, Brazil, Mexico, South Africa, New Zealand, Singapore, South Korea, Russia

**Special Country Codes**:
- `CountryCode.Unknown` - Accepts any postal code format (basic validation only)
- `CountryCode.All` - Accepts any postal code format (basic validation only)

### Handling Validation Errors

```csharp
var (result, phoneNumber) = PhoneNumber.TryCreate("invalid");

if (!result.IsValid)
{
    // Single line message
    Console.WriteLine(result.ToSingleMessage());
    
    // Bullet list format
    Console.WriteLine(result.ToBulletList());
    
    // Dictionary format (useful for APIs)
    var errors = result.ToDictionary();
    foreach (var kvp in errors)
    {
        Console.WriteLine($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
    }
}
```

### Domain Models

```csharp
public class Customer
{
    public Guid Id { get; init; }
    public EmailAddress Email { get; init; }
    public PhoneNumber Phone { get; init; }
    public DateOfBirth BirthDate { get; init; }
    public PostalCode PostalCode { get; init; }
    public WebsiteUrl? Website { get; init; }
    
    public static (ValidationResult, Customer?) Create(
        string email,
        string phone,
        DateTime birthDate,
        CountryCode country,
        string postalCode,
        string? website = null)
    {
        var validationResult = ValidationResult.Success();
        
        var (emailResult, emailValue) = EmailAddress.TryCreate(email);
        validationResult.Merge(emailResult);
        
        var (phoneResult, phoneValue) = PhoneNumber.TryCreate(phone);
        validationResult.Merge(phoneResult);
        
        var (birthResult, birthValue) = DateOfBirth.TryCreate(birthDate);
        validationResult.Merge(birthResult);
        
        var (postalResult, postalValue) = PostalCode.TryCreate(country, postalCode);
        validationResult.Merge(postalResult);
        
        WebsiteUrl? websiteValue = null;
        if (!string.IsNullOrWhiteSpace(website))
        {
            var (websiteResult, webValue) = WebsiteUrl.TryCreate(website);
            validationResult.Merge(websiteResult);
            websiteValue = webValue;
        }
        
        if (!validationResult.IsValid)
            return (validationResult, null);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = emailValue!,
            Phone = phoneValue!,
            BirthDate = birthValue!,
            PostalCode = postalValue!,
            Website = websiteValue
        };
        
        return (validationResult, customer);
    }
}

// Usage
var (result, customer) = Customer.Create(
    "john.doe@example.com",
    "+1-555-123-4567",
    new DateTime(1990, 5, 15),
    CountryCode.UnitedStates,
    "12345",
    "https://johndoe.com"
);

if (result.IsValid)
{
    // customer is guaranteed to have valid data
    Console.WriteLine($"Created customer: {customer.Email}");
}
else
{
    Console.WriteLine("Validation failed:");
    Console.WriteLine(result.ToBulletList());
}
```

### Date Ranges

```csharp
// Working with date ranges
var startDate = DateTime.Now;
var endDate = DateTime.Now.AddDays(7);

var (result, dateRange) = DateRange.TryCreate(startDate, endDate);
if (result.IsValid)
{
    Console.WriteLine($"Range: {dateRange.Start} to {dateRange.End}");
    Console.WriteLine($"Duration: {dateRange.Duration.TotalDays} days");
    
    // Check if a date is within the range
    var isInRange = dateRange.Contains(DateTime.Now.AddDays(3));
}

// DateOnly ranges
var (result2, dateOnlyRange) = DateOnlyRange.TryCreate(
    DateOnly.FromDateTime(startDate),
    DateOnly.FromDateTime(endDate)
);
```

### Custom Property Names

```csharp
// Customize validation error property names for better error messages
var (result, email) = EmailAddress.TryCreate(input, "CustomerEmail");

// Error message will reference "CustomerEmail" instead of default "Email"
if (!result.IsValid)
{
    // Output: "CustomerEmail: Invalid email format"
    Console.WriteLine(result.ToSingleMessage());
}

// Also works with postal codes
var (result2, postalCode) = PostalCode.TryCreate(
    CountryCode.UnitedStates, 
    input, 
    "ShippingPostalCode"
);
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

## Creating Custom Validated Primitives

You can easily create your own validated primitives by inheriting from `ValidatedValueObject<T>`:

```csharp
using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

public sealed record TaxId : ValidatedValueObject<string>
{
    private TaxId(string value, string propertyName = "TaxId") 
        : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(CommonValidators.Length(propertyName, 9, 11));
        // Add your custom validation logic
    }

    public static (ValidationResult Result, TaxId? Value) TryCreate(
        string value, 
        string propertyName = "TaxId")
    {
        var taxId = new TaxId(value, propertyName);
        var validationResult = taxId.Validate();
        var result = validationResult.IsValid ? taxId : null;
        return (validationResult, result);
    }
}
```

## Built-in Validators

The library includes several validator helpers:

### CommonValidators
- `NotNullOrWhitespace` - Ensures string is not null or whitespace
- `MaxLength` - Maximum string length
- `MinLength` - Minimum string length
- `Length` - Exact or range of acceptable lengths

### DateTimeValidators
- `BeforeToday` - Date must be in the past
- `AfterToday` - Date must be in the future
- `Between` - Date within a range

### EmailValidators
- `EmailFormat` - Valid email format

### PhoneValidators
- `PhoneNumber` - Valid phone number format

### UrlValidators
- `ValidUrl` - Valid URL format
- `HttpOrHttps` - Must be HTTP or HTTPS scheme

### IpValidators
- `ValidIpAddress` - Valid IPv4 or IPv6 address

### PostalCodeValidators
- `ValidFormat` - Basic postal code format (alphanumeric, spaces, hyphens)
- `ValidateCountryFormat` - Country-specific postal code validation


## Design Principles

1. **Immutability**: All value objects are records, ensuring thread safety
2. **Fail Fast**: Validation happens at creation time
3. **Explicit over Implicit**: `TryCreate` pattern makes validation explicit
4. **Rich Feedback**: Detailed validation errors with member names and codes
5. **Composability**: Easy to combine multiple validators
6. **Zero Magic**: No reflection, attributes, or hidden behavior

## Requirements

- .NET 8.0 or higher

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## License

MIT

## Resources

- [Primitive Obsession Code Smell](https://refactoring.guru/smells/primitive-obsession)
- [Value Objects in Domain-Driven Design](https://martinfowler.com/bliki/ValueObject.html)
- [C# Record Types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
