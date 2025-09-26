using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Person;

public abstract class UserRole : Entity
{
    public Guid PersonId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected UserRole(Guid id, Guid personId) : base(id)
    {
        PersonId = personId;
        CreatedAt = DateTime.UtcNow;
    }

    protected UserRole(Guid personId) : base()
    {
        PersonId = personId;
        CreatedAt = DateTime.UtcNow;
    }
}
