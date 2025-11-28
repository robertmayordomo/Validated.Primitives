using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class StateProvinceTests
{
    [Fact]
    public void TryCreate_WithValidStateProvince_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("California");

        result.IsValid.ShouldBeTrue("Result should be valid for a valid state/province name");
        stateProvince.ShouldNotBeNull("StateProvince should not be null when validation succeeds");
        stateProvince!.Value.ShouldBe("California");
    }

    [Fact]
    public void TryCreate_WithNullValue_ShouldFail()
    {
        var (result, stateProvince) = StateProvince.TryCreate(null!);

        result.IsValid.ShouldBeFalse("Result should be invalid for null value");
        stateProvince.ShouldBeNull("StateProvince should be null when validation fails");
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors[0].Code.ShouldBe("NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_WithEmptyString_ShouldFail()
    {
        var (result, stateProvince) = StateProvince.TryCreate("");

        result.IsValid.ShouldBeFalse("Result should be invalid for empty string");
        stateProvince.ShouldBeNull("StateProvince should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_WithWhitespace_ShouldFail()
    {
        var (result, stateProvince) = StateProvince.TryCreate("   ");

        result.IsValid.ShouldBeFalse("Result should be invalid for whitespace");
        stateProvince.ShouldBeNull("StateProvince should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "NotNullOrWhitespace");
    }

    [Fact]
    public void TryCreate_WithMaxLength_ShouldSucceed()
    {
        var value = new string('a', 100);
        var (result, stateProvince) = StateProvince.TryCreate(value);

        result.IsValid.ShouldBeTrue("Result should be valid for 100 character state/province name");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void TryCreate_ExceedingMaxLength_ShouldFail()
    {
        var value = new string('a', 101);
        var (result, stateProvince) = StateProvince.TryCreate(value);

        result.IsValid.ShouldBeFalse("Result should be invalid when state/province name exceeds 100 characters");
        stateProvince.ShouldBeNull("StateProvince should be null when validation fails");
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_ShouldUseInErrorMessage()
    {
        var value = new string('a', 101);
        var (result, stateProvince) = StateProvince.TryCreate(value, "State");

        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("State");
    }

    [Theory]
    [InlineData("California")]
    [InlineData("Texas")]
    [InlineData("New York")]
    [InlineData("Florida")]
    [InlineData("Illinois")]
    [InlineData("Pennsylvania")]
    [InlineData("Ohio")]
    [InlineData("North Carolina")]
    public void TryCreate_WithVariousUSStates_ShouldSucceed(string stateName)
    {
        var (result, stateProvince) = StateProvince.TryCreate(stateName);

        result.IsValid.ShouldBeTrue($"Result should be valid for: {stateName}");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe(stateName);
    }

    [Theory]
    [InlineData("Ontario")]
    [InlineData("Quebec")]
    [InlineData("British Columbia")]
    [InlineData("Alberta")]
    [InlineData("Manitoba")]
    [InlineData("Saskatchewan")]
    [InlineData("Nova Scotia")]
    [InlineData("New Brunswick")]
    public void TryCreate_WithCanadianProvinces_ShouldSucceed(string provinceName)
    {
        var (result, stateProvince) = StateProvince.TryCreate(provinceName);

        result.IsValid.ShouldBeTrue($"Result should be valid for: {provinceName}");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe(provinceName);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var (_, stateProvince) = StateProvince.TryCreate("California");

        stateProvince.ShouldNotBeNull();
        stateProvince!.ToString().ShouldBe("California");
    }

    [Fact]
    public void TryCreate_WithUnicodeCharacters_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("Québec");

        result.IsValid.ShouldBeTrue("Result should be valid for state/province with Unicode characters");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("Québec");
    }

    [Fact]
    public void TryCreate_WithAccentedCharacters_ShouldSucceed()
    {
        var provinces = new[]
        {
            "Québec",
            "Nuevo León",
            "São Paulo",
            "Bayern",
            "Île-de-France"
        };

        foreach (var provinceName in provinces)
        {
            var (result, stateProvince) = StateProvince.TryCreate(provinceName);
            result.IsValid.ShouldBeTrue($"Should be valid for {provinceName}");
            stateProvince.ShouldNotBeNull();
            stateProvince!.Value.ShouldBe(provinceName);
        }
    }

    [Fact]
    public void TryCreate_WithHyphens_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("Baden-Württemberg");

        result.IsValid.ShouldBeTrue("Result should be valid for state/province with hyphens");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("Baden-Württemberg");
    }

    [Fact]
    public void TryCreate_WithApostrophes_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("Hawai'i");

        result.IsValid.ShouldBeTrue("Result should be valid for state/province with apostrophes");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("Hawai'i");
    }

    [Fact]
    public void TryCreate_WithSpaces_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("New South Wales");

        result.IsValid.ShouldBeTrue("Result should be valid for state/province with spaces");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("New South Wales");
    }

    [Fact]
    public void TryCreate_WithPeriods_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("N.W.T.");

        result.IsValid.ShouldBeTrue("Result should be valid for state/province with periods");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("N.W.T.");
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var (_, stateProvince1) = StateProvince.TryCreate("California");
        var (_, stateProvince2) = StateProvince.TryCreate("California");

        stateProvince1.ShouldBe(stateProvince2);
        (stateProvince1 == stateProvince2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var (_, stateProvince1) = StateProvince.TryCreate("California");
        var (_, stateProvince2) = StateProvince.TryCreate("Texas");

        stateProvince1.ShouldNotBe(stateProvince2);
        (stateProvince1 != stateProvince2).ShouldBeTrue();
    }

    [Fact]
    public void Equality_CaseSensitive()
    {
        var (_, stateProvince1) = StateProvince.TryCreate("California");
        var (_, stateProvince2) = StateProvince.TryCreate("california");

        // String comparison is case-sensitive by default
        stateProvince1.ShouldNotBe(stateProvince2);
    }

    [Fact]
    public void StateProvince_ShouldHaveRecordSemantics()
    {
        var (_, stateProvince1) = StateProvince.TryCreate("California");
        var (_, stateProvince2) = StateProvince.TryCreate("California");

        // Records should be equal based on value
        stateProvince1.ShouldNotBeNull();
        stateProvince2.ShouldNotBeNull();
        stateProvince1.ShouldBe(stateProvince2);
        stateProvince1!.GetHashCode().ShouldBe(stateProvince2!.GetHashCode());
    }

    [Fact]
    public void TryCreate_WithExactly100Characters_ShouldSucceed()
    {
        var exactValue = new string('X', 100);
        var (result, stateProvince) = StateProvince.TryCreate(exactValue);

        result.IsValid.ShouldBeTrue("Exactly 100 characters should be valid");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void TryCreate_With101Characters_ShouldFail()
    {
        var tooLong = new string('X', 101);
        var (result, stateProvince) = StateProvince.TryCreate(tooLong);

        result.IsValid.ShouldBeFalse("101 characters should exceed maximum");
        stateProvince.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Fact]
    public void TryCreate_WithSingleCharacter_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("X");

        result.IsValid.ShouldBeTrue("Single character should be valid");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("X");
    }

    [Fact]
    public void TryCreate_WithNumbers_ShouldSucceed()
    {
        var (result, stateProvince) = StateProvince.TryCreate("Region 5");

        result.IsValid.ShouldBeTrue("State/province name with numbers should be valid");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("Region 5");
    }

    [Fact]
    public void TryCreate_WithMixedCase_PreservesCase()
    {
        var (result, stateProvince) = StateProvince.TryCreate("NeW YoRk");

        result.IsValid.ShouldBeTrue();
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe("NeW YoRk"); // Preserves exact case
    }

    [Fact]
    public void TryCreate_WithLeadingAndTrailingSpaces_TrimsSpaces()
    {
        var (result, stateProvince) = StateProvince.TryCreate("  California  ");

        result.IsValid.ShouldBeTrue();
        stateProvince.ShouldNotBeNull();
        // Trims leading and trailing spaces
        stateProvince!.Value.ShouldBe("California");
    }

    [Fact]
    public void TryCreate_ReturnsMultipleErrors_WhenBothValidationsFail()
    {
        var tooLong = new string('a', 101);
        var (result, stateProvince) = StateProvince.TryCreate(tooLong);

        result.IsValid.ShouldBeFalse();
        stateProvince.ShouldBeNull();
        // Should have MaxLength error
        result.Errors.ShouldContain(e => e.Code == "MaxLength");
    }

    [Theory]
    [InlineData("Victoria")]
    [InlineData("New South Wales")]
    [InlineData("Queensland")]
    [InlineData("South Australia")]
    [InlineData("Western Australia")]
    [InlineData("Tasmania")]
    [InlineData("Northern Territory")]
    [InlineData("Australian Capital Territory")]
    public void TryCreate_WithAustralianStates_ShouldSucceed(string stateName)
    {
        var (result, stateProvince) = StateProvince.TryCreate(stateName);

        result.IsValid.ShouldBeTrue($"Should be valid for {stateName}");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe(stateName);
    }

    [Theory]
    [InlineData("England")]
    [InlineData("Scotland")]
    [InlineData("Wales")]
    [InlineData("Northern Ireland")]
    public void TryCreate_WithUKRegions_ShouldSucceed(string regionName)
    {
        var (result, stateProvince) = StateProvince.TryCreate(regionName);

        result.IsValid.ShouldBeTrue($"Should be valid for {regionName}");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe(regionName);
    }

    [Theory]
    [InlineData("Bavaria")]
    [InlineData("North Rhine-Westphalia")]
    [InlineData("Baden-Württemberg")]
    [InlineData("Lower Saxony")]
    [InlineData("Hesse")]
    [InlineData("Saxony")]
    [InlineData("Rhineland-Palatinate")]
    [InlineData("Schleswig-Holstein")]
    public void TryCreate_WithGermanStates_ShouldSucceed(string stateName)
    {
        var (result, stateProvince) = StateProvince.TryCreate(stateName);

        result.IsValid.ShouldBeTrue($"Should be valid for {stateName}");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe(stateName);
    }

    [Theory]
    [InlineData("Jalisco")]
    [InlineData("Nuevo León")]
    [InlineData("Veracruz")]
    [InlineData("Chihuahua")]
    [InlineData("Yucatán")]
    [InlineData("Guanajuato")]
    public void TryCreate_WithMexicanStates_ShouldSucceed(string stateName)
    {
        var (result, stateProvince) = StateProvince.TryCreate(stateName);

        result.IsValid.ShouldBeTrue($"Should be valid for {stateName}");
        stateProvince.ShouldNotBeNull();
        stateProvince!.Value.ShouldBe(stateName);
    }
}
