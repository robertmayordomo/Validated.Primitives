using Validated.Primitives.Validation;

namespace Validated.Primitives.Domain;

/// <summary>
/// Represents a validated person's name with first name, last name, and optional middle name.
/// </summary>
public sealed record PersonName
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string? _middleName;

    /// <summary>
    /// Gets the first name.
    /// </summary>
    public required string FirstName
    {
        get => _firstName;
        init => _firstName = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public required string LastName
    {
        get => _lastName;
        init => _lastName = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Gets the optional middle name.
    /// </summary>
    public string? MiddleName
    {
        get => _middleName;
        init => _middleName = value?.Trim();
    }

    /// <summary>
    /// Gets the full name in "FirstName MiddleName LastName" format.
    /// </summary>
    public string FullName
    {
        get
        {
            var parts = new List<string> { FirstName };

            if (!string.IsNullOrWhiteSpace(MiddleName))
            {
                parts.Add(MiddleName);
            }

            parts.Add(LastName);

            return string.Join(" ", parts);
        }
    }

    /// <summary>
    /// Gets the formal name in "LastName, FirstName MiddleName" format.
    /// </summary>
    public string FormalName
    {
        get
        {
            var firstAndMiddle = string.IsNullOrWhiteSpace(MiddleName)
                ? FirstName
                : $"{FirstName} {MiddleName}";

            return $"{LastName}, {firstAndMiddle}";
        }
    }

    /// <summary>
    /// Gets the initials (e.g., "J.D." for "John Doe" or "J.M.D." for "John Michael Doe").
    /// </summary>
    public string Initials
    {
        get
        {
            var initials = new List<string> { $"{FirstName[0]}." };

            if (!string.IsNullOrWhiteSpace(MiddleName))
            {
                initials.Add($"{MiddleName[0]}.");
            }

            initials.Add($"{LastName[0]}.");

            return string.Join("", initials);
        }
    }

    /// <summary>
    /// Creates a new PersonName with validation.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="middleName">Optional middle name.</param>
    /// <returns>A tuple containing the validation result and the PersonName if valid.</returns>
    public static (ValidationResult Result, PersonName? Value) TryCreate(
        string firstName,
        string lastName,
        string? middleName = null)
    {
        var result = ValidationResult.Success();

        // Trim inputs first (handle null case)
        var trimmedFirstName = firstName?.Trim() ?? string.Empty;
        var trimmedLastName = lastName?.Trim() ?? string.Empty;
        var trimmedMiddleName = middleName?.Trim();

        // Validate first name
        if (string.IsNullOrWhiteSpace(trimmedFirstName))
        {
            result.AddError("First name is required and cannot be empty.", "FirstName", "Required");
        }
        else if (trimmedFirstName.Length < 1 || trimmedFirstName.Length > 50)
        {
            result.AddError("First name must be between 1 and 50 characters.", "FirstName", "Length");
        }

        // Validate last name
        if (string.IsNullOrWhiteSpace(trimmedLastName))
        {
            result.AddError("Last name is required and cannot be empty.", "LastName", "Required");
        }
        else if (trimmedLastName.Length < 1 || trimmedLastName.Length > 50)
        {
            result.AddError("Last name must be between 1 and 50 characters.", "LastName", "Length");
        }

        // Validate middle name if provided
        if (!string.IsNullOrWhiteSpace(trimmedMiddleName))
        {
            if (trimmedMiddleName.Length > 50)
            {
                result.AddError("Middle name cannot exceed 50 characters.", "MiddleName", "MaxLength");
            }
        }
        else
        {
            trimmedMiddleName = null; // Normalize empty middle name to null
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        var personName = new PersonName
        {
            FirstName = trimmedFirstName,
            LastName = trimmedLastName,
            MiddleName = trimmedMiddleName
        };

        return (result, personName);
    }

    /// <summary>
    /// Returns the full name.
    /// </summary>
    public override string ToString() => FullName;
}
