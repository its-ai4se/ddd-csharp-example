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

    public Connection(Direction direction, bool isConnected = false)
    {
        Direction = direction;
        IsConnected = isConnected;
    }

    public Connection Connect()
    {
        return new Connection(Direction, true);
    }

    public Connection Disconnect()
    {
        return new Connection(Direction, false);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Direction;
        yield return IsConnected;
    }

    public override string ToString() => $"{Direction}: {(IsConnected ? "Connected" : "Disconnected")}";
}
