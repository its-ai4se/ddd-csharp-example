using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Session;

public class SessionAggregate : AggregateRoot
{
    public Guid CourseId { get; private set; }
    public Guid TutorId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTime ScheduledStartTime { get; private set; }
    public Duration Duration { get; private set; }
    public SessionStatus Status { get; private set; }
    public Money Price { get; private set; }
    public string? MeetingLink { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public SessionAggregate(Guid id, Guid courseId, Guid tutorId, Guid studentId, DateTime scheduledStartTime, Duration duration, Money price) : base(id)
    {
        CourseId = courseId;
        TutorId = tutorId;
        StudentId = studentId;
        ScheduledStartTime = scheduledStartTime;
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Status = SessionStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public SessionAggregate(Guid courseId, Guid tutorId, Guid studentId, DateTime scheduledStartTime, Duration duration, Money price) : base()
    {
        CourseId = courseId;
        TutorId = tutorId;
        StudentId = studentId;
        ScheduledStartTime = scheduledStartTime;
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Status = SessionStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public void Reschedule(DateTime newStartTime)
    {
        if (Status != SessionStatus.Scheduled)
            throw new DomainException("Only scheduled sessions can be rescheduled.");

        if (newStartTime <= DateTime.UtcNow)
            throw new DomainException("Session cannot be rescheduled to a past time.");

        ScheduledStartTime = newStartTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start(string? meetingLink = null)
    {
        if (Status != SessionStatus.Scheduled)
            throw new DomainException("Only scheduled sessions can be started.");

        Status = SessionStatus.InProgress;
        ActualStartTime = DateTime.UtcNow;
        MeetingLink = meetingLink;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? notes = null)
    {
        if (Status != SessionStatus.InProgress)
            throw new DomainException("Only sessions in progress can be completed.");

        Status = SessionStatus.Completed;
        ActualEndTime = DateTime.UtcNow;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == SessionStatus.Completed)
            throw new DomainException("Completed sessions cannot be cancelled.");

        Status = SessionStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddNotes(string notes)
    {
        Notes = notes ?? "";
        UpdatedAt = DateTime.UtcNow;
    }

    public Duration? GetActualDuration()
    {
        if (ActualStartTime.HasValue && ActualEndTime.HasValue)
        {
            var actualMinutes = (int)(ActualEndTime.Value - ActualStartTime.Value).TotalMinutes;
            return new Duration(actualMinutes);
        }
        return null;
    }

    public bool IsOverdue()
    {
        return Status == SessionStatus.Scheduled && ScheduledStartTime < DateTime.UtcNow;
    }

    public override string ToString() => $"Session: {ScheduledStartTime:yyyy-MM-dd HH:mm} (ID: {Id})";
}

public enum SessionStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}
