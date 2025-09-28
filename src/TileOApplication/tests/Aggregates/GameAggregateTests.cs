using Xunit;
using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Tests.Aggregates;

public class GameAggregateTests
{
    [Fact]
    public void CreateGame_WithValidName_ShouldSucceed()
    {
        // Arrange & Act
        var game = new GameAggregate("Test Game");

        // Assert
        Assert.Equal("Test Game", game.Name);
        Assert.Equal(GameStatus.Designing, game.Status);
        Assert.Empty(game.Players);
        Assert.Empty(game.ActionCards);
    }

    [Fact]
    public void CreateGame_WithEmptyName_ShouldThrowException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new GameAggregate(""));
        Assert.Throws<ArgumentException>(() => new GameAggregate(null!));
    }

    [Fact]
    public void AddPlayer_WhenDesigning_ShouldSucceed()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var player = new PlayerAggregate("Player 1", PlayerColor.Red, 1);

        // Act
        game.AddPlayer(player);

        // Assert
        Assert.Single(game.Players);
        Assert.Equal(player.Id, game.Players.First().Id);
    }

    [Fact]
    public void AddPlayer_WhenNotDesigning_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Blue, 2);
        game.AddPlayer(player1);
        game.AddPlayer(player2);
        game.AddActionCard(ActionCardDescription.ExtraTurn);
        
        // Set up board with hidden tile and starting positions
        var tile1 = new TileEntity(new Position(0, 0), new TileState(TileType.Regular));
        var tile2 = new TileEntity(new Position(1, 0), new TileState(TileType.Hidden));
        game.Board.AddTile(tile1);
        game.Board.AddTile(tile2);
        game.Board.SetHiddenTile(new Position(1, 0));
        game.Board.SetStartingPosition(new Position(0, 0), player1.Id);
        game.Board.SetStartingPosition(new Position(1, 0), player2.Id);
        
        game.StartGame();

        var newPlayer = new PlayerAggregate("Player 3", PlayerColor.Green, 3);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => game.AddPlayer(newPlayer));
    }

    [Fact]
    public void AddPlayer_WithDuplicateColor_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Red, 2);

        // Act
        game.AddPlayer(player1);

        // Assert
        Assert.Throws<ArgumentException>(() => game.AddPlayer(player2));
    }

    [Fact]
    public void AddPlayer_MoreThanFour_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var colors = new[] { PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
        
        for (int i = 0; i < 4; i++)
        {
            game.AddPlayer(new PlayerAggregate($"Player {i + 1}", colors[i], i + 1));
        }

        var fifthPlayer = new PlayerAggregate("Player 5", new PlayerColor("Purple", "#800080"), 5);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => game.AddPlayer(fifthPlayer));
    }

    [Fact]
    public void StartGame_WithValidSetup_ShouldSucceed()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Blue, 2);
        
        game.AddPlayer(player1);
        game.AddPlayer(player2);
        game.AddActionCard(ActionCardDescription.ExtraTurn);
        
        // Set up board with hidden tile and starting positions
        var tile1 = new TileEntity(new Position(0, 0), new TileState(TileType.Regular));
        var tile2 = new TileEntity(new Position(1, 0), new TileState(TileType.Hidden));
        game.Board.AddTile(tile1);
        game.Board.AddTile(tile2);
        game.Board.SetHiddenTile(new Position(1, 0));
        game.Board.SetStartingPosition(new Position(0, 0), player1.Id);
        game.Board.SetStartingPosition(new Position(1, 0), player2.Id);

        // Act
        game.StartGame();

        // Assert
        Assert.Equal(GameStatus.ReadyToPlay, game.Status);
        Assert.Equal(player1.Id, game.CurrentPlayerId);
        Assert.Equal(1, game.CurrentTurn);
    }

    [Fact]
    public void StartGame_WithoutPlayers_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => game.StartGame());
    }

    [Fact]
    public void StartGame_WithoutActionCards_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var player = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        game.AddPlayer(player);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => game.StartGame());
    }
}
