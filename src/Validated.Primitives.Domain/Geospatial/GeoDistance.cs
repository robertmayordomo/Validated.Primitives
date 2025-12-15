using Validated.Primitives.Validation;
using System.Text.Json.Serialization;

namespace Validated.Primitives.Domain.Geospatial;

/// <summary>
/// Represents the calculated distance between two geographic coordinates.
/// Uses the Haversine formula for accurate distance calculations on a sphere.
/// </summary>
[JsonConverter(typeof(Serialization.GeoDistanceConverter))]
public sealed record GeoDistance
{
    /// <summary>
    /// The Earth's mean radius in kilometers.
    /// </summary>
    public const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// The Earth's mean radius in miles.
    /// </summary>
    public const double EarthRadiusMiles = 3958.8;

    /// <summary>
    /// Gets the starting coordinate.
    /// </summary>
    public required Coordinate From { get; init; }

    /// <summary>
    /// Gets the ending coordinate.
    /// </summary>
    public required Coordinate To { get; init; }

    /// <summary>
    /// Gets the calculated distance in kilometers.
    /// </summary>
    public double Kilometers { get; init; }

    /// <summary>
    /// Gets the calculated distance in miles.
    /// </summary>
    public double Miles => Kilometers * 0.621371;

    /// <summary>
    /// Gets the calculated distance in meters.
    /// </summary>
    public double Meters => Kilometers * 1000;

    /// <summary>
    /// Gets the calculated distance in nautical miles.
    /// </summary>
    public double NauticalMiles => Kilometers * 0.539957;

    /// <summary>
    /// Creates a new GeoDistance by calculating the distance between two coordinates.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The ending coordinate.</param>
    /// <returns>A tuple containing the validation result and the GeoDistance if valid.</returns>
    public static (ValidationResult Result, GeoDistance? Value) TryCreate(Coordinate from, Coordinate to)
    {
        var result = ValidationResult.Success();

        // Validate coordinates are provided
        if (from == null)
        {
            result.AddError("Starting coordinate (From) is required.", "From", "Required");
        }

        if (to == null)
        {
            result.AddError("Ending coordinate (To) is required.", "To", "Required");
        }

        if (!result.IsValid)
        {
            return (result, null);
        }

        // Calculate distance using Haversine formula
        var distanceKm = CalculateHaversineDistance(from!, to!);

        var geoDistance = new GeoDistance
        {
            From = from!,
            To = to!,
            Kilometers = distanceKm
        };

        return (result, geoDistance);
    }

    /// <summary>
    /// Calculates the great-circle distance between two coordinates using the Haversine formula.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The ending coordinate.</param>
    /// <returns>Distance in kilometers.</returns>
    private static double CalculateHaversineDistance(Coordinate from, Coordinate to)
    {
        // Convert degrees to radians
        var lat1Rad = DegreesToRadians((double)from.Latitude.Value);
        var lat2Rad = DegreesToRadians((double)to.Latitude.Value);
        var deltaLat = DegreesToRadians((double)to.Latitude.Value - (double)from.Latitude.Value);
        var deltaLon = DegreesToRadians((double)to.Longitude.Value - (double)from.Longitude.Value);

        // Haversine formula
        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    /// <summary>
    /// Gets the distance formatted with the specified unit.
    /// </summary>
    /// <param name="unit">The unit to format (km, mi, m, nm).</param>
    /// <param name="decimalPlaces">Number of decimal places. Default is 2.</param>
    /// <returns>Formatted distance string.</returns>
    public string ToFormattedString(DistanceUnit unit = DistanceUnit.Kilometers, int decimalPlaces = 2)
    {
        var format = $"F{decimalPlaces}";
        return unit switch
        {
            DistanceUnit.Kilometers => $"{Kilometers.ToString(format)} km",
            DistanceUnit.Miles => $"{Miles.ToString(format)} mi",
            DistanceUnit.Meters => $"{Meters.ToString(format)} m",
            DistanceUnit.NauticalMiles => $"{NauticalMiles.ToString(format)} nm",
            _ => $"{Kilometers.ToString(format)} km"
        };
    }

    /// <summary>
    /// Gets a human-readable description of the distance.
    /// </summary>
    /// <returns>Formatted description string.</returns>
    public string GetDescription()
    {
        return $"Distance from {From.ToCardinalString()} to {To.ToCardinalString()}: " +
               $"{Kilometers:F2} km ({Miles:F2} mi)";
    }

    /// <summary>
    /// Determines if the distance is within a specified radius.
    /// </summary>
    /// <param name="radiusKm">The radius in kilometers.</param>
    /// <returns>True if the distance is within the radius, false otherwise.</returns>
    public bool IsWithinRadius(double radiusKm)
    {
        return Kilometers <= radiusKm;
    }

    /// <summary>
    /// Returns a formatted string representation of the distance.
    /// </summary>
    public override string ToString()
    {
        return ToFormattedString();
    }
}

/// <summary>
/// Represents distance measurement units.
/// </summary>
public enum DistanceUnit
{
    /// <summary>
    /// Kilometers (km).
    /// </summary>
    Kilometers,

    /// <summary>
    /// Miles (mi).
    /// </summary>
    Miles,

    /// <summary>
    /// Meters (m).
    /// </summary>
    Meters,

    /// <summary>
    /// Nautical miles (nm).
    /// </summary>
    NauticalMiles
}
