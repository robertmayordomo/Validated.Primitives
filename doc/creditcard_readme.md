# CreditCard

The CreditCard namespace provides validated value objects and domain models for securely handling credit card information, including card numbers, security codes (CVV/CVC), and expiration dates. All types include built-in validation, Luhn algorithm checking, and automatic masking for security.

## ?? Package Information

The credit card types are split across two packages:

### Core Primitives (`Validated.Primitives`)
- `CreditCardNumber` - Validated credit card number with Luhn algorithm verification
- `CreditCardSecurityNumber` - CVV/CVC security code validation (3-4 digits)
- `CreditCardExpiration` - Expiration date with automatic year normalization and expiry checking

### Domain Models (`Validated.Primitives.Domain`)
- `CreditCardDetails` - Complete credit card information with comprehensive validation
- `CreditCardBuilder` - Fluent builder for constructing credit card details

---

## ?? Core Primitives

### CreditCardNumber

Represents a validated credit card number with Luhn algorithm verification. Accepts all major card types including Visa, MasterCard, American Express, and Discover.

#### Key Features
- Luhn algorithm (mod-10) checksum validation
- Supports 13-19 digit card numbers
- Automatic digit extraction (removes spaces and hyphens)
- Rejects all-identical-digit patterns (e.g., 0000000000000000)
- Card number masking for security (shows last 4 digits only)
- Supports all major card brands (Visa, MasterCard, Amex, Discover, etc.)
- JSON serialization support

#### Supported Card Formats
- **Visa**: 13 or 16 digits, starts with 4
- **MasterCard**: 16 digits, starts with 51-55 or 2221-2720
- **American Express**: 15 digits, starts with 34 or 37
- **Discover**: 16 digits, starts with 6011, 622126-622925, 644-649, or 65
- **Diners Club**: 14 digits, starts with 300-305, 36, or 38
- **JCB**: 16 digits, starts with 3528-3589

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, cardNumber) = CreditCardNumber.TryCreate("4111 1111 1111 1111");

if (result.IsValid)
{
    Console.WriteLine(cardNumber.Value);       // "4111111111111111" (digits only)
    Console.WriteLine(cardNumber.ToString());  // "4111111111111111"
    Console.WriteLine(cardNumber.Masked());    // "************1111"
}
else
{
    Console.WriteLine(result.ToBulletList());
}
```

#### Valid Card Numbers

```csharp
// Visa (16 digits)
var (r1, card1) = CreditCardNumber.TryCreate("4111111111111111");      // ? Valid
var (r2, card2) = CreditCardNumber.TryCreate("4111 1111 1111 1111");   // ? Spaces allowed

// MasterCard (16 digits)
var (r3, card3) = CreditCardNumber.TryCreate("5500000000000004");      // ? Valid
var (r4, card4) = CreditCardNumber.TryCreate("5500-0000-0000-0004");   // ? Hyphens allowed

// American Express (15 digits)
var (r5, card5) = CreditCardNumber.TryCreate("340000000000009");       // ? Valid
var (r6, card6) = CreditCardNumber.TryCreate("3400 000000 00009");     // ? Amex format

// Discover (16 digits)
var (r7, card7) = CreditCardNumber.TryCreate("6011000000000004");      // ? Valid
```

#### Invalid Card Numbers

```csharp
// Invalid Luhn checksum
var (r1, c1) = CreditCardNumber.TryCreate("4111111111111112");         // ? Fails Luhn check

// All identical digits
var (r2, c2) = CreditCardNumber.TryCreate("0000000000000000");         // ? All zeros
var (r3, c3) = CreditCardNumber.TryCreate("1111111111111111");         // ? All ones

// Too short
var (r4, c4) = CreditCardNumber.TryCreate("4111111111");               // ? Only 10 digits

// Too long
var (r5, c5) = CreditCardNumber.TryCreate("41111111111111111111");     // ? 20 digits

// Empty or null
var (r6, c6) = CreditCardNumber.TryCreate("");                         // ? Empty
var (r7, c7) = CreditCardNumber.TryCreate(null);                       // ? Null
```

#### Card Masking

```csharp
var (_, card) = CreditCardNumber.TryCreate("4111111111111111");

