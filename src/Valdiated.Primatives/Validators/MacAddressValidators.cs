using System.Text.RegularExpressions;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

/// <summary>
/// Validators for MAC addresses.
/// Supports standard formats: colon-separated (AA:BB:CC:DD:EE:FF), hyphen-separated (AA-BB-CC-DD-EE-FF), 
/// dot-separated (AABB.CCDD.EEFF), and continuous (AABBCCDDEEFF).
/// </summary>
public static partial class MacAddressValidators
{
    /// <summary>
    /// Validates that a value is a valid MAC address format.
    /// Accepts the following formats:
    /// - Colon-separated: AA:BB:CC:DD:EE:FF or aa:bb:cc:dd:ee:ff
    /// - Hyphen-separated: AA-BB-CC-DD-EE-FF or aa-bb-cc-dd-ee-ff
    /// - Dot-separated (Cisco): AABB.CCDD.EEFF or aabb.ccdd.eeff
    /// - Continuous: AABBCCDDEEFF or aabbccddeeff
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function.</returns>
    public static ValueValidator<string> ValidFormat(string fieldName = "MacAddress")
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure("MAC address cannot be empty.", fieldName, "Required");

            var trimmed = value.Trim();

            // Colon-separated format: AA:BB:CC:DD:EE:FF
            if (ColonSeparatedMacRegex().IsMatch(trimmed))
                return ValidationResult.Success();

            // Hyphen-separated format: AA-BB-CC-DD-EE-FF
            if (HyphenSeparatedMacRegex().IsMatch(trimmed))
                return ValidationResult.Success();

            // Dot-separated format (Cisco): AABB.CCDD.EEFF
            if (DotSeparatedMacRegex().IsMatch(trimmed))
                return ValidationResult.Success();

            // Continuous format: AABBCCDDEEFF
            if (ContinuousMacRegex().IsMatch(trimmed))
                return ValidationResult.Success();

            return ValidationResult.Failure(
                "Invalid MAC address format. Expected formats: AA:BB:CC:DD:EE:FF, AA-BB-CC-DD-EE-FF, AABB.CCDD.EEFF, or AABBCCDDEEFF.",
                fieldName,
                "InvalidFormat");
        };

    /// <summary>
    /// Validates that a MAC address is not a multicast address.
    /// Multicast addresses have the least significant bit of the first octet set to 1.
    /// Note: This validator will skip broadcast address FF:FF:FF:FF:FF:FF to allow it to be handled separately.
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function.</returns>
    public static ValueValidator<string> NotMulticast(string fieldName = "MacAddress")
        => value =>
        {
            var normalized = NormalizeMacAddress(value);
            if (string.IsNullOrEmpty(normalized) || normalized.Length != 12)
                return ValidationResult.Success(); // Let format validator handle this

            // Skip broadcast address - it should be handled by NotBroadcast validator
            if (normalized == "FFFFFFFFFFFF")
                return ValidationResult.Success();

            try
            {
                // Get first octet (first 2 hex digits)
                var firstOctet = Convert.ToByte(normalized.Substring(0, 2), 16);
                
                // Check if least significant bit is set (multicast)
                if ((firstOctet & 0x01) == 0x01)
                {
                    return ValidationResult.Failure(
                        "MAC address cannot be a multicast address.",
                        fieldName,
                        "MulticastAddress");
                }
            }
            catch
            {
                // If we can't parse, let the format validator handle it
                return ValidationResult.Success();
            }

            return ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a MAC address is not a broadcast address (FF:FF:FF:FF:FF:FF).
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function.</returns>
    public static ValueValidator<string> NotBroadcast(string fieldName = "MacAddress")
        => value =>
        {
            var normalized = NormalizeMacAddress(value);
            if (normalized == "FFFFFFFFFFFF")
            {
                return ValidationResult.Failure(
                    "MAC address cannot be the broadcast address (FF:FF:FF:FF:FF:FF).",
                    fieldName,
                    "BroadcastAddress");
            }

            return ValidationResult.Success();
        };

    /// <summary>
    /// Validates that a MAC address is not all zeros (00:00:00:00:00:00).
    /// </summary>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <returns>A validator function.</returns>
    public static ValueValidator<string> NotAllZeros(string fieldName = "MacAddress")
        => value =>
        {
            var normalized = NormalizeMacAddress(value);
            if (normalized == "000000000000")
            {
                return ValidationResult.Failure(
                    "MAC address cannot be all zeros (00:00:00:00:00:00).",
                    fieldName,
                    "AllZeros");
            }

            return ValidationResult.Success();
        };

    /// <summary>
    /// Normalizes a MAC address to continuous uppercase format (AABBCCDDEEFF).
    /// </summary>
    private static string NormalizeMacAddress(string macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
            return string.Empty;

        // Remove all separators and convert to uppercase
        return macAddress.Replace(":", "").Replace("-", "").Replace(".", "").ToUpperInvariant();
    }

    [GeneratedRegex(@"^([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}$")]
    private static partial Regex ColonSeparatedMacRegex();

    [GeneratedRegex(@"^([0-9A-Fa-f]{2}-){5}[0-9A-Fa-f]{2}$")]
    private static partial Regex HyphenSeparatedMacRegex();

    [GeneratedRegex(@"^([0-9A-Fa-f]{4}\.){2}[0-9A-Fa-f]{4}$")]
    private static partial Regex DotSeparatedMacRegex();

    [GeneratedRegex(@"^[0-9A-Fa-f]{12}$")]
    private static partial Regex ContinuousMacRegex();
}
