using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Player;

public class PlayerAggregate : AggregateRoot
{
    public PlayerColor Color { get; private set; }
    public Position? CurrentPosition { get; private set; }
    public int TurnOrder { get; private set; }

    public PlayerAggregate(PlayerColor color, int turnOrder) : base()
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        TurnOrder = turnOrder;
    }

    public void PlaceAtStartingPosition(Position position)
    {
        CurrentPosition = position ?? throw new ArgumentNullException(nameof(position));
    }

    public void MoveTo(Position newPosition)
    {
        CurrentPosition = newPosition ?? throw new ArgumentNullException(nameof(newPosition));
    }

    public override string ToString() => $"Player({Color})";
}
