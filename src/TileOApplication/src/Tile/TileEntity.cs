using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Tile;

public class TileEntity : Entity
{
    public Position Position { get; private set; }
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

    private static Dictionary<Direction, Connection> InitConnections() => new()
    {
        { Direction.North, new Connection(Direction.North) },
        { Direction.South, new Connection(Direction.South) },
        { Direction.East, new Connection(Direction.East) },
        { Direction.West, new Connection(Direction.West) }
    };

    internal IReadOnlyDictionary<Direction, Connection> Connections => _connections.AsReadOnly();

    internal void ConnectTo(Direction direction)
    {
        if (_connections.ContainsKey(direction))
            _connections[direction] = _connections[direction].Connect();
    }

    internal void DisconnectFrom(Direction direction)
    {
        if (_connections.ContainsKey(direction))
            _connections[direction] = _connections[direction].Disconnect();
    }

    internal void MarkAsVisited()
    {
        State = State.MarkAsVisited();
    }

    internal void ConvertToRegular(int actionTurnsRemaining)
    {
        State = State.ConvertToRegular(actionTurnsRemaining);
    }

    internal void DecrementActionTurns()
    {
        State = State.DecrementActionTurns();
    }

    internal TileState State { get; private set; }
    internal bool IsHiddenTile => State.Type == TileType.Hidden;
    internal bool IsActionTile => State.Type == TileType.Action;
    public bool IsVisited => State.IsVisited;
}
