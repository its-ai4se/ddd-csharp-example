namespace TeamSportsScoutingSystem.Domain.Shared.Common;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() : base() { }
}
