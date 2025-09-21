using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Person;

public class AttendeeRole : UserRole
{
    public AttendeeRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public AttendeeRole(Guid personId) : base(personId)
    {
    }

    public override string ToString() => $"Attendee Role for Person {PersonId}";
}