Console.WriteLine(card.Value);     // "4111111111111111" (full number)
Console.WriteLine(card.Masked());  // "************1111" (last 4 visible)

// Always use masked version for display/logging
_logger.LogInformation($"Processing payment for card: {card.Masked()}");
```

#### Format Flexibility

```csharp
// All these formats are equivalent and valid
var inputs = new[]
{
    "4111111111111111",
    "4111 1111 1111 1111",
    "4111-1111-1111-1111",
    "4111  1111  1111  1111",  // Extra spaces
    " 4111 1111 1111 1111 "     // Leading/trailing spaces
};

foreach (var input in inputs)
{
    var (result, card) = CreditCardNumber.TryCreate(input);
    // All will be valid and normalize to "4111111111111111"
    Console.WriteLine(card.Value);  // "4111111111111111"
}
```

---

### CreditCardSecurityNumber

Represents a validated credit card security code (CVV/CVC). Supports both 3-digit (most cards) and 4-digit (American Express) security codes.

#### Key Features
- 3-4 digit validation (CVV/CVC/CID)
- Automatic digit extraction (removes non-numeric characters)
- Length validation
- Never displayed in output (always masked as ***)
- JSON serialization support

#### CVV/CVC Format
- **CVV (Visa, MasterCard, Discover)**: 3 digits
- **CVC (Visa, MasterCard)**: 3 digits
- **CID (American Express)**: 4 digits
- **CVV2**: 3 digits (printed on card back)

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, cvv) = CreditCardSecurityNumber.TryCreate("123");

if (result.IsValid)
{
    Console.WriteLine(cvv.Value);      // "123"
    Console.WriteLine(cvv.ToString()); // "123"
    
    // NEVER display CVV in production - shown for demonstration only
    // Always use masking: "***"
}
```

#### Valid Security Numbers

```csharp
// 3-digit CVV (Visa, MasterCard, Discover)
var (r1, cvv1) = CreditCardSecurityNumber.TryCreate("123");     // ? Valid
var (r2, cvv2) = CreditCardSecurityNumber.TryCreate("000");     // ? Valid (all zeros allowed)
var (r3, cvv3) = CreditCardSecurityNumber.TryCreate("999");     // ? Valid

// 4-digit CID (American Express)
var (r4, cid1) = CreditCardSecurityNumber.TryCreate("1234");    // ? Valid
var (r5, cid2) = CreditCardSecurityNumber.TryCreate("0000");    // ? Valid

// With non-digit characters (automatically extracted)
var (r6, cvv4) = CreditCardSecurityNumber.TryCreate("1 2 3");   // ? Becomes "123"
var (r7, cvv5) = CreditCardSecurityNumber.TryCreate("CVV:123"); // ? Becomes "123"
```

#### Invalid Security Numbers

```csharp
// Too short
var (r1, c1) = CreditCardSecurityNumber.TryCreate("12");        // ? Only 2 digits

// Too long
var (r2, c2) = CreditCardSecurityNumber.TryCreate("12345");     // ? 5 digits

// Empty or null
var (r3, c3) = CreditCardSecurityNumber.TryCreate("");          // ? Empty
var (r4, c4) = CreditCardSecurityNumber.TryCreate(null);        // ? Null
```

#### Security Best Practice

```csharp
var (_, cvv) = CreditCardSecurityNumber.TryCreate("123");

// DON'T: Never log or display the actual CVV
Console.WriteLine($"CVV: {cvv.Value}");  // ? Security risk!

// DO: Always mask CVV in logs and displays
Console.WriteLine("CVV: ***");           // ? Safe
_logger.LogInformation("Processing payment, CVV verified");  // ? Don't include value
```

---

### CreditCardExpiration

Represents a validated credit card expiration date with automatic year normalization and expiry checking.

#### Key Features
- Month validation (1-12)
- Year validation and normalization (2-digit to 4-digit)
- Automatic expiry checking (cannot create expired cards)
- MM/YY formatted display
- Future-only validation
- JSON serialization support

