using Shouldly;
using Validated.Primitives.Domain;

namespace Validated.Primitives.Domain.Tests;

public class PersonNameTests
{
    [Fact]
    public void TryCreate_Returns_Success_With_FirstName_And_LastName()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe");

        result.IsValid.ShouldBeTrue("Result should be valid with first and last name");
        name.ShouldNotBeNull("PersonName should not be null when validation succeeds");
        name!.FirstName.ShouldBe("John");
        name.LastName.ShouldBe("Doe");
        name.MiddleName.ShouldBeNull("MiddleName should be null when not provided");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_MiddleName()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe", "Michael");

        result.IsValid.ShouldBeTrue("Result should be valid with middle name");
        name.ShouldNotBeNull();
        name!.FirstName.ShouldBe("John");
        name.MiddleName.ShouldBe("Michael");
        name.LastName.ShouldBe("Doe");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_FirstName_Is_Empty()
    {
        var (result, name) = PersonName.TryCreate("", "Doe");

        result.IsValid.ShouldBeFalse("Result should be invalid when first name is empty");
        name.ShouldBeNull("PersonName should be null when validation fails");
        result.Errors.ShouldContain(e => e.MemberName == "FirstName" && e.Code == "Required");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_FirstName_Is_Whitespace()
    {
        var (result, name) = PersonName.TryCreate("   ", "Doe");

        result.IsValid.ShouldBeFalse("Result should be invalid when first name is whitespace");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "FirstName");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_LastName_Is_Empty()
    {
        var (result, name) = PersonName.TryCreate("John", "");

        result.IsValid.ShouldBeFalse("Result should be invalid when last name is empty");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "LastName" && e.Code == "Required");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_FirstName_Exceeds_MaxLength()
    {
        var longFirstName = "Johnathon" + new string('a', 42);
        var (result, name) = PersonName.TryCreate(longFirstName, "Doe");

        result.IsValid.ShouldBeFalse("Result should be invalid when first name exceeds 50 characters");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "FirstName" && e.Code == "Length");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_LastName_Exceeds_MaxLength()
    {
        var longLastName = "Doolittle" + new string('e', 42);
        var (result, name) = PersonName.TryCreate("John", longLastName);

        result.IsValid.ShouldBeFalse("Result should be invalid when last name exceeds 50 characters");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "LastName" && e.Code == "Length");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_MiddleName_Exceeds_MaxLength()
    {
        var longMiddleName = "Christopher" + new string('r', 40);
        var (result, name) = PersonName.TryCreate("John", "Doe", longMiddleName);

        result.IsValid.ShouldBeFalse("Result should be invalid when middle name exceeds 50 characters");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "MiddleName" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Trims_Whitespace_From_Names()
    {
        var (result, name) = PersonName.TryCreate("  John  ", "  Doe  ", "  Michael  ");

        result.IsValid.ShouldBeTrue();
        name.ShouldNotBeNull();
        name!.FirstName.ShouldBe("John");
        name.LastName.ShouldBe("Doe");
        name.MiddleName.ShouldBe("Michael");
    }

    [Fact]
    public void TryCreate_Normalizes_Empty_MiddleName_To_Null()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe", "");

        result.IsValid.ShouldBeTrue();
        name.ShouldNotBeNull();
        name!.MiddleName.ShouldBeNull("Empty middle name should be normalized to null");
    }

    [Fact]
    public void TryCreate_Normalizes_Whitespace_MiddleName_To_Null()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe", "   ");

        result.IsValid.ShouldBeTrue();
        name.ShouldNotBeNull();
        name!.MiddleName.ShouldBeNull("Whitespace middle name should be normalized to null");
    }

    [Fact]
    public void TryCreate_Returns_Multiple_Errors_When_Multiple_Fields_Invalid()
    {
        var (result, name) = PersonName.TryCreate("", "");

        result.IsValid.ShouldBeFalse();
        name.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThan(1, "Should have multiple validation errors");
        result.Errors.ShouldContain(e => e.MemberName == "FirstName");
        result.Errors.ShouldContain(e => e.MemberName == "LastName");
    }

    [Fact]
    public void FullName_Returns_FirstName_And_LastName_Without_MiddleName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe");

