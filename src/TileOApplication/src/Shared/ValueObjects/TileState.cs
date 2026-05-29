using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

public enum TileType
{
    Regular,
    Action,
    Hidden
}

public class TileState : ValueObject
{
    public TileType Type { get; }
    public bool IsVisited { get; }
    public int ActionTileTurnsRemaining { get; }

    public TileState(TileType type, bool isVisited = false, int actionTileTurnsRemaining = 0)
    {
        Type = type;
        IsVisited = isVisited;
        ActionTileTurnsRemaining = actionTileTurnsRemaining;
    }

    public TileState MarkAsVisited()
    {
        return new TileState(Type, true, ActionTileTurnsRemaining);
    }

    public TileState ConvertToRegular(int turnsRemaining)
    {
        return new TileState(TileType.Regular, IsVisited, turnsRemaining);
    }

    public TileState DecrementActionTurns()
    {
        if (ActionTileTurnsRemaining <= 0)
            return this;
        
        return new TileState(Type, IsVisited, ActionTileTurnsRemaining - 1);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return IsVisited;
        yield return ActionTileTurnsRemaining;
    }
}
