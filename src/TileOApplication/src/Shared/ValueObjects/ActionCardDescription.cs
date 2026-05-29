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
