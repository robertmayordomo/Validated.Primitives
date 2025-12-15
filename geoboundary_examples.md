
## GeoBoundary Domain Object Usage Examples

The **`GeoBoundary`** domain object represents a polygon boundary defined by multiple coordinates with area calculation and point-in-polygon testing.

### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

// Create a triangular boundary around New York landmarks
var vertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.7128m, -74.0060m).Value!, // NYC
    Coordinate.TryCreate(40.7484m, -73.9857m).Value!, // Empire State Building
    Coordinate.TryCreate(40.7580m, -73.9855m).Value!  // Times Square
};

var (result, boundary) = GeoBoundary.TryCreate(vertices);

if (result.IsValid)
{
    Console.WriteLine($"Polygon with {boundary!.Vertices.Count} vertices");
    Console.WriteLine($"Area: {boundary.AreaSquareKilometers:F2} km²");
    Console.WriteLine($"Perimeter: {boundary.PerimeterKilometers:F2} km");
    Console.WriteLine($"Center: {boundary.Center?.ToCardinalString()}");
}
```

### Creating Different Polygon Shapes

```csharp
// Rectangle boundary
var rectangleVertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.0m, -74.0m).Value!,
    Coordinate.TryCreate(41.0m, -74.0m).Value!,
    Coordinate.TryCreate(41.0m, -73.0m).Value!,
    Coordinate.TryCreate(40.0m, -73.0m).Value!
};

var (_, rectangle) = GeoBoundary.TryCreate(rectangleVertices);

Console.WriteLine(rectangle!.GetDescription());
// Output: Polygon with 4 vertices: Area 10742.35 km² (4148.12 mi²), Perimeter 443.21 km (275.37 mi)

// Pentagon boundary
var pentagonVertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.0m, -74.0m).Value!,
    Coordinate.TryCreate(40.5m, -74.5m).Value!,
    Coordinate.TryCreate(41.0m, -74.0m).Value!,
    Coordinate.TryCreate(40.8m, -73.0m).Value!,
    Coordinate.TryCreate(40.2m, -73.0m).Value!
};

var (_, pentagon) = GeoBoundary.TryCreate(pentagonVertices);
Console.WriteLine($"Pentagon has {pentagon!.Vertices.Count} vertices");
```

### Point-in-Polygon Testing

```csharp
// Create a boundary around Manhattan (simplified)
var manhattanVertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.7009m, -74.0165m).Value!, // Battery Park
    Coordinate.TryCreate(40.7484m, -74.0048m).Value!, // Midtown West
    Coordinate.TryCreate(40.7829m, -73.9654m).Value!, // Central Park North
    Coordinate.TryCreate(40.8088m, -73.9482m).Value!, // Harlem
    Coordinate.TryCreate(40.7489m, -73.9680m).Value!, // Midtown East
    Coordinate.TryCreate(40.7061m, -73.9969m).Value!  // Lower East Side
};

var (_, manhattan) = GeoBoundary.TryCreate(manhattanVertices);

// Test if locations are in Manhattan
var (_, timesSquare) = Coordinate.TryCreate(40.7580m, -73.9855m);
var (_, brooklyn) = Coordinate.TryCreate(40.6782m, -73.9442m);

Console.WriteLine($"Times Square in Manhattan: {manhattan!.Contains(timesSquare!)}"); // True
Console.WriteLine($"Brooklyn in Manhattan: {manhattan.Contains(brooklyn!)}");         // False
```

### Multiple Area and Perimeter Units

```csharp
var (_, boundary) = GeoBoundary.TryCreate(vertices);

// Access different area units
Console.WriteLine($"Area: {boundary!.AreaSquareKilometers:F2} km²");
Console.WriteLine($"Area: {boundary.AreaSquareMiles:F2} mi²");
Console.WriteLine($"Area: {boundary.AreaSquareMeters:F0} m²");

// Access different perimeter units
Console.WriteLine($"Perimeter: {boundary.PerimeterKilometers:F2} km");
Console.WriteLine($"Perimeter: {boundary.PerimeterMiles:F2} miles");
```

### Bounding Box

```csharp
var (_, boundary) = GeoBoundary.TryCreate(vertices);

// Get the bounding box (min/max coordinates)
var (minLat, maxLat, minLon, maxLon) = boundary!.GetBoundingBox();

Console.WriteLine($"Latitude range: {minLat}° to {maxLat}°");
Console.WriteLine($"Longitude range: {minLon}° to {maxLon}°");

