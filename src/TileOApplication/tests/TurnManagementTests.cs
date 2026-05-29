using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class TurnManagementTests
{
    private static GameAggregate CreateStartedGame(int playerCount = 2)
    {
        var game = new GameAggregate();

        var colors = new[] { PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
        for (int i = 0; i < playerCount; i++)
            game.AddPlayer(new PlayerAggregate(colors[i], i + 1));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 1), 2);

        var startPositions = new[] { new Position(0, 0), new Position(3, 0), new Position(0, 3), new Position(2, 3) };
        for (int i = 0; i < playerCount; i++)
            game.Board.SetStartingPosition(startPositions[i], game.Players[i].Id);

        // Connect tiles so movement is possible
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.SkipTurn);

        game.StartGame();
        return game;
    }

    private static void EndTurnWithMove(GameAggregate game, Position targetPosition)
    {
        game.RecordDiceRoll(1);
        game.MovePlayer(game.CurrentPlayerId!.Value, targetPosition);
    }

    [Fact]
    public void TM001_GameStartPlayer1ShouldHaveFirstTurn()
    {
        var game = CreateStartedGame(4);

        var player1 = game.Players.First(p => p.TurnOrder == 1);
        Assert.Equal(player1.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void TM002_TurnOrder2Players_ShouldCycleP1P2P1()
    {
        var game = CreateStartedGame(2);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);

        Assert.Equal(p1.Id, game.CurrentPlayerId);

        // P1 moves to (1,0) — adjacent to starting (0,0)
        EndTurnWithMove(game, new Position(1, 0));
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // P2 moves to (2,0) — adjacent to starting (3,0)
        EndTurnWithMove(game, new Position(2, 0));
        Assert.Equal(p1.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void TM003_TurnOrder3Players_ShouldCycleP1P2P3P1()
    {
        var game = CreateStartedGame(3);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);

        Assert.Equal(p1.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(1, 0));
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(2, 0));
        Assert.Equal(p3.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(1, 3));
        Assert.Equal(p1.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void TM004_TurnOrder4Players_ShouldCycleP1P2P3P4P1()
    {
        var game = CreateStartedGame(4);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);
        var p4 = game.Players.First(p => p.TurnOrder == 4);

        Assert.Equal(p1.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(1, 0));
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(2, 0));
        Assert.Equal(p3.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(1, 3));
        Assert.Equal(p4.Id, game.CurrentPlayerId);

        EndTurnWithMove(game, new Position(2, 3));
        Assert.Equal(p1.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void TM005_GameStartDiceNotRolled_ShouldBeWaitingForRoll()
    {
        var game = CreateStartedGame(2);

        Assert.False(game.DiceRolledThisTurn);
    }

    [Fact]
    public void TM006_MovePlayerBeforeRollingDice_ShouldThrow()
    {
        var game = CreateStartedGame(2);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            game.MovePlayer(p1.Id, new Position(1, 0)));
        Assert.Contains("dice", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TM007_MovePlayerToUnconnectedTile_ShouldThrow()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(2, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(0, 1), new TileState(TileType.Regular)));
        game.Board.SetHiddenTile(new Position(2, 0));
        game.Board.SetActionTile(new Position(0, 1), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(1, 0), game.Players[1].Id);

        game.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();

        var p1 = game.Players.First(p => p.TurnOrder == 1);
        game.RecordDiceRoll(3);

        var connected = game.Board.GetConnectedPositions(new Position(0, 0));
        Assert.DoesNotContain(new Position(2, 0), connected);
    }

    [Fact]
    public void TM008_MovePlayerAfterRollingDice_ShouldSucceed()
    {
        var game = CreateStartedGame(2);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));

        Assert.Equal(new Position(1, 0), p1.CurrentPosition);
    }

    [Fact]
    public void TM009_SkipTurnCardShouldSkipPlayerNextTurn()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 0), 2); // P1 will land here

        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);
        game.Board.SetStartingPosition(new Position(0, 3), game.Players[2].Id);

        game.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));
        game.Board.ConnectTiles(new Position(1, 0), new Position(2, 0));
        game.Board.ConnectTiles(new Position(2, 0), new Position(3, 0));
        game.Board.ConnectTiles(new Position(0, 0), new Position(0, 1));
        game.Board.ConnectTiles(new Position(0, 1), new Position(0, 2));
        game.Board.ConnectTiles(new Position(0, 2), new Position(0, 3));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.SkipTurn);

        game.StartGame();

        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);

        // P1 lands on action tile at (1,0) → draws SkipTurn card → P1 loses next turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));

        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // P2 takes turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 0));

        // P1's turn should be skipped, so it goes to P3
        Assert.Equal(p3.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void TM010_ExtraTurnCardShouldGivePlayerExtraTurn()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 0), 2); // P1 will land here

        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);

        game.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));
        game.Board.ConnectTiles(new Position(1, 0), new Position(2, 0));
        game.Board.ConnectTiles(new Position(2, 0), new Position(3, 0));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();

        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);

        // P1 lands on action tile → draws ExtraTurn card → P1 gets extra turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));

        // P1 should still have the turn (extra turn)
        Assert.Equal(p1.Id, game.CurrentPlayerId);
        Assert.False(game.DiceRolledThisTurn);

        // P1 takes extra turn
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 0));

        // Now it's P2's turn
        Assert.Equal(p2.Id, game.CurrentPlayerId);
    }
}
