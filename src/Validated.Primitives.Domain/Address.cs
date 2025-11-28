using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain;

/// <summary>
/// Represents a validated physical address with postal code, street, city, and optional state/province.
/// </summary>
public sealed record Address
{
    /// <summary>
    /// Gets the street address (e.g., "123 Main St").
    /// </summary>
    public required AddressLine Street { get; init; }

    /// <summary>
    /// Gets the optional second address line (e.g., "Apt 4B").
    /// </summary>
    public AddressLine? AddressLine2 { get; init; }

    /// <summary>
    /// Gets the city name.
    /// </summary>
    public required City City { get; init; }

    /// <summary>
    /// Gets the state or province (optional).
    /// </summary>
    public StateProvince? StateProvince { get; init; }

    /// <summary>
    /// Gets the validated postal code.
    /// </summary>
    public required PostalCode PostalCode { get; init; }

    /// <summary>
    /// Gets the country code associated with this address.
    /// </summary>
    public CountryCode Country => PostalCode.CountryCode;

    /// <summary>
    /// Creates a new Address with validation.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="addressLine2">Optional second address line (apartment, suite, etc.).</param>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country code for postal code validation.</param>
    /// <param name="postalCode">The postal code string to validate.</param>
    /// <param name="stateProvince">Optional state or province.</param>
    /// <returns>A tuple containing the validation result and the Address if valid.</returns>
    public static (ValidationResult Result, Address? Value) TryCreate(
        string street,
        string? addressLine2,
        string city,
        CountryCode country,
        string postalCode,
        string? stateProvince = null)
    {
        var result = ValidationResult.Success();

        // Validate street using AddressLine primitive
        var (streetResult, streetValue) = AddressLine.TryCreate(street, "Street");
        result.Merge(streetResult);

        // Ensure street is not null (it's required)
        if (streetValue == null)
        {
            result.AddError("Street address is required and cannot be empty.", "Street", "Required");
        }

        // Validate optional address line 2
        var (line2Result, line2Value) = AddressLine.TryCreate(addressLine2, "AddressLine2");
        result.Merge(line2Result);

        // Validate city using City primitive
        var (cityResult, cityValue) = City.TryCreate(city, "City");
        result.Merge(cityResult);

        // Validate state/province if provided
        StateProvince? stateProvinceValue = null;
        if (!string.IsNullOrWhiteSpace(stateProvince))
        {
            var (stateResult, stateVal) = StateProvince.TryCreate(stateProvince, "StateProvince");
            result.Merge(stateResult);
            stateProvinceValue = stateVal;
        }

        // Validate postal code using primitive
        var (postalResult, postalValue) = PostalCode.TryCreate(country, postalCode, "PostalCode");
        result.Merge(postalResult);

        if (!result.IsValid)
        {
            return (result, null);
        }

        var address = new Address
        {
            Street = streetValue!,
            AddressLine2 = line2Value,
            City = cityValue!,
            StateProvince = stateProvinceValue,
            PostalCode = postalValue!
        };

        return (result, address);
    }

    /// <summary>
    /// Returns a formatted string representation of the address.
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string> { Street.Value };
        
        if (AddressLine2 != null)
        {
            parts.Add(AddressLine2.Value);
        }
        
        parts.Add(City.Value);
        
        if (StateProvince != null)
        {
            parts.Add(StateProvince.Value);
        }
        
        parts.Add(PostalCode.Value);
        parts.Add(PostalCode.GetCountryName());

        return string.Join(", ", parts);
    }
}
