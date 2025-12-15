# Validated.Primitives.ContactInformation

The ContactInformation namespace provides validated value objects and domain models for working with contact details including email addresses, phone numbers, and website URLs. All types include built-in validation, format checking, and country-specific phone number support.

## 📦 Package Information

The contact information types are split across two packages:

### Core Primitives (`Validated.Primitives`)
- `EmailAddress` - RFC 5322 compliant email address validation (max 256 characters)
- `PhoneNumber` - Country-specific phone number validation (30+ countries)
- `WebsiteUrl` - URL validation for websites (HTTP/HTTPS)

### Domain Models (`Validated.Primitives.Domain`)
- `ContactInformation` - Complete contact details with email, phone, and optional website

---

## 📧 Core Primitives

### EmailAddress

Represents a validated email address according to RFC 5322 standard with maximum length restriction.

#### Key Features
- RFC 5322 compliant format validation
- Maximum length: 256 characters
- Supports common email formats and special characters
- Validates local part and domain
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, email) = EmailAddress.TryCreate("john.doe@example.com");

if (result.IsValid)
{
    Console.WriteLine(email.Value);      // "john.doe@example.com"
    Console.WriteLine(email.ToString()); // "john.doe@example.com"
}
else
{
    Console.WriteLine(result.ToBulletList());
}
```

#### Valid Email Addresses

```csharp
// Simple addresses
var (r1, e1) = EmailAddress.TryCreate("user@example.com");              // ✓ Valid
var (r2, e2) = EmailAddress.TryCreate("john.doe@example.com");          // ✓ Valid
var (r3, e3) = EmailAddress.TryCreate("test@subdomain.example.com");    // ✓ Valid

// With special characters
var (r4, e4) = EmailAddress.TryCreate("user+tag@example.com");          // ✓ Valid (plus addressing)
var (r5, e5) = EmailAddress.TryCreate("user_name@example.com");         // ✓ Valid (underscore)
var (r6, e6) = EmailAddress.TryCreate("first.last@example.com");        // ✓ Valid (dot)

// International domains
var (r7, e7) = EmailAddress.TryCreate("user@example.co.uk");            // ✓ Valid (country TLD)
var (r8, e8) = EmailAddress.TryCreate("test@example.org");              // ✓ Valid (.org)
var (r9, e9) = EmailAddress.TryCreate("admin@example.gov");             // ✓ Valid (.gov)

// Complex but valid
var (r10, e10) = EmailAddress.TryCreate("name.surname@sub.domain.example.com"); // ✓ Valid
var (r11, e11) = EmailAddress.TryCreate("user123@example123.com");      // ✓ Valid (numbers)
var (r12, e12) = EmailAddress.TryCreate("a@b.c");                       // ✓ Valid (minimal)
```

#### Invalid Email Addresses

```csharp
// Missing parts
var (r1, e1) = EmailAddress.TryCreate("@example.com");                  // ✗ Missing local part
var (r2, e2) = EmailAddress.TryCreate("user@");                         // ✗ Missing domain
var (r3, e3) = EmailAddress.TryCreate("userexample.com");               // ✗ Missing @

// Invalid format
var (r4, e4) = EmailAddress.TryCreate("user @example.com");             // ✗ Space in local part
var (r5, e5) = EmailAddress.TryCreate("user@example .com");             // ✗ Space in domain
var (r6, e6) = EmailAddress.TryCreate("user@@example.com");             // ✗ Double @

// Empty or null
var (r7, e7) = EmailAddress.TryCreate("");                              // ✗ Empty
var (r8, e8) = EmailAddress.TryCreate(null);                            // ✗ Null
var (r9, e9) = EmailAddress.TryCreate("   ");                           // ✗ Whitespace only

// Too long
var longEmail = new string('a', 250) + "@example.com";
var (r10, e10) = EmailAddress.TryCreate(longEmail);                     // ✗ Exceeds 256 characters
```

#### Email Format Examples

```csharp
// Different valid patterns
var emails = new[]
{
    "simple@example.com",                           // Simple format
    "name.surname@example.co.uk",                   // Name with dot, country TLD
    "user+tag@example.org",                         // Plus addressing
    "test_user@sub.domain.example.com",             // Underscore, subdomain
    "admin123@server-name.example.com",             // Numbers, hyphen in domain
    "sales@company.com",                            // Department email
    "support@help.example.com"                      // Support email
};

