using Validated.Primitives.Validation;

namespace Validated.Primitives.Domain.Geospatial;

/// <summary>
/// Represents a segment of a route with a starting and ending coordinate.
/// Contains the calculated distance between the two points.
/// </summary>
public sealed record RouteSegment
{
    /// <summary>
    /// Gets the starting coordinate of the segment.
    /// </summary>
    public required Coordinate From { get; init; }

    /// <summary>
    /// Gets the ending coordinate of the segment.
    /// </summary>
    public required Coordinate To { get; init; }

    /// <summary>
    /// Gets the calculated distance between From and To coordinates.
    /// </summary>
    public required GeoDistance Distance { get; init; }

    /// <summary>
    /// Gets the optional name or description of the segment.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Creates a new RouteSegment with validation and distance calculation.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The ending coordinate.</param>
    /// <param name="name">Optional name or description for the segment.</param>
    /// <returns>A tuple containing the validation result and the RouteSegment if valid.</returns>
    public static (ValidationResult Result, RouteSegment? Value) TryCreate(
        Coordinate? from,
        Coordinate? to,
        string? name = null)
    {
        var result = ValidationResult.Success();

        // Validate coordinates are provided
        if (from == null)
        {
            result.AddError("Starting coordinate (From) is required.", nameof(From), "Required");
        }

        if (to == null)
        {
            result.AddError("Ending coordinate (To) is required.", nameof(To), "Required");
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        // Calculate distance between coordinates
        var (distanceResult, distance) = GeoDistance.TryCreate(from!, to!);
        if (!distanceResult.IsValid)
        {
            result.Merge(distanceResult);
            return (result, null);
        }

        var segment = new RouteSegment
        {
            From = from!,
            To = to!,
            Distance = distance!,
            Name = name
        };

        return (result, segment);
    }

    /// <summary>
    /// Gets a formatted description of the segment.
    /// </summary>
    /// <returns>Formatted segment description.</returns>
    public string GetDescription()
    {
        var description = Name != null 
            ? $"{Name}: " 
            : "Segment: ";

        description += $"{From.ToCardinalString()} ? {To.ToCardinalString()} " +
                      $"({Distance.ToFormattedString()})";

        return description;
    }

    /// <summary>
    /// Returns a formatted string representation of the segment.
    /// </summary>
    public override string ToString()
    {
        return GetDescription();
    }
}
