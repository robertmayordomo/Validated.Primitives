
## GeoDistance Domain Object Usage Examples

The **`GeoDistance`** domain object calculates the great-circle distance between two geographic coordinates using the Haversine formula.

### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

// Create two coordinates
var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, losAngeles) = Coordinate.TryCreate(34.0522m, -118.2437m);

// Calculate distance
var (result, distance) = GeoDistance.TryCreate(newYork!, losAngeles!);

if (result.IsValid)
{
    Console.WriteLine($"Distance: {distance!.Kilometers:F2} km");        // 3944.42 km
    Console.WriteLine($"Distance: {distance.Miles:F2} miles");          // 2451.03 miles
    Console.WriteLine($"Distance: {distance.Meters:F0} meters");        // 3944422 meters
    Console.WriteLine($"Distance: {distance.NauticalMiles:F2} nm");     // 2129.83 nm
}
```

### Multiple Distance Units

```csharp
var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, to) = Coordinate.TryCreate(51.5074m, -0.1278m); // London

var (_, distance) = GeoDistance.TryCreate(from!, to!);

// Access different units
Console.WriteLine(distance!.Kilometers);      // 5570.22 km
Console.WriteLine(distance.Miles);            // 3461.67 miles
Console.WriteLine(distance.Meters);           // 5570220.0 meters
Console.WriteLine(distance.NauticalMiles);    // 3007.57 nautical miles
```

### Formatted Output

```csharp
var (_, distance) = GeoDistance.TryCreate(newYork!, losAngeles!);

// Format with different units and precision
Console.WriteLine(distance!.ToFormattedString(DistanceUnit.Kilometers, 2));  // "3944.42 km"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Miles, 1));        // "2451.0 mi"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Meters, 0));       // "3944422 m"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.NauticalMiles, 2)); // "2129.83 nm"

// Default formatting
Console.WriteLine(distance.ToString()); // "3944.42 km"
```

### Human-Readable Descriptions

```csharp
var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m, decimalPlaces: 4);
var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m, decimalPlaces: 4);
var (_, distance) = GeoDistance.TryCreate(from!, to!);

Console.WriteLine(distance!.GetDescription());
// Output: Distance from 40.7128° N, 74.0060° W to 34.0522° N, 118.2437° W: 3944.42 km (2451.03 mi)
```

### Geofencing / Radius Checking

```csharp
var (_, cityCenter) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, location1) = Coordinate.TryCreate(40.7484m, -73.9857m); // ~4 km away
var (_, location2) = Coordinate.TryCreate(34.0522m, -118.2437m); // ~3944 km away

var (_, distance1) = GeoDistance.TryCreate(cityCenter!, location1!);
var (_, distance2) = GeoDistance.TryCreate(cityCenter!, location2!);

// Check if within 5km radius
Console.WriteLine(distance1!.IsWithinRadius(5.0));  // True
Console.WriteLine(distance2!.IsWithinRadius(5.0));  // False
```

### Proximity Service Example

```csharp
public class ProximityService
{
    public class NearbyLocation
    {
        public string Name { get; set; } = string.Empty;
        public Coordinate Position { get; set; } = null!;
        public GeoDistance Distance { get; set; } = null!;
    }

    public List<NearbyLocation> FindLocationsWithinRadius(
        Coordinate center,
        List<(string Name, Coordinate Position)> locations,
        double radiusKm)
    {
        var nearbyLocations = new List<NearbyLocation>();

        foreach (var (name, position) in locations)
        {
            var (result, distance) = GeoDistance.TryCreate(center, position);
            
            if (result.IsValid && distance!.IsWithinRadius(radiusKm))
            {
                nearbyLocations.Add(new NearbyLocation
                {
                    Name = name,
                    Position = position,
                    Distance = distance
                });
            }
        }

        // Sort by distance
        return nearbyLocations.OrderBy(l => l.Distance.Kilometers).ToList();
    }
}

// Usage
var service = new ProximityService();
var (_, currentLocation) = Coordinate.TryCreate(40.7128m, -74.0060m);

var locations = new List<(string, Coordinate)>
{
    ("Empire State Building", Coordinate.TryCreate(40.7484m, -73.9857m).Value!),
    ("Times Square", Coordinate.TryCreate(40.7580m, -73.9855m).Value!),
    ("Central Park", Coordinate.TryCreate(40.7829m, -73.9654m).Value!),
    ("Los Angeles", Coordinate.TryCreate(34.0522m, -118.2437m).Value!)
};

var nearby = service.FindLocationsWithinRadius(currentLocation!, locations, 10.0);

