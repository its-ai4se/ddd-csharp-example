namespace TeamSportsScoutingSystem.Domain.Person;

public class DirectorRole : UserRole
{
    public DirectorRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public DirectorRole(Guid personId) : base(personId)
    {
    }
}
