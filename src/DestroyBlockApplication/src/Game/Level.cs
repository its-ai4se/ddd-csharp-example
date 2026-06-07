using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

public class Level : Entity
{
    public LevelNumber LevelNumber { get; }
    // BR-017: admin may designate a level as random; blocks at the top are randomly selected from block set
    public bool IsRandom { get; }
    private readonly List<BlockPlacement> _blockPlacements = new();

    public Level(Guid id, LevelNumber levelNumber, bool isRandom = false) : base(id)
    {
        LevelNumber = levelNumber ?? throw new ArgumentNullException(nameof(levelNumber));
        IsRandom = isRandom;
    }

    public Level(LevelNumber levelNumber, bool isRandom = false) : base()
    {
        LevelNumber = levelNumber ?? throw new ArgumentNullException(nameof(levelNumber));
        IsRandom = isRandom;
    }

    public IReadOnlyList<BlockPlacement> BlockPlacements => _blockPlacements.AsReadOnly();

    // BR-015: admin specifies the starting arrangement of blocks on a grid for each level
    public void AddBlockPlacement(BlockPlacement placement)
    {
        ArgumentNullException.ThrowIfNull(placement);

        if (_blockPlacements.Any(bp => bp.Position.Equals(placement.Position)))
        {
            throw new InvalidOperationException($"Block already placed at position {placement.Position}.");
        }

        _blockPlacements.Add(placement);
    }

    public override string ToString() => $"Level {LevelNumber} (Random: {IsRandom})";
}
