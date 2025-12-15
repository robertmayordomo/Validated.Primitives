# Validated.Primitives.IpAddress

A validated IP address primitive that enforces proper IPv4 and IPv6 address format validation, ensuring IP addresses are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`IpAddress` is a validated value object that represents an IP address. It supports both IPv4 and IPv6 address formats and ensures proper validation. Once created, an `IpAddress` instance is guaranteed to be valid.

### Key Features

- **IPv4 Support** - Validates IPv4 address format (192.168.1.1)
- **IPv6 Support** - Validates IPv6 address format (2001:db8::1)
- **Format Validation** - Ensures proper IP address structure
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating an IP Address

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, ipAddress) = IpAddress.TryCreate("192.168.1.100");

if (result.IsValid)
{
    Console.WriteLine(ipAddress.Value);      // "192.168.1.100"
    Console.WriteLine(ipAddress.ToString()); // "192.168.1.100"
    
    // Use the validated IP address
    ProcessNetworkAddress(ipAddress);
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
public class NetworkDevice
{
    public Guid Id { get; set; }
    public IpAddress IpAddress { get; set; }
    public string Hostname { get; set; }
}

// Usage
var (result, ipAddress) = IpAddress.TryCreate(userInput);
if (result.IsValid)
{
    var device = new NetworkDevice
    {
        Id = Guid.NewGuid(),
        IpAddress = ipAddress,  // Guaranteed valid
        Hostname = hostname
    };
}
```

---

## ? Valid IP Addresses

### IPv4 Addresses

```csharp
// Valid IPv4 addresses
var (r1, i1) = IpAddress.TryCreate("192.168.1.1");      // ? Private network
var (r2, i2) = IpAddress.TryCreate("10.0.0.1");         // ? Private network
var (r3, i3) = IpAddress.TryCreate("172.16.0.1");       // ? Private network
var (r4, i4) = IpAddress.TryCreate("8.8.8.8");          // ? Google DNS
var (r5, i5) = IpAddress.TryCreate("127.0.0.1");        // ? Localhost

foreach (var (result, ip) in new[] { (r1, i1), (r2, i2), (r3, i3), (r4, i4), (r5, i5) })
{
    Console.WriteLine($"{ip.Value}: {result.IsValid}");
}
```

### IPv6 Addresses

```csharp
// Valid IPv6 addresses
var (r1, i1) = IpAddress.TryCreate("::1");                          // ? IPv6 localhost
var (r2, i2) = IpAddress.TryCreate("2001:db8::1");                  // ? Documentation address
var (r3, i3) = IpAddress.TryCreate("fe80::1%lo0");                  // ? Link-local with zone
var (r4, i4) = IpAddress.TryCreate("2001:0db8:85a3:0000:0000:8a2e:0370:7334"); // ? Full address
var (r5, i5) = IpAddress.TryCreate("2001:db8:85a3::8a2e:370:7334"); // ? Compressed
```

### Special IPv4 Addresses

```csharp
var (r1, i1) = IpAddress.TryCreate("0.0.0.0");          // ? Default route
var (r2, i2) = IpAddress.TryCreate("255.255.255.255");  // ? Broadcast
var (r3, i3) = IpAddress.TryCreate("224.0.0.1");        // ? Multicast
var (r4, i4) = IpAddress.TryCreate("169.254.1.1");      // ? Link-local (APIPA)
```

### Special IPv6 Addresses

```csharp
var (r1, i1) = IpAddress.TryCreate("::");                           // ? Unspecified address
var (r2, i2) = IpAddress.TryCreate("::ffff:192.0.2.1");            // ? IPv4-mapped IPv6
var (r3, i3) = IpAddress.TryCreate("2001:db8::");                   // ? Network address
var (r4, i4) = IpAddress.TryCreate("ff02::1");                      // ? Multicast
```

---

## ? Invalid IP Addresses

### Malformed IPv4

```csharp
var (r1, i1) = IpAddress.TryCreate("192.168.1");        // ? Missing octet
var (r2, i2) = IpAddress.TryCreate("192.168.1.1.1");    // ? Too many octets
var (r3, i3) = IpAddress.TryCreate("192.168.1.256");    // ? Octet value too high
var (r4, i4) = IpAddress.TryCreate("192.168.01.1");     // ? Leading zero
var (r5, i5) = IpAddress.TryCreate("192.168.1.-1");     // ? Negative octet
```

### Malformed IPv6

```csharp
var (r1, i1) = IpAddress.TryCreate("2001:db8::1::2");   // ? Multiple compression
var (r2, i2) = IpAddress.TryCreate("2001:db8:gggg::1"); // ? Invalid hexadecimal
var (r3, i3) = IpAddress.TryCreate("2001:db8::1:");      // ? Trailing colon
var (r4, i4) = IpAddress.TryCreate(":2001:db8::1");      // ? Leading colon
```

### Invalid Characters

```csharp
var (r1, i1) = IpAddress.TryCreate("192.168.1.abc");    // ? Letters in IPv4
var (r2, i2) = IpAddress.TryCreate("192.168.1.1.");     // ? Trailing dot
var (r3, i3) = IpAddress.TryCreate(".192.168.1.1");     // ? Leading dot
var (r4, i4) = IpAddress.TryCreate("192..168.1.1");     // ? Double dot
```

### Empty or Null

```csharp
var (r1, i1) = IpAddress.TryCreate("");                 // ? Empty string
var (r2, i2) = IpAddress.TryCreate(null);               // ? Null value
var (r3, i3) = IpAddress.TryCreate("   ");              // ? Whitespace only
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, ipAddress) = IpAddress.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated IP address
    ProcessNetworkAddress(ipAddress);
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
public class Server
{
    public Guid Id { get; set; }
    public IpAddress IpAddress { get; set; }
    public string Name { get; set; }
}

