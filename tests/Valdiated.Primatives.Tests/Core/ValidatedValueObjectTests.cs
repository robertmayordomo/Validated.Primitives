using Xunit;
using Shouldly;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Tests.Core;

public class ValidatedValueObjectTests
{
    [Fact]
    public void ImplicitConversion_EmailAddress_To_String()
    {
        // Arrange
        var email = "test@example.com";
        var (_, emailAddress) = EmailAddress.TryCreate(email);

        // Act
        string convertedValue = emailAddress!;

        // Assert
        convertedValue.ShouldBe(email);
    }

    [Fact]
    public void ImplicitConversion_Can_Be_Used_In_String_Assignment()
    {
        // Arrange
        var expectedEmail = "user@example.com";
        var (_, emailAddress) = EmailAddress.TryCreate(expectedEmail);

        // Act
        string emailString = emailAddress!;

        // Assert
        emailString.ShouldBe(expectedEmail);
    }

    [Fact]
    public void ImplicitConversion_Can_Be_Used_In_Method_Parameter()
    {
        // Arrange
        var email = "method@test.com";
        var (_, emailAddress) = EmailAddress.TryCreate(email);

        // Act
        var result = AcceptStringParameter(emailAddress!);

        // Assert
        result.ShouldBe(email);
    }

    [Fact]
    public void ImplicitConversion_Can_Be_Used_In_String_Interpolation()
    {
        // Arrange
        var email = "interpolation@test.com";
        var (_, emailAddress) = EmailAddress.TryCreate(email);

        // Act
        var message = $"Email: {(string)emailAddress!}";

        // Assert
        message.ShouldBe($"Email: {email}");
    }

    [Fact]
    public void ImplicitConversion_Can_Be_Used_In_Collection()
    {
        // Arrange
        var email1 = "first@example.com";
        var email2 = "second@example.com";
        var (_, emailAddress1) = EmailAddress.TryCreate(email1);
        var (_, emailAddress2) = EmailAddress.TryCreate(email2);

        // Act
        List<string> emailStrings = new() { emailAddress1!, emailAddress2! };

        // Assert
        emailStrings.ShouldContain(email1);
        emailStrings.ShouldContain(email2);
        emailStrings.Count.ShouldBe(2);
    }

    [Fact]
    public void ImplicitConversion_Works_With_Different_Value_Types()
    {
        // Arrange
        var urlValue = "https://example.com";
        var (_, websiteUrl) = WebsiteUrl.TryCreate(urlValue);

        // Act
        string convertedUrl = websiteUrl!;

        // Assert
        convertedUrl.ShouldBe(urlValue);
    }

    [Fact]
    public void ImplicitConversion_Preserves_Original_Value()
    {
        // Arrange
        var email = "preserve@test.com";
        var (_, emailAddress) = EmailAddress.TryCreate(email);
        emailAddress.ShouldNotBeNull();

        // Act
        string converted = emailAddress;

        // Assert
        converted.ShouldBe(email);
        emailAddress.Value.ShouldBe(email);
    }

    [Fact]
    public void ImplicitConversion_Can_Be_Used_In_Conditional_Logic()
    {
        // Arrange
        var email = "conditional@test.com";
        var (_, emailAddress) = EmailAddress.TryCreate(email);

        // Act
        string emailString = emailAddress!;
        bool containsAt = emailString.Contains("@");

        // Assert
        containsAt.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_Works_With_Equality_Comparison()
    {
        // Arrange
        var email = "compare@test.com";
        var (_, emailAddress) = EmailAddress.TryCreate(email);

        // Act
        string converted = emailAddress!;

        // Assert
        (converted == email).ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FutureDate_To_DateTime()
    {
        // Arrange
        var tomorrow = DateTime.Today.AddDays(1);
        var (_, futureDate) = FutureDate.TryCreate(tomorrow);

        // Act
        DateTime convertedValue = futureDate!;

        // Assert
        convertedValue.ShouldBe(tomorrow);
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Used_In_DateTime_Assignment()
    {
        // Arrange
        var expectedDate = DateTime.Today.AddDays(5);
        var (_, futureDate) = FutureDate.TryCreate(expectedDate);

        // Act
        DateTime dateTime = futureDate!;

        // Assert
        dateTime.ShouldBe(expectedDate);
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Used_In_Method_Parameter()
    {
        // Arrange
        var nextWeek = DateTime.Today.AddDays(7);
        var (_, futureDate) = FutureDate.TryCreate(nextWeek);

        // Act
        var result = AcceptDateTimeParameter(futureDate!);

        // Assert
        result.ShouldBe(nextWeek);
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Used_In_DateTime_Operations()
    {
        // Arrange
        var baseDate = DateTime.Today.AddDays(10);
        var (_, futureDate) = FutureDate.TryCreate(baseDate);

        // Act
        DateTime convertedDate = futureDate!;
        var daysDifference = (convertedDate - DateTime.Today).Days;

        // Assert
        daysDifference.ShouldBe(10);
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Used_In_Collection()
    {
        // Arrange
        var date1 = DateTime.Today.AddDays(1);
        var date2 = DateTime.Today.AddDays(2);
        var (_, futureDate1) = FutureDate.TryCreate(date1);
        var (_, futureDate2) = FutureDate.TryCreate(date2);

        // Act
        List<DateTime> dates = new() { futureDate1!, futureDate2! };

        // Assert
        dates.ShouldContain(date1);
        dates.ShouldContain(date2);
        dates.Count.ShouldBe(2);
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Preserves_Original_Value()
    {
        // Arrange
        var expectedDate = DateTime.Today.AddMonths(1);
        var (_, futureDate) = FutureDate.TryCreate(expectedDate);
        futureDate.ShouldNotBeNull();

        // Act
        DateTime converted = futureDate;

        // Assert
        converted.ShouldBe(expectedDate);
        futureDate.Value.ShouldBe(expectedDate);
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Used_In_Conditional_Logic()
    {
        // Arrange
        var futureDateTime = DateTime.Today.AddDays(30);
        var (_, futureDate) = FutureDate.TryCreate(futureDateTime);

        // Act
        DateTime convertedDate = futureDate!;
        bool isInFuture = convertedDate > DateTime.Today;

        // Assert
        isInFuture.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Works_With_Equality_Comparison()
    {
        // Arrange
        var expectedDate = DateTime.Today.AddYears(1);
        var (_, futureDate) = FutureDate.TryCreate(expectedDate);

        // Act
        DateTime converted = futureDate!;

        // Assert
        (converted == expectedDate).ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Used_With_DateTime_Formatting()
    {
        // Arrange
        var date = new DateTime(2025, 12, 25, 0, 0, 0);
        var (_, futureDate) = FutureDate.TryCreate(date);

        // Act
        DateTime convertedDate = futureDate!;
        var formatted = convertedDate.ToString("yyyy-MM-dd");

        // Assert
        formatted.ShouldBe("2025-12-25");
    }

    [Fact]
    public void ImplicitConversion_FutureDate_Can_Be_Compared_With_Other_DateTimes()
    {
        // Arrange
        var date1 = DateTime.Today.AddDays(10);
        var date2 = DateTime.Today.AddDays(20);
        var (_, futureDate) = FutureDate.TryCreate(date1);

        // Act
        DateTime converted = futureDate!;
        bool isEarlier = converted < date2;

        // Assert
        isEarlier.ShouldBeTrue();
    }

    private static string AcceptStringParameter(string value)
    {
        return value;
    }

    private static DateTime AcceptDateTimeParameter(DateTime value)
    {
        return value;
    }
}
