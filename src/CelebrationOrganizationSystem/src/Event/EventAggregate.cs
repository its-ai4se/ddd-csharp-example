using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Event;

public class EventAggregate : AggregateRoot
{
    public string Occasion { get; private set; }
    public EventType EventType { get; private set; }
    public DateTimeRange DateTimeRange { get; private set; }
    public Location Location { get; private set; }

    private readonly List<EventOrganizer> _organizers = [];
    private readonly List<Guid> _attendeeIds = [];
    private readonly List<Guid> _checklistTaskIds = [];

    public EventAggregate(Guid id, string occasion, EventType eventType, DateTimeRange dateTimeRange, Location location, IEnumerable<EventOrganizer> organizers) : base(id)
    {
        Occasion = ValidateOccasion(occasion);
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        DateTimeRange = dateTimeRange ?? throw new ArgumentNullException(nameof(dateTimeRange));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        AddInitialOrganizers(organizers);
    }

    public EventAggregate(string occasion, EventType eventType, DateTimeRange dateTimeRange, Location location, IEnumerable<EventOrganizer> organizers) : base()
    {
        Occasion = ValidateOccasion(occasion);
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        DateTimeRange = dateTimeRange ?? throw new ArgumentNullException(nameof(dateTimeRange));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        AddInitialOrganizers(organizers);
    }

    public IReadOnlyList<EventOrganizer> Organizers => _organizers.AsReadOnly();
    public IReadOnlyList<Guid> OrganizerIds => _organizers.Select(o => o.OrganizerId).ToList().AsReadOnly();
    public IReadOnlyList<Guid> AttendingOrganizerIds => _organizers.Where(o => o.IsAttending).Select(o => o.OrganizerId).ToList().AsReadOnly();
    public IReadOnlyList<Guid> NonAttendingOrganizerIds => _organizers.Where(o => !o.IsAttending).Select(o => o.OrganizerId).ToList().AsReadOnly();
    public IReadOnlyList<Guid> AttendeeIds => _attendeeIds.AsReadOnly();
    public IReadOnlyList<Guid> ChecklistTaskIds => _checklistTaskIds.AsReadOnly();

    public void AddOrganizer(Guid organizerId, bool isAttending)
    {
        if (_organizers.Any(o => o.OrganizerId == organizerId))
        {
            throw new InvalidOperationException("Organizer is already assigned to this event.");
        }

        _organizers.Add(new EventOrganizer(organizerId, isAttending));
    }

    public void SetOrganizerAttendance(Guid organizerId, bool isAttending)
    {
        var organizer = _organizers.FirstOrDefault(o => o.OrganizerId == organizerId)
            ?? throw new InvalidOperationException("Organizer is not assigned to this event.");

        organizer.SetAttendance(isAttending);
    }

    public void AddAttendee(Guid attendeeId)
    {
        if (attendeeId == Guid.Empty)
        {
            throw new ArgumentException("Attendee ID cannot be empty.", nameof(attendeeId));
        }

        if (!_attendeeIds.Contains(attendeeId))
        {
            _attendeeIds.Add(attendeeId);
        }
    }

    public void AddChecklistTask(Guid taskId)
    {
        if (taskId == Guid.Empty)
        {
            throw new ArgumentException("Checklist task ID cannot be empty.", nameof(taskId));
        }

        if (_checklistTaskIds.Contains(taskId))
        {
            throw new InvalidOperationException("Checklist task is already added to this event.");
        }

        _checklistTaskIds.Add(taskId);
    }

    private void AddInitialOrganizers(IEnumerable<EventOrganizer> organizers)
    {
        if (organizers is null)
        {
            throw new ArgumentNullException(nameof(organizers));
        }

        var organizerList = organizers.ToList();
        if (organizerList.Count == 0)
        {
            throw new ArgumentException("An event must have at least one organizer.", nameof(organizers));
        }

        foreach (var organizer in organizerList)
        {
            if (_organizers.Any(o => o.OrganizerId == organizer.OrganizerId))
            {
                throw new ArgumentException("An event cannot contain the same organizer more than once.", nameof(organizers));
            }

            _organizers.Add(organizer);
        }
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