foreach (var emailStr in emails)
{
    var (result, email) = EmailAddress.TryCreate(emailStr);
    Console.WriteLine($"{emailStr}: {result.IsValid}");  // All true
}
```

---

### PhoneNumber

Represents a validated phone number with country-specific format validation. Supports 30+ countries with various international formats.

#### Key Features
- Country-specific validation for 30+ countries
- Format validation for each country
- Length validation (7-20 characters)
- Supports international formats (+1, +44, etc.)
- Country code extraction
- JSON serialization support

#### Supported Countries

United States, Canada, United Kingdom, Germany, France, Italy, Spain, Netherlands, Belgium, Switzerland, Austria, Sweden, Norway, Denmark, Finland, Poland, Czech Republic, Hungary, Portugal, Ireland, Australia, New Zealand, Japan, South Korea, Brazil, Mexico, India, China, South Africa, and more.

#### Basic Usage

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
}
```

#### Valid Phone Numbers by Country

```csharp
// United States
var (r1, p1) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "+1-555-123-4567");  // ✓ International format
var (r2, p2) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "(555) 123-4567");   // ✓ Parentheses format
var (r3, p3) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "555-123-4567");     // ✓ Domestic format

// United Kingdom
var (r4, p4) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "+44 20 7123 4567"); // ✓ London landline
var (r5, p5) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "+44 7700 900123"); // ✓ Mobile

// Germany
var (r6, p6) = PhoneNumber.TryCreate(CountryCode.Germany, "+49 30 12345678");       // ✓ Berlin
var (r7, p7) = PhoneNumber.TryCreate(CountryCode.Germany, "+49 89 12345678");       // ✓ Munich

// France
var (r8, p8) = PhoneNumber.TryCreate(CountryCode.France, "+33 1 42 34 56 78");      // ✓ Paris
var (r9, p9) = PhoneNumber.TryCreate(CountryCode.France, "+33 6 12 34 56 78");      // ✓ Mobile

// Japan
var (r10, p10) = PhoneNumber.TryCreate(CountryCode.Japan, "+81 3 1234 5678");       // ✓ Tokyo
var (r11, p11) = PhoneNumber.TryCreate(CountryCode.Japan, "+81 90 1234 5678");      // ✓ Mobile

// Australia
var (r12, p12) = PhoneNumber.TryCreate(CountryCode.Australia, "+61 2 1234 5678");   // ✓ Sydney
var (r13, p13) = PhoneNumber.TryCreate(CountryCode.Australia, "+61 4 1234 5678");   // ✓ Mobile

// Canada (same format as US)
var (r14, p14) = PhoneNumber.TryCreate(CountryCode.Canada, "+1-416-555-1234");      // ✓ Toronto
```

#### Invalid Phone Numbers

```csharp
// Too short
var (r1, p1) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "123");      // ✗ Too short

// Too long
var longPhone = new string('1', 25);
var (r2, p2) = PhoneNumber.TryCreate(CountryCode.UnitedStates, longPhone);  // ✗ Too long

// Invalid format
var (r3, p3) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "abc-def-ghij"); // ✗ Letters

// Empty or null
var (r4, p4) = PhoneNumber.TryCreate(CountryCode.UnitedStates, "");         // ✗ Empty
var (r5, p5) = PhoneNumber.TryCreate(CountryCode.UnitedStates, null);       // ✗ Null
```

#### Phone Number Formats

```csharp
// US phone number formats (all valid)
var usFormats = new[]
{
    "+1-555-123-4567",      // International with dashes
    "+1 (555) 123-4567",    // International with parentheses
    "(555) 123-4567",       // Domestic with parentheses
    "555-123-4567",         // Domestic with dashes
    "555.123.4567",         // Domestic with dots
    "5551234567"            // Digits only
};

foreach (var format in usFormats)
{
    var (result, phone) = PhoneNumber.TryCreate(CountryCode.All, format);
    Console.WriteLine($"{format}: {result.IsValid}");
}
```

#### Country Name Display