foreach (var location in nearby)
{
    Console.WriteLine($"{location.Name}: {location.Distance.ToFormattedString(DistanceUnit.Kilometers, 2)}");
}
// Output:
// Empire State Building: 4.23 km
// Times Square: 5.12 km
// Central Park: 7.89 km
```

### Distance Matrix Example

```csharp
public class DistanceMatrix
{
    public static Dictionary<(string From, string To), GeoDistance> Calculate(
        Dictionary<string, Coordinate> locations)
    {
        var matrix = new Dictionary<(string, string), GeoDistance>();

        var locationList = locations.ToList();
        for (int i = 0; i < locationList.Count; i++)
        {
            for (int j = i + 1; j < locationList.Count; j++)
            {
                var from = locationList[i];
                var to = locationList[j];

                var (result, distance) = GeoDistance.TryCreate(from.Value, to.Value);
                if (result.IsValid)
                {
                    matrix[(from.Key, to.Key)] = distance!;
                    matrix[(to.Key, from.Key)] = distance!; // Symmetric
                }
            }
        }

        return matrix;
    }
}

// Usage
var cities = new Dictionary<string, Coordinate>
{
    ["New York"] = Coordinate.TryCreate(40.7128m, -74.0060m).Value!,
    ["Los Angeles"] = Coordinate.TryCreate(34.0522m, -118.2437m).Value!,
    ["Chicago"] = Coordinate.TryCreate(41.8781m, -87.6298m).Value!,
    ["London"] = Coordinate.TryCreate(51.5074m, -0.1278m).Value!
};

var matrix = DistanceMatrix.Calculate(cities);

Console.WriteLine($"New York to Los Angeles: {matrix[("New York", "Los Angeles")].ToFormattedString()}");
Console.WriteLine($"New York to Chicago: {matrix[("New York", "Chicago")].ToFormattedString()}");
Console.WriteLine($"New York to London: {matrix[("New York", "London")].ToFormattedString()}");
```

### API Integration Example

```csharp
public class DistanceRequest
{
    public decimal FromLatitude { get; set; }
    public decimal FromLongitude { get; set; }
    public decimal ToLatitude { get; set; }
    public decimal ToLongitude { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class DistanceController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult CalculateDistance([FromBody] DistanceRequest request)
    {
        // Validate and create coordinates
        var (fromResult, from) = Coordinate.TryCreate(
            request.FromLatitude,
            request.FromLongitude);

        if (!fromResult.IsValid)
            return BadRequest(new { errors = fromResult.Errors });

        var (toResult, to) = Coordinate.TryCreate(
            request.ToLatitude,
            request.ToLongitude);

        if (!toResult.IsValid)
            return BadRequest(new { errors = toResult.Errors });

        // Calculate distance
        var (distResult, distance) = GeoDistance.TryCreate(from!, to!);

        if (!distResult.IsValid)
            return BadRequest(new { errors = distResult.Errors });

        return Ok(new
        {
            from = new
            {
                latitude = from.Latitude.Value,
                longitude = from.Longitude.Value,
                formatted = from.ToCardinalString()
            },
            to = new
            {
                latitude = to.Latitude.Value,
                longitude = to.Longitude.Value,
                formatted = to.ToCardinalString()
            },
            distance = new
            {
                kilometers = Math.Round(distance!.Kilometers, 2),
                miles = Math.Round(distance.Miles, 2),
                meters = Math.Round(distance.Meters, 0),
                nauticalMiles = Math.Round(distance.NauticalMiles, 2),
                formatted = distance.ToFormattedString()
            },
            description = distance.GetDescription()
        });
    }

    [HttpGet("nearby")]
    public IActionResult FindNearby(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude,
        [FromQuery] double radiusKm = 10.0)
    {
        var (result, center) = Coordinate.TryCreate(latitude, longitude);

        if (!result.IsValid)
            return BadRequest(new { errors = result.Errors });

        // Get locations from database (example)
        var locations = GetLocationsFromDatabase();

        var nearby = new List<object>();
        foreach (var location in locations)
        {
            var (distResult, distance) = GeoDistance.TryCreate(center!, location.Position);

            if (distResult.IsValid && distance!.IsWithinRadius(radiusKm))
            {
                nearby.Add(new
                {
                    name = location.Name,
                    position = location.Position.ToCardinalString(),
                    distance = new
                    {
                        kilometers = Math.Round(distance.Kilometers, 2),
                        miles = Math.Round(distance.Miles, 2),
                        formatted = distance.ToFormattedString()
                    }
                });
            }
        }

        return Ok(new
        {
            center = center.ToCardinalString(),
            radiusKm,
            count = nearby.Count,
            locations = nearby.OrderBy(l => ((dynamic)l).distance.kilometers)
        });
    }

    private List<(string Name, Coordinate Position)> GetLocationsFromDatabase()
    {
        // Mock implementation
        return new List<(string, Coordinate)>();
    }
}
```

### Travel Distance Calculator

```csharp
public class TravelDistanceCalculator
{
    public class TravelSegment
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public GeoDistance Distance { get; set; } = null!;
    }

    public class TravelRoute
    {
        public List<TravelSegment> Segments { get; set; } = new();
        public double TotalKilometers => Segments.Sum(s => s.Distance.Kilometers);
        public double TotalMiles => Segments.Sum(s => s.Distance.Miles);

