using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain;

/// <summary>
/// Represents validated contact information including email, phone numbers, and optional website.
/// </summary>
public sealed record ContactInformation
{
    /// <summary>
    /// Gets the validated email address.
    /// </summary>
    public required EmailAddress Email { get; init; }

    /// <summary>
    /// Gets the primary validated phone number.
    /// </summary>
    public required PhoneNumber PrimaryPhone { get; init; }

    /// <summary>
    /// Gets the optional secondary phone number.
    /// </summary>
    public PhoneNumber? SecondaryPhone { get; init; }

    /// <summary>
    /// Gets the optional website URL.
    /// </summary>
    public WebsiteUrl? Website { get; init; }

    /// <summary>
    /// Creates new ContactInformation with validation.
    /// </summary>
    /// <param name="email">The email address string to validate.</param>
    /// <param name="primaryPhone">The primary phone number string to validate.</param>
    /// <param name="secondaryPhone">Optional secondary phone number string to validate.</param>
    /// <param name="website">Optional website URL string to validate.</param>
    /// <returns>A tuple containing the validation result and the ContactInformation if valid.</returns>
    public static (ValidationResult Result, ContactInformation? Value) TryCreate(
        CountryCode countryCode,
        string email,
        string primaryPhone,
        string? secondaryPhone = null,
        string? website = null)
    {
        var result = ValidationResult.Success();

        // Validate email
        var (emailResult, emailValue) = EmailAddress.TryCreate(email, "Email");
        result.Merge(emailResult);

        // Validate primary phone
        var (primaryPhoneResult, primaryPhoneValue) = PhoneNumber.TryCreate(countryCode,primaryPhone, "PrimaryPhone");
        result.Merge(primaryPhoneResult);

        // Validate secondary phone if provided
        PhoneNumber? secondaryPhoneValue = null;
        if (!string.IsNullOrWhiteSpace(secondaryPhone))
        {
            var (secondaryPhoneResult, secPhoneVal) = PhoneNumber.TryCreate(countryCode,secondaryPhone, "SecondaryPhone");
            result.Merge(secondaryPhoneResult);
            secondaryPhoneValue = secPhoneVal;
        }

        // Validate website if provided
        WebsiteUrl? websiteValue = null;
        if (!string.IsNullOrWhiteSpace(website))
        {
            var (websiteResult, webValue) = WebsiteUrl.TryCreate(website, "Website");
            result.Merge(websiteResult);
            websiteValue = webValue;
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        var contactInfo = new ContactInformation
        {
            Email = emailValue!,
            PrimaryPhone = primaryPhoneValue!,
            SecondaryPhone = secondaryPhoneValue,
            Website = websiteValue
        };

        return (result, contactInfo);
    }

    /// <summary>
    /// Returns a formatted string representation of the contact information.
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string>
        {
            $"Email: {Email.Value}",
            $"Phone: {PrimaryPhone.Value}"
        };

        if (SecondaryPhone != null)
        {
            parts.Add($"Secondary Phone: {SecondaryPhone.Value}");
        }

        if (Website != null)
        {
            parts.Add($"Website: {Website.Value}");
        }

        return string.Join(" | ", parts);
    }
}
