using Shouldly;
using Validated.Primitives.Validators;
using Xunit;

namespace Validated.Primitives.Tests.Validators;

public class MoneyValidatorsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100.50)]
    [InlineData(999999.99)]
    public void GivenNonNegative_WhenValidValues_ThenShouldPass(decimal value)
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
    public void GivenNonNegative_WhenNegativeValues_ThenShouldFail(decimal value)
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
    public void GivenNonNegative_WhenNoFieldNameProvided_ThenShouldUseDefault()
    {
        // Arrange
        var validator = MoneyValidators.NonNegative();

        // Act
        var result = validator(-1);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors[0].MemberName.ShouldBe("Money");
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    [InlineData(1.5, 2)]
    [InlineData(1.50, 2)]
    [InlineData(100.99, 2)]
    [InlineData(100.1, 2)]
    [InlineData(100, 2)]
    public void GivenValidDecimalPlaces_WhenValidPrecision_ThenShouldPass(decimal value, int maxDecimalPlaces)
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
    public void GivenValidDecimalPlaces_WhenTooManyDecimalPlaces_ThenShouldFail(decimal value, int maxDecimalPlaces)
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
    public void GivenValidDecimalPlaces_WhenNoFieldNameProvided_ThenShouldUseDefault()
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
    public void GivenValidDecimalPlaces_WhenNoMaxProvided_ThenShouldUseDefaultOf2()
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
    public void GivenValidDecimalPlaces_WhenDifferentMaxValues_ThenShouldWorkCorrectly(decimal value, int maxDecimalPlaces)
    {
        // Arrange
        var validator = MoneyValidators.ValidDecimalPlaces("TestField", maxDecimalPlaces);

        // Act
        var result = validator(value);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(50, 0, 100)]
    [InlineData(0, 0, 100)]
    [InlineData(100, 0, 100)]
    [InlineData(25.5, 10, 50)]
    public void GivenRange_WhenValuesWithinRange_ThenShouldPass(decimal value, decimal min, decimal max)
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
    public void GivenRange_WhenValuesOutsideRange_ThenShouldFail(decimal value, decimal min, decimal max)
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
    public void GivenRange_WhenBelowMinimum_ThenShouldFailWithMinValueCode()
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
    public void GivenRange_WhenAboveMaximum_ThenShouldFailWithMaxValueCode()
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
    public void GivenRange_WhenNoFieldNameProvided_ThenShouldUseDefault()
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
    public void GivenRange_WhenNoMinProvided_ThenShouldUseDefaultOfZero()
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
    public void GivenRange_WhenNoMaxProvided_ThenShouldUseDefaultOfDecimalMaxValue()
    {
        // Arrange
        var validator = MoneyValidators.Range("TestField");

        // Act
        var result = validator(decimal.MaxValue);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void GivenMultipleValidators_WhenValidValue_ThenAllShouldPass()
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
    public void GivenMultipleValidators_WhenInvalidValue_ThenShouldFailIndependently()
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
}
