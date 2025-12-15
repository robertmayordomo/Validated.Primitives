using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Domain.Geospatial;

/// <summary>
/// Represents a complete route composed of multiple contiguous segments.
/// Each segment connects to the next, forming a continuous path.
/// </summary>
[JsonConverter(typeof(Serialization.GeospatialRouteConverter))]
public sealed record GeospatialRoute
{
    /// <summary>
    /// Gets the ordered collection of route segments.
    /// Each segment's To coordinate must match the next segment's From coordinate.
    /// </summary>
    public required IReadOnlyList<RouteSegment> Segments { get; init; }

    /// <summary>
    /// Gets the total distance of the route (sum of all segment distances).
    /// </summary>
    public double TotalDistanceKilometers { get; init; }

    /// <summary>
    /// Gets the total distance in miles.
    /// </summary>
    public double TotalDistanceMiles => TotalDistanceKilometers * 0.621371;

    /// <summary>
    /// Gets the total distance in meters.
    /// </summary>
    public double TotalDistanceMeters => TotalDistanceKilometers * 1000;

    /// <summary>
    /// Gets the starting coordinate of the route (first segment's From coordinate).
    /// </summary>
    public Coordinate? StartingPoint => Segments.Count > 0 ? Segments[0].From : null;

    /// <summary>
    /// Gets the ending coordinate of the route (last segment's To coordinate).
    /// </summary>
    public Coordinate? EndingPoint => Segments.Count > 0 ? Segments[^1].To : null;

    /// <summary>
    /// Gets the optional name or description of the route.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Creates a new GeospatialRoute from a collection of segments.
    /// Validates that segments are contiguous (each segment's To matches the next segment's From).
    /// </summary>
    /// <param name="segments">The ordered collection of route segments.</param>
    /// <param name="name">Optional name or description for the route.</param>
    /// <returns>A tuple containing the validation result and the GeospatialRoute if valid.</returns>
    public static (ValidationResult Result, GeospatialRoute? Value) TryCreate(
        IEnumerable<RouteSegment>? segments,
        string? name = null)
    {
        var result = ValidationResult.Success();

        if (segments == null)
        {
            result.AddError("Segments collection is required.", nameof(Segments), "Required");
            return (result, null);
        }

        var segmentList = segments.ToList();

        // Validate minimum segments
        if (segmentList.Count == 0)
        {
            result.AddError("Route must have at least one segment.", nameof(Segments), "InsufficientSegments");
            return (result, null);
        }

        // Validate all segments are not null
        if (segmentList.Any(s => s == null))
        {
            result.AddError("All segments must be valid.", nameof(Segments), "InvalidSegment");
            return (result, null);
        }

        // Validate segments are contiguous
        for (int i = 0; i < segmentList.Count - 1; i++)
        {
            var currentSegment = segmentList[i];
            var nextSegment = segmentList[i + 1];

            // Check if current segment's To matches next segment's From
            if (!AreCoordinatesEqual(currentSegment.To, nextSegment.From))
            {
                result.AddError(
                    $"Segment {i + 1} ends at {currentSegment.To.ToCardinalString()} but segment {i + 2} starts at {nextSegment.From.ToCardinalString()}. Segments must be contiguous.",
                    $"Segments[{i + 1}]",
                    "NonContiguousSegments");
            }
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        // Calculate total distance
        var totalDistance = segmentList.Sum(s => s.Distance.Kilometers);

        var route = new GeospatialRoute
        {
            Segments = segmentList.AsReadOnly(),
            TotalDistanceKilometers = totalDistance,
            Name = name
        };

        return (result, route);
    }

    /// <summary>
    /// Gets a formatted description of the route.
    /// </summary>
    /// <returns>Formatted route description.</returns>
    public string GetDescription()
    {
        var description = Name != null 
            ? $"{Name}: " 
            : "Route: ";

        description += $"{Segments.Count} segment{(Segments.Count == 1 ? "" : "s")}, " +
                       $"Total distance: {TotalDistanceKilometers:F2} km ({TotalDistanceMiles:F2} mi)";

        if (StartingPoint != null && EndingPoint != null)
        {
            description += $"\nFrom: {StartingPoint.ToCardinalString()}";
            description += $"\nTo: {EndingPoint.ToCardinalString()}";
        }

        return description;
    }

    /// <summary>
    /// Gets all waypoints in the route (all unique coordinates in order).
    /// </summary>
    /// <returns>Ordered collection of waypoints.</returns>
    public IReadOnlyList<Coordinate> GetWaypoints()
    {
        var waypoints = new List<Coordinate>();

        if (Segments.Count == 0)
            return waypoints.AsReadOnly();

        // Add first segment's From coordinate
        waypoints.Add(Segments[0].From);

        // Add all To coordinates
        foreach (var segment in Segments)
        {
            waypoints.Add(segment.To);
        }

        return waypoints.AsReadOnly();
    }

    /// <summary>
    /// Gets the distance at a specific segment index.
    /// </summary>
    /// <param name="segmentIndex">Zero-based index of the segment.</param>
    /// <returns>The segment's distance, or null if index is out of range.</returns>
    public GeoDistance? GetSegmentDistance(int segmentIndex)
    {
        if (segmentIndex < 0 || segmentIndex >= Segments.Count)
            return null;

        return Segments[segmentIndex].Distance;
    }

    /// <summary>
    /// Gets the cumulative distance up to and including a specific segment.
    /// </summary>
    /// <param name="segmentIndex">Zero-based index of the segment.</param>
    /// <returns>Cumulative distance in kilometers, or null if index is out of range.</returns>
    public double? GetCumulativeDistance(int segmentIndex)
    {
        if (segmentIndex < 0 || segmentIndex >= Segments.Count)
            return null;

        double cumulative = 0;
        for (int i = 0; i <= segmentIndex; i++)
        {
            cumulative += Segments[i].Distance.Kilometers;
        }

        return cumulative;
    }

    /// <summary>
    /// Returns a formatted string representation of the route.
    /// </summary>
    public override string ToString()
    {
        return GetDescription();
    }

    /// <summary>
    /// Compares two coordinates for equality (same lat/long values).
    /// </summary>
    private static bool AreCoordinatesEqual(Coordinate c1, Coordinate c2)
    {
        return c1.Latitude.Value == c2.Latitude.Value &&
               c1.Longitude.Value == c2.Longitude.Value;
    }
}