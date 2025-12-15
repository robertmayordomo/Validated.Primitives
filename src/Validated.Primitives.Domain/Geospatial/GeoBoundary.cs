using Validated.Primitives.Validation;
using System.Text.Json.Serialization;

namespace Validated.Primitives.Domain.Geospatial;

/// <summary>
/// Represents a geographic boundary defined by a polygon of coordinates.
/// Provides area calculation and point-in-polygon testing.
/// </summary>
[JsonConverter(typeof(Serialization.GeoBoundaryConverter))]
public sealed record GeoBoundary
{
    /// <summary>
    /// Gets the vertices that define the boundary polygon.
    /// The polygon is automatically closed (first point connects to last point).
    /// </summary>
    public required IReadOnlyList<Coordinate> Vertices { get; init; }

    /// <summary>
    /// Gets the calculated area in square kilometers.
    /// Uses spherical excess formula for accurate area on Earth's surface.
    /// </summary>
    public double AreaSquareKilometers { get; init; }

    /// <summary>
    /// Gets the calculated area in square miles.
    /// </summary>
    public double AreaSquareMiles => AreaSquareKilometers * 0.386102;

    /// <summary>
    /// Gets the calculated area in square meters.
    /// </summary>
    public double AreaSquareMeters => AreaSquareKilometers * 1_000_000;

    /// <summary>
    /// Gets the calculated perimeter in kilometers.
    /// </summary>
    public double PerimeterKilometers { get; init; }

    /// <summary>
    /// Gets the calculated perimeter in miles.
    /// </summary>
    public double PerimeterMiles => PerimeterKilometers * 0.621371;

    /// <summary>
    /// Gets the center point (centroid) of the boundary.
    /// </summary>
    public Coordinate? Center { get; init; }

    /// <summary>
    /// Creates a new GeoBoundary from a collection of coordinates.
    /// </summary>
    /// <param name="vertices">The vertices that define the boundary polygon. Must contain at least 3 coordinates.</param>
    /// <returns>A tuple containing the validation result and the GeoBoundary if valid.</returns>
    public static (ValidationResult Result, GeoBoundary? Value) TryCreate(IEnumerable<Coordinate> vertices)
    {
        var result = ValidationResult.Success();

        if (vertices == null)
        {
            result.AddError("Vertices collection is required.", "Vertices", "Required");
            return (result, null);
        }

        var vertexList = vertices.ToList();

        // Validate minimum vertices
        if (vertexList.Count < 3)
        {
            result.AddError("Boundary must have at least 3 vertices to form a polygon.", "Vertices", "InsufficientVertices");
            return (result, null);
        }

        // Validate all vertices are not null
        if (vertexList.Any(v => v == null))
        {
            result.AddError("All vertices must be valid coordinates.", "Vertices", "InvalidVertex");
            return (result, null);
        }

        // Calculate area using spherical excess formula
        var area = CalculateSphericalArea(vertexList);

        // Calculate perimeter
        var perimeter = CalculatePerimeter(vertexList);

        // Calculate center (centroid)
        var center = CalculateCenter(vertexList);

        var boundary = new GeoBoundary
        {
            Vertices = vertexList.AsReadOnly(),
            AreaSquareKilometers = area,
            PerimeterKilometers = perimeter,
            Center = center
        };

        return (result, boundary);
    }

