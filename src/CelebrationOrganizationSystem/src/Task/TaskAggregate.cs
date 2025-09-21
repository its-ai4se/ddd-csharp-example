using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Task;

public class TaskAggregate : AggregateRoot
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskType Type { get; private set; }
    public Guid? AssignedToAttendeeId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public TaskAggregate(Guid id, string title, string? description = null, TaskType type = TaskType.General) : base(id)
    {
        Title = ValidateTitle(title);
        Description = description?.Trim();
        Type = type;
        Status = TaskStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
    }

    public TaskAggregate(string title, string? description = null, TaskType type = TaskType.General) : base()
    {
        Title = ValidateTitle(title);
        Description = description?.Trim();
        Type = type;
        Status = TaskStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string newTitle)
    {
        Title = ValidateTitle(newTitle);
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription?.Trim();
    }

    public void MarkAsInProgress()
    {
        if (Status == TaskStatus.Completed)
        {
            throw new InvalidOperationException("Cannot mark a completed task as in progress.");
        }

        Status = TaskStatus.InProgress;
    }

    public void MarkAsCompleted()
    {
        Status = TaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsNotApplicable()
    {
        Status = TaskStatus.NotApplicable;
    }

    public void AssignToAttendee(Guid attendeeId)
    {
        if (attendeeId == Guid.Empty)
        {
            throw new ArgumentException("Attendee ID cannot be empty.", nameof(attendeeId));
        }

        AssignedToAttendeeId = attendeeId;
    }

    public void UnassignFromAttendee()
    {
        AssignedToAttendeeId = null;
    }

    public bool IsAssigned => AssignedToAttendeeId.HasValue;
    public bool IsCompleted => Status == TaskStatus.Completed;
    public bool IsInProgress => Status == TaskStatus.InProgress;
    public bool IsNotApplicable => Status == TaskStatus.NotApplicable;

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title cannot be empty or whitespace.", nameof(title));
        }

        return title.Trim();
    }

    public override string ToString() => $"Task: {Title} ({Status})";
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed,
    NotApplicable
}

public enum TaskType
{
    General,
    Preparation,
    Setup,
    Cleanup,
    Food,
    Entertainment,
    Decoration
}
