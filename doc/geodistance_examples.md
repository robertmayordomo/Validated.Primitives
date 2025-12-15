# GeoDistance Examples

Distance calculation examples using `GeoDistance` helpers and the Haversine formula.

```csharp
var (_, ny) = Coordinate.TryCreate(40.7128m, -74.0060m);
var (_, la) = Coordinate.TryCreate(34.0522m, -118.2437m);
var distanceKm = ny.DistanceTo(la);
Console.WriteLine($"Distance: {distanceKm:F2} km");
```
