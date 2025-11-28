using Xunit;
using Shouldly;
using Validated.Primitives.Validators;

namespace Validated.Primitives.Tests.Validators;

public class EmailValidatorsTests
{
    public static TheoryData<string> ValidEmails => new()
    {
        "test@example.com",
        "user.name+tag+sorting@example.com",
        "x@x.au",
        "user_name@example.com",
        "user-name@example.com",
        "user123@example.co.uk",
        "user@subdomain.example.com",
        "first.last@example.com",
        "a@example.com",
        "test.email.with+symbol@example4u.net",
        "user!#$%&'*+/=?^_`{|}~@example.org"
    };

    public static TheoryData<string> InvalidEmails => new()
    {
        "",
        "   ",
        "not-an-email",
        "user@.com",
        "@example.com",
        "user@",
        "user @example.com",
        "user@example",
        "user..name@example.com",
        ".user@example.com",
        "user.@example.com",
        "user@.example.com",
        "user@example..com",
        "user@example.com.",
        "user@-example.com",
        "user@example-.com",
        "user name@example.com",
        "user@exam ple.com",
        "user@@example.com"
    };

    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void EmailFormat_Allows_Valid_Emails(string email)
    {
        var result = EmailValidators.EmailFormat()(email);
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidEmails))]
    public void EmailFormat_Fails_Invalid_Emails(string email)
    {
        var result = EmailValidators.EmailFormat("Email")(email);
        result.IsValid.ShouldBeFalse($"Expected invalid email: {email}");
    }

    [Fact]
    public void EmailFormat_Returns_Correct_Error_Message()
    {
        var result = EmailValidators.EmailFormat("EmailField")("invalid");
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Message.ShouldBe("Invalid email address format.");
        result.Errors[0].MemberName.ShouldBe("EmailField");
        result.Errors[0].Code.ShouldBe("EmailFormat");
    }

    [Fact]
    public void EmailFormat_Uses_Default_FieldName_When_Not_Provided()
    {
        var result = EmailValidators.EmailFormat()("invalid");
        result.Errors[0].MemberName.ShouldBe("Email");
    }

    [Theory]
    [InlineData("test@example.com", "Email")]
    [InlineData("user@test.co.uk", "UserEmail")]
    public void EmailFormat_Validates_With_Custom_FieldName(string email, string fieldName)
    {
        var result = EmailValidators.EmailFormat(fieldName)(email);
        result.IsValid.ShouldBeTrue();
    }
}
