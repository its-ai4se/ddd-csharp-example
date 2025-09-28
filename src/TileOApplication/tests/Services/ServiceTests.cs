using Xunit;
using TileOApplication.Domain.Services;
using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Tests.Services;

public class GameDesignServiceTests
{
    [Fact]
    public void CreateDefaultActionCardDeck_ShouldCreate32Cards()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GameDesignService();

        // Act
        service.CreateDefaultActionCardDeck(game);

        // Assert
        Assert.Equal(32, game.ActionCards.Count);
    }

    [Fact]
    public void CreateDefaultActionCardDeck_WhenNotDesigning_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GameDesignService();
        
        // Add minimum required setup to start game
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Blue, 2);
        game.AddPlayer(player1);
        game.AddPlayer(player2);
        game.AddActionCard(ActionCardDescription.ExtraTurn);
        
        var tile1 = new TileEntity(new Position(0, 0), new TileState(TileType.Regular));
        var tile2 = new TileEntity(new Position(1, 0), new TileState(TileType.Hidden));
        game.Board.AddTile(tile1);
        game.Board.AddTile(tile2);
        game.Board.SetHiddenTile(new Position(1, 0));
        game.Board.SetStartingPosition(new Position(0, 0), player1.Id);
        game.Board.SetStartingPosition(new Position(1, 0), player2.Id);
        
        game.StartGame(); // Change status from Designing

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.CreateDefaultActionCardDeck(game));
    }

    [Fact]
    public void SetupDefaultPlayers_ShouldCreate4Players()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GameDesignService();

        // Act
        service.SetupDefaultPlayers(game);

        // Assert
        Assert.Equal(4, game.Players.Count);
        Assert.Contains(game.Players, p => p.Color.Equals(PlayerColor.Red));
        Assert.Contains(game.Players, p => p.Color.Equals(PlayerColor.Blue));
        Assert.Contains(game.Players, p => p.Color.Equals(PlayerColor.Green));
        Assert.Contains(game.Players, p => p.Color.Equals(PlayerColor.Yellow));
    }

    [Fact]
    public void SetupDefaultPlayers_WhenNotDesigning_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GameDesignService();
        
        // Add minimum required setup to start game
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Blue, 2);
        game.AddPlayer(player1);
        game.AddPlayer(player2);
        game.AddActionCard(ActionCardDescription.ExtraTurn);
        
        var tile1 = new TileEntity(new Position(0, 0), new TileState(TileType.Regular));
        var tile2 = new TileEntity(new Position(1, 0), new TileState(TileType.Hidden));
        game.Board.AddTile(tile1);
        game.Board.AddTile(tile2);
        game.Board.SetHiddenTile(new Position(1, 0));
        game.Board.SetStartingPosition(new Position(0, 0), player1.Id);
        game.Board.SetStartingPosition(new Position(1, 0), player2.Id);
        
        game.StartGame(); // Change status from Designing

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.SetupDefaultPlayers(game));
    }

    [Fact]
    public void CreateSampleBoard_ShouldCreate16Tiles()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GameDesignService();
        
        // Add players first so starting positions can be set
        service.SetupDefaultPlayers(game);

        // Act
        service.CreateSampleBoard(game);

        // Assert
        Assert.Equal(16, game.Board.Tiles.Count); // 4x4 board
        Assert.NotNull(game.Board.HiddenTilePosition);
        Assert.Equal(4, game.Board.StartingPositions.Count);
    }

    [Fact]
    public void CreateSampleBoard_WhenNotDesigning_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GameDesignService();
        
        // Add minimum required setup to start game
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Blue, 2);
        game.AddPlayer(player1);
        game.AddPlayer(player2);
        game.AddActionCard(ActionCardDescription.ExtraTurn);
        
        var tile1 = new TileEntity(new Position(0, 0), new TileState(TileType.Regular));
        var tile2 = new TileEntity(new Position(1, 0), new TileState(TileType.Hidden));
        game.Board.AddTile(tile1);
        game.Board.AddTile(tile2);
        game.Board.SetHiddenTile(new Position(1, 0));
        game.Board.SetStartingPosition(new Position(0, 0), player1.Id);
        game.Board.SetStartingPosition(new Position(1, 0), player2.Id);
        
        game.StartGame(); // Change status from Designing

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.CreateSampleBoard(game));
    }
}

public class GamePlayServiceTests
{
    [Fact]
    public void RollDie_ShouldReturnValueBetween1And6()
    {
        // Arrange
        var service = new GamePlayService();

        // Act
        var result = service.RollDie();

        // Assert
        Assert.True(result >= 1 && result <= 6);
    }

    [Fact]
    public void GetValidMoves_WhenGameNotInProgress_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GamePlayService();
        var playerId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.GetValidMoves(game, playerId, 3));
    }

    [Fact]
    public void ExecuteActionCard_WhenGameNotInProgress_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GamePlayService();
        var playerId = Guid.NewGuid();
        var actionCardId = Guid.NewGuid();
        var parameters = new Dictionary<string, object>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.ExecuteActionCard(game, playerId, actionCardId, parameters));
    }

    [Fact]
    public void ExecuteActionCard_WithMissingParameters_ShouldThrowException()
    {
        // Arrange
        var game = new GameAggregate("Test Game");
        var service = new GamePlayService();
        
        // Setup game to be in progress
        var player1 = new PlayerAggregate("Player 1", PlayerColor.Red, 1);
        var player2 = new PlayerAggregate("Player 2", PlayerColor.Blue, 2);
        game.AddPlayer(player1);
        game.AddPlayer(player2);
        game.AddActionCard(ActionCardDescription.ConnectTiles);
        
        var tile1 = new TileEntity(new Position(0, 0), new TileState(TileType.Regular));
        var tile2 = new TileEntity(new Position(1, 0), new TileState(TileType.Hidden));
        game.Board.AddTile(tile1);
        game.Board.AddTile(tile2);
        game.Board.SetHiddenTile(new Position(1, 0));
        game.Board.SetStartingPosition(new Position(0, 0), player1.Id);
        game.Board.SetStartingPosition(new Position(1, 0), player2.Id);
        
        game.StartGame();
        game.BeginPlay();
        
        var playerId = player1.Id;
        var actionCardId = game.ActionCards.First().Id;
        var parameters = new Dictionary<string, object>(); // Missing required parameters

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.ExecuteActionCard(game, playerId, actionCardId, parameters));
    }
}
