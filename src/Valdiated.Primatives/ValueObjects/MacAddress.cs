using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated MAC (Media Access Control) address.
/// Supports multiple common formats:
/// - Colon-separated: AA:BB:CC:DD:EE:FF
/// - Hyphen-separated: AA-BB-CC-DD-EE-FF
/// - Dot-separated (Cisco): AABB.CCDD.EEFF
/// - Continuous: AABBCCDDEEFF
/// 
/// Validates that the address is not a multicast, broadcast, or all-zeros address by default.
/// The value is stored in normalized uppercase colon-separated format (AA:BB:CC:DD:EE:FF).
/// </summary>
[JsonConverter(typeof(MacAddressConverter))]
public sealed record MacAddress : ValidatedPrimitive<string>
{
    private MacAddress(string value, bool allowMulticast, bool allowBroadcast, bool allowAllZeros, string propertyName = "MacAddress") 
        : base(NormalizeMacAddress(value))
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(MacAddressValidators.ValidFormat(propertyName));
        
        // Check broadcast BEFORE multicast because broadcast is also multicast
        if (!allowBroadcast)
        {
            Validators.Add(MacAddressValidators.NotBroadcast(propertyName));
        }
        
        if (!allowMulticast)
        {
            Validators.Add(MacAddressValidators.NotMulticast(propertyName));
        }
        
        if (!allowAllZeros)
        {
            Validators.Add(MacAddressValidators.NotAllZeros(propertyName));
        }
    }

    /// <summary>
    /// Attempts to create a MacAddress instance with validation.
    /// </summary>
    /// <param name="value">The MAC address string in any supported format.</param>
    /// <param name="allowMulticast">Whether to allow multicast addresses (default: false).</param>
    /// <param name="allowBroadcast">Whether to allow broadcast address FF:FF:FF:FF:FF:FF (default: false).</param>
    /// <param name="allowAllZeros">Whether to allow all-zeros address 00:00:00:00:00:00 (default: false).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the MacAddress instance if valid.</returns>
    public static (ValidationResult Result, MacAddress? Value) TryCreate(
        string value,
        bool allowMulticast = false,
        bool allowBroadcast = false,
        bool allowAllZeros = false,
        string propertyName = "MacAddress")
    {
        var macAddress = new MacAddress(value, allowMulticast, allowBroadcast, allowAllZeros, propertyName);
        var validationResult = macAddress.Validate();
        var result = validationResult.IsValid ? macAddress : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the MAC address in colon-separated uppercase format (AA:BB:CC:DD:EE:FF).
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Returns the MAC address in hyphen-separated uppercase format (AA-BB-CC-DD-EE-FF).
    /// </summary>
    public string ToHyphenFormat()
    {
        return Value.Replace(':', '-');
    }

    /// <summary>
    /// Returns the MAC address in dot-separated Cisco format (AABB.CCDD.EEFF).
    /// </summary>
    public string ToDotFormat()
    {
        var normalized = Value.Replace(":", "");
        return $"{normalized.Substring(0, 4)}.{normalized.Substring(4, 4)}.{normalized.Substring(8, 4)}";
    }

    /// <summary>
    /// Returns the MAC address in continuous format without separators (AABBCCDDEEFF).
    /// </summary>
    public string ToContinuousFormat()
    {
        return Value.Replace(":", "");
    }

    /// <summary>
    /// Gets the OUI (Organizationally Unique Identifier) - the first 3 octets.
    /// </summary>
    public string GetOUI()
    {
        var parts = Value.Split(':');
        return $"{parts[0]}:{parts[1]}:{parts[2]}";
    }

    /// <summary>
    /// Gets the NIC (Network Interface Controller) specific portion - the last 3 octets.
    /// </summary>
    public string GetNIC()
    {
        var parts = Value.Split(':');
        return $"{parts[3]}:{parts[4]}:{parts[5]}";
    }

    /// <summary>
    /// Checks if this is a locally administered address.
    /// </summary>
    public bool IsLocallyAdministered()
    {
        var firstOctet = Convert.ToByte(Value.Substring(0, 2), 16);
        return (firstOctet & 0x02) == 0x02;
    }

    /// <summary>
    /// Checks if this is a universally administered address.
    /// </summary>
    public bool IsUniversallyAdministered()
    {
        return !IsLocallyAdministered();
    }

    /// <summary>
    /// Checks if this is a multicast address.
    /// </summary>
    public bool IsMulticast()
    {
        var firstOctet = Convert.ToByte(Value.Substring(0, 2), 16);
        return (firstOctet & 0x01) == 0x01;
    }

    /// <summary>
    /// Checks if this is a unicast address.
    /// </summary>
    public bool IsUnicast()
    {
        return !IsMulticast();
    }

    /// <summary>
    /// Normalizes a MAC address to colon-separated uppercase format (AA:BB:CC:DD:EE:FF).
    /// </summary>
    private static string NormalizeMacAddress(string macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
            return string.Empty;

        // Remove all separators and convert to uppercase
        var normalized = macAddress.Replace(":", "").Replace("-", "").Replace(".", "").ToUpperInvariant();

        // If not 12 characters, return as-is and let validation fail
        if (normalized.Length != 12)
            return macAddress.Trim();

        // Format as AA:BB:CC:DD:EE:FF
        return $"{normalized.Substring(0, 2)}:{normalized.Substring(2, 2)}:{normalized.Substring(4, 2)}:" +
               $"{normalized.Substring(6, 2)}:{normalized.Substring(8, 2)}:{normalized.Substring(10, 2)}";
    }

    /// <summary>
    /// Determines whether the specified MacAddress is equal to the current MacAddress.
    /// </summary>
    public bool Equals(MacAddress? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Returns the hash code for this MacAddress.
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