#### Year Normalization
- Input: `25` ? Normalized: `2025`
- Input: `2025` ? Normalized: `2025`
- Assumes 20XX for 2-digit years (25 becomes 2025, not 1925)

#### Basic Usage

```csharp
using Validated.Primitives.ValueObjects;

// Create with validation
var (result, expiration) = CreditCardExpiration.TryCreate(12, 2025);

if (result.IsValid)
{
    Console.WriteLine(expiration.Month);      // 12
    Console.WriteLine(expiration.Year);       // 2025
    Console.WriteLine(expiration.ToString()); // "12/25" (MM/YY format)
}
```

#### Valid Expirations

```csharp
// Current or future dates
var now = DateTime.UtcNow;

var (r1, exp1) = CreditCardExpiration.TryCreate(now.Month, now.Year);         // ? Current month
var (r2, exp2) = CreditCardExpiration.TryCreate(12, now.Year);                // ? Later this year
var (r3, exp3) = CreditCardExpiration.TryCreate(1, now.Year + 1);             // ? Next year
var (r4, exp4) = CreditCardExpiration.TryCreate(6, now.Year + 5);             // ? 5 years from now

// Two-digit year (automatically normalized)
var (r5, exp5) = CreditCardExpiration.TryCreate(12, 25);                      // ? Becomes 2025
var (r6, exp6) = CreditCardExpiration.TryCreate(6, 30);                       // ? Becomes 2030
```

#### Invalid Expirations

```csharp
// Invalid month
var (r1, e1) = CreditCardExpiration.TryCreate(0, 2025);     // ? Month must be 1-12
var (r2, e2) = CreditCardExpiration.TryCreate(13, 2025);    // ? Month must be 1-12
var (r3, e3) = CreditCardExpiration.TryCreate(-1, 2025);    // ? Negative month

// Expired dates
var (r4, e4) = CreditCardExpiration.TryCreate(1, 2020);     // ? In the past
var (r5, e5) = CreditCardExpiration.TryCreate(6, 2022);     // ? In the past
```

#### Formatting Examples

```csharp
var (_, exp1) = CreditCardExpiration.TryCreate(3, 2025);
Console.WriteLine(exp1.ToString());  // "03/25"

var (_, exp2) = CreditCardExpiration.TryCreate(12, 2030);
Console.WriteLine(exp2.ToString());  // "12/30"

// Two-digit year input
var (_, exp3) = CreditCardExpiration.TryCreate(6, 28);
Console.WriteLine(exp3.ToString());  // "06/28" (normalized from 2028)
Console.WriteLine(exp3.Year);        // 2028
```

---

## ?? Domain Model

### CreditCardDetails

Represents complete credit card information with comprehensive validation of all components.

#### Key Features
- Validates card number (Luhn algorithm)
- Validates security code (CVV/CVC)
- Validates expiration date (not expired)
- Atomic validation (all fields must be valid)
- Automatic masking for security
- Expiry checking
- Comprehensive error messages
- JSON serialization support

#### Basic Usage

```csharp
using Validated.Primitives.Domain;

// Create with validation
var (result, card) = CreditCardDetails.TryCreate(
    cardNumber: "4111 1111 1111 1111",
    securityNumber: "123",
    expirationMonth: 12,
    expirationYear: 2025
);

if (result.IsValid)
{
    Console.WriteLine(card.CardNumber.Value);           // "4111111111111111"
    Console.WriteLine(card.SecurityNumber.Value);       // "123"
    Console.WriteLine(card.Expiration.Month);           // 12
    Console.WriteLine(card.Expiration.Year);            // 2025
    Console.WriteLine(card.ToString());                 // "Card: ************1111, Expires: 12/25, CVV: ***"
}
else
{
    // Display validation errors
    Console.WriteLine(result.ToBulletList());
}
```

#### Validation Examples

