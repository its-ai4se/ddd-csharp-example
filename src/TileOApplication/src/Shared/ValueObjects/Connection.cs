using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

public enum Direction
{
    North,
    South,
    East,
    West
}

public class Connection : ValueObject
{
    public Direction Direction { get; }
    public bool IsConnected { get; }

    internal Connection(Direction direction, bool isConnected = false)
    {
        Direction = direction;
        IsConnected = isConnected;
    }

    internal Connection Connect() => new Connection(Direction, true);
    internal Connection Disconnect() => new Connection(Direction, false);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Direction;
        yield return IsConnected;
    }
}
