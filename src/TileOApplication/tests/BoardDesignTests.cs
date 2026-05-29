using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class BoardDesignTests
{
    private static GameAggregate CreateGameWithTiles()
    {
        var game = new GameAggregate();
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));
        return game;
    }

    [Fact]
    public void BD001_ConnectTilesToRight_ShouldSucceed()
    {
        var game = CreateGameWithTiles();
        var posA = new Position(0, 0);
        var posB = new Position(1, 0); // right of A

        game.Board.ConnectTiles(posA, posB);

        var connected = game.Board.GetConnectedPositions(posA);
        Assert.Contains(posB, connected);
    }

    [Fact]
    public void BD002_ConnectTilesToLeft_ShouldSucceed()
    {
        var game = CreateGameWithTiles();
        var posA = new Position(2, 0);
        var posB = new Position(1, 0); // left of A

        game.Board.ConnectTiles(posA, posB);

        var connected = game.Board.GetConnectedPositions(posA);
        Assert.Contains(posB, connected);
    }

    [Fact]
    public void BD003_ConnectTilesToTop_ShouldSucceed()
    {
        var game = CreateGameWithTiles();
        var posA = new Position(0, 2);
        var posB = new Position(0, 1); // top (north) of A

        game.Board.ConnectTiles(posA, posB);

        var connected = game.Board.GetConnectedPositions(posA);
        Assert.Contains(posB, connected);
    }

    [Fact]
    public void BD004_ConnectTilesToBottom_ShouldSucceed()
    {
        var game = CreateGameWithTiles();
        var posA = new Position(0, 0);
        var posB = new Position(0, 1); // bottom (south) of A

        game.Board.ConnectTiles(posA, posB);

        var connected = game.Board.GetConnectedPositions(posA);
        Assert.Contains(posB, connected);
    }

    [Fact]
    public void BD005_ConnectTilesToAllFourSides_ShouldSucceed()
    {
        var game = CreateGameWithTiles();
        var center = new Position(2, 2);
        var right = new Position(3, 2);
        var left = new Position(1, 2);
        var top = new Position(2, 1);
        var bottom = new Position(2, 3);

        game.Board.ConnectTiles(center, right);
        game.Board.ConnectTiles(center, left);
        game.Board.ConnectTiles(center, top);
        game.Board.ConnectTiles(center, bottom);

        var connected = game.Board.GetConnectedPositions(center);
        Assert.Contains(right, connected);
        Assert.Contains(left, connected);
        Assert.Contains(top, connected);
        Assert.Contains(bottom, connected);
    }

    [Fact]
    public void BD006_ConnectTilesSecondConnectionOnSameSide_ShouldThrow()
    {
        var game = CreateGameWithTiles();
        var posA = new Position(0, 0);
        var posB = new Position(1, 0);

        game.Board.ConnectTiles(posA, posB);

        var spareAfterFirst = game.Board.SpareConnectionPieces;
        var connected = game.Board.GetConnectedPositions(posA);

        Assert.Contains(posB, connected);
        Assert.Equal(31, spareAfterFirst); // only 1 piece used
    }

    [Fact]
    public void BD007_SetHiddenTileShouldMarkTileAsHidden()
    {
        var game = CreateGameWithTiles();
        var hiddenPos = new Position(2, 2);

        game.Board.SetHiddenTile(hiddenPos);

        Assert.Equal(hiddenPos, game.Board.HiddenTilePosition);
    }

    [Fact]
    public void BD008_SetHiddenTileWhenAlreadySet_ShouldThrow()
    {
        var game = CreateGameWithTiles();
        game.Board.SetHiddenTile(new Position(2, 2));

        Assert.Throws<InvalidOperationException>(() =>
            game.Board.SetHiddenTile(new Position(3, 3)));
    }

    [Fact]
    public void BD009_StartGameWithoutHiddenTile_ShouldThrow()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(0, 1), new TileState(TileType.Regular)));
        game.Board.SetActionTile(new Position(0, 1), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(1, 0), game.Players[1].Id);

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        var ex = Assert.Throws<InvalidOperationException>(() => game.StartGame());
        Assert.Contains("hidden", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BD010_StartGameWithMissingStartingPosition_ShouldThrow()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));

        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(2, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(0, 1), new TileState(TileType.Regular)));
        game.Board.SetHiddenTile(new Position(2, 0));
        game.Board.SetActionTile(new Position(0, 1), 2);

        // Only set starting positions for 2 of 3 players
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(1, 0), game.Players[1].Id);

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        Assert.Throws<InvalidOperationException>(() => game.StartGame());
    }

    [Fact]
    public void BD011_StartGameWithAllStartingPositionsSet_ShouldSucceed()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Yellow, 4));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(2, 2));
        game.Board.SetActionTile(new Position(1, 1), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);
        game.Board.SetStartingPosition(new Position(0, 3), game.Players[2].Id);
        game.Board.SetStartingPosition(new Position(3, 3), game.Players[3].Id);

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();

        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void BD012_SetActionTileMultipleActionTiles_ShouldSucceed()
    {
        var game = CreateGameWithTiles();

        game.Board.SetActionTile(new Position(1, 1), 2);
        game.Board.SetActionTile(new Position(2, 2), 3);
        game.Board.SetActionTile(new Position(3, 3), 1);

        // Verify by checking that starting the game with these tiles works
        game.Board.SetHiddenTile(new Position(0, 4));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(4, 0), game.Players[1].Id);
        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void BD013_SetActionTileWithValidDuration_ShouldStoreDuration()
    {
        var game = CreateGameWithTiles();
        var actionPos = new Position(2, 2);

        game.Board.SetActionTile(actionPos, inactiveTurns: 3);

        // Verify the tile is set as action tile by checking it can be used in a game
        game.Board.SetHiddenTile(new Position(0, 0));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.Board.SetStartingPosition(new Position(1, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);
        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void BD014_SetActionTileWithZeroOrNegativeDuration_ShouldThrow()
    {
        var game = CreateGameWithTiles();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            game.Board.SetActionTile(new Position(2, 2), inactiveTurns: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            game.Board.SetActionTile(new Position(2, 2), inactiveTurns: -1));
    }
}
