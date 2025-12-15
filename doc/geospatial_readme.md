# Validated.Primitives.Geospatial Namespace

The Geospatial namespace provides validated value objects and domain models for working with geographic coordinates, distances, boundaries, and routes. All types include built-in validation, distance calculations, and rich formatting options.

## 📦 Package Information

The geospatial types are split across two packages:

### Core Primitives (`Validated.Primitives`)
- `Latitude` - Validated latitude coordinates
- `Longitude` - Validated longitude coordinates

### Domain Models (`Validated.Primitives.Domain`)
- `Coordinate` - Complete geographic coordinate with latitude and longitude
- `GeoDistance` - Distance between two coordinates
- `GeoBoundary` - Geographic boundary/polygon
- `GeospatialRoute` - Multi-segment route with waypoints
- `RouteSegment` - Individual route segment

---

## 🌍 Core Primitives

### Latitude

Represents a validated latitude coordinate (-90 to +90 degrees) with configurable decimal precision.

#### Key Features
- Range validation: -90° to +90°
- Configurable decimal places (0-8), default is 6 for GPS precision
- Hemisphere detection (North/South)
- Cardinal direction formatting
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects.Geospatial;

// Create with validation
var (result, latitude) = Latitude.TryCreate(40.7128m, decimalPlaces: 6);

if (result.IsValid)
{
    Console.WriteLine(latitude.Value);              // 40.7128
    Console.WriteLine(latitude.ToString());         // "40.712800°"
    Console.WriteLine(latitude.ToCardinalString()); // "40.712800° N"
    Console.WriteLine(latitude.GetHemisphere());    // "North"
    Console.WriteLine(latitude.GetCardinalDirection()); // "N"
}
else
{
    Console.WriteLine(result.ToBulletList());
}
```

#### Validation Examples

```csharp
// Valid latitudes
var (result1, lat1) = Latitude.TryCreate(0m);           // ✓ Equator
var (result2, lat2) = Latitude.TryCreate(90m);          // ✓ North Pole
var (result3, lat3) = Latitude.TryCreate(-90m);         // ✓ South Pole
var (result4, lat4) = Latitude.TryCreate(40.712776m);   // ✓ New York City

// Invalid latitudes
var (result5, lat5) = Latitude.TryCreate(91m);          // ✗ Out of range
var (result6, lat6) = Latitude.TryCreate(-100m);        // ✗ Out of range
```

#### Decimal Places Control

```csharp
// Low precision (city-level)
var (_, cityLat) = Latitude.TryCreate(40.7m, decimalPlaces: 1);
Console.WriteLine(cityLat.ToString()); // "40.7°"

// High precision (GPS-level)
var (_, gpsLat) = Latitude.TryCreate(40.712776m, decimalPlaces: 6);
Console.WriteLine(gpsLat.ToString()); // "40.712776°"

// Ultra-high precision (survey-level)
var (_, surveyLat) = Latitude.TryCreate(40.71277645m, decimalPlaces: 8);
Console.WriteLine(surveyLat.ToString()); // "40.71277645°"
```

#### Custom Property Names

```csharp
var (result, latitude) = Latitude.TryCreate(
    100m, 
    propertyName: "UserLocationLatitude"
);

if (!result.IsValid)
{
    // Error message will reference "UserLocationLatitude"
    Console.WriteLine(result.Errors[0].MemberName); // "UserLocationLatitude"
}
```

---

### Longitude

Represents a validated longitude coordinate (-180 to +180 degrees) with configurable decimal precision.

#### Key Features
- Range validation: -180° to +180°
- Configurable decimal places (0-8), default is 6 for GPS precision
- Hemisphere detection (East/West)
- Cardinal direction formatting
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects.Geospatial;

// Create with validation
var (result, longitude) = Longitude.TryCreate(-74.0060m, decimalPlaces: 6);

if (result.IsValid)
{
    Console.WriteLine(longitude.Value);              // -74.0060
    Console.WriteLine(longitude.ToString());         // "-74.006000°"
    Console.WriteLine(longitude.ToCardinalString()); // "74.006000° W"
    Console.WriteLine(longitude.GetHemisphere());    // "West"
    Console.WriteLine(longitude.GetCardinalDirection()); // "W"
}
```

