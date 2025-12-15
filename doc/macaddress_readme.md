# Validated.Primitives.MacAddress

A validated MAC (Media Access Control) address primitive that supports multiple common formats and provides address type detection, ensuring MAC addresses are always valid when used in your application.

## ?? Package Information

Part of the `Validated.Primitives` package.

```bash
dotnet add package Validated.Primitives
```

## Overview

`MacAddress` is a validated value object that represents a MAC address. It supports multiple common formats (colon-separated, hyphen-separated, dot-separated, continuous) and provides methods for address type detection and format conversion. Once created, a `MacAddress` instance is guaranteed to be valid.

### Key Features

- **Multiple Formats** - Supports colon, hyphen, dot, and continuous formats
- **Address Type Detection** - Identifies unicast/multicast, locally/universally administered
- **Format Conversion** - Convert between different MAC address formats
- **OUI/NIC Extraction** - Extract organization and network interface portions
- **Validation Options** - Configurable validation for special address types
- **Immutable** - Value object pattern ensures thread safety
- **Self-Validating** - Validation happens at creation time
- **Type Safety** - Cannot be confused with other string types
- **JSON Serialization** - Full support for serialization/deserialization

---

## ?? Basic Usage

### Creating a MAC Address

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation (colon format)
var (result, macAddress) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

