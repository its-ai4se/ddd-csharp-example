namespace BusTransportManagementSystem.Domain.Shared.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity ID cannot be empty.", nameof(id));
        }
        Id = id;
    }

    protected Entity() : this(Guid.NewGuid())
    {
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity left, Entity right) => Equals(left, right);

    public static bool operator !=(Entity left, Entity right) => !Equals(left, right);
}