#### Validation Examples

```csharp
// Valid longitudes
var (result1, lon1) = Longitude.TryCreate(0m);          // ✓ Prime Meridian
var (result2, lon2) = Longitude.TryCreate(180m);        // ✓ International Date Line
var (result3, lon3) = Longitude.TryCreate(-180m);       // ✓ International Date Line
var (result4, lon4) = Longitude.TryCreate(-74.0060m);   // ✓ New York City

// Invalid longitudes
var (result5, lon5) = Longitude.TryCreate(181m);        // ✗ Out of range
var (result6, lon6) = Longitude.TryCreate(-200m);       // ✗ Out of range
```

---

## 🗺️ Domain Models

### Coordinate

Combines validated `Latitude` and `Longitude` with optional altitude and accuracy.

#### Key Features
- Validated latitude and longitude components
- Optional altitude (in meters, -500 to +10,000)
- Optional accuracy radius (in meters)
- Multiple output formats (decimal degrees, cardinal, Google Maps)
- Distance calculation using Haversine formula
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

// Create a coordinate
var (result, coordinate) = Coordinate.TryCreate(
    latitude: 40.7128m,
    longitude: -74.0060m,
    decimalPlaces: 6
);

if (result.IsValid)
{
    Console.WriteLine(coordinate.ToCardinalString());       
    // "40.712800° N, 74.006000° W"
    
    Console.WriteLine(coordinate.ToDecimalDegreesString()); 
    // "40.7128, -74.0060"
    
    Console.WriteLine(coordinate.ToGoogleMapsFormat());     
    // "40.7128,-74.0060"
}
```

#### With Altitude and Accuracy

```csharp
// Create coordinate with altitude and accuracy
var (result, nyc) = Coordinate.TryCreate(
    latitude: 40.7128m,
    longitude: -74.0060m,
    decimalPlaces: 6,
    altitude: 10m,        // 10 meters above sea level
    accuracy: 5m          // ±5 meters accuracy
);

Console.WriteLine(nyc.Altitude);  // 10
Console.WriteLine(nyc.Accuracy);  // 5
Console.WriteLine(nyc.ToString()); 
// "40.712800° N, 74.006000° W, 10.0m (±5m)"
```

#### Distance Calculation

```csharp
// Calculate distance between two coordinates
var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, losAngeles) = Coordinate.TryCreate(34.0522m, -118.2437m);

double distanceKm = newYork.DistanceTo(losAngeles);
Console.WriteLine($"Distance: {distanceKm:F2} km"); // ~3,944 km
```

#### Using CoordinateBuilder

```csharp
using Validated.Primitives.Domain.Geospatial.Builders;

var builder = new CoordinateBuilder();

// Method 1: Set individual properties
var (result1, coord1) = builder
    .WithLatitude(40.7128m)
    .WithLongitude(-74.0060m)
    .WithDecimalPlaces(6)
    .WithAltitude(10m)
    .WithAccuracy(5m)
    .Build();

// Method 2: Set coordinates together
var (result2, coord2) = builder
    .Reset()
    .WithCoordinates(40.7128m, -74.0060m)
    .Build();

// Method 3: Set complete position
var (result3, coord3) = builder
    .Reset()
    .WithPosition(
        latitude: 40.7128m,
        longitude: -74.0060m,
        altitude: 10m,
        accuracy: 5m
    )
    .Build();
```

---

### GeoDistance

Represents the calculated distance between two coordinates using the Haversine formula.

#### Key Features
- Accurate great-circle distance calculation
- Multiple unit support (kilometers, miles, meters, nautical miles)
- Haversine formula for spherical distance
- Formatted output with customizable precision
- Radius containment checking

#### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, boston) = Coordinate.TryCreate(42.3601m, -71.0589m);

var (result, distance) = GeoDistance.TryCreate(newYork, boston);

if (result.IsValid)
{
    Console.WriteLine(distance.Kilometers);       // ~306.18 km
    Console.WriteLine(distance.Miles);            // ~190.24 mi
    Console.WriteLine(distance.Meters);           // ~306,180 m
    Console.WriteLine(distance.NauticalMiles);    // ~165.32 nm
}
```

