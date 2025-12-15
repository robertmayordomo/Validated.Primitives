
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

---

## GeospatialRoute Domain Object Usage Examples

The **`GeospatialRoute`** domain object represents a complete route composed of multiple contiguous segments, where each segment connects to the next forming a continuous path.

### Basic Route Creation

```csharp
using Validated.Primitives.Domain.Geospatial;

// Create coordinates for a simple route
var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m); // Lower Manhattan
var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m); // Empire State Building
var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m); // Times Square

// Create segments
var (_, segment1) = RouteSegment.TryCreate(point1, point2, "To Empire State");
var (_, segment2) = RouteSegment.TryCreate(point2, point3, "To Times Square");

// Create route
var (result, route) = GeospatialRoute.TryCreate(
    new[] { segment1!, segment2! }, 
    "NYC Walking Tour");

if (result.IsValid)
{
    Console.WriteLine(route!.GetDescription());
    // Output: NYC Walking Tour: 2 segments, Total distance: 5.23 km (3.25 mi)
    //         From: 40.712800° N, 74.006000° W
    //         To: 40.758000° N, 73.985500° W
}
```

### Using RouteSegmentBuilder and GeospatialRouteBuilder

```csharp
using Validated.Primitives.Domain.Geospatial.Builders;

// Create coordinates
var (_, nyc) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, philly) = Coordinate.TryCreate(39.9526m, -75.1652m);
var (_, dc) = Coordinate.TryCreate(38.9072m, -77.0369m);

// Build route using fluent builder
var (result, route) = new GeospatialRouteBuilder()
    .WithName("East Coast Tour")
    .AddSegment(nyc, philly, "NYC to Philadelphia")
    .AddSegment(philly, dc, "Philadelphia to DC")
    .Build();

if (result.IsValid)
{
    Console.WriteLine($"Total Distance: {route!.TotalDistanceKilometers:F2} km");
    Console.WriteLine($"Total Distance: {route.TotalDistanceMiles:F2} miles");
    Console.WriteLine($"Segments: {route.Segments.Count}");
}
```

### Contiguous Segment Validation

The route validates that all segments are contiguous (each segment's To coordinate must match the next segment's From coordinate):

```csharp
// Valid contiguous segments
var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);

var (_, seg1) = RouteSegment.TryCreate(point1, point2);
var (_, seg2) = RouteSegment.TryCreate(point2, point3); // Starts where seg1 ends ?

var (result1, route1) = GeospatialRoute.TryCreate(new[] { seg1!, seg2! });
result1.IsValid.ShouldBeTrue();

// Invalid non-contiguous segments
var (_, point4) = Coordinate.TryCreate(41.0000m, -74.0000m);
var (_, seg3) = RouteSegment.TryCreate(point1, point2);
var (_, seg4) = RouteSegment.TryCreate(point4, point3); // Gap! Doesn't start where seg3 ends ?

var (result2, route2) = GeospatialRoute.TryCreate(new[] { seg3!, seg4! });
result2.IsValid.ShouldBeFalse(); // Error: NonContiguousSegments
```

### Accessing Route Information

```csharp
var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, point3) = Coordinate.TryCreate(40.7580m, -73.9855m);
var (_, point4) = Coordinate.TryCreate(40.7829m, -73.9654m);

var (_, segment1) = RouteSegment.TryCreate(point1, point2, "Leg 1");
var (_, segment2) = RouteSegment.TryCreate(point2, point3, "Leg 2");
var (_, segment3) = RouteSegment.TryCreate(point3, point4, "Leg 3");

var (_, route) = GeospatialRoute.TryCreate(new[] { segment1!, segment2!, segment3! });

// Total distance
Console.WriteLine($"Total: {route!.TotalDistanceKilometers:F2} km");
Console.WriteLine($"Total: {route.TotalDistanceMiles:F2} miles");
Console.WriteLine($"Total: {route.TotalDistanceMeters:F0} meters");

// Start and end points
Console.WriteLine($"Start: {route.StartingPoint!.ToCardinalString()}");
Console.WriteLine($"End: {route.EndingPoint!.ToCardinalString()}");

// All waypoints
var waypoints = route.GetWaypoints();
Console.WriteLine($"Waypoints: {waypoints.Count}"); // 4 points

// Segment distances
for (int i = 0; i < route.Segments.Count; i++)
{
    var segment = route.Segments[i];
    Console.WriteLine($"{segment.Name}: {segment.Distance.ToFormattedString()}");
}

// Cumulative distance
for (int i = 0; i < route.Segments.Count; i++)
{
    var cumulative = route.GetCumulativeDistance(i);
    Console.WriteLine($"After segment {i + 1}: {cumulative:F2} km");
}
```

### Multi-City Road Trip Example

