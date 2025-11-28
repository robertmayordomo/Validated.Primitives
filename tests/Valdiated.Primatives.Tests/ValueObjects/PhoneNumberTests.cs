using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class PhoneNumberTests
{
    public static TheoryData<string> ValidPhones => new()
    {
        "+14155552671",
        "14155552671",
        "+447911123456"
    };

    public static TheoryData<string> InvalidPhones => new()
    {
        "",
        "123",
        "phone"
    };

    [Theory]
    [MemberData(nameof(ValidPhones))]
    public void TryCreate_Succeeds_For_Valid(string phone)
    {
        var (res, val) = PhoneNumber.TryCreate(phone);
        res.IsValid.ShouldBeTrue($"Result should be valid for valid phone number: {phone}");
        val.ShouldNotBeNull($"Value should not be null when validation succeeds for: {phone}");
    }

    [Theory]
    [MemberData(nameof(InvalidPhones))]
    public void TryCreate_Fails_For_Invalid(string phone)
    {
        var (res, val) = PhoneNumber.TryCreate(phone);
        res.IsValid.ShouldBeFalse($"Result should be invalid for invalid phone number: {phone}");
        val.ShouldBeNull($"Value should be null when validation fails for: {phone}");
    }
}