```csharp
var (_, phone) = PhoneNumber.TryCreate(CountryCode.UnitedKingdom, "+44 20 7123 4567");

Console.WriteLine(phone.CountryCode);      // UnitedKingdom
Console.WriteLine(phone.GetCountryName()); // "United Kingdom"
```

---

### WebsiteUrl

Represents a validated website URL with HTTP/HTTPS protocol validation.

#### Key Features
- HTTP/HTTPS protocol validation
- Valid URL format checking
- Domain validation
- Supports query strings and fragments
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, url) = WebsiteUrl.TryCreate("https://www.example.com");

if (result.IsValid)
{
    Console.WriteLine(url.Value);      // "https://www.example.com"
    Console.WriteLine(url.ToString()); // "https://www.example.com"
}
```

#### Valid Website URLs

```csharp
// Simple URLs
var (r1, u1) = WebsiteUrl.TryCreate("https://example.com");             // ✓ HTTPS
var (r2, u2) = WebsiteUrl.TryCreate("http://example.com");              // ✓ HTTP
var (r3, u3) = WebsiteUrl.TryCreate("https://www.example.com");         // ✓ With www

// With paths
var (r4, u4) = WebsiteUrl.TryCreate("https://example.com/page");        // ✓ With path
var (r5, u5) = WebsiteUrl.TryCreate("https://example.com/path/to/page"); // ✓ Multi-level path

// With query strings
var (r6, u6) = WebsiteUrl.TryCreate("https://example.com?id=123");      // ✓ Query string
var (r7, u7) = WebsiteUrl.TryCreate("https://example.com?id=1&name=test"); // ✓ Multiple params

// With fragments
var (r8, u8) = WebsiteUrl.TryCreate("https://example.com#section");     // ✓ Fragment
var (r9, u9) = WebsiteUrl.TryCreate("https://example.com/page#top");    // ✓ Path + fragment

// Subdomains
var (r10, u10) = WebsiteUrl.TryCreate("https://blog.example.com");      // ✓ Subdomain
var (r11, u11) = WebsiteUrl.TryCreate("https://api.v2.example.com");    // ✓ Multiple subdomains

// Ports
var (r12, u12) = WebsiteUrl.TryCreate("https://example.com:8080");      // ✓ Custom port
var (r13, u13) = WebsiteUrl.TryCreate("http://localhost:3000");         // ✓ Localhost with port
```

#### Invalid Website URLs

```csharp
// Missing protocol
var (r1, u1) = WebsiteUrl.TryCreate("www.example.com");                 // ✗ No protocol
var (r2, u2) = WebsiteUrl.TryCreate("example.com");                     // ✗ No protocol

// Invalid protocol
var (r3, u3) = WebsiteUrl.TryCreate("ftp://example.com");               // ✗ FTP not supported
var (r4, u4) = WebsiteUrl.TryCreate("file:///path/to/file");            // ✗ File protocol

// Invalid format
var (r5, u5) = WebsiteUrl.TryCreate("https://");                        // ✗ Incomplete
var (r6, u6) = WebsiteUrl.TryCreate("not-a-url");                       // ✗ Invalid format

// Empty or null
var (r7, u7) = WebsiteUrl.TryCreate("");                                // ✗ Empty
var (r8, u8) = WebsiteUrl.TryCreate(null);                              // ✗ Null
```

---

## 📞 Domain Model

### ContactInformation

Represents complete contact information including email, primary phone, and optional secondary phone and website.

#### Key Features
- Validates all contact components
- Required: Email and primary phone
- Optional: Secondary phone and website
- Country-specific phone validation
- Formatted display
- Comprehensive validation with detailed error messages
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, contact) = ContactInformation.TryCreate(
    countryCode: CountryCode.UnitedStates,
    email: "john.doe@example.com",
    primaryPhone: "+1-555-123-4567"
);

if (result.IsValid)
{
    Console.WriteLine(contact.Email.Value);          // "john.doe@example.com"
    Console.WriteLine(contact.PrimaryPhone.Value);   // "+1-555-123-4567"
    Console.WriteLine(contact.SecondaryPhone);       // null
    Console.WriteLine(contact.Website);              // null
    Console.WriteLine(contact.ToString());          
    // "Email: john.doe@example.com | Phone: +1-555-123-4567"
}
```

#### Complete Contact Information

