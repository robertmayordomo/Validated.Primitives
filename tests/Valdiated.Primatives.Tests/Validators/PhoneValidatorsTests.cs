using Xunit;
using Shouldly;
using Validated.Primitives.Validators;

namespace Validated.Primitives.Tests.Validators;

public class PhoneValidatorsTests
{
    public static TheoryData<string> ValidPhones => new()
    {
        "+14155552671",
        "14155552671",
        "+447911123456",
        "+61412345678",
        "+862012345678",
        "+12025551234",
        "12025551234",
        "+919876543210",
        "+551123456789"
    };

    public static TheoryData<string> InvalidPhones => new()
    {
        "",
        "   ",
        "123",
        "phone",
        "+0123456789",      // Starts with 0
        "0123456789",       // Starts with 0
        "+1234",            // Too short
        "+12345678901234567", // Too long
        "abc123456789",     // Contains letters
        "+1 415 555 2671",  // Contains spaces
        "+1-415-555-2671",  // Contains hyphens
        "(415) 555-2671",   // Contains parentheses
        "++14155552671",    // Double plus
        "+",                // Just plus sign
        "1234567890123456"  // Too long without +
    };

    [Theory]
    [MemberData(nameof(ValidPhones))]
    public void PhoneNumber_Allows_Valid(string phone)
    {
        var result = PhoneValidators.PhoneNumber()(phone);
        result.IsValid.ShouldBeTrue($"Expected valid phone: {phone}");
    }

    [Theory]
    [MemberData(nameof(InvalidPhones))]
    public void PhoneNumber_Fails_Invalid(string phone)
    {
        var result = PhoneValidators.PhoneNumber()(phone);
        result.IsValid.ShouldBeFalse($"Expected invalid phone: {phone}");
    }

    [Fact]
    public void PhoneNumber_Returns_Correct_Error_Message()
    {
        var result = PhoneValidators.PhoneNumber("PhoneField")("invalid");
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Message.ShouldBe("Invalid phone number format.");
        result.Errors[0].MemberName.ShouldBe("PhoneField");
        result.Errors[0].Code.ShouldBe("PhoneNumber");
    }

    [Fact]
    public void PhoneNumber_Uses_Default_FieldName_When_Not_Provided()
    {
        var result = PhoneValidators.PhoneNumber()("invalid");
        result.Errors[0].MemberName.ShouldBe("PhoneNumber");
    }

    [Theory]
    [InlineData("+14155552671", "ContactPhone")]
    [InlineData("+447911123456", "MobileNumber")]
    public void PhoneNumber_Validates_With_Custom_FieldName(string phone, string fieldName)
    {
        var result = PhoneValidators.PhoneNumber(fieldName)(phone);
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void PhoneNumber_Handles_Null_Or_Empty(string? phone)
    {
        var result = PhoneValidators.PhoneNumber()(phone!);
        result.IsValid.ShouldBeFalse();
    }
}
