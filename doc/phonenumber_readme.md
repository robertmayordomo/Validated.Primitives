# Validated.Primitives.PhoneNumber

A validated phone number primitive with country-specific format validation, ensuring phone numbers are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`PhoneNumber` is a validated value object that represents a phone number with country-specific format validation. It supports 30+ countries and enforces length validation (7-20 characters). Once created, a `PhoneNumber` instance is guaranteed to be valid for the specified country.

### Key Features

- **Country-Specific Validation** - Validates phone format according to country rules
- **International Support** - Supports 30+ countries worldwide
- **Length Validation** - Enforces 7-20 character limit
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Phone Number

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, phone) = PhoneNumber.TryCreate(
    CountryCode.UnitedStates,
    "+1-555-123-4567"
);

if (result.IsValid)
{
    Console.WriteLine(phone.Value);            // "+1-555-123-4567"
    Console.WriteLine(phone.CountryCode);      // UnitedStates
    Console.WriteLine(phone.GetCountryName()); // "United States"
    Console.WriteLine(phone.ToString());       // "+1-555-123-4567"
    
    // Use the validated phone number
    await SendSmsVerification(phone);
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
public class Customer
{
    public Guid Id { get; set; }
    public PhoneNumber Phone { get; set; }  // Always valid or null
    public string Name { get; set; }
}

// Usage
var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, userInput);
if (result.IsValid)
{
    var customer = new Customer
    {
        Id = Guid.NewGuid(),
        Phone = phone,  // Guaranteed valid
        Name = customerName
    };
    
    await _customerRepository.SaveAsync(customer);
}
```

---

## ? Valid Phone Numbers

### United States

```csharp
// Valid US phone numbers
var (r1, p1) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "+1-555-123-4567");  // ? International format
var (r2, p2) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "(555) 123-4567");   // ? Parentheses format
var (r3, p3) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "555-123-4567");     // ? Domestic format
var (r4, p4) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "5551234567");       // ? Digits only

foreach (var (result, phone) in new[] { (r1, p1), (r2, p2), (r3, p3), (r4, p4) })
{
    Console.WriteLine($"{phone.Value}: {result.IsValid}");  // All true
}
```

### United Kingdom

```csharp
// Valid UK phone numbers
var (r1, p1) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "+44 20 7123 4567"); // ? London landline
var (r2, p2) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "+44 7700 900123");  // ? Mobile
var (r3, p3) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "020 7123 4567");    // ? Domestic landline
var (r4, p4) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "07700 900123");     // ? Domestic mobile
```

---

## ? Invalid Phone Numbers

### Too Short

```csharp
var (r1, p1) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "123");      // ? Too short (min 7 chars)
var (r2, p2) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "555-12");   // ? Too short

// r1.IsValid == false
// r1.Errors contains Length error
```

### Too Long

```csharp
var longPhone = new string('1', 25);
var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, longPhone);  // ? Too long (max 20 chars)

// result.IsValid == false
// result.Errors contains Length error
```

### Invalid Format

```csharp
var (r1, p1) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "abc-def-ghij"); // ? Letters not allowed
var (r2, p2) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "");             // ? Empty string
var (r3, p3) = PhoneNumber.TryCreate(CountryCode.UnitedStates, null);           // ? Null value
var (r4, p4) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "   ");          // ? Whitespace only
```

---

## ?? Test Cases

### Valid Test Cases

```csharp
public class PhoneNumberValidTests
{
    [Theory]
    [InlineData(CountryCode.UnitedStates, "+1-555-123-4567")]
    [InlineData(CountryCode.UnitedStates, "(555) 123-4567")]
    [InlineData(CountryCode.UnitedKingdom, "+44 20 7123 4567")]
    [InlineData(CountryCode.UnitedKingdom, "+44 7700 900123")]
    public void TryCreate_Succeeds_For_Valid_PhoneNumbers(CountryCode country, string phoneInput)
    {
        // Arrange & Act
        var (result, phone) = PhoneNumber.TryCreate(country, phoneInput);
        
        // Assert
        Assert.True(result.IsValid, $"Phone number should be valid: {phoneInput}");
        Assert.NotNull(phone);
        Assert.Equal(phoneInput, phone.Value);
        Assert.Equal(country, phone.CountryCode);
    }

