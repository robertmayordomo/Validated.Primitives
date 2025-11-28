using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class EmailAddressTests
{
    public static TheoryData<string> ValidEmails => new()
    {
        "test@example.com",
        "user.name+tag+sorting@example.com",
        "x@x.au"
    };

    public static TheoryData<string> InvalidEmails => new()
    {
        "",
        "not-an-email",
        "a@"
    };

    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void TryCreate_Succeeds_For_Valid(string email)
    {
        var (res, val) = EmailAddress.TryCreate(email);
        res.IsValid.ShouldBeTrue($"Result should be valid for valid email address: {email}");
        val.ShouldNotBeNull($"Value should not be null when validation succeeds for: {email}");
    }

    [Theory]
    [MemberData(nameof(InvalidEmails))]
    public void TryCreate_Fails_For_Invalid(string email)
    {
        var (res, val) = EmailAddress.TryCreate(email);
        res.IsValid.ShouldBeFalse($"Result should be invalid for invalid email address: {email}");
        val.ShouldBeNull($"Value should be null when validation fails for: {email}");
    }
}
