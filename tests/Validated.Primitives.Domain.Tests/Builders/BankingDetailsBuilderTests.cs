using Shouldly;
using Validated.Primitives.Domain.Builders;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Domain.Tests.Builders;

public class BankingDetailsBuilderTests
{
    #region Basic Builder Tests

    [Fact]
    public void Build_WithValidUsBanking_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedStates)
            .WithAccountNumber("123456789")
            .WithRoutingNumber("021000021")
            .WithSwiftCode("CHASUS33")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedStates);
        banking.RoutingNumber.ShouldNotBeNull();
        banking.SwiftCode.ShouldNotBeNull();
    }

    [Fact]
    public void Build_WithValidUkBanking_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedKingdom)
            .WithAccountNumber("12345678")
            .WithSortCode("123456")
            .WithSwiftCode("BARCGB22")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedKingdom);
        banking.SortCode.ShouldNotBeNull();
        banking.SwiftCode.ShouldNotBeNull();
    }

    [Fact]
    public void Build_WithValidInternationalBanking_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.Germany)
            .WithAccountNumber("DE89370400440532013000")
            .WithSwiftCode("DEUTDEFF")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.Germany);
        banking.UsesIban.ShouldBeTrue();
    }

    #endregion

    #region Convenience Method Tests

    [Fact]
    public void WithUsBanking_SetsAllUsFields()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithUsBanking("021000021", "123456789", "CHASUS33")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedStates);
        banking.RoutingNumber.ShouldNotBeNull();
        banking.SwiftCode.ShouldNotBeNull();
    }

    [Fact]
    public void WithUsBanking_WithoutSwiftCode_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithUsBanking("021000021", "123456789")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.SwiftCode.ShouldBeNull();
        banking.SupportsInternationalTransfers.ShouldBeFalse();
    }

    [Fact]
    public void WithUkBanking_SetsAllUkFields()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithUkBanking("123456", "12345678", "BARCGB22")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.UnitedKingdom);
        banking.SortCode.ShouldNotBeNull();
        banking.SwiftCode.ShouldNotBeNull();
    }

    [Fact]
    public void WithInternationalBanking_SetsIbanAndSwift()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithInternationalBanking("FR1420041010050500013M02606", "BNPAFRPP", CountryCode.France)
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.France);
        banking.UsesIban.ShouldBeTrue();
        banking.SupportsInternationalTransfers.ShouldBeTrue();
    }

    [Fact]
    public void WithInternationalBanking_WithoutCountry_AutoDetectsFromIban()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithInternationalBanking("DE89370400440532013000", "DEUTDEFF")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
        banking!.Country.ShouldBe(CountryCode.Germany);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void Build_WithoutCountry_ReturnsFailure()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithAccountNumber("123456789")
            .WithRoutingNumber("021000021")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "Country");
    }

    [Fact]
    public void Build_WithoutAccountNumber_ReturnsFailure()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedStates)
            .WithRoutingNumber("021000021")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "AccountNumber");
    }

    [Fact]
    public void Build_WithInvalidRoutingNumber_ReturnsFailure()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedStates)
            .WithAccountNumber("123456789")
            .WithRoutingNumber("INVALID")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void Build_WithInvalidSwiftCode_ReturnsFailure()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.Germany)
            .WithAccountNumber("DE89370400440532013000")
            .WithSwiftCode("INVALID")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void Build_WithUsWithoutRoutingNumber_ReturnsFailure()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedStates)
            .WithAccountNumber("123456789")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "RoutingNumber");
    }

    [Fact]
    public void Build_WithUkWithoutSortCode_ReturnsFailure()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedKingdom)
            .WithAccountNumber("12345678")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.ShouldContain(e => e.Code == "Required" && e.MemberName == "SortCode");
    }

    #endregion

    #region Reset Tests

    [Fact]
    public void Reset_ClearsAllFields()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();
        builder
            .WithCountry(CountryCode.UnitedStates)
            .WithAccountNumber("123456789")
            .WithRoutingNumber("021000021");

        // Act
        builder.Reset();
        var (result, banking) = builder.Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
    }

    [Fact]
    public void Reset_AllowsReuse()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Build first time
        var (result1, banking1) = builder
            .WithUsBanking("021000021", "123456789")
            .Build();

        // Reset and build second time
        var (result2, banking2) = builder
            .Reset()
            .WithUkBanking("123456", "87654321")
            .Build();

        // Assert
        result1.IsValid.ShouldBeTrue();
        banking1.ShouldNotBeNull();
        banking1!.Country.ShouldBe(CountryCode.UnitedStates);

        result2.IsValid.ShouldBeTrue();
        banking2.ShouldNotBeNull();
        banking2!.Country.ShouldBe(CountryCode.UnitedKingdom);
    }

    #endregion

    #region Formatted Input Tests

    [Fact]
    public void Build_WithFormattedIban_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.Germany)
            .WithAccountNumber("DE89 3704 0044 0532 0130 00")
            .WithSwiftCode("DEUTDEFF")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
    }

    [Fact]
    public void Build_WithFormattedRoutingNumber_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedStates)
            .WithAccountNumber("123456789")
            .WithRoutingNumber("0210-0002-1")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
    }

    [Fact]
    public void Build_WithFormattedSortCode_ReturnsSuccess()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedKingdom)
            .WithAccountNumber("12345678")
            .WithSortCode("12-34-56")
            .Build();

        // Assert
        result.IsValid.ShouldBeTrue();
        banking.ShouldNotBeNull();
    }

    #endregion

    #region Multiple Errors Tests

    [Fact]
    public void Build_WithMultipleInvalidFields_ReturnsAllErrors()
    {
        // Arrange
        var builder = new BankingDetailsBuilder();

        // Act
        var (result, banking) = builder
            .WithCountry(CountryCode.UnitedStates)
            .WithAccountNumber("INVALID")
            .WithRoutingNumber("INVALID")
            .WithSwiftCode("INVALID")
            .Build();

        // Assert
        result.IsValid.ShouldBeFalse();
        banking.ShouldBeNull();
        result.Errors.Count.ShouldBeGreaterThan(1);
    }

    #endregion

    #region Fluent Interface Tests

    [Fact]
    public void FluentInterface_AllowsMethodChaining()
    {
        // This test just verifies the fluent interface works correctly
        var builder = new BankingDetailsBuilder();

        var result = builder
            .WithCountry(CountryCode.Germany)
            .WithAccountNumber("DE89370400440532013000")
            .WithSwiftCode("DEUTDEFF")
            .Reset()
            .WithUsBanking("021000021", "123456789")
            .Reset()
            .WithUkBanking("123456", "12345678")
            .Build();

        result.Result.IsValid.ShouldBeTrue();
    }

    #endregion
}
