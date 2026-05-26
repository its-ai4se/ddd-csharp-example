using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Session;

public class SessionAggregate : AggregateRoot
{
    public Guid TutorId { get; private set; }
    public Guid StudentId { get; private set; }
    public Subject Subject { get; private set; }
    public ExpertiseLevel Level { get; private set; }
    public DateTime ScheduledStartTime { get; private set; }
    public SessionStatus Status { get; private set; }
    public Money Price { get; private set; }
    public CancelledBy? CancelledByActor { get; private set; }
    public CancellationPenalty? Penalty { get; private set; }

    public SessionAggregate(Guid tutorId, Guid studentId, Subject subject, ExpertiseLevel level,
        DateTime scheduledStartTime, Money price) : base()
    {
        TutorId = tutorId;
        StudentId = studentId;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Level = level ?? throw new ArgumentNullException(nameof(level));
        ScheduledStartTime = scheduledStartTime;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Status = SessionStatus.Scheduled;
    }

    public void Start()
    {
        if (Status != SessionStatus.Scheduled)
            throw new DomainException("Only scheduled sessions can be started.");
        Status = SessionStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != SessionStatus.InProgress)
            throw new DomainException("Only sessions in progress can be completed.");
        Status = SessionStatus.Completed;
    }

    public void Cancel(CancelledBy by, DateTime cancelledAt)
    {
        if (Status == SessionStatus.Completed)
            throw new DomainException("Completed sessions cannot be cancelled.");
        if (Status == SessionStatus.Cancelled)
            throw new DomainException("Session is already cancelled.");

        Status = SessionStatus.Cancelled;
        CancelledByActor = by;

        if ((ScheduledStartTime - cancelledAt).TotalHours < 24)
        {
            Penalty = by == CancelledBy.Student
                ? CancellationPenalty.StudentCharge(Price)
                : CancellationPenalty.TutorDiscount(Price);
        }
    }

    public BookingRequest ScheduleFollowUp(DateTime proposedTime)
    {
        if (Status != SessionStatus.InProgress)
            throw new DomainException("Follow-up sessions can only be scheduled during an active session.");
        return new BookingRequest(TutorId, StudentId, Subject, Level, proposedTime);
    }

    public override string ToString() => $"Session: {Subject} {ScheduledStartTime:yyyy-MM-dd HH:mm} (ID: {Id})";
}