    [Fact]
    public void Valid_US_PhoneNumber_Example()
    {
        // Arrange
        var phoneInput = "+1-555-123-4567";
        
        // Act
        var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, phoneInput);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.NotNull(phone);
        Assert.Equal("+1-555-123-4567", phone.Value);
        Assert.Equal(CountryCode.UnitedStates, phone.CountryCode);
        Assert.Equal("United States", phone.GetCountryName());
    }

    [Fact]
    public void Valid_UK_PhoneNumber_Example()
    {
        // Arrange
        var phoneInput = "+44 20 7123 4567";
        
        // Act
        var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, phoneInput);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.NotNull(phone);
        Assert.Equal("+44 20 7123 4567", phone.Value);
        Assert.Equal(CountryCode.UnitedKingdom, phone.CountryCode);
        Assert.Equal("United Kingdom", phone.GetCountryName());
    }
}
```

### Invalid Test Cases

```csharp
public class PhoneNumberInvalidTests
{
    [Theory]
    [InlineData(CountryCode.UnitedStates, "123")]           // Too short
    [InlineData(CountryCode.UnitedStates, "abc-def-ghij")]  // Invalid characters
    [InlineData(CountryCode.UnitedStates, "")]              // Empty string
    [InlineData(CountryCode.UnitedStates, "   ")]           // Whitespace only
    public void TryCreate_Fails_For_Invalid_PhoneNumbers(CountryCode country, string phoneInput)
    {
        // Arrange & Act
        var (result, phone) = PhoneNumber.TryCreate(country, phoneInput);
        
        // Assert
        Assert.False(result.IsValid, $"Phone number should be invalid: {phoneInput}");
        Assert.Null(phone);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Invalid_TooShort_PhoneNumber_Example()
    {
        // Arrange
        var phoneInput = "123";
        
        // Act
        var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, phoneInput);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Null(phone);
        Assert.Contains(result.Errors, e => e.Code == "Length");
    }

    [Fact]
    public void Invalid_TooLong_PhoneNumber_Example()
    {
        // Arrange
        var phoneInput = new string('1', 25);  // 25 characters
        
        // Act
        var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, phoneInput);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Null(phone);
        Assert.Contains(result.Errors, e => e.Code == "Length");
    }
}
```

---

## ?? Real-World Examples

### User Registration

```csharp
public class UserRegistrationService
{
    public async Task<RegistrationResult> RegisterUser(
        string email, 
        string phoneInput, 
        CountryCode country)
    {
        // Validate phone number
        var (phoneResult, phone) = PhoneNumber.TryCreate(country, phoneInput);
        if (!phoneResult.IsValid)
        {
            return RegistrationResult.Failed(
                $"Invalid phone number: {phoneResult.ToBulletList()}"
            );
        }

        // Create user with validated phone
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Phone = phone,  // Guaranteed valid
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        // Send SMS verification
        await _smsService.SendVerificationCodeAsync(phone.Value);

        return RegistrationResult.Success(user.Id);
    }
}
```

### SMS Verification

```csharp
public class SmsVerificationService
{
    public async Task<SendResult> SendVerificationCode(string phoneInput, CountryCode country)
    {
        // Validate phone number
        var (result, phone) = PhoneNumber.TryCreate(country, phoneInput);
        if (!result.IsValid)
        {
            return SendResult.ValidationFailed(result.Errors);
        }

        // Generate verification code
        var code = GenerateVerificationCode();
        
        // Store code in database
        await _verificationRepository.SaveCodeAsync(phone.Value, code);

        // Send SMS
        var message = $"Your verification code is: {code}";
        await _smsProvider.SendAsync(phone.Value, message);

        return SendResult.Success();
    }
}
```

### Contact Form API

```csharp
[ApiController]
[Route("api/contact")]
public class ContactFormController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SubmitContactForm([FromBody] ContactFormRequest request)
    {
        // Validate phone number
        var (phoneResult, phone) = PhoneNumber.TryCreate(request.Country, request.Phone);
        if (!phoneResult.IsValid)
        {
            return BadRequest(new
            {
                Field = "Phone",
                Errors = phoneResult.Errors.Select(e => e.Message)
            });
        }

        // Process submission
        var submission = new ContactSubmission
        {
            Id = Guid.NewGuid(),
            Phone = phone,  // Type-safe
            Message = request.Message,
            SubmittedAt = DateTime.UtcNow
        };

        await _contactService.SaveSubmissionAsync(submission);

        return Ok(new { Message = "Thank you for contacting us!" });
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, userInput);