        public string GetSummary()
        {
            var summary = $"Total Distance: {TotalKilometers:F2} km ({TotalMiles:F2} mi)\n";
            summary += "Route:\n";
            
            foreach (var segment in Segments)
            {
                summary += $"  {segment.From} ? {segment.To}: " +
                          $"{segment.Distance.ToFormattedString(DistanceUnit.Kilometers, 2)}\n";
            }

            return summary;
        }
    }

    public (ValidationResult Result, TravelRoute? Route) CalculateRoute(
        List<(string Name, Coordinate Position)> waypoints)
    {
        var result = ValidationResult.Success();

        if (waypoints.Count < 2)
        {
            result.AddError("At least two waypoints are required.", "Waypoints", "InsufficientWaypoints");
            return (result, null);
        }

        var route = new TravelRoute();

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var from = waypoints[i];
            var to = waypoints[i + 1];

            var (distResult, distance) = GeoDistance.TryCreate(from.Position, to.Position);

            if (!distResult.IsValid)
            {
                result.Merge(distResult);
                continue;
            }

            route.Segments.Add(new TravelSegment
            {
                From = from.Name,
                To = to.Name,
                Distance = distance!
            });
        }

        return (result, result.IsValid ? route : null);
    }
}

// Usage - Plan a road trip
var calculator = new TravelDistanceCalculator();

var roadTrip = new List<(string, Coordinate)>
{
    ("New York", Coordinate.TryCreate(40.7128m, -74.0060m).Value!),
    ("Philadelphia", Coordinate.TryCreate(39.9526m, -75.1652m).Value!),
    ("Washington DC", Coordinate.TryCreate(38.9072m, -77.0369m).Value!),
    ("Atlanta", Coordinate.TryCreate(33.7490m, -84.3880m).Value!),
    ("Miami", Coordinate.TryCreate(25.7617m, -80.1918m).Value!)
};

var (result, route) = calculator.CalculateRoute(roadTrip);

if (result.IsValid)
{
    Console.WriteLine(route!.GetSummary());
}
// Output:
// Total Distance: 2156.34 km (1339.82 mi)
// Route:
//   New York ? Philadelphia: 129.63 km
//   Philadelphia ? Washington DC: 199.42 km
//   Washington DC ? Atlanta: 872.18 km
//   Atlanta ? Miami: 955.11 km
```

### Constants Reference

```csharp
// Earth radius constants for reference
Console.WriteLine($"Earth Radius (km): {GeoDistance.EarthRadiusKm}");        // 6371.0
Console.WriteLine($"Earth Radius (mi): {GeoDistance.EarthRadiusMiles}");     // 3958.8

// Use these for custom calculations if needed
var circumferenceKm = 2 * Math.PI * GeoDistance.EarthRadiusKm;  // ~40,030 km
var circumferenceMi = 2 * Math.PI * GeoDistance.EarthRadiusMiles; // ~24,874 mi
```

### JSON Serialization

GeoDistance objects are fully JSON-serializable:

```csharp
using System.Text.Json;

var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
var (_, distance) = GeoDistance.TryCreate(from!, to!);

// Serialize
var json = JsonSerializer.Serialize(distance);
/* Output:
{
  "From": {
    "Latitude": { "Value": 40.7128, "DecimalPlaces": 6 },
    "Longitude": { "Value": -74.0060, "DecimalPlaces": 6 }
  },
  "To": {
    "Latitude": { "Value": 34.0522, "DecimalPlaces": 6 },
    "Longitude": { "Value": -118.2437, "DecimalPlaces": 6 }
  },
  "Kilometers": 3944.42,
  "Miles": 2451.03,
  "Meters": 3944422.0,
  "NauticalMiles": 2129.83
}
*/

// Deserialize - distance is automatically recalculated from coordinates
var deserialized = JsonSerializer.Deserialize<GeoDistance>(json);
Console.WriteLine($"Distance: {deserialized!.Kilometers:F2} km");
```

### Validation Scenarios

```csharp
// Valid distance calculation
var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, to) = Coordinate.TryCreate(34.0522m, -118.2437m);
var (result1, distance) = GeoDistance.TryCreate(from!, to!);
result1.IsValid.ShouldBeTrue();

// Invalid - null from coordinate
var (result2, _) = GeoDistance.TryCreate(null!, to!);
result2.IsValid.ShouldBeFalse(); // Error: From coordinate required

// Invalid - null to coordinate
var (result3, _) = GeoDistance.TryCreate(from!, null!);
result3.IsValid.ShouldBeFalse(); // Error: To coordinate required

// Zero distance (same coordinates)
var (_, sameCoord) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (result4, zeroDistance) = GeoDistance.TryCreate(from!, sameCoord!);
result4.IsValid.ShouldBeTrue();
zeroDistance!.Kilometers.ShouldBe(0, tolerance: 0.001);
```
