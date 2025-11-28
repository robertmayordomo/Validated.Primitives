using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class DateOfBirthTests
{
    [Fact]
    public void TryCreate_Fails_For_Future_Date()
    {
        var future = DateTime.UtcNow.AddDays(1);
        var (res, val) = DateOfBirth.TryCreate(future);
        res.IsValid.ShouldBeFalse("Result should be invalid for future dates");
        val.ShouldBeNull("Value should be null when validation fails");
    }

    [Fact]
    public void TryCreate_Succeeds_For_Past_Date()
    {
        var past = DateTime.UtcNow.AddYears(-20);
        var (res, val) = DateOfBirth.TryCreate(past);
        res.IsValid.ShouldBeTrue("Result should be valid for past dates");
        val.ShouldNotBeNull("Value should not be null when validation succeeds");
    }
}
