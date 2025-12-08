using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Builders;

/// <summary>
/// Builder for creating validated Address with a fluent interface.
/// </summary>
public sealed class AddressBuilder
{
    private string? _street;
    private string? _addressLine2;
    private string? _city;
    private string? _stateProvince;
    private string? _postalCode;
    private CountryCode _country = CountryCode.Unknown;

    /// <summary>
    /// Sets the street address.
    /// </summary>
    /// <param name="street">The street address (e.g., "123 Main St").</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithStreet(string? street)
    {
        _street = street;
        return this;
    }

    /// <summary>
    /// Sets the second address line (apartment, suite, etc.).
    /// </summary>
    /// <param name="addressLine2">The second address line.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithAddressLine2(string? addressLine2)
    {
        _addressLine2 = addressLine2;
        return this;
    }

    /// <summary>
    /// Sets the city name.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithCity(string? city)
    {
        _city = city;
        return this;
    }

    /// <summary>
    /// Sets the state or province.
    /// </summary>
    /// <param name="stateProvince">The state or province.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithStateProvince(string? stateProvince)
    {
        _stateProvince = stateProvince;
        return this;
    }

    /// <summary>
    /// Sets the postal code.
    /// </summary>
    /// <param name="postalCode">The postal code string.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithPostalCode(string? postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    /// <summary>
    /// Sets the country code for postal code validation.
    /// </summary>
    /// <param name="country">The country code.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithCountry(CountryCode? country)
    {
        _country = country ?? CountryCode.Unknown;
        return this;
    }

    /// <summary>
    /// Sets the full address using individual components.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country code.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithAddress(string? street, string? city, CountryCode? country, string? postalCode)
    {
        _street = street;
        _city = city;
        _country = country ?? CountryCode.Unknown;
        _postalCode = postalCode;
        return this;
    }

    /// <summary>
    /// Sets the full address including optional fields.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country code.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="addressLine2">Optional second address line.</param>
    /// <param name="stateProvince">Optional state or province.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder WithAddress(
        string? street, 
        string? city, 
        CountryCode? country, 
        string? postalCode,
        string? addressLine2 = null,
        string? stateProvince = null)
    {
        _street = street;
        _addressLine2 = addressLine2;
        _city = city;
        _stateProvince = stateProvince;
        _country = country ?? CountryCode.Unknown;
        _postalCode = postalCode;
        return this;
    }

    /// <summary>
    /// Builds the Address with validation.
    /// </summary>
    /// <returns>A tuple containing the validation result and the Address if valid.</returns>
    public (ValidationResult Result, Address? Value) Build()
    {
        var result = ValidationResult.Success();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(_street))
        {
            result.AddError("Street address is required.", "Street", "Required");
        }

        if (string.IsNullOrWhiteSpace(_city))
        {
            result.AddError("City is required.", "City", "Required");
        }

        if (string.IsNullOrWhiteSpace(_postalCode))
        {
            result.AddError("Postal code is required.", "PostalCode", "Required");
        }

        if (_country == CountryCode.Unknown)
        {
            result.AddError("Country is required.", "Country", "Required");
        }

        // Early return if required fields are missing
        if (!result.IsValid)
        {
            return (result, null);
        }

        // Use Address.TryCreate to perform full validation
        var (addressResult, addressValue) = Address.TryCreate(
            _street!,
            _addressLine2,
            _city!,
            _country,
            _postalCode!,
            _stateProvince);

        return (addressResult, addressValue);
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    /// <returns>The builder instance for fluent chaining.</returns>
    public AddressBuilder Reset()
    {
        _street = null;
        _addressLine2 = null;
        _city = null;
        _stateProvince = null;
        _postalCode = null;
        _country = CountryCode.Unknown;
        return this;
    }
}
