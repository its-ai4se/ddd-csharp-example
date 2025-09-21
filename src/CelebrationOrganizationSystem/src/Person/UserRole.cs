using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Person;

public abstract class UserRole : Entity
{
    public Guid PersonId { get; }

    protected UserRole(Guid id, Guid personId) : base(id)
    {
        PersonId = personId;
    }

    protected UserRole(Guid personId) : base()
    {
        PersonId = personId;
    }
}
