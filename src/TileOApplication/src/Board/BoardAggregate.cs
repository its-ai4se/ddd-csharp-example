using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;
using TileOApplication.Domain.Tile;

namespace TileOApplication.Domain.Board;

public class BoardAggregate : AggregateRoot
{
    private readonly Dictionary<Position, TileEntity> _tiles;
    private readonly Dictionary<Position, Guid> _startingPositions;
    private readonly Dictionary<Position, int> _originalActionTileDurations;
    private Position? _hiddenTilePosition;
    private int _spareConnectionPieces;

    public BoardAggregate() : base()
    {
        _tiles = new Dictionary<Position, TileEntity>();
        _startingPositions = new Dictionary<Position, Guid>();
        _originalActionTileDurations = new Dictionary<Position, int>();
        _spareConnectionPieces = 32; 
    }

    internal IReadOnlyDictionary<Position, TileEntity> Tiles => _tiles.AsReadOnly();
    public IReadOnlyDictionary<Position, Guid> StartingPositions => _startingPositions.AsReadOnly();
    public Position? HiddenTilePosition => _hiddenTilePosition;
    public int SpareConnectionPieces => _spareConnectionPieces;

    public void AddTile(TileEntity tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        _tiles[tile.Position] = tile;
    }

    internal TileEntity? GetTileAt(Position position)
    {
        return _tiles.TryGetValue(position, out var tile) ? tile : null;
    }

    public void SetHiddenTile(Position position)
    {
        if (!_tiles.ContainsKey(position))
            throw new ArgumentException("Tile does not exist at the specified position.", nameof(position));
        if (_hiddenTilePosition is not null)
            throw new InvalidOperationException("A hidden tile has already been designated.");

        var tile = _tiles[position];
        _tiles[position] = new TileEntity(tile.Id, tile.Position, new TileState(TileType.Hidden, tile.State.IsVisited));
        _hiddenTilePosition = position;
    }

    public void SetActionTile(Position position, int inactiveTurns)
    {
        if (!_tiles.ContainsKey(position))
            throw new ArgumentException("Tile does not exist at the specified position.", nameof(position));
        if (inactiveTurns <= 0)
            throw new ArgumentOutOfRangeException(nameof(inactiveTurns), "Inactive turns must be greater than zero.");

        var tile = _tiles[position];
        _tiles[position] = new TileEntity(tile.Id, tile.Position, new TileState(TileType.Action, tile.State.IsVisited, inactiveTurns));
        _originalActionTileDurations[position] = inactiveTurns;
    }

    public void SetStartingPosition(Position position, Guid playerId)
    {
        if (!_tiles.ContainsKey(position))
            throw new ArgumentException("Tile does not exist at the specified position.", nameof(position));
        _startingPositions[position] = playerId;
    }

    public void ConnectTiles(Position fromPosition, Position toPosition)
    {
        if (!_tiles.ContainsKey(fromPosition) || !_tiles.ContainsKey(toPosition))
            throw new ArgumentException("One or both tiles do not exist.");

        var direction = GetDirection(fromPosition, toPosition)
            ?? throw new ArgumentException("Tiles are not adjacent.");

        if (_spareConnectionPieces <= 0)
            throw new InvalidOperationException("No spare connection pieces available.");

        _tiles[fromPosition].ConnectTo(direction);
        _tiles[toPosition].ConnectTo(GetOppositeDirection(direction));
        _spareConnectionPieces--;
    }

    public void DisconnectTiles(Position fromPosition, Position toPosition)
    {
        if (!_tiles.ContainsKey(fromPosition) || !_tiles.ContainsKey(toPosition))
            throw new ArgumentException("One or both tiles do not exist.");

        var direction = GetDirection(fromPosition, toPosition)
            ?? throw new ArgumentException("Tiles are not adjacent.");

        _tiles[fromPosition].DisconnectFrom(direction);
        _tiles[toPosition].DisconnectFrom(GetOppositeDirection(direction));

        // BR-025: removed piece returns to spare pile; BR-013: cannot exceed 32
        if (_spareConnectionPieces >= 32)
            throw new InvalidOperationException("Spare connection piece pool is already at maximum (32).");
        _spareConnectionPieces++;
    }

    public void TickActionTiles()
    {
        foreach (var position in _originalActionTileDurations.Keys.ToList())
        {
            var tile = _tiles[position];
            // Only tick tiles that are currently inactive (Regular with remaining turns > 0)
            if (tile.State.Type != TileType.Regular || tile.State.ActionTileTurnsRemaining <= 0)
                continue;

            tile.DecrementActionTurns();

            if (tile.State.ActionTileTurnsRemaining == 0)
            {
                _tiles[position] = new TileEntity(tile.Id, tile.Position,
                    new TileState(TileType.Action, tile.State.IsVisited, _originalActionTileDurations[position]));
            }
        }
    }

    public TileView? GetTileView(Position position)
    {
        var tile = GetTileAt(position);
        if (tile == null) return null;
        var displayType = tile.State.IsVisited ? TileDisplayType.Visited : TileDisplayType.Regular;
        return new TileView(position, displayType);
    }

    public List<Position> GetConnectedPositions(Position position)
    {
        var connectedPositions = new List<Position>();
        var tile = GetTileAt(position);
        if (tile == null) return connectedPositions;

        foreach (var connection in tile.Connections)
        {
            if (connection.Value.IsConnected)
            {
                var connectedPosition = GetPositionInDirection(position, connection.Key);
                if (_tiles.ContainsKey(connectedPosition))
                    connectedPositions.Add(connectedPosition);
            }
        }

        return connectedPositions;
    }

    private Direction? GetDirection(Position from, Position to)
    {
        var diff = to - from;
        if (diff.X == 1 && diff.Y == 0) return Direction.East;
        if (diff.X == -1 && diff.Y == 0) return Direction.West;
        if (diff.X == 0 && diff.Y == 1) return Direction.South;
        if (diff.X == 0 && diff.Y == -1) return Direction.North;
        return null;
    }

    private static Direction GetOppositeDirection(Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        _ => throw new ArgumentException("Invalid direction")
    };

    private static Position GetPositionInDirection(Position position, Direction direction) => direction switch
    {
        Direction.North => new Position(position.X, position.Y - 1),
        Direction.South => new Position(position.X, position.Y + 1),
        Direction.East => new Position(position.X + 1, position.Y),
        Direction.West => new Position(position.X - 1, position.Y),
        _ => throw new ArgumentException("Invalid direction")
    };
}
