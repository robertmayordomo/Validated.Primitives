using Shouldly;
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Tests;

public class AddressTests
{
    [Fact]
    public void TryCreate_Returns_Success_With_Valid_US_Address()
    {
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeTrue("Result should be valid for a valid US address");
        address.ShouldNotBeNull("Address should not be null when validation succeeds");
        address!.Street.Value.ShouldBe("123 Main Street");
        address.AddressLine2.ShouldBeNull();
        address.City.Value.ShouldBe("New York");
        address.StateProvince.ShouldBeNull("StateProvince should be null when not provided");
        address.PostalCode.Value.ShouldBe("10001");
        address.Country.ShouldBe(CountryCode.UnitedStates);
    }

    [Fact]
    public void TryCreate_Returns_Success_With_AddressLine2()
    {
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            "Apt 4B",
            "New York",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeTrue("Result should be valid with address line 2");
        address.ShouldNotBeNull();
        address!.Street.Value.ShouldBe("123 Main Street");
        address.AddressLine2.ShouldNotBeNull();
        address.AddressLine2!.Value.ShouldBe("Apt 4B");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_StateProvince()
    {
        var (result, address) = Address.TryCreate(
            "456 Oak Avenue",
            null,
            "Los Angeles",
            CountryCode.UnitedStates,
            "90001",
            "CA");

        result.IsValid.ShouldBeTrue("Result should be valid when state is provided");
        address.ShouldNotBeNull();
        address!.StateProvince.ShouldNotBeNull();
        address.StateProvince!.Value.ShouldBe("CA");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_UK_Address()
    {
        var (result, address) = Address.TryCreate(
            "10 Downing Street",
            null,
            "London",
            CountryCode.UnitedKingdom,
            "SW1A 2AA");

        result.IsValid.ShouldBeTrue("Result should be valid for a valid UK address");
        address.ShouldNotBeNull();
        address!.PostalCode.Value.ShouldBe("SW1A 2AA");
        address.Country.ShouldBe(CountryCode.UnitedKingdom);
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_Street_Is_Empty()
    {
        var (result, address) = Address.TryCreate(
            "",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeFalse("Result should be invalid when street is empty");
        address.ShouldBeNull("Address should be null when validation fails");
        result.Errors.ShouldContain(e => e.MemberName == "Street" && e.Code == "Required");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_Street_Is_Whitespace()
    {
        var (result, address) = Address.TryCreate(
            "   ",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeFalse("Result should be invalid when street is whitespace");
        address.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_Street_Exceeds_MaxLength()
    {
        var longStreet = new string('a', 201);
        var (result, address) = Address.TryCreate(
            longStreet,
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeFalse("Result should be invalid when street exceeds 200 characters");
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Street" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_City_Is_Empty()
    {
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            null,
            "",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeFalse("Result should be invalid when city is empty");
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "City");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_City_Exceeds_MaxLength()
    {
        var longCity = new string('b', 101);
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            null,
            longCity,
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeFalse("Result should be invalid when city exceeds 100 characters");
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "City" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_StateProvince_Exceeds_MaxLength()
    {
        var longState = new string('c', 101);
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001",
            longState);

        result.IsValid.ShouldBeFalse("Result should be invalid when state/province exceeds 100 characters");
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "StateProvince" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_PostalCode_Is_Invalid()
    {
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "INVALID");

        result.IsValid.ShouldBeFalse("Result should be invalid when postal code is invalid");
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_AddressLine2_Exceeds_MaxLength()
    {
        var longLine2 = new string('d', 201);
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            longLine2,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        result.IsValid.ShouldBeFalse("Result should be invalid when address line 2 exceeds 200 characters");
        address.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "AddressLine2" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Trims_Whitespace_From_Inputs()
    {
        var (result, address) = Address.TryCreate(
            "  123 Main Street  ",
            null,
            "  New York  ",
            CountryCode.UnitedStates,
            "10001",
            "  CA  ");

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.Street.Value.ShouldBe("123 Main Street");
        address.City.Value.ShouldBe("New York");
        address.StateProvince.Value.ShouldBe("CA");
    }

    [Fact]
    public void TryCreate_Returns_Multiple_Errors_When_Multiple_Fields_Invalid()
    {
        var (result, address) = Address.TryCreate(
            "",
            null,
            "",
            CountryCode.UnitedStates,
            "INVALID");

        result.IsValid.ShouldBeFalse();
        address.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThan(1, "Should have multiple validation errors");
        result.Errors.ShouldContain(e => e.MemberName == "Street");
        result.Errors.ShouldContain(e => e.MemberName == "City");
        result.Errors.ShouldContain(e => e.MemberName == "PostalCode");
    }

    [Fact]
    public void Country_Property_Returns_PostalCode_Country()
    {
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            null,
            "Toronto",
            CountryCode.Canada,
            "M5H 2N2");

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.Country.ShouldBe(CountryCode.Canada);
    }

    [Fact]
    public void ToString_Formats_Address_Without_StateProvince_And_AddressLine2()
    {
        var (result, address) = Address.TryCreate(
            "10 Downing Street",
            null,
            "London",
            CountryCode.UnitedKingdom,
            "SW1A 2AA");

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.ToString().ShouldBe("10 Downing Street, London, SW1A 2AA, United Kingdom");
    }

    [Fact]
    public void ToString_Formats_Address_With_StateProvince()
    {
        var (result, address) = Address.TryCreate(
            "1600 Pennsylvania Avenue",
            null,
            "Washington",
            CountryCode.UnitedStates,
            "20500",
            "DC");

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.ToString().ShouldBe("1600 Pennsylvania Avenue, Washington, DC, 20500, United States");
    }

    [Fact]
    public void ToString_Formats_Address_With_AddressLine2()
    {
        var (result, address) = Address.TryCreate(
            "123 Main Street",
            "Suite 100",
            "Boston",
            CountryCode.UnitedStates,
            "02101",
            "MA");

        result.IsValid.ShouldBeTrue();
        address.ShouldNotBeNull();
        address!.ToString().ShouldBe("123 Main Street, Suite 100, Boston, MA, 02101, United States");
    }

    [Fact]
    public void Equality_Same_Values_Are_Equal()
    {
        var (_, address1) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        var (_, address2) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        address1.ShouldBe(address2, "Addresses with same values should be equal");
    }

    [Fact]
    public void Equality_Different_Street_Are_Not_Equal()
    {
        var (_, address1) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        var (_, address2) = Address.TryCreate(
            "456 Oak Avenue",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        address1.ShouldNotBe(address2, "Addresses with different streets should not be equal");
    }

    [Fact]
    public void Equality_Different_City_Are_Not_Equal()
    {
        var (_, address1) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001");

        var (_, address2) = Address.TryCreate(
            "123 Main Street",
            null,
            "Los Angeles",
            CountryCode.UnitedStates,
            "10001");

        address1.ShouldNotBe(address2, "Addresses with different cities should not be equal");
    }

    [Fact]
    public void Equality_Different_StateProvince_Are_Not_Equal()
    {
        var (_, address1) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001",
            "NY");

        var (_, address2) = Address.TryCreate(
            "123 Main Street",
            null,
            "New York",
            CountryCode.UnitedStates,
            "10001",
            "CA");

        address1.ShouldNotBe(address2, "Addresses with different state/provinces should not be equal");
    }

    [Fact]
    public void Equality_Different_AddressLine2_Are_Not_Equal()
    {
        var (_, address1) = Address.TryCreate(
            "123 Main Street",
            "Apt 1",
            "New York",
            CountryCode.UnitedStates,
            "10001");

        var (_, address2) = Address.TryCreate(
            "123 Main Street",
            "Apt 2",
            "New York",
            CountryCode.UnitedStates,
            "10001");

        address1.ShouldNotBe(address2, "Addresses with different address line 2 should not be equal");
    }

    [Fact]
    public void Works_With_Various_Countries()
    {
        var countries = new[]
        {
            (CountryCode.Germany, "123 Hauptstra�e", "Berlin", "10115"),
            (CountryCode.France, "123 Rue de Rivoli", "Paris", "75001"),
            (CountryCode.Japan, "123 Shibuya", "Tokyo", "150-0002"),
            (CountryCode.Australia, "123 George Street", "Sydney", "2000")
        };

        foreach (var (country, street, city, postalCode) in countries)
        {
            var (result, address) = Address.TryCreate(street, null, city, country, postalCode);
            
            result.IsValid.ShouldBeTrue($"Should be valid for {country}");
            address.ShouldNotBeNull();
            address!.Country.ShouldBe(country);
        }
    }
}
