using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Player;

public class PlayerAggregate : AggregateRoot
{
    // BR-003: Unique color to distinguish players
    public PlayerColor Color { get; private set; }
    public Position? CurrentPosition { get; private set; }
    // BR-013: Turn order determines sequential play order
    public int TurnOrder { get; private set; }

    public PlayerAggregate(PlayerColor color, int turnOrder) : base()
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        TurnOrder = turnOrder;
    }

    // BR-007: Placed at designer-defined starting position
    public void PlaceAtStartingPosition(Position position)
    {
        CurrentPosition = position ?? throw new ArgumentNullException(nameof(position));
    }

    // BR-015: Move along connected tiles
    public void MoveTo(Position newPosition)
    {
        CurrentPosition = newPosition ?? throw new ArgumentNullException(nameof(newPosition));
    }

    public override string ToString() => $"Player({Color})";
}