```csharp
public class RoadTripPlanner
{
    public (ValidationResult Result, GeospatialRoute? Route) PlanTrip(
        List<(string City, decimal Lat, decimal Lon)> stops,
        string tripName)
    {
        var builder = new GeospatialRouteBuilder().WithName(tripName);

        for (int i = 0; i < stops.Count - 1; i++)
        {
            var from = stops[i];
            var to = stops[i + 1];

            var (_, fromCoord) = Coordinate.TryCreate(from.Lat, from.Lon);
            var (_, toCoord) = Coordinate.TryCreate(to.Lat, to.Lon);

            builder.AddSegment(fromCoord, toCoord, $"{from.City} to {to.City}");
        }

        return builder.Build();
    }
}

// Usage
var planner = new RoadTripPlanner();

var eastCoastTrip = new List<(string, decimal, decimal)>
{
    ("New York", 40.7128m, -74.0060m),
    ("Philadelphia", 39.9526m, -75.1652m),
    ("Washington DC", 38.9072m, -77.0369m),
    ("Atlanta", 33.7490m, -84.3880m),
    ("Miami", 25.7617m, -80.1918m)
};

var (result, route) = planner.PlanTrip(eastCoastTrip, "East Coast Road Trip");

if (result.IsValid)
{
    Console.WriteLine(route!.GetDescription());
    Console.WriteLine();
    
    foreach (var segment in route.Segments)
    {
        Console.WriteLine($"  {segment.GetDescription()}");
    }
    
    Console.WriteLine();
    Console.WriteLine($"Total trip: {route.TotalDistanceMiles:F0} miles");
}
```

### Delivery Route Optimization Example

```csharp
public class DeliveryRoute
{
    public string DriverName { get; set; } = string.Empty;
    public GeospatialRoute Route { get; set; } = null!;
    public DateTime ScheduledStart { get; set; }
    public List<string> DeliveryAddresses { get; set; } = new();

    public double EstimatedDurationMinutes(double averageSpeedKmh = 50.0)
    {
        return (Route.TotalDistanceKilometers / averageSpeedKmh) * 60;
    }

    public DateTime EstimatedCompletionTime(double averageSpeedKmh = 50.0)
    {
        return ScheduledStart.AddMinutes(EstimatedDurationMinutes(averageSpeedKmh));
    }

    public string GetSummary()
    {
        var summary = $"Driver: {DriverName}\n";
        summary += $"Stops: {DeliveryAddresses.Count}\n";
        summary += $"Total Distance: {Route.TotalDistanceKilometers:F2} km ({Route.TotalDistanceMiles:F2} mi)\n";
        summary += $"Estimated Duration: {EstimatedDurationMinutes():F0} minutes\n";
        summary += $"Scheduled Start: {ScheduledStart:HH:mm}\n";
        summary += $"Estimated Completion: {EstimatedCompletionTime():HH:mm}\n";
        return summary;
    }
}

// Usage
var (_, warehouse) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, stop1) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, stop2) = Coordinate.TryCreate(40.7580m, -73.9855m);
var (_, stop3) = Coordinate.TryCreate(40.7829m, -73.9654m);

var (_, deliveryRoute) = new GeospatialRouteBuilder()
    .WithName("Morning Deliveries")
    .AddSegment(warehouse, stop1, "Warehouse to Stop 1")
    .AddSegment(stop1, stop2, "Stop 1 to Stop 2")
    .AddSegment(stop2, stop3, "Stop 2 to Stop 3")
    .AddSegment(stop3, warehouse, "Stop 3 back to Warehouse")
    .Build();

var delivery = new DeliveryRoute
{
    DriverName = "John Smith",
    Route = deliveryRoute!,
    ScheduledStart = DateTime.Today.AddHours(8),
    DeliveryAddresses = new List<string> { "123 Main St", "456 Oak Ave", "789 Elm St" }
};

Console.WriteLine(delivery.GetSummary());
```

### GPS Tracking and Route Progress

```csharp
public class RouteProgress
{
    private readonly GeospatialRoute _route;
    private int _currentSegmentIndex;

    public RouteProgress(GeospatialRoute route)
    {
        _route = route;
        _currentSegmentIndex = 0;
    }

    public RouteSegment? CurrentSegment => 
        _currentSegmentIndex < _route.Segments.Count 
            ? _route.Segments[_currentSegmentIndex] 
            : null;

    public double CompletedDistanceKm => 
        _currentSegmentIndex > 0 
            ? _route.GetCumulativeDistance(_currentSegmentIndex - 1) ?? 0 
            : 0;

    public double RemainingDistanceKm => 
        _route.TotalDistanceKilometers - CompletedDistanceKm;

    public double ProgressPercentage => 
        (_route.TotalDistanceKilometers > 0)
            ? (CompletedDistanceKm / _route.TotalDistanceKilometers) * 100
            : 0;

    public bool MoveToNextSegment()
    {
        if (_currentSegmentIndex < _route.Segments.Count - 1)
        {
            _currentSegmentIndex++;
            return true;
        }
        return false;
    }

    public bool IsComplete => _currentSegmentIndex >= _route.Segments.Count;

    public string GetProgressReport()
    {
        return $"Progress: {ProgressPercentage:F1}%\n" +
               $"Completed: {CompletedDistanceKm:F2} km\n" +
               $"Remaining: {RemainingDistanceKm:F2} km\n" +
               $"Current Segment: {CurrentSegment?.Name ?? "Complete"}";
    }
}

// Usage - Track delivery progress
var (_, route) = new GeospatialRouteBuilder()
    .WithName("Delivery Route")
    .AddSegment(warehouse, stop1, "To Customer A")
    .AddSegment(stop1, stop2, "To Customer B")
    .AddSegment(stop2, stop3, "To Customer C")
    .Build();

var progress = new RouteProgress(route!);

Console.WriteLine(progress.GetProgressReport());
// Progress: 0.0%
// Completed: 0.00 km
// Remaining: 15.32 km
// Current Segment: To Customer A

progress.MoveToNextSegment();
Console.WriteLine(progress.GetProgressReport());
// Progress: 29.2%
// Completed: 4.47 km
// Remaining: 10.85 km
// Current Segment: To Customer B
```