// Creating a server
var (ipResult, ipAddress) = IpAddress.TryCreate(input);
if (ipResult.IsValid)
{
    var server = new Server
    {
        Id = Guid.NewGuid(),
        IpAddress = ipAddress,  // Type-safe and validated
        Name = name
    };
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, ipAddress) = IpAddress.TryCreate("192.168.1.100");

// Serialize
string json = JsonSerializer.Serialize(ipAddress);
// {"Value":"192.168.1.100"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<IpAddress>(json);
Console.WriteLine(deserialized.Value);  // "192.168.1.100"
```

---

## ?? Related Documentation

- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated IP address string |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, string propertyName = "IpAddress")` | `(ValidationResult, IpAddress?)` | Static factory method to create validated IP address |
| `ToString()` | `string` | Returns the IP address value |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | IP address cannot be null, empty, or whitespace only |
| **Valid IP Format** | Must be valid IPv4 or IPv6 address format |

---

## ?? IP Address Standards

### IPv4 Address Format
- **Structure**: Four octets separated by dots (192.168.1.1)
- **Range**: Each octet 0-255
- **No leading zeros**: 192.168.1.1 (not 192.168.01.1)
- **No extra characters**: Pure numeric with dots

### IPv6 Address Format
- **Structure**: Eight groups of four hexadecimal digits separated by colons
- **Compression**: `::` can replace consecutive zero groups
- **Case insensitive**: `2001:DB8::1` same as `2001:db8::1`
- **Zone identifiers**: `fe80::1%eth0` (interface specification)

### Special IPv4 Addresses

| Address Range | Purpose | Example |
|---------------|---------|---------|
| 0.0.0.0/8 | Default route | 0.0.0.0 |
| 10.0.0.0/8 | Private network | 10.0.0.1 |
| 127.0.0.0/8 | Loopback | 127.0.0.1 |
| 169.254.0.0/16 | Link-local | 169.254.1.1 |
| 172.16.0.0/12 | Private network | 172.16.0.1 |
| 192.168.0.0/16 | Private network | 192.168.1.1 |
| 224.0.0.0/4 | Multicast | 224.0.0.1 |
| 255.255.255.255 | Broadcast | 255.255.255.255 |

### Special IPv6 Addresses

| Address | Purpose | Example |
|---------|---------|---------|
| ::1 | Loopback | ::1 |
| :: | Unspecified | :: |
| fe80::/10 | Link-local | fe80::1 |
| fc00::/7 | Unique local | fc00::1 |
| ff00::/8 | Multicast | ff02::1 |
| 2001:db8::/32 | Documentation | 2001:db8::1 |

---

## ??? Security Considerations

### IP Address Validation

```csharp
// ? DO: Validate before use
var (result, ipAddress) = IpAddress.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid IP address");
}

// ? DON'T: Trust user input without validation
var ipAddress = userInput;  // Dangerous!
ProcessNetworkAddress(ipAddress);
```

### Preventing SSRF Attacks

```csharp
// ? DO: Validate and restrict IP addresses
public async Task<FetchResult> FetchFromIpAddress(IpAddress ipAddress)
{
    // Block private/internal IP ranges
    if (IsPrivateIpAddress(ipAddress.Value))
    {
        return FetchResult.Forbidden("Private IP addresses not allowed");
    }

    // Block localhost
    if (ipAddress.Value == "127.0.0.1" || ipAddress.Value == "::1")
    {
        return FetchResult.Forbidden("Localhost not allowed");
    }

    // Safe to fetch
    return await _httpClient.GetAsync($"http://{ipAddress.Value}");
}

private bool IsPrivateIpAddress(string ipAddress)
{
    // Check for private IPv4 ranges
    if (IPAddress.TryParse(ipAddress, out var ip))
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = ip.GetAddressBytes();
            // 10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16
            return (bytes[0] == 10) ||
                   (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                   (bytes[0] == 192 && bytes[1] == 168);
        }
    }
    return false;
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize IP address input before validation
public string SanitizeIpAddressInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Trim whitespace
    input = input.Trim();
    
    // Remove common prefixes that might be added by users
    input = input.Replace("http://", "").Replace("https://", "");
    
    // For IPv6, remove zone identifiers if present
    var zoneIndex = input.IndexOf('%');
    if (zoneIndex > 0)
    {
        input = input.Substring(0, zoneIndex);
    }
    
    return input;
}

// Usage
var sanitized = SanitizeIpAddressInput(userInput);
var (result, ipAddress) = IpAddress.TryCreate(sanitized);
```

### Logging Network Data

```csharp
// ? DO: Log IP addresses appropriately
public void LogNetworkAccess(IpAddress ipAddress, string userId)
{
    // For privacy, consider masking parts of IP addresses in logs
    var maskedIp = MaskIpAddress(ipAddress.Value);
    
    _logger.LogInformation(
        "Network access by user {UserId} from {MaskedIp}",
        userId,
        maskedIp
    );
}

private string MaskIpAddress(string ipAddress)
{
    if (IPAddress.TryParse(ipAddress, out var ip))
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            // Mask IPv4: 192.168.1.100 -> 192.168.1.xxx
            var parts = ipAddress.Split('.');
            if (parts.Length == 4)
            {
                return $"{parts[0]}.{parts[1]}.{parts[2]}.xxx";
            }
        }
        else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            // Mask IPv6: 2001:db8::1 -> 2001:db8::xxxx
            return ipAddress.Replace(ipAddress.Substring(ipAddress.Length - 4), "xxxx");
        }
    }
    return ipAddress;
}

// ? DON'T: Log full IP addresses that might contain sensitive data
_logger.LogInformation($"Access from: {fullIpAddress}");  // Avoid
```
