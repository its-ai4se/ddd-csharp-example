using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Event;

public class EventOrganizer : ValueObject
{
    public Guid OrganizerId { get; }
    public bool IsAttending { get; private set; }

    public EventOrganizer(Guid organizerId, bool isAttending)
    {
        if (organizerId == Guid.Empty)
        {
            throw new ArgumentException("Organizer ID cannot be empty.", nameof(organizerId));
        }

        OrganizerId = organizerId;
        IsAttending = isAttending;
    }

    public void SetAttendance(bool isAttending)
    {
        IsAttending = isAttending;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return OrganizerId;
    }
}
