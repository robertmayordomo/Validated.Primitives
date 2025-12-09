using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain;

/// <summary>
/// Represents complete banking details including account number and routing/identification codes.
/// Supports international banking with SWIFT/BIC codes and domestic US banking with routing numbers.
/// Also supports UK/Ireland sort codes.
/// 
/// Banking details are country-specific:
/// - International: SWIFT code + Account number
/// - USA: Routing number + Account number (+ optional SWIFT for international)
/// - UK/Ireland: Sort code + Account number (+ optional SWIFT for international)
/// </summary>
public sealed record BankingDetails
{
    /// <summary>
    /// Gets the country for which these banking details are configured.
    /// </summary>
    public required CountryCode Country { get; init; }

    /// <summary>
    /// Gets the bank account number.
    /// Required for all countries. Can be IBAN or domestic format depending on country.
    /// </summary>
    public required IbanNumber AccountNumber { get; init; }

    /// <summary>
    /// Gets the SWIFT/BIC code for international transfers.
    /// Optional but recommended for international transactions.
    /// </summary>
    public SwiftCode? SwiftCode { get; init; }

    /// <summary>
    /// Gets the ABA routing number for US domestic transfers.
    /// Required for USA, not applicable for other countries.
    /// </summary>
    public RoutingNumber? RoutingNumber { get; init; }

    /// <summary>
    /// Gets the sort code for UK/Ireland domestic transfers.
    /// Required for UK/Ireland, not applicable for other countries.
    /// </summary>
    public SortCode? SortCode { get; init; }

    /// <summary>
    /// Attempts to create BankingDetails with validation.
    /// </summary>
    /// <param name="country">The country for these banking details.</param>
    /// <param name="accountNumber">The bank account number (required).</param>
    /// <param name="swiftCode">The SWIFT/BIC code (optional for most countries, recommended for international).</param>
    /// <param name="routingNumber">The ABA routing number (required for USA).</param>
    /// <param name="sortCode">The sort code (required for UK/Ireland).</param>
    /// <returns>A tuple containing the validation result and BankingDetails if valid.</returns>
    public static (ValidationResult Result, BankingDetails? Value) TryCreate(
        CountryCode country,
        IbanNumber accountNumber,
        SwiftCode? swiftCode = null,
        RoutingNumber? routingNumber = null,
        SortCode? sortCode = null)
    {
        if (accountNumber == null)
        {
            return (ValidationResult.Failure(
                "Account number is required",
                nameof(AccountNumber),
                "Required"), null);
        }

        var result = ValidationResult.Success();

        // Validate country-specific requirements
        var validationResult = ValidateCountrySpecificRequirements(
            country,
            routingNumber,
            sortCode);

        result.Merge(validationResult);

        if (!result.IsValid)
        {
            return (result, null);
        }

        var bankingDetails = new BankingDetails
        {
            Country = country,
            AccountNumber = accountNumber,
            SwiftCode = swiftCode,
            RoutingNumber = routingNumber,
            SortCode = sortCode
        };

        return (result, bankingDetails);
    }

    /// <summary>
    /// Validates country-specific requirements for banking details.
    /// </summary>
    private static ValidationResult ValidateCountrySpecificRequirements(
        CountryCode country,
        RoutingNumber? routingNumber,
        SortCode? sortCode)
    {
        var result = ValidationResult.Success();

        // USA requires routing number
        if (country == CountryCode.UnitedStates && routingNumber == null)
        {
            result.AddError(
                "Routing number is required for United States banking details",
                nameof(RoutingNumber),
                "Required");
        }

        // UK/Ireland requires sort code
        if ((country == CountryCode.UnitedKingdom || country == CountryCode.Ireland) 
            && sortCode == null)
        {
            result.AddError(
                $"Sort code is required for {country} banking details",
                nameof(SortCode),
                "Required");
        }

        // Routing number should only be provided for USA
        if (country != CountryCode.UnitedStates && routingNumber != null)
        {
            result.AddError(
                "Routing number is only applicable for United States banking",
                nameof(RoutingNumber),
                "NotApplicable");
        }

        // Sort code should only be provided for UK/Ireland
        if (country != CountryCode.UnitedKingdom 
            && country != CountryCode.Ireland 
            && sortCode != null)
        {
            result.AddError(
                "Sort code is only applicable for United Kingdom or Ireland banking",
                nameof(SortCode),
                "NotApplicable");
        }

        return result;
    }

    /// <summary>
    /// Gets whether these banking details support international transfers (has SWIFT code).
    /// </summary>
    public bool SupportsInternationalTransfers => SwiftCode != null;

    /// <summary>
    /// Gets whether these banking details use IBAN format for the account number.
    /// </summary>
    public bool UsesIban => AccountNumber.IsIban;

    /// <summary>
    /// Gets a masked representation of the account number for display purposes.
    /// </summary>
    public string MaskedAccountNumber => AccountNumber.Masked();

    /// <summary>
    /// Returns a string representation of the banking details.
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string>();

        if (SwiftCode != null)
        {
            parts.Add($"SWIFT: {SwiftCode}");
        }

        if (RoutingNumber != null)
        {
            parts.Add($"Routing: {RoutingNumber.ToFormattedString()}");
        }

        if (SortCode != null)
        {
            parts.Add($"Sort Code: {SortCode.ToFormattedString()}");
        }

        parts.Add($"Account: {MaskedAccountNumber}");
        parts.Add($"({Country})");

        return string.Join(" | ", parts);
    }
}
