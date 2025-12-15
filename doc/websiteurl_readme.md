# Validated.Primitives.WebsiteUrl

A validated website URL primitive that enforces HTTP/HTTPS protocol validation, ensuring website URLs are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`WebsiteUrl` is a validated value object that represents a website URL with HTTP/HTTPS protocol validation. It ensures URLs are properly formatted and use valid protocols. Once created, a `WebsiteUrl` instance is guaranteed to be valid.

### Key Features

- **HTTP/HTTPS Validation** - Enforces valid web protocols
- **URL Format Validation** - Validates proper URL structure
- **Domain Validation** - Ensures valid domain names
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a Website URL

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, url) = WebsiteUrl.TryCreate("https://www.example.com");

if (result.IsValid)
{
    Console.WriteLine(url.Value);      // "https://www.example.com"
    Console.WriteLine(url.ToString()); // "https://www.example.com"
    
    // Use the validated URL
    await FetchWebsiteContent(url);
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
public class Company
{
    public Guid Id { get; set; }
    public WebsiteUrl Website { get; set; }  // Always valid or null
    public string Name { get; set; }
}

// Usage
var (result, url) = WebsiteUrl.TryCreate(userInput);
if (result.IsValid)
{
    var company = new Company
    {
        Id = Guid.NewGuid(),
        Website = url,  // Guaranteed valid
        Name = companyName
    };
    
    await _companyRepository.SaveAsync(company);
}
```

---

## ? Valid Website URLs

### Simple URLs

```csharp
// Basic HTTPS URLs
var (r1, u1) = WebsiteUrl.TryCreate("https://example.com");             // ? HTTPS
var (r2, u2) = WebsiteUrl.TryCreate("http://example.com");              // ? HTTP
var (r3, u3) = WebsiteUrl.TryCreate("https://www.example.com");         // ? With www

foreach (var (result, url) in new[] { (r1, u1), (r2, u2), (r3, u3) })
{
    Console.WriteLine($"{url.Value}: {result.IsValid}");  // All true
}
```

### With Paths

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("https://example.com/page");        // ? With path
var (r2, u2) = WebsiteUrl.TryCreate("https://example.com/path/to/page"); // ? Multi-level path
var (r3, u3) = WebsiteUrl.TryCreate("https://example.com/about.html");   // ? File extension
var (r4, u4) = WebsiteUrl.TryCreate("https://example.com/blog/post/123"); // ? Complex path
```

### With Query Strings

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("https://example.com?id=123");      // ? Query string
var (r2, u2) = WebsiteUrl.TryCreate("https://example.com?id=1&name=test"); // ? Multiple params
var (r3, u3) = WebsiteUrl.TryCreate("https://example.com/search?q=url+validation"); // ? Encoded params
```

### With Fragments

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("https://example.com#section");     // ? Fragment
var (r2, u2) = WebsiteUrl.TryCreate("https://example.com/page#top");    // ? Path + fragment
var (r3, u3) = WebsiteUrl.TryCreate("https://example.com#contact-form"); // ? Named anchor
```

### Subdomains

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("https://blog.example.com");      // ? Subdomain
var (r2, u2) = WebsiteUrl.TryCreate("https://api.v2.example.com");    // ? Multiple subdomains
var (r3, u3) = WebsiteUrl.TryCreate("https://app.staging.example.com"); // ? Complex subdomains
```

### Ports

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("https://example.com:8080");      // ? Custom port
var (r2, u2) = WebsiteUrl.TryCreate("http://localhost:3000");         // ? Localhost with port
var (r3, u3) = WebsiteUrl.TryCreate("https://dev.example.com:8443");  // ? Subdomain + port
```

---

## ? Invalid Website URLs

### Missing Protocol

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("www.example.com");                 // ? No protocol
var (r2, u2) = WebsiteUrl.TryCreate("example.com");                     // ? No protocol
var (r3, u3) = WebsiteUrl.TryCreate("//example.com");                   // ? Protocol-relative
```

### Invalid Protocol

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("ftp://example.com");               // ? FTP not supported
var (r2, u2) = WebsiteUrl.TryCreate("file:///path/to/file");            // ? File protocol
var (r3, u3) = WebsiteUrl.TryCreate("mailto:user@example.com");         // ? Mailto not supported
```

### Invalid Format

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("https://");                        // ? Incomplete
var (r2, u2) = WebsiteUrl.TryCreate("https://.com");                    // ? Invalid domain
var (r3, u3) = WebsiteUrl.TryCreate("https://example.");                // ? Domain ends with dot
var (r4, u4) = WebsiteUrl.TryCreate("https://exam ple.com");            // ? Space in domain
```

### Empty or Null

```csharp
var (r1, u1) = WebsiteUrl.TryCreate("");                                // ? Empty string
var (r2, u2) = WebsiteUrl.TryCreate(null);                              // ? Null value
var (r3, u3) = WebsiteUrl.TryCreate("   ");                             // ? Whitespace only
```

---

## ?? Real-World Examples

### Company Profile Management

```csharp
public class CompanyProfileService
{
    public async Task<UpdateResult> UpdateCompanyWebsite(string companyId, string websiteInput)
    {
        // Validate website URL
        var (result, website) = WebsiteUrl.TryCreate(websiteInput);
        if (!result.IsValid)
        {
            return UpdateResult.Failed(
                $"Invalid website URL: {result.ToBulletList()}"
            );
        }

        // Update company profile
        var company = await _companyRepository.GetByIdAsync(companyId);
        company.Website = website;
        company.UpdatedAt = DateTime.UtcNow;

        await _companyRepository.UpdateAsync(company);

        return UpdateResult.Success();
    }
}
```

### Link Validation Service

```csharp
public class LinkValidationService
{
    public async Task<ValidationResult> ValidateWebsiteLink(string urlInput)
    {
        // Validate URL format
        var (result, url) = WebsiteUrl.TryCreate(urlInput);
        if (!result.IsValid)
        {
            return ValidationResult.Failed("Invalid URL format");
        }

        // Check if website is accessible
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            var response = await client.GetAsync(url.Value);
            if (!response.IsSuccessStatusCode)
            {
                return ValidationResult.Failed($"Website returned status {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return ValidationResult.Failed($"Website is not accessible: {ex.Message}");
        }

        return ValidationResult.Success();
    }
}
```

### Social Media Profile Integration

```csharp
public class SocialMediaService
{
    public async Task<ProfileResult> UpdateSocialProfile(string userId, SocialProfileRequest request)
    {
        // Validate website URL if provided
        WebsiteUrl? website = null;
        if (!string.IsNullOrWhiteSpace(request.Website))
        {
            var (result, url) = WebsiteUrl.TryCreate(request.Website);
            if (!result.IsValid)
            {
                return ProfileResult.ValidationFailed("Website", result.Errors);
            }
            website = url;
        }

        // Update social profile
        var profile = await _profileRepository.GetByUserIdAsync(userId);
        profile.Website = website;
        profile.FacebookUrl = request.FacebookUrl;  // Could also validate
        profile.TwitterUrl = request.TwitterUrl;    // Could also validate
        profile.LinkedInUrl = request.LinkedInUrl;  // Could also validate

        await _profileRepository.UpdateAsync(profile);

        return ProfileResult.Success();
    }
}
```

### Content Management System

```csharp
public class ContentManagementService
{
    public async Task<PublishResult> PublishArticle(ArticleRequest request)
    {
        // Validate source URL if provided
        WebsiteUrl? sourceUrl = null;
        if (!string.IsNullOrWhiteSpace(request.SourceUrl))
        {
            var (result, url) = WebsiteUrl.TryCreate(request.SourceUrl);
            if (!result.IsValid)
            {
                return PublishResult.ValidationFailed("Source URL", result.Errors);
            }
            sourceUrl = url;
        }

        // Create article
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            SourceUrl = sourceUrl,  // Type-safe
            PublishedAt = DateTime.UtcNow
        };

        await _articleRepository.SaveAsync(article);

        return PublishResult.Success(article.Id);
    }
}
```

### API Endpoint Validation

```csharp
[ApiController]
[Route("api/webhooks")]
public class WebhookController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterWebhook([FromBody] WebhookRegistration request)
    {
        // Validate webhook URL
        var (result, url) = WebsiteUrl.TryCreate(request.Url);
        if (!result.IsValid)
        {
            return BadRequest(new
            {
                Field = "Url",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        // Register webhook
        var webhook = new Webhook
        {
            Id = Guid.NewGuid(),
            Url = url,  // Type-safe
            EventType = request.EventType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _webhookRepository.SaveAsync(webhook);

        return Ok(new { WebhookId = webhook.Id });
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, url) = WebsiteUrl.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated URL
    await FetchContent(url);
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
public class Website
{
    public Guid Id { get; set; }
    public WebsiteUrl Url { get; set; }
    public string Name { get; set; }
}

// Creating a website
var (urlResult, url) = WebsiteUrl.TryCreate(input);
if (urlResult.IsValid)
{
    var website = new Website
    {
        Id = Guid.NewGuid(),
        Url = url,  // Type-safe and validated
        Name = name
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, url) = WebsiteUrl.TryCreate("https://www.example.com");

// Serialize
string json = JsonSerializer.Serialize(url);
// {"Value":"https://www.example.com"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<WebsiteUrl>(json);
Console.WriteLine(deserialized.Value);  // "https://www.example.com"
```

---

## ?? Related Documentation

- [ContactInformation README](contactinformation_readme.md) - Complete contact details including website
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated website URL string |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, string propertyName = "Url")` | `(ValidationResult, WebsiteUrl?)` | Static factory method to create validated website URL |
| `ToString()` | `string` | Returns the website URL value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | Website URL cannot be null, empty, or whitespace only |
| **Web URL Format** | Must be a valid HTTP or HTTPS URL |

---

## ?? URL Format Standards

### Supported Protocols

- **HTTP** - `http://example.com`
- **HTTPS** - `https://example.com` (recommended)

### URL Components

#### Scheme
- Must be `http://` or `https://`
- Case-insensitive

#### Domain
- Valid domain name format
- Supports subdomains
- Supports international domain names (IDN)
- Cannot start or end with hyphens
- Cannot contain spaces

#### Port (Optional)
- Valid port number (1-65535)
- Default ports: 80 (HTTP), 443 (HTTPS)

#### Path (Optional)
- Can contain letters, numbers, and special characters
- Supports nested paths
- Supports file extensions

#### Query String (Optional)
- Key-value pairs separated by `&`
- URL encoding supported
- Special characters allowed

#### Fragment (Optional)
- Anchor links within pages
- Used for navigation to specific sections

### Examples of Valid URL Structures

```
https://example.com
https://www.example.com
https://subdomain.example.com
https://example.com:8080
https://example.com/path
https://example.com/path/to/resource
https://example.com?id=123
https://example.com?id=1&name=test
https://example.com#section
https://example.com/path#anchor
```

---

## ??? Security Considerations

### URL Validation

```csharp
// ? DO: Validate before use
var (result, url) = WebsiteUrl.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid website URL");
}

// ? DON'T: Trust user input without validation
var url = userInput;  // Dangerous!
await FetchContent(url);
```

### HTTPS Enforcement

```csharp
// ? DO: Prefer HTTPS URLs
var (result, url) = WebsiteUrl.TryCreate(userInput);
if (result.IsValid && !url.Value.StartsWith("https://"))
{
    // Warn user about insecure HTTP
    _logger.LogWarning("HTTP URL provided instead of HTTPS: {Url}", url.Value);
}

// ? DO: Redirect HTTP to HTTPS when possible
public async Task<string> GetSecureUrl(string urlInput)
{
    var (result, url) = WebsiteUrl.TryCreate(urlInput);
    if (!result.IsValid) return null;
    
    if (url.Value.StartsWith("http://"))
    {
        return url.Value.Replace("http://", "https://");
    }
    
    return url.Value;
}
```

### Preventing SSRF Attacks

```csharp
// ? DO: Validate and whitelist domains
public async Task<FetchResult> FetchWebsiteContent(WebsiteUrl url)
{
    // Check against whitelist
    var allowedDomains = new[] { "example.com", "trusted-site.org" };
    var uri = new Uri(url.Value);
    
    if (!allowedDomains.Contains(uri.Host))
    {
        return FetchResult.Forbidden("Domain not in whitelist");
    }
    
    // Only allow HTTP/HTTPS
    if (uri.Scheme != "http" && uri.Scheme != "https")
    {
        return FetchResult.Forbidden("Only HTTP/HTTPS allowed");
    }
    
    // Fetch content safely
    return await _httpClient.GetStringAsync(url.Value);
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize before validation
public string SanitizeUrlInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Remove leading/trailing whitespace
    input = input.Trim();
    
    // Add protocol if missing
    if (!input.StartsWith("http://") && !input.StartsWith("https://"))
    {
        input = "https://" + input;
    }
    
    return input;
}

// Usage
var sanitized = SanitizeUrlInput(userInput);
var (result, url) = WebsiteUrl.TryCreate(sanitized);
```

### Rate Limiting for URL Access

```csharp
// ? DO: Implement rate limiting for URL fetching
public async Task<FetchResult> FetchWithRateLimit(WebsiteUrl url)
{
    var rateLimitKey = $"url_fetch:{url.Value}";
    var count = await _cache.GetAsync<int>(rateLimitKey);
    
    if (count >= 10)  // Max 10 fetches per hour per URL
    {
        return FetchResult.RateLimited();
    }
    
    var content = await _httpClient.GetStringAsync(url.Value);
    await _cache.SetAsync(rateLimitKey, count + 1, TimeSpan.FromHours(1));
    
    return FetchResult.Success(content);
}
```