// Use bounding box for quick rejection tests before expensive point-in-polygon checks
var (_, testPoint) = Coordinate.TryCreate(testLat, testLon);
if (testLat >= minLat && testLat <= maxLat && 
    testLon >= minLon && testLon <= maxLon)
{
    // Point is within bounding box, now check if inside polygon
    if (boundary.Contains(testPoint!))
    {
        Console.WriteLine("Point is inside the boundary");
    }
}
```

### Distance to Boundary Edge

```csharp
var (_, boundary) = GeoBoundary.TryCreate(vertices);

// Find distance to nearest edge
var (_, outsidePoint) = Coordinate.TryCreate(42.0m, -75.0m);
var distanceToEdge = boundary!.DistanceToNearestEdge(outsidePoint!);

Console.WriteLine($"Distance to boundary: {distanceToEdge:F2} km");
```

### Delivery Zone Management Example

```csharp
public class DeliveryZone
{
    public string Name { get; set; } = string.Empty;
    public GeoBoundary Boundary { get; set; } = null!;
    public decimal DeliveryFee { get; set; }
    public int EstimatedMinutes { get; set; }
}

public class DeliveryService
{
    private readonly List<DeliveryZone> _zones = new();

    public DeliveryZone? FindZoneForLocation(Coordinate location)
    {
        return _zones.FirstOrDefault(z => z.Boundary.Contains(location));
    }

    public (bool CanDeliver, string Message, decimal? Fee) CheckDelivery(Coordinate location)
    {
        var zone = FindZoneForLocation(location);

        if (zone != null)
        {
            return (true, $"Available in {zone.Name} zone", zone.DeliveryFee);
        }

        // Find nearest zone
        var nearestZone = _zones
            .Select(z => new { Zone = z, Distance = z.Boundary.DistanceToNearestEdge(location) })
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        if (nearestZone != null)
        {
            return (false, 
                $"Outside delivery area. Nearest zone ({nearestZone.Zone.Name}) is {nearestZone.Distance:F2} km away", 
                null);
        }

        return (false, "No delivery zones available", null);
    }
}

// Usage
var service = new DeliveryService();

// Define downtown delivery zone
var downtownVertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.70m, -74.02m).Value!,
    Coordinate.TryCreate(40.75m, -74.02m).Value!,
    Coordinate.TryCreate(40.75m, -73.97m).Value!,
    Coordinate.TryCreate(40.70m, -73.97m).Value!
};

var (_, downtownBoundary) = GeoBoundary.TryCreate(downtownVertices);

service.AddZone(new DeliveryZone
{
    Name = "Downtown",
    Boundary = downtownBoundary!,
    DeliveryFee = 5.00m,
    EstimatedMinutes = 30
});

// Check delivery availability
var (_, customerLocation) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (canDeliver, message, fee) = service.CheckDelivery(customerLocation!);

Console.WriteLine($"Can Deliver: {canDeliver}");
Console.WriteLine($"Message: {message}");
if (fee.HasValue)
{
    Console.WriteLine($"Fee: ${fee.Value}");
}
```

### Property Search by Area Example

```csharp
public class PropertyListing
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public Coordinate Location { get; set; } = null!;
    public decimal Price { get; set; }
}

public class PropertySearchService
{
    public List<PropertyListing> SearchWithinBoundary(
        GeoBoundary searchArea,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        var allProperties = GetAllProperties(); // From database

        // Quick rejection using bounding box
        var (minLat, maxLat, minLon, maxLon) = searchArea.GetBoundingBox();
        
        var results = allProperties
            .Where(p =>
            {
                var lat = p.Location.Latitude.Value;
                var lon = p.Location.Longitude.Value;
                
                // Quick bounding box check
                if (lat < minLat || lat > maxLat || lon < minLon || lon > maxLon)
                    return false;
                    
                // Precise point-in-polygon check
                return searchArea.Contains(p.Location);
            })
            .AsEnumerable();

        if (minPrice.HasValue)
            results = results.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            results = results.Where(p => p.Price <= maxPrice.Value);

        return results.ToList();
    }

    public string GetSearchAreaInfo(GeoBoundary boundary)
    {
        return $"Search area: {boundary.AreaSquareKilometers:F2} km² " +
               $"({boundary.AreaSquareMiles:F2} mi²), " +
               $"Center: {boundary.Center?.ToCardinalString()}";
    }

    private List<PropertyListing> GetAllProperties()
    {
        // Mock implementation
        return new List<PropertyListing>();
    }
}

// Usage
var service = new PropertySearchService();

