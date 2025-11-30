using Shouldly;
using Validated.Primitives.Validators;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class MoneyValidatorsTests
{
    #region NonNegative Tests

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100.50)]
    [InlineData(999999.99)]
    public void NonNegative_Should_Pass_For_Valid_Values(decimal value)
    {
        // Arrange
        var validator = MoneyValidators.NonNegative("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    [InlineData(-999999.99)]
    public void NonNegative_Should_Fail_For_Negative_Values(decimal value)
    {
        // Arrange
        var validator = MoneyValidators.NonNegative("TestField");

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].MemberName.ShouldBe("TestField");
        result.Errors[0].Code.ShouldBe("NonNegative");
        result.Errors[0].Message.ShouldContain("cannot be negative");
    }

    [Fact]
    public void NonNegative_Uses_Default_FieldName()
    {
        // Arrange
        var validator = MoneyValidators.NonNegative();

        // Act
        var result = validator(-1);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("Money");
    }

    #endregion

    #region ValidDecimalPlaces Tests

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    [InlineData(1.5, 2)]
    [InlineData(1.50, 2)]
    [InlineData(100.99, 2)]
    [InlineData(100.1, 2)]
    [InlineData(100, 2)]
    public void ValidDecimalPlaces_Should_Pass_For_Valid_Precision(decimal value, int maxDecimalPlaces)
    {
        // Arrange
        var validator = MoneyValidators.ValidDecimalPlaces("TestField", maxDecimalPlaces);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(100.123, 2)]
    [InlineData(100.5555, 2)]
    [InlineData(1.999, 2)]
    public void ValidDecimalPlaces_Should_Fail_For_Too_Many_Decimal_Places(decimal value, int maxDecimalPlaces)
    {
        // Arrange
        var validator = MoneyValidators.ValidDecimalPlaces("TestField", maxDecimalPlaces);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].MemberName.ShouldBe("TestField");
        result.Errors[0].Code.ShouldBe("DecimalPlaces");
        result.Errors[0].Message.ShouldContain($"cannot have more than {maxDecimalPlaces} decimal places");
    }

    [Fact]
    public void ValidDecimalPlaces_Uses_Default_FieldName()
    {
        // Arrange
        var validator = MoneyValidators.ValidDecimalPlaces();

        // Act
        var result = validator(100.123m);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("Money");
    }

    [Fact]
    public void ValidDecimalPlaces_Uses_Default_MaxDecimalPlaces_Of_2()
    {
        // Arrange
        var validator = MoneyValidators.ValidDecimalPlaces("TestField");

        // Act
        var resultValid = validator(100.12m);
        var resultInvalid = validator(100.123m);

        // Assert
        resultValid.IsValid.ShouldBeTrue();
        resultInvalid.IsValid.ShouldBeFalse();
    }

    [Theory]
    [InlineData(100.1, 1)]
    [InlineData(100.12, 2)]
    [InlineData(100.123, 3)]
    public void ValidDecimalPlaces_Works_With_Different_Max_Values(decimal value, int maxDecimalPlaces)
    {
        // Arrange
        var validator = MoneyValidators.ValidDecimalPlaces("TestField", maxDecimalPlaces);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Range Tests

    [Theory]
    [InlineData(50, 0, 100)]
    [InlineData(0, 0, 100)]
    [InlineData(100, 0, 100)]
    [InlineData(25.5, 10, 50)]
    public void Range_Should_Pass_For_Values_Within_Range(decimal value, decimal min, decimal max)
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField", min, max);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(-1, 0, 100)]
    [InlineData(101, 0, 100)]
    [InlineData(5, 10, 50)]
    [InlineData(55, 10, 50)]
    public void Range_Should_Fail_For_Values_Outside_Range(decimal value, decimal min, decimal max)
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField", min, max);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].MemberName.ShouldBe("TestField");
    }

    [Fact]
    public void Range_Should_Fail_Below_Minimum_With_MinValue_Code()
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField", 10, 100);

        // Act
        var result = validator(5);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("MinValue");
        result.Errors[0].Message.ShouldContain("must be at least 10");
    }

    [Fact]
    public void Range_Should_Fail_Above_Maximum_With_MaxValue_Code()
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField", 0, 100);

        // Act
        var result = validator(101);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].Code.ShouldBe("MaxValue");
        result.Errors[0].Message.ShouldContain("must be at most 100");
    }

    [Fact]
    public void Range_Uses_Default_FieldName()
    {
        // Arrange
        var validator = MoneyValidators.Range();

        // Act
        var result = validator(-1);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("Money");
    }

    [Fact]
    public void Range_Uses_Default_Min_Of_Zero()
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField");

        // Act
        var resultNegative = validator(-1);
        var resultZero = validator(0);

        // Assert
        resultNegative.IsValid.ShouldBeFalse();
        resultZero.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Range_Uses_Default_Max_Of_Decimal_MaxValue()
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField");

        // Act
        var result = validator(decimal.MaxValue);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Multiple_Validators_Can_Be_Combined()
    {
        // Arrange
        var nonNegativeValidator = MoneyValidators.NonNegative("Amount");
        var decimalPlacesValidator = MoneyValidators.ValidDecimalPlaces("Amount", 2);
        var rangeValidator = MoneyValidators.Range("Amount", 0, 1000);

        // Act - Valid value
        var validValue = 100.50m;
        var result1 = nonNegativeValidator(validValue);
        var result2 = decimalPlacesValidator(validValue);
        var result3 = rangeValidator(validValue);

        // Assert
        result1.IsValid.ShouldBeTrue();
        result2.IsValid.ShouldBeTrue();
        result3.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validators_Fail_Independently()
    {
        // Arrange
        var nonNegativeValidator = MoneyValidators.NonNegative("Amount");
        var decimalPlacesValidator = MoneyValidators.ValidDecimalPlaces("Amount", 2);

        // Act - Negative value with too many decimal places
        var invalidValue = -100.123m;
        var result1 = nonNegativeValidator(invalidValue);
        var result2 = decimalPlacesValidator(invalidValue);

        // Assert
        result1.IsValid.ShouldBeFalse();
        result2.IsValid.ShouldBeFalse();
    }

    #endregion
}
