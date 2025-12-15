# GeoBoundary Examples

Examples for polygon boundary operations, area, perimeter, and point-in-polygon checks.

```csharp
var polygon = new GeoBoundary(new [] {
    Coordinate.TryCreate(0m,0m).Value!,
    Coordinate.TryCreate(0m,1m).Value!,
    Coordinate.TryCreate(1m,1m).Value!
});

Console.WriteLine(polygon.AreaKm2);
```