#### Formatted Output

```csharp
var (_, distance) = GeoDistance.TryCreate(from, to);

// Different units
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Kilometers));    // "306.18 km"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Miles));         // "190.24 mi"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Meters));        // "306180.00 m"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.NauticalMiles)); // "165.32 nm"

// Custom decimal places
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Kilometers, 0)); // "306 km"
Console.WriteLine(distance.ToFormattedString(DistanceUnit.Kilometers, 4)); // "306.1800 km"

// Full description
Console.WriteLine(distance.GetDescription());
// "Distance from 40.712800° N, 74.006000° W to 42.360100° N, 71.058900° W: 306.18 km (190.24 mi)"
```

#### Radius Checking

```csharp
var (_, distance) = GeoDistance.TryCreate(pointA, pointB);

bool isNearby = distance.IsWithinRadius(10.0);      // Within 10 km?
bool isLocal = distance.IsWithinRadius(100.0);      // Within 100 km?
bool isRegional = distance.IsWithinRadius(500.0);   // Within 500 km?
```

---

### GeoBoundary

Represents a geographic boundary defined by a polygon of coordinates.

#### Key Features
- Polygon-based boundary definition (minimum 3 vertices)
- Automatic area calculation (square kilometers, miles, meters)
- Perimeter calculation
- Point-in-polygon containment testing (ray casting algorithm)
- Center point (centroid) calculation
- Bounding box extraction
- Distance to nearest edge calculation

#### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

// Define a triangular boundary (e.g., Bermuda Triangle)
var (_, point1) = Coordinate.TryCreate(25.7742m, -80.1937m); // Miami
var (_, point2) = Coordinate.TryCreate(32.3078m, -64.7505m); // Bermuda
var (_, point3) = Coordinate.TryCreate(18.4655m, -66.1057m); // Puerto Rico

var vertices = new[] { point1, point2, point3 };
var (result, boundary) = GeoBoundary.TryCreate(vertices);

if (result.IsValid)
{
    Console.WriteLine(boundary.AreaSquareKilometers);   // ~1,000,000 km²
    Console.WriteLine(boundary.AreaSquareMiles);        // ~386,000 mi²
    Console.WriteLine(boundary.PerimeterKilometers);    // ~3,000 km
    Console.WriteLine(boundary.Vertices.Count);         // 3
}
```

#### Point Containment Testing

```csharp
var (_, boundary) = GeoBoundary.TryCreate(vertices);
var (_, testPoint) = Coordinate.TryCreate(25.0m, -75.0m);

bool isInside = boundary.Contains(testPoint);
Console.WriteLine($"Point is {(isInside ? "inside" : "outside")} boundary");
```

#### Bounding Box

```csharp
var (minLat, maxLat, minLon, maxLon) = boundary.GetBoundingBox();

Console.WriteLine($"Latitude range: {minLat}° to {maxLat}°");
Console.WriteLine($"Longitude range: {minLon}° to {maxLon}°");
```

#### Distance to Boundary

```csharp
var (_, externalPoint) = Coordinate.TryCreate(30.0m, -85.0m);
double distanceKm = boundary.DistanceToNearestEdge(externalPoint);

Console.WriteLine($"Distance to boundary: {distanceKm:F2} km");
```

#### Formatted Output

```csharp
Console.WriteLine(boundary.GetDescription());
// "Polygon with 3 vertices: Area 1,000,000.00 km² (386,000.00 mi²), Perimeter 3,000.00 km (1,864.11 mi)"

