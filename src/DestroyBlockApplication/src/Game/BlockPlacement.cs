using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

// BR-015: admin specifies the starting arrangement of blocks on a grid for each level
// BR-016: each block occupies exactly one cell of the grid system
public class BlockPlacement : Entity
{
    // BR-016: grid positions start at 1/1 in the top-left corner
    public GridPosition Position { get; }
    // BR-010: references the block type (color + point value)
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