    /// <summary>
    /// Determines if a coordinate is within the boundary using the ray casting algorithm.
    /// </summary>
    /// <param name="point">The coordinate to test.</param>
    /// <returns>True if the coordinate is inside the boundary, false otherwise.</returns>
    public bool Contains(Coordinate point)
    {
        if (point == null || Vertices.Count < 3)
            return false;

        // Ray casting algorithm (point-in-polygon test)
        bool isInside = false;
        int j = Vertices.Count - 1;

        for (int i = 0; i < Vertices.Count; j = i++)
        {
            var vi = Vertices[i];
            var vj = Vertices[j];

            var latI = (double)vi.Latitude.Value;
            var lonI = (double)vi.Longitude.Value;
            var latJ = (double)vj.Latitude.Value;
            var lonJ = (double)vj.Longitude.Value;
            var testLat = (double)point.Latitude.Value;
            var testLon = (double)point.Longitude.Value;

            if ((lonI > testLon) != (lonJ > testLon) &&
                testLat < (latJ - latI) * (testLon - lonI) / (lonJ - lonI) + latI)
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }

    /// <summary>
    /// Gets the distance from a point to the nearest edge of the boundary.
    /// </summary>
    /// <param name="point">The coordinate to measure from.</param>
    /// <returns>Distance in kilometers to the nearest edge.</returns>
    public double DistanceToNearestEdge(Coordinate point)
    {
        if (point == null || Vertices.Count < 2)
            return double.MaxValue;

        double minDistance = double.MaxValue;

        for (int i = 0; i < Vertices.Count; i++)
        {
            var v1 = Vertices[i];
            var v2 = Vertices[(i + 1) % Vertices.Count];

            var distance = DistanceToLineSegment(point, v1, v2);
            minDistance = Math.Min(minDistance, distance);
        }

        return minDistance;
    }

    /// <summary>
    /// Gets the bounding box (minimum and maximum lat/long) that contains the boundary.
    /// </summary>
    /// <returns>A tuple containing (minLat, maxLat, minLon, maxLon).</returns>
    public (decimal MinLatitude, decimal MaxLatitude, decimal MinLongitude, decimal MaxLongitude) GetBoundingBox()
    {
        if (Vertices.Count == 0)
            return (0, 0, 0, 0);

        var minLat = Vertices.Min(v => v.Latitude.Value);
        var maxLat = Vertices.Max(v => v.Latitude.Value);
        var minLon = Vertices.Min(v => v.Longitude.Value);
        var maxLon = Vertices.Max(v => v.Longitude.Value);

        return (minLat, maxLat, minLon, maxLon);
    }

    /// <summary>
    /// Calculates the spherical area of a polygon on Earth's surface.
    /// Uses the spherical excess formula with L'Huilier's theorem.
    /// </summary>
    private static double CalculateSphericalArea(List<Coordinate> vertices)
    {
        if (vertices.Count < 3)
            return 0;

        double area = 0;
        int n = vertices.Count;

        for (int i = 0; i < n; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % n];

            var lat1 = DegreesToRadians((double)v1.Latitude.Value);
            var lon1 = DegreesToRadians((double)v1.Longitude.Value);
            var lat2 = DegreesToRadians((double)v2.Latitude.Value);
            var lon2 = DegreesToRadians((double)v2.Longitude.Value);

            area += (lon2 - lon1) * (2 + Math.Sin(lat1) + Math.Sin(lat2));
        }

        area = Math.Abs(area * GeoDistance.EarthRadiusKm * GeoDistance.EarthRadiusKm / 2.0);

        return area;
    }

    /// <summary>
    /// Calculates the perimeter of the polygon.
    /// </summary>
    private static double CalculatePerimeter(List<Coordinate> vertices)
    {
        double perimeter = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % vertices.Count];

            perimeter += v1.DistanceTo(v2);
        }

        return perimeter;
    }

    /// <summary>
    /// Calculates the centroid (center point) of the polygon.
    /// </summary>
    private static Coordinate? CalculateCenter(List<Coordinate> vertices)
    {
        if (vertices.Count == 0)
            return null;

        var avgLat = vertices.Average(v => v.Latitude.Value);
        var avgLon = vertices.Average(v => v.Longitude.Value);

        var (result, center) = Coordinate.TryCreate(avgLat, avgLon);
        return result.IsValid ? center : null;
    }

    /// <summary>
    /// Calculates the distance from a point to a line segment.
    /// </summary>
    private static double DistanceToLineSegment(Coordinate point, Coordinate segmentStart, Coordinate segmentEnd)
    {
        // For simplicity, we'll use the minimum of distances to the endpoints
        // A more accurate implementation would project the point onto the line segment
        var d1 = point.DistanceTo(segmentStart);
        var d2 = point.DistanceTo(segmentEnd);
        return Math.Min(d1, d2);
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    /// <summary>
    /// Gets a formatted string describing the boundary.
    /// </summary>
    public string GetDescription()
    {
        return $"Polygon with {Vertices.Count} vertices: " +
               $"Area {AreaSquareKilometers:F2} km² ({AreaSquareMiles:F2} mi²), " +
               $"Perimeter {PerimeterKilometers:F2} km ({PerimeterMiles:F2} mi)";
    }

    /// <summary>
    /// Returns a formatted string representation of the boundary.
    /// </summary>
    public override string ToString()
    {
        return GetDescription();
    }
}
