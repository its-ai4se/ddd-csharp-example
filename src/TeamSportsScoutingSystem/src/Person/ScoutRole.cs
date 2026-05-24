namespace TeamSportsScoutingSystem.Domain.Person;

public class ScoutRole : UserRole
{
    public bool IsHeadScout { get; private set; }

    public ScoutRole(Guid id, Guid personId, bool isHeadScout = false) : base(id, personId)
    {
        IsHeadScout = isHeadScout;
    }

    public ScoutRole(Guid personId, bool isHeadScout = false) : base(personId)
    {
        IsHeadScout = isHeadScout;
    }

    public void PromoteToHeadScout()
    {
        IsHeadScout = true;
    }
}
