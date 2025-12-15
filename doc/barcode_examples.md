# Barcode Examples

The `Barcode` validated primitive supports multiple barcode formats with automatic format detection and checksum validation.

## Supported Formats

- `UPC-A` (12 digits)
- `EAN-13` (13 digits)
- `EAN-8` (8 digits)
- `Code39` (alphanumeric with `*` delimiters)
- `Code128` (alphanumeric)

## Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Validate a UPC-A barcode
var (result, barcode) = Barcode.TryCreate("042100005264");
if (result.IsValid)
{
    Console.WriteLine($"Valid barcode: {barcode.Value}");
    Console.WriteLine($"Format: {barcode.Format}");
}

// Normalize input with separators
var (result2, b2) = Barcode.TryCreate("0421-0000-5264");
if (result2.IsValid)
{
    Console.WriteLine(b2.GetNormalized()); // 042100005264
}
```

## Validation Error Handling

```csharp
var (result, barcode) = Barcode.TryCreate("INVALID-CODE", "ProductBarcode");
if (!result.IsValid)
{
    Console.WriteLine(result.ToBulletList());
}
```


## Barcode Usage Examples

The **`Barcode`** validated primitive supports multiple barcode formats with automatic format detection and checksum validation:

### Supported Formats

- **UPC-A** (12 digits) - Universal Product Code with checksum validation
- **EAN-13** (13 digits) - European Article Number with checksum validation  
- **EAN-8** (8 digits) - Short EAN format with checksum validation
- **Code39** - Alphanumeric with `*` delimiters (e.g., `*PRODUCT123*`)
- **Code128** - Alphanumeric barcodes without asterisks

### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Validate a UPC-A barcode
var (result, barcode) = Barcode.TryCreate("042100005264");
if (result.IsValid)
{
    Console.WriteLine($"Valid barcode: {barcode.Value}");
    Console.WriteLine($"Format: {barcode.Format}"); // Output: UpcA
}

// Validate an EAN-13 barcode
var (eanResult, eanBarcode) = Barcode.TryCreate("5901234123457");
if (eanResult.IsValid)
{
    Console.WriteLine($"EAN-13: {eanBarcode.Value}");
    Console.WriteLine($"Format: {eanBarcode.Format}"); // Output: Ean13
}

// Validate a Code39 barcode
var (code39Result, code39) = Barcode.TryCreate("*PRODUCT-123*");
if (code39Result.IsValid)
{
    Console.WriteLine($"Code39: {code39.Value}");
    Console.WriteLine($"Format: {code39.Format}"); // Output: Code39
}
```

### Barcode with Separators

Barcodes with hyphens or spaces are automatically normalized:

```csharp
// Input with separators
var (result, barcode) = Barcode.TryCreate("0421-0000-5264");
if (result.IsValid)
{
    Console.WriteLine($"Original: {barcode.Value}");        // Output: 0421-0000-5264
    Console.WriteLine($"Normalized: {barcode.GetNormalized()}"); // Output: 042100005264
    Console.WriteLine($"Format: {barcode.Format}");         // Output: UpcA
}
```

### E-Commerce Product Example

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Barcode? UpcBarcode { get; set; }
    public Barcode? EanBarcode { get; set; }
    public Barcode? InternalCode { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = new Product { Name = request.Name };

        // Validate UPC barcode
        if (!string.IsNullOrEmpty(request.UpcBarcode))
        {
            var (upcResult, upc) = Barcode.TryCreate(request.UpcBarcode, nameof(request.UpcBarcode));
            if (!upcResult.IsValid)
                return BadRequest(new { errors = upcResult.Errors });
            
            // Ensure it's actually UPC-A format
            if (upc!.Format != BarcodeFormat.UpcA)
                return BadRequest(new { error = "UPC barcode must be in UPC-A format" });
                
            product.UpcBarcode = upc;
        }

        // Validate EAN barcode
        if (!string.IsNullOrEmpty(request.EanBarcode))
        {
            var (eanResult, ean) = Barcode.TryCreate(request.EanBarcode, nameof(request.EanBarcode));
            if (!eanResult.IsValid)
                return BadRequest(new { errors = eanResult.Errors });
                
            product.EanBarcode = ean;
        }

        // Validate internal Code128 barcode
        if (!string.IsNullOrEmpty(request.InternalCode))
        {
            var (codeResult, code) = Barcode.TryCreate(request.InternalCode, nameof(request.InternalCode));
            if (!codeResult.IsValid)
                return BadRequest(new { errors = codeResult.Errors });
                
            product.InternalCode = code;
        }

        // Save product...
        return Ok(product);
    }

    [HttpGet("{barcode}")]
    public IActionResult FindByBarcode(string barcode)
    {
        var (result, validBarcode) = Barcode.TryCreate(barcode);
        if (!result.IsValid)
            return BadRequest(new { error = "Invalid barcode format" });

        // Search by normalized barcode value to match regardless of separators
        var normalizedBarcode = validBarcode!.GetNormalized();
        
        // Search logic here...
        return Ok(new { searchTerm = normalizedBarcode, format = validBarcode.Format });
    }
}
```

### Validation Error Handling

```csharp
var (result, barcode) = Barcode.TryCreate("INVALID-CODE", "ProductBarcode");

if (!result.IsValid)
{
    // Display all validation errors
    Console.WriteLine("Validation failed:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  {error.MemberName}: {error.Message}");
    }
    
    // Or get as bullet list
    Console.WriteLine(result.ToBulletList());
    
    // Or get as dictionary for JSON response
    var errorDict = result.ToDictionary();
}
```

### JSON Serialization

Barcodes serialize as simple strings in JSON:

```csharp
var product = new Product
{
    Id = 1,
    Name = "Sample Product",
    UpcBarcode = Barcode.TryCreate("042100005264").Value,
    InternalCode = Barcode.TryCreate("*PROD-123*").Value
};

var json = JsonSerializer.Serialize(product);
// Output: {"Id":1,"Name":"Sample Product","UpcBarcode":"042100005264","InternalCode":"*PROD-123*"}

var deserialized = JsonSerializer.Deserialize<Product>(json);
Console.WriteLine(deserialized.UpcBarcode?.Format); // Output: UpcA
```