```csharp
var (result, contact) = ContactInformation.TryCreate(
    countryCode: CountryCode.UnitedStates,
    email: "jane.smith@example.com",
    primaryPhone: "+1-555-111-2222",
    secondaryPhone: "+1-555-333-4444",
    website: "https://janesmith.com"
);

if (result.IsValid)
{
    Console.WriteLine(contact.Email.Value);              // "jane.smith@example.com"
    Console.WriteLine(contact.PrimaryPhone.Value);       // "+1-555-111-2222"
    Console.WriteLine(contact.SecondaryPhone.Value);     // "+1-555-333-4444"
    Console.WriteLine(contact.Website.Value);            // "https://janesmith.com"
    
    Console.WriteLine(contact.ToString());
    // "Email: jane.smith@example.com | Phone: +1-555-111-2222 | Secondary Phone: +1-555-333-4444 | Website: https://janesmith.com"
}
```

#### Validation Examples

```csharp
// ✓ Valid - Minimal contact (email + phone only)
var (r1, c1) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "john@example.com",
    "+1-555-123-4567"
);
// r1.IsValid == true

// ✓ Valid - With secondary phone
var (r2, c2) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "jane@example.com",
    "+1-555-111-2222",
    "+1-555-333-4444"
);
// r2.IsValid == true

// ✓ Valid - With website
var (r3, c3) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "test@example.com",
    "+1-555-999-8888",
    website: "https://example.com"
);
// r3.IsValid == true

// ✓ Valid - With all fields
var (r4, c4) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "admin@example.com",
    "+1-555-777-6666",
    "+1-555-555-5555",
    "https://admin.example.com"
);
// r4.IsValid == true

// ✗ Invalid - Invalid email
var (r5, c5) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "invalid-email",
    "+1-555-123-4567"
);
// r5.IsValid == false, error on Email

// ✗ Invalid - Invalid primary phone
var (r6, c6) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "john@example.com",
    "invalid-phone"
);
// r6.IsValid == false, error on PrimaryPhone

// ✗ Invalid - Invalid secondary phone
var (r7, c7) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "john@example.com",
    "+1-555-123-4567",
    "invalid-phone"
);
// r7.IsValid == false, error on SecondaryPhone

// ✗ Invalid - Invalid website
var (r8, c8) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "john@example.com",
    "+1-555-123-4567",
    website: "not-a-url"
);
// r8.IsValid == false, error on Website

// ✗ Invalid - Multiple errors
var (r9, c9) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "invalid-email",
    "invalid-phone",
    "invalid-secondary",
    "invalid-url"
);
// r9.IsValid == false, multiple errors
```

#### Optional Fields Handling

```csharp
// Empty strings are treated as null
var (r1, c1) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "test@example.com",
    "+1-555-123-4567",
    "",  // Empty string → null
    ""   // Empty string → null
);
// c1.SecondaryPhone == null
// c1.Website == null

// Whitespace strings are treated as null
var (r2, c2) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "test@example.com",
    "+1-555-123-4567",
    "   ",  // Whitespace → null
    "   "   // Whitespace → null
);
// c2.SecondaryPhone == null
// c2.Website == null
```

#### International Contact Information

```csharp
// UK contact
var (r1, ukContact) = ContactInformation.TryCreate(
    CountryCode.UnitedKingdom,
    "contact@example.co.uk",
    "+44 20 7123 4567",
    "+44 7700 900123",
    "https://www.example.co.uk"
);

// German contact
var (r2, deContact) = ContactInformation.TryCreate(
    CountryCode.Germany,
    "info@beispiel.de",
    "+49 30 12345678",
    website: "https://www.beispiel.de"
);

// Japanese contact
var (r3, jpContact) = ContactInformation.TryCreate(
    CountryCode.Japan,
    "support@example.jp",
    "+81 3 1234 5678",
    "+81 90 1234 5678",
    "https://www.example.jp"
);
```

#### Properties and Methods