```csharp
// ? Valid - All components valid
var (r1, c1) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    2025
);
// r1.IsValid == true

// ? Valid - With formatting (spaces removed)
var (r2, c2) = CreditCardDetails.TryCreate(
    "4111 1111 1111 1111",
    "123",
    12,
    2025
);
// r2.IsValid == true

// ? Valid - Two-digit year (normalized to 2025)
var (r3, c3) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    25
);
// c3.Expiration.Year == 2025

// ? Valid - 4-digit CVV (American Express)
var (r4, c4) = CreditCardDetails.TryCreate(
    "340000000000009",
    "1234",
    6,
    2026
);
// r4.IsValid == true

// ? Invalid - Bad card number (fails Luhn check)
var (r5, c5) = CreditCardDetails.TryCreate(
    "1234567890123456",
    "123",
    12,
    2025
);
// r5.IsValid == false, error on CardNumber

// ? Invalid - Expired card
var (r6, c6) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    1,
    2020
);
// r6.IsValid == false, error on Expiration

// ? Invalid - Multiple errors
var (r7, c7) = CreditCardDetails.TryCreate(
    "",           // Empty card number
    "12",         // CVV too short
    13,           // Invalid month
    2020          // Expired
);
// r7.IsValid == false, multiple errors
// r7.Errors.Count >= 3
```

#### Working with Different Card Types

```csharp
// Visa
var (r1, visa) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    2025
);

// MasterCard
var (r2, mastercard) = CreditCardDetails.TryCreate(
    "5500000000000004",
    "123",
    12,
    2025
);

// American Express (15 digits, 4-digit CVV)
var (r3, amex) = CreditCardDetails.TryCreate(
    "340000000000009",
    "1234",
    12,
    2025
);

// Discover
var (r4, discover) = CreditCardDetails.TryCreate(
    "6011000000000004",
    "123",
    12,
    2025
);

// All are valid
Console.WriteLine($"Visa: {visa != null}");        // true
Console.WriteLine($"MC: {mastercard != null}");    // true
Console.WriteLine($"Amex: {amex != null}");        // true
Console.WriteLine($"Discover: {discover != null}");// true
```

#### Card Expiry Checking

```csharp
var (_, card) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    DateTime.UtcNow.Year + 1  // Next year
);

// Check if card has expired
if (card.IsExpired())
{
    Console.WriteLine("Card is expired");
}
else
{
    Console.WriteLine("Card is valid");
}
```

#### Masked Display

```csharp
var (_, card) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    2025
);

// Get masked card number
Console.WriteLine(card.GetMaskedCardNumber());  // "************1111"

// ToString includes all masked information
Console.WriteLine(card.ToString());
// "Card: ************1111, Expires: 12/25, CVV: ***"
```

#### Properties and Methods

```csharp
var (_, card) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    2025
);

// Access components
Console.WriteLine(card.CardNumber.Value);       // "4111111111111111"
Console.WriteLine(card.SecurityNumber.Value);   // "123"
Console.WriteLine(card.Expiration.Month);       // 12
Console.WriteLine(card.Expiration.Year);        // 2025
Console.WriteLine(card.Expiration.ToString());  // "12/25"

// Masked display
Console.WriteLine(card.GetMaskedCardNumber());  // "************1111"
Console.WriteLine(card.CardNumber.Masked());    // "************1111"

// Check expiry
bool expired = card.IsExpired();                // false

// Full string representation (all masked)
Console.WriteLine(card.ToString());
// "Card: ************1111, Expires: 12/25, CVV: ***"
```

---

### CreditCardBuilder

Fluent builder for creating validated `CreditCardDetails` with convenience methods for different input formats.

#### Key Features
- Fluent interface for step-by-step construction
- Multiple expiration format options (DateTime, string, month/year)
- Support for string or integer security codes
- Comprehensive validation before building
- Reusable with `Reset()` method

#### Basic Builder Usage

```csharp
using Validated.Primitives.Domain.Builders;

var builder = new CreditCardBuilder();

// Build step-by-step
var (result, card) = builder
    .WithCardNumber("4111 1111 1111 1111")
    .WithSecurityCode("123")
    .WithExpiration(12, 2025)
    .Build();
```

#### Expiration Format Options

