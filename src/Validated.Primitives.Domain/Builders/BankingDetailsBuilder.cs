using Validated.Primitives.Core;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Builders;

/// <summary>
/// Builder for creating BankingDetails with a fluent interface.
/// Supports multiple input formats and validates all banking components.
/// Automatically handles country-specific requirements (routing numbers for USA, sort codes for UK/Ireland).
/// </summary>
public class BankingDetailsBuilder
{
    private CountryCode? _country;
    private string? _accountNumber;
    private string? _swiftCode;
    private string? _routingNumber;
    private string? _sortCode;

    /// <summary>
    /// Sets the country for the banking details.
    /// </summary>
    /// <param name="country">The country code.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithCountry(CountryCode country)
    {
        _country = country;
        return this;
    }

    /// <summary>
    /// Sets the account number. Can be IBAN or domestic format.
    /// </summary>
    /// <param name="accountNumber">The bank account number (IBAN or domestic format).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithAccountNumber(string? accountNumber)
    {
        _accountNumber = accountNumber;
        return this;
    }

    /// <summary>
    /// Sets the SWIFT/BIC code for international transfers.
    /// </summary>
    /// <param name="swiftCode">The SWIFT/BIC code (8 or 11 characters).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithSwiftCode(string? swiftCode)
    {
        _swiftCode = swiftCode;
        return this;
    }

    /// <summary>
    /// Sets the ABA routing number for US domestic transfers.
    /// </summary>
    /// <param name="routingNumber">The 9-digit routing number.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithRoutingNumber(string? routingNumber)
    {
        _routingNumber = routingNumber;
        return this;
    }

    /// <summary>
    /// Sets the sort code for UK/Ireland domestic transfers.
    /// </summary>
    /// <param name="sortCode">The 6-digit sort code.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithSortCode(string? sortCode)
    {
        _sortCode = sortCode;
        return this;
    }

    /// <summary>
    /// Sets all US banking details in one call.
    /// </summary>
    /// <param name="routingNumber">The ABA routing number.</param>
    /// <param name="accountNumber">The account number.</param>
    /// <param name="swiftCode">Optional SWIFT code for international transfers.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithUsBanking(
        string routingNumber,
        string accountNumber,
        string? swiftCode = null)
    {
        _country = CountryCode.UnitedStates;
        _routingNumber = routingNumber;
        _accountNumber = accountNumber;
        _swiftCode = swiftCode;
        return this;
    }

    /// <summary>
    /// Sets all UK banking details in one call.
    /// </summary>
    /// <param name="sortCode">The sort code.</param>
    /// <param name="accountNumber">The account number.</param>
    /// <param name="swiftCode">Optional SWIFT code for international transfers.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithUkBanking(
        string sortCode,
        string accountNumber,
        string? swiftCode = null)
    {
        _country = CountryCode.UnitedKingdom;
        _sortCode = sortCode;
        _accountNumber = accountNumber;
        _swiftCode = swiftCode;
        return this;
    }

    /// <summary>
    /// Sets international banking details using IBAN and SWIFT code.
    /// </summary>
    /// <param name="iban">The IBAN account number.</param>
    /// <param name="swiftCode">The SWIFT/BIC code.</param>
    /// <param name="country">Optional country override (auto-detected from IBAN if not provided).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder WithInternationalBanking(
        string iban,
        string swiftCode,
        CountryCode? country = null)
    {
        _accountNumber = iban;
        _swiftCode = swiftCode;
        
        // If country not provided, it will be auto-detected from IBAN
        if (country.HasValue)
        {
            _country = country.Value;
        }
        
        return this;
    }

    /// <summary>
    /// Builds and validates the BankingDetails.
    /// Collects all validation errors from all components.
    /// </summary>
    /// <returns>A tuple containing the validation result and BankingDetails if valid.</returns>
    public (ValidationResult Result, BankingDetails? Value) Build()
    {
        var errors = new List<ValidationError>();

        // Validate country is provided first
        if (!_country.HasValue)
        {
            // Try to auto-detect from IBAN if account number looks like IBAN
            if (!string.IsNullOrWhiteSpace(_accountNumber))
            {
                var (accountResult, accountValue) = IbanNumber.TryCreate(_accountNumber);
                if (accountResult.IsValid && accountValue!.CountryCode.HasValue)
                {
                    _country = accountValue.CountryCode.Value;
                }
            }
            
            // If still no country, it's an error
            if (!_country.HasValue)
            {
                errors.Add(new ValidationError(
                    "Country is required",
                    nameof(BankingDetails.Country),
                    "Required"));
                
                var errorResult = ValidationResult.Success();
                foreach (var error in errors)
                {
                    errorResult.AddError(error.Message, error.MemberName, error.Code);
                }
                return (errorResult, null);
            }
        }

        // Validate account number is provided
        if (string.IsNullOrWhiteSpace(_accountNumber))
        {
            errors.Add(new ValidationError(
                "Account number is required",
                nameof(BankingDetails.AccountNumber),
                "Required"));
            
            var errorResult = ValidationResult.Success();
            foreach (var error in errors)
            {
                errorResult.AddError(error.Message, error.MemberName, error.Code);
            }
            return (errorResult, null);
        }

        // Create BankingDetails - this will perform all validation including country-specific validation
        return BankingDetails.TryCreate(
            _country!.Value,
            _accountNumber!,
            _swiftCode,
            _routingNumber,
            _sortCode);
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public BankingDetailsBuilder Reset()
    {
        _country = null;
        _accountNumber = null;
        _swiftCode = null;
        _routingNumber = null;
        _sortCode = null;
        return this;
    }
}