```csharp
var (_, contact) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "admin@example.com",
    "+1-555-111-2222",
    "+1-555-333-4444",
    "https://admin.example.com"
);

// Access components
Console.WriteLine(contact.Email.Value);              // "admin@example.com"
Console.WriteLine(contact.PrimaryPhone.Value);       // "+1-555-111-2222"
Console.WriteLine(contact.PrimaryPhone.CountryCode); // UnitedStates
Console.WriteLine(contact.SecondaryPhone.Value);     // "+1-555-333-4444"
Console.WriteLine(contact.Website.Value);            // "https://admin.example.com"

// Formatted output
Console.WriteLine(contact.ToString());
// "Email: admin@example.com | Phone: +1-555-111-2222 | Secondary Phone: +1-555-333-4444 | Website: https://admin.example.com"
```

#### ToString() Formatting

```csharp
// Minimal (email + phone only)
var (_, c1) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "john@example.com",
    "+1-555-123-4567"
);
Console.WriteLine(c1.ToString());
// "Email: john@example.com | Phone: +1-555-123-4567"

// With secondary phone
var (_, c2) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "jane@example.com",
    "+1-555-111-2222",
    "+1-555-333-4444"
);
Console.WriteLine(c2.ToString());
// "Email: jane@example.com | Phone: +1-555-111-2222 | Secondary Phone: +1-555-333-4444"

// With website
var (_, c3) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "test@example.com",
    "+1-555-999-8888",
    website: "https://example.com"
);
Console.WriteLine(c3.ToString());
// "Email: test@example.com | Phone: +1-555-999-8888 | Website: https://example.com"

// With all fields
var (_, c4) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "admin@example.com",
    "+1-555-777-6666",
    "+1-555-555-5555",
    "https://admin.example.com"
);
Console.WriteLine(c4.ToString());
// "Email: admin@example.com | Phone: +1-555-777-6666 | Secondary Phone: +1-555-555-5555 | Website: https://admin.example.com"
```

---

## 📋 Common Patterns

### Validation Pattern

All contact information types follow the same validation pattern:

```csharp
var (result, value) = Type.TryCreate(...);

if (result.IsValid)
{
    // Use the validated value
    SaveContact(value);
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

All types support JSON serialization:

```csharp
using System.Text.Json;

var (_, contact) = ContactInformation.TryCreate(
    CountryCode.UnitedStates,
    "john@example.com",
    "+1-555-123-4567"
);

// Serialize
string json = JsonSerializer.Serialize(contact);

// Deserialize
var deserialized = JsonSerializer.Deserialize<ContactInformation>(json);
```

---

## 🎯 Real-World Examples

### User Registration

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

public class UserRegistrationService
{
    public async Task<RegistrationResult> RegisterUser(RegistrationRequest request)
    {
        // Validate contact information
        var (contactResult, contact) = ContactInformation.TryCreate(
            request.Country,
            request.Email,
            request.Phone,
            request.SecondaryPhone,
            request.Website
        );

        if (!contactResult.IsValid)
        {
            return RegistrationResult.ValidationFailed(contactResult.Errors);
        }

        // Check if email already exists
        var emailExists = await _userRepository.EmailExistsAsync(contact.Email.Value);
        if (emailExists)
        {
            return RegistrationResult.Failed("Email address is already registered");
        }

        // Create user account
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = contact.Email.Value,
            PrimaryPhone = contact.PrimaryPhone.Value,
            SecondaryPhone = contact.SecondaryPhone?.Value,
            Website = contact.Website?.Value,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        // Send verification email
        await _emailService.SendVerificationEmailAsync(contact.Email.Value, user.Id);

        return RegistrationResult.Success(user.Id);
    }
}
```

### Contact Form Validation

```csharp
[ApiController]
[Route("api/contact")]
public class ContactFormController : ControllerBase
{
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitContactForm([FromBody] ContactFormRequest request)
    {
        // Validate contact information
        var (result, contact) = ContactInformation.TryCreate(
            request.Country,
            request.Email,
            request.Phone
        );

        if (!result.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Errors = result.Errors.Select(e => new
                {
                    Field = e.MemberName,
                    Message = e.Message,
                    Code = e.Code
                })
            });
        }

        // Process contact form
        var submission = new ContactFormSubmission
        {
            Email = contact.Email.Value,
            Phone = contact.PrimaryPhone.Value,
            Message = request.Message,
            SubmittedAt = DateTime.UtcNow
        };

        await _contactService.SaveSubmissionAsync(submission);

        // Send notification to admin
        await _notificationService.NotifyAdminAsync(submission);

        // Send confirmation to user
        await _emailService.SendConfirmationAsync(contact.Email.Value);

        return Ok(new
        {
            Success = true,
            Message = "Thank you for contacting us. We'll get back to you soon!"
        });
    }
}
```

