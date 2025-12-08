using Shouldly;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Tests.ValueObjects;

public class BankAccountNumberTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("00123456")]
    [InlineData("99999999")]
    public void TryCreate_UkWithValidAccountNumber_ReturnsSuccess(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, accountNumber);

        result.IsValid.ShouldBeTrue($"Account number {accountNumber} should be valid");
        account.ShouldNotBeNull();
        account!.CountryCode.ShouldBe(CountryCode.UnitedKingdom);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    public void TryCreate_UkWithInvalidAccountNumber_ReturnsFailure(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, accountNumber);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidCountryAccountNumberFormat");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456789")]
    [InlineData("12345678901234567")]
    public void TryCreate_UsWithValidAccountNumber_ReturnsSuccess(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedStates, accountNumber);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
        account!.CountryCode.ShouldBe(CountryCode.UnitedStates);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012345678")]
    public void TryCreate_UsWithInvalidAccountNumber_ReturnsFailure(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedStates, accountNumber);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
    }

    [Theory]
    [InlineData("DE89370400440532013000")]
    [InlineData("DE44500105175407324931")]
    public void TryCreate_GermanyWithValidIban_ReturnsSuccess(string iban)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Germany, iban);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
        account!.CountryCode.ShouldBe(CountryCode.Germany);
        account.IsIban.ShouldBeTrue();
    }

    [Theory]
    [InlineData("DE893704004405320130")]
    [InlineData("GB82WEST12345698765432")]
    public void TryCreate_GermanyWithInvalidIban_ReturnsFailure(string iban)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Germany, iban);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
    }

    [Theory]
    [InlineData("FR1420041010050500013M02606")]
    [InlineData("FR7630006000011234567890189")]
    public void TryCreate_FranceWithValidIban_ReturnsSuccess(string iban)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.France, iban);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
        account!.IsIban.ShouldBeTrue();
    }

    [Theory]
    [InlineData("NL91ABNA0417164300")]
    [InlineData("NL39RABO0300065264")]
    public void TryCreate_NetherlandsWithValidIban_ReturnsSuccess(string iban)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Netherlands, iban);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
        account!.IsIban.ShouldBeTrue();
    }

    [Theory]
    [InlineData("IE29AIBK93115212345678")]
    [InlineData("IE64IRCE92050112345678")]
    public void TryCreate_IrelandWithValidIban_ReturnsSuccess(string iban)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Ireland, iban);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
        account!.IsIban.ShouldBeTrue();
        account!.GetIbanCountryCode().ShouldBe("IE");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12345678")]
    [InlineData("123456789")]
    public void TryCreate_AustraliaWithValidAccountNumber_ReturnsSuccess(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Australia, accountNumber);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234567890")]
    public void TryCreate_AustraliaWithInvalidAccountNumber_ReturnsFailure(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Australia, accountNumber);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789012")]
    public void TryCreate_CanadaWithValidAccountNumber_ReturnsSuccess(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Canada, accountNumber);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
    }

    [Fact]
    public void TryCreate_JapanWithValidAccountNumber_ReturnsSuccess()
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Japan, "1234567");

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("12345678")]
    public void TryCreate_JapanWithInvalidAccountNumber_ReturnsFailure(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.Japan, accountNumber);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012345678")]
    public void TryCreate_IndiaWithValidAccountNumber_ReturnsSuccess(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.India, accountNumber);

        result.IsValid.ShouldBeTrue();
        account.ShouldNotBeNull();
    }

    [Fact]
    public void ToNormalizedString_RemovesSpacesAndDashes()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89 3704 0044 0532 0130 00");

        account.ShouldNotBeNull();
        account!.ToNormalizedString().ShouldBe("DE89370400440532013000");
    }

    [Fact]
    public void ToFormattedString_IbanInGroupsOfFour()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        account.ShouldNotBeNull();
        account!.ToFormattedString().ShouldBe("DE89 3704 0044 0532 0130 00");
    }

    [Fact]
    public void ToFormattedString_NonIbanReturnsNormalized()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.ToFormattedString().ShouldBe("12345678");
    }

    [Fact]
    public void IsIban_WithIbanFormat_ReturnsTrue()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        account.ShouldNotBeNull();
        account!.IsIban.ShouldBeTrue();
    }

    [Fact]
    public void IsIban_WithNonIbanFormat_ReturnsFalse()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.IsIban.ShouldBeFalse();
    }

    [Fact]
    public void GetIbanCountryCode_ReturnsFirstTwoLetters()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        account.ShouldNotBeNull();
        account!.GetIbanCountryCode().ShouldBe("DE");
    }

    [Fact]
    public void GetIbanCountryCode_NonIban_ReturnsNull()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.GetIbanCountryCode().ShouldBeNull();
    }

    [Fact]
    public void GetIbanCheckDigits_ReturnsCheckDigits()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        account.ShouldNotBeNull();
        account!.GetIbanCheckDigits().ShouldBe("89");
    }

    [Fact]
    public void GetIbanCheckDigits_NonIban_ReturnsNull()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.GetIbanCheckDigits().ShouldBeNull();
    }

    [Fact]
    public void Masked_ShowsOnlyLastFourDigits()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.Masked().ShouldBe("****5678");
    }

    [Fact]
    public void Masked_IbanShowsLastFourDigits()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        account.ShouldNotBeNull();
        var masked = account!.Masked();
        masked.ShouldEndWith("3000");
        masked.ShouldStartWith("**");
    }

    [Fact]
    public void Masked_ShortAccountNumber_AllMasked()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedStates, "1234");

        account.ShouldNotBeNull();
        account!.Masked().ShouldBe("****");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithNullOrEmpty_ReturnsFailure(string? accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, accountNumber);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Theory]
    [InlineData("12345@78")]
    [InlineData("ABC#DEF")]
    public void TryCreate_WithInvalidCharacters_ReturnsFailure(string accountNumber)
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, accountNumber);

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "InvalidFormat");
    }

    [Fact]
    public void Equals_WithSameAccountNumber_ReturnsTrue()
    {
        var (_, account1) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");
        var (_, account2) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();
        account1!.Equals(account2).ShouldBeTrue();
        account1.ShouldBe(account2);
    }

    [Fact]
    public void Equals_WithDifferentFormatting_ReturnsTrue()
    {
        var (_, account1) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");
        var (_, account2) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89 3704 0044 0532 0130 00");

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();
        account1!.Equals(account2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAccountNumber_ReturnsFalse()
    {
        var (_, account1) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");
        var (_, account2) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "87654321");

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();
        account1!.Equals(account2).ShouldBeFalse();
        account1.ShouldNotBe(account2);
    }

    [Fact]
    public void Equals_WithDifferentCountry_ReturnsFalse()
    {
        var (_, account1) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");
        var (_, account2) = BankAccountNumber.TryCreate(CountryCode.UnitedStates, "12345678");

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();
        account1!.Equals(account2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameAccountNumber_ReturnsSameHash()
    {
        var (_, account1) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89370400440532013000");
        var (_, account2) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89 3704 0044 0532 0130 00");

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();
        account1!.GetHashCode().ShouldBe(account2!.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentAccountNumber_ReturnsDifferentHash()
    {
        var (_, account1) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");
        var (_, account2) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "87654321");

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();
        account1!.GetHashCode().ShouldNotBe(account2!.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsOriginalValue()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.Germany, "DE89 3704 0044 0532 0130 00");

        account.ShouldNotBeNull();
        account!.ToString().ShouldBe("DE89 3704 0044 0532 0130 00");
    }

    [Fact]
    public void GetCountryName_ReturnsFormattedName()
    {
        var (_, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "12345678");

        account.ShouldNotBeNull();
        account!.GetCountryName().ShouldBe("United Kingdom");
    }

    [Fact]
    public void TryCreate_WithCustomPropertyName_UsesInErrors()
    {
        var (result, account) = BankAccountNumber.TryCreate(CountryCode.UnitedKingdom, "123", "MyAccount");

        result.IsValid.ShouldBeFalse();
        account.ShouldBeNull();
        result.Errors.ShouldContain(e => e.MemberName == "MyAccount");
    }
}
