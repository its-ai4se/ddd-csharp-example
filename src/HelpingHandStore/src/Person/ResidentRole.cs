using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Person;

public class ResidentRole : UserRole
{
    public ResidentRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public ResidentRole(Guid personId) : base(personId)
    {
    }
}
