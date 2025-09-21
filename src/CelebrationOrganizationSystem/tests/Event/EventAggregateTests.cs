using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.Event;

public class EventAggregateTests
{
    private EventAggregate CreateValidEvent()
    {
        var eventType = new EventType("Birthday Party", "A celebration of another year of life");
        var dateTimeRange = new DateTimeRange(
            DateTime.Now.AddDays(7),
            DateTime.Now.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));
        var organizerId = Guid.NewGuid();

        return new EventAggregate("Sarah's 25th Birthday", eventType, dateTimeRange, location, organizerId);
    }

    [Fact]
    public void CreateEvent_WithValidData_ShouldSucceed()
    {
        // Arrange
        var occasion = "Sarah's 25th Birthday";
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.Now.AddDays(7),
            DateTime.Now.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));
        var organizerId = Guid.NewGuid();

        // Act
        var eventAggregate = new EventAggregate(occasion, eventType, dateTimeRange, location, organizerId);

        // Assert
        Assert.Equal(occasion, eventAggregate.Occasion);
        Assert.Equal(eventType, eventAggregate.EventType);
        Assert.Equal(dateTimeRange, eventAggregate.DateTimeRange);
        Assert.Equal(location, eventAggregate.Location);
        Assert.Equal(organizerId, eventAggregate.OrganizerId);
        Assert.Empty(eventAggregate.AttendeeIds);
        Assert.Empty(eventAggregate.TaskIds);
        Assert.True(eventAggregate.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void CreateEvent_WithNullValues_ShouldThrowException()
    {
        // Arrange
        var occasion = "Sarah's 25th Birthday";
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.Now.AddDays(7),
            DateTime.Now.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));
        var organizerId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new EventAggregate("", eventType, dateTimeRange, location, organizerId));
        Assert.Throws<ArgumentNullException>(() => new EventAggregate(occasion, null!, dateTimeRange, location, organizerId));
        Assert.Throws<ArgumentNullException>(() => new EventAggregate(occasion, eventType, null!, location, organizerId));
        Assert.Throws<ArgumentNullException>(() => new EventAggregate(occasion, eventType, dateTimeRange, null!, organizerId));
    }

    [Fact]
    public void AddAttendee_WithValidAttendeeId_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var attendeeId = Guid.NewGuid();

        // Act
        eventAggregate.AddAttendee(attendeeId);

        // Assert
        Assert.Single(eventAggregate.AttendeeIds);
        Assert.Contains(attendeeId, eventAggregate.AttendeeIds);
    }

    [Fact]
    public void AddAttendee_WithEmptyAttendeeId_ShouldThrowException()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => eventAggregate.AddAttendee(Guid.Empty));
    }

    [Fact]
    public void AddAttendee_WithDuplicateAttendeeId_ShouldThrowException()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var attendeeId = Guid.NewGuid();
        eventAggregate.AddAttendee(attendeeId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => eventAggregate.AddAttendee(attendeeId));
    }

    [Fact]
    public void RemoveAttendee_WithExistingAttendeeId_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var attendeeId = Guid.NewGuid();
        eventAggregate.AddAttendee(attendeeId);

        // Act
        eventAggregate.RemoveAttendee(attendeeId);

        // Assert
        Assert.Empty(eventAggregate.AttendeeIds);
    }

    [Fact]
    public void AddTask_WithValidTaskId_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var taskId = Guid.NewGuid();

        // Act
        eventAggregate.AddTask(taskId);

        // Assert
        Assert.Single(eventAggregate.TaskIds);
        Assert.Contains(taskId, eventAggregate.TaskIds);
    }

    [Fact]
    public void AddTask_WithEmptyTaskId_ShouldThrowException()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => eventAggregate.AddTask(Guid.Empty));
    }

    [Fact]
    public void AddTask_WithDuplicateTaskId_ShouldThrowException()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var taskId = Guid.NewGuid();
        eventAggregate.AddTask(taskId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => eventAggregate.AddTask(taskId));
    }

    [Fact]
    public void RemoveTask_WithExistingTaskId_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var taskId = Guid.NewGuid();
        eventAggregate.AddTask(taskId);

        // Act
        eventAggregate.RemoveTask(taskId);

        // Assert
        Assert.Empty(eventAggregate.TaskIds);
    }

    [Fact]
    public void UpdateOccasion_WithValidOccasion_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var newOccasion = "Sarah's 26th Birthday";

        // Act
        eventAggregate.UpdateOccasion(newOccasion);

        // Assert
        Assert.Equal(newOccasion, eventAggregate.Occasion);
    }

    [Fact]
    public void UpdateOccasion_WithEmptyOccasion_ShouldThrowException()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => eventAggregate.UpdateOccasion(""));
    }

    [Fact]
    public void UpdateEventType_WithValidEventType_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var newEventType = new EventType("Graduation Party");

        // Act
        eventAggregate.UpdateEventType(newEventType);

        // Assert
        Assert.Equal(newEventType, eventAggregate.EventType);
    }

    [Fact]
    public void UpdateDateTimeRange_WithValidRange_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var newDateTimeRange = new DateTimeRange(
            DateTime.Now.AddDays(14),
            DateTime.Now.AddDays(14).AddHours(6)
        );

        // Act
        eventAggregate.UpdateDateTimeRange(newDateTimeRange);

        // Assert
        Assert.Equal(newDateTimeRange, eventAggregate.DateTimeRange);
    }

    [Fact]
    public void UpdateLocation_WithValidLocation_ShouldSucceed()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();
        var newLocation = new Location("Library", new Address("789 Pine St", "Anytown", "CA", "12345", "USA"));

        // Act
        eventAggregate.UpdateLocation(newLocation);

        // Assert
        Assert.Equal(newLocation, eventAggregate.Location);
    }

    [Fact]
    public void IsEventInPast_WithPastEvent_ShouldReturnTrue()
    {
        // Arrange
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.Now.AddDays(-7),
            DateTime.Now.AddDays(-7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));
        var organizerId = Guid.NewGuid();

        var eventAggregate = new EventAggregate("Past Event", eventType, dateTimeRange, location, organizerId);

        // Act & Assert
        Assert.True(eventAggregate.IsEventInPast());
        Assert.False(eventAggregate.IsEventInFuture());
        Assert.False(eventAggregate.IsEventCurrentlyHappening());
    }

    [Fact]
    public void IsEventInFuture_WithFutureEvent_ShouldReturnTrue()
    {
        // Arrange
        var eventAggregate = CreateValidEvent();

        // Act & Assert
        Assert.False(eventAggregate.IsEventInPast());
        Assert.True(eventAggregate.IsEventInFuture());
        Assert.False(eventAggregate.IsEventCurrentlyHappening());
    }

    [Fact]
    public void IsEventCurrentlyHappening_WithCurrentEvent_ShouldReturnTrue()
    {
        // Arrange
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(3)
        );
        var location = new Location("Community Center", new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA"));
        var organizerId = Guid.NewGuid();

        var eventAggregate = new EventAggregate("Current Event", eventType, dateTimeRange, location, organizerId);

        // Act & Assert
        Assert.False(eventAggregate.IsEventInPast());
        Assert.False(eventAggregate.IsEventInFuture());
        Assert.True(eventAggregate.IsEventCurrentlyHappening());
    }
}
