using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Person;

public class OrganizerRole : UserRole
{
    public OrganizerRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public OrganizerRole(Guid personId) : base(personId)
    {
    }

    public override string ToString() => $"Organizer Role for Person {PersonId}";
}