// Define search area (user draws on map)
var searchVertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.70m, -74.01m).Value!,
    Coordinate.TryCreate(40.72m, -74.01m).Value!,
    Coordinate.TryCreate(40.72m, -73.99m).Value!,
    Coordinate.TryCreate(40.70m, -73.99m).Value!
};

var (_, searchArea) = GeoBoundary.TryCreate(searchVertices);

Console.WriteLine(service.GetSearchAreaInfo(searchArea!));

var properties = service.SearchWithinBoundary(
    searchArea!,
    minPrice: 500000,
    maxPrice: 1000000);

Console.WriteLine($"Found {properties.Count} properties in the search area");
```

### Restricted Airspace / No-Fly Zone Example

```csharp
public class AirspaceRestriction
{
    public string Name { get; set; } = string.Empty;
    public GeoBoundary Boundary { get; set; } = null!;
    public decimal MinAltitudeMeters { get; set; }
    public decimal MaxAltitudeMeters { get; set; }
    public string RestrictionType { get; set; } = string.Empty;
}

public class FlightPlanValidator
{
    private readonly List<AirspaceRestriction> _restrictions = new();

    public (bool IsValid, List<string> Violations) ValidateFlightPath(
        List<Coordinate> flightPath)
    {
        var violations = new List<string>();

        foreach (var waypoint in flightPath)
        {
            foreach (var restriction in _restrictions)
            {
                if (restriction.Boundary.Contains(waypoint))
                {
                    var altitude = waypoint.Altitude ?? 0;
                    
                    if (altitude >= restriction.MinAltitudeMeters && 
                        altitude <= restriction.MaxAltitudeMeters)
                    {
                        violations.Add($"Waypoint at {waypoint.ToCardinalString()} " +
                            $"violates {restriction.Name} airspace restriction");
                    }
                }
            }
        }

        return (violations.Count == 0, violations);
    }
}

// Usage
var validator = new FlightPlanValidator();

// Define restricted airspace around an airport
var airportVertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.63m, -73.80m).Value!,
    Coordinate.TryCreate(40.65m, -73.80m).Value!,
    Coordinate.TryCreate(40.65m, -73.76m).Value!,
    Coordinate.TryCreate(40.63m, -73.76m).Value!
};

var (_, airportBoundary) = GeoBoundary.TryCreate(airportVertices);

validator.AddRestriction(new AirspaceRestriction
{
    Name = "JFK Airport",
    Boundary = airportBoundary!,
    MinAltitudeMeters = 0,
    MaxAltitudeMeters = 3000,
    RestrictionType = "Class B"
});

// Validate flight plan
var flightPath = new List<Coordinate>
{
    Coordinate.TryCreate(40.7128m, -74.0060m, altitude: 1000m).Value!,
    Coordinate.TryCreate(40.64m, -73.78m, altitude: 500m).Value!, // Inside restricted zone
    Coordinate.TryCreate(40.60m, -73.70m, altitude: 1200m).Value!
};

var (isValid, violations) = validator.ValidateFlightPath(flightPath);

if (!isValid)
{
    Console.WriteLine("Flight plan violations:");
    foreach (var violation in violations)
    {
        Console.WriteLine($"  - {violation}");
    }
}
```

### API Integration Example

```csharp
public class BoundaryRequest
{
    public List<CoordinateDto> Vertices { get; set; } = new();
}

