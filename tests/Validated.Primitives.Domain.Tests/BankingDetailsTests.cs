using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Tests;

public class BankingDetailsTests
{
    #region US Banking Tests

    [Fact]
    public void TryCreate_WithValidUsBanking_ReturnsSuccess()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            "123456789",
            "CHASUS33",
            "021000021");

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedStates);
        banking.AccountNumber.Value.ShouldBe("123456789");
        banking.RoutingNumber!.ToDigitsOnly().ShouldBe("021000021");
        banking.SwiftCode!.Value.ShouldBe("CHASUS33");
    }

    [Fact]
    public void TryCreate_WithUsBankingWithoutRoutingNumber_ReturnsFailure()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            "123456789");

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "RoutingNumber");
    }

    [Fact]
    public void TryCreate_WithUsBankingAndSortCode_ReturnsFailure()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            "123456789",
            routingNumber: "021000021",
            sortCode: "123456");

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "NotApplicable" && e.MemberName == "SortCode");
    }

    #endregion

    #region UK Banking Tests

    [Fact]
    public void TryCreate_WithValidUkBanking_ReturnsSuccess()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedKingdom,
            "12345678",
            "BARCGB22",
            sortCode: "123456");

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedKingdom);
        banking.AccountNumber.Value.ShouldBe("12345678");
        banking.SortCode!.ToDigitsOnly().ShouldBe("123456");
        banking.SwiftCode!.Value.ShouldBe("BARCGB22");
    }

    [Fact]
    public void TryCreate_WithUkBankingWithoutSortCode_ReturnsFailure()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedKingdom,
            "12345678");

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "SortCode");
    }

    [Fact]
    public void TryCreate_WithUkBankingAndRoutingNumber_ReturnsFailure()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedKingdom,
            "12345678",
            sortCode: "123456",
            routingNumber: "021000021");

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "NotApplicable" && e.MemberName == "RoutingNumber");
    }

    #endregion

    #region International IBAN Banking Tests

    [Fact]
    public void TryCreate_WithValidIbanBanking_ReturnsSuccess()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.Germany,
            "DE89370400440532013000",
            "DEUTDEFF");

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.Germany);
        banking.AccountNumber.ToNormalizedString().ShouldBe("DE89370400440532013000");
        banking.SwiftCode!.Value.ShouldBe("DEUTDEFF");
        banking.UsesIban.ShouldBeTrue();
        banking.SupportsInternationalTransfers.ShouldBeTrue();
    }

    [Fact]
    public void TryCreate_WithIbanWithoutSwiftCode_ReturnsSuccess()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.France,
            "FR1420041010050500013M02606");

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.SupportsInternationalTransfers.ShouldBeFalse();
    }

    #endregion

    #region Property Tests

    [Fact]
    public void SupportsInternationalTransfers_WithSwiftCode_ReturnsTrue()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.Germany, "DE89370400440532013000", "DEUTDEFF");

        // Assert
        banking!.SupportsInternationalTransfers.ShouldBeTrue();
    }

    [Fact]
    public void SupportsInternationalTransfers_WithoutSwiftCode_ReturnsFalse()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedKingdom, "12345678", sortCode: "123456");

        // Assert
        banking!.SupportsInternationalTransfers.ShouldBeFalse();
    }

    [Fact]
    public void UsesIban_WithIbanAccount_ReturnsTrue()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        // Assert
        banking!.UsesIban.ShouldBeTrue();
    }

    [Fact]
    public void UsesIban_WithBbanAccount_ReturnsFalse()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedStates, "123456789", routingNumber: "021000021");

        // Assert
        banking!.UsesIban.ShouldBeFalse();
    }

    [Fact]
    public void MaskedAccountNumber_ReturnsMaskedValue()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.Germany, "DE89370400440532013000");

        // Assert
        banking!.MaskedAccountNumber.ShouldBe("******************3000");
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithAllComponents_FormatsCorrectly()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedStates, "123456789", "CHASUS33", "021000021");

        // Assert
        var result = banking!.ToString();
        result.ShouldContain("SWIFT: CHASUS33");
        result.ShouldContain("Routing: 0210-0002-1");
        result.ShouldContain("Account:");
        result.ShouldContain("(UnitedStates)");
    }

    [Fact]
    public void ToString_WithUkBanking_FormatsCorrectly()
    {
        // Act
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedKingdom, "12345678", sortCode: "123456");

        // Assert
        var result = banking!.ToString();
        result.ShouldContain("Sort Code: 12-34-56");
        result.ShouldContain("Account:");
        result.ShouldContain("(UnitedKingdom)");
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void TryCreate_WithNullAccountNumber_ReturnsFailure()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.Germany,
            null!);

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required");
    }

    [Fact]
    public void TryCreate_WithIrelandBanking_RequiresSortCode()
    {
        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.Ireland,
            "12345678");

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "SortCode");
    }

    #endregion
}
