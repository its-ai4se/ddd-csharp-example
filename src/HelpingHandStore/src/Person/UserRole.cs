using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Person;

public abstract class UserRole : Entity
{
    public Guid PersonId { get; protected set; }

    protected UserRole(Guid id, Guid personId) : base(id)
    {
        PersonId = personId;
    }

    protected UserRole(Guid personId) : base()
    {
        PersonId = personId;
    }
}
