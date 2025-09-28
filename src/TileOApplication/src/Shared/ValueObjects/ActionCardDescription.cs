using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

public enum ActionCardType
{
    ExtraTurn,
    ConnectTiles,
    RemoveConnection,
    Teleport,
    SkipTurn
}

public class ActionCardDescription : ValueObject
{
    public ActionCardType Type { get; }
    public string Description { get; }

    public ActionCardDescription(ActionCardType type, string description)
    {
        Type = type;
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Description;
    }

    public override string ToString() => $"{Type}: {Description}";

    public static readonly ActionCardDescription ExtraTurn = new(ActionCardType.ExtraTurn, "Roll the die for an extra turn");
    public static readonly ActionCardDescription ConnectTiles = new(ActionCardType.ConnectTiles, "Connect two adjacent tiles with a connection piece");
    public static readonly ActionCardDescription RemoveConnection = new(ActionCardType.RemoveConnection, "Remove a connection piece from the board");
    public static readonly ActionCardDescription Teleport = new(ActionCardType.Teleport, "Move your playing piece to an arbitrary tile");
    public static readonly ActionCardDescription SkipTurn = new(ActionCardType.SkipTurn, "Lose your next turn");
}
