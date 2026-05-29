using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class WinConditionTests
{
    private static GameAggregate CreateStartedGame(int playerCount = 4)
    {
        var game = new GameAggregate();

        var colors = new[] { PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
        for (int i = 0; i < playerCount; i++)
            game.AddPlayer(new PlayerAggregate(colors[i], i + 1));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(2, 2));
        game.Board.SetActionTile(new Position(0, 2), 2);

        var startPositions = new[] { new Position(0, 0), new Position(3, 0), new Position(0, 3), new Position(3, 3) };
        for (int i = 0; i < playerCount; i++)
            game.Board.SetStartingPosition(startPositions[i], game.Players[i].Id);

        // Connect all tiles
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
    public void WC001_LandOnHiddenTile_ShouldEndGameAndDeclareWinner()
    {
        var game = CreateStartedGame(2);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        // Move P1 to (1,0)
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));

        // P2 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(game.Players.First(p => p.TurnOrder == 2).Id, new Position(2, 0));

        // P1 moves to (2,0)
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 0));

        // P2 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(game.Players.First(p => p.TurnOrder == 2).Id, new Position(2, 1));

        // P1 moves to (2,1)
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 1));

        // P2 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(game.Players.First(p => p.TurnOrder == 2).Id, new Position(2, 2));

        // P2 landed on hidden tile (2,2) — game should end
        Assert.Equal(GameStatus.Completed, game.Status);
        Assert.Equal(game.Players.First(p => p.TurnOrder == 2).Id, game.WinnerId);
    }

    [Fact]
    public void WC002_PassThroughHiddenTile_ShouldNotEndGame()
    {
        var game = CreateStartedGame(2);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);

        // P1 moves to (2,1)
        game.RecordDiceRoll(3);
        game.MovePlayer(p1.Id, new Position(2, 1));

        // P2 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(1, 0));

        // P1 moves from (2,1) to (2,3), passing through hidden tile (2,2)
        game.RecordDiceRoll(2);
        game.MovePlayer(p1.Id, new Position(2, 3));

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.WinnerId);
    }

    [Fact]
    public void WC003_AfterWin_OtherPlayersShouldNotBeAbleToTakeATurn()
    {
        var game = CreateStartedGame(4);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);

        // Move P1 to (1,0)
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));

        // P2 moves toward hidden tile (2,2)
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 0));

        // P3 takes a turn
        var p3 = game.Players.First(p => p.TurnOrder == 3);
        game.RecordDiceRoll(1);
        game.MovePlayer(p3.Id, new Position(1, 3));

        // P4 takes a turn
        var p4 = game.Players.First(p => p.TurnOrder == 4);
        game.RecordDiceRoll(1);
        game.MovePlayer(p4.Id, new Position(2, 3));

        // P1 moves to (2,0)
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 0));

        // P2 moves to (2,1)
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 1));

        // P3 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p3.Id, new Position(1, 3));

        // P4 takes a turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p4.Id, new Position(2, 3));

        // P1 moves to (2,1)
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 1));

        // P2 lands on hidden tile (2,2) — game ends immediately
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 2));

        Assert.Equal(GameStatus.Completed, game.Status);
        Assert.Equal(p2.Id, game.WinnerId);

        // P3 and P4 should not be able to take turns
        Assert.Throws<InvalidOperationException>(() => game.RecordDiceRoll(3));
    }
}