        name.ShouldNotBeNull();
        name!.FullName.ShouldBe("John Doe");
    }

    [Fact]
    public void FullName_Returns_FirstName_MiddleName_And_LastName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe", "Michael");

        name.ShouldNotBeNull();
        name!.FullName.ShouldBe("John Michael Doe");
    }

    [Fact]
    public void FormalName_Returns_LastName_FirstName_Without_MiddleName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe");

        name.ShouldNotBeNull();
        name!.FormalName.ShouldBe("Doe, John");
    }

    [Fact]
    public void FormalName_Returns_LastName_FirstName_MiddleName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe", "Michael");

        name.ShouldNotBeNull();
        name!.FormalName.ShouldBe("Doe, John Michael");
    }

    [Fact]
    public void Initials_Returns_FirstName_And_LastName_Initials_Without_MiddleName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe");

        name.ShouldNotBeNull();
        name!.Initials.ShouldBe("J.D.");
    }

    [Fact]
    public void Initials_Returns_FirstName_MiddleName_And_LastName_Initials()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe", "Michael");

        name.ShouldNotBeNull();
        name!.Initials.ShouldBe("J.M.D.");
    }

    [Fact]
    public void ToString_Returns_FullName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe", "Michael");

        name.ShouldNotBeNull();
        name!.ToString().ShouldBe("John Michael Doe");
    }

    [Fact]
    public void Equality_Same_Values_Are_Equal()
    {
        var (_, name1) = PersonName.TryCreate("John", "Doe", "Michael");
        var (_, name2) = PersonName.TryCreate("John", "Doe", "Michael");

        name1.ShouldBe(name2, "PersonNames with same values should be equal");
    }

    [Fact]
    public void Equality_Different_FirstName_Are_Not_Equal()
    {
        var (_, name1) = PersonName.TryCreate("John", "Doe");
        var (_, name2) = PersonName.TryCreate("Jane", "Doe");

        name1.ShouldNotBe(name2, "PersonNames with different first names should not be equal");
    }

    [Fact]
    public void Equality_Different_MiddleName_Are_Not_Equal()
    {
        var (_, name1) = PersonName.TryCreate("John", "Doe", "Michael");
        var (_, name2) = PersonName.TryCreate("John", "Doe", "Robert");

        name1.ShouldNotBe(name2, "PersonNames with different middle names should not be equal");
    }

    [Fact]
    public void Equality_With_And_Without_MiddleName_Are_Not_Equal()
    {
        var (_, name1) = PersonName.TryCreate("John", "Doe");
        var (_, name2) = PersonName.TryCreate("John", "Doe", "Michael");

        name1.ShouldNotBe(name2, "PersonName with and without middle name should not be equal");
    }

    [Fact]
    public void Works_With_Single_Character_Names()
    {
        var (result, name) = PersonName.TryCreate("J", "D");

        result.IsValid.ShouldBeTrue("Single character names should be valid");
        name.ShouldNotBeNull();
        name!.FirstName.ShouldBe("J");
        name.LastName.ShouldBe("D");
    }

    [Fact]
    public void Works_With_Hyphenated_Names()
    {
        var (result, name) = PersonName.TryCreate("Mary-Jane", "Smith-Jones");

        result.IsValid.ShouldBeTrue("Hyphenated names should be valid");
        name.ShouldNotBeNull();
        name!.FirstName.ShouldBe("Mary-Jane");
        name.LastName.ShouldBe("Smith-Jones");
    }

    [Fact]
    public void Works_With_Apostrophe_Names()
    {
        var (result, name) = PersonName.TryCreate("O'Brien", "D'Angelo");

        result.IsValid.ShouldBeTrue("Names with apostrophes should be valid");
        name.ShouldNotBeNull();
        name!.FirstName.ShouldBe("O'Brien");
        name.LastName.ShouldBe("D'Angelo");
    }

    [Fact]
    public void Works_With_International_Characters()
    {
        var names = new[]
        {
            ("José", "García"),
            ("François", "Müller"),
            ("Søren", "Ødegård"),
            ("W?adys?aw", "?ukowski")
        };

        foreach (var (firstName, lastName) in names)
        {
            var (result, name) = PersonName.TryCreate(firstName, lastName);
            
            result.IsValid.ShouldBeTrue($"Should be valid for {firstName} {lastName}");
            name.ShouldNotBeNull();
            name!.FirstName.ShouldBe(firstName);
            name!.LastName.ShouldBe(lastName);
        }
    }
}
