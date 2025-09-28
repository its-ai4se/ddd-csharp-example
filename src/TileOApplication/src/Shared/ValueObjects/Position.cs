using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

public class Position : ValueObject
{
    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }

    public override string ToString() => $"({X}, {Y})";

    public static Position operator +(Position left, Position right)
    {
        return new Position(left.X + right.X, left.Y + right.Y);
    }

    public static Position operator -(Position left, Position right)
    {
        return new Position(left.X - right.X, left.Y - right.Y);
    }
}
