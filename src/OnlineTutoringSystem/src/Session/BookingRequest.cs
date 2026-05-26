using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Session;

public class BookingRequest : AggregateRoot
{
    public Guid TutorId { get; private set; }
    public Guid StudentId { get; private set; }
    public Subject Subject { get; private set; }
    public ExpertiseLevel Level { get; private set; }
    public DateTime SuggestedTime { get; private set; }
    public DateTime? ProposedTime { get; private set; }
    public BookingRequestStatus Status { get; private set; }

    public BookingRequest(Guid tutorId, Guid studentId, Subject subject, ExpertiseLevel level, DateTime suggestedTime) : base()
    {
        TutorId = tutorId;
        StudentId = studentId;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Level = level ?? throw new ArgumentNullException(nameof(level));
        SuggestedTime = suggestedTime;
        Status = BookingRequestStatus.Pending;
    }

    // BR-009: tutor proposes an alternative slot
    public void ProposeAlternativeTime(DateTime alternativeTime)
    {
        if (Status != BookingRequestStatus.Pending)
            throw new DomainException("Can only propose alternative time for a pending request.");
        ProposedTime = alternativeTime;
        Status = BookingRequestStatus.TutorProposed;
    }

    // BR-009: tutor confirms the student's suggested time directly
    public void TutorConfirm()
    {
        if (Status != BookingRequestStatus.Pending)
            throw new DomainException("Tutor can only confirm a pending request.");
        Status = BookingRequestStatus.Confirmed;
    }

    // BR-010: student accepts tutor's proposed alternative
    public void StudentAccept()
    {
        if (Status != BookingRequestStatus.TutorProposed)
            throw new DomainException("Student can only accept after tutor has proposed an alternative.");
        Status = BookingRequestStatus.Confirmed;
    }

    // BR-010: produce a session only once both parties have agreed
    public SessionAggregate CreateSession(Money price)
    {
        if (Status != BookingRequestStatus.Confirmed)
            throw new DomainException("Session can only be created from a confirmed booking request.");

        var agreedTime = ProposedTime ?? SuggestedTime;
        return new SessionAggregate(TutorId, StudentId, Subject, Level, agreedTime, price);
    }

    public override string ToString() => $"BookingRequest: {Subject} ({Level}) [{Status}] (ID: {Id})";
}
