using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.ValueObjects;

public class PositionTests
{
    [Fact]
    public void Constructor_WithValidCodeAndDescription_ShouldCreatePosition()
    {
        // Arrange
        var code = "GK";
        var description = "Goalkeeper";

        // Act
        var position = new Position(code, description);

        // Assert
        Assert.Equal("GK", position.Code);
        Assert.Equal(description, position.Description);
    }

    [Fact]
    public void Constructor_WithEmptyCode_ShouldThrowArgumentException()
    {
        // Arrange
        var code = "";
        var description = "Goalkeeper";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Position(code, description));
    }

    [Fact]
    public void Constructor_WithWhitespaceDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var code = "GK";
        var description = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Position(code, description));
    }

    [Fact]
    public void Equals_WithSameCode_ShouldReturnTrue()
    {
        // Arrange
        var position1 = new Position("GK", "Goalkeeper");
        var position2 = new Position("GK", "Goal Keeper");

        // Act & Assert
        Assert.Equal(position1, position2);
    }

    [Fact]
    public void Equals_WithDifferentCode_ShouldReturnFalse()
    {
        // Arrange
        var position1 = new Position("GK", "Goalkeeper");
        var position2 = new Position("CB", "Center Back");

        // Act & Assert
        Assert.NotEqual(position1, position2);
    }

    [Fact]
    public void StaticPositions_ShouldHaveCorrectValues()
    {
        // Act & Assert
        Assert.Equal("GK", Position.Goalkeeper.Code);
        Assert.Equal("LB", Position.LeftBack.Code);
        Assert.Equal("ST", Position.Striker.Code);
    }
}
