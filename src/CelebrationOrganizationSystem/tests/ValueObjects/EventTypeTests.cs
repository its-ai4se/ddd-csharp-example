using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class EventTypeTests
{
    [Fact]
    public void CreateEventType_WithValidName_ShouldSucceed()
    {
        // Arrange
        var name = "Birthday Party";
        var description = "A celebration of another year of life";

        // Act
        var eventType = new EventType(name, description);

        // Assert
        Assert.Equal("Birthday Party", eventType.Name);
        Assert.Equal("A celebration of another year of life", eventType.Description);
    }

    [Fact]
    public void CreateEventType_WithNullDescription_ShouldSucceed()
    {
        // Arrange
        var name = "Birthday Party";

        // Act
        var eventType = new EventType(name);

        // Assert
        Assert.Equal("Birthday Party", eventType.Name);
        Assert.Null(eventType.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateEventType_WithInvalidName_ShouldThrowException(string name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new EventType(name));
    }

    [Fact]
    public void EventType_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var eventType1 = new EventType("Birthday Party");
        var eventType2 = new EventType("Birthday Party");
        var eventType3 = new EventType("Graduation Party");

        // Assert
        Assert.Equal(eventType1, eventType2);
        Assert.NotEqual(eventType1, eventType3);
    }
}
