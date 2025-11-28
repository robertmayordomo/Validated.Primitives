using System.Net;
using System.Net.Sockets;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Validators;

public static class IpValidators
{
    public static ValueValidator<string> IpAddress(string fieldName = "IpAddress")
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Failure("Invalid IP address format.", fieldName, "IpAddress");

            if (!IPAddress.TryParse(value, out var ipAddress))
                return ValidationResult.Failure("Invalid IP address format.", fieldName, "IpAddress");

            // Ensure strict format validation
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                // IPv4: Must have exactly 4 octets
                var octets = value.Split('.');
                if (octets.Length != 4)
                    return ValidationResult.Failure("Invalid IP address format.", fieldName, "IpAddress");
                
                // Verify the normalized form matches input
                // This rejects leading zeros (e.g., "192.168.001.1") and shorthand notation
                var normalized = ipAddress.ToString();
                if (normalized != value)
                    return ValidationResult.Failure("Invalid IP address format.", fieldName, "IpAddress");
            }
            else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // IPv6: The parsed address should be valid
                // IPAddress.TryParse already handles IPv6 format validation well
                // Just ensure the input isn't corrupted
                if (value.Contains(' ') || value.StartsWith('.') || value.EndsWith('.'))
                    return ValidationResult.Failure("Invalid IP address format.", fieldName, "IpAddress");
            }
            else
            {
                // Unknown address family
                return ValidationResult.Failure("Invalid IP address format.", fieldName, "IpAddress");
            }

            return ValidationResult.Success();
        };
}