### Customer Support System

```csharp
public class CustomerSupportService
{
    public async Task<Ticket> CreateSupportTicket(TicketRequest request)
    {
        // Validate customer contact information
        var (contactResult, contact) = ContactInformation.TryCreate(
            request.Country,
            request.Email,
            request.Phone,
            request.AlternatePhone
        );

        if (!contactResult.IsValid)
        {
            throw new ValidationException(
                $"Invalid contact information: {contactResult.ToBulletList()}"
            );
        }

        // Check if customer exists
        var customer = await _customerRepository.FindByEmailAsync(contact.Email.Value);
        
        if (customer == null)
        {
            // Create new customer profile
            customer = new Customer
            {
                Id = Guid.NewGuid(),
                Email = contact.Email.Value,
                PrimaryPhone = contact.PrimaryPhone.Value,
                SecondaryPhone = contact.SecondaryPhone?.Value,
                CreatedAt = DateTime.UtcNow
            };

            await _customerRepository.CreateAsync(customer);
        }

        // Create support ticket
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Subject = request.Subject,
            Description = request.Description,
            Priority = DeterminePriority(request),
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepository.CreateAsync(ticket);

        // Send confirmation email
        await _emailService.SendTicketConfirmationAsync(
            contact.Email.Value,
            ticket.Id
        );

        // Send SMS notification if phone provided
        if (contact.PrimaryPhone != null)
        {
            await _smsService.SendTicketCreatedNotificationAsync(
                contact.PrimaryPhone.Value,
                ticket.Id
            );
        }

        return ticket;
    }
}
```

### Newsletter Subscription

```csharp
public class NewsletterService
{
    public async Task<SubscriptionResult> Subscribe(string email, string phone, CountryCode country)
    {
        // Validate email
        var (emailResult, emailAddress) = EmailAddress.TryCreate(email);
        if (!emailResult.IsValid)
        {
            return SubscriptionResult.ValidationFailed("Invalid email address");
        }

        // Validate phone (optional)
        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(phone))
        {
            var (phoneResult, phoneValue) = PhoneNumber.TryCreate(country, phone);
            if (!phoneResult.IsValid)
            {
                return SubscriptionResult.ValidationFailed("Invalid phone number");
            }
            phoneNumber = phoneValue;
        }

        // Check if already subscribed
        var existing = await _subscriberRepository.FindByEmailAsync(emailAddress.Value);
        if (existing != null)
        {
            if (existing.IsActive)
            {
                return SubscriptionResult.AlreadySubscribed();
            }
            
            // Reactivate subscription
            existing.IsActive = true;
            existing.ReactivatedAt = DateTime.UtcNow;
            await _subscriberRepository.UpdateAsync(existing);
            
            return SubscriptionResult.Reactivated();
        }

        // Create new subscription
        var subscriber = new Subscriber
        {
            Id = Guid.NewGuid(),
            Email = emailAddress.Value,
            Phone = phoneNumber?.Value,
            Country = country,
            IsActive = true,
            SubscribedAt = DateTime.UtcNow,
            VerificationToken = GenerateVerificationToken()
        };

        await _subscriberRepository.CreateAsync(subscriber);

        // Send verification email
        await _emailService.SendVerificationEmailAsync(
            emailAddress.Value,
            subscriber.VerificationToken
        );

        return SubscriptionResult.Success(subscriber.Id);
    }
}
```

### Business Directory Listing

