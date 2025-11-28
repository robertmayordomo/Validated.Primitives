using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class PhoneNumberTests
{
    public static TheoryData<CountryCode, string> ValidPhones => new()
    {
        { CountryCode.UnitedStates, "+14155552671" },
        { CountryCode.UnitedStates, "14155552671" },
        { CountryCode.UnitedKingdom, "+447911123456" },
        { CountryCode.Canada, "+14165551234" },
        { CountryCode.Australia, "+61412345678" },
        { CountryCode.Germany, "+4915112345678" }
    };

    public static TheoryData<CountryCode, string> InvalidPhones => new()
    {
        { CountryCode.UnitedStates, "" },
        { CountryCode.UnitedStates, "123" },
        { CountryCode.UnitedStates, "phone" },
        { CountryCode.UnitedKingdom, "123" },
        { CountryCode.Canada, "abc" }
    };

    [Theory]
    [MemberData(nameof(ValidPhones))]
    public void TryCreate_Succeeds_For_Valid(CountryCode countryCode, string phone)
    {
        var (res, val) = PhoneNumber.TryCreate(countryCode, phone);
        res.IsValid.ShouldBeTrue($"Result should be valid for valid phone number: {phone} with country code: {countryCode}");
        val.ShouldNotBeNull($"Value should not be null when validation succeeds for: {phone}");
    }

    [Theory]
    [MemberData(nameof(InvalidPhones))]
    public void TryCreate_Fails_For_Invalid(CountryCode countryCode, string phone)
    {
        var (res, val) = PhoneNumber.TryCreate(countryCode, phone);
        res.IsValid.ShouldBeFalse($"Result should be invalid for invalid phone number: {phone} with country code: {countryCode}");
        val.ShouldBeNull($"Value should be null when validation fails for: {phone}");
    }
}