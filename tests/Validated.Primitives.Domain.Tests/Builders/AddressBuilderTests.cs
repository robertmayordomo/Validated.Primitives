using Shouldly;
using Validated.Primitives.Domain.Builders;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Domain.Tests.Builders;

public class AddressBuilderTests
{
    [Fact]
    public void Build_WithValidAddress_ReturnsSuccess()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("123 Main St")
            .WithCity("New York")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("10001")
            .Build();

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.Street.Value.ShouldBe("123 Main St");
        address.City.Value.ShouldBe("New York");
        address.PostalCode.Value.ShouldBe("10001");
        address.Country.ShouldBe(CountryCode.UnitedStates);
    }

    [Fact]
    public void Build_WithCompleteAddress_ReturnsSuccess()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("456 Oak Ave")
            .WithAddressLine2("Apt 4B")
            .WithCity("Los Angeles")
            .WithStateProvince("California")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("90001")
            .Build();

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.Street.Value.ShouldBe("456 Oak Ave");
        address.AddressLine2.ShouldNotBeNull();
        address.AddressLine2!.Value.ShouldBe("Apt 4B");
        address.City.Value.ShouldBe("Los Angeles");
        address.StateProvince.ShouldNotBeNull();
        address.StateProvince!.Value.ShouldBe("California");
        address.PostalCode.Value.ShouldBe("90001");
    }

    [Fact]
    public void Build_WithAddressMethod_ReturnsSuccess()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithAddress("789 Elm St", "Chicago", CountryCode.UnitedStates, "60601")
            .Build();

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.Street.Value.ShouldBe("789 Elm St");
        address.City.Value.ShouldBe("Chicago");
        address.PostalCode.Value.ShouldBe("60601");
    }

    [Fact]
    public void Build_WithAddressMethodIncludingOptionals_ReturnsSuccess()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithAddress(
                "321 Pine St",
                "Seattle",
                CountryCode.UnitedStates,
                "98101",
                "Suite 200",
                "Washington")
            .Build();

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.Street.Value.ShouldBe("321 Pine St");
        address.AddressLine2!.Value.ShouldBe("Suite 200");
        address.City.Value.ShouldBe("Seattle");
        address.StateProvince!.Value.ShouldBe("Washington");
        address.PostalCode.Value.ShouldBe("98101");
    }

    [Fact]
    public void Build_MissingStreet_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithCity("Boston")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("02101")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Street" && e.Code == "Required");
    }

    [Fact]
    public void Build_MissingCity_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("555 Broadway")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("10012")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "City" && e.Code == "Required");
    }

    [Fact]
    public void Build_MissingPostalCode_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("777 Market St")
            .WithCity("San Francisco")
            .WithCountry(CountryCode.UnitedStates)
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode" && e.Code == "Required");
    }

    [Fact]
    public void Build_MissingCountry_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("888 Fifth Ave")
            .WithCity("New York")
            .WithPostalCode("10022")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Country" && e.Code == "Required");
    }

    [Fact]
    public void Build_InvalidPostalCode_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("999 Park Ave")
            .WithCity("Miami")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("INVALID")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode");
    }

    [Fact]
    public void Build_MultipleErrors_ReturnsAllErrors()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(3);
        result.Errors.ShouldContain(e => e.MemberName == "Street");
        result.Errors.ShouldContain(e => e.MemberName == "City");
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode");
    }

    [Fact]
    public void Reset_ClearsAllFields()
    {
        var builder = new AddressBuilder();
        builder
            .WithStreet("123 Test St")
            .WithAddressLine2("Apt 1")
            .WithCity("Test City")
            .WithStateProvince("Test State")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("12345")
            .Reset();

        var (result, address) = builder.Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Street");
        result.Errors.ShouldContain(e => e.MemberName == "City");
    }

    [Fact]
    public void Build_CanBeCalledMultipleTimes()
    {
        var builder = new AddressBuilder()
            .WithStreet("111 First St")
            .WithCity("Denver")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("80201");

        var (result1, address1) = builder.Build();
        var (result2, address2) = builder.Build();

        result1.IsValid.ShouldBeTrue();
        result2.IsValid.ShouldBeTrue();
        address1.ShouldNotBeNull();
        address2.ShouldNotBeNull();
        address1!.Street.Value.ShouldBe(address2!.Street.Value);
    }

    [Fact]
    public void Build_WithDifferentCountries_ValidatesCorrectly()
    {
        // UK Address
        var ukBuilder = new AddressBuilder();
        var (ukResult, ukAddress) = ukBuilder
            .WithStreet("10 Downing Street")
            .WithCity("London")
            .WithCountry(CountryCode.UnitedKingdom)
            .WithPostalCode("SW1A 2AA")
            .Build();

        ukResult.IsValid.ShouldBeTrue();
        ukAddress.ShouldNotBeNull();
        ukAddress!.Country.ShouldBe(CountryCode.UnitedKingdom);

        // Canadian Address
        var caBuilder = new AddressBuilder();
        var (caResult, caAddress) = caBuilder
            .WithStreet("24 Sussex Drive")
            .WithCity("Ottawa")
            .WithStateProvince("Ontario")
            .WithCountry(CountryCode.Canada)
            .WithPostalCode("K1M 1M4")
            .Build();

        caResult.IsValid.ShouldBeTrue();
        caAddress.ShouldNotBeNull();
        caAddress!.Country.ShouldBe(CountryCode.Canada);
    }

    [Fact]
    public void Build_WithEmptyStrings_TreatsAsRequired()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("")
            .WithCity("")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Street");
        result.Errors.ShouldContain(e => e.MemberName == "City");
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode");
    }

    [Fact]
    public void Build_WithWhitespaceStrings_TreatsAsRequired()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("   ")
            .WithCity("   ")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("   ")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
    }

    [Fact]
    public void FluentInterface_ReturnsBuilderForChaining()
    {
        var builder = new AddressBuilder();
        
        var returnedBuilder = builder
            .WithStreet("123 Main St")
            .WithAddressLine2("Apt 1")
            .WithCity("Test City")
            .WithStateProvince("Test State")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("12345");

        returnedBuilder.ShouldBe(builder);
    }

    [Fact]
    public void Build_WithNullOptionalFields_AllowsNulls()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("222 Second Ave")
            .WithAddressLine2(null)
            .WithCity("Portland")
            .WithStateProvince(null)
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("97201")
            .Build();

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.AddressLine2.ShouldBeNull();
        address.StateProvince.ShouldBeNull();
    }

    [Fact]
    public void Reset_ReturnsBuilderForChaining()
    {
        var builder = new AddressBuilder();
        var returnedBuilder = builder.Reset();

        returnedBuilder.ShouldBe(builder);
    }

    [Fact]
    public void Build_WithNullStreet_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet(null)
            .WithCity("Boston")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("02101")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Street" && e.Code == "Required");
    }

    [Fact]
    public void Build_WithNullCity_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("123 Main St")
            .WithCity(null)
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode("10001")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "City" && e.Code == "Required");
    }

    [Fact]
    public void Build_WithNullPostalCode_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("456 Oak Ave")
            .WithCity("Los Angeles")
            .WithCountry(CountryCode.UnitedStates)
            .WithPostalCode(null)
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode" && e.Code == "Required");
    }

    [Fact]
    public void Build_WithNullCountry_ReturnsFailure()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithStreet("789 Elm St")
            .WithCity("Chicago")
            .WithCountry(null)
            .WithPostalCode("60601")
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Country" && e.Code == "Required");
    }

    [Fact]
    public void WithAddress_WithNullValues_AcceptsNulls()
    {
        var builder = new AddressBuilder();
        var (result, address) = builder
            .WithAddress(null, null, null, null)
            .Build();

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(3);
    }
}
