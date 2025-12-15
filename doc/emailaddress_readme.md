# Validated.Primitives.EmailAddress

A validated email address primitive that enforces RFC 5322 compliance and maximum length restrictions, ensuring email addresses are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`EmailAddress` is a validated value object that represents an email address. It ensures RFC 5322 compliance and enforces a maximum length of 256 characters. Once created, an `EmailAddress` instance is guaranteed to be valid.

### Key Features

- **RFC 5322 Compliant** - Validates email format according to the standard
- **Maximum Length** - Enforces 256 character limit
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating an Email Address

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, email) = EmailAddress.TryCreate("john.doe@example.com");

if (result.IsValid)
{
    Console.WriteLine(email.Value);      // "john.doe@example.com"
    Console.WriteLine(email.ToString()); // "john.doe@example.com"
    
    // Use the validated email
    await SendWelcomeEmail(email);
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
public class User
{
    public Guid Id { get; set; }
    public EmailAddress Email { get; set; }  // Always valid or null
    public string Name { get; set; }
}

// Usage
var (result, email) = EmailAddress.TryCreate(userInput);
if (result.IsValid)
{
    var user = new User
    {
        Id = Guid.NewGuid(),
        Email = email,  // Guaranteed valid
        Name = userName
    };
    
    await _userRepository.SaveAsync(user);
}
```


### User Registration


### Contact Form API

```csharp
[ApiController]
[Route("api/contact")]
public class ContactFormController : ControllerBase
{
    private readonly IContactService _contactService;

    [HttpPost]
    public async Task<IActionResult> SubmitContactForm([FromBody] ContactFormRequest request)
    {
        // Validate email address
        var (emailResult, email) = EmailAddress.TryCreate(request.Email);
        if (!emailResult.IsValid)
        {
            return BadRequest(new
            {
                Field = "Email",
                Errors = emailResult.Errors.Select(e => e.Message)
            });
        }

        // Create contact submission
        var submission = new ContactSubmission
        {
            Id = Guid.NewGuid(),
            Email = email,  // Type-safe
            Name = request.Name,
            Subject = request.Subject,
            Message = request.Message,
            SubmittedAt = DateTime.UtcNow
        };

        await _contactService.SaveSubmissionAsync(submission);

        // Send notifications
        await _contactService.NotifyAdminAsync(submission);
        await _contactService.SendConfirmationAsync(email.Value);

        return Ok(new { Message = "Thank you for contacting us!" });
    }
}

public class ContactFormRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}
```

### Email Validation Middleware

```csharp
public class EmailValidationMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if request contains email in form data
        if (context.Request.HasFormContentType && 
            context.Request.Form.ContainsKey("email"))
        {
            var emailInput = context.Request.Form["email"].ToString();
            var (result, email) = EmailAddress.TryCreate(emailInput);

            if (!result.IsValid)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Error = "Invalid email address",
                    Details = result.Errors.Select(e => e.Message)
                };

                await context.Response.WriteAsJsonAsync(response);
                return;
            }

            // Store validated email in HttpContext for downstream use
            context.Items["ValidatedEmail"] = email;
        }

        await _next(context);
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, email) = EmailAddress.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated email
    await ProcessEmail(email);
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


### JSON Serialization

```csharp
using System.Text.Json;

var (_, email) = EmailAddress.TryCreate("john@example.com");

// Serialize
string json = JsonSerializer.Serialize(email);
// {"Value":"john@example.com"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<EmailAddress>(json);
Console.WriteLine(deserialized.Value);  // "john@example.com"
```

---

## ?? Related Documentation

- [ContactInformation README](contactinformation_readme.md) - Complete contact details including email
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated email address string |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, string propertyName = "Email")` | `(ValidationResult, EmailAddress?)` | Static factory method to create validated email |
| `ToString()` | `string` | Returns the email address value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Email address cannot be null, empty, or whitespace only |
| **RFC 5322 Format** | Must conform to RFC 5322 email format specification |
| **Maximum Length** | Cannot exceed 256 characters |

---

## ?? Email Format Standards

### RFC 5322 Compliance

The `EmailAddress` type validates according to **RFC 5322** standards:

#### Supported Features
- **Local Part**: Letters, numbers, and special characters (`.`, `_`, `+`, `-`)
- **Domain Part**: Valid domain names with TLDs
- **Maximum Length**: 256 characters total
- **Plus Addressing**: `user+tag@example.com`
- **Subdomains**: `user@mail.example.com`
- **International TLDs**: `.com`, `.org`, `.co.uk`, etc.
- **Numbers**: Both in local and domain parts

#### Not Supported
- **IP Address Domains**: `user@[192.168.1.1]`
- **Comments**: Comments in email addresses
- **Quoted Strings**: Quoted strings in local part
- **Non-ASCII Characters**: Internationalized email addresses (IDN)

### Character Restrictions

#### Allowed in Local Part (before @)
- Letters: `a-z`, `A-Z`
- Numbers: `0-9`
- Special characters: `.` (dot), `_` (underscore), `+` (plus), `-` (hyphen)

#### Allowed in Domain Part (after @)
- Letters: `a-z`, `A-Z`
- Numbers: `0-9`
- Special characters: `.` (dot), `-` (hyphen)

### Length Limits

- **Total Length**: Maximum 256 characters
- **Local Part**: No specific limit (but contributes to total)
- **Domain Part**: No specific limit (but contributes to total)

---

## ??? Security Considerations

### Email Address Validation

```csharp
// ? DO: Validate before use
var (result, email) = EmailAddress.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid email");
}

// ? DON'T: Trust user input without validation
var email = userInput;  // Dangerous!
await SendEmail(email);
```

### SQL Injection Prevention

```csharp
// ? DO: Use parameterized queries (EmailAddress helps but always use parameters)
var (_, email) = EmailAddress.TryCreate(userInput);
var user = await _dbContext.Users
    .FirstOrDefaultAsync(u => u.Email.Value == email.Value);

// ? DON'T: Concatenate SQL strings
var query = $"SELECT * FROM Users WHERE Email = '{userInput}'";  // Dangerous!
```

### Case Sensitivity

```csharp
// Email addresses are case-insensitive for domain, case-sensitive for local part
// However, most systems treat the entire email as case-insensitive

// For lookups, use case-insensitive comparison
var emailLower = email.Value.ToLowerInvariant();
var user = await _dbContext.Users
    .FirstOrDefaultAsync(u => u.Email.Value.ToLowerInvariant() == emailLower);
```

### Preventing Email Enumeration

```csharp
// ? DO: Use same response for existing and non-existing emails
public async Task<Result> RequestPasswordReset(string emailInput)
{
    var (result, email) = EmailAddress.TryCreate(emailInput);
    if (!result.IsValid)
    {
        // Generic message
        return Result.Success("If the email exists, a reset link has been sent");
    }

    var user = await _userRepository.FindByEmailAsync(email.Value);
    if (user != null)
    {
        await SendPasswordResetEmail(email.Value);
    }

    // Same response whether user exists or not
    return Result.Success("If the email exists, a reset link has been sent");
}
```