```csharp
var builder = new CreditCardBuilder();

// Method 1: Month and year as integers
var (r1, c1) = builder
    .WithCardNumber("4111111111111111")
    .WithSecurityCode("123")
    .WithExpiration(12, 2025)
    .Build();

// Method 2: DateTime
var (r2, c2) = builder
    .Reset()
    .WithCardNumber("4111111111111111")
    .WithSecurityCode("123")
    .WithExpiration(new DateTime(2025, 12, 1))
    .Build();

// Method 3: String format "MM/YY"
var (r3, c3) = builder
    .Reset()
    .WithCardNumber("4111111111111111")
    .WithSecurityCode("123")
    .WithExpiration("12/25")
    .Build();

// Method 4: String format "MM/YYYY"
var (r4, c4) = builder
    .Reset()
    .WithCardNumber("4111111111111111")
    .WithSecurityCode("123")
    .WithExpiration("12/2025")
    .Build();
```

#### Security Code Options

```csharp
var builder = new CreditCardBuilder();

// Method 1: String security code
var (r1, c1) = builder
    .WithCardNumber("4111111111111111")
    .WithSecurityCode("123")
    .WithExpiration(12, 2025)
    .Build();

// Method 2: Integer security code
var (r2, c2) = builder
    .Reset()
    .WithCardNumber("4111111111111111")
    .WithSecurityCode(123)
    .WithExpiration(12, 2025)
    .Build();
```

#### Builder Reuse

```csharp
var builder = new CreditCardBuilder();

// First card
var (result1, card1) = builder
    .WithCardNumber("4111111111111111")
    .WithSecurityCode("123")
    .WithExpiration(12, 2025)
    .Build();

// Reuse builder for different card
var (result2, card2) = builder
    .Reset()  // Clear previous values
    .WithCardNumber("5500000000000004")
    .WithSecurityCode("456")
    .WithExpiration("06/2026")
    .Build();

// Another reuse
var (result3, card3) = builder
    .Reset()
    .WithCardNumber("340000000000009")
    .WithSecurityCode(7890)
    .WithExpiration(new DateTime(2027, 3, 1))
    .Build();
```

#### Complete Builder Example

```csharp
var builder = new CreditCardBuilder();

// Build with all options
var (result, card) = builder
    .WithCardNumber("4111 1111 1111 1111")
    .WithSecurityCode(123)
    .WithExpiration("12/25")
    .Build();

if (result.IsValid)
{
    Console.WriteLine("Card created successfully!");
    Console.WriteLine($"Masked: {card.GetMaskedCardNumber()}");
    Console.WriteLine($"Expires: {card.Expiration}");
}
else
{
    Console.WriteLine("Validation errors:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error.MemberName}: {error.Message}");
    }
}
```

#### Error Handling with Builder

```csharp
var builder = new CreditCardBuilder();

// Build with invalid data
var (result, card) = builder
    .WithCardNumber("1234")  // Invalid
    .WithSecurityCode("12")  // Too short
    .WithExpiration(13, 2020)  // Invalid month and expired
    .Build();

if (!result.IsValid)
{
    Console.WriteLine($"Found {result.Errors.Count} validation errors:");
    
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  [{error.Code}] {error.MemberName}: {error.Message}");
    }
}
```

---

## ?? Common Patterns

### Validation Pattern

All credit card types follow the same validation pattern:

```csharp
var (result, value) = Type.TryCreate(...);

if (result.IsValid)
{
    // Use the validated value
    ProcessPayment(value);
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

Use builders for flexible construction:

```csharp
var builder = new CreditCardBuilder();

var (result, value) = builder
    .WithCardNumber(cardNumber)
    .WithSecurityCode(cvv)
    .WithExpiration(month, year)
    .Build();

// Reuse the builder
builder.Reset();
var (result2, value2) = builder
    .WithCardNumber(newCardNumber)
    .WithSecurityCode(newCvv)
    .WithExpiration(newExpiration)
    .Build();
```

### JSON Serialization

All types support JSON serialization:

```csharp
using System.Text.Json;

var (_, card) = CreditCardDetails.TryCreate(
    "4111111111111111",
    "123",
    12,
    2025
);

