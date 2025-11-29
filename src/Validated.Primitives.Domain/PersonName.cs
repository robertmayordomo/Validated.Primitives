using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain;

/// <summary>
/// Represents a validated person's name with first name, last name, and optional middle name.
/// </summary>
public sealed record PersonName
{
    /// <summary>
    /// Gets the first name.
    /// </summary>
    public required HumanName FirstName { get; init; } = null!;

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public required HumanName LastName { get; init; } = null!;

    /// <summary>
    /// Gets the optional middle name.
    /// </summary>
    public HumanName? MiddleName { get; init; }

    /// <summary>
    /// Gets the full name in "FirstName MiddleName LastName" format.
    /// </summary>
    public string FullName
    {
        get
        {
            var parts = new List<string> { FirstName.Value };

            if (MiddleName != null && !string.IsNullOrWhiteSpace(MiddleName.Value))
            {
                parts.Add(MiddleName.Value);
            }

            parts.Add(LastName.Value);

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
            var firstAndMiddle = MiddleName == null || string.IsNullOrWhiteSpace(MiddleName.Value)
                ? FirstName.Value
                : $"{FirstName.Value} {MiddleName.Value}";

            return $"{LastName.Value}, {firstAndMiddle}";
        }
    }

    /// <summary>
    /// Gets the initials (e.g., "J.D." for "John Doe" or "J.M.D." for "John Michael Doe").
    /// </summary>
    public string Initials
    {
        get
        {
            var initials = new List<string> { $"{FirstName.Value[0]}." };

            if (MiddleName != null && !string.IsNullOrWhiteSpace(MiddleName.Value))
            {
                initials.Add($"{MiddleName.Value[0]}.");
            }

            initials.Add($"{LastName.Value[0]}.");

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

        // Validate and create first name
        var (firstNameResult, firstNameValue) = HumanName.TryCreate(firstName, "FirstName");
        result.Merge(firstNameResult);

        // Validate and create last name
        var (lastNameResult, lastNameValue) = HumanName.TryCreate(lastName, "LastName");
        result.Merge(lastNameResult);

        // Validate and create middle name if provided
        HumanName? middleNameValue = null;
        if (!string.IsNullOrWhiteSpace(middleName))
        {
            var (middleNameResult, middleNameVal) = HumanName.TryCreate(middleName, "MiddleName");
            result.Merge(middleNameResult);
            middleNameValue = middleNameVal;
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        var personName = new PersonName
        {
            FirstName = firstNameValue!,
            LastName = lastNameValue!,
            MiddleName = middleNameValue
        };

        return (result, personName);
    }

    /// <summary>
    /// Returns the full name.
    /// </summary>
    public override string ToString() => FullName;
}
