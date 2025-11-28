using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.ValueObjects;

public class FutureDateTests
{
    [Fact]
    public void TryCreate_Fails_For_Past_Date()
    {
        var past = DateTime.UtcNow.AddDays(-1);
        var (res, val) = FutureDate.TryCreate(past);
        res.IsValid.ShouldBeFalse("Result should be invalid for past dates");
        val.ShouldBeNull("Value should be null when validation fails");
    }

    [Fact]
    public void TryCreate_Succeeds_For_Today_Or_Future()
    {
        var today = DateTime.UtcNow.Date;
        var (res, val) = FutureDate.TryCreate(today);
        res.IsValid.ShouldBeTrue("Result should be valid for today or future dates");
        val.ShouldNotBeNull("Value should not be null when validation succeeds");
    }
}
