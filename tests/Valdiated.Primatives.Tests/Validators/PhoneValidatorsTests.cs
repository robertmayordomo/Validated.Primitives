using Xunit;
using Shouldly;
using Validated.Primitives.Validators;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.Validators;

public class PhoneValidatorsTests
{
    public static TheoryData<string> ValidFormatPhones => new()
    {
        "+14155552671",
        "14155552671",
        "+447911123456",
        "+61412345678",
        "+862012345678",
        "+12025551234",
        "12025551234",
        "+919876543210",
        "+551123456789",
        "+1-415-555-2671",
        "+1 415 555 2671",
        "(415) 555-2671",
        "415-555-2671",
        "+44 20 7946 0958",
        "+49 30 12345678",
        "+33 1 42 86 82 00"
    };

    public static TheoryData<string> InvalidFormatPhones => new()
    {
        "abc123456789",     // Contains letters
        "+1@415-555-2671",  // Contains invalid character @
        "phone",            // Just letters
        "123abc456",        // Mixed letters and numbers
        "+1#415-555-2671",  // Contains invalid character #
        "call me",          // Contains spaces and letters
        "+1.415.555.2671"   // Contains periods (not allowed)
    };

    [Theory]
    [MemberData(nameof(ValidFormatPhones))]
    public void ValidFormat_Allows_Valid_Phone_Formats(string phone)
    {
        var result = PhoneValidators.ValidFormat("Phone")(phone);
        result.IsValid.ShouldBeTrue($"Expected valid format for phone: {phone}");
    }

