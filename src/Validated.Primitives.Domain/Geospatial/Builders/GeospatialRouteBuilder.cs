using System.Collections.Generic;
using System.Linq;
using Validated.Primitives.Validation;

namespace Validated.Primitives.Domain.Geospatial.Builders;

/// <summary>
/// Builder for creating validated GeospatialRoute with a fluent interface.
/// </summary>
public sealed class GeospatialRouteBuilder
{
    private readonly List<RouteSegment> _segments = new();
    private string? _name;

    /// <summary>
    /// Adds a segment to the route.
    /// </summary>
    /// <param name="segment">The route segment to add.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public GeospatialRouteBuilder AddSegment(RouteSegment segment)
    {
        if (segment != null)
        {
            _segments.Add(segment);
        }
        return this;
    }

    /// <summary>
    /// Adds multiple segments to the route.
    /// </summary>
    /// <param name="segments">The route segments to add.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public GeospatialRouteBuilder AddSegments(IEnumerable<RouteSegment> segments)
    {
        if (segments != null)
        {
            _segments.AddRange(segments.Where(s => s != null));
        }
        return this;
    }

    /// <summary>
    /// Adds a segment by creating it from coordinates.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The ending coordinate.</param>
    /// <param name="segmentName">Optional name for the segment.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public GeospatialRouteBuilder AddSegment(Coordinate? from, Coordinate? to, string? segmentName = null)
    {
        var (result, segment) = RouteSegment.TryCreate(from, to, segmentName);
        if (result.IsValid && segment != null)
        {
            _segments.Add(segment);
        }
        return this;
    }

    /// <summary>
    /// Sets the name or description of the route.
    /// </summary>
    /// <param name="name">The route name or description.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public GeospatialRouteBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Builds the GeospatialRoute with validation.
    /// Validates that all segments are contiguous.
    /// </summary>
    /// <returns>A tuple containing the validation result and the GeospatialRoute if valid.</returns>
    public (ValidationResult Result, GeospatialRoute? Value) Build()
    {
        return GeospatialRoute.TryCreate(_segments, _name);
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    /// <returns>The builder instance for fluent chaining.</returns>
    public GeospatialRouteBuilder Reset()
    {
        _segments.Clear();
        _name = null;
        return this;
    }

    /// <summary>
    /// Gets the current number of segments in the builder.
    /// </summary>
    public int SegmentCount => _segments.Count;
}
