using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects.Geospatial;

namespace Validated.Primitives.Domain.Geospatial;

/// <summary>
/// Represents a validated geographic coordinate with latitude and longitude.
/// Combines validated Latitude and Longitude value objects for precise GPS positioning.
/// </summary>
public sealed record Coordinate
{
    /// <summary>
    /// Gets the validated latitude coordinate (-90 to +90 degrees).
    /// </summary>
    public required Latitude Latitude { get; init; }

    /// <summary>
    /// Gets the validated longitude coordinate (-180 to +180 degrees).
    /// </summary>
    public required Longitude Longitude { get; init; }

    /// <summary>
    /// Gets the optional altitude in meters above sea level.
    /// </summary>
    public decimal? Altitude { get; init; }

    /// <summary>
    /// Gets the optional accuracy radius in meters.
    /// </summary>
    public decimal? Accuracy { get; init; }

    /// <summary>
    /// Creates new Coordinate with validation.
    /// </summary>
    /// <param name="latitude">The latitude value in degrees (-90 to +90).</param>
    /// <param name="longitude">The longitude value in degrees (-180 to +180).</param>
    /// <param name="decimalPlaces">The number of decimal places for precision (0-8). Default is 6 for GPS precision.</param>
    /// <param name="altitude">Optional altitude in meters above sea level.</param>
    /// <param name="accuracy">Optional accuracy radius in meters.</param>
    /// <returns>A tuple containing the validation result and the Coordinate if valid.</returns>
    public static (ValidationResult Result, Coordinate? Value) TryCreate(
        decimal latitude,
        decimal longitude,
        int decimalPlaces = 6,
        decimal? altitude = null,
        decimal? accuracy = null)
    {
        var result = ValidationResult.Success();

        // Validate latitude
        var (latResult, latValue) = Latitude.TryCreate(latitude, decimalPlaces, "Latitude");
        result.Merge(latResult);

        // Validate longitude
        var (lonResult, lonValue) = Longitude.TryCreate(longitude, decimalPlaces, "Longitude");
        result.Merge(lonResult);

        // Validate altitude if provided
        if (altitude.HasValue && altitude.Value < -500m)
        {
            result.AddError("Altitude cannot be less than -500 meters (below sea level limit).", "Altitude", "InvalidAltitude");
        }

        if (altitude.HasValue && altitude.Value > 10000m)
        {
            result.AddError("Altitude cannot exceed 10,000 meters (typical GPS limit).", "Altitude", "InvalidAltitude");
        }

        // Validate accuracy if provided
        if (accuracy.HasValue && accuracy.Value < 0)
        {
            result.AddError("Accuracy cannot be negative.", "Accuracy", "InvalidAccuracy");
        }

        if (accuracy.HasValue && accuracy.Value > 1000000m)
        {
            result.AddError("Accuracy cannot exceed 1,000,000 meters.", "Accuracy", "InvalidAccuracy");
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        var coordinate = new Coordinate
        {
            Latitude = latValue!,
            Longitude = lonValue!,
            Altitude = altitude,
            Accuracy = accuracy
        };

        return (result, coordinate);
    }

    /// <summary>
    /// Gets the coordinate as a formatted string with cardinal directions.
    /// </summary>
    /// <returns>Formatted coordinate string (e.g., "40.712800° N, 74.006000° W").</returns>
    public string ToCardinalString()
    {
        var parts = new List<string>
        {
            Latitude.ToCardinalString(),
            Longitude.ToCardinalString()
        };

        if (Altitude.HasValue)
        {
            parts.Add($"{Altitude.Value:F1}m");
        }

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Gets the coordinate in decimal degrees format.
    /// </summary>
    /// <returns>Decimal degrees string (e.g., "40.7128, -74.0060").</returns>
    public string ToDecimalDegreesString()
    {
        return $"{Latitude.Value}, {Longitude.Value}";
    }

    /// <summary>
    /// Gets the coordinate formatted for Google Maps URL.
    /// </summary>
    /// <returns>Google Maps compatible coordinate string (e.g., "40.7128,-74.0060").</returns>
    public string ToGoogleMapsFormat()
    {
        return $"{Latitude.Value},{Longitude.Value}";
    }

    /// <summary>
    /// Calculates the approximate distance to another coordinate using the Haversine formula.
    /// </summary>
    /// <param name="other">The other coordinate to measure distance to.</param>
    /// <returns>Distance in kilometers.</returns>
    public double DistanceTo(Coordinate other)
    {
        const double earthRadiusKm = 6371.0;

        var lat1Rad = (double)Latitude.Value * Math.PI / 180.0;
        var lat2Rad = (double)other.Latitude.Value * Math.PI / 180.0;
        var deltaLat = ((double)other.Latitude.Value - (double)Latitude.Value) * Math.PI / 180.0;
        var deltaLon = ((double)other.Longitude.Value - (double)Longitude.Value) * Math.PI / 180.0;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    /// <summary>
    /// Returns a formatted string representation of the coordinate.
    /// </summary>
    public override string ToString()
    {
        var result = ToCardinalString();

        if (Accuracy.HasValue)
        {
            result += $" (±{Accuracy.Value:F0}m)";
        }

        return result;
    }
}
