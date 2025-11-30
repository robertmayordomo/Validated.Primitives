using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Tests;

public class ContactInformationTests
{
    [Fact]
    public void TryCreate_Returns_Success_With_Valid_Email_And_Phone()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "+1-555-123-4567");

        result.IsValid.ShouldBeTrue("Result should be valid with valid email and phone");
        contact.ShouldNotBeNull("ContactInformation should not be null when validation succeeds");
        contact!.Email.Value.ShouldBe("john.doe@example.com");
        contact.PrimaryPhone.Value.ShouldBe("+1-555-123-4567");
        contact.SecondaryPhone.ShouldBeNull("SecondaryPhone should be null when not provided");
        contact.Website.ShouldBeNull("Website should be null when not provided");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_All_Fields()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "jane.smith@example.com",
            "+1-555-111-2222",
            "+1-555-333-4444",
            "https://janesmith.com");

        result.IsValid.ShouldBeTrue("Result should be valid with all fields provided");
        contact.ShouldNotBeNull();
        contact!.Email.Value.ShouldBe("jane.smith@example.com");
        contact.PrimaryPhone.Value.ShouldBe("+1-555-111-2222");
        contact.SecondaryPhone.ShouldNotBeNull();
        contact.SecondaryPhone!.Value.ShouldBe("+1-555-333-4444");
        contact.Website.ShouldNotBeNull();
        contact.Website!.Value.ShouldBe("https://janesmith.com");
    }

    [Fact]
    public void TryCreate_Returns_Success_With_Optional_Website_Only()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "test@example.com",
            "+1-555-999-8888",
            website: "https://example.com");

        result.IsValid.ShouldBeTrue();
        contact.ShouldNotBeNull();
        contact!.SecondaryPhone.ShouldBeNull();
        contact.Website.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_Email_Is_Invalid()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "invalid-email",
            "+1-555-123-4567");

        result.IsValid.ShouldBeFalse("Result should be invalid when email is invalid");
        contact.ShouldBeNull("ContactInformation should be null when validation fails");
        result.Errors.ShouldContain(e => e.MemberName == "Email");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_PrimaryPhone_Is_Invalid()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "invalid-phone");

        result.IsValid.ShouldBeFalse("Result should be invalid when primary phone is invalid");
        contact.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "PrimaryPhone");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_SecondaryPhone_Is_Invalid()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "+1-555-123-4567",
            "invalid-phone");

        result.IsValid.ShouldBeFalse("Result should be invalid when secondary phone is invalid");
        contact.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "SecondaryPhone");
    }

    [Fact]
    public void TryCreate_Returns_Failure_When_Website_Is_Invalid()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "+1-555-123-4567",
            website: "not-a-url");

        result.IsValid.ShouldBeFalse("Result should be invalid when website is invalid");
        contact.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "Website");
    }

    [Fact]
    public void TryCreate_Ignores_Empty_SecondaryPhone()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "+1-555-123-4567",
            "");

        result.IsValid.ShouldBeTrue("Empty secondary phone should be treated as null");
        contact.ShouldNotBeNull();
        contact!.SecondaryPhone.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_Ignores_Whitespace_SecondaryPhone()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "+1-555-123-4567",
            "   ");

        result.IsValid.ShouldBeTrue("Whitespace secondary phone should be treated as null");
        contact.ShouldNotBeNull();
        contact!.SecondaryPhone.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_Ignores_Empty_Website()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john.doe@example.com",
            "+1-555-123-4567",
            website: "");

        result.IsValid.ShouldBeTrue("Empty website should be treated as null");
        contact.ShouldNotBeNull();
        contact!.Website.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_Returns_Multiple_Errors_When_Multiple_Fields_Invalid()
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "invalid-email",
            "invalid-phone",
            "invalid-secondary-phone",
            "invalid-website");

        result.IsValid.ShouldBeFalse();
        contact.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThan(1, "Should have multiple validation errors");
        result.Errors.ShouldContain(e => e.MemberName == "Email");
        result.Errors.ShouldContain(e => e.MemberName == "PrimaryPhone");
        result.Errors.ShouldContain(e => e.MemberName == "SecondaryPhone");
        result.Errors.ShouldContain(e => e.MemberName == "Website");
    }

    [Fact]
    public void ToString_Formats_ContactInfo_With_Email_And_Phone_Only()
    {
        var (_, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "john@example.com",
            "+1-555-123-4567");

        contact.ShouldNotBeNull();
        contact!.ToString().ShouldBe("Email: john@example.com | Phone: +1-555-123-4567");
    }

    [Fact]
    public void ToString_Formats_ContactInfo_With_All_Fields()
    {
        var (_, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "jane@example.com",
            "+1-555-111-2222",
            "+1-555-333-4444",
            "https://example.com");

        contact.ShouldNotBeNull();
        contact!.ToString().ShouldBe("Email: jane@example.com | Phone: +1-555-111-2222 | Secondary Phone: +1-555-333-4444 | Website: https://example.com");
    }

    [Fact]
    public void ToString_Formats_ContactInfo_With_SecondaryPhone_Only()
    {
        var (_, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "test@example.com",
            "+1-555-111-2222",
            "+1-555-333-4444");

        contact.ShouldNotBeNull();
        contact!.ToString().ShouldBe("Email: test@example.com | Phone: +1-555-111-2222 | Secondary Phone: +1-555-333-4444");
    }

    [Fact]
    public void ToString_Formats_ContactInfo_With_Website_Only()
    {
        var (_, contact) = ContactInformation.TryCreate(
            CountryCode.UnitedStates,
            "test@example.com",
            "+1-555-111-2222",
            website: "https://example.com");

        contact.ShouldNotBeNull();
        contact!.ToString().ShouldBe("Email: test@example.com | Phone: +1-555-111-2222 | Website: https://example.com");
    }

    [Fact]
    public void Works_With_Various_Email_Formats()
    {
        var emails = new[]
        {
            "simple@example.com",
            "name.surname@example.co.uk",
            "user+tag@example.org",
            "test_user@sub.domain.example.com"
        };

        foreach (var email in emails)
        {
            var (result, contact) = ContactInformation.TryCreate(
                CountryCode.UnitedStates,
                email,
                "+1-555-123-4567");

            result.IsValid.ShouldBeTrue($"Should be valid for email: {email}");
            contact.ShouldNotBeNull();
            contact!.Email.Value.ShouldBe(email);
        }
    }

    [Theory]
    [InlineData("+1-555-123-4567")]
    [InlineData("(555) 123-4567")]
    [InlineData("555-123-4567")]
    [InlineData("+44 20 7123 4567")]
    public void Works_With_Various_Phone_Formats(string phone)
    {
        var (result, contact) = ContactInformation.TryCreate(
            CountryCode.All,
            "test@example.com",
            phone);

        result.IsValid.ShouldBeTrue($"Should be valid for phone: {phone}: {result.ToSingleMessage()}");
        contact.ShouldNotBeNull();
    }
}
