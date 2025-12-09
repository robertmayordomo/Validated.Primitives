using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Serialization;

/// <summary>
/// JSON converter for DateOfBirth value objects.
/// Converts between DateOfBirth and DateTime for JSON serialization.
/// </summary>
public class DateOfBirthConverter : ValidatedPrimitiveConverter<DateOfBirth, DateTime>
{
    /// <summary>
    /// Initializes a new instance of the DateOfBirthConverter class.
    /// </summary>
    public DateOfBirthConverter()
        : base(DateOfBirth.TryCreate, "DateOfBirth")
    {
    }
}
