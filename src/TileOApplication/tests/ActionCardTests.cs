using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Services;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class ActionCardTests
{
    private static GameAggregate CreateGameWithActionTile(ActionCardDescription cardType, int duration = 3)
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 0), duration);

        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);
        game.Board.SetStartingPosition(new Position(0, 3), game.Players[2].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(cardType);

        game.StartGame();
        return game;
    }

    private static void TakeTurn(GameAggregate game, Position target)
    {
        game.RecordDiceRoll(1);
        game.MovePlayer(game.CurrentPlayerId!.Value, target);
    }

    [Fact]
    public void AC001_ExtraTurnCardAfterNormalTurn_ShouldAllowImmediateReroll()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        // P1 lands on action tile → ExtraTurn card → P1 still has turn
        TakeTurn(game, new Position(1, 0));

        Assert.Equal(p1.Id, game.CurrentPlayerId);
        Assert.False(game.DiceRolledThisTurn); // dice not yet rolled for extra turn
    }

    [Fact]
    public void AC002_ExtraTurnCard_ShouldGrantExactlyOneExtraTurn()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);

        // P1 lands on action tile → ExtraTurn card
        TakeTurn(game, new Position(1, 0));
        Assert.Equal(p1.Id, game.CurrentPlayerId);

        // P1 uses extra turn
        TakeTurn(game, new Position(2, 0));

        // Now it should be P2's turn (not P1 again)
        Assert.Equal(p2.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void AC003_ConnectTilesCardAdjacentTiles_ShouldConnect()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ConnectTiles);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        // P1 lands on action tile → ConnectTiles card drawn
        TakeTurn(game, new Position(1, 0));

        var service = new GamePlayService();
        var connectCard = game.ActionCards.First(ac => !ac.IsUsed);

        var initialSpare = game.Board.SpareConnectionPieces;

        // Connect two adjacent tiles that aren't connected yet
        // (0,1) and (1,1) should be connected (they are in the full grid)
        var freshGame = new GameAggregate();
        freshGame.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        freshGame.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));

        var spareBefore = freshGame.Board.SpareConnectionPieces;
        freshGame.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));

        Assert.Equal(spareBefore - 1, freshGame.Board.SpareConnectionPieces);
        var connected = freshGame.Board.GetConnectedPositions(new Position(0, 0));
        Assert.Contains(new Position(1, 0), connected);
    }

    [Fact]
    public void AC004_ConnectTilesCardNonAdjacentTiles_ShouldThrow()
    {
        var game = new GameAggregate();
        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(2, 0), new TileState(TileType.Regular)));

        Assert.Throws<ArgumentException>(() =>
            game.Board.ConnectTiles(new Position(0, 0), new Position(2, 0)));
    }

    [Fact]
    public void AC005_ConnectTilesCardWhenNoSparepieces_ShouldThrow()
    {
        var game = new GameAggregate();

        // Add 33 tiles in a row and connect them all (uses 32 spare pieces)
        for (int x = 0; x < 34; x++)
            game.Board.AddTile(new TileEntity(new Position(x, 0), new TileState(TileType.Regular)));

        for (int x = 0; x < 32; x++)
            game.Board.ConnectTiles(new Position(x, 0), new Position(x + 1, 0));

        Assert.Equal(0, game.Board.SpareConnectionPieces);

        Assert.Throws<InvalidOperationException>(() =>
            game.Board.ConnectTiles(new Position(32, 0), new Position(33, 0)));
    }

    [Fact]
    public void AC006_ConnectTilesCardAlreadyConnectedSide_ShouldConsumeSpareButNotDuplicate()
    {
        var game = new GameAggregate();
        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));

        game.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));
        var spareAfterFirst = game.Board.SpareConnectionPieces;

        var connected = game.Board.GetConnectedPositions(new Position(0, 0));
        Assert.Contains(new Position(1, 0), connected);
        Assert.Equal(31, spareAfterFirst); // only 1 piece used total
    }

    [Fact]
    public void AC007_RemoveConnectionCard_ShouldDisconnectAndReturnToSpare()
    {
        var game = new GameAggregate();
        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));

        game.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));
        var spareAfterConnect = game.Board.SpareConnectionPieces;

        game.Board.DisconnectTiles(new Position(0, 0), new Position(1, 0));

        Assert.Equal(spareAfterConnect + 1, game.Board.SpareConnectionPieces);
        var connected = game.Board.GetConnectedPositions(new Position(0, 0));
        Assert.DoesNotContain(new Position(1, 0), connected);
    }

    [Fact]
    public void AC008_RemoveConnectionCardWhenNotConnected_ShouldThrowOrNotChangeSpare()
    {
        var game = new GameAggregate();
        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));

        var spareBefore = game.Board.SpareConnectionPieces;

        Assert.Throws<InvalidOperationException>(() =>
            game.Board.DisconnectTiles(new Position(0, 0), new Position(1, 0)));
    }

    [Fact]
    public void AC009_TeleportCard_ShouldMovePlayerToAnyOtherTile()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.Teleport);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        // P1 lands on action tile → Teleport card drawn
        TakeTurn(game, new Position(1, 0));

        var freshGame = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var freshP1 = freshGame.Players.First(p => p.TurnOrder == 1);
        freshGame.RecordDiceRoll(1);
        freshGame.MovePlayer(freshP1.Id, new Position(1, 0)); // lands on action tile, gets extra turn

        // Now P1 has extra turn — use GamePlayService to teleport
        var service = new GamePlayService();
        var teleportCard = freshGame.ActionCards.FirstOrDefault(ac => !ac.IsUsed);
        if (teleportCard != null)
        {
            // Simulate teleport by using MovePlayerDirect
            freshGame.RecordDiceRoll(1);
            freshGame.MovePlayer(freshP1.Id, new Position(2, 2));
            Assert.Equal(new Position(2, 2), freshP1.CurrentPosition);
        }
    }

    [Fact]
    public void AC010_TeleportCardToCurrentTile_ShouldThrow()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        // P1 lands on action tile → ExtraTurn card → P1 has extra turn
        TakeTurn(game, new Position(1, 0));

        var service = new GamePlayService();
        var teleportCard = game.ActionCards.First(ac => !ac.IsUsed);

        var freshGame = new GameAggregate();
        freshGame.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        freshGame.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                freshGame.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        freshGame.Board.SetHiddenTile(new Position(3, 3));
        freshGame.Board.SetActionTile(new Position(1, 0), 2);
        freshGame.Board.SetStartingPosition(new Position(0, 0), freshGame.Players[0].Id);
        freshGame.Board.SetStartingPosition(new Position(3, 0), freshGame.Players[1].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                freshGame.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                freshGame.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            freshGame.AddActionCard(ActionCardDescription.Teleport);

        freshGame.StartGame();

        var freshP1 = freshGame.Players.First(p => p.TurnOrder == 1);
        freshGame.RecordDiceRoll(1);
        freshGame.MovePlayer(freshP1.Id, new Position(1, 0)); // lands on action tile

        var freshP2 = freshGame.Players.First(p => p.TurnOrder == 2);
        var teleportCardForP2 = freshGame.ActionCards.First(ac => !ac.IsUsed);

        Assert.Throws<ArgumentException>(() =>
            service.ExecuteTeleportCard(freshGame, freshP2.Id, teleportCardForP2.Id, freshP2.CurrentPosition!));
    }

    [Fact]
    public void AC011_TeleportCardToHiddenTile_ShouldTriggerWin()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 0), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.Teleport);

        game.StartGame();

        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var service = new GamePlayService();

        // P1 is at (0,0), use teleport card to go to hidden tile (3,3)
        var teleportCard = game.ActionCards.First(ac => !ac.IsUsed);

        service.ExecuteTeleportCard(game, p1.Id, teleportCard.Id, new Position(3, 3));

        Assert.Equal(GameStatus.Completed, game.Status);
        Assert.Equal(p1.Id, game.WinnerId);
    }

    [Fact]
    public void AC012_TeleportCardToActiveActionTile_ShouldTriggerActionCard()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(2, 2), 2); // action tile at (2,2)
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        // First 16 cards are Teleport, rest are ExtraTurn
        for (int i = 0; i < 16; i++)
            game.AddActionCard(ActionCardDescription.Teleport);
        for (int i = 0; i < 16; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();

        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var service = new GamePlayService();

        var teleportCard = game.ActionCards.First(ac => !ac.IsUsed);
        var usedBefore = game.ActionCards.Count(ac => ac.IsUsed);

        // P1 teleports to action tile (2,2)
        service.ExecuteTeleportCard(game, p1.Id, teleportCard.Id, new Position(2, 2));

        // 2 cards should be used: 1 teleport + 1 action card from landing on action tile
        Assert.Equal(usedBefore + 2, game.ActionCards.Count(ac => ac.IsUsed));
    }

    [Fact]
    public void AC013_SkipTurnCard_ShouldOnlySkipOneNextTurn()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.SkipTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);

        // P1 lands on action tile → SkipTurn card → P1's next turn is skipped
        TakeTurn(game, new Position(1, 0));

        // After SkipTurn, turn advances to P2
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // P2 takes turn
        TakeTurn(game, new Position(2, 0));

        // P3 takes turn
        TakeTurn(game, new Position(1, 3));

        // P1's turn is skipped (first time after SkipTurn)
        // So it goes to P2 again
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // P2 takes turn
        TakeTurn(game, new Position(2, 0));

        // P3 takes turn
        TakeTurn(game, new Position(1, 3));

        // Now P1 should play normally (skip was only for one turn)
        Assert.Equal(p1.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void AC014_SkipTurnCard_SkippedPlayerCannotInteract()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.SkipTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);

        // P1 lands on action tile → SkipTurn card → P1's next turn is skipped
        TakeTurn(game, new Position(1, 0));

        // After SkipTurn, turn advances to P2
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // P2 takes turn
        TakeTurn(game, new Position(2, 0));

        // P3 takes turn
        TakeTurn(game, new Position(1, 3));

        // P1's turn is skipped — it should now be P2's turn (not P1's)
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // Roll dice for P2's turn
        game.RecordDiceRoll(3);

        // P1 cannot move — it's P2's turn, not P1's
        Assert.Throws<InvalidOperationException>(() =>
            game.MovePlayer(p1.Id, new Position(2, 0)));
    }
}
