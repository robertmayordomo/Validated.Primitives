# Geospatial Examples

This file contains examples for `Latitude`, `Longitude`, and `Coordinate` value objects and domain types.

## Latitude

```csharp
var (result, latitude) = Latitude.TryCreate(40.7128m, decimalPlaces: 6);
Console.WriteLine(latitude.ToCardinalString());
```

## Longitude

```csharp
var (result, longitude) = Longitude.TryCreate(-74.0060m, decimalPlaces: 6);
Console.WriteLine(longitude.ToCardinalString());
```

## Coordinate

```csharp
var (result, coordinate) = Coordinate.TryCreate(40.7128m, -74.0060m, decimalPlaces: 6);
Console.WriteLine(coordinate.ToGoogleMapsFormat());
```