Console.WriteLine(boundary.Center.ToCardinalString());
// "25.182500° N, 70.350967° W" (approximate centroid)
```

---

### RouteSegment

Represents a segment of a route with starting and ending coordinates.

#### Key Features
- Start and end coordinate validation
- Automatic distance calculation
- Optional segment naming
- Formatted segment description

#### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, philadelphia) = Coordinate.TryCreate(39.9526m, -75.1652m);

var (result, segment) = RouteSegment.TryCreate(
    from: newYork,
    to: philadelphia,
    name: "NYC to Philly"
);

if (result.IsValid)
{
    Console.WriteLine(segment.Distance.Kilometers);  // ~130 km
    Console.WriteLine(segment.GetDescription());
    // "NYC to Philly: 40.712800° N, 74.006000° W → 39.952600° N, 75.165200° W (130.00 km)"
}
```

#### Using RouteSegmentBuilder

```csharp
using Validated.Primitives.Domain.Geospatial.Builders;

var builder = new RouteSegmentBuilder();

var (result, segment) = builder
    .WithFrom(startCoordinate)
    .WithTo(endCoordinate)
    .WithName("Leg 1")
    .Build();

// Or set both coordinates at once
var (result2, segment2) = builder
    .Reset()
    .WithCoordinates(startCoordinate, endCoordinate)
    .WithName("Leg 2")
    .Build();
```

---

### GeospatialRoute

Represents a complete route composed of multiple contiguous segments.

#### Key Features
- Multi-segment route composition
- Contiguity validation (each segment's end must match next segment's start)
- Total distance calculation (sum of all segments)
- Multiple distance units (kilometers, miles, meters)
- Waypoint extraction
- Cumulative distance tracking
- Optional route naming

#### Basic Usage

```csharp
using Validated.Primitives.Domain.Geospatial;

// Create individual coordinates
var (_, newYork) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, philadelphia) = Coordinate.TryCreate(39.9526m, -75.1652m);
var (_, baltimore) = Coordinate.TryCreate(39.2904m, -76.6122m);
var (_, washington) = Coordinate.TryCreate(38.9072m, -77.0369m);

// Create route segments
var (_, seg1) = RouteSegment.TryCreate(newYork, philadelphia, "NYC to Philly");
var (_, seg2) = RouteSegment.TryCreate(philadelphia, baltimore, "Philly to Baltimore");
var (_, seg3) = RouteSegment.TryCreate(baltimore, washington, "Baltimore to DC");

// Create route
var segments = new[] { seg1, seg2, seg3 };
var (result, route) = GeospatialRoute.TryCreate(
    segments, 
    name: "Northeast Corridor"
);

if (result.IsValid)
{
    Console.WriteLine(route.Segments.Count);              // 3
    Console.WriteLine(route.TotalDistanceKilometers);     // ~360 km
    Console.WriteLine(route.TotalDistanceMiles);          // ~224 mi
    Console.WriteLine(route.StartingPoint.ToCardinalString()); 
    // "40.712800° N, 74.006000° W"
    Console.WriteLine(route.EndingPoint.ToCardinalString());   
    // "38.907200° N, 77.036900° W"
}
```

#### Route Validation

Routes validate that segments are contiguous:

```csharp
var (_, nyc) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, boston) = Coordinate.TryCreate(42.3601m, -71.0589m);
var (_, miami) = Coordinate.TryCreate(25.7617m, -80.1918m);
var (_, chicago) = Coordinate.TryCreate(41.8781m, -87.6298m);

var (_, seg1) = RouteSegment.TryCreate(nyc, boston);
var (_, seg2) = RouteSegment.TryCreate(miami, chicago); // ✗ Doesn't connect!

var (result, route) = GeospatialRoute.TryCreate(new[] { seg1, seg2 });

// result.IsValid == false
// Error: "Segment 1 ends at 42.360100° N, 71.058900° W but segment 2 starts at 
//         25.761700° N, 80.191800° W. Segments must be contiguous."
```

#### Working with Waypoints

