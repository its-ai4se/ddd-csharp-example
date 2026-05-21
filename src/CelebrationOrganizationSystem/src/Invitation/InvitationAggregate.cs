using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Invitation;

public class InvitationAggregate : AggregateRoot
{
    public Guid EventId { get; private set; }
    public Guid? AttendeeId { get; private set; }
    public EmailAddress AttendeeEmail { get; private set; }
    public PersonName AttendeeName { get; private set; }
    public InvitationResponse? Response { get; private set; }

    public InvitationAggregate(Guid id, Guid eventId, EmailAddress attendeeEmail, PersonName attendeeName) : base(id)
    {
        EventId = ValidateEventId(eventId);
        AttendeeEmail = attendeeEmail ?? throw new ArgumentNullException(nameof(attendeeEmail));
        AttendeeName = attendeeName ?? throw new ArgumentNullException(nameof(attendeeName));
    }

    public InvitationAggregate(Guid eventId, EmailAddress attendeeEmail, PersonName attendeeName) : base()
    {
        EventId = ValidateEventId(eventId);
        AttendeeEmail = attendeeEmail ?? throw new ArgumentNullException(nameof(attendeeEmail));
        AttendeeName = attendeeName ?? throw new ArgumentNullException(nameof(attendeeName));
    }

    public void LinkToAttendee(Guid attendeeId)
    {
        if (attendeeId == Guid.Empty)
        {
            throw new ArgumentException("Attendee ID cannot be empty.", nameof(attendeeId));
        }

        if (AttendeeId.HasValue && AttendeeId.Value != attendeeId)
        {
            throw new InvalidOperationException("Invitation is already linked to a different attendee account.");
        }

        AttendeeId = attendeeId;
    }

    public void RespondToInvitation(InvitationStatus status)
    {
        if (Response is not null)
        {
            throw new InvalidOperationException("Invitation has already been responded to.");
        }

        Response = new InvitationResponse(status);
    }

    public bool HasResponded => Response is not null;
    public bool IsWillAttend => Response?.Status == InvitationStatus.WillAttend;
    public bool IsMaybeWillAttend => Response?.Status == InvitationStatus.MaybeWillAttend;
    public bool IsCannotAttend => Response?.Status == InvitationStatus.CannotAttend;
    public bool IsUnreplied => Response is null;

    private static Guid ValidateEventId(Guid eventId)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));
        }

        return eventId;
    }

    public override string ToString() => $"Invitation for {AttendeeName} ({AttendeeEmail}) to Event {EventId}";
}
