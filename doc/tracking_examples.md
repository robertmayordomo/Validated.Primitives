# Tracking Number Examples

The `TrackingNumber` validated primitive supports multiple carrier formats (UPS, FedEx, USPS, DHL, Amazon, Royal Mail, etc.) and automatic carrier detection.

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

var (result, tracking) = TrackingNumber.TryCreate("1Z999AA10123456784");
if (result.IsValid)
{
    Console.WriteLine(tracking.Value);
    Console.WriteLine(tracking.GetCarrierName());
}
```

## Normalization and Separators

```csharp
var (result, tracking) = TrackingNumber.TryCreate("1Z-999AA-1012345-6784");
Console.WriteLine(tracking.GetNormalized()); // 1Z999AA10123456784
```

## Validation Error Handling

```csharp
var (result, tracking) = TrackingNumber.TryCreate("INVALID-TRACKING", "ShipmentTracking");
if (!result.IsValid)
    Console.WriteLine(result.ToBulletList());
```