```csharp
var (_, route) = GeospatialRoute.TryCreate(segments, "My Route");

// Get all waypoints (unique coordinates in order)
IReadOnlyList<Coordinate> waypoints = route.GetWaypoints();
// Returns: [NYC, Philadelphia, Baltimore, Washington DC]

foreach (var waypoint in waypoints)
{
    Console.WriteLine(waypoint.ToCardinalString());
}
```

#### Cumulative Distance Tracking

```csharp
var (_, route) = GeospatialRoute.TryCreate(segments);

// Get distance at specific segment
GeoDistance? segmentDistance = route.GetSegmentDistance(1); // Second segment
Console.WriteLine(segmentDistance?.Kilometers);

// Get cumulative distance up to segment
double? cumulative = route.GetCumulativeDistance(1); // Through second segment
Console.WriteLine($"Distance through segment 1: {cumulative:F2} km");

// Show cumulative distances for all segments
for (int i = 0; i < route.Segments.Count; i++)
{
    double? dist = route.GetCumulativeDistance(i);
    Console.WriteLine($"After segment {i}: {dist:F2} km");
}
```

#### Formatted Output

```csharp
Console.WriteLine(route.GetDescription());
// "Northeast Corridor: 3 segments, Total distance: 360.00 km (223.69 mi)
//  From: 40.712800° N, 74.006000° W
//  To: 38.907200° N, 77.036900° W"

Console.WriteLine(route.ToString()); // Same as GetDescription()
```

#### Using GeospatialRouteBuilder

```csharp
using Validated.Primitives.Domain.Geospatial.Builders;

var builder = new GeospatialRouteBuilder();

// Method 1: Add pre-created segments
var (result1, route1) = builder
    .AddSegment(segment1)
    .AddSegment(segment2)
    .AddSegment(segment3)
    .WithName("My Route")
    .Build();

// Method 2: Add multiple segments at once
var (result2, route2) = builder
    .Reset()
    .AddSegments(new[] { segment1, segment2, segment3 })
    .WithName("Bulk Route")
    .Build();

// Method 3: Create segments inline
var (result3, route3) = builder
    .Reset()
    .AddSegment(coord1, coord2, "Leg 1")
    .AddSegment(coord2, coord3, "Leg 2")
    .AddSegment(coord3, coord4, "Leg 3")
    .WithName("Inline Route")
    .Build();

// Check segment count before building
Console.WriteLine(builder.SegmentCount); // Number of segments added
```

---

##  Common Patterns

### Validation Pattern

All geospatial types follow the same validation pattern:

```csharp
var (result, value) = Type.TryCreate(...);

if (result.IsValid)
{
    // Use the validated value
    DoSomething(value);
}
else
{
    // Handle validation errors
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.MemberName}: {error.Message}");
    }
    
    // Or use formatted output
    Console.WriteLine(result.ToBulletList());
}
```

### Builder Pattern

Complex types provide fluent builders:

```csharp
var builder = new CoordinateBuilder(); // or RouteSegmentBuilder, GeospatialRouteBuilder

var (result, value) = builder
    .WithProperty1(value1)
    .WithProperty2(value2)
    // ... more properties
    .Build();

// Reuse the builder
builder.Reset();
var (result2, value2) = builder
    .WithProperty1(newValue1)
    .Build();
```

### JSON Serialization

All types support JSON serialization out of the box:

```csharp
using System.Text.Json;

var (_, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m);

// Serialize
string json = JsonSerializer.Serialize(coordinate);

// Deserialize
var deserialized = JsonSerializer.Deserialize<Coordinate>(json);
```

---

## 🎯 Real-World Examples

### Location-Based Service

```csharp
// User's current location
var (_, userLocation) = Coordinate.TryCreate(40.7580m, -73.9855m);

// Restaurant location
var (_, restaurantLocation) = Coordinate.TryCreate(40.7614m, -73.9776m);

// Calculate distance
var (_, distance) = GeoDistance.TryCreate(userLocation, restaurantLocation);

if (distance.IsWithinRadius(1.0)) // Within 1 km
{
    Console.WriteLine($"Restaurant is {distance.ToFormattedString(DistanceUnit.Meters)} away!");
    Console.WriteLine("Nearby!");
}
```

