using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

// BR-016: Tracks visited state (white → black); BR-019: Tracks action tile cooldown
public enum TileType
{
    Regular,
    Action,
    Hidden
}

public class TileState : ValueObject
{
    public TileType Type { get; }
    // BR-016: Whether tile has been visited (color changed to black)
    public bool IsVisited { get; }
    // BR-019: Remaining turns before action tile reactivates
    public int ActionTileTurnsRemaining { get; }

    public TileState(TileType type, bool isVisited = false, int actionTileTurnsRemaining = 0)
    {
        Type = type;
        IsVisited = isVisited;
        ActionTileTurnsRemaining = actionTileTurnsRemaining;
    }

    // BR-016: Mark tile as visited
    public TileState MarkAsVisited()
    {
        return new TileState(Type, true, ActionTileTurnsRemaining);
    }

    // BR-019: Convert action tile to regular with cooldown
    public TileState ConvertToRegular(int turnsRemaining)
    {
        return new TileState(TileType.Regular, IsVisited, turnsRemaining);
    }

    // BR-019: Decrement action tile cooldown
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
