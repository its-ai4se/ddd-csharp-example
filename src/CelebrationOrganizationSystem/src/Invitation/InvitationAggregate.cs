using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Invitation;

public class InvitationAggregate : AggregateRoot
{
    public Guid EventId { get; private set; }
    public Guid AttendeeId { get; private set; }
    public EmailAddress AttendeeEmail { get; private set; }
    public PersonName AttendeeName { get; private set; }
    public DateTime SentAt { get; private set; }
    public InvitationResponse? Response { get; private set; }

    public InvitationAggregate(Guid id, Guid eventId, Guid attendeeId, EmailAddress attendeeEmail, PersonName attendeeName) : base(id)
    {
        EventId = eventId;
        AttendeeId = attendeeId;
        AttendeeEmail = attendeeEmail ?? throw new ArgumentNullException(nameof(attendeeEmail));
        AttendeeName = attendeeName ?? throw new ArgumentNullException(nameof(attendeeName));
        SentAt = DateTime.UtcNow;
    }

    public InvitationAggregate(Guid eventId, Guid attendeeId, EmailAddress attendeeEmail, PersonName attendeeName) : base()
    {
        EventId = eventId;
        AttendeeId = attendeeId;
        AttendeeEmail = attendeeEmail ?? throw new ArgumentNullException(nameof(attendeeEmail));
        AttendeeName = attendeeName ?? throw new ArgumentNullException(nameof(attendeeName));
        SentAt = DateTime.UtcNow;
    }

    public void RespondToInvitation(InvitationStatus status)
    {
        if (Response != null)
        {
            throw new InvalidOperationException("Invitation has already been responded to.");
        }

        Response = new InvitationResponse(status, DateTime.UtcNow);
    }

    public void UpdateResponse(InvitationStatus newStatus)
    {
        if (Response == null)
        {
            throw new InvalidOperationException("No response has been given yet.");
        }

        Response = new InvitationResponse(newStatus, DateTime.UtcNow);
    }

    public bool HasResponded => Response != null;
    public bool IsAccepted => Response?.Status == InvitationStatus.Accepted;
    public bool IsMaybe => Response?.Status == InvitationStatus.Maybe;
    public bool IsDeclined => Response?.Status == InvitationStatus.Declined;
    public bool IsPending => Response == null || Response.Status == InvitationStatus.Pending;

    public override string ToString() => $"Invitation for {AttendeeName} ({AttendeeEmail}) to Event {EventId}";
}
