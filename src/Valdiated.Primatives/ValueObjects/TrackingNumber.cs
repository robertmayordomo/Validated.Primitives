using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated tracking number supporting multiple carrier formats.
/// Supports UPS, FedEx (Express/Ground/SmartPost), USPS, DHL (Express/eCommerce/Global Mail), 
/// Amazon, Royal Mail, Canada Post, Australia Post, and other international carriers.
/// </summary>
[JsonConverter(typeof(TrackingNumberConverter))]
public sealed record TrackingNumber : ValidatedPrimitive<string>
{
    /// <summary>
    /// Gets the carrier format type.
    /// </summary>
    public TrackingNumberFormat Format { get; }

    private TrackingNumber(string value, string propertyName = "TrackingNumber") : base(value)
    {
        Validators.Add(CommonValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(TrackingNumberValidators.TrackingNumber(propertyName));
        
        // Detect format after validation
        Format = DetectFormat(value);
    }

    /// <summary>
    /// Attempts to create a TrackingNumber instance with validation.
    /// </summary>
    /// <param name="value">The tracking number string to validate.</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the TrackingNumber instance if valid.</returns>
    public static (ValidationResult Result, TrackingNumber? Value) TryCreate(string value, string propertyName = "TrackingNumber")
    {
        var trackingNumber = new TrackingNumber(value, propertyName);
        var validationResult = trackingNumber.Validate();
        var result = validationResult.IsValid ? trackingNumber : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Gets the normalized tracking number value without spaces or hyphens.
    /// </summary>
    public string GetNormalized() => Value.Replace(" ", "").Replace("-", "").ToUpperInvariant();

    /// <summary>
    /// Gets the carrier name based on the detected format.
    /// </summary>
    public string GetCarrierName() => Format switch
    {
        TrackingNumberFormat.UPS => "UPS",
        TrackingNumberFormat.FedExExpress => "FedEx Express",
        TrackingNumberFormat.FedExGround => "FedEx Ground",
        TrackingNumberFormat.FedExSmartPost => "FedEx SmartPost",
        TrackingNumberFormat.USPS => "USPS",
        TrackingNumberFormat.DHLExpress => "DHL Express",
        TrackingNumberFormat.DHLEcommerce => "DHL eCommerce",
        TrackingNumberFormat.DHLGlobalMail => "DHL Global Mail",
        TrackingNumberFormat.AmazonLogistics => "Amazon Logistics",
        TrackingNumberFormat.RoyalMail => "Royal Mail",
        TrackingNumberFormat.CanadaPost => "Canada Post",
        TrackingNumberFormat.AustraliaPost => "Australia Post",
        TrackingNumberFormat.TNT => "TNT",
        TrackingNumberFormat.ChinaPost => "China Post",
        TrackingNumberFormat.LaserShip => "LaserShip",
        TrackingNumberFormat.OnTrac => "OnTrac",
        TrackingNumberFormat.IrishPost => "Irish Post",
        _ => "Unknown Carrier"
    };

    /// <summary>
    /// Detects the carrier format from the tracking number.
    /// </summary>
    private static TrackingNumberFormat DetectFormat(string value)
    {
        var cleanValue = value.Replace(" ", "").Replace("-", "").ToUpperInvariant();

        // UPS: 18 digits starting with "1Z"
        if (cleanValue.StartsWith("1Z") && cleanValue.Length == 18)
            return TrackingNumberFormat.UPS;

        // Amazon Logistics: "TBA" followed by 12 digits
        if (cleanValue.StartsWith("TBA") && cleanValue.Length == 15 && cleanValue.Substring(3).All(char.IsDigit))
            return TrackingNumberFormat.AmazonLogistics;

        // DHL eCommerce: 22 alphanumeric starting with "GM"
        if (cleanValue.StartsWith("GM") && cleanValue.Length == 22)
            return TrackingNumberFormat.DHLEcommerce;

        // LaserShip: "1LS" followed by 12 digits
        if (cleanValue.StartsWith("1LS") && cleanValue.Length == 15 && cleanValue.Substring(3).All(char.IsDigit))
            return TrackingNumberFormat.LaserShip;

        // OnTrac: "C" followed by 14 digits
        if (cleanValue.StartsWith("C") && cleanValue.Length == 15 && cleanValue.Substring(1).All(char.IsDigit))
            return TrackingNumberFormat.OnTrac;

        // TNT: 9 digits
        if (cleanValue.Length == 9 && cleanValue.All(char.IsDigit))
            return TrackingNumberFormat.TNT;

        // DHL Express: 10 digits
        if (cleanValue.Length == 10 && cleanValue.All(char.IsDigit))
            return TrackingNumberFormat.DHLExpress;

        // FedEx Express: 12 digits
        if (cleanValue.Length == 12 && cleanValue.All(char.IsDigit))
            return TrackingNumberFormat.FedExExpress;

        // Royal Mail / China Post / USPS / Irish Post: 13 characters (2 letters + 9 digits + 2 letters)
        // Need to check suffix to distinguish between these carriers
        if (cleanValue.Length == 13 && 
            char.IsLetter(cleanValue[0]) && char.IsLetter(cleanValue[1]) &&
            cleanValue.Substring(2, 9).All(char.IsDigit) &&
            char.IsLetter(cleanValue[11]) && char.IsLetter(cleanValue[12]))
        {
            // Check last two letters to distinguish carriers
            var suffix = cleanValue.Substring(11, 2);
            
            // Royal Mail uses GB suffix
            if (suffix == "GB")
                return TrackingNumberFormat.RoyalMail;
            
            // China Post uses CN suffix
            if (suffix == "CN")
                return TrackingNumberFormat.ChinaPost;
            
            // Irish Post uses IE suffix
            if (suffix == "IE")
                return TrackingNumberFormat.IrishPost;
            
            // USPS uses US suffix or other two-letter country codes
            // Common USPS suffixes: US, etc.
            if (suffix == "US")
                return TrackingNumberFormat.USPS;
            
            // For other international tracking formats with this pattern,
            // default to USPS as it's commonly used for international mail
            return TrackingNumberFormat.USPS;
        }

        // Australia Post / TNT: 13 digits
        // NOTE: These formats are ambiguous - both use 13 digits
        // Australia Post typically starts with specific prefixes, but without additional context
        // we default to Australia Post as it's more commonly used internationally
        if (cleanValue.Length == 13 && cleanValue.All(char.IsDigit))
        {
            // Australia Post tracking numbers often start with specific digit patterns
            // Common prefixes: 3, 7, or other specific patterns
            // For now, we'll default to Australia Post
            // In a real application, you might need additional context or user input
            return TrackingNumberFormat.AustraliaPost;
        }

        // FedEx Ground: 15 digits
        if (cleanValue.Length == 15 && cleanValue.All(char.IsDigit))
            return TrackingNumberFormat.FedExGround;

        // Canada Post: 16 alphanumeric (must check before DHL Global Mail)
        if (cleanValue.Length == 16 && cleanValue.All(char.IsLetterOrDigit))
            return TrackingNumberFormat.CanadaPost;

        // USPS: 20-22 alphanumeric, often starts with specific patterns
        // NOTE: Must check AFTER FedEx SmartPost (22 all-digits) to avoid false positives
        if (cleanValue.Length >= 20 && cleanValue.Length <= 22)
        {
            // FedEx SmartPost: 22 digits (check before USPS)
            if (cleanValue.Length == 22 && cleanValue.All(char.IsDigit))
                return TrackingNumberFormat.FedExSmartPost;
            
            // USPS: remaining 20-22 alphanumeric patterns
            if (cleanValue.All(char.IsLetterOrDigit))
                return TrackingNumberFormat.USPS;
        }

        // DHL Global Mail: 13-16 alphanumeric (generic fallback for this range)
        // Check this AFTER more specific 13-16 character patterns
        if (cleanValue.Length >= 13 && cleanValue.Length <= 16 && cleanValue.All(char.IsLetterOrDigit))
            return TrackingNumberFormat.DHLGlobalMail;

        return TrackingNumberFormat.Unknown;
    }

    /// <summary>
    /// Returns the tracking number value as a string.
    /// </summary>
    public override string ToString() => Value;
}

/// <summary>
/// Represents the carrier format type of a tracking number.
/// </summary>
public enum TrackingNumberFormat
{
    /// <summary>
    /// Unknown carrier format.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// UPS tracking number (18 characters starting with "1Z").
    /// </summary>
    UPS = 1,

    /// <summary>
    /// FedEx Express tracking number (12 digits).
    /// </summary>
    FedExExpress = 2,

    /// <summary>
    /// FedEx Ground tracking number (15 digits).
    /// </summary>
    FedExGround = 3,

    /// <summary>
    /// FedEx SmartPost tracking number (22 digits).
    /// </summary>
    FedExSmartPost = 4,

    /// <summary>
    /// USPS tracking number (20-22 alphanumeric, or 13 characters with 2 letters + 9 digits + 2 letters format).
    /// For 13-character format, typically ends with "US" or other non-GB/CN country codes.
    /// </summary>
    USPS = 5,

    /// <summary>
    /// DHL Express tracking number (10 digits).
    /// </summary>
    DHLExpress = 6,

    /// <summary>
    /// DHL eCommerce tracking number (22 alphanumeric starting with "GM").
    /// </summary>
    DHLEcommerce = 7,

    /// <summary>
    /// DHL Global Mail tracking number (13-16 alphanumeric).
    /// </summary>
    DHLGlobalMail = 8,

    /// <summary>
    /// Amazon Logistics tracking number ("TBA" followed by 12 digits).
    /// </summary>
    AmazonLogistics = 9,

    /// <summary>
    /// Royal Mail tracking number (13 characters: 2 letters + 9 digits + 2 letters ending in "GB").
    /// </summary>
    RoyalMail = 10,

    /// <summary>
    /// Canada Post tracking number (16 alphanumeric).
    /// </summary>
    CanadaPost = 11,

    /// <summary>
    /// Australia Post tracking number (13 digits).
    /// Note: 13-digit format is ambiguous with TNT. Defaults to Australia Post.
    /// </summary>
    AustraliaPost = 12,

    /// <summary>
    /// TNT tracking number (9 or 13 digits).
    /// Note: 13-digit format is ambiguous with Australia Post. Use 9-digit format for unambiguous TNT detection.
    /// </summary>
    TNT = 13,

    /// <summary>
    /// China Post tracking number (13 characters: 2 letters + 9 digits + 2 letters ending in "CN").
    /// </summary>
    ChinaPost = 14,

    /// <summary>
    /// LaserShip tracking number ("1LS" followed by 12 digits).
    /// </summary>
    LaserShip = 15,

    /// <summary>
    /// OnTrac tracking number ("C" followed by 14 digits).
    /// </summary>
    OnTrac = 16,

    /// <summary>
    /// Irish Post (An Post) tracking number (13 characters: 2 letters + 9 digits + 2 letters ending in "IE").
    /// </summary>
    IrishPost = 17
}
