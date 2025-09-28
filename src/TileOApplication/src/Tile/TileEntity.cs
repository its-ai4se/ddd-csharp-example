using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Tile;

public class TileEntity : Entity
{
    public Position Position { get; private set; }
    public TileState State { get; private set; }
    private readonly Dictionary<Direction, Connection> _connections;

    public TileEntity(Guid id, Position position, TileState state) : base(id)
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        State = state ?? throw new ArgumentNullException(nameof(state));
        _connections = new Dictionary<Direction, Connection>
        {
            { Direction.North, new Connection(Direction.North) },
            { Direction.South, new Connection(Direction.South) },
            { Direction.East, new Connection(Direction.East) },
            { Direction.West, new Connection(Direction.West) }
        };
    }

    public TileEntity(Position position, TileState state) : base()
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        State = state ?? throw new ArgumentNullException(nameof(state));
        _connections = new Dictionary<Direction, Connection>
        {
            { Direction.North, new Connection(Direction.North) },
            { Direction.South, new Connection(Direction.South) },
            { Direction.East, new Connection(Direction.East) },
            { Direction.West, new Connection(Direction.West) }
        };
    }

    public IReadOnlyDictionary<Direction, Connection> Connections => _connections.AsReadOnly();

    public void ConnectTo(Direction direction)
    {
        if (_connections.ContainsKey(direction))
        {
            _connections[direction] = _connections[direction].Connect();
        }
    }

    public void DisconnectFrom(Direction direction)
    {
        if (_connections.ContainsKey(direction))
        {
            _connections[direction] = _connections[direction].Disconnect();
        }
    }

    public bool IsConnectedTo(Direction direction)
    {
        return _connections.ContainsKey(direction) && _connections[direction].IsConnected;
    }

    public void MarkAsVisited()
    {
        State = State.MarkAsVisited();
    }

    public void ConvertToRegular(int actionTurnsRemaining)
    {
        State = State.ConvertToRegular(actionTurnsRemaining);
    }

    public void DecrementActionTurns()
    {
        State = State.DecrementActionTurns();
    }

    public bool IsHiddenTile => State.Type == TileType.Hidden;
    public bool IsActionTile => State.Type == TileType.Action;
    public bool IsStartingTile => State.Type == TileType.Starting;
    public bool IsVisited => State.IsVisited;
}
