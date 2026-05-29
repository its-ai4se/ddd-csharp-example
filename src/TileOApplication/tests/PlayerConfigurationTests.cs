using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class PlayerConfigurationTests
{
    private static GameAggregate CreateGameWithBoard()
    {
        var game = new GameAggregate();

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 1), 2);

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        return game;
    }

    private static void AddPlayersAndStartingPositions(GameAggregate game, int count)
    {
        var colors = new[] { PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
        var positions = new[] { new Position(0, 0), new Position(3, 0), new Position(0, 3), new Position(3, 2) };
        for (int i = 0; i < count; i++)
        {
            game.AddPlayer(new PlayerAggregate(colors[i], i + 1));
            game.Board.SetStartingPosition(positions[i], game.Players[i].Id);
        }
    }

    [Fact]
    public void PC001_StartGameWith2Players_ShouldSucceed()
    {
        var game = CreateGameWithBoard();
        AddPlayersAndStartingPositions(game, 2);

        game.StartGame();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(2, game.Players.Count);
    }

    [Fact]
    public void PC002_StartGameWith4Players_ShouldSucceed()
    {
        var game = CreateGameWithBoard();
        AddPlayersAndStartingPositions(game, 4);

        game.StartGame();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(4, game.Players.Count);
    }

    [Fact]
    public void PC003_StartGameWith3Players_ShouldSucceed()
    {
        var game = CreateGameWithBoard();
        AddPlayersAndStartingPositions(game, 3);

        game.StartGame();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(3, game.Players.Count);
    }

    [Fact]
    public void PC004_StartGameWith1Player_ShouldThrow()
    {
        var game = CreateGameWithBoard();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);

        var ex = Assert.Throws<InvalidOperationException>(() => game.StartGame());
        Assert.Contains("2", ex.Message);
    }

    [Fact]
    public void PC005_StartGameWith0Players_ShouldThrow()
    {
        var game = CreateGameWithBoard();

        Assert.Throws<InvalidOperationException>(() => game.StartGame());
    }

    [Fact]
    public void PC006_AddPlayer_WhenAlready4Players_ShouldThrow()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Yellow, 4));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            game.AddPlayer(new PlayerAggregate(new PlayerColor("Purple"), 5)));
        Assert.Contains("4", ex.Message);
    }

    [Fact]
    public void PC007_Players_WithUniqueColors_ShouldAllBeAdded()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Yellow, 4));

        Assert.Equal(4, game.Players.Count);
        var colors = game.Players.Select(p => p.Color).ToList();
        Assert.Equal(colors.Count, colors.Distinct().Count());
    }

    [Fact]
    public void PC008_AddPlayer_WithDuplicateColor_ShouldThrow()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));

        Assert.Throws<ArgumentException>(() =>
            game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 2)));
    }
}
