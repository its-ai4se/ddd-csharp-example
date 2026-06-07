using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

public class GameAggregate : AggregateRoot
{
    // BR-008: each game must have a unique name
    public GameName Name { get; private set; }
    // BR-007: there is only one admin per game
    public Guid AdminId { get; private set; }
    // BR-021: a player can only play a game after it has been published
    public bool IsPublished { get; private set; }
    // BR-019: ball speed starts at minimum and increases each level
    public Speed MinimumSpeed { get; private set; }
    public double SpeedIncreaseFactor { get; private set; }
    // BR-020: paddle starts at maximum length and decreases to minimum across levels
    public PaddleLength MaximumPaddleLength { get; private set; }
    public PaddleLength MinimumPaddleLength { get; private set; }
    // BR-018: number of blocks shown at the beginning of each level is fixed and admin-defined
    public int BlocksPerLevel { get; private set; }

    private readonly List<BlockType> _blockTypes = [];
    private readonly List<Level> _levels = [];

    public GameAggregate(Guid id, GameName name, Guid adminId, Speed minimumSpeed,
        double speedIncreaseFactor, PaddleLength maximumPaddleLength,
        PaddleLength minimumPaddleLength, int blocksPerLevel) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AdminId = adminId;
        MinimumSpeed = minimumSpeed ?? throw new ArgumentNullException(nameof(minimumSpeed));
        SpeedIncreaseFactor = speedIncreaseFactor;
        MaximumPaddleLength = maximumPaddleLength ?? throw new ArgumentNullException(nameof(maximumPaddleLength));
        MinimumPaddleLength = minimumPaddleLength ?? throw new ArgumentNullException(nameof(minimumPaddleLength));
        BlocksPerLevel = blocksPerLevel;
        IsPublished = false;
        ValidateGameConfiguration();
    }

    public GameAggregate(GameName name, Guid adminId, Speed minimumSpeed,
        double speedIncreaseFactor, PaddleLength maximumPaddleLength,
        PaddleLength minimumPaddleLength, int blocksPerLevel) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AdminId = adminId;
        MinimumSpeed = minimumSpeed ?? throw new ArgumentNullException(nameof(minimumSpeed));
        SpeedIncreaseFactor = speedIncreaseFactor;
        MaximumPaddleLength = maximumPaddleLength ?? throw new ArgumentNullException(nameof(maximumPaddleLength));
        MinimumPaddleLength = minimumPaddleLength ?? throw new ArgumentNullException(nameof(minimumPaddleLength));
        BlocksPerLevel = blocksPerLevel;
        IsPublished = false;
        ValidateGameConfiguration();
    }

    public IReadOnlyList<BlockType> BlockTypes => _blockTypes.AsReadOnly();
    public IReadOnlyList<Level> Levels => _levels.AsReadOnly();

    private void ValidateGameConfiguration()
    {
        if (SpeedIncreaseFactor < 0)
            throw new ArgumentException("Speed increase factor cannot be negative.", nameof(SpeedIncreaseFactor));

        if (MaximumPaddleLength.Value <= MinimumPaddleLength.Value)
            throw new ArgumentException("Maximum paddle length must be greater than minimum paddle length.");

        if (BlocksPerLevel <= 0)
            throw new ArgumentException("Blocks per level must be positive.", nameof(BlocksPerLevel));
    }

    // BR-010: admin defines a set of blocks for the game; each block has a color and point value
    public void AddBlockType(BlockType blockType)
    {
        ArgumentNullException.ThrowIfNull(blockType);
        if (IsPublished) throw new InvalidOperationException("Cannot modify published game.");
        if (_blockTypes.Any(bt => bt.Color.Equals(blockType.Color)))
            throw new InvalidOperationException($"Block type with color {blockType.Color} already exists.");

        _blockTypes.Add(blockType);
    }

    // BR-013: levels are numbered starting at 1; BR-014: maximum 99 levels
    public void AddLevel(Level level)
    {
        ArgumentNullException.ThrowIfNull(level);
        if (IsPublished) throw new InvalidOperationException("Cannot modify published game.");
        if (_levels.Any(l => l.LevelNumber.Equals(level.LevelNumber)))
            throw new InvalidOperationException($"Level {level.LevelNumber} already exists.");
        if (_levels.Count >= 99)
            throw new InvalidOperationException("Maximum number of levels (99) reached.");

        _levels.Add(level);
    }

    // BR-012: a game must have at least one level before it can be published
    // BR-018: each non-random level must have exactly BlocksPerLevel block placements
    public void Publish()
    {
        if (_blockTypes.Count == 0)
            throw new InvalidOperationException("Cannot publish game without block types.");
        if (_levels.Count == 0)
            throw new InvalidOperationException("Cannot publish game without levels.");

        foreach (var level in _levels)
        {
            if (!level.IsRandom && level.BlockPlacements.Count != BlocksPerLevel)
                throw new InvalidOperationException(
                    $"Level {level.LevelNumber} must have exactly {BlocksPerLevel} block placements, but has {level.BlockPlacements.Count}.");
        }

        IsPublished = true;
    }

    public Level? GetLevel(LevelNumber levelNumber)
        => _levels.FirstOrDefault(l => l.LevelNumber.Equals(levelNumber));

    // BR-019: speed starts at minimum and increases by factor per level
    public Speed GetSpeedForLevel(LevelNumber levelNumber)
    {
        // speed = minSpeed * (1 + factor * levelIndex)
        // level=1 → index=0 → speed=minSpeed; level=2 → index=1 → speed=minSpeed*(1+factor)
        var levelIndex = levelNumber.Value - 1;
        return new Speed(MinimumSpeed.Value * (1 + SpeedIncreaseFactor * levelIndex));
    }

    // BR-020: paddle starts at maximum length and reduces gradually to minimum across levels
    public PaddleLength GetPaddleLengthForLevel(LevelNumber levelNumber)
    {
        if (_levels.Count <= 1)
            return MaximumPaddleLength;

        var levelIndex = levelNumber.Value - 1;
        var totalLevels = _levels.Count - 1;
        var reductionPerLevel = Math.Round((MaximumPaddleLength.Value - MinimumPaddleLength.Value) / totalLevels);
        var reduced = MaximumPaddleLength.Value - reductionPerLevel * levelIndex;
        var clamped = Math.Max(reduced, MinimumPaddleLength.Value);
        return new PaddleLength(clamped);
    }

    public override string ToString() => $"Game: {Name} (ID: {Id})";
}
