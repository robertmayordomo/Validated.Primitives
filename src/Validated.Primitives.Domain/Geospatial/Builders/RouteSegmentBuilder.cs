using Validated.Primitives.Validation;

namespace Validated.Primitives.Domain.Geospatial.Builders;

/// <summary>
/// Builder for creating validated RouteSegment with a fluent interface.
/// </summary>
public sealed class RouteSegmentBuilder
{
    private Coordinate? _from;
    private Coordinate? _to;
    private string? _name;

    /// <summary>
    /// Sets the starting coordinate.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public RouteSegmentBuilder WithFrom(Coordinate? from)
    {
        _from = from;
        return this;
    }

    /// <summary>
    /// Sets the ending coordinate.
    /// </summary>
    /// <param name="to">The ending coordinate.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public RouteSegmentBuilder WithTo(Coordinate? to)
    {
        _to = to;
        return this;
    }

    /// <summary>
    /// Sets both From and To coordinates.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The ending coordinate.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public RouteSegmentBuilder WithCoordinates(Coordinate? from, Coordinate? to)
    {
        _from = from;
        _to = to;
        return this;
    }

    /// <summary>
    /// Sets the name or description of the segment.
    /// </summary>
    /// <param name="name">The segment name or description.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public RouteSegmentBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Builds the RouteSegment with validation.
    /// </summary>
    /// <returns>A tuple containing the validation result and the RouteSegment if valid.</returns>
    public (ValidationResult Result, RouteSegment? Value) Build()
    {
        return RouteSegment.TryCreate(_from, _to, _name);
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    /// <returns>The builder instance for fluent chaining.</returns>
    public RouteSegmentBuilder Reset()
    {
        _from = null;
        _to = null;
        _name = null;
        return this;
    }
}
