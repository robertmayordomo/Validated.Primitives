using Shouldly;

namespace Validated.Primitives.Domain.Tests;

public class PersonNameTests
{
    [Fact]
    public void TryCreate_Returns_Success_With_FirstName_And_LastName()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe");

        result.IsValid.ShouldBeTrue("Result should be valid with first and last name");
        name.ShouldNotBeNull("PersonName should not be null when validation succeeds");
        name!.FirstName.Value.ShouldBe("John");
        name.LastName.Value.ShouldBe("Doe");
        name.MiddleName.ShouldBeNull("MiddleName should be null when not provided");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_MiddleName()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe", "Michael");

        result.IsValid.ShouldBeTrue("Result should be valid with middle name");
        name.ShouldNotBeNull();
        name!.FirstName.Value.ShouldBe("John");
        name.MiddleName!.Value.ShouldBe("Michael");
        name.LastName.Value.ShouldBe("Doe");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_FirstName_Is_Empty()
    {
        var (result, name) = PersonName.TryCreate("", "Doe");

        result.IsValid.ShouldBeFalse("Result should be invalid when first name is empty");
        name.ShouldBeNull("PersonName should be null when validation fails");
        result.Errors.ShouldContain(e => e.MemberName == "FirstName" && e.Code == "NotNullOrWhitespace");
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
        result.Errors.ShouldContain(e => e.MemberName == "LastName" && e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_FirstName_Exceeds_MaxLength()
    {
        var longFirstName = new string('a', 51);
        var (result, name) = PersonName.TryCreate(longFirstName, "Doe");

        result.IsValid.ShouldBeFalse("Result should be invalid when first name exceeds 50 characters");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "FirstName" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_LastName_Exceeds_MaxLength()
    {
        var longLastName = new string('e', 51);
        var (result, name) = PersonName.TryCreate("John", longLastName);

        result.IsValid.ShouldBeFalse("Result should be invalid when last name exceeds 50 characters");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "LastName" && e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_MiddleName_Exceeds_MaxLength()
    {
        var longMiddleName = new string('r', 51);
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
        name!.FirstName.Value.ShouldBe("John");
        name.LastName.Value.ShouldBe("Doe");
        name.MiddleName!.Value.ShouldBe("Michael");
    }

    [Fact]
    public void TryCreate_Normalizes_Empty_MiddleName_To_Null()
    {
        var (result, name) = PersonName.TryCreate("John", "Doe", "   ");

        result.IsValid.ShouldBeTrue();
        name.ShouldNotBeNull();
        name!.MiddleName.ShouldBeNull();
    }

    [Fact]
    public void FullName_Returns_FirstName_And_LastName_Without_MiddleName()
    {
        var (_, name) = PersonName.TryCreate("John", "Doe");

        name.ShouldNotBeNull();
        name!.FullName.ShouldBe("John Doe");
    }

    [Fact]
    public void FullName_Returns_FirstName_MiddleName_LastName()
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
        name!.FirstName.Value.ShouldBe("J");
        name.LastName.Value.ShouldBe("D");
    }

    [Fact]
    public void Works_With_Hyphenated_Names()
    {
        var (result, name) = PersonName.TryCreate("Mary-Jane", "Smith-Jones");

        result.IsValid.ShouldBeTrue("Hyphenated names should be valid");
        name.ShouldNotBeNull();
        name!.FirstName.Value.ShouldBe("Mary-Jane");
        name.LastName.Value.ShouldBe("Smith-Jones");
    }

    [Fact]
    public void Works_With_Apostrophe_Names()
    {
        var (result, name) = PersonName.TryCreate("O'Brien", "D'Angelo");

        result.IsValid.ShouldBeTrue("Names with apostrophes should be valid");
        name.ShouldNotBeNull();
        name!.FirstName.Value.ShouldBe("O'Brien");
        name.LastName.Value.ShouldBe("D'Angelo");
    }

    [Fact]
    public void Rejects_Names_With_Numbers()
    {
        var (result, name) = PersonName.TryCreate("John123", "Doe");

        result.IsValid.ShouldBeFalse("Names with numbers should be invalid");
        name.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "AlphaWithHyphenAndApostrophe");
    }

    [Fact]
    public void Rejects_Names_With_Spaces()
    {
        var (result, name) = PersonName.TryCreate("John Paul", "Doe");

        result.IsValid.ShouldBeFalse("Names with spaces should be invalid");
        name.ShouldBeNull();
    }

    [Fact]
    public void Rejects_Names_With_Special_Characters()
    {
        var (result, name) = PersonName.TryCreate("John@Doe", "Smith");

        result.IsValid.ShouldBeFalse("Names with special characters should be invalid");
        name.ShouldBeNull();
    }
}