if (result.IsValid)
{
    Console.WriteLine(macAddress.Value);              // "AA:BB:CC:DD:EE:FF"
    Console.WriteLine(macAddress.ToString());         // "AA:BB:CC:DD:EE:FF"
    Console.WriteLine(macAddress.ToHyphenFormat());   // "AA-BB-CC-DD-EE-FF"
    Console.WriteLine(macAddress.ToDotFormat());      // "AABB.CCDD.EEFF"
    Console.WriteLine(macAddress.ToContinuousFormat()); // "AABBCCDDEEFF"
    
    // Address type detection
    Console.WriteLine(macAddress.IsUnicast());        // true
    Console.WriteLine(macAddress.IsUniversallyAdministered()); // true
    Console.WriteLine(macAddress.GetOUI());           // "AA:BB:CC"
    Console.WriteLine(macAddress.GetNIC());           // "DD:EE:FF"
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

### Different Input Formats

```csharp
// All of these create the same normalized MAC address
var formats = new[]
{
    "AA:BB:CC:DD:EE:FF",    // Colon format
    "AA-BB-CC-DD-EE-FF",    // Hyphen format
    "AABB.CCDD.EEFF",       // Dot format (Cisco)
    "AABBCCDDEEFF"          // Continuous format
};

foreach (var format in formats)
{
    var (result, mac) = MacAddress.TryCreate(format);
    if (result.IsValid)
    {
        Console.WriteLine($"{format} -> {mac.Value}");
    }
}
```

### Validation Options

```csharp
// Allow special address types
var (result1, mac1) = MacAddress.TryCreate(
    "FF:FF:FF:FF:FF:FF",    // Broadcast address
    allowBroadcast: true    // Allow broadcast addresses
);

var (result2, mac2) = MacAddress.TryCreate(
    "01:23:45:67:89:AB",    // Multicast address
    allowMulticast: true    // Allow multicast addresses
);

var (result3, mac3) = MacAddress.TryCreate(
    "00:00:00:00:00:00",    // All-zeros address
    allowAllZeros: true     // Allow all-zeros addresses
);
```

---

## ?? Common Patterns

### Validation Pattern

```csharp
var (result, macAddress) = MacAddress.TryCreate(userInput);

if (result.IsValid)
{
    // Use the validated MAC address
    ProcessNetworkDevice(macAddress);
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

### Format Conversion Pattern

```csharp
var (result, macAddress) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

if (result.IsValid)
{
    // Convert to different formats as needed
    var colonFormat = macAddress.Value;              // "AA:BB:CC:DD:EE:FF"
    var hyphenFormat = macAddress.ToHyphenFormat();  // "AA-BB-CC-DD-EE-FF"
    var dotFormat = macAddress.ToDotFormat();        // "AABB.CCDD.EEFF"
    var continuousFormat = macAddress.ToContinuousFormat(); // "AABBCCDDEEFF"
}
```

### Address Analysis Pattern

```csharp
var (result, macAddress) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

if (result.IsValid)
{
    // Analyze address properties
    var isUnicast = macAddress.IsUnicast();
    var isUniversallyAdministered = macAddress.IsUniversallyAdministered();
    var oui = macAddress.GetOUI();       // Organization identifier
    var nic = macAddress.GetNIC();       // Network interface identifier
    
    // Use analysis results
    if (isUniversallyAdministered)
    {
        // Handle vendor-assigned address
    }
}
```

### JSON Serialization

```csharp
using System.Text.Json;

var (_, macAddress) = MacAddress.TryCreate("AA:BB:CC:DD:EE:FF");

// Serialize
string json = JsonSerializer.Serialize(macAddress);
// {"Value":"AA:BB:CC:DD:EE:FF"}

// Deserialize
var deserialized = JsonSerializer.Deserialize<MacAddress>(json);
Console.WriteLine(deserialized.Value);  // "AA:BB:CC:DD:EE:FF"
```

---

## ?? Related Documentation

- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string` | The validated MAC address in colon-separated uppercase format |

### Methods

| Method | Return Type | Description |
|--------|-------------|-------------|
| `TryCreate(string value, bool allowMulticast = false, bool allowBroadcast = false, bool allowAllZeros = false, string propertyName = "MacAddress")` | `(ValidationResult, MacAddress?)` | Static factory method to create validated MAC address |
| `ToString()` | `string` | Returns MAC address in colon format (AA:BB:CC:DD:EE:FF) |
| `ToHyphenFormat()` | `string` | Returns MAC address in hyphen format (AA-BB-CC-DD-EE-FF) |
| `ToDotFormat()` | `string` | Returns MAC address in dot format (AABB.CCDD.EEFF) |
| `ToContinuousFormat()` | `string` | Returns MAC address in continuous format (AABBCCDDEEFF) |
| `GetOUI()` | `string` | Returns Organizationally Unique Identifier (first 3 octets) |
| `GetNIC()` | `string` | Returns Network Interface Controller portion (last 3 octets) |
| `IsLocallyAdministered()` | `bool` | Checks if address is locally administered |
| `IsUniversallyAdministered()` | `bool` | Checks if address is universally administered |
| `IsMulticast()` | `bool` | Checks if address is multicast |
| `IsUnicast()` | `bool` | Checks if address is unicast |

### Validation Rules

| Rule | Description |
|------|-------------|
| **Not Null or Whitespace** | MAC address cannot be null, empty, or whitespace only |
| **Valid Format** | Must be valid MAC address format (any supported format accepted) |
| **Not Broadcast** | Cannot be broadcast address FF:FF:FF:FF:FF:FF (unless allowed) |
| **Not Multicast** | Cannot be multicast address (unless allowed) |
| **Not All Zeros** | Cannot be all-zeros address 00:00:00:00:00:00 (unless allowed) |

---

## ?? MAC Address Standards

### Supported Formats

| Format | Example | Description |
|--------|---------|-------------|
| **Colon-separated** | `AA:BB:CC:DD:EE:FF` | Standard IEEE 802 format |
| **Hyphen-separated** | `AA-BB-CC-DD-EE-FF` | Windows format |
| **Dot-separated** | `AABB.CCDD.EEFF` | Cisco format |
| **Continuous** | `AABBCCDDEEFF` | Compact format |

### Address Structure

A MAC address consists of 6 octets (48 bits):

```
AA:BB:CC:DD:EE:FF
?????????????????????
OUI ?   ?   ? NIC   ?
```

- **OUI (Organizationally Unique Identifier)**: First 3 octets, assigned by IEEE
- **NIC (Network Interface Controller)**: Last 3 octets, assigned by manufacturer

### Address Types

#### Unicast vs Multicast
- **Unicast**: Least significant bit of first octet is 0
- **Multicast**: Least significant bit of first octet is 1

#### Universal vs Local Administration
- **Universal**: Assigned by IEEE, LSB of first octet is 0
- **Local**: Locally assigned, LSB of first octet is 1

### Special Addresses

| Address | Type | Description |
|---------|------|-------------|
| `FF:FF:FF:FF:FF:FF` | Broadcast | Sent to all devices on network |
| `00:00:00:00:00:00` | All Zeros | Invalid/null address |
| Addresses starting with `01:` | Multicast | Group communication |

---

## ??? Security Considerations

### MAC Address Validation

```csharp
// ? DO: Validate before use
var (result, macAddress) = MacAddress.TryCreate(userInput);
if (!result.IsValid)
{
    return BadRequest("Invalid MAC address");
}

// ? DON'T: Trust user input without validation
var macAddress = userInput;  // Dangerous!
ProcessNetworkDevice(macAddress);
```

### Preventing Spoofing

```csharp
// ? DO: Validate MAC address format and reject suspicious patterns
var (result, macAddress) = MacAddress.TryCreate(userInput);
if (result.IsValid)
{
    // Additional security checks
    if (macAddress.Value == "FF:FF:FF:FF:FF:FF")
    {
        // Reject broadcast address
        return BadRequest("Broadcast address not allowed");
    }
    
    if (macAddress.Value == "00:00:00:00:00:00")
    {
        // Reject null address
        return BadRequest("Null address not allowed");
    }
    
    // Check for known problematic OUIs
    var oui = macAddress.GetOUI();
    if (IsBlockedOUI(oui))
    {
        return BadRequest("Address from blocked manufacturer");
    }
}
```

### Input Sanitization

```csharp
// ? DO: Sanitize MAC address input before validation
public string SanitizeMacAddressInput(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return input;
    
    // Trim whitespace
    input = input.Trim();
    
    // Remove common separators and convert to uppercase
    var sanitized = input.Replace(":", "").Replace("-", "").Replace(".", "").ToUpperInvariant();
    
    // Ensure only hexadecimal characters
    sanitized = new string(sanitized.Where(c => 
        char.IsDigit(c) || (c >= 'A' && c <= 'F')).ToArray());
    
    return sanitized;
}

// Usage
var sanitized = SanitizeMacAddressInput(userInput);
var (result, macAddress) = MacAddress.TryCreate(sanitized);
```

### Logging Network Data

```csharp
// ? DO: Log MAC addresses appropriately for privacy
public void LogNetworkDevice(MacAddress macAddress, string deviceId)
{
    // For privacy, consider masking parts of MAC addresses in logs
    var maskedMac = MaskMacAddress(macAddress.Value);
    
    _logger.LogInformation(
        "Network device {DeviceId} with MAC {MaskedMac}",
        deviceId,
        maskedMac
    );
}

private string MaskMacAddress(string macAddress)
{
    // Mask last 3 octets: AA:BB:CC:DD:EE:FF -> AA:BB:CC:XX:XX:XX
    var parts = macAddress.Split(':');
    if (parts.Length == 6)
    {
        return $"{parts[0]}:{parts[1]}:{parts[2]}:XX:XX:XX";
    }
    return macAddress;
}

// ? DON'T: Log full MAC addresses that might contain sensitive data
_logger.LogInformation($"Device MAC: {fullMacAddress}");  // Avoid
```
