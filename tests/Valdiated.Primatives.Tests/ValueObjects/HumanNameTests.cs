using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class HumanNameTests
{
    public static TheoryData<string> ValidNames => new()
    {
        "John",
        "Mary-Jane",
        "O'Brien",
        "D'Angelo",
        "Anne-Marie",
        "Mary-Anne-Elizabeth",
        "O'Malley-Smith",
        "J",
        "a",
        new string('a', 50) // Max length
    };

    public static TheoryData<string, string> InvalidNamesWithReason => new()
    {
        { "", "Empty string" },
        { "   ", "Whitespace only" },
        { "John123", "Contains numbers" },
        { "John@Doe", "Contains special characters" },
        { "John Doe", "Contains space" },
        { "John.Doe", "Contains period" },
        { new string('a', 51), "Exceeds max length" },
        { "John_Doe", "Contains underscore" },
        { "John!Doe", "Contains exclamation" }
    };

    [Theory]
    [MemberData(nameof(ValidNames))]
    public void TryCreate_Succeeds_For_Valid_Names(string name)
    {
        var (result, value) = HumanName.TryCreate(name);
        
        result.IsValid.ShouldBeTrue($"Result should be valid for: {name}");
        value.ShouldNotBeNull($"Value should not be null for: {name}");
        value!.Value.ShouldBe(name.Trim());
    }

    [Theory]
    [MemberData(nameof(InvalidNamesWithReason))]
    public void TryCreate_Fails_For_Invalid_Names(string name, string reason)
    {
        var (result, value) = HumanName.TryCreate(name);
        
        result.IsValid.ShouldBeFalse($"Result should be invalid for: {reason}");
        value.ShouldBeNull($"Value should be null for: {reason}");
        result.Errors.ShouldNotBeEmpty($"Should have errors for: {reason}");
    }

    [Fact]
    public void TryCreate_Trims_Whitespace()
    {
        var (result, value) = HumanName.TryCreate("  John  ");
        
        result.IsValid.ShouldBeTrue();
        value.ShouldNotBeNull();
        value!.Value.ShouldBe("John");
    }

    [Fact]
    public void TryCreate_Null_Returns_Invalid()
    {
        var (result, value) = HumanName.TryCreate(null!);
        
        result.IsValid.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Theory]
    [InlineData("O'Brien")]
    [InlineData("O'Malley")]
    [InlineData("D'Angelo")]
    public void TryCreate_Accepts_Irish_Names_With_Apostrophe(string name)
    {
        var (result, value) = HumanName.TryCreate(name);
        
        result.IsValid.ShouldBeTrue($"Should accept Irish name: {name}");
        value.ShouldNotBeNull();
        value!.Value.ShouldBe(name);
    }

    [Theory]
    [InlineData("Mary-Jane")]
    [InlineData("Anne-Marie")]
    [InlineData("Smith-Jones")]
    public void TryCreate_Accepts_Hyphenated_Names(string name)
    {
        var (result, value) = HumanName.TryCreate(name);
        
        result.IsValid.ShouldBeTrue($"Should accept hyphenated name: {name}");
        value.ShouldNotBeNull();
        value!.Value.ShouldBe(name);
    }

    [Fact]
    public void TryCreate_Uses_Custom_Property_Name_In_Error()
    {
        var (result, _) = HumanName.TryCreate("", "CustomField");
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.MemberName == "CustomField");
    }

    [Fact]
    public void ToString_Returns_Value()
    {
        var (_, name) = HumanName.TryCreate("O'Brien");
        
        name.ShouldNotBeNull();
        name!.ToString().ShouldBe("O'Brien");
    }

    [Fact]
    public void ImplicitConversion_To_String_Works()
    {
        var (_, name) = HumanName.TryCreate("John");
        
        name.ShouldNotBeNull();
        string nameString = name!;
        nameString.ShouldBe("John");
    }

    [Fact]
    public void Equality_Same_Values_Are_Equal()
    {
        var (_, name1) = HumanName.TryCreate("John");
        var (_, name2) = HumanName.TryCreate("John");
        
        name1.ShouldBe(name2);
    }

    [Fact]
    public void Equality_Different_Values_Are_Not_Equal()
    {
        var (_, name1) = HumanName.TryCreate("John");
        var (_, name2) = HumanName.TryCreate("Jane");
        
        name1.ShouldNotBe(name2);
    }

    [Fact]
    public void Equality_Case_Sensitive()
    {
        var (_, name1) = HumanName.TryCreate("John");
        var (_, name2) = HumanName.TryCreate("john");
        
        name1.ShouldNotBe(name2);
    }
}

