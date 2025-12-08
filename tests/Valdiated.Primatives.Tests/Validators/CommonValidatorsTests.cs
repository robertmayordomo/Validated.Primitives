using Xunit;
using Shouldly;
using Validated.Primitives.Validators;

namespace Validated.Primitives.Tests.Validators;

public class CommonValidatorsTests
{
    public static TheoryData<string?> NotNullOrWhitespace_Invalid_Data => new()
    {
        null,
        string.Empty,
        "   ",
        "\t",
        "\n",
        "\r\n",
        "     \t  "
    };

    public static TheoryData<string> NotNullOrWhitespace_Valid_Data => new()
    {
        "a",
        "hello",
        " hello ",
        "test123",
        "!@#$%"
    };

    [Theory]
    [MemberData(nameof(NotNullOrWhitespace_Invalid_Data))]
    public void GivenNotNullOrWhitespace_WhenNullOrWhitespace_ThenShouldFail(string? input)
    {
        var result = CommonValidators.NotNullOrWhitespace("Field")(input!);
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Message.ShouldContain("cannot be null or whitespace");
    }

    [Theory]
    [MemberData(nameof(NotNullOrWhitespace_Valid_Data))]
    public void GivenNotNullOrWhitespace_WhenNonEmpty_ThenShouldSucceed(string input)
    {
        var result = CommonValidators.NotNullOrWhitespace("Field")(input);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenNotNullOrWhitespace_WhenNull_ThenShouldReturnCorrectErrorDetails()
    {
        var result = CommonValidators.NotNullOrWhitespace("UserName")(null!);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("UserName");
        result.Errors[0].Code.ShouldBe("NotNullOrWhitespace");
    }

    [Theory]
    [InlineData(5, "abcdef")]
    [InlineData(5, "abcdefgh")]
    [InlineData(10, "this is a very long string")]
    public void GivenMaxLength_WhenTooLong_ThenShouldFail(int maxLength, string input)
    {
        var validator = CommonValidators.MaxLength("Field", maxLength);
        var result = validator(input);
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Message.ShouldContain($"at most {maxLength}");
    }

    [Theory]
    [InlineData(5, "abc")]
    [InlineData(5, "abcde")]
    [InlineData(10, "short")]
    public void GivenMaxLength_WhenWithinLimit_ThenShouldSucceed(int maxLength, string input)
    {
        var validator = CommonValidators.MaxLength("Field", maxLength);
        var result = validator(input);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenMaxLength_WhenExactLength_ThenShouldSucceed()
    {
        var validator = CommonValidators.MaxLength("Field", 5);
        var result = validator("hello");
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenMaxLength_WhenNull_ThenShouldSucceed()
    {
        var validator = CommonValidators.MaxLength("Field", 5);
        var result = validator(null!);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenMaxLength_WhenTooLong_ThenShouldReturnCorrectErrorDetails()
    {
        var validator = CommonValidators.MaxLength("Description", 10);
        var result = validator("This is too long");
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("Description");
        result.Errors[0].Code.ShouldBe("MaxLength");
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("abc")]
    public void GivenMaxLength_WhenEmptyOrShortStrings_ThenShouldSucceed(string input)
    {
        var validator = CommonValidators.MaxLength("Field", 5);
        var result = validator(input);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenMaxLengthZero_WhenNonEmpty_ThenShouldFail()
    {
        var validator = CommonValidators.MaxLength("Field", 0);
        var result = validator("a");
        result.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GivenMaxLengthZero_WhenEmpty_ThenShouldSucceed()
    {
        var validator = CommonValidators.MaxLength("Field", 0);
        var result = validator("");
        result.IsValid.ShouldBeTrue();
    }
}
