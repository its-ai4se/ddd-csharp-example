using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests;

public class GameDesignTests
{
    private static GameAggregate NewGame() => new(new GameName("G"), Guid.NewGuid(),
        new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

    [Fact]
    public void GD001_AddBlockPoints1_Succeeds()
    {
        var game = NewGame();
        game.AddBlockType(new BlockType(new Color("red"), new Score(1)));
        Assert.Single(game.BlockTypes);
    }

    [Fact]
    public void GD002_AddBlockPoints1000_Succeeds()
    {
        var game = NewGame();
        game.AddBlockType(new BlockType(new Color("blue"), new Score(1000)));
        Assert.Single(game.BlockTypes);
    }

    [Fact]
    public void GD003_AddBlockPoints500_Succeeds()
    {
        var game = NewGame();
        game.AddBlockType(new BlockType(new Color("green"), new Score(500)));
        Assert.Single(game.BlockTypes);
    }

    [Fact]
    public void GD004_AddBlockPoints0_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new BlockType(new Color("yellow"), new Score(0)));
    }

    [Fact]
    public void GD005_AddBlockPoints1001_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new BlockType(new Color("purple"), new Score(1001)));
    }

    [Fact]
    public void GD006_AddBlockNegativePoints_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Score(-5));
    }

    [Fact]
    public void GD007_AddBlockEmptyColor_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Color(""));
    }

    [Fact]
    public void GD008_CreateLevelLevel1_Succeeds()
    {
        Assert.Equal(1, new Level(new LevelNumber(1)).LevelNumber.Value);
    }

    [Fact]
    public void GD009_CreateLevelLevel99_Succeeds()
    {
        Assert.Equal(99, new Level(new LevelNumber(99)).LevelNumber.Value);
    }

    [Fact]
    public void GD010_CreateLevelLevel100_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LevelNumber(100));
    }

    [Fact]
    public void GD011_CreateLevelLevel0_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LevelNumber(0));
    }

    [Fact]
    public void GD012_AddBlockPlacements_StoredCorrectly()
    {
        var level = new Level(new LevelNumber(1));
        var id = Guid.NewGuid();
        level.AddBlockPlacement(new BlockPlacement(new GridPosition(1, 1), id));
        level.AddBlockPlacement(new BlockPlacement(new GridPosition(2, 1), id));
        Assert.Equal(2, level.BlockPlacements.Count);
    }

    [Fact]
    public void GD013_GridPosition_1_1_IsValid()
    {
        var pos = new GridPosition(1, 1);
        Assert.Equal(1, pos.X);
        Assert.Equal(1, pos.Y);
    }

    [Fact]
    public void GD014_GridPosition_2_1_IsRightOf_1_1()
    {
        var pos1 = new GridPosition(1, 1);
        var pos2 = new GridPosition(2, 1);
        Assert.True(pos2.X > pos1.X);
        Assert.Equal(pos1.Y, pos2.Y);
    }

    [Fact]
    public void GD015_GridPosition_1_2_IsBelowOf_1_1()
    {
        var pos1 = new GridPosition(1, 1);
        var pos2 = new GridPosition(1, 2);
        Assert.Equal(pos1.X, pos2.X);
        Assert.True(pos2.Y > pos1.Y);
    }

    [Fact]
    public void GD016_AddBlockPlacementDuplicatePosition_ThrowsInvalidOperationException()
    {
        var level = new Level(new LevelNumber(1));
        var id = Guid.NewGuid();
        level.AddBlockPlacement(new BlockPlacement(new GridPosition(1, 1), id));
        Assert.Throws<InvalidOperationException>(() =>
            level.AddBlockPlacement(new BlockPlacement(new GridPosition(1, 1), id)));
    }

    [Fact]
    public void GD017_LevelSetAsRandom_IsRandom()
    {
        Assert.True(new Level(new LevelNumber(2), isRandom: true).IsRandom);
    }

    [Fact]
    public void GD018_BlocksPerLevel_ConsistentAcrossLevels()
    {
        Assert.Equal(10, NewGame().BlocksPerLevel);
    }

    [Fact]
    public void GD019_BlocksPerLevelSingleValue_EnforcesConsistency()
    {
        Assert.Equal(10, NewGame().BlocksPerLevel);
    }

    [Fact]
    public void GD020_MinimumSpeed_StoredCorrectly()
    {
        Assert.Equal(2, NewGame().MinimumSpeed.Value);
    }

    [Fact]
    public void GD021_SpeedIncreaseFactor_0_1_StoredCorrectly()
    {
        Assert.Equal(0.1, NewGame().SpeedIncreaseFactor);
    }

    [Fact]
    public void GD022_SpeedForLevel_IncreasesEachLevel()
    {
        var game = NewGame();
        Assert.Equal(2.0, game.GetSpeedForLevel(new LevelNumber(1)).Value, precision: 5);
        Assert.Equal(2.2, game.GetSpeedForLevel(new LevelNumber(2)).Value, precision: 5);
    }

    [Fact]
    public void GD023_SpeedAtLevel1_EqualsMinimumSpeed()
    {
        var game = NewGame();
        Assert.Equal(game.MinimumSpeed.Value, game.GetSpeedForLevel(new LevelNumber(1)).Value, precision: 5);
    }

    [Fact]
    public void GD024_MaxPaddleLength_StoredCorrectly()
    {
        Assert.Equal(200, NewGame().MaximumPaddleLength.Value);
    }

    [Fact]
    public void GD025_MinPaddleLength_StoredCorrectly()
    {
        Assert.Equal(50, NewGame().MinimumPaddleLength.Value);
    }

    [Fact]
    public void GD026_PaddleLength_DecreasesGradually()
    {
        var game = NewGame();
        for (int i = 1; i <= 10; i++) game.AddLevel(new Level(new LevelNumber(i)));

        Assert.Equal(200, game.GetPaddleLengthForLevel(new LevelNumber(1)).Value, precision: 0);
        Assert.Equal(183, game.GetPaddleLengthForLevel(new LevelNumber(2)).Value, precision: 0);
        Assert.Equal(166, game.GetPaddleLengthForLevel(new LevelNumber(3)).Value, precision: 0);
    }

    [Fact]
    public void GD027_PaddleLengthAtLevel1_EqualsMaximum()
    {
        var game = NewGame();
        game.AddLevel(new Level(new LevelNumber(1)));
        Assert.Equal(game.MaximumPaddleLength.Value, game.GetPaddleLengthForLevel(new LevelNumber(1)).Value, precision: 5);
    }

    [Fact]
    public void GD028_PaddleLength_NeverBelowMinimum()
    {
        var game = NewGame();
        for (int i = 1; i <= 10; i++) game.AddLevel(new Level(new LevelNumber(i)));
        Assert.True(game.GetPaddleLengthForLevel(new LevelNumber(10)).Value >= game.MinimumPaddleLength.Value);
    }
}
