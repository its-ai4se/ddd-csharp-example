using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Task;

public class ChecklistTaskTemplate : Entity
{
    public string EventTypeName { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsAttendeeAccomplishableByDefault { get; private set; }

    public ChecklistTaskTemplate(Guid id, string eventTypeName, string title, string? description = null, bool isAttendeeAccomplishableByDefault = false) : base(id)
    {
        EventTypeName = ValidateEventTypeName(eventTypeName);
        Title = ValidateTitle(title);
        Description = description?.Trim();
        IsAttendeeAccomplishableByDefault = isAttendeeAccomplishableByDefault;
    }

    public ChecklistTaskTemplate(string eventTypeName, string title, string? description = null, bool isAttendeeAccomplishableByDefault = false) : base()
    {
        EventTypeName = ValidateEventTypeName(eventTypeName);
        Title = ValidateTitle(title);
        Description = description?.Trim();
        IsAttendeeAccomplishableByDefault = isAttendeeAccomplishableByDefault;
    }

    private static string ValidateEventTypeName(string eventTypeName)
    {
        if (string.IsNullOrWhiteSpace(eventTypeName))
        {
            throw new ArgumentException("Event type name cannot be empty or whitespace.", nameof(eventTypeName));
        }

        return eventTypeName.Trim();
    }

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Checklist task title cannot be empty or whitespace.", nameof(title));
        }

        return title.Trim();
    }
}

public class ChecklistTaskAggregate : AggregateRoot
{
    public Guid EventId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public ChecklistTaskStatus Status { get; private set; }
    public bool IsAttendeeAccomplishable { get; private set; }
    public Guid? SelectedByAttendeeId { get; private set; }

    public ChecklistTaskAggregate(Guid id, Guid eventId, string title, string? description = null, bool isAttendeeAccomplishable = false) : base(id)
    {
        EventId = ValidateEventId(eventId);
        Title = ValidateTitle(title);
        Description = description?.Trim();
        IsAttendeeAccomplishable = isAttendeeAccomplishable;
        Status = ChecklistTaskStatus.NeedsToBeDone;
    }

    public ChecklistTaskAggregate(Guid eventId, string title, string? description = null, bool isAttendeeAccomplishable = false) : base()
    {
        EventId = ValidateEventId(eventId);
        Title = ValidateTitle(title);
        Description = description?.Trim();
        IsAttendeeAccomplishable = isAttendeeAccomplishable;
        Status = ChecklistTaskStatus.NeedsToBeDone;
    }

    public void SetStatus(ChecklistTaskStatus status)
    {
        Status = status;
    }

    public void DesignateForAttendees()
    {
        IsAttendeeAccomplishable = true;
    }

    public void SelectByAttendee(Guid attendeeId)
    {
        if (attendeeId == Guid.Empty)
        {
            throw new ArgumentException("Attendee ID cannot be empty.", nameof(attendeeId));
        }

        if (!IsAttendeeAccomplishable)
        {
            throw new InvalidOperationException("Checklist task is not designated for attendees.");
        }

        if (Status == ChecklistTaskStatus.NotApplicable)
        {
            throw new InvalidOperationException("Not applicable checklist tasks cannot be selected by attendees.");
        }

        if (SelectedByAttendeeId.HasValue && SelectedByAttendeeId.Value != attendeeId)
        {
            throw new InvalidOperationException("Checklist task has already been selected by another attendee.");
        }

        SelectedByAttendeeId = attendeeId;
    }

    public bool IsDone => Status == ChecklistTaskStatus.Done;
    public bool IsNotApplicable => Status == ChecklistTaskStatus.NotApplicable;

    private static Guid ValidateEventId(Guid eventId)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));
        }

        return eventId;
    }

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Checklist task title cannot be empty or whitespace.", nameof(title));
        }

        return title.Trim();
    }

    public override string ToString() => $"Checklist task: {Title} ({Status})";
}

public enum ChecklistTaskStatus
{
    NeedsToBeDone,
    Done,
    NotApplicable
}