```csharp
public class BusinessDirectoryService
{
    public async Task<ListingResult> CreateListing(BusinessListingRequest request)
    {
        // Validate contact information
        var (contactResult, contact) = ContactInformation.TryCreate(
            request.Country,
            request.Email,
            request.Phone,
            request.SecondaryPhone,
            request.Website
        );

        if (!contactResult.IsValid)
        {
            return ListingResult.ValidationFailed(contactResult.Errors);
        }

        // Verify business ownership via email
        var verificationToken = GenerateVerificationToken();
        await _emailService.SendBusinessVerificationAsync(
            contact.Email.Value,
            verificationToken
        );

        // Create business listing
        var listing = new BusinessListing
        {
            Id = Guid.NewGuid(),
            BusinessName = request.BusinessName,
            Description = request.Description,
            Email = contact.Email.Value,
            Phone = contact.PrimaryPhone.Value,
            SecondaryPhone = contact.SecondaryPhone?.Value,
            Website = contact.Website?.Value,
            Country = request.Country,
            IsVerified = false,
            VerificationToken = verificationToken,
            CreatedAt = DateTime.UtcNow
        };

        await _listingRepository.CreateAsync(listing);

        return ListingResult.Success(listing.Id, "Verification email sent");
    }

    public async Task<UpdateResult> UpdateContactInfo(
        Guid listingId,
        string email,
        string phone,
        string? secondaryPhone,
        string? website,
        CountryCode country)
    {
        // Validate new contact information
        var (result, contact) = ContactInformation.TryCreate(
            country,
            email,
            phone,
            secondaryPhone,
            website
        );

        if (!result.IsValid)
        {
            return UpdateResult.ValidationFailed(result.Errors);
        }

        var listing = await _listingRepository.GetByIdAsync(listingId);
        if (listing == null)
        {
            return UpdateResult.NotFound();
        }

        // Update contact details
        listing.Email = contact.Email.Value;
        listing.Phone = contact.PrimaryPhone.Value;
        listing.SecondaryPhone = contact.SecondaryPhone?.Value;
        listing.Website = contact.Website?.Value;
        listing.UpdatedAt = DateTime.UtcNow;

        await _listingRepository.UpdateAsync(listing);

        return UpdateResult.Success();
    }
}
```

### Multi-Channel Communication

```csharp
public class CommunicationService
{
    public async Task<SendResult> SendNotification(
        ContactInformation contact,
        string message,
        NotificationChannel preferredChannel)
    {
        var results = new List<ChannelResult>();

        switch (preferredChannel)
        {
            case NotificationChannel.Email:
                var emailResult = await SendEmailAsync(contact.Email.Value, message);
                results.Add(new ChannelResult("Email", emailResult));
                break;

            case NotificationChannel.SMS:
                var smsResult = await SendSmsAsync(contact.PrimaryPhone.Value, message);
                results.Add(new ChannelResult("SMS", smsResult));
                break;

            case NotificationChannel.All:
                // Send via all available channels
                results.Add(new ChannelResult(
                    "Email",
                    await SendEmailAsync(contact.Email.Value, message)
                ));

                results.Add(new ChannelResult(
                    "SMS",
                    await SendSmsAsync(contact.PrimaryPhone.Value, message)
                ));

                if (contact.SecondaryPhone != null)
                {
                    results.Add(new ChannelResult(
                        "Secondary SMS",
                        await SendSmsAsync(contact.SecondaryPhone.Value, message)
                    ));
                }
                break;
        }

        var allSucceeded = results.All(r => r.Success);
        return new SendResult(allSucceeded, results);
    }
}
```

---

## 🔗 Related Documentation

- [Main README](../README.md) - Complete library overview
- [Address README](address_readme.md) - Physical address validation
- [Banking README](banking_readme.md) - Banking information validation

---

## 📚 API Reference

For complete API documentation, see:
- XML documentation comments in source code
- IntelliSense in Visual Studio
- [Validated.Primitives Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)
- [Validated.Primitives.Domain Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)

---

## 🌍 Supported Countries for Phone Numbers

The `PhoneNumber` type supports country-specific validation for 30+ countries:

### North America
- **United States** - +1 format, various domestic formats
- **Canada** - +1 format (same as US)
- **Mexico** - +52 format

### Europe
- **United Kingdom** - +44 format (landline and mobile)
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

## ✉️ Email Validation Standards

The `EmailAddress` type validates according to **RFC 5322** standards:

### Supported Features
- Local part: Letters, numbers, and special characters (., _, +, -)
- Domain part: Valid domain names with TLDs
- Maximum length: 256 characters
- Plus addressing: user+tag@example.com
- Subdomains: user@mail.example.com
- International TLDs: .com, .org, .co.uk, etc.

### Not Supported
- IP address domains: user@[192.168.1.1]
- Comments in addresses
- Quoted strings in local part
- Non-ASCII characters (internationalized email addresses)
