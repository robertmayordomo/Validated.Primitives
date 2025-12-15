# Builder Examples

Examples for `AddressBuilder`, `CreditCardBuilder`, and `BankingDetailsBuilder`.

## AddressBuilder

```csharp
var (result, address) = new AddressBuilder()
    .WithStreet("123 Main Street")
    .WithCity("New York")
    .WithCountry(CountryCode.UnitedStates)
    .WithPostalCode("10001")
    .Build();

if (result.IsValid)
    Console.WriteLine(address.ToString());
```

## CreditCardBuilder

```csharp
var (result, card) = new CreditCardBuilder()
    .WithCardNumber("4111 1111 1111 1111")
    .WithSecurityCode("123")
    .WithExpiration(12, 2025)
    .Build();
```

## BankingDetailsBuilder

```csharp
var (result, bankingDetails) = new BankingDetailsBuilder()
    .WithCountry(CountryCode.UnitedStates)
    .WithAccountNumber("123456789")
    .Build();
```