### Delivery Route Planning

```csharp
// Warehouse
var (_, warehouse) = Coordinate.TryCreate(40.7128m, -74.0060m);

// Delivery stops
var (_, stop1) = Coordinate.TryCreate(40.7589m, -73.9851m);
var (_, stop2) = Coordinate.TryCreate(40.7831m, -73.9712m);
var (_, stop3) = Coordinate.TryCreate(40.7489m, -73.9680m);

// Create route segments
var (_, leg1) = RouteSegment.TryCreate(warehouse, stop1, "To First Customer");
var (_, leg2) = RouteSegment.TryCreate(stop1, stop2, "To Second Customer");
var (_, leg3) = RouteSegment.TryCreate(stop2, stop3, "To Third Customer");
var (_, leg4) = RouteSegment.TryCreate(stop3, warehouse, "Return to Warehouse");

// Build complete route
var (result, route) = new GeospatialRouteBuilder()
    .AddSegments(new[] { leg1, leg2, leg3, leg4 })
    .WithName("Morning Deliveries")
    .Build();

if (result.IsValid)
{
    Console.WriteLine($"Total route distance: {route.TotalDistanceKilometers:F2} km");
    Console.WriteLine($"Number of stops: {route.Segments.Count}");
    
    // Track progress
    for (int i = 0; i < route.Segments.Count; i++)
    {
        var segment = route.Segments[i];
        var cumulative = route.GetCumulativeDistance(i);
        Console.WriteLine($"After {segment.Name}: {cumulative:F2} km total");
    }
}
```

### Geofencing

```csharp
// Define a delivery zone (polygon)
var (_, corner1) = Coordinate.TryCreate(40.7000m, -74.0200m);
var (_, corner2) = Coordinate.TryCreate(40.7200m, -74.0200m);
var (_, corner3) = Coordinate.TryCreate(40.7200m, -73.9800m);
var (_, corner4) = Coordinate.TryCreate(40.7000m, -73.9800m);

var (_, deliveryZone) = GeoBoundary.TryCreate(new[] 
{ 
    corner1, corner2, corner3, corner4 
});

// Check if delivery address is in zone
var (_, deliveryAddress) = Coordinate.TryCreate(40.7100m, -74.0000m);

if (deliveryZone.Contains(deliveryAddress))
{
    Console.WriteLine("Address is within delivery zone!");
}
else
{
    double distance = deliveryZone.DistanceToNearestEdge(deliveryAddress);
    Console.WriteLine($"Address is {distance:F2} km outside delivery zone");
}
```

### GPS Tracking with Altitude

```csharp
// Flight path tracking
var (_, takeoff) = Coordinate.TryCreate(
    latitude: 40.7128m,
    longitude: -74.0060m,
    altitude: 10m,        // Ground level
    accuracy: 5m
);

var (_, cruising) = Coordinate.TryCreate(
    latitude: 41.0000m,
    longitude: -75.0000m,
    altitude: 10000m,     // 10km altitude
    accuracy: 50m
);

var (_, landing) = Coordinate.TryCreate(
    latitude: 42.3601m,
    longitude: -71.0589m,
    altitude: 15m,        // Ground level
    accuracy: 5m
);

Console.WriteLine($"Takeoff: {takeoff}");
Console.WriteLine($"Cruising: {cruising}");
Console.WriteLine($"Landing: {landing}");
```

---

## 🔗 Related Documentation

- [Geospatial Examples](geospatial_examples.md) - Basic usage examples
- [GeoDistance Examples](geodistance_examples.md) - Distance calculation examples
- [GeoBoundary Examples](geoboundary_examples.md) - Boundary and polygon examples
- [Builder Examples](builders_examples.md) - Builder pattern usage

---

## 📚 API Reference

For complete API documentation, see:
- XML documentation comments in source code
- IntelliSense in Visual Studio
- [Validated.Primitives Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)
- [Validated.Primitives.Domain Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)
