using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects.Geospatial;

namespace Validated.Primitives.Domain.Geospatial.Builders;

/// <summary>
/// Builder for creating validated Coordinate with a fluent interface.
/// </summary>
public sealed class CoordinateBuilder
{
    private decimal? _latitude;
    private decimal? _longitude;
    private int _decimalPlaces = 6;
    private decimal? _altitude;
    private decimal? _accuracy;

    /// <summary>
    /// Sets the latitude coordinate.
    /// </summary>
    /// <param name="latitude">The latitude value in degrees (-90 to +90).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithLatitude(decimal latitude)
    {
        _latitude = latitude;
        return this;
    }

    /// <summary>
    /// Sets the longitude coordinate.
    /// </summary>
    /// <param name="longitude">The longitude value in degrees (-180 to +180).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithLongitude(decimal longitude)
    {
        _longitude = longitude;
        return this;
    }

    /// <summary>
    /// Sets both latitude and longitude coordinates.
    /// </summary>
    /// <param name="latitude">The latitude value in degrees (-90 to +90).</param>
    /// <param name="longitude">The longitude value in degrees (-180 to +180).</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithCoordinates(decimal latitude, decimal longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
        return this;
    }

    /// <summary>
    /// Sets the number of decimal places for precision.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places (0-8). Default is 6.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithDecimalPlaces(int decimalPlaces)
    {
        _decimalPlaces = decimalPlaces;
        return this;
    }

    /// <summary>
    /// Sets the altitude in meters above sea level.
    /// </summary>
    /// <param name="altitude">The altitude in meters.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithAltitude(decimal? altitude)
    {
        _altitude = altitude;
        return this;
    }

    /// <summary>
    /// Sets the accuracy radius in meters.
    /// </summary>
    /// <param name="accuracy">The accuracy radius in meters.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithAccuracy(decimal? accuracy)
    {
        _accuracy = accuracy;
        return this;
    }

    /// <summary>
    /// Sets the complete coordinate information.
    /// </summary>
    /// <param name="latitude">The latitude value in degrees.</param>
    /// <param name="longitude">The longitude value in degrees.</param>
    /// <param name="altitude">Optional altitude in meters.</param>
    /// <param name="accuracy">Optional accuracy radius in meters.</param>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder WithPosition(
        decimal latitude, 
        decimal longitude,
        decimal? altitude = null,
        decimal? accuracy = null)
    {
        _latitude = latitude;
        _longitude = longitude;
        _altitude = altitude;
        _accuracy = accuracy;
        return this;
    }

    /// <summary>
    /// Builds the Coordinate with validation.
    /// </summary>
    /// <returns>A tuple containing the validation result and the Coordinate if valid.</returns>
    public (ValidationResult Result, Coordinate? Value) Build()
    {
        var result = ValidationResult.Success();

        // Validate required fields
        if (!_latitude.HasValue)
        {
            result.AddError("Latitude is required.", "Latitude", "Required");
        }

        if (!_longitude.HasValue)
        {
            result.AddError("Longitude is required.", "Longitude", "Required");
        }

        // Early return if required fields are missing
        if (!result.IsValid)
        {
            return (result, null);
        }

        // Use Coordinate.TryCreate to perform full validation
        var (coordinateResult, coordinateValue) = Coordinate.TryCreate(
            _latitude!.Value,
            _longitude!.Value,
            _decimalPlaces,
            _altitude,
            _accuracy);

        return (coordinateResult, coordinateValue);
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    /// <returns>The builder instance for fluent chaining.</returns>
    public CoordinateBuilder Reset()
    {
        _latitude = null;
        _longitude = null;
        _decimalPlaces = 6;
        _altitude = null;
        _accuracy = null;
        return this;
    }
}
