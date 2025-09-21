using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class DateTimeRangeTests
{
    [Fact]
    public void CreateDateTimeRange_WithValidRange_ShouldSucceed()
    {
        // Arrange
        var startDateTime = DateTime.Now.AddDays(7);
        var endDateTime = DateTime.Now.AddDays(7).AddHours(4);

        // Act
        var dateTimeRange = new DateTimeRange(startDateTime, endDateTime);

        // Assert
        Assert.Equal(startDateTime, dateTimeRange.StartDateTime);
        Assert.Equal(endDateTime, dateTimeRange.EndDateTime);
        Assert.Equal(TimeSpan.FromHours(4).TotalMilliseconds, dateTimeRange.Duration.TotalMilliseconds, 1);
    }

    [Fact]
    public void CreateDateTimeRange_WithStartAfterEnd_ShouldThrowException()
    {
        // Arrange
        var startDateTime = DateTime.Now.AddDays(7);
        var endDateTime = DateTime.Now.AddDays(6);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DateTimeRange(startDateTime, endDateTime));
    }

    [Fact]
    public void CreateDateTimeRange_WithSameStartAndEnd_ShouldThrowException()
    {
        // Arrange
        var dateTime = DateTime.Now.AddDays(7);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DateTimeRange(dateTime, dateTime));
    }

    [Fact]
    public void DateTimeRange_IsInRange_ShouldWorkCorrectly()
    {
        // Arrange
        var startDateTime = DateTime.Now.AddDays(7);
        var endDateTime = DateTime.Now.AddDays(7).AddHours(4);
        var dateTimeRange = new DateTimeRange(startDateTime, endDateTime);

        // Act & Assert
        Assert.True(dateTimeRange.IsInRange(startDateTime));
        Assert.True(dateTimeRange.IsInRange(endDateTime));
        Assert.True(dateTimeRange.IsInRange(startDateTime.AddHours(2)));
        Assert.False(dateTimeRange.IsInRange(startDateTime.AddDays(-1)));
        Assert.False(dateTimeRange.IsInRange(endDateTime.AddDays(1)));
    }

    [Fact]
    public void DateTimeRange_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var startDateTime = DateTime.Now.AddDays(7);
        var endDateTime = DateTime.Now.AddDays(7).AddHours(4);
        var dateTimeRange1 = new DateTimeRange(startDateTime, endDateTime);
        var dateTimeRange2 = new DateTimeRange(startDateTime, endDateTime);
        var dateTimeRange3 = new DateTimeRange(startDateTime, endDateTime.AddHours(1));

        // Assert
        Assert.Equal(dateTimeRange1, dateTimeRange2);
        Assert.NotEqual(dateTimeRange1, dateTimeRange3);
    }
}
