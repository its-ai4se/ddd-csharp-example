using Xunit;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Tests.ValueObjects;

public class PositionTests
{
    [Fact]
    public void CreatePosition_WithValidCoordinates_ShouldSucceed()
    {
        // Arrange & Act
        var position = new Position(5, 10);

        // Assert
        Assert.Equal(5, position.X);
        Assert.Equal(10, position.Y);
    }

    [Fact]
    public void PositionEquality_WithSameCoordinates_ShouldBeEqual()
    {
        // Arrange
        var position1 = new Position(3, 4);
        var position2 = new Position(3, 4);

        // Act & Assert
        Assert.Equal(position1, position2);
        Assert.True(position1 == position2);
        Assert.False(position1 != position2);
    }

    [Fact]
    public void PositionEquality_WithDifferentCoordinates_ShouldNotBeEqual()
    {
        // Arrange
        var position1 = new Position(3, 4);
        var position2 = new Position(3, 5);

        // Act & Assert
        Assert.NotEqual(position1, position2);
        Assert.False(position1 == position2);
        Assert.True(position1 != position2);
    }

    [Fact]
    public void PositionAddition_ShouldWorkCorrectly()
    {
        // Arrange
        var position1 = new Position(2, 3);
        var position2 = new Position(1, 2);

        // Act
        var result = position1 + position2;

        // Assert
        Assert.Equal(3, result.X);
        Assert.Equal(5, result.Y);
    }

    [Fact]
    public void PositionSubtraction_ShouldWorkCorrectly()
    {
        // Arrange
        var position1 = new Position(5, 7);
        var position2 = new Position(2, 3);

        // Act
        var result = position1 - position2;

        // Assert
        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
    }

    [Fact]
    public void PositionToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var position = new Position(10, 20);

        // Act
        var result = position.ToString();

        // Assert
        Assert.Equal("(10, 20)", result);
    }
}

public class PlayerColorTests
{
    [Fact]
    public void CreatePlayerColor_WithValidNameAndHex_ShouldSucceed()
    {
        // Arrange & Act
        var color = new PlayerColor("Red", "#FF0000");

        // Assert
        Assert.Equal("Red", color.Name);
        Assert.Equal("#FF0000", color.HexCode);
    }

    [Fact]
    public void CreatePlayerColor_WithEmptyName_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PlayerColor("", "#FF0000"));
        Assert.Throws<ArgumentException>(() => new PlayerColor(null!, "#FF0000"));
    }

    [Fact]
    public void CreatePlayerColor_WithInvalidHex_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PlayerColor("Red", "invalid"));
        Assert.Throws<ArgumentException>(() => new PlayerColor("Red", "#GG0000"));
        Assert.Throws<ArgumentException>(() => new PlayerColor("Red", "FF0000"));
    }

    [Fact]
    public void PlayerColorEquality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var color1 = new PlayerColor("Red", "#FF0000");
        var color2 = new PlayerColor("Red", "#FF0000");

        // Act & Assert
        Assert.Equal(color1, color2);
    }

    [Fact]
    public void PlayerColorEquality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var color1 = new PlayerColor("Red", "#FF0000");
        var color2 = new PlayerColor("Blue", "#0000FF");

        // Act & Assert
        Assert.NotEqual(color1, color2);
    }

    [Fact]
    public void PredefinedColors_ShouldBeValid()
    {
        // Act & Assert
        Assert.Equal("Red", PlayerColor.Red.Name);
        Assert.Equal("#FF0000", PlayerColor.Red.HexCode);
        
        Assert.Equal("Blue", PlayerColor.Blue.Name);
        Assert.Equal("#0000FF", PlayerColor.Blue.HexCode);
        
        Assert.Equal("Green", PlayerColor.Green.Name);
        Assert.Equal("#00FF00", PlayerColor.Green.HexCode);
        
        Assert.Equal("Yellow", PlayerColor.Yellow.Name);
        Assert.Equal("#FFFF00", PlayerColor.Yellow.HexCode);
    }
}

public class ActionCardDescriptionTests
{
    [Fact]
    public void CreateActionCardDescription_WithValidValues_ShouldSucceed()
    {
        // Arrange & Act
        var description = new ActionCardDescription(ActionCardType.ExtraTurn, "Roll the die for an extra turn");

        // Assert
        Assert.Equal(ActionCardType.ExtraTurn, description.Type);
        Assert.Equal("Roll the die for an extra turn", description.Description);
    }

    [Fact]
    public void CreateActionCardDescription_WithNullDescription_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ActionCardDescription(ActionCardType.ExtraTurn, null!));
    }

    [Fact]
    public void PredefinedActionCards_ShouldBeValid()
    {
        // Act & Assert
        Assert.Equal(ActionCardType.ExtraTurn, ActionCardDescription.ExtraTurn.Type);
        Assert.Equal(ActionCardType.ConnectTiles, ActionCardDescription.ConnectTiles.Type);
        Assert.Equal(ActionCardType.RemoveConnection, ActionCardDescription.RemoveConnection.Type);
        Assert.Equal(ActionCardType.Teleport, ActionCardDescription.Teleport.Type);
        Assert.Equal(ActionCardType.SkipTurn, ActionCardDescription.SkipTurn.Type);
    }
}