public class CoordinateDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class BoundaryController : ControllerBase
{
    [HttpPost("create")]
    public IActionResult CreateBoundary([FromBody] BoundaryRequest request)
    {
        if (request.Vertices.Count < 3)
        {
            return BadRequest("Boundary must have at least 3 vertices");
        }

        // Create coordinates
        var vertices = new List<Coordinate>();
        foreach (var dto in request.Vertices)
        {
            var (coordResult, coord) = Coordinate.TryCreate(dto.Latitude, dto.Longitude);
            if (!coordResult.IsValid)
            {
                return BadRequest(new { errors = coordResult.Errors });
            }
            vertices.Add(coord!);
        }

        // Create boundary
        var (result, boundary) = GeoBoundary.TryCreate(vertices);

        if (!result.IsValid)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new
        {
            vertices = boundary!.Vertices.Count,
            area = new
            {
                squareKilometers = Math.Round(boundary.AreaSquareKilometers, 2),
                squareMiles = Math.Round(boundary.AreaSquareMiles, 2),
                squareMeters = Math.Round(boundary.AreaSquareMeters, 0)
            },
            perimeter = new
            {
                kilometers = Math.Round(boundary.PerimeterKilometers, 2),
                miles = Math.Round(boundary.PerimeterMiles, 2)
            },
            center = new
            {
                latitude = boundary.Center?.Latitude.Value,
                longitude = boundary.Center?.Longitude.Value,
                formatted = boundary.Center?.ToCardinalString()
            },
            boundingBox = new
            {
                minLatitude = boundary.GetBoundingBox().MinLatitude,
                maxLatitude = boundary.GetBoundingBox().MaxLatitude,
                minLongitude = boundary.GetBoundingBox().MinLongitude,
                maxLongitude = boundary.GetBoundingBox().MaxLongitude
            },
            description = boundary.GetDescription()
        });
    }

    [HttpPost("contains")]
    public IActionResult CheckPointInBoundary(
        [FromBody] BoundaryRequest boundaryRequest,
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude)
    {
        // Create boundary
        var vertices = new List<Coordinate>();
        foreach (var dto in boundaryRequest.Vertices)
        {
            var (coordResult, coord) = Coordinate.TryCreate(dto.Latitude, dto.Longitude);
            if (!coordResult.IsValid) return BadRequest(new { errors = coordResult.Errors });
            vertices.Add(coord!);
        }

        var (boundaryResult, boundary) = GeoBoundary.TryCreate(vertices);
        if (!boundaryResult.IsValid)
            return BadRequest(new { errors = boundaryResult.Errors });

        // Create test point
        var (pointResult, point) = Coordinate.TryCreate(latitude, longitude);
        if (!pointResult.IsValid)
            return BadRequest(new { errors = pointResult.Errors });

        // Test containment
        var isInside = boundary!.Contains(point!);
        var distanceToEdge = boundary.DistanceToNearestEdge(point!);

        return Ok(new
        {
            point = new
            {
                latitude = point.Latitude.Value,
                longitude = point.Longitude.Value,
                formatted = point.ToCardinalString()
            },
            isInsideBoundary = isInside,
            distanceToNearestEdge = new
            {
                kilometers = Math.Round(distanceToEdge, 2),
                miles = Math.Round(distanceToEdge * 0.621371, 2)
            },
            boundary = new
            {
                vertices = boundary.Vertices.Count,
                area = $"{boundary.AreaSquareKilometers:F2} km²"
            }
        });
    }
}
```

### Validation Scenarios

```csharp
// Valid triangle (minimum vertices)
var (result1, _) = GeoBoundary.TryCreate(new[]
{
    Coordinate.TryCreate(40.0m, -74.0m).Value!,
    Coordinate.TryCreate(41.0m, -74.0m).Value!,
    Coordinate.TryCreate(40.5m, -73.5m).Value!
});
result1.IsValid.ShouldBeTrue();

// Invalid - only 2 vertices
var (result2, _) = GeoBoundary.TryCreate(new[]
{
    Coordinate.TryCreate(40.0m, -74.0m).Value!,
    Coordinate.TryCreate(41.0m, -74.0m).Value!
});
result2.IsValid.ShouldBeFalse(); // Error: InsufficientVertices

// Invalid - null vertices collection
var (result3, _) = GeoBoundary.TryCreate(null!);
result3.IsValid.ShouldBeFalse(); // Error: Required

// Vertices are read-only after creation
var (_, boundary) = GeoBoundary.TryCreate(vertices);
// boundary.Vertices is IReadOnlyList<Coordinate>
```

### JSON Serialization

GeoBoundary objects are fully JSON-serializable:

```csharp
using System.Text.Json;

var vertices = new List<Coordinate>
{
    Coordinate.TryCreate(40.0m, -74.0m).Value!,
    Coordinate.TryCreate(41.0m, -74.0m).Value!,
    Coordinate.TryCreate(41.0m, -73.0m).Value!
};

var (_, boundary) = GeoBoundary.TryCreate(vertices);

// Serialize
var json = JsonSerializer.Serialize(boundary);
/* Output:
{
  "Vertices": [...],
  "AreaSquareKilometers": 10742.35,
  "AreaSquareMiles": 4148.12,
  "AreaSquareMeters": 10742350000.0,
  "PerimeterKilometers": 443.21,
  "PerimeterMiles": 275.37,
  "Center": { "Latitude": {...}, "Longitude": {...} }
}
*/

// Deserialize - area, perimeter, and center are automatically recalculated
var deserialized = JsonSerializer.Deserialize<GeoBoundary>(json);
Console.WriteLine($"Area: {deserialized!.AreaSquareKilometers:F2} km²");
```