    [Theory]
    [MemberData(nameof(InvalidFormatPhones))]
    public void ValidFormat_Rejects_Invalid_Phone_Formats(string phone)
    {
        var result = PhoneValidators.ValidFormat("Phone")(phone);
        result.IsValid.ShouldBeFalse($"Expected invalid format for phone: {phone}");
        result.Errors[0].Code.ShouldBe("InvalidPhoneNumberFormat");
        result.Errors[0].Message.ShouldContain("can only contain numbers, spaces, plus sign, hyphens, and parentheses");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidFormat_Allows_Null_Or_Empty(string? phone)
    {
        var result = PhoneValidators.ValidFormat("Phone")(phone!);
        result.IsValid.ShouldBeTrue("ValidFormat should allow null or empty values");
    }

    [Fact]
    public void ValidFormat_Uses_Custom_FieldName_In_Errors()
    {
        var result = PhoneValidators.ValidFormat("ContactPhone")("invalid@phone");
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("ContactPhone");
    }

    public static TheoryData<string> ValidUSPhones => new()
    {
        "+14155552671",
        "14155552671",
        "+1-415-555-2671",
        "+1 415 555 2671",
        "(415) 555-2671",
        "415-555-2671",
        "4155552671",
        "+1(415)555-2671"
    };

    public static TheoryData<string> InvalidUSPhones => new()
    {
        "123",              // Too short
        "12345",            // Too short
        "123456789012345",  // Too long
        "+44 20 7946 0958", // UK number
        "+61 412 345 678"   // Australian number
    };

    [Theory]
    [MemberData(nameof(ValidUSPhones))]
    public void ValidateCountryFormat_Accepts_Valid_US_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.UnitedStates)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid US phone: {phone}");
    }

    [Theory]
    [MemberData(nameof(InvalidUSPhones))]
    public void ValidateCountryFormat_Rejects_Invalid_US_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.UnitedStates)(phone);
        result.IsValid.ShouldBeFalse($"Expected invalid US phone: {phone}");
        result.Errors[0].Code.ShouldBe("InvalidCountryPhoneFormat");
        result.Errors[0].Message.ShouldContain("UnitedStates");
    }

    public static TheoryData<string> ValidUKPhones => new()
    {
        "+447911123456",    // 4 + 6 digits
        "07911123456",      // 0 + 4 + 6 digits
        "02079460958",      // 0 + 3 + 3 + 4 digits  
        "+442079460958"     // +44 + 3 + 3 + 4 digits
    };

    public static TheoryData<string> InvalidUKPhones => new()
    {
        "123",
        "+14155552671",     // US number
        "+447",             // Too short
        "abcd1234567890"    // Contains letters
    };

    [Theory]
    [MemberData(nameof(ValidUKPhones))]
    public void ValidateCountryFormat_Accepts_Valid_UK_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.UnitedKingdom)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid UK phone: {phone}");
    }

    [Theory]
    [MemberData(nameof(InvalidUKPhones))]
    public void ValidateCountryFormat_Rejects_Invalid_UK_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.UnitedKingdom)(phone);
        result.IsValid.ShouldBeFalse($"Expected invalid UK phone: {phone}");
    }

    public static TheoryData<string> ValidCanadaPhones => new()
    {
        "+14165551234",
        "14165551234",
        "+1-416-555-1234",
        "+1 416 555 1234",
        "(416) 555-1234",
        "416-555-1234",
        "4165551234"
    };

    [Theory]
    [MemberData(nameof(ValidCanadaPhones))]
    public void ValidateCountryFormat_Accepts_Valid_Canada_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Canada)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid Canada phone: {phone}");
    }

    public static TheoryData<string> ValidAustraliaPhones => new()
    {
        "+61412345678",
        "+61 4 1234 5678",
        "0412345678",
        "+61-4-1234-5678"
    };

    [Theory]
    [MemberData(nameof(ValidAustraliaPhones))]
    public void ValidateCountryFormat_Accepts_Valid_Australia_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Australia)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid Australia phone: {phone}");
    }

    public static TheoryData<string> ValidGermanyPhones => new()
    {
        "+4915112345678",
        "+49 151 12345678",
        "015112345678",
        "+49-30-12345678"
    };

    [Theory]
    [MemberData(nameof(ValidGermanyPhones))]
    public void ValidateCountryFormat_Accepts_Valid_Germany_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Germany)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid Germany phone: {phone}");
    }

    public static TheoryData<string> ValidFrancePhones => new()
    {
        "+33142868200",
        "+33 1 42 86 82 00",
        "0142868200",
        "+33-1-42-86-82-00"
    };

    [Theory]
    [MemberData(nameof(ValidFrancePhones))]
    public void ValidateCountryFormat_Accepts_Valid_France_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.France)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid France phone: {phone}");
    }

    public static TheoryData<string> ValidJapanPhones => new()
    {
        "+81312345678",
        "+81 3 1234 5678",
        "0312345678",
        "+81-3-1234-5678"
    };

    [Theory]
    [MemberData(nameof(ValidJapanPhones))]
    public void ValidateCountryFormat_Accepts_Valid_Japan_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Japan)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid Japan phone: {phone}");
    }

    public static TheoryData<string> ValidChinaPhones => new()
    {
        "+8613912345678",
        "+86 139 1234 5678",
        "13912345678",
        "+86-139-1234-5678"
    };

    [Theory]
    [MemberData(nameof(ValidChinaPhones))]
    public void ValidateCountryFormat_Accepts_Valid_China_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.China)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid China phone: {phone}");
    }

    public static TheoryData<string> ValidIndiaPhones => new()
    {
        "+919876543210",
        "+91 98765 43210",
        "9876543210",
        "+91-98765-43210"
    };

    [Theory]
    [MemberData(nameof(ValidIndiaPhones))]
    public void ValidateCountryFormat_Accepts_Valid_India_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.India)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid India phone: {phone}");
    }

    public static TheoryData<string> ValidBrazilPhones => new()
    {
        "+5511987654321",
        "+55 11 98765-4321",
        "(11) 98765-4321",
        "+55-11-98765-4321",
        "+551198765432"  // 8 digits (landline)
    };

    [Theory]
    [MemberData(nameof(ValidBrazilPhones))]
    public void ValidateCountryFormat_Accepts_Valid_Brazil_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Brazil)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid Brazil phone: {phone}");
    }

    public static TheoryData<string> ValidMexicoPhones => new()
    {
        "+525512345678",
        "5512345678"
    };

    [Theory]
    [MemberData(nameof(ValidMexicoPhones))]
    public void ValidateCountryFormat_Accepts_Valid_Mexico_Phones(string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Mexico)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid Mexico phone: {phone}");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateCountryFormat_Allows_Null_Or_Empty(string? phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.UnitedStates)(phone!);
        result.IsValid.ShouldBeTrue("ValidateCountryFormat should allow null or empty values");
    }

    [Fact]
    public void ValidateCountryFormat_Allows_Unknown_CountryCode()
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Unknown)("+123456789");
        result.IsValid.ShouldBeTrue("Unknown country code should allow any valid format");
    }

    [Fact]
    public void ValidateCountryFormat_Allows_All_CountryCode()
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.All)("+123456789");
        result.IsValid.ShouldBeTrue("All country code should allow any valid format");
    }

    [Fact]
    public void ValidateCountryFormat_Uses_Custom_FieldName_In_Errors()
    {
        var result = PhoneValidators.ValidateCountryFormat("MobileNumber", CountryCode.UnitedStates)("123");
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("MobileNumber");
        result.Errors[0].Message.ShouldContain("MobileNumber");
    }

    [Fact]
    public void ValidateCountryFormat_Error_Includes_CountryCode()
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.Germany)("123");
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Message.ShouldContain("Germany");
    }

    [Theory]
    [InlineData(CountryCode.Spain, "+34612345678")]
    [InlineData(CountryCode.Italy, "+393912345678")]
    [InlineData(CountryCode.Netherlands, "+31612345678")]
    [InlineData(CountryCode.Belgium, "+32471234567")]
    [InlineData(CountryCode.Switzerland, "+41791234567")]
    [InlineData(CountryCode.Austria, "+43664123456")]
    [InlineData(CountryCode.Sweden, "+46701234567")]
    [InlineData(CountryCode.Norway, "+4712345678")]
    [InlineData(CountryCode.Denmark, "+4512345678")]
    [InlineData(CountryCode.Finland, "+358401234567")]
    [InlineData(CountryCode.Poland, "+48123456789")]
    [InlineData(CountryCode.CzechRepublic, "+420123456789")]
    [InlineData(CountryCode.Hungary, "+36201234567")]
    [InlineData(CountryCode.Portugal, "+351123456789")]
    [InlineData(CountryCode.Ireland, "+353851234567")]
    [InlineData(CountryCode.SouthAfrica, "+27821234567")]
    [InlineData(CountryCode.NewZealand, "+6421234567")]
    [InlineData(CountryCode.Singapore, "+6512345678")]
    [InlineData(CountryCode.SouthKorea, "+821012345678")]
    [InlineData(CountryCode.Russia, "+79123456789")]
    public void ValidateCountryFormat_Accepts_Valid_Phones_For_Various_Countries(CountryCode countryCode, string phone)
    {
        var result = PhoneValidators.ValidateCountryFormat("Phone", countryCode)(phone);
        result.IsValid.ShouldBeTrue($"Expected valid {countryCode} phone: {phone}");
    }

    [Theory]
    [InlineData("+14155552671", CountryCode.UnitedStates)]
    [InlineData("+447911123456", CountryCode.UnitedKingdom)]
    [InlineData("+61412345678", CountryCode.Australia)]
    [InlineData("+4915112345678", CountryCode.Germany)]
    public void Combined_ValidFormat_And_ValidateCountryFormat_Both_Pass(string phone, CountryCode countryCode)
    {
        var formatResult = PhoneValidators.ValidFormat("Phone")(phone);
        var countryResult = PhoneValidators.ValidateCountryFormat("Phone", countryCode)(phone);
        
        formatResult.IsValid.ShouldBeTrue($"Format validation should pass for: {phone}");
        countryResult.IsValid.ShouldBeTrue($"Country validation should pass for: {phone}");
    }

    [Fact]
    public void Combined_ValidFormat_Passes_But_ValidateCountryFormat_Fails_For_Wrong_Country()
    {
        var phone = "+14155552671"; // US phone
        
        var formatResult = PhoneValidators.ValidFormat("Phone")(phone);
        var countryResult = PhoneValidators.ValidateCountryFormat("Phone", CountryCode.UnitedKingdom)(phone);
        
        formatResult.IsValid.ShouldBeTrue("Format validation should pass");
        countryResult.IsValid.ShouldBeFalse("Country validation should fail for wrong country");
    }
}
