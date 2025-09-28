using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Player;

public class PlayerAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public PlayerColor Color { get; private set; }
    public Position? CurrentPosition { get; private set; }
    public bool IsActive { get; private set; }
    public int TurnOrder { get; private set; }

    public PlayerAggregate(Guid id, string name, PlayerColor color, int turnOrder) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        TurnOrder = turnOrder;
        IsActive = true;
    }

    public PlayerAggregate(string name, PlayerColor color, int turnOrder) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        TurnOrder = turnOrder;
        IsActive = true;
    }

    public void PlaceAtStartingPosition(Position position)
    {
        CurrentPosition = position ?? throw new ArgumentNullException(nameof(position));
    }

    public void MoveTo(Position newPosition)
    {
        CurrentPosition = newPosition ?? throw new ArgumentNullException(nameof(newPosition));
    }

    public void SkipNextTurn()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Player name cannot be empty or whitespace.", nameof(newName));
        }
        Name = newName.Trim();
    }

    public void UpdateColor(PlayerColor newColor)
    {
        Color = newColor ?? throw new ArgumentNullException(nameof(newColor));
    }

    public override string ToString() => $"{Name} ({Color.Name})";
}
