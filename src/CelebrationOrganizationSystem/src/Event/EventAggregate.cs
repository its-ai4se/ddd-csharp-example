using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Event;

public class EventAggregate : AggregateRoot
{
    public string Occasion { get; private set; }
    public EventType EventType { get; private set; }
    public DateTimeRange DateTimeRange { get; private set; }
    public Location Location { get; private set; }
    public Guid OrganizerId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Guid> _attendeeIds = new();
    private readonly List<Guid> _taskIds = new();

    public EventAggregate(Guid id, string occasion, EventType eventType, DateTimeRange dateTimeRange, Location location, Guid organizerId) : base(id)
    {
        Occasion = ValidateOccasion(occasion);
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        DateTimeRange = dateTimeRange ?? throw new ArgumentNullException(nameof(dateTimeRange));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        OrganizerId = organizerId;
        CreatedAt = DateTime.UtcNow;
    }

    public EventAggregate(string occasion, EventType eventType, DateTimeRange dateTimeRange, Location location, Guid organizerId) : base()
    {
        Occasion = ValidateOccasion(occasion);
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        DateTimeRange = dateTimeRange ?? throw new ArgumentNullException(nameof(dateTimeRange));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        OrganizerId = organizerId;
        CreatedAt = DateTime.UtcNow;
    }

    public IReadOnlyList<Guid> AttendeeIds => _attendeeIds.AsReadOnly();
    public IReadOnlyList<Guid> TaskIds => _taskIds.AsReadOnly();

    public void UpdateOccasion(string newOccasion)
    {
        Occasion = ValidateOccasion(newOccasion);
    }

    public void UpdateEventType(EventType newEventType)
    {
        EventType = newEventType ?? throw new ArgumentNullException(nameof(newEventType));
    }

    public void UpdateDateTimeRange(DateTimeRange newDateTimeRange)
    {
        DateTimeRange = newDateTimeRange ?? throw new ArgumentNullException(nameof(newDateTimeRange));
    }

    public void UpdateLocation(Location newLocation)
    {
        Location = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
    }

    public void AddAttendee(Guid attendeeId)
    {
        if (attendeeId == Guid.Empty)
        {
            throw new ArgumentException("Attendee ID cannot be empty.", nameof(attendeeId));
        }

        if (_attendeeIds.Contains(attendeeId))
        {
            throw new InvalidOperationException("Attendee is already added to this event.");
        }

        _attendeeIds.Add(attendeeId);
    }

    public void RemoveAttendee(Guid attendeeId)
    {
        _attendeeIds.Remove(attendeeId);
    }

    public void AddTask(Guid taskId)
    {
        if (taskId == Guid.Empty)
        {
            throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));
        }

        if (_taskIds.Contains(taskId))
        {
            throw new InvalidOperationException("Task is already added to this event.");
        }

        _taskIds.Add(taskId);
    }

    public void RemoveTask(Guid taskId)
    {
        _taskIds.Remove(taskId);
    }

    public bool IsEventInPast()
    {
        return DateTimeRange.EndDateTime < DateTime.UtcNow;
    }

    public bool IsEventInFuture()
    {
        return DateTimeRange.StartDateTime > DateTime.UtcNow;
    }

    public bool IsEventCurrentlyHappening()
    {
        var now = DateTime.UtcNow;
        return DateTimeRange.IsInRange(now);
    }

    private static string ValidateOccasion(string occasion)
    {
        if (string.IsNullOrWhiteSpace(occasion))
        {
            throw new ArgumentException("Occasion cannot be empty or whitespace.", nameof(occasion));
        }

        return occasion.Trim();
    }

    public override string ToString() => $"Event: {Occasion} ({EventType}) at {Location}";
}