// Serialize
string json = JsonSerializer.Serialize(card);

// Deserialize
var deserialized = JsonSerializer.Deserialize<CreditCardDetails>(json);
```

### Security - Always Mask Sensitive Data

```csharp
var (_, card) = CreditCardDetails.TryCreate(...);

// DON'T: Display full card number
Console.WriteLine($"Card: {card.CardNumber.Value}");

// DO: Use masked version
Console.WriteLine($"Card: {card.GetMaskedCardNumber()}");

// DON'T: Display CVV
Console.WriteLine($"CVV: {card.SecurityNumber.Value}");

// DO: Mask CVV
Console.WriteLine("CVV: ***");

// DO: Use ToString() which includes masking
Console.WriteLine(card.ToString());
// "Card: ************1111, Expires: 12/25, CVV: ***"
```

---

## ?? Real-World Examples

### Payment Processing

```csharp
using Validated.Primitives.Domain;
using Validated.Primitives.Domain.Builders;

public class PaymentProcessor
{
    public async Task<PaymentResult> ProcessPayment(
        decimal amount,
        string cardNumber,
        string cvv,
        int expMonth,
        int expYear)
    {
        // Validate credit card details
        var (validationResult, card) = CreditCardDetails.TryCreate(
            cardNumber,
            cvv,
            expMonth,
            expYear
        );

        if (!validationResult.IsValid)
        {
            return PaymentResult.Failed(
                $"Invalid card details: {validationResult.ToBulletList()}"
            );
        }

        // Check if card is expired
        if (card.IsExpired())
        {
            return PaymentResult.Failed("Card has expired");
        }

        // Log with masked card number for security
        _logger.LogInformation(
            "Processing payment: Amount={Amount}, Card={MaskedCard}, Expires={Expiration}",
            amount,
            card.GetMaskedCardNumber(),
            card.Expiration
        );

        // Process payment with validated card
        return await _paymentGateway.ChargeAsync(amount, card);
    }
}
```

### E-Commerce Checkout

```csharp
public class CheckoutService
{
    public async Task<CheckoutResult> CompleteCheckout(CheckoutRequest request)
    {
        // Build credit card from form data
        var (cardResult, card) = new CreditCardBuilder()
            .WithCardNumber(request.CardNumber)
            .WithSecurityCode(request.Cvv)
            .WithExpiration(request.ExpirationDate)  // "12/25" format
            .Build();

        if (!cardResult.IsValid)
        {
            return CheckoutResult.ValidationFailed(cardResult.Errors);
        }

        // Verify card is not expired
        if (card.IsExpired())
        {
            return CheckoutResult.Failed("Your card has expired. Please use a different card.");
        }

        // Process the order
        var order = await CreateOrder(request);
        
        // Charge the card
        var paymentResult = await ChargeCard(card, order.Total);

        if (paymentResult.Success)
        {
            // Log successful payment (with masked card)
            await LogPayment(order.Id, card.GetMaskedCardNumber(), order.Total);
            
            return CheckoutResult.Success(order.Id);
        }

        return CheckoutResult.Failed(paymentResult.ErrorMessage);
    }

