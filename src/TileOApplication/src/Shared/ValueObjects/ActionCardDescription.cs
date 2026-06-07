using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Shared.ValueObjects;

// BR-011: Five predefined action card types
public enum ActionCardType
{
    ExtraTurn,       // BR-021: Roll dice for extra turn
    ConnectTiles,    // BR-023: Connect two adjacent tiles from spare pile
    RemoveConnection,// BR-024: Remove connection piece to spare pile
    Teleport,        // BR-022: Move to any tile other than current
    SkipTurn         // BR-025: Lose the next turn
}

// BR-011: Action cards chosen from five predefined types only
public class ActionCardDescription : ValueObject
{
    public ActionCardType Type { get; }

    private ActionCardDescription(ActionCardType type)
    {
        Type = type;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
    }

    public override string ToString() => Type.ToString();

    public static readonly ActionCardDescription ExtraTurn = new(ActionCardType.ExtraTurn);
    public static readonly ActionCardDescription ConnectTiles = new(ActionCardType.ConnectTiles);
    public static readonly ActionCardDescription RemoveConnection = new(ActionCardType.RemoveConnection);
    public static readonly ActionCardDescription Teleport = new(ActionCardType.Teleport);
    public static readonly ActionCardDescription SkipTurn = new(ActionCardType.SkipTurn);
}
