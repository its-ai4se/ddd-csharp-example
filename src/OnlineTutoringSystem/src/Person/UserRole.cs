using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Person;

public abstract class UserRole(Guid personId) : Entity()
{
    public Guid PersonId { get; private set; } = personId;
}