    private async Task LogPayment(string orderId, string maskedCard, decimal amount)
    {
        // Always use masked card number in logs and database
        await _auditLog.RecordPayment(new PaymentAudit
        {
            OrderId = orderId,
            MaskedCardNumber = maskedCard,  // e.g., "************1111"
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

### Subscription Management

```csharp
public class SubscriptionService
{
    public async Task<UpdatePaymentResult> UpdatePaymentMethod(
        string userId,
        string cardNumber,
        string cvv,
        string expiration)  // "MM/YY" format
    {
        // Validate new payment method
        var builder = new CreditCardBuilder();
        var (result, card) = builder
            .WithCardNumber(cardNumber)
            .WithSecurityCode(cvv)
            .WithExpiration(expiration)
            .Build();

        if (!result.IsValid)
        {
            return UpdatePaymentResult.ValidationFailed(result.Errors);
        }

        // Check expiration is far enough in the future
        var monthsUntilExpiry = GetMonthsUntilExpiry(card.Expiration);
        if (monthsUntilExpiry < 3)
        {
            return UpdatePaymentResult.Failed(
                "Card expires too soon. Please use a card valid for at least 3 months."
            );
        }

        // Store card details (encrypted)
        await StoreEncryptedCardDetails(userId, card);

        // Verify card with test charge
        var verificationResult = await VerifyCard(card);
        
        if (!verificationResult.Success)
        {
            return UpdatePaymentResult.Failed(
                "Unable to verify card. Please check your card details."
            );
        }

        return UpdatePaymentResult.Success();
    }

    private int GetMonthsUntilExpiry(CreditCardExpiration expiration)
    {
        var now = DateTime.UtcNow;
        var expiryDate = new DateTime(expiration.Year, expiration.Month, 1);
        return ((expiryDate.Year - now.Year) * 12) + expiryDate.Month - now.Month;
    }

    private async Task StoreEncryptedCardDetails(string userId, CreditCardDetails card)
    {
        // IMPORTANT: Never store raw card numbers or CVV
        // This is a simplified example - follow PCI DSS compliance in production
        
        var encryptedData = new
        {
            MaskedCardNumber = card.GetMaskedCardNumber(),  // Only last 4 digits visible
            ExpirationMonth = card.Expiration.Month,
            ExpirationYear = card.Expiration.Year,
            // CVV should NEVER be stored - not included here
            // Full card number should be tokenized via payment gateway
        };

        await _database.SavePaymentMethod(userId, encryptedData);
    }
}
```

### Card Validation API

```csharp
[ApiController]
[Route("api/cards")]
public class CardValidationController : ControllerBase
{
    [HttpPost("validate")]
    public IActionResult ValidateCard([FromBody] CardValidationRequest request)
    {
        var (result, card) = CreditCardDetails.TryCreate(
            request.CardNumber,
            request.Cvv,
            request.ExpirationMonth,
            request.ExpirationYear
        );

        if (!result.IsValid)
        {
            return BadRequest(new
            {
                Valid = false,
                Errors = result.Errors.Select(e => new
                {
                    Field = e.MemberName,
                    Message = e.Message,
                    Code = e.Code
                })
            });
        }

        // Additional checks
        var warnings = new List<string>();
        
        if (card.IsExpired())
        {
            warnings.Add("Card has expired");
        }

        var monthsUntilExpiry = GetMonthsUntilExpiry(card.Expiration);
        if (monthsUntilExpiry < 6)
        {
            warnings.Add($"Card expires in {monthsUntilExpiry} months");
        }

        return Ok(new
        {
            Valid = true,
            MaskedCardNumber = card.GetMaskedCardNumber(),
            Expiration = card.Expiration.ToString(),
            Warnings = warnings
        });
    }

    private int GetMonthsUntilExpiry(CreditCardExpiration expiration)
    {
        var now = DateTime.UtcNow;
        var expiryDate = new DateTime(expiration.Year, expiration.Month, 1);
        return ((expiryDate.Year - now.Year) * 12) + expiryDate.Month - now.Month;
    }
}

public class CardValidationRequest
{
    public string CardNumber { get; set; }
    public string Cvv { get; set; }
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
}
```

### Secure Display Component

```csharp
public class CreditCardDisplay
{
    public string GetSafeDisplayHtml(CreditCardDetails card)
    {
        // NEVER display full card number or CVV
        var html = new StringBuilder();
        
        html.AppendLine("<div class='credit-card-display'>");
        html.AppendLine($"  <div class='card-number'>{card.GetMaskedCardNumber()}</div>");
        html.AppendLine($"  <div class='card-expiry'>Expires: {card.Expiration}</div>");
        html.AppendLine($"  <div class='card-cvv'>CVV: ***</div>");
        html.AppendLine("</div>");
        
        return html.ToString();
    }

    public void LogCardUsage(ILogger logger, CreditCardDetails card, string action)
    {
        // Always use masked card number in logs
        logger.LogInformation(
            "Card action: {Action}, Card: {MaskedCard}, Expires: {Expiration}",
            action,
            card.GetMaskedCardNumber(),
            card.Expiration
        );
    }

    public string GetReceiptText(CreditCardDetails card, decimal amount)
    {
        return $"""
            Payment Receipt
            ---------------
            Amount: ${amount:F2}
            Card: {card.GetMaskedCardNumber()}
            Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}
            
            Thank you for your payment!
            """;
    }
}
```

---

## ?? Related Documentation

- [Builder Examples](builders_examples.md) - General builder pattern usage
- [Main README](../README.md) - Complete library overview

---

## ?? API Reference

For complete API documentation, see:
- XML documentation comments in source code
- IntelliSense in Visual Studio
- [Validated.Primitives Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)
- [Validated.Primitives.Domain Source](https://github.com/robertmayordomo/Validated.PrimitivesV1)

---

## ?? Security Best Practices

### 1. Never Log or Display Full Card Numbers

```csharp
var (_, card) = CreditCardDetails.TryCreate(...);

// ? DON'T: Log full card number
_logger.LogInformation($"Processing card: {card.CardNumber.Value}");

// ? DO: Always use masked version
_logger.LogInformation($"Processing card: {card.GetMaskedCardNumber()}");
```

### 2. Never Store CVV/CVC

```csharp
// ? DON'T: Store CVV in database
await _database.SaveCard(new {
    CardNumber = card.CardNumber.Value,
    CVV = card.SecurityNumber.Value  // NEVER DO THIS!
});

// ? DO: Never store CVV (per PCI DSS)
await _database.SaveCard(new {
    MaskedCardNumber = card.GetMaskedCardNumber(),
    ExpirationMonth = card.Expiration.Month,
    ExpirationYear = card.Expiration.Year
    // No CVV stored!
});
```

### 3. Use Card Tokenization

```csharp
// ? DO: Use payment gateway tokenization
var token = await _paymentGateway.TokenizeCard(card);

// Store only the token, not the card
await _database.SaveCard(new {
    UserId = userId,
    CardToken = token,  // Payment gateway token
    MaskedCardNumber = card.GetMaskedCardNumber(),
    Expiration = card.Expiration
});
```

### 4. Validate Before Processing

```csharp
// ? DO: Always validate before processing
var (result, card) = CreditCardDetails.TryCreate(...);
if (!result.IsValid)
{
    return Error("Invalid card");
}

// ? DO: Check expiration
if (card.IsExpired())
{
    return Error("Card expired");
}

ProcessPayment(card);
```

### 5. Use HTTPS and Encryption

```csharp
// ? DO: Only transmit card data over HTTPS
// ? DO: Use TLS 1.2 or higher
// ? DO: Encrypt sensitive data at rest
// ? DO: Follow PCI DSS compliance requirements
```

### 6. Implement Audit Logging

```csharp
// ? DO: Log all card-related actions
_auditLogger.LogCardAction(new CardAudit
{
    UserId = currentUser.Id,
    Action = "PaymentProcessed",
    MaskedCard = card.GetMaskedCardNumber(),
    Amount = amount,
    Timestamp = DateTime.UtcNow,
    Success = true
});
```

### 7. Secure Card Data in Transit

```csharp
// ? DO: Use encrypted payloads for API calls
public class SecurePaymentRequest
{
    [Encrypted]
    public string CardNumber { get; set; }
    
    [Encrypted]
    public string CVV { get; set; }
    
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
}
```

### 8. PCI DSS Compliance Reminders

```csharp
/*
 * PCI DSS Requirements for Card Data:
 * 
 * ? DO:
 * - Mask card numbers (show last 4 digits only)
 * - Encrypt card data in transit (TLS 1.2+)
 * - Use tokenization for stored cards
 * - Log all access to card data
 * - Implement strong access controls
 * - Regularly update security patches
 * 
 * ? DON'T:
 * - Store CVV/CVC codes (forbidden by PCI DSS)
 * - Store full track data
 * - Store PIN numbers
 * - Log full card numbers
 * - Display full card numbers
 * - Transmit card data unencrypted
 */
```
