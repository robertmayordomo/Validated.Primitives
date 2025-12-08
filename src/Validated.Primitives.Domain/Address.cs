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
        string? street,
        string? addressLine2,
        string? city,
        CountryCode? country,
        string? postalCode,
        string? stateProvince = null)
    {
        var result = ValidationResult.Success();

        // Validate street using AddressLine primitive
        var (streetResult, streetValue) = AddressLine.TryCreate(street, "Street");
        if (!streetResult.IsValid)
        {
            result.Merge(streetResult);
        }
        
        // Street is required - check if it's null after validation
        if (streetValue == null)
        {
            result.AddError("Street address is required.", "Street", "Required");
        }

        // Validate optional address line 2
        AddressLine? line2Value = null;
        if (!string.IsNullOrWhiteSpace(addressLine2))
        {
            var (line2Result, line2Val) = AddressLine.TryCreate(addressLine2, "AddressLine2");
            if (!line2Result.IsValid)
            {
                result.Merge(line2Result);
            }
            line2Value = line2Val;
        }

        // Validate city using City primitive
        var (cityResult, cityValue) = City.TryCreate(city, "City");
        if (!cityResult.IsValid)
        {
            result.Merge(cityResult);
        }

        // Validate state/province if provided
        StateProvince? stateProvinceValue = null;
        if (!string.IsNullOrWhiteSpace(stateProvince))
        {
            var (stateResult, stateVal) = StateProvince.TryCreate(stateProvince, "StateProvince");
            if (!stateResult.IsValid)
            {
                result.Merge(stateResult);
            }
            stateProvinceValue = stateVal;
        }

        // Validate postal code using primitive - check for null country first
        PostalCode? postalValue = null;
        if (!country.HasValue || country.Value == CountryCode.Unknown)
        {
            result.AddError("Country is required.", "Country", "Required");
        }
        else
        {
            var (postalResult, postalVal) = PostalCode.TryCreate(country.Value, postalCode, "PostalCode");
            if (!postalResult.IsValid)
            {
                result.Merge(postalResult);
            }
            postalValue = postalVal;
        }

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
