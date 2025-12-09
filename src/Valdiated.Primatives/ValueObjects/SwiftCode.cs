using System.Text.Json.Serialization;
using Validated.Primitives.Core;
using Validated.Primitives.Serialization;
using Validated.Primitives.Validation;
using Validated.Primitives.Validators;

namespace Validated.Primitives.ValueObjects;

/// <summary>
/// Represents a validated SWIFT code (also known as BIC - Bank Identifier Code) according to ISO 9362 standard.
/// SWIFT codes are used to identify banks and financial institutions globally for international wire transfers.
/// 
/// ISO 9362 Format: AAAABBCCXXX where:
/// - AAAA: Institution code (4 letters A-Z) - identifies the bank or financial institution
/// - BB: Country code (2 letters A-Z, ISO 3166-1 alpha-2) - identifies the country
/// - CC: Location code (2 characters A-Z or 0-9) - identifies the location/city
/// - XXX: Branch code (optional, 3 characters A-Z or 0-9) - identifies the specific branch
/// 
/// Valid lengths according to ISO 9362:
/// - 8 characters: BIC8 (Business Identifier Code without branch - represents primary office)
/// - 11 characters: BIC11 (Business Identifier Code with branch)
/// 
/// Examples:
/// - DEUTDEFF (Deutsche Bank, Germany, Frankfurt - primary office, BIC8)
/// - DEUTDEFFXXX (same as above, with explicit branch code, BIC11)
/// - CHASUS33 (Chase Bank, USA - primary office, BIC8)
/// - NATAAU3303M (National Australia Bank, Melbourne branch, BIC11)
/// 
/// Reference: ISO 9362:2014 - Banking — Banking telecommunication messages — Business identifier code (BIC)
/// </summary>
[JsonConverter(typeof(SwiftCodeConverter))]
public sealed record SwiftCode : ValidatedPrimitive<string>
{
    private SwiftCode(string value, bool allowTestCodes, string propertyName = "SwiftCode") : base(value)
    {
        Validators.Add(SwiftCodeValidators.NotNullOrWhitespace(propertyName));
        Validators.Add(SwiftCodeValidators.ValidFormat(propertyName));
        Validators.Add(SwiftCodeValidators.ValidLength(propertyName));
        Validators.Add(SwiftCodeValidators.ValidStructure(propertyName));
        Validators.Add(SwiftCodeValidators.ValidCountryCode(propertyName));
        
        if (!allowTestCodes)
        {
            Validators.Add(SwiftCodeValidators.NotTestCode(propertyName));
        }
    }

    /// <summary>
    /// Attempts to create a SwiftCode instance with validation according to ISO 9362.
    /// </summary>
    /// <param name="value">The SWIFT code value (8 or 11 characters).</param>
    /// <param name="allowTestCodes">Whether to allow test SWIFT codes (ISO 9362 test BICs, default: false).</param>
    /// <param name="propertyName">The property name for error messages.</param>
    /// <returns>A tuple containing the validation result and the SwiftCode instance if valid.</returns>
    public static (ValidationResult Result, SwiftCode? Value) TryCreate(
        string value,
        bool allowTestCodes = false,
        string propertyName = "SwiftCode")
    {
        var swiftCode = new SwiftCode(value, allowTestCodes, propertyName);
        var validationResult = swiftCode.Validate();
        var result = validationResult.IsValid ? swiftCode : null;
        return (validationResult, result);
    }

    /// <summary>
    /// Returns the SWIFT code in normalized uppercase format as per ISO 9362 standard.
    /// </summary>
    public string ToNormalizedString()
    {
        return Value.Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Returns the SWIFT code in BIC11 format (11 characters).
    /// If the code is 8 characters (BIC8), appends 'XXX' to indicate primary office.
    /// ISO 9362: XXX as branch code indicates the primary office/head office.
    /// </summary>
    public string ToFullFormat()
    {
        var normalized = ToNormalizedString();
        return normalized.Length == 8 ? normalized + "XXX" : normalized;
    }

    /// <summary>
    /// Gets the institution code (first 4 characters).
    /// ISO 9362: Identifies the bank or financial institution (4 letters A-Z).
    /// </summary>
    public string InstitutionCode
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length >= 4 ? normalized.Substring(0, 4) : string.Empty;
        }
    }

    /// <summary>
    /// Gets the bank code (alias for InstitutionCode for backward compatibility).
    /// ISO 9362: Identifies the bank or financial institution (4 letters A-Z).
    /// </summary>
    public string BankCode => InstitutionCode;

    /// <summary>
    /// Gets the country code (characters 5-6).
    /// ISO 9362: ISO 3166-1 alpha-2 country code (2 letters A-Z).
    /// </summary>
    public string CountryCode
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length >= 6 ? normalized.Substring(4, 2) : string.Empty;
        }
    }

    /// <summary>
    /// Gets the location code (characters 7-8).
    /// ISO 9362: Identifies the location or city (2 characters A-Z or 0-9).
    /// Note: If second character is '0', this is typically a test BIC.
    /// </summary>
    public string LocationCode
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length >= 8 ? normalized.Substring(6, 2) : string.Empty;
        }
    }

    /// <summary>
    /// Gets the branch code (characters 9-11).
    /// ISO 9362: Returns 'XXX' for primary office if not specified (BIC8), or the actual branch code (BIC11).
    /// 'XXX' indicates the primary office/head office of the institution.
    /// </summary>
    public string BranchCode
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length == 11 ? normalized.Substring(8, 3) : "XXX";
        }
    }

    /// <summary>
    /// Gets whether this is a primary office code (8 characters or ending with 'XXX').
    /// ISO 9362: BIC8 or BIC11 with 'XXX' branch code indicates the primary/head office.
    /// </summary>
    public bool IsPrimaryOffice
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length == 8 || normalized.EndsWith("XXX");
        }
    }

    /// <summary>
    /// Gets whether this appears to be a test SWIFT code according to ISO 9362.
    /// ISO 9362: Test BICs have location code where the second character is '0'.
    /// </summary>
    public bool IsTestCode
    {
        get
        {
            var locationCode = LocationCode;
            return locationCode.Length >= 2 && locationCode[1] == '0';
        }
    }

    /// <summary>
    /// Gets whether this is a BIC8 format (8 characters, without explicit branch code).
    /// </summary>
    public bool IsBic8
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length == 8;
        }
    }

    /// <summary>
    /// Gets whether this is a BIC11 format (11 characters, with explicit branch code).
    /// </summary>
    public bool IsBic11
    {
        get
        {
            var normalized = ToNormalizedString();
            return normalized.Length == 11;
        }
    }

    /// <summary>
    /// Returns the SWIFT code value in normalized uppercase format.
    /// </summary>
    public override string ToString() => ToNormalizedString();

    /// <summary>
    /// Determines whether the specified SwiftCode is equal to the current SwiftCode.
    /// Comparison is case-insensitive and normalizes BIC8 to BIC11 format (e.g., DEUTDEFF == DEUTDEFFXXX).
    /// </summary>
    public bool Equals(SwiftCode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare full format to handle BIC8 vs BIC11 equivalence (e.g., DEUTDEFF == DEUTDEFFXXX)
        return ToFullFormat() == other.ToFullFormat();
    }

    /// <summary>
    /// Returns the hash code for this SwiftCode.
    /// </summary>
    public override int GetHashCode()
    {
        return ToFullFormat().GetHashCode();
    }
}