if (result.IsValid)
{
    // Use the validated phone number
    await SendSms(phone);
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
public class Contact
{
    public Guid Id { get; set; }
    public PhoneNumber Phone { get; set; }
    public string Name { get; set; }
}

// Creating a contact
var (phoneResult, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, input);
if (phoneResult.IsValid)
{
    var contact = new Contact
    {
        Id = Guid.NewGuid(),
        Phone = phone,  // Type-safe and validated
        Name = name
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "+1-555-123-4567");

// Serialize
string json = JsonSerializer.Serialize(phone);
// {"Value":"+1-555-123-4567","CountryCode":"UnitedStates"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<PhoneNumber>(json);
Console.WriteLine(deserialized.Value);       // "+1-555-123-4567"
Console.WriteLine(deserialized.CountryCode); // UnitedStates
```

---

## ?? Related Documentation

- [ContactInformation README](contactinformation_readme.md) - Complete contact details including phone
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated phone number string |
| `CountryCode` | `CountryCode` | The country code for the phone number |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(CountryCode countryCode, string value, string propertyName = "PhoneNumber")` | `(ValidationResult, PhoneNumber?)` | Static factory method to create validated phone number |
| `GetCountryName()` | `string` | Returns display-friendly country name |
| `ToString()` | `string` | Returns the phone number value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Phone number cannot be null, empty, or whitespace only |
| **Valid Format** | Must conform to valid phone number format |
| **Length** | Must be between 7 and 20 characters |
| **Country Format** | Must conform to country-specific format (when country is specified) |

---

## ?? Supported Countries

The `PhoneNumber` type supports country-specific validation for 30+ countries:

### North America
- **United States** - +1 format
- **Canada** - +1 format
- **Mexico** - +52 format

### Europe
- **United Kingdom** - +44 format
- **Germany** - +49 format
- **France** - +33 format
- **Italy** - +39 format
- **Spain** - +34 format
- **Netherlands** - +31 format
- **Belgium** - +32 format
- **Switzerland** - +41 format
- **Austria** - +43 format
- **Sweden** - +46 format
- **Norway** - +47 format
- **Denmark** - +45 format
- **Finland** - +358 format
- **Poland** - +48 format
- **Czech Republic** - +420 format
- **Hungary** - +36 format
- **Portugal** - +351 format
- **Ireland** - +353 format

### Asia-Pacific
- **Japan** - +81 format
- **South Korea** - +82 format
- **Australia** - +61 format
- **New Zealand** - +64 format
- **China** - +86 format
- **India** - +91 format

### Other Regions
- **Brazil** - +55 format
- **South Africa** - +27 format

---

## ??? Security Considerations

### Phone Number Validation

```csharp
// ? DO: Validate before use
var (result, phone) = PhoneNumber.TryCreate(CountryCode.UnitedStates, userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid phone number");
}

// ? DON'T: Trust user input without validation
var phone = userInput;  // Dangerous!
await SendSms(phone);
```

### Rate Limiting for SMS

```csharp
// ? DO: Implement rate limiting for SMS sending
public async Task<SendResult> SendSms(PhoneNumber phone, string message)
{
    var rateLimitKey = $"sms:{phone.Value}";
    var count = await _cache.GetAsync<int>(rateLimitKey);
    
    if (count >= 5)  // Max 5 SMS per hour
    {
        return SendResult.RateLimited();
    }
    
    await _smsProvider.SendAsync(phone.Value, message);
    await _cache.SetAsync(rateLimitKey, count + 1, TimeSpan.FromHours(1));
    
    return SendResult.Success();
}
```

### Preventing Phone Number Enumeration

```csharp
// ? DO: Use same response for existing and non-existing phone numbers
public async Task<Result> RequestPhoneVerification(string phoneInput, CountryCode country)
{
    var (result, phone) = PhoneNumber.TryCreate(country, phoneInput);
    if (!result.IsValid)
    {
        // Generic message
        return Result.Success("If the phone number is registered, a code has been sent");
    }

    var user = await _userRepository.FindByPhoneAsync(phone.Value);
    if (user != null)
    {
        await SendVerificationCode(phone.Value);
    }

    // Same response whether user exists or not
    return Result.Success("If the phone number is registered, a code has been sent");
}
```
