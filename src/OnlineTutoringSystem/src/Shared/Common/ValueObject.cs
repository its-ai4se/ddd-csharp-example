namespace OnlineTutoringSystem.Domain.Shared.Common;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;
        return GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());
    }

    public override int GetHashCode()
        => GetEqualityComponents().Select(x => x?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);

    public static bool operator ==(ValueObject one, ValueObject two)
    {
        if (ReferenceEquals(one, null) ^ ReferenceEquals(two, null)) return false;
        return ReferenceEquals(one, two) || one!.Equals(two);
    }

    public static bool operator !=(ValueObject one, ValueObject two) => !(one == two);
}
