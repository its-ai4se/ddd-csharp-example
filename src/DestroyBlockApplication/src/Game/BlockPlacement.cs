using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

public class BlockPlacement : Entity
{
    public GridPosition Position { get; }
    public Guid BlockTypeId { get; }

    public BlockPlacement(Guid id, GridPosition position, Guid blockTypeId) : base(id)
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        BlockTypeId = blockTypeId;
    }

    public BlockPlacement(GridPosition position, Guid blockTypeId) : base()
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        BlockTypeId = blockTypeId;
    }

    public override string ToString() => $"Block at {Position} (Type: {BlockTypeId})";
}
