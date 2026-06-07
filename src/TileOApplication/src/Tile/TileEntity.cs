using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Tile;

public class TileEntity : Entity
{
    public Position Position { get; private set; }
    // BR-004: At most one connection per side (North, South, East, West)
    private readonly Dictionary<Direction, Connection> _connections;

    internal TileEntity(Guid id, Position position, TileState state) : base(id)
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        State = state ?? throw new ArgumentNullException(nameof(state));
        _connections = InitConnections();
    }

    public TileEntity(Position position, TileState state) : base()
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        State = state ?? throw new ArgumentNullException(nameof(state));
        _connections = InitConnections();
    }

    // BR-004: Four sides, each can hold at most one connection
    private static Dictionary<Direction, Connection> InitConnections() => new()
    {
        { Direction.North, new Connection(Direction.North) },
        { Direction.South, new Connection(Direction.South) },
        { Direction.East, new Connection(Direction.East) },
        { Direction.West, new Connection(Direction.West) }
    };

    internal IReadOnlyDictionary<Direction, Connection> Connections => _connections.AsReadOnly();

    // BR-005: Connect via connection piece
    internal void ConnectTo(Direction direction)
    {
        if (_connections.ContainsKey(direction))
            _connections[direction] = _connections[direction].Connect();
    }

    // BR-024: Disconnect and return piece to spare pile
    internal void DisconnectFrom(Direction direction)
    {
        if (_connections.ContainsKey(direction))
            _connections[direction] = _connections[direction].Disconnect();
    }

    // BR-016: Tile color changes from white to black (visited)
    internal void MarkAsVisited()
    {
        State = State.MarkAsVisited();
    }

    // BR-019: Action tile converts to regular tile
    internal void ConvertToRegular(int actionTurnsRemaining)
    {
        State = State.ConvertToRegular(actionTurnsRemaining);
    }

    // BR-019: Decrement cooldown turns
    internal void DecrementActionTurns()
    {
        State = State.DecrementActionTurns();
    }

    internal TileState State { get; private set; }
    internal bool IsHiddenTile => State.Type == TileType.Hidden;
    internal bool IsActionTile => State.Type == TileType.Action;
    // BR-016: Visited state
    public bool IsVisited => State.IsVisited;
}
