using Shouldly;
using Validated.Primitives.Domain;
using Validated.Primitives.ValueObjects;
using Xunit;

namespace Validated.Primitives.Domain.Tests;

public class BankingDetailsTests
{
    #region US Banking Tests

    [Fact]
    public void TryCreate_WithValidUsBanking_ReturnsSuccess()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);
        var (_, routing) = RoutingNumber.TryCreate("021000021");
        var (_, swift) = SwiftCode.TryCreate("CHASUS33");

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            account!,
            swift,
            routing);

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedStates);
        banking.AccountNumber.ShouldBe(account);
        banking.RoutingNumber.ShouldBe(routing);
        banking.SwiftCode.ShouldBe(swift);
    }

    [Fact]
    public void TryCreate_WithUsBankingWithoutRoutingNumber_ReturnsFailure()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            account!);

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "RoutingNumber");
    }

    [Fact]
    public void TryCreate_WithUsBankingAndSortCode_ReturnsFailure()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);
        var (_, routing) = RoutingNumber.TryCreate("021000021");
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedStates,
            account!,
            sortCode: sortCode,
            routingNumber: routing);

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
        // Arrange
        var (_, account) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, swift) = SwiftCode.TryCreate("BARCGB22");

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedKingdom,
            account!,
            swift,
            sortCode: sortCode);

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedKingdom);
        banking.AccountNumber.ShouldBe(account);
        banking.SortCode.ShouldBe(sortCode);
        banking.SwiftCode.ShouldBe(swift);
    }

    [Fact]
    public void TryCreate_WithUkBankingWithoutSortCode_ReturnsFailure()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedKingdom,
            account!);

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "SortCode");
    }

    [Fact]
    public void TryCreate_WithUkBankingAndRoutingNumber_ReturnsFailure()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, routing) = RoutingNumber.TryCreate("021000021");

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.UnitedKingdom,
            account!,
            sortCode: sortCode,
            routingNumber: routing);

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
        // Arrange
        var (_, account) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, swift) = SwiftCode.TryCreate("DEUTDEFF");

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.Germany,
            account!,
            swift);

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.Germany);
        banking.AccountNumber.ShouldBe(account);
        banking.SwiftCode.ShouldBe(swift);
        banking.UsesIban.ShouldBeTrue();
        banking.SupportsInternationalTransfers.ShouldBeTrue();
    }

    [Fact]
    public void TryCreate_WithIbanWithoutSwiftCode_ReturnsSuccess()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("FR1420041010050500013M02606");

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.France,
            account!);

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
        // Arrange
        var (_, account) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, swift) = SwiftCode.TryCreate("DEUTDEFF");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.Germany, account!, swift);

        // Assert
        banking!.SupportsInternationalTransfers.ShouldBeTrue();
    }

    [Fact]
    public void SupportsInternationalTransfers_WithoutSwiftCode_ReturnsFalse()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedKingdom, account!, sortCode: sortCode);

        // Assert
        banking!.SupportsInternationalTransfers.ShouldBeFalse();
    }

    [Fact]
    public void UsesIban_WithIbanAccount_ReturnsTrue()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.Germany, account!);

        // Assert
        banking!.UsesIban.ShouldBeTrue();
    }

    [Fact]
    public void UsesIban_WithBbanAccount_ReturnsFalse()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);
        var (_, routing) = RoutingNumber.TryCreate("021000021");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedStates, account!, routingNumber: routing);

        // Assert
        banking!.UsesIban.ShouldBeFalse();
    }

    [Fact]
    public void MaskedAccountNumber_ReturnsMaskedValue()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("DE89370400440532013000");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.Germany, account!);

        // Assert
        banking!.MaskedAccountNumber.ShouldBe("******************3000");
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithAllComponents_FormatsCorrectly()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("123456789", CountryCode.UnitedStates);
        var (_, routing) = RoutingNumber.TryCreate("021000021");
        var (_, swift) = SwiftCode.TryCreate("CHASUS33");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedStates, account!, swift, routing);

        // Act
        var result = banking!.ToString();

        // Assert
        result.ShouldContain("SWIFT: CHASUS33");
        result.ShouldContain("Routing: 0210-0002-1");
        result.ShouldContain("Account:");
        result.ShouldContain("(UnitedStates)");
    }

    [Fact]
    public void ToString_WithUkBanking_FormatsCorrectly()
    {
        // Arrange
        var (_, account) = IbanNumber.TryCreate("12345678", CountryCode.UnitedKingdom);
        var (_, sortCode) = SortCode.TryCreate(CountryCode.UnitedKingdom, "123456");
        var (_, banking) = BankingDetails.TryCreate(CountryCode.UnitedKingdom, account!, sortCode: sortCode);

        // Act
        var result = banking!.ToString();

        // Assert
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
        // Arrange
        var (_, account) = IbanNumber.TryCreate("12345678", CountryCode.Ireland);

        // Act
        var (result, banking) = BankingDetails.TryCreate(
            CountryCode.Ireland,
            account!);

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "SortCode");
    }

    #endregion
}
