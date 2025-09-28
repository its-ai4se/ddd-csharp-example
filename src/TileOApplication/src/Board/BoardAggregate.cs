using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;
using TileOApplication.Domain.Tile;

namespace TileOApplication.Domain.Board;

public class BoardAggregate : AggregateRoot
{
    private readonly Dictionary<Position, TileEntity> _tiles;
    private readonly Dictionary<Position, Guid> _startingPositions;
    private Position? _hiddenTilePosition;
    private int _spareConnectionPieces;

    public BoardAggregate(Guid id) : base(id)
    {
        _tiles = new Dictionary<Position, TileEntity>();
        _startingPositions = new Dictionary<Position, Guid>();
        _spareConnectionPieces = 32;
    }

    public BoardAggregate() : base()
    {
        _tiles = new Dictionary<Position, TileEntity>();
        _startingPositions = new Dictionary<Position, Guid>();
        _spareConnectionPieces = 32;
    }

    public IReadOnlyDictionary<Position, TileEntity> Tiles => _tiles.AsReadOnly();
    public IReadOnlyDictionary<Position, Guid> StartingPositions => _startingPositions.AsReadOnly();
    public Position? HiddenTilePosition => _hiddenTilePosition;
    public int SpareConnectionPieces => _spareConnectionPieces;

    public void AddTile(TileEntity tile)
    {
        if (tile == null)
        {
            throw new ArgumentNullException(nameof(tile));
        }

        _tiles[tile.Position] = tile;
    }

    public void RemoveTile(Position position)
    {
        if (_tiles.ContainsKey(position))
        {
            _tiles.Remove(position);
        }
    }

    public TileEntity? GetTileAt(Position position)
    {
        return _tiles.TryGetValue(position, out var tile) ? tile : null;
    }

    public void SetHiddenTile(Position position)
    {
        if (!_tiles.ContainsKey(position))
        {
            throw new ArgumentException("Tile does not exist at the specified position.", nameof(position));
        }

        var tile = _tiles[position];
        var newState = new TileState(TileType.Hidden, tile.State.IsVisited, tile.State.ActionTileTurnsRemaining);
        _tiles[position] = new TileEntity(tile.Id, tile.Position, newState);
        _hiddenTilePosition = position;
    }

    public void SetStartingPosition(Position position, Guid playerId)
    {
        if (!_tiles.ContainsKey(position))
        {
            throw new ArgumentException("Tile does not exist at the specified position.", nameof(position));
        }

        _startingPositions[position] = playerId;
    }

    public void ConnectTiles(Position fromPosition, Position toPosition)
    {
        if (!_tiles.ContainsKey(fromPosition) || !_tiles.ContainsKey(toPosition))
        {
            throw new ArgumentException("One or both tiles do not exist.");
        }

        var direction = GetDirection(fromPosition, toPosition);
        if (direction == null)
        {
            throw new ArgumentException("Tiles are not adjacent.");
        }

        var fromTile = _tiles[fromPosition];
        var toTile = _tiles[toPosition];

        fromTile.ConnectTo(direction.Value);
        toTile.ConnectTo(GetOppositeDirection(direction.Value));
    }

    public void DisconnectTiles(Position fromPosition, Position toPosition)
    {
        if (!_tiles.ContainsKey(fromPosition) || !_tiles.ContainsKey(toPosition))
        {
            throw new ArgumentException("One or both tiles do not exist.");
        }

        var direction = GetDirection(fromPosition, toPosition);
        if (direction == null)
        {
            throw new ArgumentException("Tiles are not adjacent.");
        }

        var fromTile = _tiles[fromPosition];
        var toTile = _tiles[toPosition];

        fromTile.DisconnectFrom(direction.Value);
        toTile.DisconnectFrom(GetOppositeDirection(direction.Value));
    }

    public void UseConnectionPiece()
    {
        if (_spareConnectionPieces <= 0)
        {
            throw new InvalidOperationException("No spare connection pieces available.");
        }
        _spareConnectionPieces--;
    }

    public void ReturnConnectionPiece()
    {
        if (_spareConnectionPieces >= 32)
        {
            throw new InvalidOperationException("Cannot exceed maximum spare connection pieces.");
        }
        _spareConnectionPieces++;
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
                {
                    connectedPositions.Add(connectedPosition);
                }
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

    private Direction GetOppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => throw new ArgumentException("Invalid direction")
        };
    }

    private Position GetPositionInDirection(Position position, Direction direction)
    {
        return direction switch
        {
            Direction.North => new Position(position.X, position.Y - 1),
            Direction.South => new Position(position.X, position.Y + 1),
            Direction.East => new Position(position.X + 1, position.Y),
            Direction.West => new Position(position.X - 1, position.Y),
            _ => throw new ArgumentException("Invalid direction")
        };
    }
}
