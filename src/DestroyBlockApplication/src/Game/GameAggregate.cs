using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Game;

public class GameAggregate : AggregateRoot
{
    public GameName Name { get; private set; }
    public Guid AdminId { get; private set; }
    public bool IsPublished { get; private set; }
    public Speed MinimumSpeed { get; private set; }
    public double SpeedIncreaseFactor { get; private set; }
    public PaddleLength MaximumPaddleLength { get; private set; }
    public PaddleLength MinimumPaddleLength { get; private set; }
    public int BlocksPerLevel { get; private set; }

    private readonly List<BlockType> _blockTypes = new();
    private readonly List<Level> _levels = new();

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
        if (SpeedIncreaseFactor <= 1.0)
        {
            throw new ArgumentException("Speed increase factor must be greater than 1.0.", nameof(SpeedIncreaseFactor));
        }

        if (MaximumPaddleLength.Value <= MinimumPaddleLength.Value)
        {
            throw new ArgumentException("Maximum paddle length must be greater than minimum paddle length.");
        }

        if (BlocksPerLevel <= 0)
        {
            throw new ArgumentException("Blocks per level must be positive.", nameof(BlocksPerLevel));
        }
    }

    public void AddBlockType(BlockType blockType)
    {
        if (blockType == null)
        {
            throw new ArgumentNullException(nameof(blockType));
        }

        if (IsPublished)
        {
            throw new InvalidOperationException("Cannot modify published game.");
        }

        if (_blockTypes.Any(bt => bt.Color.Equals(blockType.Color)))
        {
            throw new InvalidOperationException($"Block type with color {blockType.Color} already exists.");
        }

        _blockTypes.Add(blockType);
    }

    public void RemoveBlockType(Color color)
    {
        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        if (IsPublished)
        {
            throw new InvalidOperationException("Cannot modify published game.");
        }

        var blockTypeToRemove = _blockTypes.FirstOrDefault(bt => bt.Color.Equals(color));
        if (blockTypeToRemove != null)
        {
            _blockTypes.Remove(blockTypeToRemove);
        }
    }

    public void AddLevel(Level level)
    {
        if (level == null)
        {
            throw new ArgumentNullException(nameof(level));
        }

        if (IsPublished)
        {
            throw new InvalidOperationException("Cannot modify published game.");
        }

        if (_levels.Any(l => l.LevelNumber.Equals(level.LevelNumber)))
        {
            throw new InvalidOperationException($"Level {level.LevelNumber} already exists.");
        }

        if (_levels.Count >= 99)
        {
            throw new InvalidOperationException("Maximum number of levels (99) reached.");
        }

        _levels.Add(level);
    }

    public void RemoveLevel(LevelNumber levelNumber)
    {
        if (levelNumber == null)
        {
            throw new ArgumentNullException(nameof(levelNumber));
        }

        if (IsPublished)
        {
            throw new InvalidOperationException("Cannot modify published game.");
        }

        var levelToRemove = _levels.FirstOrDefault(l => l.LevelNumber.Equals(levelNumber));
        if (levelToRemove != null)
        {
            _levels.Remove(levelToRemove);
        }
    }

    public void Publish()
    {
        if (_blockTypes.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish game without block types.");
        }

        if (_levels.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish game without levels.");
        }

        IsPublished = true;
    }

    public void Unpublish()
    {
        IsPublished = false;
    }

    public Level? GetLevel(LevelNumber levelNumber)
    {
        return _levels.FirstOrDefault(l => l.LevelNumber.Equals(levelNumber));
    }

    public BlockType? GetBlockType(Color color)
    {
        return _blockTypes.FirstOrDefault(bt => bt.Color.Equals(color));
    }

    public Speed GetSpeedForLevel(LevelNumber levelNumber)
    {
        var levelIndex = levelNumber.Value - 1;
        return new Speed(MinimumSpeed.Value * Math.Pow(SpeedIncreaseFactor, levelIndex));
    }

    public PaddleLength GetPaddleLengthForLevel(LevelNumber levelNumber)
    {
        if (_levels.Count <= 1)
        {
            return MaximumPaddleLength;
        }

        var levelIndex = levelNumber.Value - 1;
        var totalLevels = _levels.Count - 1;
        var reductionPerLevel = (MaximumPaddleLength.Value - MinimumPaddleLength.Value) / totalLevels;
        var totalReduction = reductionPerLevel * levelIndex;

        return MaximumPaddleLength - totalReduction;
    }

    public override string ToString() => $"Game: {Name} (ID: {Id})";
}
