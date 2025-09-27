using DestroyBlockApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests.ValueObjects;

public class ScoreTests
{
    [Fact]
    public void Constructor_ValidScore_ShouldCreateInstance()
    {
        // Arrange
        var scoreValue = 150;

        // Act
        var score = new Score(scoreValue);

        // Assert
        Assert.Equal(scoreValue, score.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NegativeValue_ShouldThrowArgumentException(int value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Score(value));
    }

    [Theory]
    [InlineData(1001)]
    [InlineData(2000)]
    public void Constructor_ValueExceeds1000_ShouldThrowArgumentException(int value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Score(value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(500)]
    [InlineData(1000)]
    public void Constructor_ValidRange_ShouldCreateInstance(int value)
    {
        // Act
        var score = new Score(value);

        // Assert
        Assert.Equal(value, score.Value);
    }

    [Fact]
    public void Addition_TwoScores_ShouldReturnSum()
    {
        // Arrange
        var score1 = new Score(100);
        var score2 = new Score(50);

        // Act
        var result = score1 + score2;

        // Assert
        Assert.Equal(150, result.Value);
    }

    [Fact]
    public void Subtraction_TwoScores_ShouldReturnDifference()
    {
        // Arrange
        var score1 = new Score(100);
        var score2 = new Score(30);

        // Act
        var result = score1 - score2;

        // Assert
        Assert.Equal(70, result.Value);
    }

    [Fact]
    public void Subtraction_ResultNegative_ShouldReturnZero()
    {
        // Arrange
        var score1 = new Score(50);
        var score2 = new Score(100);

        // Act
        var result = score1 - score2;

        // Assert
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Equals_SameValues_ShouldReturnTrue()
    {
        // Arrange
        var score1 = new Score(150);
        var score2 = new Score(150);

        // Act & Assert
        Assert.Equal(score1, score2);
        Assert.True(score1 == score2);
    }

    [Fact]
    public void Equals_DifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var score1 = new Score(150);
        var score2 = new Score(200);

        // Act & Assert
        Assert.NotEqual(score1, score2);
        Assert.True(score1 != score2);
    }
}
