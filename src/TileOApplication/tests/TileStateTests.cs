using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class TileStateTests
{
    private static GameAggregate CreateStartedGame()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(2, 2), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);

        // Connect tiles
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();
        return game;
    }

    [Fact]
    public void TS001_LandOnWhiteTile_ShouldChangeToBlack()
    {
        var game = CreateStartedGame();
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var targetPos = new Position(1, 0); // unvisited tile

        // Verify tile is not visited before landing
        var tileView = game.Board.GetTileView(targetPos);
        Assert.NotNull(tileView);
        Assert.Equal(TileDisplayType.Regular, tileView.DisplayType);

        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, targetPos);

        // After landing, tile should be visited (black)
        var tileViewAfter = game.Board.GetTileView(targetPos);
        Assert.NotNull(tileViewAfter);
        Assert.Equal(TileDisplayType.Visited, tileViewAfter.DisplayType);
    }

    [Fact]
    public void TS002_LandOnBlackTile_ShouldRemainBlack()
    {
        var game = CreateStartedGame();
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var targetPos = new Position(1, 0);

        // P1 lands on tile first
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, targetPos);

        // P2 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 0));

        // P1 moves back to the same tile
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));

        // Tile should still be visited (black)
        var tileView = game.Board.GetTileView(targetPos);
        Assert.NotNull(tileView);
        Assert.Equal(TileDisplayType.Visited, tileView.DisplayType);
    }

    [Fact]
    public void TS003_StartingPositionTile_ShouldBeBlackAfterGameStart()
    {
        var game = CreateStartedGame();

        // Starting positions are (0,0) for P1 and (3,0) for P2
        var p1StartPos = new Position(0, 0);
        var p2StartPos = new Position(3, 0);

        var p1TileView = game.Board.GetTileView(p1StartPos);
        var p2TileView = game.Board.GetTileView(p2StartPos);

        Assert.NotNull(p1TileView);
        Assert.Equal(TileDisplayType.Visited, p1TileView.DisplayType);

        Assert.NotNull(p2TileView);
        Assert.Equal(TileDisplayType.Visited, p2TileView.DisplayType);
    }
}