### API Integration Example

```csharp
public class RouteRequest
{
    public string? RouteName { get; set; }
    public List<WaypointDto> Waypoints { get; set; } = new();
}

public class WaypointDto
{
    public string? Name { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateRoute([FromBody] RouteRequest request)
    {
        if (request.Waypoints.Count < 2)
        {
            return BadRequest(new { error = "At least 2 waypoints are required" });
        }

        var builder = new GeospatialRouteBuilder();
        
        if (!string.IsNullOrEmpty(request.RouteName))
        {
            builder.WithName(request.RouteName);
        }

        // Build segments from waypoints
        for (int i = 0; i < request.Waypoints.Count - 1; i++)
        {
            var from = request.Waypoints[i];
            var to = request.Waypoints[i + 1];

            var (fromResult, fromCoord) = Coordinate.TryCreate(from.Latitude, from.Longitude);
            var (toResult, toCoord) = Coordinate.TryCreate(to.Latitude, to.Longitude);

            if (!fromResult.IsValid)
                return BadRequest(new { errors = fromResult.Errors });
            if (!toResult.IsValid)
                return BadRequest(new { errors = toResult.Errors });

            var segmentName = !string.IsNullOrEmpty(from.Name) && !string.IsNullOrEmpty(to.Name)
                ? $"{from.Name} to {to.Name}"
                : null;

            builder.AddSegment(fromCoord, toCoord, segmentName);
        }

        var (result, route) = builder.Build();

        if (!result.IsValid)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new
        {
            routeName = route!.Name,
            segmentCount = route.Segments.Count,
            totalDistance = new
            {
                kilometers = Math.Round(route.TotalDistanceKilometers, 2),
                miles = Math.Round(route.TotalDistanceMiles, 2),
                meters = Math.Round(route.TotalDistanceMeters, 0)
            },
            startingPoint = route.StartingPoint!.ToCardinalString(),
            endingPoint = route.EndingPoint!.ToCardinalString(),
            segments = route.Segments.Select((s, i) => new
            {
                index = i,
                name = s.Name,
                from = s.From.ToCardinalString(),
                to = s.To.ToCardinalString(),
                distance = new
                {
                    kilometers = Math.Round(s.Distance.Kilometers, 2),
                    miles = Math.Round(s.Distance.Miles, 2)
                },
                cumulativeKm = Math.Round(route.GetCumulativeDistance(i) ?? 0, 2)
            })
        });
    }
}
```

### JSON Serialization

GeospatialRoute objects are fully JSON-serializable:

```csharp
using System.Text.Json;

var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, segment) = RouteSegment.TryCreate(point1, point2, "Downtown");
var (_, route) = GeospatialRoute.TryCreate(new[] { segment! }, "City Tour");

// Serialize
var json = JsonSerializer.Serialize(route, new JsonSerializerOptions { WriteIndented = true });

// Deserialize - distances are automatically recalculated
var deserialized = JsonSerializer.Deserialize<GeospatialRoute>(json);
Console.WriteLine($"Route: {deserialized!.Name}");
Console.WriteLine($"Total: {deserialized.TotalDistanceKilometers:F2} km");
```

### Validation Scenarios

```csharp
// Valid single segment route
var (_, from) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, to) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, segment) = RouteSegment.TryCreate(from, to);
var (result1, route1) = GeospatialRoute.TryCreate(new[] { segment! });
result1.IsValid.ShouldBeTrue();

// Invalid - empty segments
var (result2, route2) = GeospatialRoute.TryCreate(Array.Empty<RouteSegment>());
result2.IsValid.ShouldBeFalse(); // Error: InsufficientSegments

// Invalid - null segments collection
var (result3, route3) = GeospatialRoute.TryCreate(null);
result3.IsValid.ShouldBeFalse(); // Error: Required

// Invalid - non-contiguous segments
var (_, point1) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, point2) = Coordinate.TryCreate(40.7484m, -73.9857m);
var (_, point3) = Coordinate.TryCreate(41.0000m, -74.0000m);
var (_, seg1) = RouteSegment.TryCreate(point1, point2);
var (_, seg2) = RouteSegment.TryCreate(point2, point3);
var (_, seg3) = RouteSegment.TryCreate(point1, point3); // Gap!
var (result4, route4) = GeospatialRoute.TryCreate(new[] { seg1!, seg3! });
result4.IsValid.ShouldBeFalse(); // Error: NonContiguousSegments
```
