using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Person;

public class HeadCoachRole : UserRole
{
    public HeadCoachRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public HeadCoachRole(Guid personId) : base(personId)
    {
    }
}
